using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBuffType
{
     //��ġ�� ��
    OverWhelmingPower,
    //���� ����
    ThronArmor,
    //���
    Defenseless,
    //��� �޽�
    AdvancedRest,
    //���
    HealingFactor,
    //������
    ReCharge,
    //��Ż
    Rapine,
    //������ �� ��
    EXPPower,
    //����������
    WeaponMaster,
    //����
    AttackDebuff,
    //���
    DefenseDebuff,
    //����
    Fear,
    //ȭ��
    Burn,
    //��
    Poison,
    //������ ����
    CurseOfDeath,
    //���������
    RegenArmor,
    //�ұ��ǰ���
    UnbreakableArmor,
    //������
    Weakness,
    //����
    FrostBite,
    //�ܴ����Ǻ�
    ToughSkin,
    //�ܴ����ָ�
    ToughFist,
    //���
    Luck,
    //����
    Misfortune,
    //�һ�//���� ���� �����߰�.
    UnDead,
    //Buff������ �´� BuffList�迭 ������ �����ִ°���
    CountOfBuff
}

public class BuffInfo
{
    public int[] BuffList = new int[(int)EBuffType.CountOfBuff];
    public void InitBuff()
    {
        for(int i = 0; i < BuffList.Length; i++)
        {
            BuffList[i] = 0;
        }
    }
    /*
    //��ġ�� ��
    public int OverWhelmingPower = 0;
    //���� ����
    public int ThronArmor = 0;
    //���
    public int SuddenAttack = 0;
    //��� �޽�
    public int AdvancedRest = 0;
    //���
    public int HealingFactor = 0;
    //������
    public int ReCharge = 0;
    //��Ż
    public int Rapine = 0;
    //������ �� ��
    public int EXPPower = 0;
    //����������
    public int WeaponMaster = 0;
    //����
    public int AttackDebuff = 0;
    //���
    public int DefenseDebuff = 0;
    //����
    public int Fear = 0;
    //ȭ��
    public int Burn = 0;
    //��
    public int poison = 0;
    //������ ����
    public int CurseOfDeath = 0;
    //���������
    public int RegenArmor = 0;
    //�ұ��ǰ���
    public int UnbreakableArmor = 0;
    //�һ�
    public int UnDead = 0;
    //������
    public int Weakness = 0;
    //����
    public int FrostBite = 0;
    //�ܴ����Ǻ�
    public int ToughSkin = 0;
    //�ܴ����ָ�
    public int ToughFist = 0;
    */
}