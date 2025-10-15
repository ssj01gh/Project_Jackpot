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
    int GluttonyNextActionState = (int)EGluttonyState.Action01;
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
        GluttonyNormalAction();
        GluttonyNextActionState = (int)EGluttonyState.Action02;
        TryConsumePercent = 0;
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        switch(GluttonyNextActionState)
        {
            case (int)EGluttonyState.Action01:
                GluttonyNormalAction();
                GluttonyNextActionState = (int)EGluttonyState.Action02;
                TryConsumePercent = 0;
                break;
            case (int)EGluttonyState.Action02:
                GluttonyNormalAction();
                GluttonyNextActionState = (int)EGluttonyState.TryConsume;
                TryConsumePercent += Random.Range(5, 16);
                break;
            case (int)EGluttonyState.TryConsume:
                int RandomConsumeNum = Random.Range(1, 101);
                if(RandomConsumeNum > TryConsumePercent || MonsterBuff.BuffList[(int)EBuffType.Gluttony] <= 0)
                {//TryConsumePercent가 점점 높아지면 흡수 시도할 확률도 높아 져야 함
                    //여기는 흡수 안하는 곳 -> TryConsumePercent가 낮을때 확률이 높은곳
                    //혹은 Gluttony버프 스택이 0일때
                    GluttonyNormalAction();
                    TryConsumePercent += Random.Range(5, 16);
                }
                else
                {//여기가 흡수 하는곳
                    MonsterCurrentState = (int)EMonsterActionState.ConsumeGluttony;
                    TryConsumePercent = 0;
                    GluttonyNextActionState = (int)EGluttonyState.Action01;
                }
                break;
            default:
                break;
        }
    }

    private void GluttonyNormalAction()
    {
        int RandNum = Random.Range(0, 3);
        if(RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        else
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
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
        {//음수일때 -> 흡수 실패 -> 진짜로 데미지를 입음 // 스택도 초기화
            MonsterBuff.BuffList[(int)EBuffType.Gluttony] = 0;
            int RealDamagePoint = -(int)DamagePoint;
            base.MonsterDamage(RealDamagePoint);
        }
    }
}
