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

    public override void MonsterDamage(float DamagePoint)
    {
        float RestDamage = 0;
        if (MonsterBuff.BuffList[(int)EBuffType.Defense] >= 1)
            DamagePoint -= MonsterBuff.BuffList[(int)EBuffType.Defense];

        if (DamagePoint < 0)
            DamagePoint = 0;

        if (MonsterBuff.BuffList[(int)EBuffType.Defenseless] >= 1)
            DamagePoint = DamagePoint * 2;

        if (MonTotalStatus.MonsterCurrentShieldPoint >= DamagePoint)
        {
            RecordMonsterBeforeShield();
            MonTotalStatus.MonsterCurrentShieldPoint -= DamagePoint;
        }
        else
        {
            RestDamage = DamagePoint - MonTotalStatus.MonsterCurrentShieldPoint;
            RecordMonsterBeforeShield();
            MonTotalStatus.MonsterCurrentShieldPoint = 0;
        }

        if (RestDamage >= 1)
        {//적응 내구 증가
            MonsterBuff.BuffList[(int)EBuffType.DurabilityAdaptation] += 1;
        }

        MonTotalStatus.MonsterCurrentHP -= RestDamage;
    }
}
