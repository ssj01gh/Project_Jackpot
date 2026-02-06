using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage03EventDetailAction
{
    //-----------------------------------------Event3000
    public int Event3000(int ButtonType, ref string Getting, ref string Losing)
    {
        //0.1. 0과 1은 상관없은 50% 독X 50% 독 10
        //2. 독 5
        Getting = "";
        Losing = "";
        int RandNum = Random.Range(0, 2);//0~1
        switch(ButtonType)
        {
            case 0:
            case 1:
                if(RandNum == 0)
                {//해독 3001
                    SoundManager.Instance.PlaySFX("Buff_Healing");
                    JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin = 0;
                    JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin_DisInject = 0;
                    return 3001;
                }
                else
                {//독 3002
                    if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                        Losing = "For the next 5 battles, start combat with 10 stacks of Poison";
                    else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                        Losing = "次の5回の戦闘で、戦闘開始時に毒を10スタック獲得";
                    else
                        Losing = "전투 5회 동안 전투 시작시 독 10스택 보유";
                    SoundManager.Instance.PlaySFX("Buff_Consume");
                    JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin = 5;
                    return 3002;
                }
            case 2://3003
                if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                    Losing = "For the next 5 battles, start combat with 5 stacks of Poison";
                else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                    Losing = "次の5回の戦闘で、戦闘開始時に毒を5スタック獲得";
                else
                    Losing = "전투 5회 동안 전투 시작시 독 5스택 보유";
                JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin_DisInject = 5;
                return 3003;
        }
        return 3000;
    }
    //-----------------------------------------Event3010
    public int Event3010(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0.JsonEvent 등록 보안체계 // 3011
        //1. 피로 회복 + 소 // 3012
        //2.JsonEvent 등록 이상한 구체 // 3013
        //3. 이탈                         // 3014
        Getting = "";
        Losing = "";
        int RandomSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                JsonReadWriteManager.Instance.LkEv_Info.Lab_Security = true;
                return 3011;
            case 1:
                Getting = "+STA : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 3012;
            case 2:
                JsonReadWriteManager.Instance.LkEv_Info.Lab_Sphere = true;
                return 3013;
            case 3:
                return 3014;
        }
        return 3010;
    }
    //-----------------------------------------Event3020
    public int Event3020()
    {
        return 3021;
    }
    public void Event3021(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 3300;//이런것도 바끼어야 할려나
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-----------------------------------------Event3030
    public int Event3030(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 전투 개시 gk + 2 // 3031
        //1. 이탈                 // 3032
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 3;
                return 3031;
            case 1:
                return 3032;
        }
        return 3030;
    }

    public void Event3031(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 3150;//이런것도 바끼어야 할려나
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-----------------------------------------Event3040
    public int Event3040(int ButtonType)
    {
        //0. 전투 개시 3041
        //1. 이탈         3042
        switch(ButtonType)
        {
            case 0:
                return 3041;
            case 1:
                return 3042;
        }
        return 3040;
    }
    public void Event3041(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
    {
        int CurrentEventMonsterSpawnPatternCode = 3301;//이런것도 바끼어야 할려나
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEventMonsterSpawnPatternCode;
        PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-----------------------------------------Event3050
    public int Event3050(int ButtonType, ref string Getting, ref string Losing)
    {
        //0. 이탈                                                 3051
        //1. 마주한다. -> 보스 전투 시작시 도핑 부여   3052
        Getting = "";
        Losing = "";
        switch (ButtonType)
        {
            case 0:
                return 3051;
            case 1:
                if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                    Losing = "When a battle with an Administrator or a Guardian begins\nmonsters start with 99 stacks of Doping";
                else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                    Losing = "管理者または守護者との戦闘開始時\nモンスターはドーピングを99スタック所持。";
                else
                    Losing = "관리자 혹은 수호자와의 전투 시작시\n몬스터가 도핑 99스택 보유";
                JsonReadWriteManager.Instance.LkEv_Info.ReadyForBattle = true;
                return 3052;
        }
        return 3050;
    }
    //-----------------------------------------Event3060
    public int Event3060(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //1.이탈                  //조건에 따라 3061 Or 3063
        //2. 경험치 상 + bk + 3// 3062
        Getting = "";
        Losing = "";
        JsonReadWriteManager.Instance.LkEv_Info.IsMeetTalkingDopple = true;
        int RandomReward = Random.Range(-(StageAverageReward * 3 / 4), (StageAverageReward * 3 / 4) + 1);
        switch(ButtonType)
        {
            case 0:
                if (JsonReadWriteManager.Instance.LkEv_Info.TalkingMonster == true &&
                    JsonReadWriteManager.Instance.LkEv_Info.TalkingDirtGolem == true)
                {//토토의 이름을 알때
                    return 3063;
                }
                else//토토의 이름을 모를때
                    return 3061;
            case 1:
                Getting = "+EXP : " + ((StageAverageReward * 3) + RandomReward).ToString();
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount((StageAverageReward * 3) + RandomReward);
                UIMgr.GI_UI.ActiveGettingUI(0, true);
                return 3062;
        }
        return 3060;
    }
}
