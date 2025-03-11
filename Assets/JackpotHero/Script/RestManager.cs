using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpGradeAfterStatus
{
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
    private PlaySceneManager PlaySceneMgr;
    

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
    //------------------------------RestFunc
    public void SetRestResult(int RestQuality)//결과를 결정한다. <- 받아야 하는 것은 휴식 횟수, 휴식 퀄리티
    {
        //VeryBad -> 0 ~ 49, Bad -> 0 ~ 24, Good -> 0 ~ 9, VeryGood -> 0 ~ 4, Perfect -> 확률 없음
        MaxRestTime = (int)TimeCountSlider.value;
        CurrentRestTime = 0;
        IsPeacefulRest.Clear();
        int MonAttackRange = 0;

        switch (RestQuality)
        {
            case (int)EPlayerRestQuality.VeryBad:
                MonAttackRange = 50;
                break;
            case (int)EPlayerRestQuality.Bad:
                MonAttackRange = 25;
                break;
            case (int)EPlayerRestQuality.Good:
                MonAttackRange = 10;
                break;
            case (int)EPlayerRestQuality.VeryGood:
                MonAttackRange = 5;
                break;
            case (int)EPlayerRestQuality.Perfect:
                MonAttackRange = 0;
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

    public void StartRestCheck(RestUIScript R_UI)
    {
        StartCoroutine(RestCheckCoroutine(R_UI));
    }

    IEnumerator RestCheckCoroutine(RestUIScript R_UI)
    {
        yield return null;
        while(CurrentRestTime < MaxRestTime)
        {
            if (IsPeacefulRest[CurrentRestTime] == true)//평화롭게 지나간다면
            {
                R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1)) / MaxRestTime, false);
                while(true)
                {
                    yield return null;
                    if(R_UI.FillAmountAnimEnd == true)
                    {
                        break;
                    }
                }
                PlayerMgr.GetPlayerInfo().RecoverHPNSTAByRest(0.1f);
                UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
                CurrentRestTime++;
            }
            else//습격이 일어난다면
            {
                //시계를 턴의 절반정도만 하고 습격시작
                //DurationTime을 추가로 넣을수 있음 절반이 아닌 0.3 ~ 0.8까지 랜덤값을 주도록
                //1 -> 1초, 0.5 -> 0.5초
                float RandAmount = Random.Range(0.2f, 0.8f);
                R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1) + RandAmount) / MaxRestTime, true, RandAmount);
                while(true)
                {
                    yield return null;
                    if(R_UI.FillAmountAnimEnd == true)
                    {
                        break;
                    }
                }
                R_UI.InActiveLeftTimeObject();
                //휴식 UI는 끄고
                PlaySceneMgr.SuddenAttackByMonsterInRest(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails);
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
            R_UI.InActiveLeftTimeObject();
            R_UI.ActiveRestActionSelection();
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        }
        Debug.Log("RestEnd");
    }

    //---------------------------------PlayerUpgradeFunc
    public void InitUpgradeAfterStatus()
    {
        AfterStatus.AfterSTR = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().StrengthLevel;
        AfterStatus.AfterDUR = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DurabilityLevel;
        AfterStatus.AfterRES = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ResilienceLevel;
        AfterStatus.AfterSPD = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SpeedLevel;
        AfterStatus.AfterLUK = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().LuckLevel;
        AfterStatus.AfterLevel = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Level;
        AfterStatus.NeededEXP = 0;
        BeforeLevel = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Level;
    }

    public void PlayerUpgradePlusButtonClick(string UpGradeType)
    {
        AfterStatus.AfterLevel++;
        switch(UpGradeType)
        {
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

    public void PlayerUpgradeMinusButtonClick(string UpGradeType)
    {
        AfterStatus.AfterLevel--;
        switch (UpGradeType)
        {
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
        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < AfterStatus.NeededEXP)
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughEXP_PlayerUpgrade);
            return;
        }
        //여기까지 오면 경험치는 충분한거임
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-AfterStatus.NeededEXP, true);//경험치를 줄이거나 늘리고
        PlayerMgr.GetPlayerInfo().UpgradePlayerStatus(AfterStatus);//스텟을 업그래이드
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//Json에 저장

        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//UI에 있는 스텟 갱신
        UIMgr.R_UI.ActiveRestActionSelection();//휴식 선택창 띄우기
    }
    //---------------------------------------------------EquipGatchaFunc
    public void InActiveEquipGambling()
    {
        UIMgr.R_UI.ActiveRestActionSelection();//휴식 선택창 띄우기
        //이 창이 꺼졌다는건 플레이어의 스탯과 장비가 변했을 가능성이 높음
        PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//장비 변화에 의한 스텟 변화적용
        UIMgr.PE_UI.SetEquipmentImage(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//장비 변화에 의한 장비표시 ui 변화 적용
        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //장비 변화에 의한 스탯표시 ui 변화 적용
    }
}
