using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_PrairieKnight : Monster
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
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
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
    }
}
