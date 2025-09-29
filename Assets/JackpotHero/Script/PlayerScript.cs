using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerForm
{
    Normal_Form,
    ATK_Form,
    DUR_Form,
    RES_Form,
    SPD_Form,
    LUK_Form,
    HP_Form,
    STA_Form,
    EXP_Form,
    EXPMG_Form,
    EQUIP_Form
}

public enum EPlayerAnimationState
{
    Idle,
    Walk,
    Idle_Battle,
    Defeat,
    Rest,
}

public class TotalPlayerState
{
    public float MaxHP;
    public float CurrentHP;
    public float MaxSTA;
    public float CurrentSTA;
    public float TotalSTR;
    public float TotalDUR;
    public float TotalRES;
    public float TotalSPD;
    public float TotalLUK;
    public int CurrentForm;
}

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ShieldUIPos;
    public GameObject BuffUIPos;
    public GameObject ActionTypePos;
    public GameObject HeroineBody;
    //PlayerState
    protected PlayerInfo PlayerState;
    protected TotalPlayerState PlayerTotalState = new TotalPlayerState();
    [HideInInspector]
    public BuffInfo PlayerBuff = new BuffInfo();
    protected float AllEquipmentTier = 0f;
    public int BeforeShield { protected set; get; } = 0;

    public float AttackAverageIncrease { protected set; get; } = 0;
    public float DefenseAverageIncrease { protected set; get; } = 0;
    public float RestAverageIncrease { protected set; get; } = 0;

    const float BasicHP = 100f;
    const float BasicSTA = 1000f;
    const float BasicSTR = 5f;
    const float BasicDUR = 5f;
    const float BasicRES = 5f;
    const float BasicSPD = 5f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerInit()
    {
        PlayerState = JsonReadWriteManager.Instance.GetCopyPlayerInfo();
        SetPlayerTotalStatus();
        //SetEXP
        if(PlayerState.Experience == 0)
            PlayerState.Experience = (int)JsonReadWriteManager.Instance.GetEarlyState("EXP") + PlayerState.Experience;
        //SetCurrentForm
        List<int> CanActiveForm = new List<int>();
        if(JsonReadWriteManager.Instance.E_Info.EarlyStrengthLevel >= 7)
            CanActiveForm.Add((int)PlayerForm.STA_Form);

        if(JsonReadWriteManager.Instance.E_Info.EarlyDurabilityLevel >= 7)
            CanActiveForm.Add((int)PlayerForm.DUR_Form);

        if (JsonReadWriteManager.Instance.E_Info.EarlyResilienceLevel >= 7)
            CanActiveForm.Add((int)PlayerForm.RES_Form);

        if (JsonReadWriteManager.Instance.E_Info.EarlySpeedLevel >= 7)
            CanActiveForm.Add((int)PlayerForm.SPD_Form);

        if (JsonReadWriteManager.Instance.E_Info.EarlyLuckLevel >= 7)
            CanActiveForm.Add((int)PlayerForm.LUK_Form);

        if (JsonReadWriteManager.Instance.E_Info.EarlyHpLevel >= 7)
            CanActiveForm.Add((int)PlayerForm.HP_Form);

        if (JsonReadWriteManager.Instance.E_Info.EarlyTirednessLevel >= 7)
            CanActiveForm.Add((int)PlayerForm.STA_Form);

        if (JsonReadWriteManager.Instance.E_Info.EarlyExperience >= 7)
            CanActiveForm.Add((int)PlayerForm.EXP_Form);

        if (JsonReadWriteManager.Instance.E_Info.EarlyExperienceMagnification >= 7)
            CanActiveForm.Add((int)PlayerForm.EXPMG_Form);

        if (JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 7)
            CanActiveForm.Add((int)PlayerForm.EQUIP_Form);

        if (CanActiveForm.Count <= 0)
            CanActiveForm.Add((int)PlayerForm.Normal_Form);

        int Rand = Random.Range(0, CanActiveForm.Count);

        PlayerTotalState.CurrentForm = CanActiveForm[Rand];
        //SetCurrentFloor
        if (PlayerState.CurrentFloor <= 0)
        {
            PlayerState.CurrentFloor = 1;
        }
        //---------------Test
        //PlayerBuff.BuffList[(int)EBuffType.Recharge] = 20;
        //PlayerBuff.BuffList[(int)EBuffType.ToughFist] = 20;
        //PlayerBuff.BuffList[(int)EBuffType.EXPPower] = 20;
        //PlayerBuff.BuffList[5] = 6;
        //---------------
    }

    public void SetPlayerTotalStatus()//���� ����Ҷ� �ص� ������/// ������ ���� ���� ��ȭ�� ǥ���ؾ� �ɰ� ���⵵ �ϰ�
    {
        //SetHP
        PlayerTotalState.MaxHP = BasicHP + JsonReadWriteManager.Instance.GetEarlyState("HP") +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddHPAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddHPAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddHPAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddHPAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddHPAmount;
        PlayerTotalState.CurrentHP = PlayerTotalState.MaxHP * PlayerState.CurrentHpRatio;
        //SetSTA
        PlayerTotalState.MaxSTA = BasicSTA + JsonReadWriteManager.Instance.GetEarlyState("STA") +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddTirednessAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddTirednessAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddTirednessAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddTirednessAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddTirednessAmount;
        PlayerTotalState.CurrentSTA = PlayerTotalState.MaxSTA * PlayerState.CurrentTirednessRatio;
        //SetTotalSTR
        PlayerTotalState.TotalSTR = BasicSTR + JsonReadWriteManager.Instance.GetEarlyState("STR") +
            PlayerState.StrengthLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddSTRAmount;
        //SetTotalDUR
        PlayerTotalState.TotalDUR = BasicDUR + JsonReadWriteManager.Instance.GetEarlyState("DUR") +
            PlayerState.DurabilityLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddDURAmount;
        //SetTotalRES
        PlayerTotalState.TotalRES = BasicRES + JsonReadWriteManager.Instance.GetEarlyState("RES") +
            PlayerState.ResilienceLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddRESAmount;
        //SetTotalSPD
        PlayerTotalState.TotalSPD = BasicSPD + JsonReadWriteManager.Instance.GetEarlyState("SPD") +
            PlayerState.SpeedLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddSPDAmount;
        //SetTotalLUK
        PlayerTotalState.TotalLUK = JsonReadWriteManager.Instance.GetEarlyState("LUK") +
            PlayerState.LuckLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddLUKAmount;
        if (PlayerBuff.BuffList[(int)EBuffType.Luck] >= 1)
        {
            PlayerTotalState.TotalLUK += 10;
        }
        if (PlayerBuff.BuffList[(int)EBuffType.Misfortune] >= 1)
        {
            PlayerTotalState.TotalLUK -= 10;
        }

        //������ ������� ��·�
        AttackAverageIncrease = 0;
        List<float> IncreaseDifference = new List<float>();
        foreach (EquipmentSlot Slots in EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).EquipmentSlots)
        {
            int PositiveCount = 0;
            foreach(bool IsPositive in Slots.IsPositive)
            {
                if (IsPositive == true)
                    PositiveCount++;
            }
            int MagnificationAmount = 60 / Slots.IsPositive.Length;
            float BeforePositivePercent = (PositiveCount * MagnificationAmount) / 60f;
            float AfterPositivePercent = 0;
            if (60 + PlayerTotalState.TotalLUK > 0)
            {
                AfterPositivePercent = ((PositiveCount * MagnificationAmount) + PlayerTotalState.TotalLUK) / (60 + PlayerTotalState.TotalLUK);
            }
            IncreaseDifference.Add(AfterPositivePercent - BeforePositivePercent);
        }
        foreach (float InDif in IncreaseDifference)
        {
            AttackAverageIncrease += InDif;
        }
        AttackAverageIncrease = (AttackAverageIncrease / EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).EquipmentSlots.Length) * 100;
        //����� ������� ��·�
        DefenseAverageIncrease = 0;
        IncreaseDifference.Clear();
        foreach (EquipmentSlot Slots in EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).EquipmentSlots)
        {
            int PositiveCount = 0;
            foreach (bool IsPositive in Slots.IsPositive)
            {
                if (IsPositive == true)
                    PositiveCount++;
            }
            int MagnificationAmount = 60 / Slots.IsPositive.Length;
            float BeforePositivePercent = (PositiveCount * MagnificationAmount) / 60f;
            float AfterPositivePercent = 0;
            if (60 + PlayerTotalState.TotalLUK > 0)
            {
                AfterPositivePercent = ((PositiveCount * MagnificationAmount) + PlayerTotalState.TotalLUK) / (60 + PlayerTotalState.TotalLUK);
            }
            IncreaseDifference.Add(AfterPositivePercent - BeforePositivePercent);
        }
        foreach (float InDif in IncreaseDifference)
        {
            DefenseAverageIncrease += InDif;
        }
        DefenseAverageIncrease = (DefenseAverageIncrease / EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).EquipmentSlots.Length) * 100;
        //�޽��� ������� ��·�
        RestAverageIncrease = 0;
        IncreaseDifference.Clear();
        foreach (EquipmentSlot Slots in EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).EquipmentSlots)
        {
            int PositiveCount = 0;
            foreach (bool IsPositive in Slots.IsPositive)
            {
                if (IsPositive == true)
                    PositiveCount++;
            }
            int MagnificationAmount = 60 / Slots.IsPositive.Length;
            float BeforePositivePercent = (PositiveCount * MagnificationAmount) / 60f;
            float AfterPositivePercent = 0;
            if (60 + PlayerTotalState.TotalLUK > 0)
            {
                AfterPositivePercent = ((PositiveCount * MagnificationAmount) + PlayerTotalState.TotalLUK) / (60 + PlayerTotalState.TotalLUK);
            }
            IncreaseDifference.Add(AfterPositivePercent - BeforePositivePercent);
        }
        foreach (float InDif in IncreaseDifference)
        {
            RestAverageIncrease += InDif;
        }
        RestAverageIncrease = (RestAverageIncrease / EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).EquipmentSlots.Length) * 100;
    }
    public PlayerInfo GetPlayerStateInfo()
    {
        return PlayerState;
    }

    public TotalPlayerState GetTotalPlayerStateInfo()
    {
        return PlayerTotalState;
    }
    public void SetInitBuffByMonsters(List<GameObject> ActiveMonsters)
    {
        
        for(int i = 0; i < ActiveMonsters.Count; i++)
        {
            /*
            if (ActiveMonsters[i].GetComponent<Monster>().MonsterName == "Mandrake")
            {
                PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath] += ActiveMonsters[i].GetComponent<Monster>().MonsterGiveCurseOfDead();
            }
            */
        }
    }
    public void SetInitBuff()//���߿� ���� �� �þ�� ���⼭//���ʹ� Monster�� SetInitBuff����
    {
        if (JsonReadWriteManager.Instance.E_Info.EarlyStrengthLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.OverWhelmingPower] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyDurabilityLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.ThornArmor] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyResilienceLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.AdvancedRest] = 99;

        /*
        ���ǵ� 7������ �ش��ϴ� ������ �����Ǵ� ���Ϳ��� �����
        if (JsonReadWriteManager.Instance.E_Info.EarlySpeedLevel >= 7)
        */

        if (JsonReadWriteManager.Instance.E_Info.EarlyHpLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.BloodFamiliy] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyTirednessLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.TiredControll] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyExperience >= 7)
            PlayerBuff.BuffList[(int)EBuffType.Exploitation] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyExperienceMagnification >= 7)
            PlayerBuff.BuffList[(int)EBuffType.EXPPower] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.WeaponMaster] = 99;

        SetDefeseResilienceBuff();

        if (PlayerState.SaveRestQualityBySuddenAttack != -1)//������ ���ߴٸ�
        {
            PlayerBuff.BuffList[(int)EBuffType.Defenseless] = 1;
        }

        if(JsonReadWriteManager.Instance.LkEv_Info.PowwersCeremony >= 1)
        {
            PlayerBuff.BuffList[(int)EBuffType.Luck] += 3;
            JsonReadWriteManager.Instance.LkEv_Info.PowwersCeremony -= 1;
        }

        //PlayerBuff.BuffList[(int)EBuffType.TiredControll] = 99;
        //PlayerBuff.BuffList[(int)EBuffType.AttackDebuff] = 20;
        //PlayerBuff.BuffList[(int)EBuffType.EXPPower] = 20;
        //PlayerBuff.BuffList[(int)EBuffType.Misfortune] = 20;
    }

    public void SetDefeseResilienceBuff()
    {
        PlayerBuff.BuffList[(int)EBuffType.Defense] = (int)(PlayerTotalState.TotalDUR / 5);
        PlayerBuff.BuffList[(int)EBuffType.Resilience] = (int)(PlayerTotalState.TotalRES / 5);
    }

    public void SetChainAttackBuff(bool IsAttack, bool IsPlayerTurn)
    {
        //Ư�� ������ �Ǹ� ����Ÿ�� ����
        //�ӽ÷� 52001
        if(PlayerState.EquipAccessoriesCode == 0)
        {
            if(IsAttack == true && IsPlayerTurn == true)
            {
                PlayerBuff.BuffList[(int)EBuffType.ChainAttack] += 1;
            }
            else
            {
                PlayerBuff.BuffList[(int)EBuffType.ChainAttack] = 0;
            }
        }
    }

    public void ApplyBuff(int ApplyBuffType, int BuffCount)
    {
        Debug.Log("IsThisActive");
        PlayerBuff.BuffList[ApplyBuffType] += BuffCount;
    }
    public void SetIsSuddenAttackAndRestQuality()
    {
        PlayerState.SaveRestQualityBySuddenAttack = PlayerState.CurrentPlayerActionDetails;
    }

    public void EndOfAction(bool IsBossBattle = false)//��� �ൿ�� ��������
    {
        BeforeShield = 0;
        PlayerState.ShieldAmount = 0;
        for(int i = 0; i < PlayerBuff.BuffList.Length; i++)
        {
            if (PlayerBuff.BuffList[i] >= 1)
            {
                PlayerBuff.BuffList[i] = 0;
            }
        }
        
        if(IsBossBattle == true)
        {

        }
        else
        {
            if (PlayerState.SaveRestQualityBySuddenAttack != -1)//�޽��� ���ݴ������� ���� ���·� �ǵ���
            {
                PlayerState.CurrentPlayerAction = (int)EPlayerCurrentState.Rest;
                PlayerState.CurrentPlayerActionDetails = PlayerState.SaveRestQualityBySuddenAttack;
                PlayerState.SaveRestQualityBySuddenAttack = -1;
            }
            else if (PlayerState.SaveRestQualityBySuddenAttack == -1)
            {
                PlayerState.CurrentPlayerAction = (int)EPlayerCurrentState.SelectAction;
                PlayerState.CurrentPlayerActionDetails = 0;
            }
        }
    }

    public void WinBossBattle()
    {
        PlayerState.CurrentPlayerAction = (int)EPlayerCurrentState.SelectAction;
        PlayerState.CurrentFloor++;
        PlayerState.DetectNextFloorPoint = 0;
        PlayerState.CurrentPlayerActionDetails = 0;
    }

    public float GetAllEquipTier()//�÷��̾��� ������ ���� �κ��丮�� ����� ��� Ƽ���� ���� ������(EQUIP 7���� ������ ����)
    {
        AllEquipmentTier = 0f;

        //���� �÷��̾ ���� �ִ� ���
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).EquipmentTier;
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).EquipmentTier;
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).EquipmentTier;
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).EquipmentTier;
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).EquipmentTier;

        //���� �κ��丮�� �ִ� ��� ���
        for(int i = 0; i < PlayerState.EquipmentInventory.Length; i++)
        {
            if(PlayerState.EquipmentInventory[i] >= 1)
            {
                AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipmentInventory[i]).EquipmentTier;
            }
        }

        return AllEquipmentTier;
    }
    public void RecordKillCount(int KillNormalCount = 0, int KillEliteCount = 0)
    {
        PlayerState.KillNormalMonster += KillNormalCount;
        PlayerState.KillEliteMonster += KillEliteCount;
    }


    public void RecordBeforeShield()//ShieldAmount�� ��ȭ�� ����Ǳ�����//������, �� ����Ҷ�, ������ ������
    {
        BeforeShield = (int)PlayerState.ShieldAmount;
    }

    public void PlayerDamage(float DamagePoint, bool IsIgnoreDefense = false)//�ڽ��� �޴� �������� ����ϰ� �������� �޾� ����� ü���� ���ϋ�
    {
        if (IsIgnoreDefense == false && PlayerBuff.BuffList[(int)EBuffType.Defense] >= 1)
            DamagePoint -= PlayerBuff.BuffList[(int)EBuffType.Defense];

        if (DamagePoint < 0)
            DamagePoint = 0;

        if (IsIgnoreDefense == false && PlayerBuff.BuffList[(int)EBuffType.Defenseless] >= 1)
            DamagePoint = DamagePoint * 2;

        float RestDamage = 0;
        if(PlayerState.ShieldAmount >= DamagePoint)
        {
            RecordBeforeShield();
            PlayerState.ShieldAmount -= DamagePoint;
        }
        else
        {
            RestDamage = DamagePoint - PlayerState.ShieldAmount;
            RecordBeforeShield();
            PlayerState.ShieldAmount = 0;
        }
        PlayerTotalState.CurrentHP -= RestDamage;
        PlayerState.CurrentHpRatio = PlayerTotalState.CurrentHP / PlayerTotalState.MaxHP;
    }

    public void PlayerGetShield(float ShieldPoint)//���尡 ��������
    {
        RecordBeforeShield();
        PlayerState.ShieldAmount += ShieldPoint;
    }

    public void PlayerRegenSTA(float RestPoint)//������ �Ƿε� ȸ���� �ҋ�
    {
        PlayerTotalState.CurrentSTA += RestPoint;
        if(PlayerTotalState.MaxSTA <= PlayerTotalState.CurrentSTA)
        {
            PlayerTotalState.CurrentSTA = PlayerTotalState.MaxSTA;
        }
        PlayerState.CurrentTirednessRatio = PlayerTotalState.CurrentSTA / PlayerTotalState.MaxSTA;
    }

    public void PlayerSpendSTA(float SpendPoint)
    {
        PlayerTotalState.CurrentSTA -= SpendPoint;
        if(PlayerTotalState.CurrentSTA < 0)
        {
            PlayerTotalState.CurrentSTA = 0;
        }
        PlayerState.CurrentTirednessRatio = PlayerTotalState.CurrentSTA / PlayerTotalState.MaxSTA;
    }

    public void PlayerRegenHp(float RegenPoint)
    {
        PlayerTotalState.CurrentHP += RegenPoint;
        if(PlayerTotalState.CurrentHP >= PlayerTotalState.MaxHP)
        {
            PlayerTotalState.CurrentHP = PlayerTotalState.MaxHP;
        }
        else if(PlayerTotalState.CurrentHP < 1)
        {
            PlayerTotalState.CurrentHP = 1;
        }
        PlayerState.CurrentHpRatio = PlayerTotalState.CurrentHP / PlayerTotalState.MaxHP;
    }

    public int ReturnEXPByEXPMagnification(int EXPAmount)//����ġ�� ������ ����ġ�� �����
    {
        return (int)(EXPAmount * JsonReadWriteManager.Instance.GetEarlyState("EXPMG"));
    }

    public void SetPlayerEXPAmount(int EXPAmount, bool IsAlreadyCalculate = false)//����ġ�� ������ IsAlreadyCalculate�� ���¿� ���� ����ġ�� �����
    {
        if (IsAlreadyCalculate == true)//�ٸ� ������ ReturnEXPByEXPMagnification���� ����� ������
        {
            PlayerState.Experience += EXPAmount;
        }
        else if (IsAlreadyCalculate == false)
        {
            PlayerState.Experience += ReturnEXPByEXPMagnification(EXPAmount);
        }

        if(PlayerState.Experience < 0)
        {
            PlayerState.Experience = 0;
        }
        /*
        if (EXPAmount > 0)
        {
            

        }
        else
        {
            PlayerState.Experience += EXPAmount;
            //PlayerState.SpendEXP += EXPAmount;
            //��� EXP�� �ϸ� �ȵɰ� ����. upgrade�� �Ѿ��� �ø��� �־....
            //���� EXP�� ����
        }
        */
    }

    public bool SpendSTA(string ActionType)//��Ʋ�� �ൿ�� �ϸ� �Ƿε��� ���� �Ҷ�
    {
        float SpendTired = 0;
        switch(ActionType)
        {
            case "Attack":
                SpendTired += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).SpendTiredness;
                break;
            case "Defense":
                SpendTired += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).SpendTiredness;
                break;
        }

        if (PlayerBuff.BuffList[(int)EBuffType.Petrification] >= 1)
        {
            SpendTired += PlayerBuff.BuffList[(int)EBuffType.Petrification] * 20;
        }

        //�Ƿε� ������ ���� �ڿ� ����
        if (PlayerBuff.BuffList[(int)EBuffType.TiredControll] >= 1)
        {
            float TiredRatio = PlayerState.CurrentTirednessRatio;
            if (TiredRatio < 0.1)
                TiredRatio = 0.1f;

            SpendTired = SpendTired * TiredRatio;
        }

        if (PlayerTotalState.CurrentSTA < SpendTired)
        {
            return false;
        }
        else
        {
            PlayerSpendSTA((int)SpendTired);
        }

        return true;
    }

    public void CalculateEarlyPoint()//�������� ������//���ų� ���ӿ��� �̱�ų�
    {
        int EarlyPoint = 0;
        if (PlayerState.CurrentFloor > JsonReadWriteManager.Instance.E_Info.PlayerReachFloor)
            JsonReadWriteManager.Instance.E_Info.PlayerReachFloor = PlayerState.CurrentFloor;

        EarlyPoint += JsonReadWriteManager.Instance.E_Info.PlayerReachFloor;

        if (JsonReadWriteManager.Instance.P_Info.KillNormalMonster / 4 > 5)
            EarlyPoint += 5;
        else
            EarlyPoint += (int)(JsonReadWriteManager.Instance.P_Info.KillNormalMonster / 4);

        if (JsonReadWriteManager.Instance.P_Info.KillEliteMonster * 7 / 20 > 5)
            EarlyPoint += 5;
        else
            EarlyPoint += (int)(JsonReadWriteManager.Instance.P_Info.KillEliteMonster * 7 / 20);

        EarlyPoint += (int)(JsonReadWriteManager.Instance.P_Info.Experience / 2000);
        EarlyPoint += (int)(JsonReadWriteManager.Instance.P_Info.GoodKarma / 10);
        /*
        EarlyPoint += (int)(PlayerState.GiveDamage / 1000);
        EarlyPoint += (int)(PlayerState.ReceiveDamage / 500);
        EarlyPoint += (int)(PlayerState.MostPowerfulDamage / 100);
        EarlyPoint += (int)(PlayerState.Experience / 2000);
        */

        JsonReadWriteManager.Instance.E_Info.PlayerEarlyPoint = EarlyPoint;//������ �������� �ʱ�ȭ �ؾ��ҵ�? ���߿�? ���⼭�ϸ� ����� �Ұ���
    }

    public void RecoverHPNSTAByRest(float RecoverAmount)
    {
        if(PlayerState.CurrentHpRatio < 1f)
        {
            PlayerState.CurrentHpRatio += RecoverAmount;
            if(PlayerState.CurrentHpRatio > 1f )
            {
                PlayerState.CurrentHpRatio = 1f;
            }
            PlayerTotalState.CurrentHP = PlayerTotalState.MaxHP * PlayerState.CurrentHpRatio;
        }
        if(PlayerState.CurrentTirednessRatio < 1f)
        {
            PlayerState.CurrentTirednessRatio += RecoverAmount;
            if(PlayerState.CurrentTirednessRatio > 1f)
            {
                PlayerState.CurrentTirednessRatio = 1f;
            }
            PlayerTotalState.CurrentSTA = PlayerTotalState.MaxSTA * PlayerState.CurrentTirednessRatio;
        }
    }

    public void UpgradePlayerStatus(UpGradeAfterStatus UpgradeStatus)
    {
        PlayerState.StrengthLevel = UpgradeStatus.AfterSTR;
        PlayerState.DurabilityLevel = UpgradeStatus.AfterDUR;
        PlayerState.ResilienceLevel = UpgradeStatus.AfterRES;
        PlayerState.SpeedLevel = UpgradeStatus.AfterSPD;
        PlayerState.LuckLevel = UpgradeStatus.AfterLUK;
        PlayerState.Level = UpgradeStatus.AfterLevel;
        SetPlayerTotalStatus();
    }

    public void UpgradePlayerSingleStatus(string UpgradeState, int Amount)
    {
        switch(UpgradeState)
        {
            case "STR":
                PlayerState.StrengthLevel += Amount;
                break;
            case "DUR":
                PlayerState.DurabilityLevel += Amount;
                break;
            case "RES":
                PlayerState.ResilienceLevel += Amount;
                break;
            case "SPD":
                PlayerState.SpeedLevel += Amount;
                break;
            case "LUK":
                PlayerState.LuckLevel += Amount;
                break;
        }
        PlayerState.Level += Amount;
        SetPlayerTotalStatus();
    }

    public bool IsInventoryFull()
    {
        //JsonReadWriteManager.Instance.GetEarlyState("EQUIP") -> �̰ɷ� ���� �ʱ� ��ȭ�� ���� �÷��̾��� ���� �κ��丮 ���� �˼� ����
        //�κ��丮�� �ڵ� ���� 0�ʰ��̸� ��� ����ִ� ĭ�� ++�Ѵ�
        int FullInventoryCount = 0;
        for(int i = 0; i < PlayerState.EquipmentInventory.Length; i++)
        {
            if (PlayerState.EquipmentInventory[i] > 0)//��������
            {
                FullInventoryCount++;
            }
        }
        if(JsonReadWriteManager.Instance.GetEarlyState("EQUIP") > FullInventoryCount)
        {//��밡���� �κ��丮 ĭ�� ���ִ� �κ��丮 ĭ���� ũ�ٸ�(�� ��á�ٸ�)
            return false;
        }
        else//�̰� �� á�ٸ�
        {
            return true;
        }
    }

    public void PutEquipmentToInven(int EquipmentCode)
    {
        if (IsInventoryFull() == true)
            return;

        int InventoryAmount = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");//�̰� ��밡���� �κ��丮 ����
        for (int i = 0; i < InventoryAmount; i++)//��밡�� �κ��丮 ���� ��ŭ �ݺ�//0�����̸� 0~3���� �ݺ��� 5���� �̻��̸� 0~11����
        {
            if (PlayerState.EquipmentInventory[i] == 0)//����ִٸ�
            {
                PlayerState.EquipmentInventory[i] = EquipmentCode;//���� ���� �κ��丮 list�� ����
                break;
            }
        }
    }

    //-----------------------------------------HeroineAnimation
    public void SetPlayerAnimation(int PlayerTargetState)
    {
        Animator HeroineAnimator = HeroineBody.GetComponent<Animator>();
        switch(PlayerTargetState)
        {
            case (int)EPlayerAnimationState.Idle:
                if(HeroineAnimator.GetInteger("HeroineState") != (int)EPlayerAnimationState.Idle)
                {
                    HeroineAnimator.SetInteger("HeroineState", (int)EPlayerAnimationState.Idle);
                }
                break;
            case (int)EPlayerAnimationState.Walk:
                if (HeroineAnimator.GetInteger("HeroineState") != (int)EPlayerAnimationState.Walk)
                {
                    HeroineAnimator.SetInteger("HeroineState", (int)EPlayerAnimationState.Walk);
                }
                break;
            case (int)EPlayerAnimationState.Idle_Battle:
                if (HeroineAnimator.GetInteger("HeroineState") != (int)EPlayerAnimationState.Idle_Battle)
                {
                    HeroineAnimator.SetInteger("HeroineState", (int)EPlayerAnimationState.Idle_Battle);
                }
                break;
            case (int)EPlayerAnimationState.Defeat:
                if (HeroineAnimator.GetInteger("HeroineState") != (int)EPlayerAnimationState.Defeat)
                {
                    HeroineAnimator.SetInteger("HeroineState", (int)EPlayerAnimationState.Defeat);
                }
                break;
            case (int)EPlayerAnimationState.Rest:
                if(HeroineAnimator.GetInteger("HeroineState") != (int)EPlayerAnimationState.Rest)
                {
                    HeroineAnimator.SetInteger("HeroineState", (int)EPlayerAnimationState.Rest);
                }
                break;
        }
    }
}
