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
    protected const float EngageMonster = 200f;
    protected const float OccurEvent = 200f;//원래 50
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
        //전체적인 포인트
        //0 ~ EngageMonster - 1 -> 몬스터 조우,
        //EngageMonster ~ EngageMonster + OccurEvent - 1 -> 이벤트 발생,
        //EngageMonster + OccurEvent ~ FullEventChange - 1 -> 다음 층으로 이동 이벤트
        int FullEventPoint = (int)EngageMonster + (int)OccurEvent + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint;
        int RandPoint = Random.Range(0, FullEventPoint);
        if(RandPoint >= 0 && RandPoint < EngageMonster)//전투시작
        {
            int RandResearchPoint = Random.Range(1, SearchNextFloorMaxPoint+1);
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandResearchPoint;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
            Debug.Log("EngageMonster");
        }
        else if(RandPoint >= EngageMonster && RandPoint < EngageMonster + OccurEvent)//랜덤 이벤트 발생
        {
            int RandResearchPoint = Random.Range(1, SearchNextFloorMaxPoint + 1);
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandResearchPoint;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.OtherEvent;
            Debug.Log("RandomEvent");
        }
        else if(RandPoint >= EngageMonster + OccurEvent && RandPoint < FullEventPoint)//다음 층 발견
        {
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.SelectAction;
            Debug.Log("ResearchNextFloor");
        }
        else
        {
            Debug.LogError("Error");
            //예외처리
        }
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();//갱신

        UIMgr.BG_UI.MoveBackGround();
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
                break;
            case (int)EPlayerCurrentState.Battle:
                //MonMgr.SetSpawnPattern(PlayerMgr);//몬스터의 스폰 패턴 설정
                //MonMgr.SpawnCurrentSpawnPatternMonster();//스폰 패턴에 맞게 몬스터 생성//ActiveMonster설정
                BattleMgr.InitCurrentBattleMonsters();
                BattleMgr.InitMonsterNPlayerActiveGuage();
                BattleMgr.ProgressBattle();
                //ProgressBattle();
                //이때에 몬스터를 딱 스폰해야됨//여기에서 DetailOfEvent들어오면 그거에 맞게 몬스터 스폰하기
                break;
            case (int)EPlayerCurrentState.OtherEvent:
                EventMgr.SetCurrentEvent();//현재 발생할 이벤트 설정
                UIMgr.E_UI.ActiveEventUI(EventMgr);
                break;
            case (int)EPlayerCurrentState.Rest:
                //여기서는 즉각적으로 뭔가 결정할게 없긴함
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
        switch(Quality)//->퀄리티에 따라 피로도 소모 및 피로도 부족시에 안내 매세지 띄우기
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
        //휴식 상태를 저장함
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        StartCoroutine(CheckBackGroundMoveEnd(true));
    }
}
