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

    //여기 아래꺼가 바뀌면 CurrentStageProgressUI에서 쓰고있는 상수도 바꿔야함
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
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)PlayerCurrentState.Battle;
            Debug.Log("EngageMonster");
        }
        else if(RandPoint >= EngageMonster && RandPoint < EngageMonster + OccurEvent)//랜덤 이벤트 발생
        {
            int RandResearchPoint = Random.Range(1, SearchNextFloorMaxPoint + 1);
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandResearchPoint;
            //PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)PlayerCurrentState.OtherEvent;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)PlayerCurrentState.SelectAction;
            Debug.Log("RandomEvent");
        }
        else if(RandPoint >= EngageMonster + OccurEvent && RandPoint < FullEventPoint)//다음 층 발견
        {
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)PlayerCurrentState.SelectAction;
            Debug.Log("ResearchNextFloor");
        }
        else
        {
            Debug.LogError("Error");
            //예외처리
        }
        JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();//갱신

        UIMgr.ForResearchButtonClick();
        StartCoroutine(CheckBackGroundMoveEnd());
    }

    IEnumerator CheckBackGroundMoveEnd()
    {
        new WaitForSeconds(0.1f);
        while(true)
        {
            yield return null;
            if(UIMgr.IsBGMoveEnd())//백그라운드 움직임이 끝나면 while문을 벗어나기
            {
                break;
            }
        }
        //현재 상태에 맞게 UI 조절
        UIMgr.SetUI(PlayerMgr, MonMgr);
        //여기서 현재 action에 맞게 행동
        switch (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction)
        {
            case (int)PlayerCurrentState.SelectAction:
                break;
            case (int)PlayerCurrentState.Battle:
                MonMgr.SetSpawnPattern(PlayerMgr);//몬스터의 스폰 패턴 설정
                MonMgr.SpawnCurrentSpawnPatternMonster();//스폰 패턴에 맞게 몬스터 생성
                ProgressBattle();
                //이때에 몬스터를 딱 스폰해야됨//여기에서 DetailOfEvent들어오면 그거에 맞게 몬스터 스폰하기
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
    protected void ProgressBattle()//이게 전투가 계속되는 동안 지속적으로 불러와져야될 함수임
    {
        BattleMgr.SetBattleTurn(PlayerMgr, MonMgr);//플레이어와 몬스터의 SPD값에 영향을 받은 Turn을 결정
        UIMgr.B_UI.SetBattleTurnUI(BattleMgr.GetBattleTurn());//결정된 Turn을 ui에 표시
        UIMgr.B_UI.SetBattleShieldNBuffUI(PlayerMgr.GetPlayerInfo(), MonMgr.GetActiveMonsters());

        BattleMgr.DecideCurrentBattleTurn();//여기서 현재 누구의 차례인지 결정

        if (BattleMgr.CurrentState == (int)EBattleStates.PlayerTurn)
        {
            //플레이어의 턴이라면 플레이어의 행동을 결정하는 버튼을 나타나게 한다.
            UIMgr.B_UI.ActiveBattleSelectionUI();//행동 결정 버튼이 나타남
        }
        else if (BattleMgr.CurrentState == (int)EBattleStates.MonsterTurn)
        {
            /*
            MonMgr.SetCurrentTargetMonster(BattleMgr.CurrentTurnObject.GetComponent<Monster>());
            BattleButtonClick("Monster");
            */
        }
    }

    //플레이어 Battle 행동 선택에 쓰이는 함수임
    public void PlayerBattleActionSelectionButtonClick(string ActionButtonType)//누른 버튼의 행동에 따라 달라짐
    {
        //공격의 경우 공격 상대가 정해져 있어야함
        BattleMgr.CalculatePlayerAction(ActionButtonType);
        //이 함수를 실행시키면 현재 BattleMgr에는 플레이어 행동에 따른 결과값이 저장됨 -> 이걸 BattleUI가 잘 전달 받아서 작동하면 될듯?
        //이제 결정된 수치를 바탕으로 MainBattle을 활성화 시킴
    }
    public void PressTurnUIImage(int ButtonNum)//순서 칸에서 몬스터 버튼 눌렀을때 사용
    {
        GameObject obj = BattleMgr.GetBattleTurn().ElementAt(ButtonNum);
        if(obj.tag == "Monster")
        {
            MonMgr.SetCurrentTargetMonster(obj.GetComponent<Monster>());
        }
    }

}
