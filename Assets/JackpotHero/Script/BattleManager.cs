using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public enum EBattleStates
{
    Idle,
    PlayerTurn,
    MonsterTurn,
}

public class BattleResultStates
{
    public float BaseAmount;
    public float BaseAmountPlus;
    public List<float> ResultMagnification = new List<float>();
    public float FinalResultAmountPlus;
    public float FinalResultAmount;
}

public class BattleManager : MonoBehaviour
{
    protected List<GameObject> BattleTurn = new List<GameObject>();
    public PlayerManager PlayerMgr;
    public MonsterManager MonMgr;
    public PlaySceneUIManager UIMgr;
    // Start is called before the first frame update
    public GameObject CurrentTurnObject { protected set; get; }
    public BattleResultStates BattleResultStatus { protected set; get; } = new BattleResultStates();
    public int CurrentState { protected set; get; } = (int)EBattleStates.Idle;

    protected float PlayerActiveGauge = 0;
    //protected List<float> MonsterActiveGuage = new List<float>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.P))
        {
            foreach(GameObject obj in BattleTurn)
            {
                Debug.Log(obj.name);
            }
        }
        */
    }
    public List<GameObject> GetBattleTurn()
    {
        return BattleTurn;
    }

    public void InitCurrentBattleMonsters()
    {
        MonMgr.SetSpawnPattern(PlayerMgr);//몬스터의 스폰 패턴 설정
        MonMgr.SpawnCurrentSpawnPatternMonster();//스폰 패턴에 맞게 몬스터 생성//ActiveMonster설정
        //스폰 패턴이 저장된후 다시 저장
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    public void InitMonsterNPlayerActiveGuage()//초기화
    {
        BattleTurn.Clear();
        PlayerActiveGauge = 0;
        for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
        {
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge = 0;
        }

        if(PlayerMgr.GetPlayerInfo().IsSuddenAttackInRestTime == true)//습격을 당했다면
        {
            if (MonMgr.GetActiveMonsters().Count >= 1)
            {
                //SetBattleTurn에서 첫번째꺼를 지우기 때문에
                //ActiveMonster[0]을 한번 넣은 다음에 다시 넣어야함
                BattleTurn.Add(MonMgr.GetActiveMonsters()[0]);
            }
            for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                BattleTurn.Add(MonMgr.GetActiveMonsters()[i]);
            }
        }
    }

    public void ProgressBattle()//이게 전투가 계속되는 동안 지속적으로 불러와져야될 함수임
    {//CheckBackGroundMoveEnd 1개, BattleUI함수로 람다식 전달 2개
        MonMgr.CheckActiveMonstersRSurvive();//현재 스폰된 몬스터중 죽은 몬스터 정리
        SetBattleTurn();//플레이어와 몬스터의 SPD값에 영향을 받은 Turn을 결정->아마 여기서 확인할듯?
        DecideCurrentBattleTurn();//여기서 현재 누구의 차례인지 결정
        UIMgr.B_UI.SetBattleTurnUI(BattleTurn);//결정된 Turn을 ui에 표시
        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//플레이어 정보도 갱신
        MonMgr.SetCurrentTargetMonster(null);//CurrentTarget초기화
        UIMgr.B_UI.UnDisplayMonsterDetailUI();//몬스터의 상세 표시 UnActive//상세 스테이터스와 장비같은 것들;
        UIMgr.B_UI.SetBattleShieldNBuffUI(PlayerMgr.GetPlayerInfo(), MonMgr.GetActiveMonsters());//이거 전에 몬스터가 죽엇는지 아닌지 확인해야됨

        if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP <= 0)
        {
            //스프라이트가 쓰러지는 스프라이트가 재생된다던가?
            //ActiveMonster전부다 해제
            MonMgr.InActiveAllActiveMonster();
            UIMgr.B_UI.SetBattleShieldNBuffUI(PlayerMgr.GetPlayerInfo(), MonMgr.GetActiveMonsters());//여기있는 Set~~은 다 없애기 위함
            PlayerMgr.GetPlayerInfo().DefeatFromBattle();
            UIMgr.B_UI.DefeatBattle(PlayerMgr.GetPlayerInfo());
            UIMgr.PlayerDefeat();//플레이어의 장비창, 스탯창, 현재 스테이지 진행창 같은것들
            //다른 플레이어 UI도 없에기
            return;
        }
        if (MonMgr.GetActiveMonsters().Count <= 0)
        {
            Debug.Log("Winner");
            PlayerMgr.GetPlayerInfo().EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화
            //휴식중에 습격을 받은거라면 다시 휴식 행동 선택으로 돌아감
            int RewardEXP = PlayerMgr.GetPlayerInfo().ReturnEXPByEXPMagnification((int)MonMgr.CurrentSpawnPatternReward);
            PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RewardEXP, true);
            UIMgr.B_UI.VictoryBattle(RewardEXP);
            return;
            //다 죽으면 게임을 전투를 끝낸다.
        }


        if (CurrentState == (int)EBattleStates.PlayerTurn)
        {
            //플레이어의 턴이라면 플레이어의 행동을 결정하는 버튼을 나타나게 한다.
            UIMgr.B_UI.ActiveBattleSelectionUI();//행동 결정 버튼이 나타남
        }
        else if (CurrentState == (int)EBattleStates.MonsterTurn)
        {
            //몬스터의 턴이라는 행동 버튼이 나타남
            UIMgr.B_UI.ActiveBattleSelectionUI_Mon();
            /*
            MonMgr.SetCurrentTargetMonster(BattleMgr.CurrentTurnObject.GetComponent<Monster>());
            BattleButtonClick("Monster");
            */
        }
    }

    //
    public void PlayerBattleActionSelectionButtonClick(string ActionButtonType)//누른 버튼의 행동에 따라 달라짐
    {
        //공격의 경우 공격 상대가 정해져 있어야함
        if (ActionButtonType == "Attack" && MonMgr.CurrentTarget == null)
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.AttackGuideMessage);
            return;
        }
        if (PlayerMgr.GetPlayerInfo().SpendSTA(ActionButtonType) == false)//피로도가 부족하면
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_Battle);
            return;
        }
        //행동하기전에 감소해야 할것들은 여기서 할듯?
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount = 0;
        SetPlayerBattleStatus(ActionButtonType);

        if (ActionButtonType == "Attack")//공격이라면 BattleMgr에서 나온 결과로 현재 타겟의 체력을 깍음
        {
            PlayerMgr.GetPlayerInfo().PlayerRecordGiveDamage(BattleResultStatus.FinalResultAmount);
            MonMgr.CurrentTarget.MonsterDamage(BattleResultStatus.FinalResultAmount);
        }
        else if (ActionButtonType == "Defense")//방어라면 결과로 플레이어의 방어 수치를 높임
        {
            PlayerMgr.GetPlayerInfo().PlayerGetShield(BattleResultStatus.FinalResultAmount);
        }
        else if (ActionButtonType == "Rest")//휴식이라면 결과로 플레이어의 피로도를 회복함
        {
            PlayerMgr.GetPlayerInfo().PlayerGetRest(BattleResultStatus.FinalResultAmount);
        }
        //이 함수를 실행시키면 현재 BattleMgr에는 플레이어 행동에 따른 결과값이 저장됨 -> 이걸 BattleUI가 잘 전달 받아서 작동하면 될듯?
        //이제 결정된 수치를 바탕으로 MainBattle을 활성화 시킴
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentForm, MonMgr.CurrentTarget, ActionButtonType, BattleResultStatus, CurrentState, ProgressBattle);
    }

    public void MonsterBattleActionSelectionButtonClick()
    {
        //행동하기전에 감소해야 할것들은 여기서 할듯?
        CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint = 0;
        CalculateMonsterAction();
        string CurrentBattleState;
        switch (CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState)
        {
            case (int)EMonsterActionState.Attack:
                //데미지가 플레이어가 가지고있는 쉴드보다 작거나 같다면
                CurrentBattleState = "Attack";
                PlayerMgr.GetPlayerInfo().PlayerDamage(BattleResultStatus.FinalResultAmount);
                break;
            case (int)EMonsterActionState.Defense:
                CurrentBattleState = "Defense";
                CurrentTurnObject.GetComponent<Monster>().MonsterGetShield(BattleResultStatus.FinalResultAmount);
                break;
            default:
                CurrentBattleState = "Another";
                break;
        }

        CurrentTurnObject.GetComponent<Monster>().SetNextMonsterState();//몬스터의 행동 패턴에 따라 다음 행동 결정
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentForm, CurrentTurnObject.GetComponent<Monster>(), CurrentBattleState, BattleResultStatus, CurrentState, ProgressBattle);
    }
    public void PressVictoryButton()
    {
        UIMgr.B_UI.ClickVictoryButton();
        UIMgr.GI_UI.ActiveGettingUI();
        UIMgr.SetUI();
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
    }

    public void PressDefeatButton()
    {
        JsonReadWriteManager.Instance.InitPlayerInfo(true);
        LoadingScene.Instance.LoadAnotherScene("TitleScene");
        //초기화 JsonManager의 P_Info 초기화
    }

    public void PressTurnUIImage(int ButtonNum)//순서 칸에서 몬스터 버튼 눌렀을때 사용
    {
        GameObject obj;
        obj = GetBattleTurn()[ButtonNum];

        if (obj.tag == "Monster")
        {
            MonMgr.SetCurrentTargetMonster(obj.GetComponent<Monster>());
        }
    }
    //이거는 몬스터나 플레이어가 턴을 완료했을때 계속 불러와지는거니까 여기서 State를 초기화 한다?
    public void SetBattleTurn()
    {
        if (BattleTurn.Count >= 1)
            BattleTurn.RemoveAt(0);//첫번째꺼를 지운다.(왜냐하면 여기에 크기가 1이상인 BattleTurn이 들어왔다는것은 이미 0번째에 있는 오브젝트는 턴을 쓴것임

        for(int i = BattleTurn.Count - 1; i >= 0; i--)
        {
            if (BattleTurn[i].activeSelf == false)
            {
                Debug.Log("Empty");
                BattleTurn.RemoveAt(i);
            }
        }
        float PlayerSPD = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().TotalSPD;

        float[] MonsterSPD = new float[MonMgr.GetActiveMonsters().Count];
        for (int i = 0; i < MonsterSPD.Length; i++)
        {
            MonsterSPD[i] = MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentSPD;
        }

        while(BattleTurn.Count < 9)
        {
            PlayerActiveGauge += PlayerSPD;
            //ActiveMonster의 갯수를 줄여도 MonsterActiveGuage의 갯수는 그대로 유지가 된다.
            //만약 index1인 몬스터가 죽으면 원래는 index2였던 몬스터가 index1이 쓰던 MonsterActiveGuage를 이어받게 되는 문제가 생긴다.
            for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge += MonsterSPD[i];
            }


            if(PlayerActiveGauge >= 100)
            {
                PlayerActiveGauge -= 100;
                BattleTurn.Add(PlayerMgr.GetPlayerInfo().gameObject);
            }

            List<int> ChargedMonster = new List<int>();
            for(int i = 0; i < MonsterSPD.Length; i++)
            {
                if (MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge >= 100)
                {
                    ChargedMonster.Add(i);
                    //MonsterActiveGuage[i] -= 100;
                    //BattleTurn.Enqueue(MonMgr.GetActiveMonsters()[i]);
                }
            }

            while(ChargedMonster.Count > 0)
            {
                float FastestMonster = MonMgr.GetActiveMonsters()[ChargedMonster[0]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge;
                int ChargedMonsterIndex = 0;
                for(int i = 0; i < ChargedMonster.Count; i++)
                {
                    if(FastestMonster < MonMgr.GetActiveMonsters()[ChargedMonster[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                    {
                        FastestMonster = MonMgr.GetActiveMonsters()[ChargedMonster[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge;
                        ChargedMonsterIndex = i;
                    }
                }
                //여기로 나오면 FastestMonster가 결정된거임
                MonMgr.GetActiveMonsters()[ChargedMonster[ChargedMonsterIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge -= 100;
                BattleTurn.Add(MonMgr.GetActiveMonsters()[ChargedMonster[ChargedMonsterIndex]]);
                ChargedMonster.RemoveAt(ChargedMonsterIndex);
            }
        }
        CurrentState = (int)EBattleStates.Idle;
    }

    public void DecideCurrentBattleTurn()
    {
        if(CurrentState == (int)EBattleStates.Idle)
        {
            CurrentTurnObject = BattleTurn[0];
            //BattleTurn.RemoveAt(0);
            if (CurrentTurnObject.tag == "Player")
            {
                CurrentState = (int)EBattleStates.PlayerTurn;
            }
            else if (CurrentTurnObject.tag == "Monster")
            {
                CurrentState = (int)EBattleStates.MonsterTurn;
            }
        }
    }

    //CalculatePlayerAction
    protected void SetPlayerBattleStatus(string ActionButtonType)
    {
        PlayerInfo P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        TotalPlayerState TP_Info = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo();
        float AllEquipTier = PlayerMgr.GetPlayerInfo().GetAllEquipTier();

        int EquipCode = 0;
        switch (ActionButtonType)
        {
            case "Attack":
                EquipCode = P_Info.EquipWeaponCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalSTR;
                break;
            case "Defense":
                EquipCode = P_Info.EquipArmorCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalDUR;
                break;
            case "Rest":
                EquipCode = P_Info.EquipHatCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalRES;
                break;
        }
        EquipmentSO Equipment_Info = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(EquipCode);


        //기초 추가 수치 // 거의 유일하게 EQUIP 초반 강화 효과에 영향을 받는 수치임
        if (JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 7)//7레벨 이상일때만
        {
            //플레이어간 끼고 있는 장비들을 포함한 모든 장비들의 티어의 합
            BattleResultStatus.BaseAmountPlus = AllEquipTier;
        }
        else
        {
            BattleResultStatus.BaseAmountPlus = 0f;
        }
        //배율들//무기의 slot을 랜덤으로 결정해야함, luck에 영향을 받아서 결정됨
        //각 슬롯의 슬롯 스테이트 수만큼
        BattleResultStatus.ResultMagnification.Clear();
        for (int i = 0; i < Equipment_Info.EquipmentSlots.Length; i++)
        {
            List<float> NegativeList = new List<float>();
            List<float> PositiveList = new List<float>();
            float WholeSlotStateAmount = Equipment_Info.EquipmentSlots[i].SlotState.Length;
            float MultiplyNum = 60 / WholeSlotStateAmount;
            float NegativeAmount = 0f;
            float PositiveAmount = 0f;
            for (int j = 0; j < Equipment_Info.EquipmentSlots[i].SlotState.Length; j++)
            {
                if (Equipment_Info.EquipmentSlots[i].IsPositive[j] == true)
                {
                    PositiveAmount++;
                    PositiveList.Add(Equipment_Info.EquipmentSlots[i].SlotState[j]);
                }
                else
                {
                    NegativeAmount++;
                    NegativeList.Add(Equipment_Info.EquipmentSlots[i].SlotState[j]);
                }
            }

            //Luk이 겁나 낮아지는거 예외처리
            int RandNum;
            if (60 + (int)TP_Info.TotalLUK <= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)TP_Info.TotalLUK);
            }

            if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //부정적 List에서 하나를 뽑아서 저장
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)TP_Info.TotalLUK)
            {
                //긍정적 List에서 하나를 뽑아서 저장
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
        }
        //최종 추가 수치 // 거의 유일하게 EXPMG 초반 강화 효과에 영향을 받는 수치임
        if (JsonReadWriteManager.Instance.E_Info.EarlyExperienceMagnification >= 7)//7레벨 이상일때만
        {
            BattleResultStatus.FinalResultAmountPlus = P_Info.Experience / 10f;
        }
        else
        {
            BattleResultStatus.FinalResultAmountPlus = 0f;
        }
        //최종 수치 -> (기초 수치 + 기초 추가 수치) * 배율들 안의 배율 1 * 배율들 안의 배율 2 ...... * 배율들 안의 배율 n + 최종 추가 수치;
        float FinalMultiplyNum = 1f;
        for (int i = 0; i < BattleResultStatus.ResultMagnification.Count; i++)
        {
            FinalMultiplyNum *= BattleResultStatus.ResultMagnification[i];
        }
        BattleResultStatus.FinalResultAmount = (int)(((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * FinalMultiplyNum) + BattleResultStatus.FinalResultAmountPlus);
    }

    public void CalculateMonsterAction()
    {
        MonsterCurrentStatus MC_Info = CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus();
        int CurrentState = CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState;
        int MonEquipmentCode = 0;
        switch (CurrentState)
        {
            case (int)EMonsterActionState.Attack:
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterWeaponCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentATK;
                break;
            case (int)EMonsterActionState.Defense:
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterArmorCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentDUR;
                break;
        }
        EquipmentSO ESO_Info = EquipmentInfoManager.Instance.GetMonEquipmentInfo(MonEquipmentCode);

        BattleResultStatus.BaseAmount = 0f;
        BattleResultStatus.BaseAmountPlus = 0f;
        BattleResultStatus.ResultMagnification.Clear();
        BattleResultStatus.FinalResultAmountPlus = 0f;
        BattleResultStatus.FinalResultAmount = 0f;

        switch(CurrentState)
        {
            case (int)EMonsterActionState.Attack:
                ESO_Info = EquipmentInfoManager.Instance.GetMonEquipmentInfo(CurrentTurnObject.GetComponent<Monster>().MonsterWeaponCode);
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentATK;
                break;
            case (int)EMonsterActionState.Defense:
                ESO_Info = EquipmentInfoManager.Instance.GetMonEquipmentInfo(CurrentTurnObject.GetComponent<Monster>().MonsterArmorCode);
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentDUR;
                break;
        }

        if(BattleResultStatus.BaseAmount > 0)
        {
            SetMonsterBattleStatus(MC_Info, ESO_Info);
        }
    }

    protected void SetMonsterBattleStatus(MonsterCurrentStatus MC_Info, EquipmentSO ESO_Info)
    {
        //BattleResult에 결과값 저장
        for (int i = 0; i < ESO_Info.EquipmentSlots.Length; i++)
        {
            List<float> NegativeList = new List<float>();
            List<float> PositiveList = new List<float>();
            float WholeSlotStateAmount = ESO_Info.EquipmentSlots[i].SlotState.Length;
            float MultiplyNum = 60 / WholeSlotStateAmount;
            float NegativeAmount = 0f;
            float PositiveAmount = 0f;
            for (int j = 0; j < ESO_Info.EquipmentSlots[i].SlotState.Length; j++)
            {
                if (ESO_Info.EquipmentSlots[i].IsPositive[j] == true)
                {
                    PositiveAmount++;
                    PositiveList.Add(ESO_Info.EquipmentSlots[i].SlotState[j]);
                }
                else
                {
                    NegativeAmount++;
                    NegativeList.Add(ESO_Info.EquipmentSlots[i].SlotState[j]);
                }
            }

            //Luk이 겁나 낮아지는거 예외처리
            int RandNum;
            if (60 + (int)MC_Info.MonsterCurrentLUK <= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)MC_Info.MonsterCurrentLUK);
            }

            if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //부정적 List에서 하나를 뽑아서 저장
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)MC_Info.MonsterCurrentLUK)
            {
                //긍정적 List에서 하나를 뽑아서 저장
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
        }

        float FinalMultiplyNum = 1f;
        for (int i = 0; i < BattleResultStatus.ResultMagnification.Count; i++)
        {
            FinalMultiplyNum *= BattleResultStatus.ResultMagnification[i];
        }
        BattleResultStatus.FinalResultAmount = ((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * FinalMultiplyNum) + BattleResultStatus.FinalResultAmountPlus;
    }
}
