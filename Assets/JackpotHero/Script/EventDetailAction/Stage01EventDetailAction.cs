using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stage01EventDetailAction
{
    protected int Stage01AverageReward = 28;
    //-------------------------------------------------Event1000
    public int Event1000(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 아무일 없음
        //1. 50% 체력 +30 50% 체력 -30
        int RandomHP = Random.Range(-15, 16);
        switch (ButtonType)
        {
            case 0:
                return 1001;
            case 1:
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//독초
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30 - RandomHP);
                    SoundManager.Instance.PlaySFX("Buff_Consume");
                    return 1002;
                }
                else if(Rand == 1)
                {//약초
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHP);
                    SoundManager.Instance.PlaySFX("Buff_Healing");
                    return 1003;
                }
                break;
        }
        return 1000;
    }
    //-------------------------------------------------Event1010
    public int Event1010(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. 아무일 없음
        //1. 50% 경험치 56/ 50% 전투
        switch(ButtonType)
        {
            case 0:
                return 1011;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//기습성공
                    int RewardRange = (int)(Stage01AverageReward / 4);
                    PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(Stage01AverageReward + Random.Range(-RewardRange, RewardRange +1));

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
    public int Event1020(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 앞으로 2번의 전투동안 행운 3 부여 50% // 전투 50%
        //1. 주술사와 전투
        //2. 아무일 없음
        switch(ButtonType)
        {
            case 0:
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//훔치기 성공
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                    JsonReadWriteManager.Instance.LkEv_Info.PowwersCeremony = 2;
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
    public int Event1030(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 힘, 내구, 속도 중 레벨 낮은거 1렙업, 피로도 -300
        //1. 전투
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
                    PlayerMgr.GetPlayerInfo().UpgradePlayerSingleStatus("STR", 1);
                }
                else if(SmallestLevel == DURLevel)
                {
                    PlayerMgr.GetPlayerInfo().UpgradePlayerSingleStatus("DUR", 1);
                }
                else if(SmallestLevel == SPDLevel)
                {
                    PlayerMgr.GetPlayerInfo().UpgradePlayerSingleStatus("SPD", 1);
                }
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
    public int Event1040(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 피로도 300회복
        //2. 탐색 수치 증가
        int RandSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 1041;
            case 1:
                //1 ~ 7 까지중 랜덤으로
                int RandomIncrease = Random.Range(1, 8);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandomIncrease;
                return 1042;
        }
        return 1040;
    }
    //-------------------------------------------------Event1050
    public int Event1050(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 50% 체력 소모30 // 50% 피로도 회복 300
        //1. 피로도 소모 300
        int RandHP = Random.Range(-15, 16);
        int RandSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//부딪힘
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30 - RandHP);
                    return 1051;
                }
                else
                {//안부딪힘
                    PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandSTA);
                    SoundManager.Instance.PlaySFX("Buff_Healing");
                    return 1052;
                }
            case 1:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandSTA);
                return 1053;
        }
        return 1050;
    }
}
