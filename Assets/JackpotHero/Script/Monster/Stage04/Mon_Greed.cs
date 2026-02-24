using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Greed : Monster
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
        //MonsterBuff.BuffList[(int)EBuffType.Charm] = 99;
        MonsterCurrentState = (int)EMonsterActionState.ApplyGreed;
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        int RandomNum = Random.Range(0, 3);
        if(RandomNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if(RandomNum == 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        else
        {
            MonsterCurrentState = (int)EMonsterActionState.ApplyGreed;
        }
    }

    public override void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        if(i_BuffType == (int)EBuffType.Greed)
        {
            int RandomPercent = Random.Range(5, 21);
            int GreedStack = (int)(BuffCount * 0.01 * RandomPercent);
            base.MonsterGetBuff(i_BuffType, GreedStack);
        }
    }
}
