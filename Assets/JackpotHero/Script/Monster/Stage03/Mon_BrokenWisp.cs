using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_BrokenWisp : Monster
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
        MonsterBuff.BuffList[(int)EBuffType.UnDead] = 99;
        MonsterBuff.BuffList[(int)EBuffType.PowerOfDeath] = 99;
        //초기에는 공격 혹은 방어로
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

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        if (MonsterBuff.BuffList[(int)EBuffType.UnDead] >= 1)
        {//불사가 남아있을때
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
        else
        {//불사가 안남아 있을때
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
    }
}
