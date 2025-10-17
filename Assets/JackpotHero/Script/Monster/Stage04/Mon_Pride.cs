using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Pride : Monster
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
        SetPrideState();
        SetPrideStack();
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        SetPrideState();
    }

    private void SetPrideState()
    {
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

    public override void MonsterDamage(float DamagePoint)
    {
        if (DamagePoint > 0)
        {
            MonsterBuff.BuffList[(int)EBuffType.Reflect] += (int)DamagePoint;
        }
        base.MonsterDamage(DamagePoint);
        //프라이드 수치가 풀체력 일때 = 10 체력이 1일때 = 90
        SetPrideStack();
    }

    private void SetPrideStack()
    {
        float HpRatio = (MonTotalStatus.MonsterCurrentHP - 1) / (MonTotalStatus.MonsterMaxHP - 1);
        int PrideStack = (int)Mathf.Lerp(90, 10, HpRatio);
        MonsterBuff.BuffList[(int)EBuffType.Pride] = PrideStack;
    }
}
