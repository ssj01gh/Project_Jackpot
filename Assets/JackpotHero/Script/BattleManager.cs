using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public enum EBattleStates
{
    Idle,
    PlayerTurn,
    MonsterTurn,
}

public class BattleResultStates
{
    public float BaseAmount;
    public float BaseAmountPlus;
    public List<float> ResultMagnification = new List<float>();
    public float BuffMagnification;
    public float FinalResultAmountPlus;
    public float FinalResultAmount;
}
/*
public class ActivePlayerMonsterActionGuages
{
    public float ThisTurnPlayerGuage = 0;
    public float[] ThisTurnMonsterGuages = new float[3] { 0, 0, 0 };
}
*/
public class BattleManager : MonoBehaviour
{
    protected List<GameObject> BattleTurn = new List<GameObject>();
    //protected List<ActivePlayerMonsterActionGuages> ThisTurnPlayerMosnterGuage = new List<ActivePlayerMonsterActionGuages>();
    [SerializeField]
    private PlayerManager PlayerMgr;
    [SerializeField]
    private MonsterManager MonMgr;
    [SerializeField]
    private PlaySceneUIManager UIMgr;
    // Start is called before the first frame update
    public GameObject CurrentTurnObject { protected set; get; }
    public BattleResultStates BattleResultStatus { protected set; get; } = new BattleResultStates();
    public int CurrentState { protected set; get; } = (int)EBattleStates.Idle;

    protected GameObject TargetObject = null;
    protected float PlayerActiveGauge = 0;

    protected float NextPlayerActiveGauge = 0;

    protected bool IgnoreBuffProgressAtFirstBattleProgress = false;

    private const int CurrentFinalStage = 1;
    public float CurrentSpawnPatternReward { protected set; get; }

    protected List<string> SpawnMonstersID = new List<string>();
    protected GameObject SummonerMonster = null;
    //protected List<float> MonsterActiveGuage = new List<float>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.P))
        {
            foreach(GameObject obj in BattleTurn)
            {
                Debug.Log(obj.name);
            }
        }
        */
    }
    public List<GameObject> GetBattleTurn()
    {
        return BattleTurn;
    }

    public void InitCurrentBattleMonsters(bool IsBossBattle = false)//시작할때 한번만 실행됨
    {
        IgnoreBuffProgressAtFirstBattleProgress = true;
        CurrentTurnObject = null;
        CurrentSpawnPatternReward = 0;
        if(IsBossBattle == false)
        {
            MonMgr.SetSpawnPattern(PlayerMgr);//몬스터의 스폰 패턴 설정
        }
        else
        {//보스 스폰이라면 다르게 //보스전에서 껏다 켰다면 위에 꺼로 가지만 문제는 x
            MonMgr.SetBossSpawn(PlayerMgr);
        }
        
        MonMgr.SpawnCurrentSpawnPatternMonster();//스폰 패턴에 맞게 몬스터 생성//ActiveMonster설정//버프도 다 0으로 최기화됨
        MonMgr.GiveBuffActiveMonsterByPlayer(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //몬스터는 이 SpawnCurrentSpawnPatternMonster에서 얻을 버프를 다 받음
        //스폰 패턴이 저장된후 다시 저장
        PlayerMgr.GetPlayerInfo().SetInitBuffByMonsters(MonMgr.GetActiveMonsters());
        PlayerMgr.GetPlayerInfo().SetInitBuff();
        //플레이어의 상태에 맞는 버프를 적용 시켜야함
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    public void InitMonsterNPlayerActiveGuage()//초기화
    {
        BattleTurn.Clear();
        PlayerActiveGauge = 0;
        NextPlayerActiveGauge = 0;
        for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
        {
            //MonsterActiveGauge[i] = 0;
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge = 0;
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge = 0;
        }

        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack != -1)//습격을 당했다면
        {
            if (MonMgr.GetActiveMonsters().Count >= 1)
            {
                //BattleTurn.Add(MonMgr.GetActiveMonsters()[0]);
                for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
                {
                    //BattleTurn.Add(MonMgr.GetActiveMonsters()[i]);
                    //몬스터의 NextGauge에 100+SPD를 할당한다 -> 습격한 몬스터 중에 빠른놈부터 행동
                    MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge
                        = 100 + MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentSPD;
                }
            }
        }
    }

    public void ProgressBattle()//이게 전투가 계속되는 동안 지속적으로 불러와져야될 함수임
    {
        //%1000을 했을때 200이상, 300미만이 보스
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 >= 200 &&
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 < 300)
        {
            SoundManager.Instance.PlayBGM("BossBattleBGM");
        }
        else
        {
            SoundManager.Instance.PlayBGM("NormalBattleBGM");
        }

        //몬스터는 여기서 불사를 계산해야함
        List<int> RewardEXPs = MonMgr.CheckActiveMonstersRSurvive(PlayerMgr);//현재 스폰된 몬스터중 죽은 몬스터 정리//죽을때 Reward증가
        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack == -1)
        {//-1이란 것은 습격이 아니다 -> 보상을 획득 해야 한다.
            for (int i = 0; i < RewardEXPs.Count; i++)
            {
                CurrentSpawnPatternReward += RewardEXPs[i];
            }
        }

        if(SpawnMonstersID.Count >= 1)
        {
            MonMgr.SpawnMonsterBySummonMonster(SpawnMonstersID, SummonerMonster);
            SummonerMonster = null;
        }
        
        //버프 계산 <- 이놈은 근데 전의 녀석을 계산함 만약 없으면 계산X
        if (CurrentTurnObject != null)
        {
            AfterBuffProgress();//AfterBuffProgress
            //위축 -> 공포는 모든 턴마다 전환
            while (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] >= 3)
            {
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] -= 3;
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] += 1;
            }
        }
        
        //스탯 계산
        MonMgr.SetActiveMonstersStatus();//현재 Active되있는 몬스터 들의 status 계산
        PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//버프 디버프에의한 스탯 계산용
        PlayerMgr.GetPlayerInfo().SetDefeseResilienceBuff();
        foreach (GameObject Mon in MonMgr.GetActiveMonsters())
        {
            Mon.GetComponent<Monster>().SetMonsterVariousBuff();
        }
        //턴계산 하기전에 전의 턴의 녀석이 죽었나 확인. 전의 녀석이 죽었으면 CurrentTurnObject를 초기화한다?
        if (CurrentTurnObject != null && CurrentTurnObject.tag == "Monster")
        {
            if (CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentHP <= 0)
            {
                //몬스터가 죽으면 progressBattle을 다시 실행한다.
                CurrentTurnObject = null;
                ProgressBattle();
                return;
            }
        }
        SetBattleTurn();//플레이어와 몬스터의 SPD값에 영향을 받은 Turn을 결정->아마 여기서 확인할듯?
        DecideCurrentBattleTurn();//여기서 현재 누구의 차례인지 결정
        UIMgr.B_UI.SetBattleTurnUI(BattleTurn);//결정된 Turn을 ui에 표시
        MonMgr.SetCurrentTargetMonster(null);//CurrentTarget초기화
        UIMgr.B_UI.UnDisplayMonsterDetailUI();//몬스터의 상세 표시 UnActive//상세 스테이터스와 장비같은 것들;
        //여기 밑에서 BeforeBuffProgress
        if(CurrentTurnObject != null)
        {
            BeforeBuffProgress();
        }

        /*
        if (IgnoreBuffProgressAtFirstBattleProgress == true)
        {
            MonMgr.SetActiveMonstersStatus();//현재 Active되있는 몬스터 들의 status 계산
            PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//버프 디버프에의한 스탯 계산용
        }//처음용

        SetBattleTurn();//플레이어와 몬스터의 SPD값에 영향을 받은 Turn을 결정->아마 여기서 확인할듯?
        DecideCurrentBattleTurn();//여기서 현재 누구의 차례인지 결정
        UIMgr.B_UI.SetBattleTurnUI(BattleTurn);//결정된 Turn을 ui에 표시
        MonMgr.SetCurrentTargetMonster(null);//CurrentTarget초기화
        UIMgr.B_UI.UnDisplayMonsterDetailUI();//몬스터의 상세 표시 UnActive//상세 스테이터스와 장비같은 것들;

        //여기에서 특정 버프의 데미지라던가 줘야할듯(살아있는 놈의 자신의 턴에)
        if (IgnoreBuffProgressAtFirstBattleProgress == true)
        {
            IgnoreBuffProgressAtFirstBattleProgress = false;
            ActiveBuffEffectAtFirstTime();
        }
        else
        {
            BuffProgress();//이거 처음은 무시하게해야함//여기서 이펙트 발생임.....
                           //hit 이펙트와 사운드는 BattleUI에서 앞으로 나갈때 발생 해야 할듯?
            MonMgr.SetActiveMonstersStatus();//현재 Active되있는 몬스터 들의 status 계산
            PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//버프 디버프에의한 스탯 계산용
            //처음 이후로는 후반에
        }
        */


        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(), 
            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);//플레이어 정보도 갱신
        UIMgr.B_UI.SetPlayerBattleUI(PlayerMgr.GetPlayerInfo());
        UIMgr.B_UI.SetMonsterBattleUI(MonMgr.GetActiveMonsters());
        //UIMgr.B_UI.ActivePlayerShieldNBuffUI(PlayerMgr.GetPlayerInfo());//이거 전에 몬스터가 죽엇는지 아닌지 확인해야됨

        if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP < 1)
        {
            //스프라이트가 쓰러지는 스프라이트가 재생된다던가?
            //ActiveMonster전부다 해제
            MonMgr.InActiveAllActiveMonster();
            //UIMgr.B_UI.ActivePlayerShieldNBuffUI(PlayerMgr.GetPlayerInfo());//여기있는 Set~~은 다 없애기 위함
            PlayerMgr.GetPlayerInfo().CalculateEarlyPoint();
            PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Defeat);
            UIMgr.B_UI.DefeatBattle(PlayerMgr.GetPlayerInfo());
            UIMgr.PlayerDefeat();//플레이어의 장비창, 스탯창, 현재 스테이지 진행창 같은것들
            //다른 플레이어 UI도 없에기
            return;
        }
        if (MonMgr.GetActiveMonsters().Count <= 0)
        {
            Debug.Log("Winner");
            PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle);
            //현재의 상대가 보스라는것을 확일할 방법이 있는가? 엑셀 참고//보스 200이상 300미만
            if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 >= 200 &&
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 < 300)
            {
                PlayerMgr.GetPlayerInfo().EndOfAction(true);//여기서 버프가 다 0이됨
                PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//버프 디버프에의한 스탯 계산용
                UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);//플레이어 정보도 갱신
                int RewardEXP = PlayerMgr.GetPlayerInfo().ReturnEXPByEXPMagnification((int)CurrentSpawnPatternReward);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RewardEXP, true);
                UIMgr.B_UI.VictoryBattle(RewardEXP);
            }
            else//보스가 아니라면 평범하게
            {
                PlayerMgr.GetPlayerInfo().EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화//여기서 버프가 다 0이됨
                PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//버프 디버프에의한 스탯 계산용//VictoryButton
                UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);//버프가 없어진것에 따른 정보 갱신
                                                                   //휴식중에 습격을 받은거라면 다시 휴식 행동 선택으로 돌아감
                int RewardEXP = PlayerMgr.GetPlayerInfo().ReturnEXPByEXPMagnification((int)CurrentSpawnPatternReward);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RewardEXP, true);
                UIMgr.B_UI.VictoryBattle(RewardEXP);
            }
            return;
            //다 죽으면 게임을 전투를 끝낸다.
        }

        if (CurrentState == (int)EBattleStates.PlayerTurn)
        {
            //플레이어의 턴이라면 플레이어의 행동을 결정하는 버튼을 나타나게 한다.
            UIMgr.B_UI.ActiveBattleSelectionUI(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] >= 1,
                PlayerMgr.GetPlayerInfo().AttackAverageIncrease, PlayerMgr.GetPlayerInfo().DefenseAverageIncrease, PlayerMgr.GetPlayerInfo().RestAverageIncrease);//행동 결정 버튼이 나타남

            //플레이어의 턴이 됬으니 살아있는 몬스터들의 연속 타격 초기화
            MonMgr.SetActiveMonsterChainAttack(false, false);
        }
        else if (CurrentState == (int)EBattleStates.MonsterTurn)
        {
            //몬스터의 턴이라는 행동 버튼이 나타남
            UIMgr.B_UI.ActiveBattleSelectionUI_Mon();

            //몬스텅의 턴이 됬으니 플레이어의 연속 타격 초기화
            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(false, false);
            /*
            MonMgr.SetCurrentTargetMonster(BattleMgr.CurrentTurnObject.GetComponent<Monster>());
            BattleButtonClick("Monster");
            */
        }
    }

    //
    public void PlayerBattleActionSelectionButtonClick(string ActionButtonType)//누른 버튼의 행동에 따라 달라짐
    {
        //공격의 경우 공격 상대가 정해져 있어야함
        SoundManager.Instance.PlayUISFX("UI_Button");
        SpawnMonstersID.Clear();
        bool IsTargetHasShield = false;
        if (ActionButtonType == "Attack" && MonMgr.CurrentTarget == null)
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.AttackGuideMessage);
            return;
        }
        if (PlayerMgr.GetPlayerInfo().SpendSTA(ActionButtonType) == false)//피로도가 부족하면
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_Battle);
            return;
        }

        //매혹이 있다면//확률에 걸렸다면//ActionButtonType을 Charm으로 바꿔야 할듯?
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charm] >= 1)
        {
            //1~11에서 랜덤값
            int GetCharm = PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charm];
            int RandCharmNum = Random.Range(1, 11);
            if(GetCharm >= RandCharmNum)
            {
                ActionButtonType = "Charm";
            }
        }

        SetPlayerBattleStatus(ActionButtonType);

        if (ActionButtonType == "Attack")//공격이라면 BattleMgr에서 나온 결과로 현재 타겟의 체력을 깍음
        {
            TargetObject = MonMgr.CheckActiveMonsterHaveProvocation().gameObject;
            //몬스터중에 도발을 가지고 있는 녀석이 있다면 그녀석으로 CurrentTarget을 바꿔야함.
            GiveDebuffToMonsterByAttack(TargetObject.GetComponent<Monster>());
            //
            PlayerMgr.GetPlayerInfo().GetBuffByAttack();
            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(true, true);
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] >= 1)
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower]--;

            //충전을 공격시에 소모됨
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charging] >= 1)
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charging] = 0;

            //넘치는 힘 버프 보유시
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.OverWhelmingPower] >= 1)
            {//체력 + 보호막 보다 데미지가 높을때
                if (MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentHP + MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint
                    < BattleResultStatus.FinalResultAmount)
                {
                    List<GameObject> UnTargetMonsters = new List<GameObject>();
                    foreach(GameObject obj in MonMgr.GetActiveMonsters())
                    {
                        if(obj != MonMgr.CurrentTarget.gameObject)
                        {
                            UnTargetMonsters.Add(obj);
                        }
                    }
                    //각각의 몬스터에 들어갈 데미지 계산
                    float LastDamage = 
                        (BattleResultStatus.FinalResultAmount - MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentHP) / UnTargetMonsters.Count;
                    foreach(GameObject Mon in UnTargetMonsters)
                    {
                        Mon.GetComponent<Monster>().MonsterDamage((int)LastDamage);
                        GiveDebuffToMonsterByAttack(Mon.GetComponent<Monster>());
                    }
                }
            }
            //적이 가시값옷 보유시
            if (MonMgr.CurrentTarget.MonsterBuff.BuffList[(int)EBuffType.ThornArmor] >= 1)
            {//내가 주는 데미지가 적의 보호막에 다 막힐때 => 나의 데미지 < 적의 보호막
                if(BattleResultStatus.FinalResultAmount < MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint)
                {
                    float ReflectionDamage =
                        (MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint + MonMgr.CurrentTarget.MonsterBuff.BuffList[(int)EBuffType.Defense])
                        - BattleResultStatus.FinalResultAmount;

                    PlayerMgr.GetPlayerInfo().PlayerDamage((int)ReflectionDamage);
                }
            }
            //착취보유시
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Exploitation] >= 1)
            {
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount((int)(BattleResultStatus.FinalResultAmount / 5));
            }
            //흡혈보유시
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.LifeSteal] >= 1)
            {
                PlayerMgr.GetPlayerInfo().PlayerRegenHp((int)(BattleResultStatus.FinalResultAmount / 20));
            }
            MonMgr.CurrentTarget.MonsterDamage(BattleResultStatus.FinalResultAmount);
            if(MonMgr.CurrentTarget.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint >= 1)
            {//쉴드가 남아있으면
                IsTargetHasShield = true;
            }
        }
        else if (ActionButtonType == "Charm")
        {
            TargetObject = PlayerMgr.GetPlayerInfo().gameObject;
            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(true, true);
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] >= 1)
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower]--;

            PlayerMgr.GetPlayerInfo().PlayerDamage(BattleResultStatus.FinalResultAmount, false, true);
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount >= 1)
            {//쉴드가 남아있으면
                IsTargetHasShield = true;
            }
        }
        else if (ActionButtonType == "Defense")//방어라면 결과로 플레이어의 방어 수치를 높임
        {
            TargetObject = null;
            GiveDebuffToMosnterByDefense();

            PlayerMgr.GetPlayerInfo().GetBuffByDefense((int)BattleResultStatus.FinalResultAmount);
            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(true, false);
            PlayerMgr.GetPlayerInfo().PlayerGetShield(BattleResultStatus.FinalResultAmount);
        }
        else if (ActionButtonType == "Rest")//휴식이라면 결과로 플레이어의 피로도를 회복함
        {
            TargetObject = null;
            GiveDebuffToMonsterByRest();

            MonMgr.SetAcitveMonsterMountainLord();

            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] >= 1)
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear]--;

            PlayerMgr.GetPlayerInfo().GetBuffByRest();
            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(true, false);
            PlayerMgr.GetPlayerInfo().PlayerRegenSTA(BattleResultStatus.FinalResultAmount);
            //상급휴식 보유시
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.AdvancedRest] >= 1)
            {
                PlayerMgr.GetPlayerInfo().PlayerRegenHp((int)(BattleResultStatus.FinalResultAmount / 10));
            }
        }
        //이 함수를 실행시키면 현재 BattleMgr에는 플레이어 행동에 따른 결과값이 저장됨 -> 이걸 BattleUI가 잘 전달 받아서 작동하면 될듯?
        //이제 결정된 수치를 바탕으로 MainBattle을 활성화 시킴
        UIMgr.EDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.MEDI_UI.InActiveEquipmentDetailInfoUI();
        //여기서는 공격주체가 플레이어
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().gameObject, MonMgr.CurrentTarget, ActionButtonType, BattleResultStatus, 
            PlayerMgr.GetPlayerInfo().gameObject.transform.position, IsTargetHasShield, ProgressBattle);
    }
    protected void GiveDebuffToMonsterByAttack(Monster TargetMon)
    {
        int STRWeapon = 10; int DURWeapon = 11; int RESWeapon = 12;
        //이벤트 장비 유무 / 장비 성향/ 장비 종류에 따라서 적에게 디버프 부여 -> 만의 자리, 백의 자리, 십의 자리로 구분가능
        int IsEventEquip = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode / 10000;
        int EquipStateType = (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode / 100) % 10;
        int WeaponType = (IsEventEquip * 10) + EquipStateType;
        if (WeaponType == STRWeapon)
        {
            TargetMon.MonsterBuff.BuffList[(int)EBuffType.DefenseDebuff] += 3;
        }
        if (WeaponType == DURWeapon)
        {
            TargetMon.MonsterBuff.BuffList[(int)EBuffType.AttackDebuff] += 3;
        }
        if (WeaponType == RESWeapon)
        {
            TargetMon.MonsterBuff.BuffList[(int)EBuffType.Poison] += 5;
            TargetMon.MonsterBuff.BuffList[(int)EBuffType.Burn] += 3;
        }
    }
    protected void GiveDebuffToMosnterByDefense()
    {
        int RESArmor = 12;
        int IsEventEquip = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode / 10000;
        int EquipStateType = (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode / 100) % 10;
        int ArmorType = (IsEventEquip * 10) + EquipStateType;

        if(ArmorType == RESArmor)
        {
            foreach(GameObject Mon in MonMgr.GetActiveMonsters())
            {
                Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Misfortune] += 3;
            }
        }
    }
    protected void GiveDebuffToMonsterByRest()
    {
        int HPHelmet = 15;
        int IsEventEquip = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode / 10000;
        int EquipStateType = (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode / 100) % 10;
        int HelmetType = (IsEventEquip * 10) + EquipStateType;

        if(HelmetType == HPHelmet)
        {
            foreach (GameObject Mon in MonMgr.GetActiveMonsters())
            {
                Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charm] += 1;
            }
        }
    }

    public void MonsterBattleActionSelectionButtonClick()
    {
        if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charm] >= 1)
        {
            int GetCharm = CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charm];
            int RandCharmNum = Random.Range(1, 11);
            if(GetCharm >= RandCharmNum)
            {
                CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState = (int)EMonsterActionState.Charm;
            }
        }

        CalculateMonsterAction();
        SoundManager.Instance.PlayUISFX("UI_Button");
        string CurrentBattleState;
        bool IsTargetHasShield = false;
        bool IsMonsterAttack = false;
        bool IsAlreadyDamageCalculate = false;
        SpawnMonstersID.Clear();
        TargetObject = null;
        switch (CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState)
        {
            case (int)EMonsterActionState.Attack:
                //데미지가 플레이어가 가지고있는 쉴드보다 작거나 같다면
                CurrentBattleState = "Attack";
                TargetObject = PlayerMgr.GetPlayerInfo().gameObject;
                IsMonsterAttack = true;

                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] >= 1)
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] = 0;

                //플레이어가 가시 갑옷을 가지고 있을경우
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ThornArmor] >= 1)
                {//데미지 < 플레이어 보호막 일경우
                    if (BattleResultStatus.FinalResultAmount < PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount)
                    {
                        float ReflectionDamage = (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount + PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Defense])
                            - BattleResultStatus.FinalResultAmount;
                        CurrentTurnObject.GetComponent<Monster>().MonsterDamage(ReflectionDamage);
                    }
                }
                //강탈을 가지고 있을때
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Plunder] >= 1)
                {
                    int CurrentPlayerExperience = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience;
                    if (CurrentPlayerExperience >= BattleResultStatus.FinalResultAmount)//경험치만 깍고 끝
                    {
                        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-(int)BattleResultStatus.FinalResultAmount, true);
                        CurrentTurnObject.GetComponent<Monster>().AdditionalEXP += (int)(BattleResultStatus.FinalResultAmount * 0.5);
                        IsAlreadyDamageCalculate = true;
                    }
                    else//아니라면 경험치를 깍고 쉴드 및 체력을 깍음
                    {
                        CurrentTurnObject.GetComponent<Monster>().AdditionalEXP += (int)(CurrentPlayerExperience * 0.5);
                        float RemainDamange = BattleResultStatus.FinalResultAmount - CurrentPlayerExperience;
                        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-CurrentPlayerExperience, true);
                        PlayerMgr.GetPlayerInfo().PlayerDamage(RemainDamange);
                        IsAlreadyDamageCalculate = true;
                    }
                }

                switch(CurrentTurnObject.GetComponent<Monster>().MonsterName)
                {
                    case "Administrator_Hammer":
                        PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.DefenseDebuff, 
                            CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.DefenseDebuff));
                        break;
                    case "Administrator_Saw":
                        PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.AttackDebuff,
                            CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.AttackDebuff));
                        break;
                    case "Administrator_Syringe":
                        int[] DebuffArray = new int[3] { (int)EBuffType.Poison, (int)EBuffType.Weakness, (int)EBuffType.Slow };
                        int RandomDebuff = Random.Range(0, 3);
                        PlayerMgr.GetPlayerInfo().ApplyBuff(DebuffArray[RandomDebuff],
                            CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff(DebuffArray[RandomDebuff]));
                        break;
                }

                if(IsAlreadyDamageCalculate == false)
                {
                    PlayerMgr.GetPlayerInfo().PlayerDamage((int)BattleResultStatus.FinalResultAmount);
                }
                if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount >= 1 || 
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Defense] >= (int)BattleResultStatus.FinalResultAmount)
                {//여기에 들어오면 적응_힘 증가
                    
                    if(CurrentTurnObject.GetComponent<Monster>().MonsterName == "Guardian")
                    {
                        CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.StrengthAdaptation] += 1;
                    }
                    
                    IsTargetHasShield = true;
                }
                break;
            case (int)EMonsterActionState.Charm:
                CurrentBattleState = "Charm";
                TargetObject = CurrentTurnObject;
                IsMonsterAttack = true;

                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] >= 1)
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] = 0;

                CurrentTurnObject.GetComponent<Monster>().MonsterDamage((int)BattleResultStatus.FinalResultAmount);
                if(CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint >= 1)
                {
                    IsTargetHasShield = true;
                }
                break;
            case (int)EMonsterActionState.Defense:
                CurrentBattleState = "Defense";

                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] >= 1)
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] = 0;

                CurrentTurnObject.GetComponent<Monster>().MonsterGetShield(BattleResultStatus.FinalResultAmount);
                break;
            case (int)EMonsterActionState.SpawnMonster:
                SpawnMonstersID = CurrentTurnObject.GetComponent<Monster>().GetSummonMonsters();
                SummonerMonster = CurrentTurnObject;
                //여기서 다음 턴에 어떤 몬스터를 스폰할지 결정하면 될듯?
                CurrentBattleState = "Another";
                break;
            case (int)EMonsterActionState.ApplyLuck:
                CurrentBattleState = "Luck";
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.Luck);
                break;
            case (int)EMonsterActionState.GivePoison:
                CurrentBattleState = "Poison";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Poison, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Poison));
                break;
            case (int)EMonsterActionState.GiveMisFortune:
                CurrentBattleState = "MisFortune";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Misfortune, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Misfortune));
                break;
            case (int)EMonsterActionState.GiveCurseOfDeath:
                CurrentBattleState = "CurseOfDeath";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.CurseOfDeath, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.CurseOfDeath));
                break;
            case (int)EMonsterActionState.ApplyThornArmor:
                CurrentBattleState = "ThornArmor";
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.ThornArmor);
                break;
            case (int)EMonsterActionState.GiveCower:
                CurrentBattleState = "Cower";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Cower, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Cower));
                break;
            case (int)EMonsterActionState.ApplyCopyStrength:
                //사전강화 + 레벨업으로 인한 강화
                CurrentBattleState = "CopyStrength";
                int STRPlusAmount = (int)(JsonReadWriteManager.Instance.GetEarlyState("STR") + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().StrengthLevel);
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.CopyStrength, STRPlusAmount);
                break;
            case (int)EMonsterActionState.ApplyCopyDurability:
                CurrentBattleState = "CopyDurability";
                int DURPlusAmount = (int)(JsonReadWriteManager.Instance.GetEarlyState("DUR") + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DurabilityLevel);
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.CopyDurability, DURPlusAmount);
                break;
            case (int)EMonsterActionState.ApplyCopySpeed:
                CurrentBattleState = "CopySpeed";
                int SPDPlusAmount = (int)(JsonReadWriteManager.Instance.GetEarlyState("SPD") + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SpeedLevel);
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.CopySpeed, SPDPlusAmount);
                break;
            case (int)EMonsterActionState.ApplyCopyLuck:
                CurrentBattleState = "CopyLuck";
                int LUKPlusAmount = (int)(JsonReadWriteManager.Instance.GetEarlyState("LUK") + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().LuckLevel);
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.CopyLuck, LUKPlusAmount);
                break;
            case (int)EMonsterActionState.ApplyGreed:
                CurrentBattleState = "Greed";
                int GreedStackAmount = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience;
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.Greed, GreedStackAmount);
                break;
            case (int)EMonsterActionState.GiveEnvy:
                CurrentBattleState = "Envy";
                int EnvyStackAmount = (int)PlayerMgr.GetPlayerInfo().GetAllEquipTier();
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Envy, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Envy, EnvyStackAmount));
                break;
            case (int)EMonsterActionState.ConsumeGluttony:
                //Stack이 자기 자신의 최대 체력보다 많을때는 데미지
                int GluttonyMonMaxHP = (int)(CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterMaxHP);
                int GluttonyStack = CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Gluttony];
                if (GluttonyMonMaxHP >= GluttonyStack)
                {//체력이 더 많거나 같을때 -> 소화 가능 -> 졸개 소환
                    CurrentBattleState = "SurvantByGluttony";
                    SpawnMonstersID = CurrentTurnObject.GetComponent<Monster>().GetSummonMonsters();
                    SummonerMonster = CurrentTurnObject;
                }
                else
                {//체력이 더 적을때 -> 소화 불가능 -> 데미지
                    CurrentBattleState = "CantConsume";
                    //-로 데미지를 기록해서 전달함 -> 플레이어가 주는 데미지와 구분하기 위해서
                    CurrentTurnObject.GetComponent<Monster>().MonsterDamage(-(GluttonyStack * 0.5f));
                }
                    //Stack이 자기 자신으 최대 체력보다 적을때는 졸개 소환
                break;
            case (int)EMonsterActionState.GiveDefenseDebuff:
                CurrentBattleState = "DefenseDebuff";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.DefenseDebuff, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.DefenseDebuff));
                break;
            case (int)EMonsterActionState.ApplyRegeneration:
                CurrentBattleState = "Regeneration";
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.Regeneration);
                break;
            case (int)EMonsterActionState.GiveBurn:
                CurrentBattleState = "Burn";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Burn, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Burn));
                break;
            case (int)EMonsterActionState.GiveAttackDebuff:
                CurrentBattleState = "AttackDebuff";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.AttackDebuff, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.AttackDebuff));
                break;
            case (int)EMonsterActionState.GiveOverChargeToServant:
                CurrentBattleState = "OverCharge";
                MonMgr.GiveBuffToActiveServent((int)EBuffType.OverCharge, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.OverCharge), CurrentTurnObject);
                //살아있는 몬스터중에 졸개 버프를 가지고있고, 마스터 몬스터가 해당 턴의 몬스터인 모든 몬스터에게
                break;
            case (int)EMonsterActionState.GiveCharm:
                CurrentBattleState = "GiveCharm";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Charm, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Charm));
                break;
            case (int)EMonsterActionState.GivePetrification:
                CurrentBattleState = "Petrification";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Petrification, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Petrification));
                break;
            case (int)EMonsterActionState.ApplyCharging:
                CurrentBattleState = "Charging";
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.Charging);
                break;
            default:
                CurrentBattleState = "Another";
                break;
        }

        MonMgr.SetActiveMonsterChainAttack(true, IsMonsterAttack, CurrentTurnObject.GetComponent<Monster>());

        CurrentTurnObject.GetComponent<Monster>().CheckEnemyBuff(PlayerMgr.GetPlayerInfo().PlayerBuff);
        CurrentTurnObject.GetComponent<Monster>().CheckCanSummonMonster(SpawnMonstersID.Count, MonMgr.GetActiveMonsters().Count);
        CurrentTurnObject.GetComponent<Monster>().SetNextMonsterState();//몬스터의 행동 패턴에 따라 다음 행동 결정
        UIMgr.EDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.MEDI_UI.InActiveEquipmentDetailInfoUI();
        //여기선 공격주체가 몬스터
        UIMgr.B_UI.ActiveMainBattleUI(CurrentTurnObject.GetComponent<Monster>().gameObject, CurrentTurnObject.GetComponent<Monster>(), CurrentBattleState, BattleResultStatus,
            PlayerMgr.GetPlayerInfo().gameObject.transform.position, IsTargetHasShield, ProgressBattle);
    }
    public void PressVictoryButton()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 >= 200 &&
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 < 300)//아닐때는 보스일때만임
        {//여기서 클릭 됬을때 보스일때 따로 연출을 넣어야 할듯?
            Debug.Log("Boss");
            //CurrentPlayerActionDetail로 구분하는건 끝났음 0으로 바꾸기//CurrentFloor도 늘리게
            if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails / 1000 >= CurrentFinalStage)
            {//현재 개방된 것 보다 크거나 같으면 게임 승리임
                UIMgr.B_UI.ClickVictoryButton();
                PlayerMgr.GetPlayerInfo().CalculateEarlyPoint();
                UIMgr.B_UI.WinGame(PlayerMgr.GetPlayerInfo());
            }
            else
            {
                PlayerMgr.GetPlayerInfo().WinBossBattle();
                UIMgr.B_UI.ClickVictoryButton();//승리버튼 눌렀을때 UI끄고
                UIMgr.BossBattleWinFade();//Fade해야함//여기에 SetUI랑 이것저것 있음
                JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            }
        }
        else
        {
            UIMgr.B_UI.ClickVictoryButton();
            UIMgr.GI_UI.ActiveGettingUI();
            UIMgr.SetUI();
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        }
    }

    public void PressDefeatButton()//이겼을때도 똑같긴하네
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        //여기서 초기 강화 포인트를 얼마나 줄지 계산해야 되는거 아니여?
        if (JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 2)
        {//2이상이면 장비를 랜덤하게 인벤토리에 넣는다 -> 
            //초기화->하기 전에 장비에 대한 정보를 어디에 넣어놓고 작업하면 될듯?
            //JsonManager에 있는 정보 갱신은 전투를 시작할때 갱신됬으니 문제는 없고
            SetEquipSuccession();
        }
        else
        {
            JsonReadWriteManager.Instance.InitPlayerInfo(true);//초기화
            JsonReadWriteManager.Instance.InitEarlyStrengthenInfo(true);//ReachFloor와 EarlyPoint를 제외하고 초기화시킴
        }
        LoadingScene.Instance.LoadAnotherScene("TitleScene");
        //초기화 JsonManager의 P_Info 초기화
    }

    protected void SetEquipSuccession()
    {
        JsonReadWriteManager.Instance.InitPlayerInfo(true);//초기화
        JsonReadWriteManager.Instance.InitEarlyStrengthenInfo(true);//ReachFloor와 EarlyPoint를 제외하고 초기화시킴
        List<int> InPossessionEquip = new List<int>();
        //장비하고있는 장비 코드 저장
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode);
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode);
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode);
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode);
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode);
        //인벤토리에 있는 장비 코드저장
        for(int i = 0; i < (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP"); i++)
        {
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)
            {
                InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]);
            }
        }
        //다 저장했으면 계승 레벨에 맞게 n개의 랜덤한 장비를 뽑음
        for(int i = 0; i < (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIPSUC"); i++)
        {
            if (InPossessionEquip.Count <= 0)
                break;

            int RandNum = Random.Range(0, InPossessionEquip.Count);
            JsonReadWriteManager.Instance.P_Info.EquipmentInventory[i] = InPossessionEquip[RandNum];
            InPossessionEquip.RemoveAt(RandNum);
        }

    }

    public void PressTurnUIImage(int ButtonNum)//순서 칸에서 몬스터 버튼 눌렀을때 사용
    {
        GameObject obj;
        obj = GetBattleTurn()[ButtonNum];

        if (obj.tag == "Monster")
        {
            MonMgr.SetCurrentTargetMonster(obj.GetComponent<Monster>());
        }
    }
    protected void ActiveBuffEffectAtFirstTime()
    {
        if (CurrentTurnObject.tag == "Player")
        {
            if(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Petrification] > 0)
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Frost", PlayerMgr.GetPlayerInfo().gameObject.transform.position);
            }
            if(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] > 0)
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Fear", PlayerMgr.GetPlayerInfo().gameObject.transform.position);
            }
        }
        /*//몬스터는 딱히 걸리지 않는다? -> 턴 결정하는 매커니즘이 바뀌면 공포는 걸릴듯, 그리고 동상의 효과가 변한다던가?
        else if(CurrentTurnObject.tag == "Monster")
        {
            Monster MonInfo = CurrentTurnObject.GetComponent<Monster>();
            if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.FrostBite] > 0)
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Frost", MonInfo.gameObject.transform.position);
            }
            if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.Fear] > 0)
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Fear", MonInfo.gameObject.transform.position);
            }
        }
        */
    }
    protected void AfterBuffProgress()
    {
        //각 플레이어와 몬스터에 버프가 존재하면 버프의 턴을 1씩 감소시키고 그거에 맞춰서 데미지를 주거나 해야함
        for(int i = 0; i < (int)EBuffType.CountOfBuff; i++)
        {
            AfterBuffCalculate(i);
        }
    }
    protected void AfterBuffCalculate(int BuffsType)
    {
        if (CurrentTurnObject.tag == "Player" && MonMgr.GetActiveMonsters().Count > 0)
        {
            Vector3 PlayerBuffPos = PlayerMgr.GetPlayerInfo().gameObject.transform.position;
            PlayerBuffPos.y += 1.5f;
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[BuffsType] > 0)
            {
                switch (BuffsType)//1씩 줄어들지 않거나 데미지를 주거나 회복시키는것만
                {
                    case (int)EBuffType.Poison:
                        PlayerMgr.GetPlayerInfo().PlayerDamage(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison], true);
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison] = PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison] / 2;
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Poison", PlayerBuffPos);
                        break;
                    case (int)EBuffType.Recharge:
                        float RegenSTAAmount = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxSTA - PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA;
                        RegenSTAAmount = RegenSTAAmount / 20;
                        PlayerMgr.GetPlayerInfo().PlayerRegenSTA(RegenSTAAmount);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_ReCharge", PlayerBuffPos);
                        break;
                    case (int)EBuffType.UnDead:
                        if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP < 1)
                        {
                            PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP = 1;
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentHpRatio =
                                PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP / PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP;
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnDead] = 0;
                            EffectManager.Instance.ActiveEffect("BattleEffect_Buff_UnDead", PlayerBuffPos);
                        }
                        else
                        {//위의 조건이 아니라면 --
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnDead]--;
                        }
                        break;
                    case (int)EBuffType.Cower:
                        /*
                        while(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] >= 3)
                        {
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] -= 3;
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] += 1;
                        }
                        */
                        break;
                    case (int)EBuffType.Fear:
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Fear", PlayerBuffPos);
                        break;
                    case (int)EBuffType.OverCharge:
                        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.OverCharge] <= 1)
                        {
                            PlayerMgr.GetPlayerInfo().PlayerDamage(99999);
                        }
                        break;
                }

                //독, 죽음의 저주, 불사, 무방비, 방어력, 회복력, 연속 타격, 위축, 공포, 축적 제외
                //업보_선, 업보_악, 반사, 질투 제외
                if (BuffsType != (int)EBuffType.Poison && BuffsType != (int)EBuffType.CurseOfDeath &&
                    BuffsType != (int)EBuffType.UnDead && BuffsType != (int)EBuffType.Defenseless && 
                    BuffsType != (int)EBuffType.Defense && BuffsType != (int)EBuffType.Resilience &&
                    BuffsType != (int)EBuffType.ChainAttack && BuffsType != (int)EBuffType.Cower &&
                    BuffsType != (int)EBuffType.Fear && BuffsType != (int)EBuffType.Charging &&
                    BuffsType != (int)EBuffType.GoodKarma && BuffsType != (int)EBuffType.BadKarma &&
                    BuffsType != (int)EBuffType.Reflect && BuffsType != (int)EBuffType.Envy)//독 과 죽음의 저주, 불사, 무방비, 방어력, 회복력 따로 계산
                {
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[BuffsType]--;
                }
            }
        }
        else if (CurrentTurnObject.tag == "Monster")
        {
            Monster MonInfo = CurrentTurnObject.GetComponent<Monster>();
            if (MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP > 0 && MonInfo.MonsterBuff.BuffList[BuffsType] > 0)
            {//살아있는 놈들 중에서
                switch (BuffsType)//1씩 줄어들지 않거나 데미지를 주거나 회복시키는것만
                {
                    //재생 , 기충전, 화상, 독, 죽음의 저주, 재생형갑옷, 나약함, 불사(이놈은 마지막에 계산되야 될것 같은데)
                    case (int)EBuffType.Poison:
                        MonInfo.MonsterDamage(MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison]);
                        MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison] = MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison] / 2;
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Poison", MonInfo.gameObject.transform.position);
                        break;
                    case (int)EBuffType.CurseOfDeath:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath] <= 1)
                        {
                            float CurseOfDeathDamage = MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP * 9 / 10;
                            MonInfo.MonsterDamage((int)CurseOfDeathDamage);
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath] = 0;
                            EffectManager.Instance.ActiveEffect("BattleEffect_Buff_CurseOfDeath", MonInfo.gameObject.transform.position);
                        }
                        else
                        {
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath]--;
                        }
                        break;
                    case (int)EBuffType.UnDead:
                        MonInfo.MonsterBuff.BuffList[(int)EBuffType.UnDead]--;
                        /*
                        if (MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP < 1)
                        {
                            MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP = 1;
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.UnDead] = 0;
                            EffectManager.Instance.ActiveEffect("BattleEffect_Buff_UnDead", MonInfo.gameObject.transform.position);
                        }
                        else
                        {
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.UnDead]--;
                        }
                        */
                        break;
                    case (int)EBuffType.Servant:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.Servant] <= 1)
                        {
                            MonInfo.MonsterDamage(99999);
                        }
                        break;
                    case (int)EBuffType.SelfDestruct:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.SelfDestruct] <= 1)
                        {
                            MonInfo.MonsterDamage(99999);
                            PlayerMgr.GetPlayerInfo().PlayerDamage(50);
                        }
                        break;
                    case (int)EBuffType.OverCharge:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.OverCharge] <= 1)
                        {
                            MonInfo.MonsterDamage(99999);
                        }
                        break;
                    case (int)EBuffType.Lust:
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charm] += 1;
                        break;
                }

                //독, 죽음의 저주, 불사, 방어력, 연속타격, 산군, 복사_힘, 복사_내구, 복사_속도, 복사_행운, 흡수 제외
                //적응_힘, 적응_내구, 적응_속도, 축적, 반사 제외
                //교만, 탐욕, 질투, 색욕, 식탐, 분노 제외
                if (BuffsType != (int)EBuffType.Poison && BuffsType != (int)EBuffType.CurseOfDeath &&
                    BuffsType != (int)EBuffType.UnDead && BuffsType != (int)EBuffType.Defense &&
                    BuffsType != (int)EBuffType.ChainAttack && BuffsType != (int)EBuffType.MountainLord &&
                    BuffsType != (int)EBuffType.CopyStrength && BuffsType != (int)EBuffType.CopyDurability &&
                    BuffsType != (int)EBuffType.CopySpeed && BuffsType != (int)EBuffType.CopyLuck &&
                    BuffsType != (int)EBuffType.Consume && BuffsType != (int)EBuffType.StrengthAdaptation &&
                    BuffsType != (int)EBuffType.DurabilityAdaptation && BuffsType != (int)EBuffType.SpeedAdaptation &&
                    BuffsType != (int)EBuffType.Charging && BuffsType != (int)EBuffType.Reflect &&
                    BuffsType != (int)EBuffType.Pride && BuffsType != (int)EBuffType.Greed &&
                    BuffsType != (int)EBuffType.Envy && BuffsType != (int)EBuffType.Lust &&
                    BuffsType != (int)EBuffType.Gluttony && BuffsType != (int)EBuffType.Wrath)//독 과 죽음의 저주, 불사, 방어력은 따로 계산
                {
                    MonInfo.MonsterBuff.BuffList[BuffsType]--;
                }
            }
        }
    }

    protected void BeforeBuffProgress()
    {
        //각 플레이어와 몬스터에 버프가 존재하면 버프의 턴을 1씩 감소시키고 그거에 맞춰서 데미지를 주거나 해야함
        for (int i = 0; i < (int)EBuffType.CountOfBuff; i++)
        {
            BeforeBuffCalculate(i);
        }
    }

    protected void BeforeBuffCalculate(int BuffsType)
    {
        if (CurrentTurnObject.tag == "Player" && MonMgr.GetActiveMonsters().Count > 0)
        {
            Vector3 PlayerBuffPos = PlayerMgr.GetPlayerInfo().gameObject.transform.position;
            PlayerBuffPos.y += 1.5f;
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[BuffsType] > 0)
            {
                switch (BuffsType)//1씩 줄어들지 않거나 데미지를 주거나 회복시키는것만
                {
                    //재생 , 기충전, 화상, 독, 죽음의 저주, 재생형갑옷, 나약함, 불사(이놈은 마지막에 계산되야 될것 같은데)
                    case (int)EBuffType.Resilience:
                        //PlayerRegenSTA
                        PlayerMgr.GetPlayerInfo().PlayerRegenSTA(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Resilience]);
                        break;
                    case (int)EBuffType.Cower:
                        float DebuffSpendSTA = PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] * 20;
                        PlayerMgr.GetPlayerInfo().PlayerSpendSTA(DebuffSpendSTA);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Cower", PlayerBuffPos);
                        //BattleEffect_Buff_Cower
                        //durl
                        break;
                    case (int)EBuffType.Burn:
                        float BurnDamage = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP / 20;
                        PlayerMgr.GetPlayerInfo().PlayerDamage((int)BurnDamage, true);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Burn", PlayerBuffPos);
                        break;
                    case (int)EBuffType.RegenArmor:
                        PlayerMgr.GetPlayerInfo().PlayerGetShield(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.RegenArmor]);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_RegenArmor", PlayerBuffPos);
                        break;
                    case (int)EBuffType.Weakness:
                        float WeaknessSpendSTA = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA / 20;
                        PlayerMgr.GetPlayerInfo().PlayerSpendSTA(WeaknessSpendSTA);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Weakness", PlayerBuffPos);
                        break;
                    case (int)EBuffType.Regeneration:
                        float RegenHPAmount = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP - PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP;
                        RegenHPAmount = RegenHPAmount / 20;
                        PlayerMgr.GetPlayerInfo().PlayerRegenHp((int)RegenHPAmount);
                        //BattleEffect_Buff_Regeneration
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Regeneration", PlayerBuffPos);
                        break;
                    case (int)EBuffType.Recharge:
                        float RegenSTAAmount = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxSTA - PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA;
                        RegenSTAAmount = RegenSTAAmount / 20;
                        PlayerMgr.GetPlayerInfo().PlayerRegenSTA(RegenSTAAmount);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_ReCharge", PlayerBuffPos);
                        break;
                }
            }
        }
        else if (CurrentTurnObject.tag == "Monster")
        {
            Monster MonInfo = CurrentTurnObject.GetComponent<Monster>();
            if (MonInfo.MonsterBuff.BuffList[BuffsType] > 0)
            {
                switch (BuffsType)//1씩 줄어들지 않거나 데미지를 주거나 회복시키는것만
                {
                    //재생 , 기충전, 화상, 독, 죽음의 저주, 재생형갑옷, 나약함, 불사(이놈은 마지막에 계산되야 될것 같은데)
                    case (int)EBuffType.Burn:
                        float BurnDamage = MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP / 20;
                        MonInfo.MonsterDamage((int)BurnDamage);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Burn", MonInfo.gameObject.transform.position);
                        break;
                    case (int)EBuffType.RegenArmor:
                        MonInfo.MonsterGetShield(MonInfo.MonsterBuff.BuffList[(int)EBuffType.RegenArmor]);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_RegenArmor", MonInfo.gameObject.transform.position);
                        break;
                    case (int)EBuffType.Regeneration:
                        float RegenHPAmount = MonInfo.GetMonsterCurrentStatus().MonsterMaxHP - MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP;
                        RegenHPAmount = RegenHPAmount / 20;
                        MonInfo.MonsterRegenHP(RegenHPAmount);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Regeneration", MonInfo.gameObject.transform.position);
                        break;
                }
            }
        }
    }

    public void SetBattleTurn()
    {
        //여기에서 첫번째 꺼 다음꺼를 기준으로 삼아야함.
        /*
        if (BattleTurn.Count >= 1)
            BattleTurn.RemoveAt(0);//첫번째꺼를 지운다.(왜냐하면 여기에 크기가 1이상인 BattleTurn이 들어왔다는것은 이미 0번째에 있는 오브젝트는 턴을 쓴것임

        for(int i = BattleTurn.Count - 1; i >= 0; i--)
        {
            if (BattleTurn[i].tag == "Monster")
            {
                if (BattleTurn[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentHP <= 0)
                {
                    Debug.Log("Empty");
                    BattleTurn.RemoveAt(i);
                }
            }
        }
        float PlayerSPD = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().TotalSPD;

        float[] MonsterSPD = new float[MonMgr.GetActiveMonsters().Count];
        for (int i = 0; i < MonsterSPD.Length; i++)
        {
            MonsterSPD[i] = MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentSPD;
        }

        while(BattleTurn.Count < 9)
        {
            PlayerActiveGauge += PlayerSPD;
            //ActiveMonster의 갯수를 줄여도 MonsterActiveGuage의 갯수는 그대로 유지가 된다.
            //만약 index1인 몬스터가 죽으면 원래는 index2였던 몬스터가 index1이 쓰던 MonsterActiveGuage를 이어받게 되는 문제가 생긴다.
            for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge += MonsterSPD[i];
            }


            if(PlayerActiveGauge >= 100)
            {
                PlayerActiveGauge -= 100;
                BattleTurn.Add(PlayerMgr.GetPlayerInfo().gameObject);
            }

            List<int> ChargedMonster = new List<int>();
            for(int i = 0; i < MonsterSPD.Length; i++)
            {
                if (MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge >= 100)
                {
                    ChargedMonster.Add(i);
                    //MonsterActiveGuage[i] -= 100;
                    //BattleTurn.Enqueue(MonMgr.GetActiveMonsters()[i]);
                }
            }

            while(ChargedMonster.Count > 0)
            {
                float FastestMonster = MonMgr.GetActiveMonsters()[ChargedMonster[0]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge;
                int ChargedMonsterIndex = 0;
                for(int i = 0; i < ChargedMonster.Count; i++)
                {
                    if(FastestMonster < MonMgr.GetActiveMonsters()[ChargedMonster[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                    {
                        FastestMonster = MonMgr.GetActiveMonsters()[ChargedMonster[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge;
                        ChargedMonsterIndex = i;
                    }
                }
                //여기로 나오면 FastestMonster가 결정된거임
                MonMgr.GetActiveMonsters()[ChargedMonster[ChargedMonsterIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge -= 100;
                BattleTurn.Add(MonMgr.GetActiveMonsters()[ChargedMonster[ChargedMonsterIndex]]);
                ChargedMonster.RemoveAt(ChargedMonsterIndex);
            }
        }
        */
        //지금턴의 다음꺼를 첫번째로 잡는다.//Current Gauge에 nextGauge의 값을 넣는다.
        //BattleTurn의 Count가 2가 될때 NextGauge에 CurrentGuage를 저장한다.
        BattleTurn.Clear();
        List<int> ChargedObject = new List<int>();

        PlayerActiveGauge = NextPlayerActiveGauge;
        if (PlayerActiveGauge >= 100)
        {
            ChargedObject.Add(-1);
        }
        for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
        {
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge =
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge;
            if (MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge >= 100)
            {
                ChargedObject.Add(i);
            }
        }
        //초기 계산
        while (ChargedObject.Count >= 1)
        {
            //MostFastMonster는 Active몬스터의 Index임
            int MostFastObjectIndex = 0;
            for(int i = 1; i < ChargedObject.Count; i++)
            {
                if (ChargedObject[MostFastObjectIndex] == -1)
                {//가장 우선 순위가 높은 오브젝트가 플레이어 일때
                    if (PlayerActiveGauge <=
                        MonMgr.GetActiveMonsters()[ChargedObject[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                    {//몬스터가 플레이어 보다 우선이라면 MostFastObjectIndex를 변경함
                        MostFastObjectIndex = i;
                    }
                }
                else
                {//가장 우선순위가 높은 오브젝트가 플레이어가 아닐때
                    if (MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge <=
                        MonMgr.GetActiveMonsters()[ChargedObject[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                    {
                        MostFastObjectIndex = i;
                    }
                }
            }
            //여기까지 오면 처음부터 끝까지 다 검사 한거임 //제일 Gauge의 값이 큰 놈이 들어오게 됨
            if(ChargedObject[MostFastObjectIndex] == -1)
            {//Gauge가 가장 큰놈이 플레이어 일때
                PlayerActiveGauge -= 100;
                BattleTurn.Add(PlayerMgr.GetPlayerInfo().gameObject);
                RecordNextActiveGauge();
            }
            else
            {//가장 큰놈이 플레이어가 아닐때
                MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge -= 100;
                BattleTurn.Add(MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]]);
                RecordNextActiveGauge();
            }
            ChargedObject.RemoveAt(MostFastObjectIndex);//MostFastMonster는 겹치지 않음 RemoveAt이 아니여도 문제 없음
        }
        //나머지 채우기
        while(BattleTurn.Count < 6 )//TurnUI가 6칸이여서 Count = 6까지만 나오면 될듯함
        {//6보다 작을때 While이 작동함( 6이 되면 벗어남)
            //Speed값에 따라 ActionGuage 증가 및 게이지의 수가 100이 넘어간 오브젝트들 저장
            ChargedObject.Clear();
            PlayerActiveGauge += PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().TotalSPD;
            if (PlayerActiveGauge >= 100)
            {
                ChargedObject.Add(-1);
            }
            for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge +=
                    MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentSPD;
                if (MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge >= 100)
                {
                    ChargedObject.Add(i);
                }
            }
            //턴 계산
            while (ChargedObject.Count >= 1)
            {
                //MostFastMonster는 Active몬스터의 Index임
                int MostFastObjectIndex = 0;
                for (int i = 1; i < ChargedObject.Count; i++)
                {
                    if (ChargedObject[MostFastObjectIndex] == -1)
                    {//가장 우선 순위가 높은 오브젝트가 플레이어 일때
                        if (PlayerActiveGauge <=
                            MonMgr.GetActiveMonsters()[ChargedObject[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                        {//몬스터가 플레이어 보다 우선이라면 MostFastObjectIndex를 변경함
                            MostFastObjectIndex = i;
                        }
                    }
                    else
                    {//가장 우선순위가 높은 오브젝트가 플레이어가 아닐때
                        if (MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge <=
                            MonMgr.GetActiveMonsters()[ChargedObject[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                        {
                            MostFastObjectIndex = i;
                        }
                    }
                }
                //여기까지 오면 처음부터 끝까지 다 검사 한거임 //제일 Gauge의 값이 큰 놈이 들어오게 됨
                if (ChargedObject[MostFastObjectIndex] == -1)
                {//Gauge가 가장 큰놈이 플레이어 일때
                    PlayerActiveGauge -= 100;
                    BattleTurn.Add(PlayerMgr.GetPlayerInfo().gameObject);
                    RecordNextActiveGauge();
                }
                else
                {//가장 큰놈이 플레이어가 아닐때
                    MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge -= 100;
                    BattleTurn.Add(MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]]);
                    RecordNextActiveGauge();
                }
                ChargedObject.RemoveAt(MostFastObjectIndex);//MostFastMonster는 겹치지 않음 RemoveAt이 아니여도 문제 없음
            }
        }

        CurrentState = (int)EBattleStates.Idle;
    }

    protected void RecordNextActiveGauge()
    {
        if(BattleTurn.Count == 2)//->0,1이 존재 한다는 거임
        {//BattleTurn 1에 해당하는 놈이 누구냐에 따라 걔가 쓰는 NextActionGauge에 100을 더해 준다.
            NextPlayerActiveGauge = PlayerActiveGauge;
            //Debug.Log(NextPlayerActiveGauge);
            for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge =
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge;
                //Debug.Log(MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge);
            }

            if (BattleTurn[1].tag == "Player")
            {
                NextPlayerActiveGauge += 100;
            }
            else if (BattleTurn[1].tag == "Monster")
            {
                BattleTurn[1].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge += 100;
            }
        }
    }

    /*
    protected void RecordPlayerNMonsterBeforeShield()//DecideCurrentBattleTurn 이 실행되기 전에 실행되야함
    {//몬스터나 플레이어가 맞았을때도 확인해야 할텐데?
        PlayerMgr.GetPlayerInfo().RecordBeforeShield();
        foreach (GameObject Mon in MonMgr.GetActiveMonsters())
        {
            Mon.GetComponent<Monster>().RecordMonsterBeforeShield();
        }
    }
    */

    protected void DecideCurrentBattleTurn()
    {
        if(CurrentState == (int)EBattleStates.Idle)
        {
            if(CurrentTurnObject != null)
            {
                if (CurrentTurnObject != PlayerMgr.GetPlayerInfo().gameObject)//전의 턴에 있던게 플레이어가 아닐때
                {
                    //(TargetObject != null && TargetObject != PlayerMgr.GetPlayerInfo().gameObject)
                    if(TargetObject != null)//Null이 아닐때는 그 타겟이 내가 아니여야 갱신가능
                    {
                        if (TargetObject != PlayerMgr.GetPlayerInfo().gameObject)
                        {
                            PlayerMgr.GetPlayerInfo().RecordBeforeShield();
                        }
                    }
                    else//타겟이 딱히 정해진게 아니니까 갱신가능
                    {
                        PlayerMgr.GetPlayerInfo().RecordBeforeShield();
                    }
                }
                foreach (GameObject Mon in MonMgr.GetActiveMonsters())
                {
                    if(CurrentTurnObject != Mon)//전의 턴이였던게 해당 몬스터가 아닌놈들만 
                    {
                        //(TargetObject != null && TargetObject != Mon)
                        if(TargetObject != null)
                        {
                            if(TargetObject != Mon)
                            {
                                //여기에 해당한는 몬스터들은 RegenArmor가 있을때 Record를 하지 않는다?
                                if (Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.RegenArmor] <= 0)
                                    Mon.GetComponent<Monster>().RecordMonsterBeforeShield();
                            }
                        }
                        else
                        {
                            //여기에 해당한는 몬스터들은 RegenArmor가 있을때 Record를 하지 않는다?
                            if (Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.RegenArmor] <= 0)
                                Mon.GetComponent<Monster>().RecordMonsterBeforeShield();
                        }
                    }
                }
            }
            else
            {
                PlayerMgr.GetPlayerInfo().RecordBeforeShield();
                foreach (GameObject Mon in MonMgr.GetActiveMonsters())
                {
                    Mon.GetComponent<Monster>().RecordMonsterBeforeShield();
                }
            }

            if(CurrentTurnObject != null)
            {
                if(CurrentTurnObject.tag == "Player" && CurrentTurnObject == BattleTurn[0])
                {//전턴의 오브젝트가 플레이어이고 전턴의 오브젝트가 이번턴에도 행동할때
                    foreach (GameObject Mon in MonMgr.GetActiveMonsters())
                    {
                        
                        if(Mon.GetComponent<Monster>().MonsterName == "Guardian")
                        {
                            Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.SpeedAdaptation] += 1;
                        }
                        
                    }
                }
            }
            //이 줄 위쪽의 CurrentTurnObject는 전턴의 오브젝트임
            //살아있는 몬스터중 오만을 가지고 있는 몬스터가 있다면
            foreach(GameObject Mon in MonMgr.GetActiveMonsters())
            {
                if (Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Pride] >= 1)
                {
                    int PercentOfPride = Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Pride];
                    int RandPrideNum = Random.Range(1, 101);
                    if(PercentOfPride >= RandPrideNum)
                    {//여기 들어오면 해당 몬스터의 턴이 됨
                        BattleTurn[0] = Mon;
                    }
                }
            }

            CurrentTurnObject = BattleTurn[0];
            //BattleTurn.RemoveAt(0);
            if (CurrentTurnObject.tag == "Player")
            {
                CurrentState = (int)EBattleStates.PlayerTurn;
                ActionOfStartPlayerTurn();
            }
            else if (CurrentTurnObject.tag == "Monster")
            {
                CurrentState = (int)EBattleStates.MonsterTurn;
                ActionOfStartMonsterTurn(CurrentTurnObject);
            }
        }
    }



    protected void ActionOfStartPlayerTurn()//플레이어턴이 되었을때
    {
        //버프의 턴수를 줄인다던가, 체력이 감소한다던가 하는것들
        PlayerMgr.GetPlayerInfo().RecordBeforeShield();
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnbreakableArmor] >= 1)
        {
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount = 
                (int)(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount * 0.5f);
        }
        else
        {
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount = 0;
        }
        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Defenseless] = 0;
    }

    protected void ActionOfStartMonsterTurn(GameObject Monster)
    {
        Monster.GetComponent<Monster>().RecordMonsterBeforeShield();
        if (Monster.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.UnbreakableArmor] >= 1)
        {
            Monster.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint =
                (int)(Monster.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint * 0.5f);
        }
        else
        {
            Monster.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint = 0;
        }
        Monster.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Defenseless] = 0;

        if(Monster.GetComponent<Monster>().MonsterName == "MountainLord")
        {
            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] += 1;
        }
    }

    //CalculatePlayerAction
    protected void SetPlayerBattleStatus(string ActionButtonType)
    {
        PlayerInfo P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        TotalPlayerState TP_Info = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo();
        float AllEquipTier = PlayerMgr.GetPlayerInfo().GetAllEquipTier();

        int EquipCode = 0;
        BattleResultStatus.BaseAmountPlus = 0;
        switch (ActionButtonType)
        {
            case "Charm":
            case "Attack":
                EquipCode = P_Info.EquipWeaponCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalSTR;
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ChainAttack] >= 1)
                {
                    BattleResultStatus.BaseAmountPlus += PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ChainAttack];
                }
                break;
            case "Defense":
                EquipCode = P_Info.EquipArmorCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalDUR;
                break;
            case "Rest":
                EquipCode = P_Info.EquipHatCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalRES;
                break;
        }
        EquipmentInfo Equipment_Info = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(EquipCode);


        //기초 추가 수치 // 거의 유일하게 EQUIP 초반 강화 효과에 영향을 받는 수치임
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.WeaponMaster] >= 1)//웨폰마스터 버프를 보유중일때
        {
            //플레이어간 끼고 있는 장비들을 포함한 모든 장비들의 티어의 합
            BattleResultStatus.BaseAmountPlus += AllEquipTier;
        }
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.BloodFamiliy] >= 1)
        {
            int TenPercentHP = (int)(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP * 0.03f);
            BattleResultStatus.BaseAmountPlus += TenPercentHP;
        }

        //배율들//무기의 slot을 랜덤으로 결정해야함, luck에 영향을 받아서 결정됨
        //각 슬롯의 슬롯 스테이트 수만큼
        BattleResultStatus.ResultMagnification.Clear();
        for (int i = 0; i < Equipment_Info.EquipmentSlots.Length; i++)
        {
            List<float> NegativeList = new List<float>();
            List<float> PositiveList = new List<float>();
            float WholeSlotStateAmount = Equipment_Info.EquipmentSlots[i].SlotState.Length;
            float MultiplyNum = 60 / WholeSlotStateAmount;
            float NegativeAmount = 0f;
            float PositiveAmount = 0f;
            for (int j = 0; j < Equipment_Info.EquipmentSlots[i].SlotState.Length; j++)
            {
                if (Equipment_Info.EquipmentSlots[i].IsPositive[j] == true)
                {
                    PositiveAmount++;
                    PositiveList.Add(Equipment_Info.EquipmentSlots[i].SlotState[j]);
                }
                else
                {
                    NegativeAmount++;
                    NegativeList.Add(Equipment_Info.EquipmentSlots[i].SlotState[j]);
                }
            }
            //Debug.Log("부정 : 긍정 = 1 : 2 => 부정 : 0 ~ 20"  + " 긍정 : 20 ~ " + (60 + (int)TP_Info.TotalLUK + LuckBuffNum));
            //Luk이 겁나 낮아지는거 예외처리
            int RandNum;
            if (60 + (int)TP_Info.TotalLUK <= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)TP_Info.TotalLUK);
            }

            if(NegativeAmount == 0)//긍정에서 하나 뽑아서 저장
            {
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
            else if(PositiveAmount == 0)//부정에서 하나 뽑아서 저장
            {
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //부정적 List에서 하나를 뽑아서 저장
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)TP_Info.TotalLUK)
            {
                //긍정적 List에서 하나를 뽑아서 저장
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
        }
        //버프에 의한 배율
        BattleResultStatus.BuffMagnification = 1f;
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.PowerOfDeath] >= 1)
        {
            float HPRatio = (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP - 1) / (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP - 1);
            float PowerOfDeathMagnification = Mathf.Lerp(3, 1, HPRatio);
            BattleResultStatus.BuffMagnification *= PowerOfDeathMagnification;
        }

        switch (ActionButtonType)
        {
            case "Charm":
            case "Attack":
                //공깍
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.AttackDebuff] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 0.5f;
                }
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ToughFist] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 2f;
                }
                break;
            case "Defense":
                //방깍
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.DefenseDebuff] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 0.5f;
                }
                //방업 보유시
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ToughSkin] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 2f;
                }
                break;
            case "Rest":
                break;
        }
        BattleResultStatus.FinalResultAmountPlus = 0f;
        switch(ActionButtonType)
        {
            case "Charm":
            case "Attack":
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.EXPPower] >= 1)//경험은 힘 버프 보유시
                {
                    BattleResultStatus.FinalResultAmountPlus += (int)(P_Info.Experience / 100f);
                }
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    BattleResultStatus.FinalResultAmountPlus += PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect];
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] = 0;
                }
                break;
            case "Defense":
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] = (int)(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] * 0.5f);
                }
                break;
            case "Rest":
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] = (int)(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] * 0.5f);
                }
                break;
        }
        //최종 추가 수치 // 거의 유일하게 EXPMG 초반 강화 효과에 영향을 받는 수치임
        
        //최종 수치 -> (기초 수치 + 기초 추가 수치) * 배율들 안의 배율 1 * 배율들 안의 배율 2 ...... * 배율들 안의 배율 n + 최종 추가 수치;
        float FinalMultiplyNum = 1f;
        for (int i = 0; i < BattleResultStatus.ResultMagnification.Count; i++)
        {
            FinalMultiplyNum *= BattleResultStatus.ResultMagnification[i];
        }
        BattleResultStatus.FinalResultAmount =
            (int)(((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * (FinalMultiplyNum * BattleResultStatus.BuffMagnification)) + BattleResultStatus.FinalResultAmountPlus);
        //BattleResultStatus.FinalResultAmount = (int)(((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * FinalMultiplyNum) + BattleResultStatus.FinalResultAmountPlus);
    }

    public void CalculateMonsterAction()
    {
        MonsterCurrentStatus MC_Info = CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus();
        int CurrentState = CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState;
        int MonEquipmentCode = 0;
        BattleResultStatus.BaseAmount = 0f;
        BattleResultStatus.BaseAmountPlus = 0f;
        BattleResultStatus.ResultMagnification.Clear();
        BattleResultStatus.ResultMagnification.Add(1);//혹시모를 오류 방지
        BattleResultStatus.BuffMagnification = 1f;
        BattleResultStatus.FinalResultAmountPlus = 0f;
        BattleResultStatus.FinalResultAmount = 0f;

        switch (CurrentState)
        {
            case (int)EMonsterActionState.Charm:
            case (int)EMonsterActionState.Attack:
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterWeaponCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentATK;
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ChainAttack] >= 1)
                {
                    BattleResultStatus.BaseAmountPlus += CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ChainAttack];
                }
                break;
            case (int)EMonsterActionState.Defense:
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterArmorCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentDUR;
                break;
        }
        EquipmentInfo ESO_Info = EquipmentInfoManager.Instance.GetMonEquipmentInfo(MonEquipmentCode);

        if(BattleResultStatus.BaseAmount > 0)
        {
            //여기 들어왔을때 ResultMagnification 비우기
            BattleResultStatus.ResultMagnification.Clear();
            SetMonsterBattleStatus(MC_Info, ESO_Info, CurrentState);
        }
    }

    protected void SetMonsterBattleStatus(MonsterCurrentStatus MC_Info, EquipmentInfo ESO_Info, int CurrentState)
    {
        if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.BloodFamiliy] >= 1)
        {
            BattleResultStatus.BaseAmountPlus += (int)(MC_Info.MonsterCurrentHP / 10);
        }

        if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.PowerOfDeath] >= 1)
        {
            float HPRatio = (MC_Info.MonsterCurrentHP - 1) / (MC_Info.MonsterMaxHP - 1);
            float PowerOfDeathMagnification = Mathf.Lerp(3, 1, HPRatio);
            BattleResultStatus.BuffMagnification *= PowerOfDeathMagnification;
        }
        switch (CurrentState)
        {
            case (int)EMonsterActionState.Charm:
            case (int)EMonsterActionState.Attack:
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.AttackDebuff] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 0.5f;
                }
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ToughFist] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 2f;
                }
                break;
            case (int)EMonsterActionState.Defense:
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.DefenseDebuff] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 0.5f;
                }
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ToughSkin] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 2f;
                }
                break;
        }
        //BattleResultStatus.FinalResultPlus
        switch (CurrentState)
        {
            case (int)EMonsterActionState.Charm:
            case (int)EMonsterActionState.Attack:
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    BattleResultStatus.FinalResultAmountPlus += CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect];
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] = 0;
                }
                break;
            default:
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] =
                        (int)(CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] * 0.5f);
                }
                break;
        }
        //BattleResult에 결과값 저장
        for (int i = 0; i < ESO_Info.EquipmentSlots.Length; i++)
        {
            List<float> NegativeList = new List<float>();
            List<float> PositiveList = new List<float>();
            float WholeSlotStateAmount = ESO_Info.EquipmentSlots[i].SlotState.Length;
            float MultiplyNum = 60 / WholeSlotStateAmount;
            float NegativeAmount = 0f;
            float PositiveAmount = 0f;
            for (int j = 0; j < ESO_Info.EquipmentSlots[i].SlotState.Length; j++)
            {
                if (ESO_Info.EquipmentSlots[i].IsPositive[j] == true)
                {
                    PositiveAmount++;
                    PositiveList.Add(ESO_Info.EquipmentSlots[i].SlotState[j]);
                }
                else
                {
                    NegativeAmount++;
                    NegativeList.Add(ESO_Info.EquipmentSlots[i].SlotState[j]);
                }
            }

            //Debug.Log("부정 : 긍정 = 1 : 1 => 부정 : 0 ~ 30" + " 긍정 : 30 ~ " + (60 + (int)MC_Info.MonsterCurrentLUK + LuckBuffNum));
            //Luk이 겁나 낮아지는거 예외처리
            int RandNum;
            if (60 + (int)MC_Info.MonsterCurrentLUK <= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)MC_Info.MonsterCurrentLUK);
            }

            if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //부정적 List에서 하나를 뽑아서 저장
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)MC_Info.MonsterCurrentLUK)
            {
                //긍정적 List에서 하나를 뽑아서 저장
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
        }

        float FinalMultiplyNum = 1f;
        for (int i = 0; i < BattleResultStatus.ResultMagnification.Count; i++)
        {
            FinalMultiplyNum *= BattleResultStatus.ResultMagnification[i];
        }
        BattleResultStatus.FinalResultAmount =
            (int)(((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * (FinalMultiplyNum * BattleResultStatus.BuffMagnification)) + BattleResultStatus.FinalResultAmountPlus);
        //BattleResultStatus.FinalResultAmount = (int)(((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * FinalMultiplyNum) + BattleResultStatus.FinalResultAmountPlus);
    }
}
