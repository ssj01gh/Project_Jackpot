using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_AdminServant : Monster
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
        MonsterBuff.BuffList[(int)EBuffType.Servant] = 99;
        MonsterCurrentState = (int)EMonsterActionState.Attack;
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        MonsterCurrentState = (int)EMonsterActionState.Attack;
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        switch(i_BuffType)
        {
            case (int)EBuffType.AttackDebuff:
                return base.MonsterGiveBuff(i_BuffType, 2);
            case (int)EBuffType.DefenseDebuff:
                return base.MonsterGiveBuff(i_BuffType, 2);
            case (int)EBuffType.Poison:
                return base.MonsterGiveBuff(i_BuffType, 5);
            case (int)EBuffType.Weakness:
                return base.MonsterGiveBuff(i_BuffType, 2);
            case (int)EBuffType.Slow:
                return base.MonsterGiveBuff(i_BuffType, 2);
            default:
                return 0;
        }
    }

    public override void SetMonsterAnimation(string AnimationType = "")
    {
        //MonsterAnimator
        string AnimeStateName = "";
        switch(MonsterName)
        {
            case "Administrator_Hammer":
                AnimeStateName = "Admin_HammerState";
                break;
            case "Administrator_Saw":
                AnimeStateName = "Admin_SawState";
                break;
            case "Administrator_Knife":
                AnimeStateName = "Admin_KnifeState";
                break;
            case "Administrator_Syringe":
                AnimeStateName = "Admin_SyringeState";
                break;
        }
        if (AnimationType == "Attack")
        {
            MonsterAnimator.SetInteger(AnimeStateName, 1);
        }
        else
        {
            MonsterAnimator.SetInteger(AnimeStateName, 0);
        }
    }

    public override bool CheckmonsterAnimationEnd(string AnimationType = "")
    {
        string AnimeStateName = "";
        switch (MonsterName)
        {
            case "Administrator_Hammer":
                AnimeStateName = "Admin_HammerState";
                break;
            case "Administrator_Saw":
                AnimeStateName = "Admin_SawState";
                break;
            case "Administrator_Knife":
                AnimeStateName = "Admin_KnifeState";
                break;
            case "Administrator_Syringe":
                AnimeStateName = "Admin_SyringeState";
                break;
        }
        if (AnimationType == "Attack")
        {
            if (MonsterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                MonsterAnimator.SetInteger(AnimeStateName, 0);
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
