using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_ShortLegBird : Monster
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
        //MonsterBuff.BuffList[(int)EBuffType.Sloth] = 4;
        //공격 : 방어 = 1 : 1
        
        int RandNum = Random.Range(0, 2);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if (RandNum == 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        
        //MonsterCurrentState = (int)EMonsterActionState.GiveEnvy;
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        //공격 : 방어 = 1 : 1
        int RandNum = Random.Range(0, 5);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if (RandNum == 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
    }
    /*
    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.Cower)
        {
            return base.MonsterGiveBuff(i_BuffType, 2);
        }
        else
        {
            return 0;
        }
    }
    */
    /*
    public override List<string> GetSummonMonsters(float SetSummonMonHP = 0, float SetSummonMonSTR = 0, float SetSummonMonDUR = 0,
        float SetSummonMonLUK = 0, float SetSummonMonSPD = 0)
    {
        if(SetSummonMonHP >= 1)
        {
            SummonMonHP = (int)SetSummonMonHP;
            SummonMonSTR = (int)SetSummonMonSTR;
            SummonMonDUR = (int)SetSummonMonDUR;
            SummonMonLUK = (int)SetSummonMonLUK;
            SummonMonSPD = (int)SetSummonMonSPD;
        }
        List<string> SummonMonsters = new List<string>();
        for (int i = 0; i < SummonMonsterCount; i++)
        {
            SummonMonsters.Add(CanSummonMonsterIDs[0]);
        }

        return SummonMonsters;
        //return base.GetSummonMonsters();
    }
    */
    
}
