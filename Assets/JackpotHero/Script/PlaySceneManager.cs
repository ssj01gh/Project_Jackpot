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
    public EventManager EventMgr;
    public RestManager RestMgr;
    // Start is called before the first frame update

    //���� �Ʒ����� �ٲ�� CurrentStageProgressUI���� �����ִ� ����� �ٲ����
    protected const float EngageMonster = 200f;
    protected const float OccurEvent = 200f;//���� 50
    protected const int SearchNextFloorMaxPoint = 10;
    void Start()
    {
        PlayerMgr.InitPlayerManager();
        UIMgr.SetUI();
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

    IEnumerator CheckBackGroundMoveEnd(bool IsIgnoreWhile = false)
    {
        new WaitForSeconds(0.1f);
        while(true)
        {
            yield return null;
            if(UIMgr.IsBGMoveEnd())//��׶��� �������� ������ while���� �����
            {
                break;
            }
            if(IsIgnoreWhile == true)
            {
                break;
            }
        }
        //���� ���¿� �°� UI ����
        UIMgr.SetUI();
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
                EventMgr.SetCurrentEvent(PlayerMgr.GetPlayerInfo());//���� �߻��� �̺�Ʈ ����
                UIMgr.E_UI.ActiveEventUI(EventMgr);
                break;
            case (int)EPlayerCurrentState.Rest:
                //���⼭�� �ﰢ������ ���� �����Ұ� ������
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
            PlayerMgr.GetPlayerInfo().EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
            int RewardEXP = PlayerMgr.GetPlayerInfo().ReturnEXPByEXPMagnification((int)MonMgr.CurrentSpawnPatternReward);
            PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RewardEXP, true);
            UIMgr.B_UI.VictoryBattle(RewardEXP);
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
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_Battle);
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
        UIMgr.GI_UI.ActiveGettingUI();
        UIMgr.SetUI();
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

        if(obj.tag == "Monster")
        {
            MonMgr.SetCurrentTargetMonster(obj.GetComponent<Monster>());
        }
    }
    //-----------------------------PressEventButton
    public void PressEventSelectionButton(int SelectionType)
    {
        EventMgr.OccurEventBySelection(SelectionType, PlayerMgr.GetPlayerInfo(), UIMgr);//���⼭ �̺�Ʈ�� ���ÿ� �°� �̺�Ʈ�� �߻���
        /*
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        PlayerMgr.GetPlayerInfo().EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
        *///������ 2���� ���¿� �°� ������ �̺�Ʈ���� �߻��ؾ� �ҵ� -> ��� ���, EXP�� ��°� �������� ��Ʋ�� �����Ѵٸ� ��������
        JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();//����
        StartCoroutine(CheckBackGroundMoveEnd(true));//���⿡ ���� ���� ���¿� �°� ui�� �����ǰ� �������� ��
        //UIMgr.SetUI(PlayerMgr);
    }
    //-------------------------------PressRestQualityButton
    public void PressRestQualityButton(int Quality)
    {
        switch(Quality)//->����Ƽ�� ���� �Ƿε� �Ҹ� �� �Ƿε� �����ÿ� �ȳ� �ż��� ����
        {
            case (int)EPlayerRestQuality.VeryBad:
                break;
            case (int)EPlayerRestQuality.Bad:
                if(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 100)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                break;
            case (int)EPlayerRestQuality.Good:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 250)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                break;
            case (int)EPlayerRestQuality.VeryGood:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 350)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                break;
            case (int)EPlayerRestQuality.Perfect:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 500)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                break;
        }

        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Rest;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = Quality;
        StartCoroutine(CheckBackGroundMoveEnd(true));
    }
    //-------------------------------PressRestTimeYesButton
    public void SetRestMgrRestResult()
    {
        RestMgr.SetRestResult(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails);
        UIMgr.R_UI.ActiveLeftTimeObject(RestMgr.IsPeacefulRest, RestMgr);
    }
    //----------------------------------SuddenAttackByMonsterInRestTIme
    public void SuddenAttackByMonsterInRest(int RestQuality)
    {
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = 0;
        PlayerMgr.GetPlayerInfo().SetIsSuddenAttackAndRestQuality(RestQuality);//�̰Ÿ� �ϸ� PlayerScript�� ���ݴ��� �������ٴ� ��ǰ� �޽��� ǰ���� �����
        //������ ������ ������ ������ EndOfAction���� ���¸� ��� ��ȭ ��ų�� ���ϸ� �ɵ�?
        //������ �Ͼ�� ����(���Ͱ� ���� �ѹ��� �����ϰԵ�)�ǰ��ϱ�

        StartCoroutine(CheckBackGroundMoveEnd(true));

        //������ ���� �Ŀ� CurrentPlayerAction�� CurrentPlayerActionDetail�� ���¸� ���� ���·� �ǵ����� 
        //SetUI()�ϴ°� �߿���
    }
}
