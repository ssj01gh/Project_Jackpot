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

    //여기 아래꺼가 바뀌면 CurrentStageProgressUI에서 쓰고있는 상수도 바꿔야함
    protected const float EngageMonster = 175f;//원래 175
    protected const float OccurEvent = 125f;//원래 125
    protected const int SearchNextFloorMaxPoint = 19;
    void Start()
    {
        PlayerMgr.InitPlayerManager();
        UIMgr.BG_UI.SetBackGroundSprite(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor);
        //UIMgr.SetUI();
        MonMgr.CurrentTargetChange += UIMgr.B_UI.DisplayMonsterDetailUI;
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        StartCoroutine(CheckBackGroundMoveEnd());
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.A))//도
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link01");
        }
        if (Input.GetKeyDown(KeyCode.S))//레
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link02");
        }
        if (Input.GetKeyDown(KeyCode.D))//미
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link03");
        }
        if (Input.GetKeyDown(KeyCode.F))//파
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link04");
        }
        if (Input.GetKeyDown(KeyCode.G))//솔
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link05");
        }
        if (Input.GetKeyDown(KeyCode.H))//라
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link06");
        }
        if (Input.GetKeyDown(KeyCode.J))//시
        {
            SoundManager.Instance.PlaySFX("CardOpen_Link07");
        }
        if (Input.GetKeyDown(KeyCode.K))//도
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
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Walk);
        //전체적인 포인트
        //0 ~ EngageMonster - 1 -> 몬스터 조우,
        //EngageMonster ~ EngageMonster + OccurEvent - 1 -> 이벤트 발생,
        //EngageMonster + OccurEvent ~ FullEventChange - 1 -> 다음 층으로 이동 이벤트
        //기댓값 11번째에 보스 이벤트를 만나게 1 ~ 19 사이로 오른다면 -> 평균값 10
        //이벤트가 결정된 다음에 포인트가 오른다 -> 결정 평균 10번째에 100이 되야 하고, 11번째에 보스 이벤트가 발생해야함

        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint < 100)
        {
            int FullEventPoint = (int)EngageMonster + (int)OccurEvent;
            int RandPoint = Random.Range(0, FullEventPoint);//0~299

            if (RandPoint >= 0 && RandPoint < EngageMonster)//전투시작
            {
                //0~174 랜덤 값까지 걸리면 전투
                int RandResearchPoint = Random.Range(1, SearchNextFloorMaxPoint + 1);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandResearchPoint;
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
                Debug.Log("EngageMonster");
            }
            else if (RandPoint >= EngageMonster && RandPoint < EngageMonster + OccurEvent)//랜덤 이벤트 발생
            {
                //175 ~ 299 랜덤 값까지 걸리면 이벤트
                int RandResearchPoint = Random.Range(1, SearchNextFloorMaxPoint + 1);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandResearchPoint;
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.OtherEvent;
                Debug.Log("RandomEvent");
            }
            else
            {
                Debug.LogError("Error");
                //예외처리
            }
        }
        else
        {//100일상 일때
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint >= 9999)//이게 되면 보스 이벤트를 완료했다는 뜻
            {
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Boss_Battle;
            }
            else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint >= 100)//다음 층 발견
            {
                //다음층은 DetectNextFloorPoint가 100이 넘을때
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Boss_Event;
                Debug.Log("ResearchNextFloor_Evnet");
            }
            else
            {
                Debug.LogError("Error");
                //예외처리
            }
        }

        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        UIMgr.BG_UI.MoveBackGround(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor);
        StartCoroutine(CheckBackGroundMoveEnd());
    }

    IEnumerator CheckBackGroundMoveEnd(bool IsIgnoreWhile = false)
    {
        new WaitForSeconds(0.1f);
        while(true)
        {
            yield return null;
            if(UIMgr.BG_UI.IsMoveEnd)//백그라운드 움직임이 끝나면 while문을 벗어나기
            {
                break;
            }
            if(IsIgnoreWhile == true)
            {
                break;
            }
        }
        //현재 상태에 맞게 UI 조절
        UIMgr.SetUI();
        //여기서 현재 action에 맞게 행동
        switch (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction)
        {
            case (int)EPlayerCurrentState.SelectAction:
                PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle);
                break;
            case (int)EPlayerCurrentState.Battle:
                PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
                BattleMgr.InitCurrentBattleMonsters();
                //여기 아래에서 보스냐 아니냐에 따라 BGM이 달라져야 할듯?
                /*
                if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails >= 1000)//1000이 넘는건 보스 밖에 없음
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
                //이때에 몬스터를 딱 스폰해야됨//여기에서 DetailOfEvent들어오면 그거에 맞게 몬스터 스폰하기
                break;
            case (int)EPlayerCurrentState.OtherEvent:
                PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle);
                SoundManager.Instance.PlayBGM("BaseBGM");
                EventMgr.SetCurrentEvent();//현재 발생할 이벤트 설정
                UIMgr.E_UI.ActiveEventUI(EventMgr);
                break;
            case (int)EPlayerCurrentState.Rest:
                //여기서는 즉각적으로 뭔가 결정할게 없긴함
                break;
            case (int)EPlayerCurrentState.Boss_Event:
                SoundManager.Instance.PlayBGM("BaseBGM");
                PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle);
                EventMgr.SetCurrentEvent(true);
                UIMgr.E_UI.ActiveEventUI(EventMgr);
                break;
            case (int)EPlayerCurrentState.Boss_Battle:
                //SoundManager.Instance.PlayBGM("BossBattleBGM");
                PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
                BattleMgr.InitCurrentBattleMonsters(true);
                BattleMgr.InitMonsterNPlayerActiveGuage();
                BattleMgr.ProgressBattle();
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
        switch (Quality)//->퀄리티에 따라 피로도 소모 및 피로도 부족시에 안내 매세지 띄우기
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
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 450)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(450);
                }
                break;
            case (int)EPlayerRestQuality.Perfect:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 700)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_RestQuality);
                    return;
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(700);
                }
                break;
        }

        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Rest;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = Quality;
        //휴식 상태를 저장함
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        StartCoroutine(CheckBackGroundMoveEnd(true));
    }
}
