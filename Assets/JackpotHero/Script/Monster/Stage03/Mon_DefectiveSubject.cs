using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_DefectiveSubject : Monster
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
        //초기에는 공격 혹은 방어로
        MonsterBuff.BuffList[(int)EBuffType.SelfDestruct] = 5;
        int RandNum = Random.Range(0, 3);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }

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
        int RandNum = Random.Range(0, 3);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
    }
}
