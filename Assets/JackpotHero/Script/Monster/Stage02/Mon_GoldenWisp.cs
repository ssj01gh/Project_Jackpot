using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_GoldenWisp : Monster
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
        MonsterBuff.BuffList[(int)EBuffType.Plunder] = 99;
        MonsterCurrentState = (int)EMonsterActionState.Attack;
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        MonsterCurrentState = (int)EMonsterActionState.Attack;
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        return 0;
    }
}
