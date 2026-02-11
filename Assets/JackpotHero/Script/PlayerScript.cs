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
    Attack_Battle,
    Defense_Battle,
    STARecovery_Battle,
    Charm_Battle
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

    public float WithOutBuffSTR;
    public float WithOutBuffDUR;
    public float WithOutBuffRES;
    public float WithOutBuffSPD;
    public float WithOutBuffLUK;
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
        //PlayerBuff.BuffList[(int)EBuffType.BloodFamiliy] = 10;
        //PlayerBuff.BuffList[(int)EBuffType.ToughFist] = 20;
        //PlayerBuff.BuffList[(int)EBuffType.EXPPower] = 20;
        //PlayerBuff.BuffList[5] = 6;
        //---------------
    }

    public void SetPlayerTotalStatus()//버프 계산할때 해도 될지두/// 버프에 의한 스탯 변화도 표시해야 될것 같기도 하고
    {
        //SetHP
        PlayerTotalState.MaxHP = BasicHP + JsonReadWriteManager.Instance.GetEarlyState("HP") + (PlayerState.HPLevel * 20) +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddHPAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddHPAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddHPAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddHPAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddHPAmount;
        PlayerTotalState.CurrentHP = PlayerTotalState.MaxHP * PlayerState.CurrentHpRatio;
        //SetSTA
        PlayerTotalState.MaxSTA = BasicSTA + JsonReadWriteManager.Instance.GetEarlyState("STA") + (PlayerState.STALevel * 200) +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddTirednessAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddTirednessAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddTirednessAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddTirednessAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddTirednessAmount;
        PlayerTotalState.CurrentSTA = PlayerTotalState.MaxSTA * PlayerState.CurrentTirednessRatio;
        //SetTotalSTR
        PlayerTotalState.WithOutBuffSTR = BasicSTR + JsonReadWriteManager.Instance.GetEarlyState("STR") +
            PlayerState.StrengthLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddSTRAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddSTRAmount;
        //SetTotalDUR
        PlayerTotalState.WithOutBuffDUR = BasicDUR + JsonReadWriteManager.Instance.GetEarlyState("DUR") +
            PlayerState.DurabilityLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddDURAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddDURAmount;
        //SetTotalRES
        PlayerTotalState.WithOutBuffRES = BasicRES + JsonReadWriteManager.Instance.GetEarlyState("RES") +
            PlayerState.ResilienceLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddRESAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddRESAmount;
        //SetTotalSPD
        PlayerTotalState.WithOutBuffSPD = BasicSPD + JsonReadWriteManager.Instance.GetEarlyState("SPD") +
            PlayerState.SpeedLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddSPDAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddSPDAmount;
        //SetTotalLUK
        PlayerTotalState.WithOutBuffLUK = JsonReadWriteManager.Instance.GetEarlyState("LUK") +
            PlayerState.LuckLevel +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipWeaponCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipArmorCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipHatCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipShoesCode).AddLUKAmount +
            EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerState.EquipAccessoriesCode).AddLUKAmount;

        if(JsonReadWriteManager.Instance.LkEv_Info.TradeWithDevil >= 1)
        {
            PlayerTotalState.WithOutBuffSTR += JsonReadWriteManager.Instance.LkEv_Info.TradeWithDevil;
        }

        if(JsonReadWriteManager.Instance.LkEv_Info.ML_GKPerson == true)
        {
            PlayerTotalState.WithOutBuffSTR += 1;
            PlayerTotalState.WithOutBuffDUR += 1;
            PlayerTotalState.WithOutBuffRES += 1;
            PlayerTotalState.WithOutBuffSPD += 1;
            PlayerTotalState.WithOutBuffLUK += 1;
        }

        PlayerTotalState.TotalSTR = PlayerTotalState.WithOutBuffSTR;
        PlayerTotalState.TotalDUR = PlayerTotalState.WithOutBuffDUR;
        PlayerTotalState.TotalRES = PlayerTotalState.WithOutBuffRES;
        PlayerTotalState.TotalSPD = PlayerTotalState.WithOutBuffSPD;
        PlayerTotalState.TotalLUK = PlayerTotalState.WithOutBuffLUK;

        for(int i = 0; i < (int)EBuffType.CountOfBuff; i++)
        {
            if (PlayerBuff.BuffList[i] < 1)
                continue;

            switch(i)
            {
                case (int)EBuffType.Luck:
                    PlayerTotalState.TotalLUK += 10;
                    break;
                case (int)EBuffType.Misfortune:
                    PlayerTotalState.TotalLUK -= 10;
                    break;
                case (int)EBuffType.OverCharge:
                    PlayerTotalState.TotalSPD += 20;
                    break;
                case (int)EBuffType.CorruptSerum:
                    PlayerTotalState.TotalSTR += 3;
                    PlayerTotalState.TotalDUR += 3;
                    PlayerTotalState.TotalSPD += 3;
                    break;
                case (int)EBuffType.Slow:
                    PlayerTotalState.TotalSPD -= 10;
                    break;
                case (int)EBuffType.Haste:
                    PlayerTotalState.TotalSPD += 10;
                    break;
                case (int)EBuffType.Charging:
                    PlayerTotalState.TotalLUK += (PlayerBuff.BuffList[(int)EBuffType.Charging] * 15);
                    break;
                case (int)EBuffType.GoodKarma:
                    int IncreaseStateByGK = PlayerBuff.BuffList[(int)EBuffType.GoodKarma];
                    PlayerTotalState.TotalSTR += IncreaseStateByGK;
                    PlayerTotalState.TotalDUR += IncreaseStateByGK;
                    PlayerTotalState.TotalRES += IncreaseStateByGK;
                    PlayerTotalState.TotalSPD += IncreaseStateByGK;
                    PlayerTotalState.TotalLUK += IncreaseStateByGK;
                    break;
                case (int)EBuffType.BadKarma:
                    int DecreaseStateByBK = PlayerBuff.BuffList[(int)EBuffType.BadKarma];
                    PlayerTotalState.TotalSTR -= DecreaseStateByBK;
                    PlayerTotalState.TotalDUR -= DecreaseStateByBK;
                    PlayerTotalState.TotalRES -= DecreaseStateByBK;
                    PlayerTotalState.TotalSPD -= DecreaseStateByBK;
                    PlayerTotalState.TotalLUK -= DecreaseStateByBK;
                    break;
                case (int)EBuffType.Envy:
                    int DecreaseStateByEnvy = (int)(PlayerBuff.BuffList[(int)EBuffType.Envy] * 0.3f);
                    PlayerTotalState.TotalSTR -= DecreaseStateByEnvy;
                    PlayerTotalState.TotalDUR -= DecreaseStateByEnvy;
                    PlayerTotalState.TotalRES -= DecreaseStateByEnvy;
                    PlayerTotalState.TotalSPD -= DecreaseStateByEnvy;
                    PlayerTotalState.TotalLUK -= DecreaseStateByEnvy;
                    break;
            }
        }

        if(PlayerTotalState.TotalSTR < 0)
            PlayerTotalState.TotalSTR = 0;
        if(PlayerTotalState.TotalDUR < 0)
            PlayerTotalState.TotalDUR = 0;
        if(PlayerTotalState.TotalRES < 0)
            PlayerTotalState.TotalRES = 0;
        if(PlayerTotalState.TotalSPD < 1)
            PlayerTotalState.TotalSPD = 1;

        //공격의 평균적이 상승량
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
        //방어의 평균적인 상승량
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
        //휴식의 평균적인 상승량
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
    public void SetInitBuff()//나중에 뭐가 더 늘어나면 여기서//몬스터는 Monster쪽 SetInitBuff에서
    {
        //일단 버프 싹 초기화
        for (int i = 0; i < PlayerBuff.BuffList.Length; i++)
        {
            if (PlayerBuff.BuffList[i] >= 1)
            {
                PlayerBuff.BuffList[i] = 0;
            }
        }
        //PlayerBuff.BuffList[(int)EBuffType.ToughFist] = 99;
        //PlayerBuff.BuffList[(int)EBuffType.WeaponMaster] = 99;
        //PlayerBuff.BuffList[(int)EBuffType.Charm] = 99;
        SetInitBuffByEarlyUpgrade();
        /*
        스피드 7레벨에 해당하는 버프는 스폰되는 몬스터에게 적용됨
        if (JsonReadWriteManager.Instance.E_Info.EarlySpeedLevel >= 7)
        */
        SetDefeseResilienceBuff();

        if (PlayerState.SaveRestQualityBySuddenAttack != -1)//습격을 당했다면
        {
            PlayerBuff.BuffList[(int)EBuffType.Defenseless] = 1;
        }
        SetInitBuffByEvent();
        SetInitBuffByKarma();
        SetInitBuffByEquipmnet();
    }
    protected void SetInitBuffByEarlyUpgrade()
    {
        if (JsonReadWriteManager.Instance.E_Info.EarlyStrengthLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.OverWhelmingPower] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyDurabilityLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.ThornArmor] = 99;

        if (JsonReadWriteManager.Instance.E_Info.EarlyResilienceLevel >= 7)
            PlayerBuff.BuffList[(int)EBuffType.AdvancedRest] = 99;

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
    }
    public void SetDefeseResilienceBuff()
    {
        PlayerBuff.BuffList[(int)EBuffType.Defense] = (int)(PlayerTotalState.TotalDUR / 5);
        PlayerBuff.BuffList[(int)EBuffType.Resilience] = (int)(PlayerTotalState.TotalRES / 5);
    }
    protected void SetInitBuffByEvent()
    {
        if (JsonReadWriteManager.Instance.LkEv_Info.PowwersCeremony >= 1)
        {
            PlayerBuff.BuffList[(int)EBuffType.Luck] += 3;
            JsonReadWriteManager.Instance.LkEv_Info.PowwersCeremony -= 1;
        }
        if(JsonReadWriteManager.Instance.LkEv_Info.ForestHut_Regen >= 1)
        {
            PlayerBuff.BuffList[(int)EBuffType.Regeneration] += 3;
            JsonReadWriteManager.Instance.LkEv_Info.ForestHut_Regen -= 1;
        }
        if(JsonReadWriteManager.Instance.LkEv_Info.ForestHut_Poison >= 1)
        {
            PlayerBuff.BuffList[(int)EBuffType.Poison] += 5;
            JsonReadWriteManager.Instance.LkEv_Info.ForestHut_Poison -= 1;
        }
        if(JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin >= 1)
        {
            PlayerBuff.BuffList[(int)EBuffType.Poison] += 10;
            JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin -= 1;
        }
        if(JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin_DisInject >= 1)
        {
            PlayerBuff.BuffList[(int)EBuffType.Poison] += 5;
            JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin_DisInject -= 1;
        }
    }
    protected void SetInitBuffByKarma()
    {
        if(PlayerState.CurrentFloor == 4)
        {
            PlayerBuff.BuffList[(int)EBuffType.GoodKarma] += (int)PlayerState.GoodKarma;
            PlayerBuff.BuffList[(int)EBuffType.BadKarma] += (int)PlayerState.BadKarma;
        }
    }
    protected void SetInitBuffByEquipmnet()
    {
        int STREquip = 10; int DUREquip = 11; int SPDEquip = 13; int LUKEquip = 14; int HPEquip = 15; int STAEquip = 16;

        int BootsTier = (PlayerState.EquipShoesCode / 1000) % 10;
        int IsEventBoots = PlayerState.EquipShoesCode / 10000;
        int BootsStateType = (PlayerState.EquipShoesCode / 100) % 10;
        int BootsType = (IsEventBoots * 10) + BootsStateType;

        int AccTier = (PlayerState.EquipAccessoriesCode / 1000) % 10;
        int IsEventAcc = PlayerState.EquipAccessoriesCode / 10000;
        int AccStateType = (PlayerState.EquipAccessoriesCode / 100) % 10;
        int AccType = (IsEventAcc * 10) + AccStateType;

        if(BootsType == STREquip)
            PlayerBuff.BuffList[(int)EBuffType.OverCharge] += (BootsTier * 2);
        if(BootsType == DUREquip)
            PlayerGetShield(BootsTier * 40);
        if(BootsType == SPDEquip)
            PlayerBuff.BuffList[(int)EBuffType.Haste] += BootsTier;
        if(BootsType == LUKEquip)
            PlayerBuff.BuffList[(int)EBuffType.Luck] += BootsTier;
        if(BootsType == HPEquip)
            PlayerBuff.BuffList[(int)EBuffType.UnDead] += BootsTier;
        if(BootsType == STAEquip)
            PlayerBuff.BuffList[(int)EBuffType.Petrification] += (BootsTier * 2);

        if (AccType == STREquip)
            PlayerBuff.BuffList[(int)EBuffType.ToughFist] += (AccTier * 3);
        if (AccType == DUREquip)
            PlayerBuff.BuffList[(int)EBuffType.ToughSkin] += (AccTier * 3);
        if (AccType == SPDEquip)
            PlayerBuff.BuffList[(int)EBuffType.CorruptSerum] += (AccTier * 3);
        if (AccType == HPEquip)
            PlayerBuff.BuffList[(int)EBuffType.PowerOfDeath] += (AccTier * 3);
        if (AccType == STAEquip)
            PlayerBuff.BuffList[(int)EBuffType.Petrification] += (AccTier * 2);

        if(PlayerState.EquipAccessoriesCode == 26044)
        {
            PlayerBuff.BuffList[(int)EBuffType.Regeneration] += 9;
            PlayerBuff.BuffList[(int)EBuffType.Recharge] += 9;
        }
    }
    public void GetBuffByAttack()
    {
        int HPWeapon = 15;
        int IsEventEquip = PlayerState.EquipWeaponCode / 10000;
        int EquipStateType = (PlayerState.EquipWeaponCode / 100) % 10;
        int WeaponType = (IsEventEquip * 10) + EquipStateType;
        if (WeaponType == HPWeapon)
        {
            PlayerBuff.BuffList[(int)EBuffType.LifeSteal] += 3;
        }
    }
    public void GetBuffByDefense(int DefensePointAmount = 0)
    {
        int STRArmor = 10; int DURArmor = 11; int SPDArmor = 13; int HPArmor = 15;
        int IsEventEquip = PlayerState.EquipArmorCode / 10000;
        int EquipStateType = (PlayerState.EquipArmorCode / 100) % 10;
        int ArmorType = (IsEventEquip * 10) + EquipStateType;
        if (ArmorType == STRArmor)
        {
            int Amout = (int)(DefensePointAmount * 0.05f);
            PlayerBuff.BuffList[(int)EBuffType.UnDead] += Amout;
        }
        if(ArmorType == DURArmor)
        {
            PlayerBuff.BuffList[(int)EBuffType.UnbreakableArmor] += 3;
        }
        if(ArmorType == SPDArmor)
        {
            int Amount = (int)(DefensePointAmount * 0.2f);
            PlayerBuff.BuffList[(int)EBuffType.RegenArmor] += Amount;
        }
        if(ArmorType == HPArmor)
        {
            PlayerBuff.BuffList[(int)EBuffType.Regeneration] += 3;
        }
    }
    public void GetBuffByRest()
    {
        int STRHelmet = 10; int DURHelmet = 11; int RESHelmet = 12; int SPDHelmet = 13;
        int IsEventEquip = PlayerState.EquipHatCode / 10000;
        int EquipStateType = (PlayerState.EquipHatCode / 100) % 10;
        int HelmetType = (IsEventEquip * 10) + EquipStateType;

        if(HelmetType == STRHelmet)
        {
            PlayerBuff.BuffList[(int)EBuffType.Recharge] += 3;
        }
        if(HelmetType == DURHelmet)
        {
            PlayerBuff.BuffList[(int)EBuffType.RegenArmor] += 5;
        }
        if(HelmetType == RESHelmet)
        {
            PlayerBuff.BuffList[(int)EBuffType.Regeneration] += 3;
        }
        if(HelmetType == SPDHelmet)
        {
            PlayerBuff.BuffList[(int)EBuffType.OverCharge] += 3;
        }
    }

    public void SetChainAttackBuff(bool IsAttack, bool IsPlayerTurn)
    {
        int SPDWeapon = 13;
        int IsEventEquip = PlayerState.EquipWeaponCode / 10000;
        int EquipStateType = (PlayerState.EquipWeaponCode / 100) % 10;
        int WeaponType = (IsEventEquip * 10) + EquipStateType;
        //특정 조건이 되면 연속타격 쌓임
        //임시로 52001
        if (WeaponType == SPDWeapon)
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

    public void EndOfAction(bool IsBossBattle = false)//어떠한 행동이 끝났을떄
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
            if (PlayerState.SaveRestQualityBySuddenAttack != -1)//휴식중 습격당했으면 원래 상태로 되돌림
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
    public void RecordKillCount(int KillNormalCount = 0, int KillEliteCount = 0)
    {
        PlayerState.KillNormalMonster += KillNormalCount;
        PlayerState.KillEliteMonster += KillEliteCount;
    }


    public void RecordBeforeShield()//ShieldAmount에 변화가 적용되기전에//맞을때, 방어를 사용할때, 내턴이 됬을때
    {
        BeforeShield = (int)PlayerState.ShieldAmount;
    }

    public void PlayerDamage(float DamagePoint, bool IsIgnoreDefense = false, bool IsSelfDamage = false)//자신이 받는 데미지를 기록하고 데미지를 받아 쉴드와 체력이 깍일떄
    {
        if(DamagePoint > 0)
        {
            if (PlayerState.EquipAccessoriesCode == -1)
            {
                PlayerBuff.BuffList[(int)EBuffType.Reflect] += (int)DamagePoint;
            }
        }

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

        if (IsSelfDamage == true)
        {
            if (PlayerBuff.BuffList[(int)EBuffType.Charm] >= 1)
            {
                PlayerBuff.BuffList[(int)EBuffType.Charm] -= (int)(RestDamage * 0.1f);
                if (PlayerBuff.BuffList[(int)EBuffType.Charm] < 0)
                    PlayerBuff.BuffList[(int)EBuffType.Charm] = 0;
            }
        }

        PlayerTotalState.CurrentHP -= RestDamage;
        PlayerState.CurrentHpRatio = PlayerTotalState.CurrentHP / PlayerTotalState.MaxHP;
        /*
        if(PlayerTotalState.CurrentHP < 1 && PlayerBuff.BuffList[(int)EBuffType.UnDead] >= 1)
        {
            PlayerTotalState.CurrentHP = 1;
            PlayerState.CurrentHpRatio = PlayerTotalState.CurrentHP / PlayerTotalState.MaxHP;
            PlayerBuff.BuffList[(int)EBuffType.UnDead] = 0;
            Vector3 PlayerBuffPos = gameObject.transform.position;
            PlayerBuffPos.y += 1.5f;
            EffectManager.Instance.ActiveEffect("BattleEffect_Buff_UnDead", PlayerBuffPos);
        }
        */
    }
    public void CheckUnDeadBuff()
    {
        if(PlayerTotalState.CurrentHP < 1 && PlayerBuff.BuffList[(int)EBuffType.UnDead] >= 1)
        {
            PlayerTotalState.CurrentHP = 1;
            PlayerState.CurrentHpRatio = PlayerTotalState.CurrentHP / PlayerTotalState.MaxHP;
            PlayerBuff.BuffList[(int)EBuffType.UnDead] = 0;
            Vector3 PlayerBuffPos = gameObject.transform.position;
            PlayerBuffPos.y += 1.5f;
            EffectManager.Instance.ActiveEffect("BattleEffect_Buff_UnDead", PlayerBuffPos);
        }
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
        else if(PlayerTotalState.CurrentHP < 1)
        {
            PlayerTotalState.CurrentHP = 1;
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
            //사용 EXP를 하면 안될것 같음. upgrade로 한없이 올릴수 있어서....
            //남은 EXP로 변경
        }
        */
    }

    public bool SpendSTA(string ActionType)//배틀중 행동을 하며 피로도를 쓸때 할때
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

        if ((ActionType == "Attack" || ActionType == "Defense") && PlayerBuff.BuffList[(int)EBuffType.Petrification] >= 1)
        {
            SpendTired += PlayerBuff.BuffList[(int)EBuffType.Petrification] * 20;
        }

        //피로도 조절은 모든거 뒤에 실행
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

    public void CalculateEarlyPoint()//전투에서 졌을떄//지거나 게임에서 이기거나
    {
        int EarlyPoint = 0;
        if (PlayerState.CurrentFloor > JsonReadWriteManager.Instance.E_Info.PlayerReachFloor)
            JsonReadWriteManager.Instance.E_Info.PlayerReachFloor = PlayerState.CurrentFloor;

        EarlyPoint += JsonReadWriteManager.Instance.E_Info.PlayerReachFloor;
        EarlyPoint += (int)(JsonReadWriteManager.Instance.P_Info.KillNormalMonster / 4);
        EarlyPoint += (int)(JsonReadWriteManager.Instance.P_Info.KillEliteMonster * 7 / 20);
        EarlyPoint += (int)(JsonReadWriteManager.Instance.P_Info.Experience / 2000);
        EarlyPoint += (int)(JsonReadWriteManager.Instance.P_Info.GoodKarma / 10);

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
        PlayerState.HPLevel = UpgradeStatus.AfterHP;
        PlayerState.STALevel = UpgradeStatus.AfterSTA;
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
            case "HP":
                PlayerState.HPLevel += Amount;
                break;
            case "STA":
                PlayerState.STALevel += Amount;
                break;
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
            case (int)EPlayerAnimationState.Attack_Battle:
                if (HeroineAnimator.GetInteger("HeroineState") != (int)EPlayerAnimationState.Attack_Battle)
                {
                    HeroineAnimator.SetInteger("HeroineState", (int)EPlayerAnimationState.Attack_Battle);
                }
                break;
            case (int)EPlayerAnimationState.Defense_Battle:
                if (HeroineAnimator.GetInteger("HeroineState") != (int)EPlayerAnimationState.Defense_Battle)
                {
                    HeroineAnimator.SetInteger("HeroineState", (int)EPlayerAnimationState.Defense_Battle);
                }
                break;
            case (int)EPlayerAnimationState.STARecovery_Battle:
                if (HeroineAnimator.GetInteger("HeroineState") != (int)EPlayerAnimationState.STARecovery_Battle)
                {
                    HeroineAnimator.SetInteger("HeroineState", (int)EPlayerAnimationState.STARecovery_Battle);
                }
                break;
            case (int)EPlayerAnimationState.Charm_Battle:
                if (HeroineAnimator.GetInteger("HeroineState") != (int)EPlayerAnimationState.Charm_Battle)
                {
                    HeroineAnimator.SetInteger("HeroineState", (int)EPlayerAnimationState.Charm_Battle);
                }
                break;
        }
    }
}
