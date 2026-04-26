using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Mon_Doppelganger : Monster
{
    enum ECopyState
    {
        STR,
        DUR,
        LUK,
        SPD
    }

    enum EShadowState
    {
        Normal,
        Heroine_Idle,
        Heroine_Attack,
        Heroine_Defense,
        Heroine_Special
    }

    [SerializeField]
    private GameObject DopplegangerShadow;

    private List<int> DontCopyStateList = new List<int>();
    private bool IsAttackTurn = false;
    private bool IsCopyComplete = false;

    private float ShadowZPos = 0.1f;
    private Vector2[] ShadowPos =
    {
        new Vector2(0f,0f),
        new Vector2(0f, 0f),
        new Vector2(-0.04f, 0f),
        new Vector2(-0.08f, 0f),
        new Vector2(-0.04f, 0f)
    };
    private Vector2[] ShadowScale =
    {
        new Vector2(0.25f, 0.05f),
        new Vector2(0.25f, 0.05f),
        new Vector2(0.2f, 0.05f),
        new Vector2(0.35f, 0.05f),
        new Vector2(0.25f, 0.05f)
    };

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

    public override void SetMonsterAnimation(int AnimationState)
    {
        base.SetMonsterAnimation(AnimationState);//애니메이션이 변경된다.

        if(MonsterAnimator.GetInteger("DoppelgangerState") == 0)
        {
            DopplegangerShadow.transform.localPosition = new Vector3(ShadowPos[(int)EShadowState.Normal].x, ShadowPos[(int)EShadowState.Normal].y, ShadowZPos);
            DopplegangerShadow.transform.localScale = new Vector3(ShadowScale[(int)EShadowState.Normal].x, ShadowScale[(int)EShadowState.Normal].y, 1f);
        }
        else
        {
            switch(AnimationState)
            {
                case 0:
                    DopplegangerShadow.transform.localPosition = new Vector3(ShadowPos[(int)EShadowState.Heroine_Idle].x, ShadowPos[(int)EShadowState.Heroine_Idle].y, ShadowZPos);
                    DopplegangerShadow.transform.localScale = new Vector3(ShadowScale[(int)EShadowState.Heroine_Idle].x, ShadowScale[(int)EShadowState.Heroine_Idle].y, 1f);
                    break;
                case 1:
                case 3:
                    DopplegangerShadow.transform.localPosition = new Vector3(ShadowPos[(int)EShadowState.Heroine_Attack].x, ShadowPos[(int)EShadowState.Heroine_Attack].y, ShadowZPos);
                    DopplegangerShadow.transform.localScale = new Vector3(ShadowScale[(int)EShadowState.Heroine_Attack].x, ShadowScale[(int)EShadowState.Heroine_Attack].y, 1f);
                    break;
                case 2:
                    DopplegangerShadow.transform.localPosition = new Vector3(ShadowPos[(int)EShadowState.Heroine_Defense].x, ShadowPos[(int)EShadowState.Heroine_Defense].y, ShadowZPos);
                    DopplegangerShadow.transform.localScale = new Vector3(ShadowScale[(int)EShadowState.Heroine_Defense].x, ShadowScale[(int)EShadowState.Heroine_Defense].y, 1f);
                    break;
                case 5:
                    DopplegangerShadow.transform.localPosition = new Vector3(ShadowPos[(int)EShadowState.Heroine_Special].x, ShadowPos[(int)EShadowState.Heroine_Special].y, ShadowZPos);
                    DopplegangerShadow.transform.localScale = new Vector3(ShadowScale[(int)EShadowState.Heroine_Special].x, ShadowScale[(int)EShadowState.Heroine_Special].y, 1f);
                    break;
            }
        }
    }
}
