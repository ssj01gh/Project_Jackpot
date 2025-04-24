using JetBrains.Annotations;

[System.Serializable]
public class PlayerInfo
{
    public int EquipWeaponCode;
    public int EquipArmorCode;
    public int EquipShoesCode;
    public int EquipHatCode;
    public int EquipAccessoriesCode;
    public float ShieldAmount;
    public float CurrentHpRatio;
    public float CurrentTirednessRatio;
    public int Level;
    public int StrengthLevel;
    public int DurabilityLevel;
    public int SpeedLevel;
    public int ResilienceLevel;
    public int LuckLevel;
    public int Experience;
    public int EquipmentGamblingLevel;
    public int[] EquipmentInventory;
    public int CurrentFloor;
    public int DetectNextFloorPoint;
    public int CurrentPlayerAction;
    public int CurrentPlayerActionDetails;
    public float GiveDamage;
    public float ReceiveDamage;
    public float MostPowerfulDamage;
    public float Karma;
    public int SaveRestQualityBySuddenAttack;
}

[System.Serializable]
public class EarlyStrengthenInfo
{
    public int PlayerReachFloor;
    public int PlayerEarlyPoint;
    public int EarlyStrengthLevel;
    public int EarlyDurabilityLevel;
    public int EarlySpeedLevel;
    public int EarlyResilienceLevel;
    public int EarlyLuckLevel;
    public int EarlyHpLevel;
    public int EarlyTirednessLevel;
    public int EarlyExperience;
    public int EarlyExperienceMagnification;
    public int EquipmentSuccessionLevel;
}
//힘, 내구, 속도, 회복, 행운

[System.Serializable]
public class OptionInfo
{
    public float MasterVolume;
    public float BGMVolume;
    public float SFXVolume;
    public float UISFXVolume;
    public float ScreenResolutionWidth;
    public bool IsFullScreen;
}