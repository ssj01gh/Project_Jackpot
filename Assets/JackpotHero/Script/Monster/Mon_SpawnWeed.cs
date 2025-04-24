using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_SpawnWeed : Monster
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
        MonsterCurrentState = (int)EMonsterActionState.SpawnMonster;
        /*
        int RandNum = Random.Range(0, 3);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if (RandNum == 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        else if(RandNum == 2)
        {
            
        }
        */
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        MonsterCurrentState = (int)EMonsterActionState.SpawnMonster;
        /*
        int RandNum = Random.Range(0, 3);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else if (RandNum == 1)
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        else if(RandNum == 2)
        {
            
        }
        */
    }

    public override List<string> GetSummonMonsters()
    {
        List<string> SummonMonsters = new List<string>();
        for(int i = 0; i < SummonMonsterCount; i++)
        {
            SummonMonsters.Add(CanSummonMonsterIDs[0]);
        }

        return SummonMonsters;
        //return base.GetSummonMonsters();
    }
}
