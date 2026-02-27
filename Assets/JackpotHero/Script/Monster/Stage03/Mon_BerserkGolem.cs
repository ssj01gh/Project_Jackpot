using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_BerserkGolem : Monster
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
        MonsterBuff.BuffList[(int)EBuffType.OverCharge] = 5;
        int RandNum = Random.Range(0, 2);
        MonsterCurrentState = (int)EMonsterActionState.Attack;
        /*
        MonsterCurrentState = (int)EMonsterActionState.SpawnMonster;
        MonsterCurrentState = (int)EMonsterActionState.ApplyRegeneration;
        */
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
}
