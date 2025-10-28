using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;

public enum EPlayerCurrentState
{
    SelectAction,
    Battle,
    OtherEvent,
    Rest,
    Boss_Event,
    Boss_Battle,
}

public enum EPlayerRestQuality
{
    VeryBad,
    Bad,
    Good,
    VeryGood,
    Perfect
}

public enum EEquipTier
{
    TierZero,
    TierOne,
    TierTwo,
    TierThree,
    TierFour,
    TierFive,
    TierSix,
    TierEvent
}
public enum EEquipStateType
{
    StateSTR,
    StateDUR,
    StateRES,
    StateSPD,
    StateLUK,
    StateHP,
    StateSTA,
    StateNormal
}
public enum EEquipType
{
    TypeWeapon,
    TypeArmor,
    TypeHelmet,
    TypeBoots,
    TypeAcc,
}
public enum EEquipMultiType
{
    MultiSteady,
    MultiBalanced,
    MultiVolatile,
}

public class JsonReadWriteManager : MonoSingletonDontDestroy<JsonReadWriteManager>
{
    [HideInInspector]
    public PlayerInfo P_Info;
    [HideInInspector]
    public EarlyStrengthenInfo E_Info;
    [HideInInspector]
    public OptionInfo O_Info;
    [HideInInspector]
    public LinkageEventInfo LkEv_Info;

    protected int[] EarlyState_SDRSL = new int[8] { 0, 1, 2, 3, 4, 5, 5, 5 };
    protected int[] EarlyState_HP = new int[8] { 0, 20, 40, 60, 100, 100, 100, 100 };
    protected int[] EarlyState_STA = new int[8] { 0, 200, 400, 600, 800, 1000, 1000, 1000 };
    protected int[] EarlyState_EXP = new int[8] { 0, 30, 60, 90, 120, 150, 150, 150 };
    protected float[] EarlyState_EXPMG = new float[8] { 1f, 1.05f, 1.1f, 1.15f, 1.2f, 1.25f, 1.25f, 1.25f };
    protected int[] EarlyState_EquipInven = new int[8] { 4, 6, 8, 10, 12, 12, 12, 12 };
    protected int[] EarlyState_EquipSuccession = new int[8] { 0, 0, 1, 2, 3, 4, 4, 4 };
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        InitPlayerInfo();
        InitEarlyStrengthenInfo();
        InitOptionInfo();
        InitLinkageEventInfo();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    //이 함수는 게임을 새로 시작할때마다 계속 불러와질 예정
    public void InitPlayerInfo(bool IsRestartGame = false)
    {
        string FileName = "PlayerInfo";
        string path = Application.persistentDataPath + "/" + FileName + ".json";
        if (!File.Exists(path) || IsRestartGame)//없으면 생성
        {
            //Player Equipment Setting
            P_Info.EquipWeaponCode = 10001;
            P_Info.EquipArmorCode = 20001;
            P_Info.EquipHatCode = 30001;
            P_Info.EquipShoesCode = 40001;
            P_Info.EquipAccessoriesCode = 50001;
            //Player State Setting
            P_Info.CurrentHpRatio = 1f;
            P_Info.CurrentTirednessRatio = 1f;
            P_Info.ShieldAmount = 0f;
            P_Info.Level = 0;
            P_Info.HPLevel = 0;
            P_Info.STALevel = 0;
            P_Info.StrengthLevel = 0;
            P_Info.DurabilityLevel = 0;
            P_Info.SpeedLevel = 0;
            P_Info.ResilienceLevel = 0;
            P_Info.LuckLevel = 0;
            //Player Inventory Setting
            P_Info.Experience = 0;
            P_Info.EquipmentGamblingLevel = 0;
            P_Info.EquipmentInventory = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//0이하는 인벤토리가 비어있는거임
            P_Info.CurrentFloor = 0;
            P_Info.DetectNextFloorPoint = 0;
            //Player Current Action Type Setting
            P_Info.CurrentPlayerAction = (int)EPlayerCurrentState.SelectAction;
            P_Info.CurrentPlayerActionDetails = 0;
            //PlayerRecordSetting
            P_Info.KillNormalMonster = 0f;
            P_Info.KillEliteMonster = 0f;
            P_Info.GoodKarma = 0f;
            P_Info.BadKarma = 0f;
            P_Info.SaveRestQualityBySuddenAttack = -1;

            string classToJson = JsonUtility.ToJson(P_Info, true);
            File.WriteAllText(path, classToJson);
        }
        //불러오기
        P_Info = JsonUtility.FromJson<PlayerInfo>(File.ReadAllText(path));
    }
    public void InitEarlyStrengthenInfo(bool IsRestartGame = false)
    {
        string FileName = "EarlyStrengthenInfo";
        string path = Application.persistentDataPath + "/" + FileName + ".json";
        if (!File.Exists(path) || IsRestartGame == true)//없으면 생성
        {
            //Player EarlyStrenghenInfo Setting
            if (IsRestartGame == false)
            {
                E_Info.PlayerReachFloor = 0;
                E_Info.PlayerEarlyPoint = 0;
            }//나머지는 다 초기화//IsRestartGame이 true이 상황에서 PlyaerReachFloor와 PlyaerEarlyPoint의 수치는 
            //PlayerScript의 DefeatformBattle에서 설정된다.나머지는 초기화하고 저장시키면 됨
            E_Info.EarlyStrengthLevel = 0;
            E_Info.EarlyDurabilityLevel = 0;
            E_Info.EarlySpeedLevel = 0;
            E_Info.EarlyResilienceLevel = 0;
            E_Info.EarlyLuckLevel = 0;
            E_Info.EarlyHpLevel = 0;
            E_Info.EarlyTirednessLevel = 0;
            E_Info.EarlyExperience = 0;
            E_Info.EarlyExperienceMagnification = 0;
            E_Info.EquipmentSuccessionLevel = 0;

            string classToJson = JsonUtility.ToJson(E_Info, true);
            File.WriteAllText(path, classToJson);
        }
        //불러오기
        E_Info = JsonUtility.FromJson<EarlyStrengthenInfo>(File.ReadAllText(path));
    }

    protected void InitOptionInfo()
    {
        string FileName = "OptionInfo";
        string path = Application.persistentDataPath + "/" + FileName + ".json";
        if (!File.Exists(path))//없으면 생성
        {
            //OptinInfo Setting
            O_Info.MasterVolume = 0.8f;
            O_Info.BGMVolume = 0.8f;
            O_Info.SFXVolume = 0.8f;
            O_Info.UISFXVolume = 0.8f;
            O_Info.ScreenResolutionWidth = 1920f;
            O_Info.IsFullScreen = false;

            string classToJson = JsonUtility.ToJson(O_Info, true);
            File.WriteAllText(path, classToJson);
        }
        //불러오기
        O_Info = JsonUtility.FromJson<OptionInfo>(File.ReadAllText(path));
    }

    public void InitLinkageEventInfo(bool IsRestartGame = false)
    {
        string FileName = "LinkageEventInfo";
        string path = Application.persistentDataPath + "/" + FileName + ".json";
        if(!File.Exists(path) || IsRestartGame == true)
        {
            LkEv_Info.TalkingMonster = false;
            LkEv_Info.TalkingDirtGolem = false;
            LkEv_Info.TotoRepayFavor = false;
            LkEv_Info.TotoCursedSword = false;
            LkEv_Info.TotoBlessedSword = false;
            LkEv_Info.RestInPeace = false;
            LkEv_Info.OminousSword = false;
            LkEv_Info.CleanOminousSword = false;
            LkEv_Info.PowwersCeremony = 0;

            string classToJson = JsonUtility.ToJson(LkEv_Info, true);
            File.WriteAllText(path, classToJson);
        }

        LkEv_Info = JsonUtility.FromJson<LinkageEventInfo>(File.ReadAllText(path));
    }

    public PlayerInfo GetCopyPlayerInfo()
    {
        return JsonUtility.FromJson<PlayerInfo>(JsonUtility.ToJson(P_Info));
    }

    public void SavePlayerInfo(PlayerInfo PInfo)
    {
        P_Info = new PlayerInfo
        {
            EquipWeaponCode = PInfo.EquipWeaponCode,
            EquipArmorCode = PInfo.EquipArmorCode,
            EquipShoesCode = PInfo.EquipShoesCode,
            EquipHatCode = PInfo.EquipHatCode,
            EquipAccessoriesCode = PInfo.EquipAccessoriesCode,
            ShieldAmount = PInfo.ShieldAmount,
            CurrentHpRatio = PInfo.CurrentHpRatio,
            CurrentTirednessRatio = PInfo.CurrentTirednessRatio,
            Level = PInfo.Level,
            HPLevel = PInfo.HPLevel,
            STALevel = PInfo.STALevel,
            StrengthLevel = PInfo.StrengthLevel,
            DurabilityLevel = PInfo.DurabilityLevel,
            SpeedLevel = PInfo.SpeedLevel,
            ResilienceLevel = PInfo.ResilienceLevel,
            LuckLevel = PInfo.LuckLevel,
            Experience = PInfo.Experience,
            EquipmentGamblingLevel = PInfo.EquipmentGamblingLevel,
            EquipmentInventory = new int[12]
            { PInfo.EquipmentInventory[0], PInfo.EquipmentInventory[1], PInfo.EquipmentInventory[2], PInfo.EquipmentInventory[3],
                PInfo.EquipmentInventory[4], PInfo.EquipmentInventory[5], PInfo.EquipmentInventory[6], PInfo.EquipmentInventory[7],
                PInfo.EquipmentInventory[8], PInfo.EquipmentInventory[9], PInfo.EquipmentInventory[10], PInfo.EquipmentInventory[11] },
            CurrentFloor = PInfo.CurrentFloor,
            DetectNextFloorPoint = PInfo.DetectNextFloorPoint,
            CurrentPlayerAction = PInfo.CurrentPlayerAction,
            CurrentPlayerActionDetails = PInfo.CurrentPlayerActionDetails,
            KillNormalMonster = PInfo.KillNormalMonster,
            KillEliteMonster = PInfo.KillEliteMonster,
            GoodKarma = PInfo.GoodKarma,
            BadKarma = PInfo.BadKarma,
            SaveRestQualityBySuddenAttack = PInfo.SaveRestQualityBySuddenAttack
        };
    }
    public EarlyStrengthenInfo GetCopyEarlyInfo()
    {
        return JsonUtility.FromJson<EarlyStrengthenInfo>(JsonUtility.ToJson(E_Info));
    }
    public void SaveEarlyInfo(EarlyStrengthenInfo EInfo)
    {
        E_Info = new EarlyStrengthenInfo
        {
            PlayerReachFloor = EInfo.PlayerReachFloor,
            PlayerEarlyPoint = EInfo.PlayerEarlyPoint,
            EarlyStrengthLevel = EInfo.EarlyStrengthLevel,
            EarlyDurabilityLevel = EInfo.EarlyDurabilityLevel,
            EarlySpeedLevel = EInfo.EarlySpeedLevel,
            EarlyResilienceLevel = EInfo.EarlyResilienceLevel,
            EarlyLuckLevel = EInfo.EarlyLuckLevel,
            EarlyHpLevel = EInfo.EarlyHpLevel,
            EarlyTirednessLevel = EInfo.EarlyTirednessLevel,
            EarlyExperience = EInfo.EarlyExperience,
            EarlyExperienceMagnification = EInfo.EarlyExperienceMagnification,
            EquipmentSuccessionLevel = EInfo.EquipmentSuccessionLevel
        };
    }
    /*
    public OptionInfo GetCopyOptionInfo()
    {
        return JsonUtility.FromJson<OptionInfo>(JsonUtility.ToJson(O_Info));
    }

    public void SaveOptionInfo(OptionInfo OInfo)
    {
        O_Info = new OptionInfo
        {
            MasterVolume = OInfo.MasterVolume,
            BGMVolume = OInfo.BGMVolume,
            SFXVolume = OInfo.SFXVolume,
            UISFXVolume = OInfo.UISFXVolume,
            ScreenResolutionWidth = OInfo.ScreenResolutionWidth,
            IsFullScreen = OInfo.IsFullScreen
        };
    }
    */
    public float GetEarlyState(string EarlyType)
    {
        switch(EarlyType)
        {
            case "STR":
                return EarlyState_SDRSL[E_Info.EarlyStrengthLevel];
            case "DUR":
                return EarlyState_SDRSL[E_Info.EarlyDurabilityLevel];
            case "RES":
                return EarlyState_SDRSL[E_Info.EarlyResilienceLevel];
            case "SPD":
                return EarlyState_SDRSL[E_Info.EarlySpeedLevel];
            case "LUK":
                return EarlyState_SDRSL[E_Info.EarlyLuckLevel];
            case "HP":
                return EarlyState_HP[E_Info.EarlyHpLevel];
            case "STA":
                return EarlyState_STA[E_Info.EarlyTirednessLevel];
            case "EXP":
                return EarlyState_EXP[E_Info.EarlyExperience];
            case "EXPMG":
                return EarlyState_EXPMG[E_Info.EarlyExperienceMagnification];
            case "EQUIP":
                return EarlyState_EquipInven[E_Info.EquipmentSuccessionLevel];
            case "EQUIPSUC":
                return EarlyState_EquipSuccession[E_Info.EquipmentSuccessionLevel];
        }
        return 0;
    }

    private void OnApplicationQuit()
    {
        string FileName = "PlayerInfo";
        string path = Application.persistentDataPath + "/" + FileName + ".json";
        string classToJson = JsonUtility.ToJson(P_Info, true);
        File.WriteAllText(path, classToJson);

        FileName = "EarlyStrengthenInfo";
        path = Application.persistentDataPath + "/" + FileName + ".json";
        classToJson = JsonUtility.ToJson(E_Info, true);
        File.WriteAllText(path, classToJson);

        FileName = "OptionInfo";
        path = Application.persistentDataPath + "/" + FileName + ".json";
        classToJson = JsonUtility.ToJson(O_Info, true);
        File.WriteAllText(path, classToJson);

        FileName = "LinkageEventInfo";
        path = Application.persistentDataPath + "/" + FileName + ".json";
        classToJson = JsonUtility.ToJson(LkEv_Info, true);
        File.WriteAllText(path, classToJson);
    }
}
