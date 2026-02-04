using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stage01EventDetailAction
{
    //-------------------------------------------------Event1000
    public int Event1000(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 아무일 없음
        //1. 50% 체력 +30 50% 체력 -30
        Getting = "";
        Losing = "";
        int RandomHP = Random.Range(-15, 16);
        switch (ButtonType)
        {
            case 0:
                return 1001;
            case 1:
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//독초
                    Losing = "-HP : " + (30 + RandomHP).ToString();
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30 - RandomHP);
                    SoundManager.Instance.PlaySFX("Buff_Consume");
                    return 1002;
                }
                else if(Rand == 1)
                {//약초
                    Getting = "+HP : " + (30 + RandomHP).ToString();
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHP);
                    SoundManager.Instance.PlaySFX("Buff_Healing");
                    return 1003;
                }
                break;
        }
        return 1000;
    }
    //-------------------------------------------------Event1010
    public int Event1010(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 아무일 없음
        //1. 50% 경험치 56/ 50% 전투
        Getting = "";
        Losing = "";
        switch(ButtonType)
        {
            case 0:
                return 1011;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//기습성공
                    int RewardRange = (int)(StageAverageReward / 4);
                    int RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                    Getting = "+EXP : " + RandomReward.ToString();
                    PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                    UIMgr.GI_UI.ActiveGettingUI(0, true);
                    return 1012;
                }
                else if(Rand == 1)
                {//기습 실패
                    return 1013;
                }
                break;
        }
        return 1010;
    }
    public void Event1013(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 1300;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-------------------------------------------------Event1020
    public int Event1020(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 앞으로 2번의 전투동안 행운 3 부여 50% // 전투 50%
        //1. 주술사와 전투
        //2. 아무일 없음
        Getting = "";
        Losing = "";
        switch(ButtonType)
        {
            case 0:
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//훔치기 성공
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                    JsonReadWriteManager.Instance.LkEv_Info.PowwersCeremony = 2;
                    if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                        Getting = "For the next 2 battles, start combat with 3 stacks of Luck.";
                    else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                        Getting = "次の2回の戦闘で、戦闘開始時に運を3スタック獲得。";
                    else
                        Getting = "전투 2회 동안 전투 시작시 행운 3스택 보유";
                    return 1021;
                }
                else
                {//훔치기 실패
                    return 1022;
                }
            case 1:
                return 1023;
            case 2:
                return 1024;
        }
        return 1020;
    }
    public void Event1022_3(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 1100;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-------------------------------------------------Event1030
    public int Event1030(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 힘, 내구, 속도 중 레벨 낮은거 1렙업, 피로도 -300
        //1. 전투
        Getting = "";
        Losing = "";
        int RandSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandSTA);
                int STRLevel = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().StrengthLevel;
                int DURLevel = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DurabilityLevel;
                int SPDLevel = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SpeedLevel;
                int SmallestLevel = Mathf.Min(STRLevel, DURLevel, SPDLevel);
                if(SmallestLevel == STRLevel)
                {
                    if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                        Getting = "+STR Level : 1";
                    else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                        Getting = "+STRレベル : 1";
                    else
                        Getting = "+STR 레벨 : 1";
                    PlayerMgr.GetPlayerInfo().UpgradePlayerSingleStatus("STR", 1);
                }
                else if(SmallestLevel == DURLevel)
                {
                    if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                        Getting = "+DUR Level : 1";
                    else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                        Getting = "+DURレベル : 1";
                    else
                        Getting = "+DUR 레벨 : 1";
                    PlayerMgr.GetPlayerInfo().UpgradePlayerSingleStatus("DUR", 1);
                }
                else if(SmallestLevel == SPDLevel)
                {
                    if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                        Getting = "+SPD Level : 1";
                    else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                        Getting = "+SPDレベル : 1";
                    else
                        Getting = "+SPD 레벨 : 1";
                    PlayerMgr.GetPlayerInfo().UpgradePlayerSingleStatus("SPD", 1);
                }
                PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();
                return 1031;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                return 1032;
        }
        return 1030;
    }
    public void Event1032(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 1301;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-------------------------------------------------Event1040
    public int Event1040(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 피로도 300회복
        //2. 탐색 수치 증가
        Getting = "";
        Losing = "";
        int RandSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                Getting = "+STA : " + (300 + RandSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 1041;
            case 1:
                //1 ~ 20 까지중 랜덤으로
                int RandomIncrease = Random.Range(1, 20);
                if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                    Getting = "+Exploration : " + RandomIncrease.ToString();
                else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                    Getting = "+探索度 : " + RandomIncrease.ToString();
                else
                    Getting = "+탐색도 : " + RandomIncrease.ToString();
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandomIncrease;
                return 1042;
        }
        return 1040;
    }
    //-------------------------------------------------Event1050
    public int Event1050(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 50% 체력 소모30 // 50% 피로도 회복 300
        //1. 피로도 소모 300
        Getting = "";
        Losing = "";
        int RandHP = Random.Range(-15, 16);
        int RandSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//부딪힘
                    Losing = "-HP : " + (30 + RandHP).ToString();
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30 - RandHP);
                    return 1051;
                }
                else
                {//안부딪힘
                    Getting = "+STA : " + (300 + RandSTA).ToString();
                    PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandSTA);
                    SoundManager.Instance.PlaySFX("Buff_Healing");
                    return 1052;
                }
            case 1:
                Losing = "-STA : " + (300 + RandSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandSTA);
                return 1053;
        }
        return 1050;
    }
}
