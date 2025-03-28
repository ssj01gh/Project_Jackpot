using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

public class BattleManager : MonoBehaviour
{
    protected List<GameObject> BattleTurn = new List<GameObject>();
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

    protected bool IgnoreBuffProgressAtFirstBattleProgress = false;
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

    public void InitCurrentBattleMonsters()//시작할때 한번만 실행됨
    {
        IgnoreBuffProgressAtFirstBattleProgress = true;
        MonMgr.SetSpawnPattern(PlayerMgr);//몬스터의 스폰 패턴 설정
        MonMgr.SpawnCurrentSpawnPatternMonster();//스폰 패턴에 맞게 몬스터 생성//ActiveMonster설정
        //스폰 패턴이 저장된후 다시 저장
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    public void InitMonsterNPlayerActiveGuage()//초기화
    {
        BattleTurn.Clear();
        PlayerActiveGauge = 0;
        for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
        {
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge = 0;
        }

        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack != -1)//습격을 당했다면
        {
            if (MonMgr.GetActiveMonsters().Count >= 1)
            {
                //SetBattleTurn에서 첫번째꺼를 지우기 때문에
                //ActiveMonster[0]을 한번 넣은 다음에 다시 넣어야함
                BattleTurn.Add(MonMgr.GetActiveMonsters()[0]);
            }
            for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                BattleTurn.Add(MonMgr.GetActiveMonsters()[i]);
            }
        }
    }

    public void ProgressBattle()//이게 전투가 계속되는 동안 지속적으로 불러와져야될 함수임
    {
        //여기에서 특정 버프의 데미지라던가 줘야할듯(턴당이니까)
        if(IgnoreBuffProgressAtFirstBattleProgress == true)
            IgnoreBuffProgressAtFirstBattleProgress = false;
        else
            BuffProgress();//이거 처음은 무시하게해야함

        MonMgr.CheckActiveMonstersRSurvive();//현재 스폰된 몬스터중 죽은 몬스터 정리
        SetBattleTurn();//플레이어와 몬스터의 SPD값에 영향을 받은 Turn을 결정->아마 여기서 확인할듯?
        DecideCurrentBattleTurn();//여기서 현재 누구의 차례인지 결정
        UIMgr.B_UI.SetBattleTurnUI(BattleTurn);//결정된 Turn을 ui에 표시
        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//플레이어 정보도 갱신
        MonMgr.SetCurrentTargetMonster(null);//CurrentTarget초기화
        UIMgr.B_UI.UnDisplayMonsterDetailUI();//몬스터의 상세 표시 UnActive//상세 스테이터스와 장비같은 것들;
        UIMgr.B_UI.SetPlayerBattleUI(PlayerMgr.GetPlayerInfo());
        UIMgr.B_UI.SetMonsterBattleUI(MonMgr.GetActiveMonsters());
        //UIMgr.B_UI.ActivePlayerShieldNBuffUI(PlayerMgr.GetPlayerInfo());//이거 전에 몬스터가 죽엇는지 아닌지 확인해야됨

        if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP <= 0)
        {
            //스프라이트가 쓰러지는 스프라이트가 재생된다던가?
            //ActiveMonster전부다 해제
            MonMgr.InActiveAllActiveMonster();
            //UIMgr.B_UI.ActivePlayerShieldNBuffUI(PlayerMgr.GetPlayerInfo());//여기있는 Set~~은 다 없애기 위함
            PlayerMgr.GetPlayerInfo().DefeatFromBattle();
            UIMgr.B_UI.DefeatBattle(PlayerMgr.GetPlayerInfo());
            UIMgr.PlayerDefeat();//플레이어의 장비창, 스탯창, 현재 스테이지 진행창 같은것들
            //다른 플레이어 UI도 없에기
            return;
        }
        if (MonMgr.GetActiveMonsters().Count <= 0)
        {
            Debug.Log("Winner");
            //현재의 상대가 보스라는것을 확일할 방법이 있는가?//스테이지마다 마지막을 199이런식으로 한다? %100을 했을때 99라면 보스인거지
            //%100의 값이 90이상이면 보스인거임. 보스가 1개만 있을수는 없으니
            if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 100 >= 90)
            {
                //PlayerMgr.GetPlayerInfo().DefeatFromBattle();
                //UIMgr.B_UI.WinGame(PlayerMgr.GetPlayerInfo());
                //UIMgr.PlayerDefeat();
            }
            else//보스가 아니라면 평범하게
            {
                PlayerMgr.GetPlayerInfo().EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화
                                                        //휴식중에 습격을 받은거라면 다시 휴식 행동 선택으로 돌아감
                int RewardEXP = PlayerMgr.GetPlayerInfo().ReturnEXPByEXPMagnification((int)MonMgr.CurrentSpawnPatternReward);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RewardEXP, true);
                UIMgr.B_UI.VictoryBattle(RewardEXP);
            }
            return;
            //다 죽으면 게임을 전투를 끝낸다.
        }


        if (CurrentState == (int)EBattleStates.PlayerTurn)
        {
            //플레이어의 턴이라면 플레이어의 행동을 결정하는 버튼을 나타나게 한다.
            UIMgr.B_UI.ActiveBattleSelectionUI(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] >= 1);//행동 결정 버튼이 나타남
        }
        else if (CurrentState == (int)EBattleStates.MonsterTurn)
        {
            //몬스터의 턴이라는 행동 버튼이 나타남
            UIMgr.B_UI.ActiveBattleSelectionUI_Mon();
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
        //행동하기전에 감소해야 할것들은 여기서 할듯?//여기가 아니라 턴 정해질때 감소해야함//DecideTurn에서
        //PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount = 0;
        SetPlayerBattleStatus(ActionButtonType);

        if (ActionButtonType == "Attack")//공격이라면 BattleMgr에서 나온 결과로 현재 타겟의 체력을 깍음
        {
            PlayerMgr.GetPlayerInfo().PlayerRecordGiveDamage(BattleResultStatus.FinalResultAmount);
            TargetObject = MonMgr.CurrentTarget.gameObject;
            //넘치는 힘 버프 보유시
            if(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.OverWhelmingPower] >= 1)
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
                    }
                }
            }
            //적이 가시값옷 보유시
            if (MonMgr.CurrentTarget.MonsterBuff.BuffList[(int)EBuffType.ThronArmor] >= 1)
            {//내가 주는 데미지가 적의 보호막에 다 막힐때 => 나의 데미지 < 적의 보호막
                if(BattleResultStatus.FinalResultAmount < MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint)
                {
                    float ReflectionDamage =
                        MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint - BattleResultStatus.FinalResultAmount;

                    PlayerMgr.GetPlayerInfo().PlayerDamage((int)ReflectionDamage);
                }
            }
            //강탈 보유시
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Rapine] >= 1)
            {
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount((int)(BattleResultStatus.FinalResultAmount / 5));
            }
            MonMgr.CurrentTarget.MonsterDamage(BattleResultStatus.FinalResultAmount);
        }
        else if (ActionButtonType == "Defense")//방어라면 결과로 플레이어의 방어 수치를 높임
        {
            TargetObject = null;
            PlayerMgr.GetPlayerInfo().PlayerGetShield(BattleResultStatus.FinalResultAmount);
        }
        else if (ActionButtonType == "Rest")//휴식이라면 결과로 플레이어의 피로도를 회복함
        {
            TargetObject = null;
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
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentForm, MonMgr.CurrentTarget, ActionButtonType, BattleResultStatus, CurrentState, ProgressBattle);
    }

    public void MonsterBattleActionSelectionButtonClick()
    {
        CalculateMonsterAction();
        string CurrentBattleState;
        switch (CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState)
        {
            case (int)EMonsterActionState.Attack:
                //데미지가 플레이어가 가지고있는 쉴드보다 작거나 같다면
                CurrentBattleState = "Attack";
                TargetObject = PlayerMgr.GetPlayerInfo().gameObject;
                //플레이어가 가시 갑옷을 가지고 있을경우
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ThronArmor] >= 1)
                {//데미지 < 플레이어 보호막 일경우
                    if (BattleResultStatus.FinalResultAmount < PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount)
                    {
                        float ReflectionDamage = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount - BattleResultStatus.FinalResultAmount;
                        CurrentTurnObject.GetComponent<Monster>().MonsterDamage(ReflectionDamage);
                    }
                }
                //강탈을 가지고 있을때
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Rapine] >= 1)
                {
                    PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-(int)(BattleResultStatus.FinalResultAmount / 5), true);
                }
                PlayerMgr.GetPlayerInfo().PlayerDamage(BattleResultStatus.FinalResultAmount);
                break;
            case (int)EMonsterActionState.Defense:
                CurrentBattleState = "Defense";
                TargetObject = null;
                CurrentTurnObject.GetComponent<Monster>().MonsterGetShield(BattleResultStatus.FinalResultAmount);
                break;
            default:
                CurrentBattleState = "Another";
                break;
        }

        CurrentTurnObject.GetComponent<Monster>().SetNextMonsterState();//몬스터의 행동 패턴에 따라 다음 행동 결정
        UIMgr.EDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.MEDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentForm, CurrentTurnObject.GetComponent<Monster>(), CurrentBattleState, BattleResultStatus, CurrentState, ProgressBattle);
    }
    public void PressVictoryButton()
    {
        UIMgr.B_UI.ClickVictoryButton();
        UIMgr.GI_UI.ActiveGettingUI();
        UIMgr.SetUI();
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
    }

    public void PressDefeatButton()//이겼을때도 똑같긴하네
    {
        if(JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 2)
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
    protected void BuffProgress()
    {
        //각 플레이어와 몬스터에 버프가 존재하면 버프의 턴을 1씩 감소시키고 그거에 맞춰서 데미지를 주거나 해야함
        for(int i = 0; i < (int)EBuffType.CountOfBuff; i++)
        {
            BuffCalculate(i);
        }
    }
    protected void BuffCalculate(int BuffsType)
    {
        //플레이어의 버프 계산
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[BuffsType] > 0)
        {
            switch (BuffsType)//1씩 줄어들지 않거나 데미지를 주거나 회복시키는것만
            {
                //재생 , 기충전, 화상, 독, 죽음의 저주, 재생형갑옷, 나약함, 불사(이놈은 마지막에 계산되야 될것 같은데)
                case (int)EBuffType.HealingFactor:
                    float LostHP = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP - PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP;
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp((int)(LostHP / 20));
                    break;
                case (int)EBuffType.ReCharge:
                    float LostSTA = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxSTA - PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA;
                    PlayerMgr.GetPlayerInfo().PlayerRegenSTA((int)(LostSTA / 20));
                    break;
                case (int)EBuffType.Burn:
                    float BurnDamage = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP / 20;
                    PlayerMgr.GetPlayerInfo().PlayerDamage((int)BurnDamage);
                    break;
                case (int)EBuffType.Poison:
                    PlayerMgr.GetPlayerInfo().PlayerDamage(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison]);
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison] = PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison] / 2;
                    break;
                case (int)EBuffType.CurseOfDeath:
                    if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath] <= 1)//1보다 작을때 = 지금 0이된다 = 데미지를 입는다.
                    {//여기까지 오면 CurseOfDeath의 스택은 0초과 1이하 즉 1임
                        float CurseOfDeathDamage = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP * 9 / 10;
                        PlayerMgr.GetPlayerInfo().PlayerDamage((int)CurseOfDeathDamage);
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath] = 0;
                    }
                    else
                    {//0초과이기만 할때는 --
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath]--;
                    }
                    break;
                case (int)EBuffType.RegenArmor:
                    PlayerMgr.GetPlayerInfo().PlayerGetShield(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.RegenArmor]);
                    break;
                case (int)EBuffType.Weakness:
                    float WeaknessSpendSTA = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA / 20;
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(WeaknessSpendSTA);
                    break;
                case (int)EBuffType.UnDead:
                    if(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP < 1)
                    {
                        PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP = 1;
                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentHpRatio =
                            PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP / PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP;
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnDead] = 0;
                    }
                    else
                    {//위의 조건이 아니라면 --
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnDead]--;
                    }
                    break;
            }

            if(BuffsType != (int)EBuffType.Poison && BuffsType != (int)EBuffType.CurseOfDeath &&
                BuffsType != (int)EBuffType.UnDead && BuffsType != (int)EBuffType.Defenseless)//독 과 죽음의 저주, 불사, 무방비는 따로 계산
            {
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[BuffsType]--;
            }
        }

        foreach(GameObject Mon in MonMgr.GetActiveMonsters())
        {
            Monster MonInfo = Mon.GetComponent<Monster>();
            if (MonInfo.MonsterBuff.BuffList[BuffsType] > 0)
            {
                switch (BuffsType)//1씩 줄어들지 않거나 데미지를 주거나 회복시키는것만
                {
                    //재생 , 기충전, 화상, 독, 죽음의 저주, 재생형갑옷, 나약함, 불사(이놈은 마지막에 계산되야 될것 같은데)
                    case (int)EBuffType.HealingFactor:
                        float LostHP = MonInfo.GetMonsterCurrentStatus().MonsterMaxHP - MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP;
                        MonInfo.MonsterRegenHP((int)(LostHP / 20));
                        break;
                    case (int)EBuffType.Burn:
                        float BurnDamage = MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP / 20;
                        MonInfo.MonsterDamage((int)BurnDamage);
                        break;
                    case (int)EBuffType.Poison:
                        MonInfo.MonsterDamage(MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison]);
                        MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison] = MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison] / 2;
                        break;
                    case (int)EBuffType.CurseOfDeath:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath] <= 1)
                        {
                            float CurseOfDeathDamage = MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP * 9 / 10;
                            MonInfo.MonsterDamage((int)CurseOfDeathDamage);
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath] = 0;
                        }
                        else
                        {
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath]--;
                        }
                        break;
                    case (int)EBuffType.RegenArmor:
                        MonInfo.MonsterGetShield(MonInfo.MonsterBuff.BuffList[(int)EBuffType.RegenArmor]);
                        break;
                    case (int)EBuffType.UnDead:
                        if(MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP < 1)
                        {
                            MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP = 1;
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.UnDead] = 0;
                        }
                        else
                        {
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.UnDead]--;
                        }
                        break;
                }

                if (BuffsType != (int)EBuffType.Poison && BuffsType != (int)EBuffType.CurseOfDeath &&
                    BuffsType != (int)EBuffType.UnDead)//독 과 죽음의 저주, 불사는 따로 계산
                {
                    MonInfo.MonsterBuff.BuffList[BuffsType]--;
                }
            }
        }
    }
    public void SetBattleTurn()
    {
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
        CurrentState = (int)EBattleStates.Idle;
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
    }

    //CalculatePlayerAction
    protected void SetPlayerBattleStatus(string ActionButtonType)
    {
        PlayerInfo P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        TotalPlayerState TP_Info = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo();
        float AllEquipTier = PlayerMgr.GetPlayerInfo().GetAllEquipTier();

        int EquipCode = 0;
        switch (ActionButtonType)
        {
            case "Attack":
                EquipCode = P_Info.EquipWeaponCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalSTR;
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
        EquipmentSO Equipment_Info = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(EquipCode);


        //기초 추가 수치 // 거의 유일하게 EQUIP 초반 강화 효과에 영향을 받는 수치임
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.WeaponMaster] >= 1)//웨폰마스터 버프를 보유중일때
        {
            //플레이어간 끼고 있는 장비들을 포함한 모든 장비들의 티어의 합
            BattleResultStatus.BaseAmountPlus = AllEquipTier;
        }
        else
        {
            BattleResultStatus.BaseAmountPlus = 0f;
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
            int LuckBuffNum = 0;
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Luck] >= 1)
            {
                LuckBuffNum += 30;
            }
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Misfortune] >= 1)
            {
                LuckBuffNum -= 30;
            }

            //Debug.Log("부정 : 긍정 = 1 : 2 => 부정 : 0 ~ 20"  + " 긍정 : 20 ~ " + (60 + (int)TP_Info.TotalLUK + LuckBuffNum));
            //Luk이 겁나 낮아지는거 예외처리
            int RandNum;
            if (60 + (int)TP_Info.TotalLUK + LuckBuffNum<= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)TP_Info.TotalLUK + LuckBuffNum);
            }

            if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //부정적 List에서 하나를 뽑아서 저장
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)TP_Info.TotalLUK + LuckBuffNum)
            {
                //긍정적 List에서 하나를 뽑아서 저장
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
        }
        //버프에 의한 배율
        BattleResultStatus.BuffMagnification = 1f;
        switch (ActionButtonType)
        {
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
        //최종 추가 수치 // 거의 유일하게 EXPMG 초반 강화 효과에 영향을 받는 수치임
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.EXPPower] >= 1)//경험은 힘 버프 보유시
        {
            BattleResultStatus.FinalResultAmountPlus = (int)(P_Info.Experience / 100f);
        }
        else
        {
            BattleResultStatus.FinalResultAmountPlus = 0f;
        }
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
            case (int)EMonsterActionState.Attack:
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterWeaponCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentATK;
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
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterArmorCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentDUR;
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
        EquipmentSO ESO_Info = EquipmentInfoManager.Instance.GetMonEquipmentInfo(MonEquipmentCode);

        if(BattleResultStatus.BaseAmount > 0)
        {
            //여기 들어왔을때 ResultMagnification 비우기
            BattleResultStatus.ResultMagnification.Clear();
            SetMonsterBattleStatus(MC_Info, ESO_Info);
        }
    }

    protected void SetMonsterBattleStatus(MonsterCurrentStatus MC_Info, EquipmentSO ESO_Info)
    {
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

            int LuckBuffNum = 0;
            if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Luck] >= 1)
            {
                LuckBuffNum += 30;
            }
            if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Misfortune] >= 1)
            {
                LuckBuffNum -= 30;
            }
            Debug.Log("부정 : 긍정 = 1 : 1 => 부정 : 0 ~ 30" + " 긍정 : 30 ~ " + (60 + (int)MC_Info.MonsterCurrentLUK + LuckBuffNum));
            //Luk이 겁나 낮아지는거 예외처리
            int RandNum;
            if (60 + (int)MC_Info.MonsterCurrentLUK + LuckBuffNum <= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)MC_Info.MonsterCurrentLUK +LuckBuffNum);
            }

            if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //부정적 List에서 하나를 뽑아서 저장
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)MC_Info.MonsterCurrentLUK + LuckBuffNum)
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
