using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonReadWriteManager : MonoSingletonDontDestroy<JsonReadWriteManager>
{
    [HideInInspector]
    public PlayerInfo P_Info;
    [HideInInspector]
    public EarlyStrengthenInfo E_Info;
    [HideInInspector]
    public OptionInfo O_Info;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        InitPlayerInfo();
        InitEarlyStrengthenInfo();
        InitOptionInfo();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //이 함수는 게임을 새로 시작할때마다 계속 불러와질 예정
    protected void InitPlayerInfo(bool IsRestartGame = false)
    {
        string FileName = "PlayerInfo";
        string path = Application.persistentDataPath + "/" + FileName + ".json";
        if (!File.Exists(path) || IsRestartGame)//없으면 생성
        {
            //Player Equipment Setting
            P_Info.EquipWeaponCode = 10000;
            P_Info.EquipArmorCode = 20000;
            P_Info.EquipShoesCode = 30000;
            P_Info.EquipHatCode = 40000;
            P_Info.EquipAccessoriesCode = 50000;
            //Player State Setting
            P_Info.CurrentHpRatio = 1f;
            P_Info.CurrentTirednessRatio = 1f;
            P_Info.Level = 0;
            P_Info.StrengthLevel = 0;
            P_Info.DurabilityLevel = 0;
            P_Info.SpeedLevel = 0;
            P_Info.ResilienceLevel = 0;
            P_Info.LuckLevel = 0;
            //Player Inventory Setting
            P_Info.Experience = 0;
            P_Info.EquipmentGamblingLevel = 0;
            P_Info.EquipmentInventory = new int[12] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};//0이하는 인벤토리가 비어있는거임
            P_Info.CurrentFloor = 0;
            P_Info.DetectNextFloorPoint = 0;

            string classToJson = JsonUtility.ToJson(P_Info, true);
            File.WriteAllText(path, classToJson);
        }
        //불러오기
        P_Info = JsonUtility.FromJson<PlayerInfo>(File.ReadAllText(path));
    }
    protected void InitEarlyStrengthenInfo()
    {
        string FileName = "EarlyStrengthenInfo";
        string path = Application.persistentDataPath + "/" + FileName + ".json";
        if (!File.Exists(path))//없으면 생성
        {
            //Player EarlyStrenghenInfo Setting
            E_Info.PlayerReachFloor = 0;
            E_Info.PlayerEarlyPoint = 0;
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
            O_Info.MasterVolume = 0.7f;
            O_Info.BGMVolume = 0.7f;
            O_Info.SFXVolume = 0.7f;
            O_Info.UISFXVolume = 0.7f;
            O_Info.ScreenResolutionWidth = 1920f;
            O_Info.IsFullScreen = false;

            string classToJson = JsonUtility.ToJson(O_Info, true);
            File.WriteAllText(path, classToJson);
        }
        //불러오기
        O_Info = JsonUtility.FromJson<OptionInfo>(File.ReadAllText(path));
    }
}
