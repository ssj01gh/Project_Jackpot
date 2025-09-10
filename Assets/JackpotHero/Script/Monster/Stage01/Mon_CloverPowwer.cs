using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_CloverPowwer : Monster
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
        if (MonsterBuff.BuffList[(int)EBuffType.Luck] <= 0)//����� ���ٸ�
        {
            MonsterCurrentState = (int)EMonsterActionState.ApplyLuck;
        }
        else//����� �ִٸ�
        {
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
        }
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        //���� : ��� = 1 : 1
        if (MonsterBuff.BuffList[(int)EBuffType.Luck] <= 0)//����� ���ٸ�
        {
            MonsterCurrentState = (int)EMonsterActionState.ApplyLuck;
        }
        else//����� �ִٸ�
        {
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
        }
    }
    public override void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.Luck)
            base.MonsterGetBuff((int)EBuffType.Luck, 2);
    }
}
