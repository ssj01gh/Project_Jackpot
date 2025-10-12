using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Administrator : Monster
{
    enum EAdministratorState
    {
        SummonServant,
        OverChargeServant,
        Acting01,
        Acting02
    }

    int AdministratorNextAction = 0;
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
        MonsterCurrentState = (int)EMonsterActionState.SpawnMonster;
        AdministratorNextAction = (int)EAdministratorState.OverChargeServant;
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        //1. 몬스터를 소환한다.
        //2. 부하들에게 과부하를 부여한다.
        //3. 공격 혹은 방어를 2번 실행한다.
        //1부터 다시 반복한다.
        //만약 이 함수에 들어왔을때 소환되어 있는 졸개가 아무것도 없다면 1번부터 다시 실행한다.

        if(IsAllServantDead)
        {
            AdministratorNextAction = (int)EAdministratorState.SummonServant;
        }

        int RandNum = Random.Range(0, 3);
        switch(AdministratorNextAction)
        {
            case (int)EAdministratorState.SummonServant:
                MonsterCurrentState = (int)EMonsterActionState.SpawnMonster;
                AdministratorNextAction = (int)EAdministratorState.OverChargeServant;
                break;
            case (int)EAdministratorState.OverChargeServant:
                MonsterCurrentState = (int)EMonsterActionState.GiveOverChargeToServant;
                AdministratorNextAction = (int)EAdministratorState.Acting01;
                break;
            case (int)EAdministratorState.Acting01:
                if(RandNum == 0)
                    MonsterCurrentState = (int)EMonsterActionState.Attack;
                else
                    MonsterCurrentState = (int)EMonsterActionState.Defense;

                AdministratorNextAction = (int)EAdministratorState.Acting02;

                break;
            case (int)EAdministratorState.Acting02:
                if (RandNum == 0)
                    MonsterCurrentState = (int)EMonsterActionState.Attack;
                else
                    MonsterCurrentState = (int)EMonsterActionState.Defense;

                AdministratorNextAction = (int)EAdministratorState.SummonServant;

                break;
        }
    }

    public override List<string> GetSummonMonsters()
    {
        List<string> SummonMonsters = new List<string>();
        for (int i = 0; i < SummonMonsterCount; i++)
        {
            int RandNum = Random.Range(0, CanSummonMonsterIDs.Length);
            SummonMonsters.Add(CanSummonMonsterIDs[RandNum]);
        }

        return SummonMonsters;
    }

    public override int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        if (i_BuffType == (int)EBuffType.OverCharge)
            return base.MonsterGiveBuff(i_BuffType, 3);
        else
            return 0;
    }
}
