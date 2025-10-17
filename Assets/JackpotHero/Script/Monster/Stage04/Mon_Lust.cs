using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Lust : Monster
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
        MonsterBuff.BuffList[(int)EBuffType.Lust] = 99;
        MonsterCurrentState = (int)EMonsterActionState.GiveCharm;
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
            MonsterCurrentState = (int)EMonsterActionState.GiveCharm;
        }
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.Charm)
        {
            int RandomCharmStack = Random.Range(1, 4);
            return base.MonsterGiveBuff(i_BuffType, RandomCharmStack);
        }


        return 0;
    }
}
