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

    //�̰Ŵ� ���ͳ� �÷��̾ ���� �Ϸ������� ��� �ҷ������°Ŵϱ� ���⼭ State�� �ʱ�ȭ �Ѵ�?
    public void SetBattleTurn(PlayerManager PlayerMgr, MonsterManager MonMgr)
    {
        if (BattleTurn.Count >= 1)
            BattleTurn.RemoveAt(0);//ù��°���� �����.(�ֳ��ϸ� ���⿡ ũ�Ⱑ 1�̻��� BattleTurn�� ���Դٴ°��� �̹� 0��°�� �ִ� ������Ʈ�� ���� ������

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
            //ActiveMonster�� ������ �ٿ��� MonsterActiveGuage�� ������ �״�� ������ �ȴ�.
            //���� index1�� ���Ͱ� ������ ������ index2���� ���Ͱ� index1�� ���� MonsterActiveGuage�� �̾�ް� �Ǵ� ������ �����.
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
                //����� ������ FastestMonster�� �����Ȱ���
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
    public void CalculatePlayerAction(string PlayerAction)//�÷��̾� �ൿ�� ������� ����ϰ� �����ϴ� �Լ�
    {
        //���⿡ ���Դٴ°� CurrentTurnObject�� Player��� ����
        //CurrentTurnObject.GetComponent<PlayerScript>().GetPlayerStateInfo().EquipWeaponCode;�÷��̾ ���� ���� �ִ� 
        //�÷��̾��� ���⿡���� slot�� ������ �ʿ�
        //���� ��ġ�� ���� �÷��̾� ��Ż �������ͽ��� ������
        //�÷��̾��� �ʱ� ��ȭ�� ���� ���� ��ġ�� ���� ��ġ�� ��ȭ�� ����� -> BattleResult�� ���
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
        //���� �߰� ��ġ // ���� �����ϰ� EQUIP �ʹ� ��ȭ ȿ���� ������ �޴� ��ġ��
        if (JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 7)//7���� �̻��϶���
        {
            //�÷��̾ ���� �ִ� ������ ������ ��� ������ Ƽ���� ��
            BattleResultStatus.BaseAmountPlus = AllEquipTier;
        }
        else
        {
            BattleResultStatus.BaseAmountPlus = 0f;
        }
        //������//������ slot�� �������� �����ؾ���, luck�� ������ �޾Ƽ� ������
        //�� ������ ���� ������Ʈ ����ŭ
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

            //Luk�� �̳� �������°� ����ó��
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
                //������ List���� �ϳ��� �̾Ƽ� ����
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)TP_Info.TotalLUK)
            {
                //������ List���� �ϳ��� �̾Ƽ� ����
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
        }
        //���� �߰� ��ġ // ���� �����ϰ� EXPMG �ʹ� ��ȭ ȿ���� ������ �޴� ��ġ��
        if (JsonReadWriteManager.Instance.E_Info.EarlyExperienceMagnification >= 7)//7���� �̻��϶���
        {
            BattleResultStatus.FinalResultAmountPlus = P_Info.Experience / 10f;
        }
        else
        {
            BattleResultStatus.FinalResultAmountPlus = 0f;
        }
        //���� ��ġ -> (���� ��ġ + ���� �߰� ��ġ) * ������ ���� ���� 1 * ������ ���� ���� 2 ...... * ������ ���� ���� n + ���� �߰� ��ġ;
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
        //BattleResult�� ����� ����
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

            //Luk�� �̳� �������°� ����ó��
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
                //������ List���� �ϳ��� �̾Ƽ� ����
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)MC_Info.MonsterCurrentLUK)
            {
                //������ List���� �ϳ��� �̾Ƽ� ����
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
