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
    protected float AllEquipmentTier = 0f;

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
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddSTRAmount;
        //SetTotalDUR
        PlayerTotalState.TotalDUR = JsonReadWriteManager.Instance.GetEarlyState("DUR") +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddDURAmount;
        //SetTotalRES
        PlayerTotalState.TotalRES = JsonReadWriteManager.Instance.GetEarlyState("RES") +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddRESAmount;
        //SetTotalSPD
        PlayerTotalState.TotalSPD = JsonReadWriteManager.Instance.GetEarlyState("SPD") +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddSPDAmount;
        //SetTotalLUK
        PlayerTotalState.TotalLUK = JsonReadWriteManager.Instance.GetEarlyState("LUK") +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddLUKAmount;
        //SetEXP
        PlayerState.Experience = (int)JsonReadWriteManager.Instance.GetEarlyState("EXP");
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
    }

    public PlayerInfo GetPlayerStateInfo()
    {
        return PlayerState;
    }

    public TotalPlayerState GetTotalPlayerStateInfo()
    {
        return PlayerTotalState;
    }

    public float GetAllEquipTier()
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
            AllEquipmentTier += EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipmentInventory[i]).EquipmentTier;
        }

        return AllEquipmentTier;
    }
}
