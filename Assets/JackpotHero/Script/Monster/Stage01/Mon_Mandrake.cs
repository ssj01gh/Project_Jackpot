using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Mandrake : Monster
{
    protected bool IsEnemyMisFortune = false;
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
        MonsterCurrentState = (int)EMonsterActionState.GiveCurseOfDeath;
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
        if (EnemyBuff.BuffList[(int)EBuffType.Misfortune] >= 1)
        {
            IsEnemyMisFortune = true;
        }
        else
        {
            IsEnemyMisFortune = false;
        }
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        if(IsEnemyMisFortune == true)//������ ������ �ִٸ� �Ϲ��ൿ
        {
            //���� : ��� = 1 : 1
            int RandNum = Random.Range(0, 2);
            if(RandNum == 0)
            {
                MonsterCurrentState = (int)EMonsterActionState.Attack;
            }
            else
            {
                MonsterCurrentState = (int)EMonsterActionState.Defense;
            }
        }
        else//���ٸ� ���� Ȥ�� �Ϲ� �ൿ
        {
            //���� �ο� : ���� : ��� = 2 : 1 : 1
            int RandNum = Random.Range(0, 4);
            if(RandNum == 0)
            {
                MonsterCurrentState = (int)EMonsterActionState.Attack;
            }
            else if(RandNum == 1)
            {
                MonsterCurrentState = (int)EMonsterActionState.Defense;
            }
            else
            {
                MonsterCurrentState = (int)EMonsterActionState.GiveMisFortune;
            }
        }
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.Misfortune)
        {
            return base.MonsterGiveBuff(i_BuffType, 2);
        }
        else if (i_BuffType == (int)EBuffType.CurseOfDeath)
        {
            return base.MonsterGiveBuff(i_BuffType, 10);
        }
        else
        {
            return 0;
        }
    }
}
