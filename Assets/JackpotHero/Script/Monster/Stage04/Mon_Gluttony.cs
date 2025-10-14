using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Gluttony : Monster
{
    enum EGluttonyState
    {
        Action01,
        Action02,
        TryConsume
    }
    int TryConsumePercent = 0;
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
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
    }

    public override List<string> GetSummonMonsters()
    {
        List<string> SummonMonsters = new List<string>();
        for (int i = 0; i < SummonMonsterCount; i++)
        {
            SummonMonsters.Add(CanSummonMonsterIDs[0]);
        }

        return SummonMonsters;
        //return base.GetSummonMonsters();
    }

    public override void MonsterDamage(float DamagePoint)
    {
        if(DamagePoint >= 0)
        {//0포함 양수 일때 -> 식탐 스택으로 저장함
            MonsterBuff.BuffList[(int)EBuffType.Gluttony] += (int)DamagePoint;
        }
        else
        {//음수일때 -> 흡수 실패 -> 진짜로 데미지를 입음
            int RealDamagePoint = -(int)DamagePoint;
            base.MonsterDamage(RealDamagePoint);
        }
    }
}
