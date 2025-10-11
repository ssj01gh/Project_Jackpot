using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_WoodElemental : Monster
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
        MonsterBuff.BuffList[(int)EBuffType.ToughSkin] = 99;
        MonsterBuff.BuffList[(int)EBuffType.Regeneration] = 99;
        MonsterAnimator.SetInteger("WoodElementalState", 0);
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

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        int RandNum = Random.Range(0, 2);
        if (RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        return 0;
    }

    public override void SetMonsterAnimation(string AnimationType = "")
    {
        //MonsterAnimator
        if (AnimationType == "Attack")
        {
            MonsterAnimator.SetInteger("WoodElementalState", 1);
        }
        else
        {
            MonsterAnimator.SetInteger("WoodElementalState", 0);
        }
    }

    public override bool CheckmonsterAnimationEnd(string AnimationType = "")
    {
        if(AnimationType == "Attack")
        {
            if(MonsterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                MonsterAnimator.SetInteger("WoodElementalState", 0);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
}
