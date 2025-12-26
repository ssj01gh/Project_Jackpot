using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Phamtom : Monster
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

    public override void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        base.MonsterGetBuff(i_BuffType, BuffCount);
    }
}
