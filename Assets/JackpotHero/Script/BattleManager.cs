using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

    public void InitMonsterNPlayerActiveGuage(int MonsterCount, MonsterManager MonMgr)
    {
        BattleTurn.Clear();
        PlayerActiveGauge = 0;
        for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
        {
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge = 0;
        }
    }

    //이거는 몬스터나 플레이어가 턴을 완료했을때 계속 불러와지는거니까 여기서 State를 초기화 한다?
    public void SetBattleTurn(PlayerManager PlayerMgr, MonsterManager MonMgr)
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
                BattleResultStatus.BaseAmount = TP_Info.TotalSTR;
                break;
            case "Defense":
                ESO_Info = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(P_Info.EquipArmorCode);
                BattleResultStatus.BaseAmount = TP_Info.TotalDUR;
                break;
            case "Rest":
                ESO_Info = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(P_Info.EquipHatCode);
                BattleResultStatus.BaseAmount = TP_Info.TotalRES;
                break;
        }
        SetPlayerBattleStatus(P_Info, TP_Info, ESO_Info, CurrentTurnObject.GetComponent<PlayerScript>().GetAllEquipTier());
    }
    protected void SetPlayerBattleStatus(PlayerInfo P_Info, TotalPlayerState TP_Info, EquipmentSO Equipment_Info, float AllEquipTier)
    {
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
        BattleResultStatus.FinalResultAmount = ((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * FinalMultiplyNum) + BattleResultStatus.FinalResultAmountPlus;
    }

    public void CalculateMonsterAction()
    {
        MonsterCurrentStatus MC_Info = CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus();
        int CurrentState = CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState;
        EquipmentSO ESO_Info = new EquipmentSO();

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
