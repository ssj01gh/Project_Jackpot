using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mon_Doppelganger : Monster
{
    enum ECopyState
    {
        STR,
        DUR,
        LUK,
        SPD
    }

    private List<int> DontCopyStateList = new List<int>();
    private bool IsAttackTurn = false;
    private bool IsCopyComplete = false;

    protected override void Start()
    {
        base.Start();
    }
    //Body가 꺼졌다가 켜졌을때 원래 모습으로 돌아왔다가 되돌아감..... 이걸 해결 할 방법이 있나?
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void InitMonsterState()
    {
        base.InitMonsterState();
        IsCopyComplete = false;
        IsAttackTurn = false;
        MonsterAnimator.SetInteger("DoppelgangerState", 0);
        DecideCopyState();
        //한번 처음부터 쭉 해보기 얘는
    }

    public override void CheckEnemyBuff(BuffInfo EnemyBuff)
    {
        base.CheckEnemyBuff(EnemyBuff);
    }

    public override void SetNextMonsterState()
    {
        base.SetNextMonsterState();
        if(IsCopyComplete == true)
        {
            int RandNum = Random.Range(0, 2);
            if (RandNum == 0)
                MonsterCurrentState = (int)EMonsterActionState.Attack;
            else
                MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        else
        {
            if(IsAttackTurn == true)
            {
                MonsterCurrentState = (int)EMonsterActionState.Attack;
                IsAttackTurn = false;
            }
            else
            {
                DecideCopyState();
            }
        }

        //DecideCopyState();
    }

    private void DecideCopyState()
    {
        DontCopyStateList.Clear();
        if (MonsterBuff.BuffList[(int)EBuffType.CopyStrength] < 1)
        {//없을때
            DontCopyStateList.Add((int)ECopyState.STR);
        }
        if (MonsterBuff.BuffList[(int)EBuffType.CopyDurability] < 1)
        {
            DontCopyStateList.Add((int)ECopyState.DUR);
        }
        if (MonsterBuff.BuffList[(int)EBuffType.CopyLuck] < 1)
        {
            DontCopyStateList.Add((int)ECopyState.LUK);
        }
        if (MonsterBuff.BuffList[(int)EBuffType.CopySpeed] < 1)
        {
            DontCopyStateList.Add((int)ECopyState.SPD);
        }

        int RandNum = 0;
        if(DontCopyStateList.Count <= 0)
        {//여기에 들어왔다 == 다 복사함
            IsCopyComplete = true;
            RandNum = Random.Range(0, 2);
            if (RandNum == 0)
                MonsterCurrentState = (int)EMonsterActionState.Attack;
            else
                MonsterCurrentState = (int)EMonsterActionState.Defense;
        }
        else
        {//하나 이상 부족한게 있을때
            IsAttackTurn = true;
            RandNum = Random.Range(0, DontCopyStateList.Count);
            switch (DontCopyStateList[RandNum])
            {
                case (int)ECopyState.STR:
                    MonsterCurrentState = (int)EMonsterActionState.ApplyCopyStrength;
                    break;
                case (int)ECopyState.DUR:
                    MonsterCurrentState = (int)EMonsterActionState.ApplyCopyDurability;
                    break;
                case (int)ECopyState.LUK:
                    MonsterCurrentState = (int)EMonsterActionState.ApplyCopyLuck;
                    break;
                case (int)ECopyState.SPD:
                    MonsterCurrentState = (int)EMonsterActionState.ApplyCopySpeed;
                    break;
                default:
                    RandNum = Random.Range(0, 2);
                    if (RandNum == 0)
                        MonsterCurrentState = (int)EMonsterActionState.Attack;
                    else
                        MonsterCurrentState = (int)EMonsterActionState.Defense;
                    break;
            }
        }
    }

    public override void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        base.MonsterGetBuff(i_BuffType, BuffCount);
    }
}
