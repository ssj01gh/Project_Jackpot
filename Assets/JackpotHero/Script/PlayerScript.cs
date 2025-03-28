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
    //PlayerState
    protected PlayerInfo PlayerState;
    protected TotalPlayerState PlayerTotalState = new TotalPlayerState();
    [HideInInspector]
    public BuffInfo PlayerBuff = new BuffInfo();
    protected float AllEquipmentTier = 0f;
    public int BeforeShield { protected set; get; } = 0;

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
        PlayerBuff.BuffList[(int)EBuffType.WeaponMaster] = 20;
        PlayerBuff.BuffList[(int)EBuffType.ToughFist] = 20;
        PlayerBuff.BuffList[(int)EBuffType.EXPPower] = 20;
        //PlayerBuff.BuffList[5] = 6;
        //---------------
    }

    public void SetPlayerTotalStatus()
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
    }
    public PlayerInfo GetPlayerStateInfo()
    {
        return PlayerState;
    }

    public TotalPlayerState GetTotalPlayerStateInfo()
    {
        return PlayerTotalState;
    }
    public void SetIsSuddenAttackAndRestQuality()
    {
        PlayerState.SaveRestQualityBySuddenAttack = PlayerState.CurrentPlayerActionDetails;
    }

    public void EndOfAction()//어떠한 행동이 끝났을떄
    {
        BeforeShield = 0;
        PlayerState.ShieldAmount = 0;
        if(PlayerState.SaveRestQualityBySuddenAttack != -1)//휴식중 습격당했으면 원래 상태로 되돌림
        {
            PlayerState.CurrentPlayerAction = (int)EPlayerCurrentState.Rest;
            PlayerState.CurrentPlayerActionDetails = PlayerState.SaveRestQualityBySuddenAttack;
            PlayerState.SaveRestQualityBySuddenAttack = -1;
        }
        else if(PlayerState.SaveRestQualityBySuddenAttack == -1)
        {
            PlayerState.CurrentPlayerAction = (int)EPlayerCurrentState.SelectAction;
            PlayerState.CurrentPlayerActionDetails = 0;
        }
    }

    public float GetAllEquipTier()//플레이어의 장착한 장비와 인벤토리속 장비의 모든 티어의 합을 리턴함(EQUIP 7레벨 구현을 위함)
    {
        AllEquipmentTier = 0f;

        //현재 플레이어가 끼고 있는 장비
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).EquipmentTier;
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).EquipmentTier;
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).EquipmentTier;
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).EquipmentTier;
        AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).EquipmentTier;

        //현재 인벤토리에 있는 모든 장비
        for(int i = 0; i < PlayerState.EquipmentInventory.Length; i++)
        {
            if(PlayerState.EquipmentInventory[i] >= 1)
            {
                AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipmentInventory[i]).EquipmentTier;
            }
        }

        return AllEquipmentTier;
    }

    public void RecordBeforeShield()//ShieldAmount에 변화가 적용되기전에//맞을때, 방어를 사용할때, 내턴이 됬을때
    {
        BeforeShield = (int)PlayerState.ShieldAmount;
    }

    public void PlayerRecordGiveDamage(float GiveDamage)//몬스터에게 주는 데미지를 기록하기 위함
    {
        PlayerState.GiveDamage += GiveDamage;
        if(PlayerState.MostPowerfulDamage < GiveDamage)
        {
            PlayerState.MostPowerfulDamage = GiveDamage;
        }
    }

    public void PlayerDamage(float DamagePoint)//자신이 받는 데미지를 기록하고 데미지를 받아 쉴드와 체력이 깍일떄
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

    public void PlayerGetShield(float ShieldPoint)//쉴드가 생겼을떄
    {
        RecordBeforeShield();
        PlayerState.ShieldAmount += ShieldPoint;
    }

    public void PlayerRegenSTA(float RestPoint)//전투중 피로도 회복을 할떄
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

    public int ReturnEXPByEXPMagnification(int EXPAmount)//경험치를 얻을때 경험치를 배수함
    {
        return (int)(EXPAmount * JsonReadWriteManager.Instance.GetEarlyState("EXPMG"));
    }

    public void SetPlayerEXPAmount(int EXPAmount, bool IsAlreadyCalculate = false)//경험치를 얻을때 IsAlreadyCalculate의 상태에 따라 경험치를 배수함
    {
        if (IsAlreadyCalculate == true)//다른 곳에서 ReturnEXPByEXPMagnification으로 계산이 됬을때
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
            //사용 EXP를 하면 안될것 같음. upgrade로 한없이 올릴수 있어서....
            //남은 EXP로 변경
        }
        */
    }

    public bool SpendSTA(string ActionType)//배틀중 행동을 하며 피로도를 쓸때 할때
    {
        switch(ActionType)
        {
            case "Attack":
                if(PlayerTotalState.CurrentSTA < EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).SpendTiredness)//피로도가 부족할때
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
                if (PlayerTotalState.CurrentSTA < EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).SpendTiredness)//피로도가 부족할때
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

    public void DefeatFromBattle()//전투에서 졌을떄//지거나 게임에서 이기거나
    {
        int EarlyPoint = 0;
        if (PlayerState.CurrentFloor > JsonReadWriteManager.Instance.E_Info.PlayerReachFloor)
            JsonReadWriteManager.Instance.E_Info.PlayerReachFloor = PlayerState.CurrentFloor;

        EarlyPoint += JsonReadWriteManager.Instance.E_Info.PlayerReachFloor / 5;
        EarlyPoint += (int)(PlayerState.GiveDamage / 1000);
        EarlyPoint += (int)(PlayerState.ReceiveDamage / 500);
        EarlyPoint += (int)(PlayerState.MostPowerfulDamage / 100);
        EarlyPoint += (int)(PlayerState.Experience / 2000);

        JsonReadWriteManager.Instance.E_Info.PlayerEarlyPoint = EarlyPoint;//나머지 레벨들은 초기화 해야할듯? 나중에? 여기서하면 계승이 불가능
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
        //JsonReadWriteManager.Instance.GetEarlyState("EQUIP") -> 이걸로 현재 초기 강화에 따른 플레이어의 열린 인벤토리 수를 알수 있음
        //인벤토리의 코드 값이 0초과이면 장비가 들어있는 칸임 ++한다
        int FullInventoryCount = 0;
        for(int i = 0; i < PlayerState.EquipmentInventory.Length; i++)
        {
            if (PlayerState.EquipmentInventory[i] > 0)//차있을때
            {
                FullInventoryCount++;
            }
        }
        if(JsonReadWriteManager.Instance.GetEarlyState("EQUIP") > FullInventoryCount)
        {//사용가능한 인벤토리 칸이 차있는 인벤토리 칸보다 크다면(다 안찼다면)
            return false;
        }
        else//이건 다 찼다면
        {
            return true;
        }
    }

    public void PutEquipmentToInven(int EquipmentCode)
    {
        if (IsInventoryFull() == true)
            return;

        int InventoryAmount = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");//이게 사용가능한 인벤토리 갯수
        for (int i = 0; i < InventoryAmount; i++)//사용가능 인벤토리 갯수 만큼 반복//0레벨이면 0~3까지 반복함 5레벨 이상이면 0~11까지
        {
            if (PlayerState.EquipmentInventory[i] == 0)//비어있다면
            {
                PlayerState.EquipmentInventory[i] = EquipmentCode;//접근 가능 인벤토리 list에 저장
                break;
            }
        }
    }
}
