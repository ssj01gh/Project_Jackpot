using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_ForestWitch : Monster
{
    private bool IsHaveBurn;
    private bool IsHaveAttackDebuff;
    private bool IsHaveDefenseDebuff;
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
        //시작은 화상 부여
        MonsterCurrentState = (int)EMonsterActionState.GiveBurn;
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        if (EnemyBuff.BuffList[(int)EBuffType.Burn] >= 1)
            IsHaveBurn = true;
        else
            IsHaveBurn = false;

        if (EnemyBuff.BuffList[(int)EBuffType.AttackDebuff] >= 1)
            IsHaveAttackDebuff = true;
        else
            IsHaveAttackDebuff = false;

        if (EnemyBuff.BuffList[(int)EBuffType.DefenseDebuff] >= 1)
            IsHaveDefenseDebuff = true;
        else
            IsHaveDefenseDebuff = false;
    }
    /*
     *  * 나의 체력이 50%이하 이고 재생이 없을때 재생 5 부여
 * 적에게 화상을 없을시 화상 3 부여
 * 적에게 화상이 있을시 공격력 감소 2 부여
 * 적에게 공감이 있을 경우 방어력 감소 2 부여
 *공감, 방감이 있을 경우 독 5 부여
*/

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        if(MonTotalStatus.MonsterCurrentHP / MonTotalStatus.MonsterMaxHP <= 0.5f &&
            MonsterBuff.BuffList[(int)EBuffType.Regeneration] < 1)//50퍼 보다 작고 재생이 없을때
            MonsterCurrentState = (int)EMonsterActionState.ApplyRegeneration;
        else
        {//50퍼 초과거나 재생있을때
            if(IsHaveBurn == false)//화상이 없을때
                MonsterCurrentState = (int)EMonsterActionState.GiveBurn;
            else
            {//화상이 있을때
                if (IsHaveAttackDebuff == false)//공깍이 없을때
                    MonsterCurrentState = (int)EMonsterActionState.GiveAttackDebuff;
                else
                {//공깍이 있을때
                    if (IsHaveDefenseDebuff == false)//방깍 없을때
                        MonsterCurrentState = (int)EMonsterActionState.GiveDefenseDebuff;
                    else//방깍 있을때
                        MonsterCurrentState = (int)EMonsterActionState.GivePoison;
                }
            }
        }
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        switch(i_BuffType)
        {
            case (int)EBuffType.Burn:
                return base.MonsterGiveBuff(i_BuffType, 4);
            case (int)EBuffType.Poison:
                return base.MonsterGiveBuff(i_BuffType, 5);
            case (int)EBuffType.AttackDebuff:
                return base.MonsterGiveBuff(i_BuffType, 2);
            case (int)EBuffType.DefenseDebuff:
                return base.MonsterGiveBuff(i_BuffType, 2);
            default:
                return 0;
        }
    }

    public override void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        if(i_BuffType == (int)EBuffType.Regeneration)
        {
            base.MonsterGetBuff(i_BuffType, 5);
        }
    }
}
