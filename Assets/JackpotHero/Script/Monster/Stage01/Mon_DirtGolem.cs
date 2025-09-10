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
    }
}
