using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_SpiritWolf : Monster
{
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void InitMonsterState()
    {
        base.InitMonsterState();
        //MonsterBuff.BuffList[(int)EBuffType.Plunder] = 99;
        int RandNum = Random.Range(0, 4);
        if(RandNum == 0)//¹æ±ï
        {
            MonsterCurrentState = (int)EMonsterActionState.GiveDefenseDebuff;
        }
        else if(RandNum >= 1 && RandNum < 3)//°ø°İ
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else//¹æ¾î
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        int RandNum = Random.Range(0, 4);
        if (RandNum == 0)//¹æ±ï
        {
            MonsterCurrentState = (int)EMonsterActionState.GiveDefenseDebuff;
        }
        else if (RandNum >= 1 && RandNum < 3)//°ø°İ
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else//¹æ¾î
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.DefenseDebuff)
        {
            return base.MonsterGiveBuff(i_BuffType, 2);
        }
        else
        {
            return 0;
        }
    }
}
