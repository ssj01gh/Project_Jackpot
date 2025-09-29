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
        //���� : ��� = 1 : 1
        int RandNum = Random.Range(0, 2);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if (RandNum == 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        //MonsterCurrentState = (int)EMonsterActionState.SpawnMonster;
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        //���� : ��� = 1 : 1
        int RandNum = Random.Range(0, 5);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if (RandNum == 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        //MonsterCurrentState = (int)EMonsterActionState.SpawnMonster;
    }

    /*
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
    */
}
