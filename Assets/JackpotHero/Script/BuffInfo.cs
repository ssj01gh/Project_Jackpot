using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBuffType
{
    //방어력
    Defense,
    //회복력
    Resilience,
     //넘치는 힘
    OverWhelmingPower,
    //가시 갑옷
    ThornArmor,
    //무방비
    Defenseless,
    //상급 휴식
    AdvancedRest,
    //피의 일족
    BloodFamiliy,
    //피로도 조절
    TiredControll,
    //착취
    Exploitation,
    //경험이 곧 힘
    EXPPower,
    //웨폰마스터
    WeaponMaster,
    //공깍
    AttackDebuff,
    //방깍
    DefenseDebuff,
    //위축
    Cower,
    //화상
    Burn,
    //독
    Poison,
    //죽음의 저주
    CurseOfDeath,
    //재생형갑옷
    RegenArmor,
    //불굴의갑옷
    UnbreakableArmor,
    //나약함
    Weakness,
    //석화
    Petrification,
    //단단한피부
    ToughSkin,
    //단단한주먹
    ToughFist,
    //행운
    Luck,
    //불운
    Misfortune,
    //도발
    Provocation,
    //재생
    Regeneration,
    //기충전
    Recharge,
    //강탈
    Plunder,
    //연속타격
    ChainAttack,
    //졸개
    Survant,
    //공포
    Fear,
    //산군
    MountainLord,
    //자폭
    SelfDestruct,
    //죽음의 힘
    PowerOfDeath,
    //과부하
    OverCharge,
    //복사_힘
    CopyStrength,
    //복사_내구
    CopyDurability,
    //복사_속도
    CopySpeed,
    //복사_행운
    CopyLuck,
    //흡수
    Consume,
    //도핑
    CorruptSerum,
    //감속
    Slow,
    //신속
    Haste,
    //적응_힘
    StrengthAdaptation,
    //적응_내구
    DurabilityAdaptation,
    //적응_속도
    SpeedAdaptation,
    //매혹
    Charm,
    //축적
    Charging,
    //업보_선
    GoodKarma,
    //업보_악
    BadKarma,
    //반사
    Reflect,
    //교만
    Pride,
    //탐욕
    Greed,
    //질투
    Envy,
    //색욕
    Lust,
    //식탐
    Gluttony,
    //나태
    Sloth,
    //분노
    Wrath,
    //불사//여기 위에 버프추가.
    UnDead,
    //Buff종류에 맞는 BuffList배열 생성을 위해있는것임
    CountOfBuff
}

public class BuffInfo
{
    public int[] BuffList = new int[(int)EBuffType.CountOfBuff];
    public void InitBuff()
    {
        for(int i = 0; i < BuffList.Length; i++)
        {
            BuffList[i] = 0;
        }
    }
    /*
    //넘치는 힘
    public int OverWhelmingPower = 0;
    //가시 갑옷
    public int ThronArmor = 0;
    //기습
    public int SuddenAttack = 0;
    //상급 휴식
    public int AdvancedRest = 0;
    //재생
    public int HealingFactor = 0;
    //기충전
    public int ReCharge = 0;
    //강탈
    public int Rapine = 0;
    //경험이 곧 힘
    public int EXPPower = 0;
    //웨폰마스터
    public int WeaponMaster = 0;
    //공깍
    public int AttackDebuff = 0;
    //방깍
    public int DefenseDebuff = 0;
    //공포
    public int Fear = 0;
    //화상
    public int Burn = 0;
    //독
    public int poison = 0;
    //죽음의 저주
    public int CurseOfDeath = 0;
    //재생형갑옷
    public int RegenArmor = 0;
    //불굴의갑옷
    public int UnbreakableArmor = 0;
    //불사
    public int UnDead = 0;
    //나약함
    public int Weakness = 0;
    //동상
    public int FrostBite = 0;
    //단단한피부
    public int ToughSkin = 0;
    //단단한주먹
    public int ToughFist = 0;
    */
}