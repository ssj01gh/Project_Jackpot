using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

//이건 계속해서 늘어나야 할 enum일듯?
public enum EMonsterActionState
{
    Attack,
    Defense,
    SpawnMonster,
    ApplyLuck,
    GivePoison,
    GiveMisFortune,
    GiveCurseOfDeath,
    ApplyThornArmor,
    GiveCower,
    ApplyCopyStrength,
    ApplyCopyDurability,
    ApplyCopySpeed,
    ApplyCopyLuck,
    Charm,
    ApplyGreed,
    GiveEnvy,
    ConsumeGluttony,
    GiveDefenseDebuff,
    ApplyRegeneration,
    GiveBurn,
    GiveAttackDebuff,
    GiveOverChargeToServant,
    GiveCharm
}
public class MonsterCurrentStatus
{
    public float MonsterMaxHP;
    public float MonsterCurrentHP;

    public float MonsterCurrentActionGauge;
    public float MonsterNextActionGauge;
    public float MonsterCurrentShieldPoint;

    public float MonsterCurrentATK;
    public float MonsterCurrentDUR;
    public float MonsterCurrentLUK;
    public float MonsterCurrentSPD;

    public float MonsterReward;
}

public class Monster : MonoBehaviour
{
    public string MonsterName;
    public Sprite MonsterHead;
    public SpriteRenderer MonsterBody;
    public Animator MonsterAnimator;
    public bool IsHaveAttackAnimation;
    [Header("Monster_Tier")]
    public bool IsTierOne;
    public bool IsSummonTier;
    [Header("HP_MonsterState")]
    public float MonsterBaseHP;
    public float HPVarianceAmount;
    [Header("ATK_MonsterState")]
    public float MonsterBaseATK;
    public float ATKVarianceAmount;
    [Header("DUR_MonsterState")]
    public float MonsterBaseDUR;
    public float DURVarianceAmount;
    [Header("LUK_MonsterState")]
    public float MonsterBaseLuk;
    public float LUKVarianceAmount;
    [Header("SPD_MonsterState")]
    public float MonsterBaseSPD;
    public float SPDVarianceAmount;
    [Header("EXP_MonsterReward")]
    public float MonsterBaseEXP;
    public float EXPVarianceAmount;

    public int MonsterWeaponCode;
    public int MonsterArmorCode;
    public int[] MonsterAnotherEquipmentCode;
    [Header("MonsterUIPos")]
    public GameObject MonsterShieldPos;
    public GameObject HpSliderPos;
    public float HpsliderWidth;
    public float BuffPosUpperHp;
    public float ActionPosUpperHP;
    [Header("SummonMonsterForSpecialAction")]//몇몇 특수 몬스터 들을 위한 소환할 몬스터 종류
    public string[] CanSummonMonsterIDs;
    public int SummonMonsterCount;

    //--------------------^GetFromInspector\

    public event System.Action<Monster> MonsterClicked;
    [HideInInspector]
    public int MonsterCurrentState;
    [HideInInspector]
    public BuffInfo MonsterBuff = new BuffInfo();
    [HideInInspector]
    public float AdditionalEXP = 0;
    [HideInInspector]
    public GameObject MasterMonster = null;
    //protected BuffInfo EnemyBuff = new BuffInfo();
    protected MonsterCurrentStatus MonTotalStatus = new MonsterCurrentStatus();
    protected Collider2D MonCollider;
    public int BeforeMonsterShield { protected set; get; } = 0;

    protected bool IsCanSummonMonster = false;
    protected bool IsAllServantDead = false;

    protected float CurrentBaseATK = 0;
    protected float CurrentBaseDUR = 0;
    protected float CurrentBaseLUK = 0;
    protected float CurrentBaseSPD = 0;
    //public int UsedMonsterActionGuageIndex { protected set; get; } = 0;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        MonCollider = gameObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(gameObject.activeSelf == true && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 충돌 검사//지금 이거는 MainBattle이 진행중일때도 바뀌어 버림 나중에 바꿔야함
            //BattleTurn이 진행중일때 안클릭되게...
            //필요없나? 몬스터의 턴일때는 CurrentTarget이 필요한 상황에서 전부다 CurrentTurnObject가 일을 하고.
            //몬스터를 클릭하고 버튼을 누르는 순간 이미 모든 계산은 끝나있음.
            //사람이 다른 몬스터를 클릭해도 결과는 바뀌지 않는다.

            if (MonCollider != null && MonCollider.OverlapPoint(mousePosition))
            {
                MonsterClicked?.Invoke(gameObject.GetComponent<Monster>());
            }
        }
    }

    public void SpawnMonster(Vector2 SpawnPosition)
    {
        gameObject.SetActive(true);
        Color MonColor = MonsterBody.color;
        MonColor.a = 0;
        MonsterBody.color = MonColor;
        MonsterAnimator.speed = 0f;
        gameObject.transform.position = SpawnPosition;

        if(MasterMonster != null)
        {
            Monster MasterMon = MasterMonster.GetComponent<Monster>();
            if(MasterMon.MonsterName == "Slime")
            {
                MonsterBaseHP = (int)(MasterMon.MonTotalStatus.MonsterCurrentHP * 0.5f);
                MonsterBaseATK = (int)(MasterMon.MonTotalStatus.MonsterCurrentATK * 0.5f);
                MonsterBaseDUR = (int)(MasterMon.MonTotalStatus.MonsterCurrentDUR * 0.5f);
                MonsterBaseSPD = (int)(MasterMon.MonTotalStatus.MonsterCurrentSPD * 0.5f);
                MonsterBaseLuk = (int)(MasterMon.MonTotalStatus.MonsterCurrentLUK * 0.5f);
            }
            else if(MasterMon.MonsterName == "ABC")
            {

            }
        }

        //SetHP
        int Rand = Random.Range(-(int)HPVarianceAmount, (int)HPVarianceAmount + 1);
        MonTotalStatus.MonsterMaxHP = MonsterBaseHP + Rand;
        MonTotalStatus.MonsterCurrentHP = MonTotalStatus.MonsterMaxHP;
        //SetATK
        Rand = Random.Range(-(int)ATKVarianceAmount, (int)ATKVarianceAmount + 1);
        CurrentBaseATK = MonsterBaseATK + Rand;
        //SetDUR
        Rand = Random.Range(-(int)DURVarianceAmount, (int)DURVarianceAmount + 1);
        CurrentBaseDUR = MonsterBaseDUR + Rand;
        //SetLUK
        Rand = Random.Range(-(int)LUKVarianceAmount, (int)LUKVarianceAmount + 1);
        CurrentBaseLUK = MonsterBaseLuk + Rand;
        //SetSPD
        Rand = Random.Range(-(int)SPDVarianceAmount, (int)SPDVarianceAmount + 1);
        CurrentBaseSPD = MonsterBaseSPD + Rand;
        //SetReward
        Rand = Random.Range(-(int)EXPVarianceAmount, (int)EXPVarianceAmount + 1);
        MonTotalStatus.MonsterReward = MonsterBaseEXP + Rand;

        //Action게이지도 초기화
        MonTotalStatus.MonsterCurrentActionGauge = 0;
        MonTotalStatus.MonsterNextActionGauge = 0;
        //몬스터도 플레이어의 상태에 맞게 버프를 바아야함
        InitAllBuff();//일단 초기화 하고 추가
        SetMonsterStatus();
        SetMonsterVariousBuff();

        SetInitBuffByPlayerState();
        //일단은 방심만
        //개인적 몬스터에대한 상태에 대한 버프는 InitMonsterState에서
        InitMonsterState();
        SpawnFadeIn();
    }

    protected void SpawnFadeIn()
    {
        Color MonColor = MonsterBody.color;
        MonColor.a = 0;
        MonsterBody.color = MonColor;
        MonsterBody.DOFade(1, 1).OnComplete(() => { MonsterAnimator.speed = 1; });
    }

    public void DeSpawnFadeOut()
    {
        Color MonColor = MonsterBody.color;
        MonColor.a = 1;
        MonsterBody.color = MonColor;
        MonsterBody.DOFade(0, 1).OnComplete(() => { gameObject.SetActive(false); });
    }

    public MonsterCurrentStatus GetMonsterCurrentStatus()
    {
        return MonTotalStatus;
    }

    public float GetMonsterCurrentBaseStatus(string StatusType)
    {
        switch(StatusType)
        {
            case "STR":
                return CurrentBaseATK;
            case "DUR":
                return CurrentBaseDUR;
            case "LUK":
                return CurrentBaseLUK;
            case "SPD":
                return CurrentBaseSPD;
            default:
                return 0;
        }
    }

    public void RecordMonsterBeforeShield()
    {
        BeforeMonsterShield = (int)MonTotalStatus.MonsterCurrentShieldPoint;
    }

    protected virtual void InitMonsterState()
    {
        AdditionalEXP = 0;
        //Test
        //MonsterBuff.BuffList[(int)EBuffType.BloodFamiliy] = 99;
        //MonsterBuff.BuffList[(int)BuffType.ThronArmor] = 10;
        //MonsterBuff.BuffList[15] = 10;
        //Test
    }
    public virtual void CheckEnemyBuff(BuffInfo EnemyBuff)
    {

    }
    public void CheckCanSummonMonster(int SummonMonsterCount, int ActiveMonsterCount)
    {//아직 스폰되기 전임
        if(SummonMonsterCount + ActiveMonsterCount < 3)
        {
            IsCanSummonMonster = true;
        }
        else if(SummonMonsterCount + ActiveMonsterCount >= 3)
        {
            IsCanSummonMonster = false;
        }
        
        //소환 예정인 아이도 없고, 혼자 남아있다 -> 모든 서번트가 죽어있다?
        if(SummonMonsterCount + ActiveMonsterCount == 1)
        {
            IsAllServantDead = true;
        }
        else if(SummonMonsterCount + ActiveMonsterCount > 1)
        {
            IsAllServantDead = false;
        }
    }
    public virtual void SetNextMonsterState()
    {

    }
    public virtual void SetMonsterAnimation(string AnimationType = "")
    {

    }

    public virtual bool CheckmonsterAnimationEnd(string AnimationType = "")
    {
        return true;
    }

    public void SetMonsterStatus()
    {
        MonTotalStatus.MonsterCurrentATK = CurrentBaseATK;
        MonTotalStatus.MonsterCurrentDUR = CurrentBaseDUR;
        MonTotalStatus.MonsterCurrentLUK = CurrentBaseLUK;
        MonTotalStatus.MonsterCurrentSPD = CurrentBaseSPD;

        for(int i = 0; i < (int)EBuffType.CountOfBuff; i++)
        {
            if (MonsterBuff.BuffList[i] < 1)
                continue;

            switch(i)
            {
                case (int)EBuffType.Luck:
                    MonTotalStatus.MonsterCurrentLUK += 10;
                    break;
                case (int)EBuffType.Misfortune:
                    MonTotalStatus.MonsterCurrentLUK -= 10;
                    break;
                case (int)EBuffType.MountainLord:
                    MonTotalStatus.MonsterCurrentATK += MonsterBuff.BuffList[(int)EBuffType.MountainLord];
                    MonTotalStatus.MonsterCurrentDUR += MonsterBuff.BuffList[(int)EBuffType.MountainLord];
                    MonTotalStatus.MonsterCurrentLUK += MonsterBuff.BuffList[(int)EBuffType.MountainLord];
                    MonTotalStatus.MonsterCurrentSPD += MonsterBuff.BuffList[(int)EBuffType.MountainLord];
                    break;
                case (int)EBuffType.OverCharge:
                    MonTotalStatus.MonsterCurrentSPD += 20;
                    break;
                case (int)EBuffType.CopyStrength:
                    MonTotalStatus.MonsterCurrentATK += MonsterBuff.BuffList[(int)EBuffType.CopyStrength];
                    break;
                case (int)EBuffType.CopyDurability:
                    MonTotalStatus.MonsterCurrentDUR += MonsterBuff.BuffList[(int)EBuffType.CopyDurability];
                    break;
                case (int)EBuffType.CopySpeed:
                    MonTotalStatus.MonsterCurrentSPD += MonsterBuff.BuffList[(int)EBuffType.CopySpeed];
                    break;
                case (int)EBuffType.CopyLuck:
                    MonTotalStatus.MonsterCurrentLUK += MonsterBuff.BuffList[(int)EBuffType.CopyLuck];
                    break;
                case (int)EBuffType.Consume:
                    int StatusIncreaseByConsume = MonsterBuff.BuffList[(int)EBuffType.Consume] * 5;
                    MonTotalStatus.MonsterCurrentATK += StatusIncreaseByConsume;
                    MonTotalStatus.MonsterCurrentDUR += StatusIncreaseByConsume;
                    MonTotalStatus.MonsterCurrentLUK += StatusIncreaseByConsume;
                    MonTotalStatus.MonsterCurrentSPD += StatusIncreaseByConsume;
                    break;
                case (int)EBuffType.CorruptSerum:
                    MonTotalStatus.MonsterCurrentATK += 3;
                    MonTotalStatus.MonsterCurrentDUR += 3;
                    MonTotalStatus.MonsterCurrentSPD += 3;
                    break;
                case (int)EBuffType.Slow:
                    MonTotalStatus.MonsterCurrentSPD -= 10;
                    break;
                case (int)EBuffType.Haste:
                    MonTotalStatus.MonsterCurrentSPD += 10;
                    break;
                case (int)EBuffType.StrengthAdaptation:
                    int StackOfSTRAdap = MonsterBuff.BuffList[(int)EBuffType.StrengthAdaptation];
                    MonTotalStatus.MonsterCurrentATK += (StackOfSTRAdap * StackOfSTRAdap);
                    break;
                case (int)EBuffType.DurabilityAdaptation:
                    int StackOfDURAdap = MonsterBuff.BuffList[(int)EBuffType.DurabilityAdaptation];
                    MonTotalStatus.MonsterCurrentDUR += (StackOfDURAdap * StackOfDURAdap);
                    break;
                case (int)EBuffType.SpeedAdaptation:
                    int StackOfSPDAdap = MonsterBuff.BuffList[(int)EBuffType.SpeedAdaptation];
                    MonTotalStatus.MonsterCurrentSPD += (StackOfSPDAdap * StackOfSPDAdap);
                    break;
                case (int)EBuffType.Charging:
                    MonTotalStatus.MonsterCurrentLUK += (MonsterBuff.BuffList[(int)EBuffType.Charging] * 15);
                    break;
                case (int)EBuffType.Greed:
                    int IncreaseStateByGreed = (int)(MonsterBuff.BuffList[(int)EBuffType.Greed] * 0.05f);
                    int CurrentDamange = (int)(MonTotalStatus.MonsterMaxHP - MonTotalStatus.MonsterCurrentHP);
                    MonTotalStatus.MonsterMaxHP = MonsterBaseHP + MonsterBuff.BuffList[(int)EBuffType.Greed];
                    MonTotalStatus.MonsterCurrentHP = MonTotalStatus.MonsterMaxHP - CurrentDamange;
                    MonTotalStatus.MonsterCurrentATK += IncreaseStateByGreed;
                    MonTotalStatus.MonsterCurrentDUR += IncreaseStateByGreed;
                    MonTotalStatus.MonsterCurrentSPD += IncreaseStateByGreed;
                    MonTotalStatus.MonsterCurrentLUK += IncreaseStateByGreed;
                    break;
            }
        }
        if(MonTotalStatus.MonsterCurrentATK < 0)
        {
            MonTotalStatus.MonsterCurrentATK = 0;
        }
        if (MonTotalStatus.MonsterCurrentDUR < 0)
        {
            MonTotalStatus.MonsterCurrentDUR = 0;
        }
        if (MonTotalStatus.MonsterCurrentSPD < 0)
        {
            MonTotalStatus.MonsterCurrentSPD = 0;
        }
    }

    public void SetMonsterVariousBuff()
    {
        MonsterBuff.BuffList[(int)EBuffType.Defense] = (int)(MonTotalStatus.MonsterCurrentDUR / 5);

        if (MonsterName == "Doppelganger")
        {//여기서 변하게....
            bool CopySTR = false;
            bool CopyDUR = false;
            bool CopyLUK = false;
            bool CopySPD = false;

            if (MonsterBuff.BuffList[(int)EBuffType.CopyStrength] >= 1)
                CopySTR = true;
            if (MonsterBuff.BuffList[(int)EBuffType.CopyDurability] >= 1)
                CopyDUR = true;
            if (MonsterBuff.BuffList[(int)EBuffType.CopyLuck] >= 1)
                CopyLUK = true;
            if (MonsterBuff.BuffList[(int)EBuffType.CopySpeed] >= 1)
                CopySPD = true;

            if (CopySTR == true && CopyDUR == true && CopyLUK == true && CopySPD == true)
            {
                MonsterAnimator.SetInteger("DoppelgangerState", 1);
            }
        }
    }

    protected void SetInitBuffByPlayerState()
    {
        if(JsonReadWriteManager.Instance.E_Info.EarlySpeedLevel >= 7)
        {
            MonsterBuff.BuffList[(int)EBuffType.Defenseless] = 1;
        }
    }

    public void InitAllBuff()//소환 될때랑 죽을때//쉴드량 포함
    {
        BeforeMonsterShield = 0;
        MonTotalStatus.MonsterCurrentShieldPoint = 0;

        for (int i = 0; i < MonsterBuff.BuffList.Length; i++)
        {
            MonsterBuff.BuffList[i] = 0;
        }
    }
    public Vector2 GetMonActionTypePos()
    {
        return new Vector2(HpSliderPos.transform.position.x, HpSliderPos.transform.position.y + ActionPosUpperHP);
    }
    public virtual void MonsterDamage(float DamagePoint)//여기서 싹 데미지 계산
    {

        float RestDamage = 0;
        if (MonsterBuff.BuffList[(int)EBuffType.Defense] >= 1)
            DamagePoint -= MonsterBuff.BuffList[(int)EBuffType.Defense];

        if (DamagePoint < 0)
            DamagePoint = 0;

        if (MonsterBuff.BuffList[(int)EBuffType.Defenseless] >= 1)
            DamagePoint = DamagePoint * 2;
        
        if (MonTotalStatus.MonsterCurrentShieldPoint >= DamagePoint)
        {
            RecordMonsterBeforeShield();
            MonTotalStatus.MonsterCurrentShieldPoint -= DamagePoint;
        }
        else
        {
            RestDamage = DamagePoint - MonTotalStatus.MonsterCurrentShieldPoint;
            RecordMonsterBeforeShield();
            MonTotalStatus.MonsterCurrentShieldPoint = 0;
        }
        MonTotalStatus.MonsterCurrentHP -= RestDamage;
    }

    public void MonsterRegenHP(float RegenPoint)
    {
        MonTotalStatus.MonsterCurrentHP += RegenPoint;
        if(MonTotalStatus.MonsterCurrentHP >= MonTotalStatus.MonsterMaxHP)
        {
            MonTotalStatus.MonsterCurrentHP = MonTotalStatus.MonsterMaxHP;
        }
    }

    public void MonsterGetShield(float ShieldPoint)
    {
        RecordMonsterBeforeShield();
        MonTotalStatus.MonsterCurrentShieldPoint += ShieldPoint;
    }

    //-------------------------SpecialAction------------------------

    public virtual void MonsterGetBuff(int i_BuffType, int BuffCount = 0)
    {
        MonsterBuff.BuffList[i_BuffType] += BuffCount;
    }
    /*
    public virtual void MonsterGetThronArmor(int ThronArmorCount = 0)
    {
        MonsterBuff.BuffList[(int)EBuffType.]
    }
    */
    public virtual int MonsterGiveBuff(int i_BuffType, int BuffCount = 0)
    {
        return BuffCount;
    }

    public virtual List<string> GetSummonMonsters()
    {
        List<string> SummonMosnters = new List<string>();
        return SummonMosnters;
    }
}
