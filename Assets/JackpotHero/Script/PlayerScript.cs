using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
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
        //PlayerBuff.BuffList[(int)EBuffType.Burn] = 20;
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
        PlayerTotalState.TotalSTR = JsonReadWriteManager.Instance.GetEarlyState("STR") +
            PlayerState.StrengthLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddSTRAmount;
        //SetTotalDUR
        PlayerTotalState.TotalDUR = JsonReadWriteManager.Instance.GetEarlyState("DUR") +
            PlayerState.DurabilityLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddDURAmount;
        //SetTotalRES
        PlayerTotalState.TotalRES = JsonReadWriteManager.Instance.GetEarlyState("RES") +
            PlayerState.ResilienceLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddRESAmount;
        //SetTotalSPD
        PlayerTotalState.TotalSPD = JsonReadWriteManager.Instance.GetEarlyState("SPD") +
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
            PlayerTotalState.TotalLUK += 30;
        }
        if (PlayerBuff.BuffList[(int)EBuffType.Misfortune] >= 1)
        {
            PlayerTotalState.TotalLUK -= 30;
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

    public void SetInitBuff()//���߿� ���� �� �þ�� ���⼭//���ʹ� Monster�� SetInitBuff����
    {
        if (JsonReadWriteManager.Instance.E_Info.EarlyStrengthLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.OverWhelmingPower] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyDurabilityLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.ThronArmor] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyResilienceLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.AdvancedRest] = 99;

        /*
        ���ǵ� 7������ �ش��ϴ� ������ �����Ǵ� ���Ϳ��� �����
        if (JsonReadWriteManager.Instance.E_Info.EarlySpeedLevel >= 7)
        */

        if (JsonReadWriteManager.Instance.E_Info.EarlyHpLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.HealingFactor] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyTirednessLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.ReCharge] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyExperience >= 7)
            PlayerBuff.BuffList[(int)EBuffType.Rapine] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyExperienceMagnification >= 7)
            PlayerBuff.BuffList[(int)EBuffType.EXPPower] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.WeaponMaster] = 99;

        //PlayerBuff.BuffList[(int)EBuffType.WeaponMaster] = 20;
        //PlayerBuff.BuffList[(int)EBuffType.AttackDebuff] = 20;
        //PlayerBuff.BuffList[(int)EBuffType.EXPPower] = 20;
        PlayerBuff.BuffList[(int)EBuffType.Misfortune] = 20;
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

    public void RecordBeforeShield()//ShieldAmount�� ��ȭ�� ����Ǳ�����//������, �� ����Ҷ�, ������ ������
    {
        BeforeShield = (int)PlayerState.ShieldAmount;
    }

    public void PlayerRecordGiveDamage(float GiveDamage)//���Ϳ��� �ִ� �������� ����ϱ� ����
    {
        PlayerState.GiveDamage += GiveDamage;
        if(PlayerState.MostPowerfulDamage < GiveDamage)
        {
            PlayerState.MostPowerfulDamage = GiveDamage;
        }
    }

    public void PlayerDamage(float DamagePoint)//�ڽ��� �޴� �������� ����ϰ� �������� �޾� ����� ü���� ���ϋ�
    {
        PlayerState.ReceiveDamage += DamagePoint;//GameRecord
        if (PlayerBuff.BuffList[(int)EBuffType.Defenseless] >= 1)
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
        switch(ActionType)
        {
            case "Attack":
                if(PlayerTotalState.CurrentSTA < EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).SpendTiredness)//�Ƿε��� �����Ҷ�
                {
                    return false;
                }
                else
                {
                    if (PlayerBuff.BuffList[(int)EBuffType.FrostBite] >= 1)
                    {
                        PlayerSpendSTA(EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).SpendTiredness + 50);
                    }
                    else
                    {
                        PlayerSpendSTA(EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).SpendTiredness);
                    }
                }
                break;
            case "Defense":
                if (PlayerTotalState.CurrentSTA < EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).SpendTiredness)//�Ƿε��� �����Ҷ�
                {
                    return false;
                }
                else
                {
                    if (PlayerBuff.BuffList[(int)EBuffType.FrostBite] >= 1)
                    {
                        PlayerSpendSTA(EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).SpendTiredness + 50);
                    }
                    else
                    {
                        PlayerSpendSTA(EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).SpendTiredness);
                    }
                }
                break;
        }
        return true;
    }

    public void DefeatFromBattle()//�������� ������//���ų� ���ӿ��� �̱�ų�
    {
        int EarlyPoint = 0;
        if (PlayerState.CurrentFloor > JsonReadWriteManager.Instance.E_Info.PlayerReachFloor)
            JsonReadWriteManager.Instance.E_Info.PlayerReachFloor = PlayerState.CurrentFloor;

        EarlyPoint += JsonReadWriteManager.Instance.E_Info.PlayerReachFloor / 5;
        EarlyPoint += (int)(PlayerState.GiveDamage / 1000);
        EarlyPoint += (int)(PlayerState.ReceiveDamage / 500);
        EarlyPoint += (int)(PlayerState.MostPowerfulDamage / 100);
        EarlyPoint += (int)(PlayerState.Experience / 2000);

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
}
