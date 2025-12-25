using JetBrains.Annotations;
using System.Collections.Generic;

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
    public int HPLevel;
    public int STALevel;
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
    public float KillNormalMonster;
    public float KillEliteMonster;
    public float GoodKarma;
    public float BadKarma;
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

[System.Serializable]
public class LinkageEventInfo
{
    public bool IsMeetTalkingMonster;
    public int TradeWithDevil;
    public bool TalkingMonster;
    public bool TalkingDirtGolem;
    public bool TotoRepayFavor;
    public bool TotoCursedSword;
    public bool TotoBlessedSword;
    public bool RestInPeace;
    public bool OminousSword;
    public bool CleanOminousSword;
    public int PowwersCeremony;
    public int GreatDevilKillCount;
    public bool ForestBracelet;
    public int ForestHut_Regen;
    public int ForestHut_Poison;
    public bool ML_GKPerson;
    public bool ML_NorPerson;
    public bool ML_BKPerson;
    public bool IsMeetTalkingGiant;
    public int LetTheGameBegin;
    public bool Lab_Security;
    public bool Lab_Sphere;
    public bool ReadyForBattle;
    public bool IsMeetTalkingDopple;
    public int Stage04EventCount;
}

[System.Serializable]
public class TutorialInfo
{
    public bool TitleEarlyStrengthen;
    public bool Research;
    public bool ResearchOpenBag;
    public bool ResearchSelectRest;
    public bool Battle;
    public bool BattlePlayerTurn;
    public bool BattlePlayerTurnMagCard;
    public bool BattleMonsterTurn;
    public bool BattleSuddenAttack;
    public bool Event;
    public bool Camping;
    public bool CampingRest;
    public bool CampingLevelUp;
    public bool CampingEquipment;
}