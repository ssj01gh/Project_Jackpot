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
        //MonsterBuff.BuffList[(int)EBuffType.Charm] = 99;
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
            //버프의 성능이 3.3배 올랐으니 주는 스택이 1/3이 되어야함
            int RandomPercent = Random.Range(10, 16);//2~7->이러면 거의 안줄어드는데.....들어올 수 있는 BuffCount = 0 ~ 30 15정도가 보통이라고 생각하면.... 10% ~ 15%
            int GiveEnvyStack = (int)(BuffCount * 0.01 * RandomPercent);
            return base.MonsterGiveBuff(i_BuffType, GiveEnvyStack);
        }


        return 0;
    }
}
