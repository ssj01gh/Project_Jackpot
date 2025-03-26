using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBuffType
{
     //넘치는 힘
    OverWhelmingPower,
    //가시 갑옷
    ThronArmor,
    //기습
    Defenseless,
    //상급 휴식
    AdvancedRest,
    //재생
    HealingFactor,
    //기충전
    ReCharge,
    //강탈
    Rapine,
    //경험이 곧 힘
    EXPPower,
    //웨폰마스터
    WeaponMaster,
    //공깍
    AttackDebuff,
    //방깍
    DefenseDebuff,
    //공포
    Fear,
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
    //동상
    FrostBite,
    //단단한피부
    ToughSkin,
    //단단한주먹
    ToughFist,
    //행운
    Luck,
    //불행
    Misfortune,
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