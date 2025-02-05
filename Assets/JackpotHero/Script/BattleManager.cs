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

    //�̰Ŵ� ���ͳ� �÷��̾ ���� �Ϸ������� ��� �ҷ������°Ŵϱ� ���⼭ State�� �ʱ�ȭ �Ѵ�?
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
        //���� ��ġ
        BattleResultStatus.BaseAmount = TP_Info.TotalSTR;
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

            int RandNum = Random.Range(0, 60 + (int)TP_Info.TotalLUK);
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
            BattleResultStatus.FianlResultAmountPlus = P_Info.Experience / 10f;
        }
        else
        {
            BattleResultStatus.FianlResultAmountPlus = 0f;
        }
        //���� ��ġ -> (���� ��ġ + ���� �߰� ��ġ) * ������ ���� ���� 1 * ������ ���� ���� 2 ...... * ������ ���� ���� n + ���� �߰� ��ġ;
        float FinalMultiplyNum = 1f;
        for(int i = 0; i < BattleResultStatus.ResultMagnification.Count; i++)
        {
            FinalMultiplyNum *= BattleResultStatus.ResultMagnification[i];
        }
        BattleResultStatus.FinalResultAmount = ((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * FinalMultiplyNum) + BattleResultStatus.FianlResultAmountPlus;
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
