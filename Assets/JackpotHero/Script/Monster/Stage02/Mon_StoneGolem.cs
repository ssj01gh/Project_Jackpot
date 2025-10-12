using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_StoneGolem : Monster
{
    float RecordOfHP = 0;
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
        MonsterBuff.BuffList[(int)EBuffType.ToughFist] = 99;
        MonsterBuff.BuffList[(int)EBuffType.RegenArmor] = 15;
        MonTotalStatus.MonsterCurrentShieldPoint = 15;
        RecordOfHP = MonTotalStatus.MonsterCurrentHP;
        MonsterCurrentState = (int)EMonsterActionState.Attack;
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        //행동할 패턴이 정해진다 -> 적이 공격한다 -> 내턴이 됬을때 행동할 패턴을 실행한다. ->
        //SetNextMonsterState가 됬을때 RecordOfHP와 나의 현재 HP를 비교한다.
        //RecordOfHP가 현재 HP보다 많다면 보호막이 깨진적이 있다 -> 공격 : 방어 = 1 : 2로 실행한다.
        //RecordOfHp가 현재 HP보다 적거나 같다면 깨진적이 없다 -> 공격을 실행한다.
        //RecordOfHP를 갱신한다.
        if(RecordOfHP > MonTotalStatus.MonsterCurrentHP)
        {//기록한 Hp가 더 많을때
            int RandNum = Random.Range(0, 4);//0,1,2, 3
            if(RandNum == 0)
            {
                MonsterCurrentState = (int)EMonsterActionState.Attack;
                //MonsterCurrentState = (int)EMonsterActionState.Defense;
            }
            else
            {
                MonsterCurrentState = (int)EMonsterActionState.Defense;
            }
        }
        else
        {//기록한 Hp가 더 많거나 같을때
            MonsterCurrentState = (int)EMonsterActionState.Attack;
        }
        RecordOfHP = MonTotalStatus.MonsterCurrentHP;
    }
}
