using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_ThornTurtle : Monster
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
        MonsterBuff.BuffList[(int)EBuffType.UnbreakableArmor] = 99;
        SetMonsterAction();
    }
    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        SetMonsterAction();
    }
    protected void SetMonsterAction()
    {
        if (MonsterBuff.BuffList[(int)EBuffType.ThronArmor] >= 1)
        {//가시 갑옷이 존재 할때
            //공격 : 방어 = 1 : 2
            int RandNum = Random.Range(0, 3);
            if (RandNum == 0)
            {
                MonsterCurrentState = (int)EMonsterActionState.Attack;
            }
            else
            {
                MonsterCurrentState = (int)EMonsterActionState.Defense;
            }
        }
        else
        {//가시 갑옷이 존재 하지 않을때
            MonsterCurrentState = (int)EMonsterActionState.ApplyThornArmor;
        }
    }

    public override void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        if(i_BuffType == (int)EBuffType.ThronArmor)
            base.MonsterGetBuff(i_BuffType, 3);
    }
}
