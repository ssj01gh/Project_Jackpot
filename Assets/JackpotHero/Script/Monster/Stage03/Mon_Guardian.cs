using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Guardian : Monster
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
        int RandNum = Random.Range(0, 2);
        if(RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else
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
        int RandNum = Random.Range(0, 2);
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
