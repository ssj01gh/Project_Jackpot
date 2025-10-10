using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Slime : Monster
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
        //초기에는 공격 혹은 방어로
        int RandNum = Random.Range(0, 2);
        if(RandNum == 0)
        {
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        else
        {
            MonsterCurrentState = (int)EMonsterActionState.Defense;
        }

        /*
        MonsterCurrentState = (int)EMonsterActionState.SpawnMonster;
        MonsterCurrentState = (int)EMonsterActionState.ApplyRegeneration;
        */
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        int RandNum = Random.Range(0, 2);
        if (MonTotalStatus.MonsterCurrentHP / MonTotalStatus.MonsterMaxHP >= 0.5f)
        {
            if(IsCanSummonMonster == true)
            {
                MonsterCurrentState = (int)EMonsterActionState.SpawnMonster;
            }
            else
            {
                if (RandNum == 0)
                    MonsterCurrentState = (int)EMonsterActionState.Attack;
                else
                    MonsterCurrentState = (int)EMonsterActionState.Defense;
            }
        }
        else
        {//체력이 50퍼보다 작을때
            if (MonsterBuff.BuffList[(int)EBuffType.Regeneration] < 1)
            {//재생이 부여되어 있지 않으면
                MonsterCurrentState = (int)EMonsterActionState.ApplyRegeneration;
            }
            else
            {
                if (RandNum == 0)
                    MonsterCurrentState = (int)EMonsterActionState.Attack;
                else
                    MonsterCurrentState = (int)EMonsterActionState.Defense;
            }
        }
        //소환 가능한 상태 = spawnmonsterID + activeMosnter의 갯수가 3미만일때
    }

    public override void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        if(i_BuffType == (int)EBuffType.Regeneration)
        {
            base.MonsterGetBuff(i_BuffType, 6);
        }
    }

    public override List<string> GetSummonMonsters()
    {
        List<string> SummonMonsters = new List<string>();
        for (int i = 0; i < SummonMonsterCount; i++)
        {
            SummonMonsters.Add(CanSummonMonsterIDs[0]);
        }

        return SummonMonsters;
        //return base.GetSummonMonsters();
    }
}
