using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Assassnake : Monster
{
    protected int EnemyPoisonCount = 0;
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
        //PoisonCount ��ġ�� ���� �Ϲ��ൿ�� �������� Ȯ���� �޶���
        //PoisonCount = 10�̻��϶� �Ϲ��ൿ Ȯ�� = 100%
        //���� : ��� = 1 : 1
        EnemyPoisonCount = 0;
        int RandNum = Random.Range(0, 11);
        if(RandNum > EnemyPoisonCount)//���� �� ��ġ�� ���� ���� Ȯ�� ���� Ȯ�� ����
        {
            MonsterCurrentState = (int)EMonsterActionState.GivePoison;
        }
        else//������ �Ϲ� �ൿ
        {
            RandNum = Random.Range(0, 2);
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

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
        EnemyPoisonCount = EnemyBuff.BuffList[(int)EBuffType.Poison];
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        //PoisonCount ��ġ�� ���� �Ϲ��ൿ�� �������� Ȯ���� �޶���
        //PoisonCount = 10�̻��϶� �Ϲ��ൿ Ȯ�� = 100%
        //���� : ��� = 1 : 1
        int RandNum = Random.Range(0, 11);
        Debug.Log(EnemyPoisonCount);
        if (RandNum > EnemyPoisonCount)//���� �� ��ġ�� ���� ���� Ȯ�� ���� Ȯ�� ����
        {
            MonsterCurrentState = (int)EMonsterActionState.GivePoison;
        }
        else//������ �� ������
        {
            RandNum = Random.Range(0, 2);
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

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.Poison)
            return base.MonsterGiveBuff(i_BuffType, 4);
        else
            return 0;
    }
}
