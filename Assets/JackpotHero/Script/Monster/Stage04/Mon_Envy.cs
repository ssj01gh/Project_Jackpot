using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Envy : Monster
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
        MonsterCurrentState = (int)EMonsterActionState.GiveEnvy;
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        int RandomNum = Random.Range(0, 3);
        if (RandomNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if (RandomNum == 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        else
        {
            MonsterCurrentState = (int)EMonsterActionState.GiveEnvy;
        }
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if(i_BuffType == (int)EBuffType.Envy)
        {
            int RandomPercent = Random.Range(5, 21);
            int GiveEnvyStack = (int)(BuffCount * 0.01 * RandomPercent);
            return base.MonsterGiveBuff(i_BuffType, GiveEnvyStack);
        }


        return 0;
    }
}
