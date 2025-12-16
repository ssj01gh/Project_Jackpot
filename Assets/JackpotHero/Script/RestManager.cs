using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpGradeAfterStatus
{
    public int AfterHP = 0;
    public int AfterSTA = 0;
    public int AfterSTR = 0;
    public int AfterDUR = 0;
    public int AfterRES = 0;
    public int AfterSPD = 0;
    public int AfterLUK = 0;
    public int AfterLevel = 0;
    public int NeededEXP = 0;
}

public class RestManager : MonoBehaviour
{
    [SerializeField]
    private PlayerManager PlayerMgr;
    [SerializeField]
    private PlaySceneUIManager UIMgr;
    [SerializeField]
    private BattleManager BattleMgr;
    [SerializeField]
    private TutorialManager TutorialMgr;
    

    public Slider TimeCountSlider;
    // Start is called before the first frame update
    public List<bool> IsPeacefulRest { protected set; get; } = new List<bool>();
    protected int MaxRestTime = 0;
    protected int CurrentRestTime = 0;

    public UpGradeAfterStatus AfterStatus { protected set; get; } = new UpGradeAfterStatus();


    protected int BeforeLevel;
    /*
    protected int[] LevelEXP = new int[100] 
    { }
    */
    protected const int LevelUpgradeBasePoint = 10;
    protected const float LevelUpIncreaseRatio = 1.2f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActiveRestActionSelection()
    {
        UIMgr.R_UI.ActiveRestActionSelection();
    }
    public void PlayButtonSoundInRestUI()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
    }

    //------------------------------Rest

    public void PressRestActionRest()//휴식 선택창에서 휴식을 누르면 휴식 시간 선택창 활성화
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.R_UI.ActiveRestTimeSelectionUI(PlayerMgr.GetPlayerInfo());
        if(JsonReadWriteManager.Instance.T_Info.CampingRest == false)
        {
            JsonReadWriteManager.Instance.T_Info.CampingRest = true;
            TutorialMgr.SetLinkedTutorialNStartTutorial("Tutorial/CampingRest");
        }
    }
    public void SetRestMgrRestResult()//휴식할 시간을 조절하고
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        SetRestResult();
        UIMgr.R_UI.ActiveLeftTimeObject(IsPeacefulRest, this);
    }
    protected void SetRestResult()//결과를 결정한다. <- 받아야 하는 것은 휴식 횟수, 휴식 퀄리티
    {
        //VeryBad -> 0 ~ 49, Bad -> 0 ~ 24, Good -> 0 ~ 9, VeryGood -> 0 ~ 4, Perfect -> 확률 없음
        int RestQuality = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails;
        MaxRestTime = (int)TimeCountSlider.value;
        CurrentRestTime = 0;
        IsPeacefulRest.Clear();
        int MonAttackRange = 5;

        switch (RestQuality)
        {
            case (int)EPlayerRestQuality.VeryBad:
                MonAttackRange = 55;
                break;
            case (int)EPlayerRestQuality.Bad:
                MonAttackRange = 35;
                break;
            case (int)EPlayerRestQuality.Good:
                MonAttackRange = 20;
                break;
            case (int)EPlayerRestQuality.VeryGood:
                MonAttackRange = 10;
                break;
            case (int)EPlayerRestQuality.Perfect:
                MonAttackRange = 5;
                break;
        }

        for (int i = 0; i < MaxRestTime; i++)
        {
            int RandNum = Random.Range(0, 100);
            if (RandNum < MonAttackRange)// 50이상 나오면 페스 //25이상 나오면 페스
            {//여기 걸리면 몬스터 조우임
                IsPeacefulRest.Add(false);
            }
            else
            {
                IsPeacefulRest.Add(true);
            }
        }
    }

    public void StartRestCheck()
    {
        StartCoroutine(RestCheckCoroutine());
    }

    IEnumerator RestCheckCoroutine()
    {
        yield return null;
        while(CurrentRestTime < MaxRestTime)
        {
            if (IsPeacefulRest[CurrentRestTime] == true)//평화롭게 지나간다면
            {
                UIMgr.R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1)) / MaxRestTime, false);
                while(true)
                {
                    yield return null;
                    if(UIMgr.R_UI.FillAmountAnimEnd == true)//1턴이 지난간거임 여기서 째깍?//회복하는 이펙트도 넣을까?
                    {
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_RegenHP", PlayerMgr.GetPlayerInfo().gameObject.transform.position + new Vector3(0, 0.5f));
                        SoundManager.Instance.PlaySFX("Rest_TickTack");
                        break;
                    }
                }
                PlayerMgr.GetPlayerInfo().RecoverHPNSTAByRest(0.1f);
                UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);
                CurrentRestTime++;
            }
            else//습격이 일어난다면
            {
                //시계를 턴의 절반정도만 하고 습격시작
                //DurationTime을 추가로 넣을수 있음 절반이 아닌 0.3 ~ 0.8까지 랜덤값을 주도록
                //1 -> 1초, 0.5 -> 0.5초
                float RandAmount = Random.Range(0.2f, 0.8f);
                UIMgr.R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1) + RandAmount) / MaxRestTime, true, RandAmount);
                while(true)
                {
                    yield return null;
                    if(UIMgr.R_UI.FillAmountAnimEnd == true)
                    {
                        break;
                    }
                }
                UIMgr.R_UI.InActiveLeftTimeObject();
                //휴식 UI는 끄고
                PlayerMgr.GetPlayerInfo().SetIsSuddenAttackAndRestQuality();
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = 0;

                PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
                BattleMgr.InitCurrentBattleMonsters();
                BattleMgr.InitMonsterNPlayerActiveGuage();
                BattleMgr.ProgressBattle();
                //PlaySceneMgr.SuddenAttackByMonsterInRest(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails);
                //바로 전투 들어가면 될듯? 굳이 시간은 안줄이고 어짜피 다시 휴식 선택으로 돌아갈껀데
                break;
            }
            yield return null;
        }
        //중간에 습격이 안끝나고 휴식이 끝났을때
        if(CurrentRestTime >= MaxRestTime)
        {
            //회복이 끝나면 다시 휴식 행동 선택창이 나와야함
            //Player의 정보도 Json에 갱신하고
            Debug.Log("FullRest");
            UIMgr.R_UI.InActiveLeftTimeObject();
            UIMgr.R_UI.ActiveRestActionSelection();
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        }
        Debug.Log("RestEnd");
    }

    //---------------------------------PlayerUpgradeFunc
    public void PressPlayerUpgradeButton()
    {
        InitUpgradeAfterStatus();
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.R_UI.ActivePlayerUpGradeUI(PlayerMgr.GetPlayerInfo());
        if (JsonReadWriteManager.Instance.T_Info.CampingLevelUp == false)
        {
            JsonReadWriteManager.Instance.T_Info.CampingLevelUp = true;
            TutorialMgr.SetLinkedTutorialNStartTutorial("Tutorial/CampingLevelUp");
        }
    }
    protected void InitUpgradeAfterStatus()
    {
        AfterStatus.AfterHP = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().HPLevel;
        AfterStatus.AfterSTA = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().STALevel;
        AfterStatus.AfterSTR = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().StrengthLevel;
        AfterStatus.AfterDUR = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DurabilityLevel;
        AfterStatus.AfterRES = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ResilienceLevel;
        AfterStatus.AfterSPD = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SpeedLevel;
        AfterStatus.AfterLUK = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().LuckLevel;
        AfterStatus.AfterLevel = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Level;
        AfterStatus.NeededEXP = 0;
        BeforeLevel = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Level;
    }

    public void PressPlayerUpGradePlusButton(string ButtonType)
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerUpgradePlusButtonClick(ButtonType);
        UIMgr.R_UI.PlayerUpgradePLUSMINUSButtonClick(PlayerMgr.GetPlayerInfo(), AfterStatus);
    }
    public void PressPlayerUpGradeMinusButton(string ButtonType)
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerUpgradeMinusButtonClick(ButtonType);
        UIMgr.R_UI.PlayerUpgradePLUSMINUSButtonClick(PlayerMgr.GetPlayerInfo(), AfterStatus);
    }

    protected void PlayerUpgradePlusButtonClick(string UpGradeType)
    {
        AfterStatus.AfterLevel++;
        switch(UpGradeType)
        {
            case "HP":
                AfterStatus.AfterHP++;
                break;
            case "STA":
                AfterStatus.AfterSTA++;
                break;
            case "STR":
                AfterStatus.AfterSTR++;
                break;
            case "DUR":
                AfterStatus.AfterDUR++;
                break;
            case "RES":
                AfterStatus.AfterRES++;
                break;
            case "SPD":
                AfterStatus.AfterSPD++;
                break;
            case "LUK":
                AfterStatus.AfterLUK++;
                break;
        }
        CalculateNeededEXP(true);
    }

    protected void PlayerUpgradeMinusButtonClick(string UpGradeType)
    {
        AfterStatus.AfterLevel--;
        switch (UpGradeType)
        {
            case "HP":
                AfterStatus.AfterHP--;
                break;
            case "STA":
                AfterStatus.AfterSTA--;
                break;
            case "STR":
                AfterStatus.AfterSTR--;
                break;
            case "DUR":
                AfterStatus.AfterDUR--;
                break;
            case "RES":
                AfterStatus.AfterRES--;
                break;
            case "SPD":
                AfterStatus.AfterSPD--;
                break;
            case "LUK":
                AfterStatus.AfterLUK--;
                break;
        }
        CalculateNeededEXP(false);
    }

    protected void CalculateNeededEXP(bool IsPlus)
    {
        //AfterStatus.NeededEXP += Mathf.CeilToInt(LevelUpgradeBasePoint * Mathf.Pow(LevelUpIncreaseRatio, i));
        if(IsPlus == true)
        {
            AfterStatus.NeededEXP += Mathf.CeilToInt(LevelUpgradeBasePoint * Mathf.Pow(LevelUpIncreaseRatio, AfterStatus.AfterLevel - 1));
        }
        else if(IsPlus == false)
        {
            AfterStatus.NeededEXP -= Mathf.CeilToInt(LevelUpgradeBasePoint * Mathf.Pow(LevelUpIncreaseRatio, AfterStatus.AfterLevel));
        }
    }
    public void PlayerUpgradeOKButton()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < AfterStatus.NeededEXP)
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughEXP_PlayerUpgrade);
            return;
        }
        //여기까지 오면 경험치는 충분한거임
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-AfterStatus.NeededEXP, true);//경험치를 줄이거나 늘리고
        PlayerMgr.GetPlayerInfo().UpgradePlayerStatus(AfterStatus);//스텟을 업그래이드
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//Json에 저장

        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList, true);//UI에 있는 스텟 갱신
        UIMgr.R_UI.ActiveRestActionSelection();//휴식 선택창 띄우기
    }
    //---------------------------------------------------EquipGatchaFunc
    public void ActiveEquipGambling()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.EDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.MEDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.R_UI.ActivePlayerEquipMg();
        if (JsonReadWriteManager.Instance.T_Info.CampingEquipment == false)
        {
            JsonReadWriteManager.Instance.T_Info.CampingEquipment = true;
            TutorialMgr.SetLinkedTutorialNStartTutorial("Tutorial/CampingEquip");
        }
    }
    public void InActiveEquipGambling()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.R_UI.ActiveRestActionSelection();//휴식 선택창 띄우기
        //이 창이 꺼졌다는건 플레이어의 스탯과 장비가 변했을 가능성이 높음
        PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//장비 변화에 의한 스텟 변화적용
        UIMgr.PE_UI.SetEquipmentImage(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//장비 변화에 의한 장비표시 ui 변화 적용
        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);
        //장비 변화에 의한 스탯표시 ui 변화 적용
    }
    //-----------------------------------------------------RestEndButton
    public void PressRestEndButton()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int) EPlayerCurrentState.SelectAction;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = 0;

        UIMgr.PressRestEnd();
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //StartCoroutine(CheckBackGroundMoveEnd(true));
    }
}
