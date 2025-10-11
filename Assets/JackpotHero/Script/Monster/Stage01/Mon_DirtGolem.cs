using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_DirtGolem : Monster
{
    // Start is called before the first frame update
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
        //MonsterBuff.BuffList[(int)EBuffType.Survant] = 5;
        //MonsterBuff.BuffList[(int)EBuffType.Plunder] = 99;
        //공격 : 방어 = 1 : 4
        //0~4
        int RandNum = Random.Range(0, 5);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if (RandNum >= 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        //MonsterCurrentState = (int)EMonsterActionState.ConsumeGluttony;
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        //공격 : 방어 = 1 : 4
        //0~4
        int RandNum = Random.Range(0, 5);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if (RandNum >= 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        //MonsterCurrentState = (int)EMonsterActionState.ConsumeGluttony;
    }

    /*
    public override List<string> GetSummonMonsters(float SetSummonMonHP = 0, float SetSummonMonSTR = 0, float SetSummonMonDUR = 0,
        float SetSummonMonLUK = 0, float SetSummonMonSPD = 0)
    {
        if (SetSummonMonHP >= 1)
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
