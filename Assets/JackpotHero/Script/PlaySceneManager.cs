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
    protected const float OccurEvent = 100f;//���� 50
    protected const int SearchNextFloorMaxPoint = 10;
    void Start()
    {
        PlayerMgr.InitPlayerManager();
        UIMgr.BG_UI.SetBackGroundSprite(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor);
        UIMgr.SetUI();
        MonMgr.CurrentTargetChange += UIMgr.B_UI.DisplayMonsterDetailUI;
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        StartCoroutine(CheckBackGroundMoveEnd());
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.A))//��
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link01");
        }
        if (Input.GetKeyDown(KeyCode.S))//��
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link02");
        }
        if (Input.GetKeyDown(KeyCode.D))//��
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link03");
        }
        if (Input.GetKeyDown(KeyCode.F))//��
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link04");
        }
        if (Input.GetKeyDown(KeyCode.G))//��
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link05");
        }
        if (Input.GetKeyDown(KeyCode.H))//��
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link06");
        }
        if (Input.GetKeyDown(KeyCode.J))//��
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link07");
        }
        if (Input.GetKeyDown(KeyCode.K))//��
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link08");
        }
        */
    }

    private void OnApplicationQuit()
    {
        MonMgr.CurrentTargetChange -= UIMgr.B_UI.DisplayMonsterDetailUI;
    }


    //-------------------------------IdlePhase
    public void ResearchButtonClick()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
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
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.BossBattle;
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
                BattleMgr.InitCurrentBattleMonsters();
                //���� �Ʒ����� ������ �ƴϳĿ� ���� BGM�� �޶����� �ҵ�?
                /*
                if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails >= 1000)//1000�� �Ѵ°� ���� �ۿ� ����
                {
                    SoundManager.Instance.PlayBGM("BossBattleBGM");
                }
                else
                {
                    SoundManager.Instance.PlayBGM("NormalBattleBGM");
                }
                */
                BattleMgr.InitMonsterNPlayerActiveGuage();
                BattleMgr.ProgressBattle();
                //�̶��� ���͸� �� �����ؾߵ�//���⿡�� DetailOfEvent������ �װſ� �°� ���� �����ϱ�
                break;
            case (int)EPlayerCurrentState.OtherEvent:
                SoundManager.Instance.PlayBGM("BaseBGM");
                EventMgr.SetCurrentEvent();//���� �߻��� �̺�Ʈ ����
                UIMgr.E_UI.ActiveEventUI(EventMgr);
                break;
            case (int)EPlayerCurrentState.Rest:
                //���⼭�� �ﰢ������ ���� �����Ұ� ������
                break;
            case (int)EPlayerCurrentState.BossBattle:
                SoundManager.Instance.PlayBGM("BossBattleBGM");
                EventMgr.SetCurrentEvent(true);
                UIMgr.E_UI.ActiveEventUI(EventMgr);
                break;
        }
        yield break;
    }

    public void PressRestButton()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.RestButtonClick();
    }

    public void PressRestQuitButton()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.NotRestButtonClick();
    }
    public void PressRestQualityButton(int Quality)
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        switch (Quality)//->����Ƽ�� ���� �Ƿε� �Ҹ� �� �Ƿε� �����ÿ� �ȳ� �ż��� ����
        {
            case (int)EPlayerRestQuality.VeryBad:
                break;
            case (int)EPlayerRestQuality.Bad:
                if(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 100)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(100);
                }
                break;
            case (int)EPlayerRestQuality.Good:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 250)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(250);
                }
                break;
            case (int)EPlayerRestQuality.VeryGood:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 350)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(350);
                }
                break;
            case (int)EPlayerRestQuality.Perfect:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 500)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(500);
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
