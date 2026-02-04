using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage02EventDetailAction
{
    //-----------------------------------------Event2000
    public int Event2000(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 피로도 +300 +-150 -> 이벤트 2005로
        //1. 피로도 -300 +-150 -> 이벤트 2001로
        Getting = "";
        Losing = "";
        int RandomSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                Getting = "+STA : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 2005;
            case 1:
                if(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 300)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NoEnoughEnergy_FoggedForest);
                    return 2000;
                }
                Losing = "-STA : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                return 2001;
        }    

        return 2000;
    }
    public int Event2001(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 피로도 +300 +-150 -> 이벤트 2005로
        //1. 피로도 -300 +-150 -> 50% 이벤트 2006로 50% 2002로
        Getting = "";
        Losing = "";
        int RandomSTA = Random.Range(-150, 151);
        int RandomPath = Random.Range(0, 2);//0~1
        switch (ButtonType)
        {
            case 0:
                Getting = "+STA : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 2005;
            case 1:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 300)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NoEnoughEnergy_FoggedForest);
                    return 2001;
                }
                Losing = "-STA : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                if (RandomPath == 0)//0걸리면 2002
                {
                    return 2002;
                }
                else//아니면 2006
                {
                    return 2006;
                }
        }
        return 2001;
    }
    public int Event2002(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 피로도 +300 +-150 -> 이벤트 2005로
        //1. 피로도 -300 +-150 -> 75% 이벤트 2006로 25% 2003로
        Getting = "";
        Losing = "";
        int RandomSTA = Random.Range(-150, 151);
        int RandomPath = Random.Range(0, 4);//0~3
        switch (ButtonType)
        {
            case 0:
                Getting = "+STA : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 2005;
            case 1:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 300)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NoEnoughEnergy_FoggedForest);
                    return 2002;
                }
                Losing = "-STA : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                if (RandomPath == 0)
                {//이거 걸리면 2003
                    return 2003;
                }
                else
                {//이거 걸리면 2006
                    return 2006;
                }
        }
        return 2002;
    }
    public int Event2003(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 피로도 +300 +-150 -> 이벤트 2005로
        //1. 피로도 -300 +-150 -> 90% 이벤트 2006로 10% 2004로
        Getting = "";
        Losing = "";
        int RandomSTA = Random.Range(-150, 151);
        int RandomPath = Random.Range(0, 10);//0 ~ 9
        switch (ButtonType)
        {
            case 0:
                Getting = "+STA : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 2005;
            case 1:
                if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA < 300)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NoEnoughEnergy_FoggedForest);
                    return 2003;
                }
                Losing = "-STA : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                if (RandomPath == 0)
                {//이거 걸리면 2004
                    return 2004;
                }
                else
                {//이거 걸리면 2006
                    return 2006;
                }
        }
        return 2002;
    }
    public int Event2004(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 장비 획득 -> 이벤트 2007로
        Getting = "";
        Losing = "";
        int ForestBukelt = 26044;
        switch (ButtonType)
        {
            case 0:
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NoEnoughEnergy_FoggedForest);
                    return 2004;
                }
                if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                    Getting = "+Equipment : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(ForestBukelt).EquipmentName;
                else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                    Getting = "+装備 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(ForestBukelt).EquipmentName;
                else
                    Getting = "+장비 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(ForestBukelt).EquipmentName;
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(ForestBukelt);
                UIMgr.NonInven_UI.UpdateNonRestInventoryWhenOpen();
                UIMgr.GI_UI.ActiveGettingUI(ForestBukelt);
                JsonReadWriteManager.Instance.LkEv_Info.ForestBracelet = true;
                return 2007;
        }
        return 2002;
    }
    public int Event2006(int ButtonType)
    {
        //0.다시 2000으로
        switch (ButtonType)
        {
            case 0:
                return 2000;
        }
        return 2006;
    }
    //-----------------------------------------Event2010
    public int Event2010(int ButtonType, ref string Getting, ref string Losing)
    {
        //0.다시 2000으로
        Getting = "";
        Losing = "";
        int RandNum = Random.Range(0, 2);//0~1
        switch (ButtonType)
        {
            //0. 50% 독 2012 50% 재생 2011
            //1. 그냥 탐색 2013
            case 0:
                if(RandNum == 0)//재생
                {
                    if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                        Getting = "For the next 2 battles, start combat with 3 stacks of Regeneration.";
                    else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                        Getting = "次の2回の戦闘で、戦闘開始時に再生を3スタック獲得。";
                    else
                        Getting = "전투 2회 동안 전투 시작시 재생 3스택 보유";
                    JsonReadWriteManager.Instance.LkEv_Info.ForestHut_Regen = 2;
                    SoundManager.Instance.PlaySFX("Buff_Healing");
                    return 2011;
                }
                else//독
                {
                    if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                        Losing = "For the next 2 battles, start combat with 5 stacks of Poison";
                    else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                        Losing = "次の2回の戦闘で、戦闘開始時に毒を5スタック獲得";
                    else
                        Losing = "전투 2회 동안 전투 시작시 독 5스택 보유";

                    JsonReadWriteManager.Instance.LkEv_Info.ForestHut_Poison = 2;
                    SoundManager.Instance.PlaySFX("Buff_Consume");
                    return 2012;
                }
            case 1:
                return 2013;
        }
        return 2010;
    }
    //-----------------------------------------Event2020
    public int Event2020(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0.75% 경험치 +하 +- 하 / 4 2021 25% 전투 2025
        //1.그냥 탐색 2024
        Getting = "";
        Losing = "";
        int RandNum = Random.Range(0, 4);//0~4
        int RandomRewardRange = 0;
        int RandomReward = 0;
        switch(ButtonType)
        {
            case 0:
                if(RandNum == 0)//전투 시작
                {
                    return 2025;
                }
                else//경험치를 얻음
                {
                    RandomRewardRange = StageAverageReward / 4;
                    RandomReward = StageAverageReward + Random.Range(-RandomRewardRange, RandomRewardRange + 1);
                    Getting = "+EXP : " + RandomReward.ToString();
                    PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                    UIMgr.GI_UI.ActiveGettingUI(0, true);
                    return 2021;
                }
            case 1:
                return 2024;
        }
        return 2020;
    }
    public int Event2021(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0.50% 경험치 +하 +- 하 / 4 2022 50% 전투 2025
        //1.그냥 탐색 2023
        Getting = "";
        Losing = "";
        int RandNum = Random.Range(0, 2);//0~1
        int RandomRewardRange = 0;
        int RandomReward = 0;
        switch (ButtonType)
        {
            case 0:
                if (RandNum == 0)//전투 시작
                {
                    return 2025;
                }
                else//경험치를 얻음
                {
                    RandomRewardRange = StageAverageReward / 4;
                    RandomReward = StageAverageReward + Random.Range(-RandomRewardRange, RandomRewardRange + 1);
                    Getting = "+EXP : " + RandomReward.ToString();
                    PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                    UIMgr.GI_UI.ActiveGettingUI(0, true);
                    return 2022;
                }
            case 1:
                return 2023;
        }
        return 2021;
    }
    public int Event2022(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0.25% 경험치 +하 +- 하 / 4 2022 75% 전투 2025
        //1.그냥 탐색 2023
        Getting = "";
        Losing = "";
        int RandNum = Random.Range(0, 4);//0~3
        int RandomRewardRange = 0;
        int RandomReward = 0;
        switch (ButtonType)
        {
            case 0:
                if (RandNum == 0)//경험치 획득
                {
                    RandomRewardRange = StageAverageReward / 4;
                    RandomReward = StageAverageReward + Random.Range(-RandomRewardRange, RandomRewardRange + 1);
                    Getting = "+EXP : " + RandomReward.ToString();
                    PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                    UIMgr.GI_UI.ActiveGettingUI(0, true);
                    return 2022;
                }
                else//전투 시작
                {
                    return 2025;
                }
            case 1:
                return 2023;
        }
        return 2022;
    }
    public void Event2025(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 2150;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-----------------------------------------Event2030
    public int Event2030(PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //버튼 뭘 누르던 상관 없이 33%확률로 전투 2032 // 회복 = 2031
        Getting = "";
        Losing = "";
        int RandomNum = Random.Range(0, 3);//0~2
        int RandomHPAverage = Random.Range(-15, 16);
        int RandomSTAAverage = Random.Range(-150, 151);
        if (RandomNum == 0)
        {//전투
            return 2032;
        }
        else
        {//회복
            SoundManager.Instance.PlaySFX("Buff_Healing");
            Getting = "+HP : " + (30 + RandomHPAverage).ToString() + "\n" +
                "+STA : " + (300 + RandomSTAAverage).ToString();
            PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHPAverage);
            PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTAAverage);
            return 2031;
        }
    }
    public void Event2032(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 2300;//이런것도 바끼어야 할려나
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-----------------------------------------Event2040
    public int Event2040(PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //누를수 있는 버튼은 1개밖에 없음, 플레이어의 Gk와 BK 수치에 따라 이벤트 연계가 달라짐
        Getting = "";
        Losing = "";
        int PlayerGKP = (int)PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma;
        int PlayerBKP = (int)PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma;
        JsonReadWriteManager.Instance.LkEv_Info.ML_GKPerson = false;
        JsonReadWriteManager.Instance.LkEv_Info.ML_NorPerson = false;
        JsonReadWriteManager.Instance.LkEv_Info.ML_BKPerson = false;
        if (PlayerGKP - PlayerBKP >= 3)
        {//선인//2041
            if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                Getting = "Permanently increases\nSTR, DUR, RES, SPD, LUK by 1";
            else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                Getting = "STR、DUR、RES、SPD、LUKが\n恒久的に1上昇";
            else
                Getting = "영구적으로 STR, DUR, RES, SPD, LUK의\n기초 능력치 1증가";
            JsonReadWriteManager.Instance.LkEv_Info.ML_GKPerson = true;
            PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();
            return 2041;
        }
        else if(PlayerBKP - PlayerGKP >= 3)
        {//악인//2043
            JsonReadWriteManager.Instance.LkEv_Info.ML_BKPerson = true;
            return 2043;
        }
        else
        {//평범함//2042
            JsonReadWriteManager.Instance.LkEv_Info.ML_NorPerson = true;
            return 2042;
        }
    }
    public void Event2043(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 2301;//이런것도 바끼어야 할려나
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-----------------------------------------Event2050
    public int Event2050(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0.거인과 전투 gk + 3 // 2051 // 조건 만족시 2054
        //1.그냥 이탈               // 2052
        //2.경험치 상 bk + 3    //2053
        Getting = "";
        Losing = "";
        int RandomRewardRange = 0;
        int RandomReward = 0;
        JsonReadWriteManager.Instance.LkEv_Info.IsMeetTalkingGiant = true;
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 3;
                if(JsonReadWriteManager.Instance.LkEv_Info.TalkingMonster == true &&
                    JsonReadWriteManager.Instance.LkEv_Info.TalkingDirtGolem == true)
                {
                    return 2054;
                }
                else
                    return 2051;
            case 1:
                return 2052;
            case 2:
                RandomRewardRange = StageAverageReward * 3 / 4;
                RandomReward = (StageAverageReward * 3) + Random.Range(-RandomRewardRange, RandomRewardRange + 1);
                Getting = "+EXP : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                UIMgr.GI_UI.ActiveGettingUI(0, true);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                return 2053;
        }
        return 2050;
    }

    public void Event2051(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 2302;//이런것도 바끼어야 할려나
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
}
