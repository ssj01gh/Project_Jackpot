using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();//����

        UIMgr.BG_UI.MoveBackGround();
        StartCoroutine(CheckBackGroundMoveEnd());
    }

    IEnumerator CheckBackGroundMoveEnd(bool IsIgnoreWhile = false)
    {
        new WaitForSeconds(0.1f);
        while(true)
        {
            yield return null;
            if(UIMgr.BG_UI.IsMoveEnd)//��׶��� �������� ������ while���� �����
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
                //MonMgr.SetSpawnPattern(PlayerMgr);//������ ���� ���� ����
                //MonMgr.SpawnCurrentSpawnPatternMonster();//���� ���Ͽ� �°� ���� ����//ActiveMonster����
                BattleMgr.InitCurrentBattleMonsters();
                BattleMgr.InitMonsterNPlayerActiveGuage();
                BattleMgr.ProgressBattle();
                //ProgressBattle();
                //�̶��� ���͸� �� �����ؾߵ�//���⿡�� DetailOfEvent������ �װſ� �°� ���� �����ϱ�
                break;
            case (int)EPlayerCurrentState.OtherEvent:
                EventMgr.SetCurrentEvent();//���� �߻��� �̺�Ʈ ����
                UIMgr.E_UI.ActiveEventUI(EventMgr);
                break;
            case (int)EPlayerCurrentState.Rest:
                //���⼭�� �ﰢ������ ���� �����Ұ� ������
                break;
        }
        yield break;
    }

    public void PressRestButton()
    {
        UIMgr.RestButtonClick();
    }

    public void PressRestQuitButton()
    {
        UIMgr.NotRestButtonClick();
    }
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
        //�޽� ���¸� ������
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        StartCoroutine(CheckBackGroundMoveEnd(true));
    }
}
