using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_ScreamSubject : Monster
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
        MonsterBuff.BuffList[(int)EBuffType.SelfDestruct] = 5;
        int RandNum = Random.Range(0, 3);
        if (RandNum == 0)//위축
        {
            MonsterCurrentState = (int)EMonsterActionState.GiveCower;
        }
        else if (RandNum == 1)//공격
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else//방어
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
        int RandNum = Random.Range(0, 3);
        if (RandNum == 0)//위축
        {
            MonsterCurrentState = (int)EMonsterActionState.GiveCower;
        }
        else if (RandNum == 1)//공격
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else//방어
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.Cower)
        {
            return base.MonsterGiveBuff(i_BuffType, 2);
        }
        else
        {
            return 0;
        }
    }
}
