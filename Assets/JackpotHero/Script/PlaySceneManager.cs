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
        UIMgr.SetUI(PlayerMgr, MonMgr);
        MonMgr.CurrentTargetChange += UIMgr.B_UI.DisplayMonsterDetailUI;
        JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
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
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)PlayerCurrentState.Battle;
            Debug.Log("EngageMonster");
        }
        else if(RandPoint >= EngageMonster && RandPoint < EngageMonster + OccurEvent)//���� �̺�Ʈ �߻�
        {
            int RandResearchPoint = Random.Range(1, SearchNextFloorMaxPoint + 1);
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandResearchPoint;
            //PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)PlayerCurrentState.OtherEvent;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)PlayerCurrentState.SelectAction;
            Debug.Log("RandomEvent");
        }
        else if(RandPoint >= EngageMonster + OccurEvent && RandPoint < FullEventPoint)//���� �� �߰�
        {
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)PlayerCurrentState.SelectAction;
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
        UIMgr.SetUI(PlayerMgr, MonMgr);
        //���⼭ ���� action�� �°� �ൿ
        switch (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction)
        {
            case (int)PlayerCurrentState.SelectAction:
                break;
            case (int)PlayerCurrentState.Battle:
                MonMgr.SetSpawnPattern(PlayerMgr);//������ ���� ���� ����
                MonMgr.SpawnCurrentSpawnPatternMonster();//���� ���Ͽ� �°� ���� ����
                ProgressBattle();
                //�̶��� ���͸� �� �����ؾߵ�//���⿡�� DetailOfEvent������ �װſ� �°� ���� �����ϱ�
                break;
            case (int)PlayerCurrentState.OtherEvent:
                //EventUI.SetActive(true);
                break;
            case (int)PlayerCurrentState.Rest:
                //RestUI.SetActive(true);
                break;
        }
        yield break;
    }

    //-------------------------BattlePhase
    protected void ProgressBattle()//�̰� ������ ��ӵǴ� ���� ���������� �ҷ������ߵ� �Լ���
    {
        BattleMgr.SetBattleTurn(PlayerMgr, MonMgr);//�÷��̾�� ������ SPD���� ������ ���� Turn�� ����
        UIMgr.B_UI.SetBattleTurnUI(BattleMgr.GetBattleTurn());//������ Turn�� ui�� ǥ��
        UIMgr.B_UI.SetBattleShieldNBuffUI(PlayerMgr.GetPlayerInfo(), MonMgr.GetActiveMonsters());

        BattleMgr.DecideCurrentBattleTurn();//���⼭ ���� ������ �������� ����

        if (BattleMgr.CurrentState == (int)EBattleStates.PlayerTurn)
        {
            //�÷��̾��� ���̶�� �÷��̾��� �ൿ�� �����ϴ� ��ư�� ��Ÿ���� �Ѵ�.
            UIMgr.B_UI.ActiveBattleSelectionUI();//�ൿ ���� ��ư�� ��Ÿ��
        }
        else if (BattleMgr.CurrentState == (int)EBattleStates.MonsterTurn)
        {
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
        BattleMgr.CalculatePlayerAction(ActionButtonType);
        //�� �Լ��� �����Ű�� ���� BattleMgr���� �÷��̾� �ൿ�� ���� ������� ����� -> �̰� BattleUI�� �� ���� �޾Ƽ� �۵��ϸ� �ɵ�?
        //���� ������ ��ġ�� �������� MainBattle�� Ȱ��ȭ ��Ŵ
    }
    public void PressTurnUIImage(int ButtonNum)//���� ĭ���� ���� ��ư �������� ���
    {
        GameObject obj = BattleMgr.GetBattleTurn().ElementAt(ButtonNum);
        if(obj.tag == "Monster")
        {
            MonMgr.SetCurrentTargetMonster(obj.GetComponent<Monster>());
        }
    }

}
