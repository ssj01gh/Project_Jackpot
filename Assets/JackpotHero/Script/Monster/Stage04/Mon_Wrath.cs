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
        Debug.Log(JsonReadWriteManager.Instance.LkEv_Info.GreatDevilKillCount);
        int WrathCount = 7 - JsonReadWriteManager.Instance.LkEv_Info.GreatDevilKillCount;
        MonsterBuff.BuffList[(int)EBuffType.Wrath] = WrathCount;
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
}
