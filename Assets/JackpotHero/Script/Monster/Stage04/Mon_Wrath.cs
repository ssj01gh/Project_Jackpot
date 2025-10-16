using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Wrath : Monster
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
        //나중에 여기서 쓰러트린 7죄종 갯수만큼 분노를 차감하면 될듯?
        MonsterBuff.BuffList[(int)EBuffType.Wrath] = 3;
        SetWrathActionState();
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        SetWrathActionState();
    }

    private void SetWrathActionState()
    {
        int RandNum = Random.Range(0, 4);//0,1,2,3
        switch(MonsterBuff.BuffList[(int)EBuffType.Charging])
        {
            case 0://100%
                MonsterCurrentState = (int)EMonsterActionState.ApplyCharging;
                break;
            case 1://75%
                if(RandNum == 0)//0
                    MonsterCurrentState = (int)EMonsterActionState.Attack;
                else//1,2,3
                    MonsterCurrentState = (int)EMonsterActionState.ApplyCharging;
                break;
            case 2://50%
                if(RandNum >= 1)//0,1
                    MonsterCurrentState = (int)EMonsterActionState.Attack;
                else//2,3
                    MonsterCurrentState = (int)EMonsterActionState.ApplyCharging;
                break;
            case 3://25%
                if (RandNum >= 2)//0,1,2
                    MonsterCurrentState = (int)EMonsterActionState.Attack;
                else//3
                    MonsterCurrentState = (int)EMonsterActionState.ApplyCharging;
                break;
            case 4://0%
                MonsterCurrentState = (int)EMonsterActionState.Attack;
                break;
            default://Attack으로
                MonsterCurrentState = (int)EMonsterActionState.Attack;
                break;
        }
    }

    public override void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.Charging)
        {
            base.MonsterGetBuff(i_BuffType, 1);
        }
    }

    public override void SetMonsterAnimation(string AnimationType = "")
    {
        //MonsterAnimator
        if (AnimationType == "Attack")
        {
            /*
            if (MonsterBuff.BuffList[(int)EBuffType.Charging] <= 1)
            {//1보다 작은 상황
                MonsterAnimator.SetInteger("WrathAnimeState", 11);
            }
            else if(MonsterBuff.BuffList[(int)EBuffType.Charging] == 2)
            {
                MonsterAnimator.SetInteger("WrathAnimeState", 21);
                //WrathAnimeState
            }
            else if(MonsterBuff.BuffList[(int)EBuffType.Charging] >= 3)
            {//스택이 3이상 쌓여있을때
                MonsterAnimator.SetInteger("WrathAnimeState", 31);
            }
            */
            if (MonsterAnimator.GetInteger("WrathAnimeState") == 1)
            {//1보다 작은 상황
                MonsterAnimator.SetInteger("WrathAnimeState", 11);
            }
            else if (MonsterAnimator.GetInteger("WrathAnimeState") == 2)
            {
                MonsterAnimator.SetInteger("WrathAnimeState", 21);
                //WrathAnimeState
            }
            else if (MonsterAnimator.GetInteger("WrathAnimeState") == 3)
            {//스택이 3이상 쌓여있을때
                MonsterAnimator.SetInteger("WrathAnimeState", 31);
            }
        }
        else
        {
            if (MonsterBuff.BuffList[(int)EBuffType.Charging] <= 0)
            {//1보다 작은 상황
                MonsterAnimator.SetInteger("WrathAnimeState", 0);
            }
            else if (MonsterBuff.BuffList[(int)EBuffType.Charging] == 1)
            {//1보다 작은 상황
                MonsterAnimator.SetInteger("WrathAnimeState", 1);
            }
            else if (MonsterBuff.BuffList[(int)EBuffType.Charging] == 2)
            {
                MonsterAnimator.SetInteger("WrathAnimeState", 2);
            }
            else if (MonsterBuff.BuffList[(int)EBuffType.Charging] >= 3)
            {//스택이 3이상 쌓여있을때
                MonsterAnimator.SetInteger("WrathAnimeState", 3);
            }
        }
    }

    public override bool CheckmonsterAnimationEnd(string AnimationType = "")
    {
        if (AnimationType == "Attack")
        {
            if (MonsterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                MonsterAnimator.SetInteger("WrathAnimeState", 0);
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
