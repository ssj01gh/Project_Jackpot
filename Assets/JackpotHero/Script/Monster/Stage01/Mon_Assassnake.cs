using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Assassnake : Monster
{
    protected int EnemyPoisonCount = 0;
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
        //PoisonCount 수치에 따라 일반행동과 독주입의 확률이 달라짐
        //PoisonCount = 10이상일때 일반행동 확률 = 100%
        //공격 : 방어 = 1 : 1
        EnemyPoisonCount = 0;
        int RandNum = Random.Range(0, 11);
        if(RandNum > EnemyPoisonCount)//적의 독 수치가 작을 수록 확률 이쪽 확률 증가
        {
            MonsterCurrentState = (int)EMonsterActionState.GivePoison;
        }
        else//이쪽은 일반 행동
        {
            RandNum = Random.Range(0, 2);
            if (RandNum == 0)
            {
                MonsterCurrentState = (int)EMonsterActionState.Attack;
            }
            else if (RandNum == 1)
            {
                MonsterCurrentState = (int)EMonsterActionState.Defense;
            }
        }
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
        EnemyPoisonCount = EnemyBuff.BuffList[(int)EBuffType.Poison];
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        //PoisonCount 수치에 따라 일반행동과 독주입의 확률이 달라짐
        //PoisonCount = 10이상일때 일반행동 확률 = 100%
        //공격 : 방어 = 1 : 1
        int RandNum = Random.Range(0, 11);
        Debug.Log(EnemyPoisonCount);
        if (RandNum > EnemyPoisonCount)//적의 독 수치가 작을 수록 확률 이쪽 확률 증가
        {
            MonsterCurrentState = (int)EMonsterActionState.GivePoison;
        }
        else//이쪽은 독 주입임
        {
            RandNum = Random.Range(0, 2);
            if (RandNum == 0)
            {
                MonsterCurrentState = (int)EMonsterActionState.Attack;
            }
            else if (RandNum == 1)
            {
                MonsterCurrentState = (int)EMonsterActionState.Defense;
            }
        }
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.Poison)
            return base.MonsterGiveBuff(i_BuffType, 4);
        else
            return 0;
    }
}
