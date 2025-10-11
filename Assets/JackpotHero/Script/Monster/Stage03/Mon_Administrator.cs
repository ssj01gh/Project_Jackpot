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
}
