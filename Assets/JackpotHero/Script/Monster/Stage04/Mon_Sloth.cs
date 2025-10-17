using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Sloth : Monster
{
    enum ESlothState
    {
        NormalAction01,
        NormalAction02,
        NormalAction03,
        SpecialAction01,
        SpecialAction02
    }

    private int SlothNextActionState = (int)ESlothState.NormalAction01;
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
        MonsterBuff.BuffList[(int)EBuffType.Sloth] = 4;
        SlothNextActionState = (int)ESlothState.NormalAction01;
        MonsterAnimator.SetInteger("SlothAnimeState", 0);
        SetSlothActionState();
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        SetSlothActionState();
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if(i_BuffType == (int)EBuffType.Petrification)
        {
            return base.MonsterGiveBuff(i_BuffType, 5);
        }
        return 0;
    }

    private void SetSlothActionState()
    {
        switch(SlothNextActionState)
        {
            case (int)ESlothState.NormalAction01:
                SetSlothNormalAction();
                SlothNextActionState = (int)ESlothState.NormalAction02;
                break;
            case (int)ESlothState.NormalAction02:
                SetSlothNormalAction();
                SlothNextActionState = (int)ESlothState.NormalAction03;
                break;
            case (int)ESlothState.NormalAction03:
                SetSlothNormalAction();
                SlothNextActionState = (int)ESlothState.SpecialAction01;
                break;
            case (int)ESlothState.SpecialAction01:
                int RandomState = Random.Range(0, 2);
                if (RandomState == 0)
                {
                    SetSlothNormalAction();
                    SlothNextActionState = (int)ESlothState.SpecialAction02;
                }
                else
                {
                    MonsterCurrentState = (int)EMonsterActionState.GivePetrification;
                    SlothNextActionState = (int)ESlothState.NormalAction01;
                }

                break;
            case (int)ESlothState.SpecialAction02:
                MonsterCurrentState = (int)EMonsterActionState.GivePetrification;
                SlothNextActionState = (int)ESlothState.NormalAction01;
                break;
        }
    }
    private void SetSlothNormalAction()
    {
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
        if (AnimationType == "Attack")
        {
            if (MonsterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
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
