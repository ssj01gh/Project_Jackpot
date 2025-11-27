using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage03EventDetailAction
{
    //-----------------------------------------Event3000
    public int Event3000(int ButtonType)
    {
        //0.1. 0과 1은 상관없은 50% 독X 50% 독 10
        //2. 독 5
        int RandNum = Random.Range(0, 2);//0~1
        switch(ButtonType)
        {
            case 0:
            case 1:
                if(RandNum == 0)
                {//해독 3001
                    SoundManager.Instance.PlaySFX("Buff_Healing");
                    JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin = 0;
                    return 3001;
                }
                else
                {//독 3002
                    SoundManager.Instance.PlaySFX("Buff_Consume");
                    JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin = 10;
                    return 3002;
                }
            case 2://3003
                JsonReadWriteManager.Instance.LkEv_Info.LetTheGameBegin = 5;
                return 3003;
        }
        return 3000;
    }
    //-----------------------------------------Event3010
    public int Event3010(int ButtonType, PlayerManager PlayerMgr)
    {
        //0.JsonEvent 등록 보안체계 // 3011
        //1. 피로 회복 + 소 // 3012
        //2.JsonEvent 등록 이상한 구체 // 3013
        //3. 이탈                         // 3014
        int RandomSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                JsonReadWriteManager.Instance.LkEv_Info.Lab_Security = true;
                return 3011;
            case 1:
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
    public int Event3030(int ButtonType)
    {
        //0. 전투 개시 gk + 2
        //1. 이탈
        return 3030;
    }
    //-----------------------------------------Event3040
    //-----------------------------------------Event3050
    //-----------------------------------------Event3060
}
