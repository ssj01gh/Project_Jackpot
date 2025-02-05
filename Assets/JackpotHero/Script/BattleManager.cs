using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
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
    public float FianlResultAmountPlus;
    public float FinalResultAmount;
}

public class BattleManager : MonoBehaviour
{
    protected Queue<GameObject> BattleTurn = new Queue<GameObject>();
    // Start is called before the first frame update
    public GameObject CurrentTurnObject { protected set; get; }
    public BattleResultStates BattleResultStatus { protected set; get; }
    public int CurrentState { protected set; get; } = (int)EBattleStates.Idle;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Queue<GameObject> GetBattleTurn()
    {
        return BattleTurn;
    }

    //이거는 몬스터나 플레이어가 턴을 완료했을때 계속 불러와지는거니까 여기서 State를 초기화 한다?
    public void SetBattleTurn(PlayerManager PlayerMgr, MonsterManager MonMgr)
    {
        BattleTurn.Clear();
        float PlayerSPD = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().TotalSPD;
        float PlayerActiveGauge = 0;

        float[] MonsterSPD = new float[MonMgr.GetActiveMonsters().Count];
        float[] MonsterActiveGauge = new float[MonMgr.GetActiveMonsters().Count];
        for (int i = 0; i < MonsterSPD.Length; i++)
        {
            MonsterSPD[i] = MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentSPD;
        }

        while(BattleTurn.Count < 9)
        {
            PlayerActiveGauge += PlayerSPD;
            for(int i = 0; i < MonsterSPD.Length; i++)
            {
                MonsterActiveGauge[i] += MonsterSPD[i];
            }


            if(PlayerActiveGauge >= 100)
            {
                PlayerActiveGauge -= 100;
                BattleTurn.Enqueue(PlayerMgr.GetPlayerInfo().gameObject);
            }

            for(int i = 0; i < MonsterSPD.Length; i++)
            {
                if (MonsterActiveGauge[i] >= 100)
                {
                    MonsterActiveGauge[i] -= 100;
                    BattleTurn.Enqueue(MonMgr.GetActiveMonsters()[i]);
                }
            }
        }
        CurrentState = (int)EBattleStates.Idle;
    }

    public void DecideCurrentBattleTurn()
    {
        if(CurrentState == (int)EBattleStates.Idle)
        {
            CurrentTurnObject = BattleTurn.Peek();
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

    protected void SetBattleStatus(PlayerInfo P_Info, TotalPlayerState TP_Info, EquipmentSO Equipment_Info, float AllEquipTier)
    {
        //기초 수치
        BattleResultStatus.BaseAmount = TP_Info.TotalSTR;
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

            int RandNum = Random.Range(0, 60 + (int)TP_Info.TotalLUK);
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
            BattleResultStatus.FianlResultAmountPlus = P_Info.Experience / 10f;
        }
        else
        {
            BattleResultStatus.FianlResultAmountPlus = 0f;
        }
        //최종 수치 -> (기초 수치 + 기초 추가 수치) * 배율들 안의 배율 1 * 배율들 안의 배율 2 ...... * 배율들 안의 배율 n + 최종 추가 수치;
        float FinalMultiplyNum = 1f;
        for(int i = 0; i < BattleResultStatus.ResultMagnification.Count; i++)
        {
            FinalMultiplyNum *= BattleResultStatus.ResultMagnification[i];
        }
        BattleResultStatus.FinalResultAmount = ((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * FinalMultiplyNum) + BattleResultStatus.FianlResultAmountPlus;
    }

    //CalculatePlayerAction
    public void CalculatePlayerAction(string PlayerAction)//플레이어 행동의 결과값을 계산하고 저장하는 함수
    {
        //여기에 들어왔다는건 CurrentTurnObject가 Player라는 것임
        //CurrentTurnObject.GetComponent<PlayerScript>().GetPlayerStateInfo().EquipWeaponCode;플레이어가 현재 끼고 있는 
        //플레이어의 무기에서는 slot의 정보만 필요
        //기초 수치인 힘은 플레이어 토탈 스테이터스에 존재함
        //플레이어의 초기 강화에 따라 기초 수치와 최종 수치에 변화를 줘야함 -> BattleResult에 기록
        PlayerInfo P_Info = CurrentTurnObject.GetComponent<PlayerScript>().GetPlayerStateInfo();
        TotalPlayerState TP_Info = CurrentTurnObject.GetComponent<PlayerScript>().GetTotalPlayerStateInfo();
        EquipmentSO ESO_Info = new EquipmentSO();
        switch (PlayerAction)
        {
            case "Attack":
                ESO_Info = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(P_Info.EquipWeaponCode);
                break;
            case "Defense":
                ESO_Info = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(P_Info.EquipArmorCode);
                break;
            case "Rest":
                ESO_Info = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(P_Info.EquipHatCode);
                break;
        }
        SetBattleStatus(P_Info, TP_Info, ESO_Info, CurrentTurnObject.GetComponent<PlayerScript>().GetAllEquipTier());
    }
}
