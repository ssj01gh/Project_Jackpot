using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_GluttonyServant : Monster
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
        MonsterBuff.BuffList[(int)EBuffType.Servant] = 99;
        MonsterBuff.BuffList[(int)EBuffType.SelfDestruct] = 7;
        int RandNum = Random.Range(0, 2);
        if (RandNum == 0)//공격
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
        int RandNum = Random.Range(0, 2);
        if (RandNum == 0)//공격
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else//방어
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
    }
}
