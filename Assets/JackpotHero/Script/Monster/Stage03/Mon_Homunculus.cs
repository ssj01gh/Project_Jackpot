using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Homunculus : Monster
{
    enum EConsumeState
    {
        ConsumeToughSkin,
        ConsumeToughFist,
        ConsumeLuck
    }

    private List<int> DontConsumeStateList = new List<int>();

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        /*
        if(MonsterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Momunculus_Idle"))
        {
            if(MonsterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
        }
        else if(MonsterAnimator.GetCurrentAnimatorStateInfo(0).IsName("Homunculus_Watch_Idle"))
        {

        }
        */
    }

    public void CheckSpecialAnimation()
    {
        if(MonsterAnimator.GetInteger("HomunculusState") == 0)
        {
            int RandNum = Random.Range(0, 20);
            if(RandNum == 0)
                MonsterAnimator.SetInteger("HomunculusState", 1);

        }
        else
        {
            MonsterAnimator.SetInteger("HomunculusState", 0);
        }
    }


    protected override void InitMonsterState()
    {
        base.InitMonsterState();
        ConsumeBuff();
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

    private void ConsumeBuff()
    {
        DontConsumeStateList.Clear();
        if (MonsterBuff.BuffList[(int)EBuffType.ToughSkin] < 1)
        {
            DontConsumeStateList.Add((int)EConsumeState.ConsumeToughSkin);
        }
        if (MonsterBuff.BuffList[(int)EBuffType.ToughFist] < 1)
        {
            DontConsumeStateList.Add((int)EConsumeState.ConsumeToughFist);
        }
        if (MonsterBuff.BuffList[(int)EBuffType.Luck] < 1)
        {
            DontConsumeStateList.Add((int)EConsumeState.ConsumeLuck);
        }

        if(DontConsumeStateList.Count < 1)
        {
            return;
        }
        int Rand = Random.Range(0, DontConsumeStateList.Count);
        switch(Rand)
        {
            case (int)EConsumeState.ConsumeToughSkin:
                MonsterBuff.BuffList[(int)EBuffType.ToughSkin] = 99;
                break;
            case (int)EConsumeState.ConsumeToughFist:
                MonsterBuff.BuffList[(int)EBuffType.ToughFist] = 99;
                break;
            case (int)EConsumeState.ConsumeLuck:
                MonsterBuff.BuffList[(int)EBuffType.Luck] = 99;
                break;
        }
    }

    public override void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        if(i_BuffType == (int)EBuffType.Consume)
        {
            ConsumeBuff();
            base.MonsterGetBuff(i_BuffType, 1);
        }
    }
}
