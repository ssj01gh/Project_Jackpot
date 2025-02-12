using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaySceneManager : MonoBehaviour
{
    public PlaySceneUIManager UIMgr;
    public PlayerManager PlayerMgr;
    public MonsterManager MonMgr;
    public BattleManager BattleMgr;
    // Start is called before the first frame update

    //���� �Ʒ����� �ٲ�� CurrentStageProgressUI���� �����ִ� ����� �ٲ����
    protected const float EngageMonster = 200f;
    protected const float OccurEvent = 50f;
    protected const int SearchNextFloorMaxPoint = 10;
    void Start()
    {
        PlayerMgr.InitPlayerManager();
        UIMgr.SetUI(PlayerMgr);
        MonMgr.CurrentTargetChange += UIMgr.B_UI.DisplayMonsterDetailUI;
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        StartCoroutine(CheckBackGroundMoveEnd());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        MonMgr.CurrentTargetChange -= UIMgr.B_UI.DisplayMonsterDetailUI;
    }


    //-------------------------------IdlePhase
    public void ResearchButtonClick()
    {
        //��ü���� ����Ʈ
        //0 ~ EngageMonster - 1 -> ���� ����,
        //EngageMonster ~ EngageMonster + OccurEvent - 1 -> �̺�Ʈ �߻�,
        //EngageMonster + OccurEvent ~ FullEventChange - 1 -> ���� ������ �̵� �̺�Ʈ
        int FullEventPoint = (int)EngageMonster + (int)OccurEvent + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint;
        int RandPoint = Random.Range(0, FullEventPoint);
        if(RandPoint >= 0 && RandPoint < EngageMonster)//��������
        {
            int RandResearchPoint = Random.Range(1, SearchNextFloorMaxPoint+1);
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandResearchPoint;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
            Debug.Log("EngageMonster");
        }
        else if(RandPoint >= EngageMonster && RandPoint < EngageMonster + OccurEvent)//���� �̺�Ʈ �߻�
        {
            int RandResearchPoint = Random.Range(1, SearchNextFloorMaxPoint + 1);
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandResearchPoint;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.OtherEvent;
            Debug.Log("RandomEvent");
        }
        else if(RandPoint >= EngageMonster + OccurEvent && RandPoint < FullEventPoint)//���� �� �߰�
        {
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.SelectAction;
            Debug.Log("ResearchNextFloor");
        }
        else
        {
            Debug.LogError("Error");
            //����ó��
        }
        JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();//����

        UIMgr.ForResearchButtonClick();
        StartCoroutine(CheckBackGroundMoveEnd());
    }

    IEnumerator CheckBackGroundMoveEnd()
    {
        new WaitForSeconds(0.1f);
        while(true)
        {
            yield return null;
            if(UIMgr.IsBGMoveEnd())//��׶��� �������� ������ while���� �����
            {
                break;
            }
        }
        //���� ���¿� �°� UI ����
        UIMgr.SetUI(PlayerMgr);
        //���⼭ ���� action�� �°� �ൿ
        switch (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction)
        {
            case (int)EPlayerCurrentState.SelectAction:
                break;
            case (int)EPlayerCurrentState.Battle:
                MonMgr.SetSpawnPattern(PlayerMgr);//������ ���� ���� ����
                MonMgr.SpawnCurrentSpawnPatternMonster();//���� ���Ͽ� �°� ���� ����//ActiveMonster����
                BattleMgr.InitMonsterNPlayerActiveGuage(MonMgr.GetActiveMonsters().Count, MonMgr);
                ProgressBattle();
                //�̶��� ���͸� �� �����ؾߵ�//���⿡�� DetailOfEvent������ �װſ� �°� ���� �����ϱ�
                break;
            case (int)EPlayerCurrentState.OtherEvent:
                //EventUI.SetActive(true);
                break;
            case (int)EPlayerCurrentState.Rest:
                //RestUI.SetActive(true);
                break;
        }
        yield break;
    }

    //-------------------------BattlePhase
    protected void ProgressBattle()//�̰� ������ ��ӵǴ� ���� ���������� �ҷ������ߵ� �Լ���
    {
        MonMgr.CheckActiveMonstersRSurvive();
        BattleMgr.SetBattleTurn(PlayerMgr, MonMgr);//�÷��̾�� ������ SPD���� ������ ���� Turn�� ����->�Ƹ� ���⼭ Ȯ���ҵ�?
        BattleMgr.DecideCurrentBattleTurn();//���⼭ ���� ������ �������� ����
        UIMgr.B_UI.SetBattleTurnUI(BattleMgr.GetBattleTurn());//������ Turn�� ui�� ǥ��
        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//�÷��̾� ������ ����
        MonMgr.SetCurrentTargetMonster(null);//CurrentTarget�ʱ�ȭ
        UIMgr.B_UI.UnDisplayMonsterDetailUI();//������ �� ǥ�� UnActive;
        UIMgr.B_UI.SetBattleShieldNBuffUI(PlayerMgr.GetPlayerInfo(), MonMgr.GetActiveMonsters());//�̰� ���� ���Ͱ� �׾����� �ƴ��� Ȯ���ؾߵ�

        if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP <= 0)
        {
            Debug.Log("Loser");
            //��������Ʈ�� �������� ��������Ʈ�� ����ȴٴ���?
            //ActiveMonster���δ� ����
            MonMgr.InActiveAllActiveMonster();
            UIMgr.B_UI.SetBattleShieldNBuffUI(PlayerMgr.GetPlayerInfo(), MonMgr.GetActiveMonsters());//�����ִ� Set~~�� �� ���ֱ� ����
            PlayerMgr.GetPlayerInfo().DefeatFromBattle();
            UIMgr.B_UI.DefeatBattle(PlayerMgr.GetPlayerInfo());
            UIMgr.PlayerDefeat();
            //�ٸ� �÷��̾� UI�� ������
            return;
        }
        if (MonMgr.GetActiveMonsters().Count <= 0)
        {
            Debug.Log("Winner");
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience += (int)MonMgr.CurrentSpawnPatternReward;
            UIMgr.B_UI.VictoryBattle((int)MonMgr.CurrentSpawnPatternReward);
            return;
            //�� ������ ������ ������ ������.
        }


        if (BattleMgr.CurrentState == (int)EBattleStates.PlayerTurn)
        {
            //�÷��̾��� ���̶�� �÷��̾��� �ൿ�� �����ϴ� ��ư�� ��Ÿ���� �Ѵ�.
            UIMgr.B_UI.ActiveBattleSelectionUI();//�ൿ ���� ��ư�� ��Ÿ��
        }
        else if (BattleMgr.CurrentState == (int)EBattleStates.MonsterTurn)
        {
            Debug.Log("MonsterTurn");
            //������ ���̶�� �ൿ ��ư�� ��Ÿ��
            UIMgr.B_UI.ActiveBattleSelectionUI_Mon();
            /*
            MonMgr.SetCurrentTargetMonster(BattleMgr.CurrentTurnObject.GetComponent<Monster>());
            BattleButtonClick("Monster");
            */
        }
    }

    //�÷��̾� Battle �ൿ ���ÿ� ���̴� �Լ���
    public void PlayerBattleActionSelectionButtonClick(string ActionButtonType)//���� ��ư�� �ൿ�� ���� �޶���
    {
        //������ ��� ���� ��밡 ������ �־����
        if(ActionButtonType == "Attack" && MonMgr.CurrentTarget == null)
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.AttackGuideMessage);
            return;
        }
        if(PlayerMgr.GetPlayerInfo().SpendSTA(ActionButtonType) == false)//�Ƿε��� �����ϸ�
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage);
            return;
        }
        //�ൿ�ϱ����� �����ؾ� �Ұ͵��� ���⼭ �ҵ�?
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount = 0;
        BattleMgr.CalculatePlayerAction(ActionButtonType);

        if(ActionButtonType == "Attack")//�����̶�� BattleMgr���� ���� ����� ���� Ÿ���� ü���� ����
        {
            PlayerMgr.GetPlayerInfo().PlayerRecordGiveDamage(BattleMgr.BattleResultStatus.FinalResultAmount);
            MonMgr.CurrentTarget.MonsterDamage(BattleMgr.BattleResultStatus.FinalResultAmount);
        }
        else if(ActionButtonType == "Defense")//����� ����� �÷��̾��� ��� ��ġ�� ����
        {
            PlayerMgr.GetPlayerInfo().PlayerGetShield(BattleMgr.BattleResultStatus.FinalResultAmount);
        }
        else if(ActionButtonType == "Rest")//�޽��̶�� ����� �÷��̾��� �Ƿε��� ȸ����
        {
            PlayerMgr.GetPlayerInfo().PlayerGetRest(BattleMgr.BattleResultStatus.FinalResultAmount);
        }
        //�� �Լ��� �����Ű�� ���� BattleMgr���� �÷��̾� �ൿ�� ���� ������� ����� -> �̰� BattleUI�� �� ���� �޾Ƽ� �۵��ϸ� �ɵ�?
        //���� ������ ��ġ�� �������� MainBattle�� Ȱ��ȭ ��Ŵ
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentForm, MonMgr.CurrentTarget, ActionButtonType, BattleMgr.BattleResultStatus, BattleMgr.CurrentState, ProgressBattle);
    }

    public void MonsterBattleActionSelectionButtonClick()
    {
        //�ൿ�ϱ����� �����ؾ� �Ұ͵��� ���⼭ �ҵ�?
        BattleMgr.CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint = 0;
        BattleMgr.CalculateMonsterAction();
        string CurrentBattleState;
        switch(BattleMgr.CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState)
        {
            case (int)EMonsterActionState.Attack:
                //�������� �÷��̾ �������ִ� ���庸�� �۰ų� ���ٸ�
                CurrentBattleState = "Attack";
                PlayerMgr.GetPlayerInfo().PlayerDamage(BattleMgr.BattleResultStatus.FinalResultAmount);
                break;
            case (int)EMonsterActionState.Defense:
                CurrentBattleState = "Defense";
                BattleMgr.CurrentTurnObject.GetComponent<Monster>().MonsterGetShield(BattleMgr.BattleResultStatus.FinalResultAmount);
                break;
            default:
                CurrentBattleState = "Another";
                break;
        }

        BattleMgr.CurrentTurnObject.GetComponent<Monster>().SetNextMonsterState();
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentForm, BattleMgr.CurrentTurnObject.GetComponent<Monster>(), CurrentBattleState, BattleMgr.BattleResultStatus, BattleMgr.CurrentState, ProgressBattle);
    }
    public void PressVictoryButton()
    {
        UIMgr.B_UI.ClickVictoryButton();
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.SelectAction;
        UIMgr.SetUI(PlayerMgr);
        JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
    }

    public void PressDefeatButton()
    {
        JsonReadWriteManager.Instance.InitPlayerInfo(true);
        LoadingScene.Instance.LoadAnotherScene("TitleScene");
        //�ʱ�ȭ JsonManager�� P_Info �ʱ�ȭ
    }

    public void PressTurnUIImage(int ButtonNum)//���� ĭ���� ���� ��ư �������� ���
    {
        GameObject obj;
        obj = BattleMgr.GetBattleTurn()[ButtonNum];
        /*
        if (ButtonNum == 0)//�̰� �������� CurrentTurnObject�� obj�� ���ٴ°ǵ�(�����갡)
        {
            obj = BattleMgr.CurrentTurnObject;
        }
        else
        {
            obj = BattleMgr.GetBattleTurn()[ButtonNum - 1];
        }
        */
        if(obj.tag == "Monster")
        {
            MonMgr.SetCurrentTargetMonster(obj.GetComponent<Monster>());
        }
    }

}
