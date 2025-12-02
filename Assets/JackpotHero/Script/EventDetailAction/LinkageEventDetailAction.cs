using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class LinkageEventDetailAction
{
    //---------------------------------Event8000
    public int Event8000(int ButtonType, int StageAverageReward,PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 평균보상 * 3, bk + 1
        //1. bk - 1
        //2. -평균보상 bk - 2
        //3. 저주가 옅어진 검//플레이어가 장착하고 있거나 인벤토리에 잇는 검을 바꿔야함
        Getting = "";
        Losing = "";
        int RewardRange = 0;
        int RandomReward = 0;
        switch (ButtonType)
        {
            case 0:
                RewardRange = (int)(StageAverageReward * 3 / 4);
                RandomReward = (int)(StageAverageReward * 3) + Random.Range(-RewardRange, RewardRange + 1);
                Getting = "경험치 획득 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                UIMgr.GI_UI.ActiveGettingUI(0, true);

                return 8001;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma -= 1;

                return 8002;
            case 2:
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                Losing = "경험치 소모 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma -= 2;

                return 8002;
            case 3:
                int SmallCursedSword = 24001;
                int CursedSword = 23000;
                bool IsHaveCursedSword = false;
                if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == CursedSword)
                {
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = SmallCursedSword;
                    IsHaveCursedSword = true;
                }
                else
                {
                    for(int i = 0; i < PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory.Length; i++)
                    {
                        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] == CursedSword)
                        {
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = SmallCursedSword;
                            IsHaveCursedSword = true;
                            break;
                        }
                    }
                }
                if(IsHaveCursedSword == false)
                {
                    return 8004;
                }
                Getting = "장비 획득 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(SmallCursedSword).EquipmentName;
                Losing = "장비 소모 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CursedSword).EquipmentName;
                UIMgr.GI_UI.ActiveGettingUI(SmallCursedSword);
                JsonReadWriteManager.Instance.LkEv_Info.CleanOminousSword = true;
                return 8003;
        }
        return 8000;
    }
    //-------------------------------------------------Event8010
    public int Event8010(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 체력15, 피로도150 회복
        //1. gk+1
        //2. 피로도 -300 gk + 2
        Getting = "";
        Losing = "";
        int RandomSTA = Random.Range(-150, 151);
        int RandomHP = Random.Range(-15, 16);
        switch(ButtonType)
        {
            case 0:
                int RandomNum = Random.Range(0, 2);
                if(RandomNum == 0)
                {
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHP);
                    Getting = "체력 회복 : " + (30 + RandomHP).ToString();
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                    Getting = "피로도 회복 : " + (300 + RandomSTA).ToString();
                }
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8011;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                return 8012;
            case 2:
                Losing = "피로도 소모 : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 2;
                JsonReadWriteManager.Instance.LkEv_Info.TalkingDirtGolem = true;
                return 8013;
        }
        return 8010;
    }
    //----------------------------------------------------Event8020
    public int Event8020(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 스테이지 보상 * 2
        Getting = "";
        Losing = "";
        switch(ButtonType)
        {
            case 0:
                int RewardRange = (int)(StageAverageReward * 2 / 4);
                int RandomReward = (int)(StageAverageReward * 2) + Random.Range(-RewardRange, RewardRange + 1);
                Getting = "경험치 획득 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                UIMgr.GI_UI.ActiveGettingUI(0, true);
                JsonReadWriteManager.Instance.LkEv_Info.RestInPeace = false;

                return 8021;
        }
        return 8020;
    }
    //---------------------------------------------------Event8030
    public int Event8030(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0.신성이 깃던검 25002
        Getting = "";
        Losing = "";
        int HolySword = 25002;
        if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
            return 8030;
        }
        switch (ButtonType)
        {
            case 0:
                Getting = "장비 획득 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(HolySword).EquipmentName;
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(HolySword);
                UIMgr.GI_UI.ActiveGettingUI(HolySword);
                JsonReadWriteManager.Instance.LkEv_Info.TotoRepayFavor = true;
                return 8031;
        }
        return 8030;
    }
    //-------------------------------------------------Event8040
    public int Event8040(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. -저주 받은검, 혹은 - 저주가 옅어진 검
        //1. 아무일 X
        Getting = "";
        Losing = "";
        int SmallCursedSword = 24001;
        int CursedSword = 23000;
        int WeaponCode = 0;
        switch (ButtonType)
        {
            case 0:
                if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == CursedSword ||
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == SmallCursedSword)
                {
                    WeaponCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
                    Losing = "장비 소모 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(WeaponCode).EquipmentName;
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = 0;
                }
                else
                {
                    for(int i = 0; i < PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory.Length; i++)
                    {
                        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] == CursedSword ||
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] == SmallCursedSword)
                        {
                            WeaponCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i];
                            Losing = "장비 소모 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(WeaponCode).EquipmentName;
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = 0;
                            break;
                        }
                    }
                }
                //여기 들어오면 일단 없어도 진행은 되게
                JsonReadWriteManager.Instance.LkEv_Info.TotoCursedSword = true;
                return 8041;
            case 1:
                return 8042;
        }
        return 8040;
    }
    //---------------------------------------Event8050
    public int Event8050(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        Getting = "";
        Losing = "";
        int BlessedSword = 26003;
        if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
            return 8050;
        }
        switch (ButtonType)
        {
            case 0:
                Getting = "장비 획득 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(BlessedSword).EquipmentName;
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(BlessedSword);
                UIMgr.GI_UI.ActiveGettingUI(BlessedSword);
                JsonReadWriteManager.Instance.LkEv_Info.TotoBlessedSword = true;
                return 8051;
        }
        return 8051;
    }
    //-----------------------------------------Event8060
    public int Event8060(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 체력 30, 피로도 300회복
        //1. gk + 3
        Getting = "";
        Losing = "";
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                Getting = "체력 회복 : " + (30 + RandomHP).ToString() + "\n" +
                    "피로도 회복 : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHP);
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8061;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 3;
                return 8062;
        }
        return 8060;
    }
    //--------------------------------------------Event8070
    public int Event8070(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 체력 -10 피로도 -100
        Getting = "";
        Losing = "";
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                int RandNum = Random.Range(0, 2);
                if(RandNum == 0)
                {
                    Losing = "피로도 소모 : " + (300 + RandomSTA).ToString();
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                }
                else
                {
                    Losing = "체력 소모 : " + (30 + RandomHP).ToString();
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30 - RandomHP);
                }
                return 8071;
        }
        return 8070;
    }
    //--------------------------------------------Event8080
    public int Event8080()
    {
        //아무일 없음
        return 8081;
    }
    //--------------------------------------------Event8090
    public int Event8090(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //체력 -15, 피로도-150
        Getting = "";
        Losing = "";
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                Losing = "체력 소모 : " + (30 + RandomHP).ToString() + "\n" +
                    "피로도 소모 : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30 - RandomHP);
                return 8091;
        }
        return 8090;
    }
    //--------------------------------------------Event8100
    public int Event8100(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        Getting = "";
        Losing = "";
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                int RandNum = Random.Range(0, 2);
                if(RandNum == 0)
                {
                    Getting = "체력 회복 : " + (30 + RandomHP).ToString();
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHP);
                }
                else
                {
                    Getting = "피로도 회복 : " + (300 + RandomSTA).ToString();
                    PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                }
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8101;
        }
        return 8100;
    }
    //--------------------------------------------Event8110
    public int Event8110(PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        Getting = "";
        Losing = "";
        int RandomSTA = Random.Range(-150, 151);
        Getting = "피로도 회복 : " + (300 + RandomSTA).ToString();
        PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
        SoundManager.Instance.PlaySFX("Buff_Healing");
        return 8111;
    }
    //--------------------------------------------Event8120
    public int Event8120()
    {
        return 8121;
    }
    //--------------------------------------------Event8130
    public int Event8130()
    {
        return 8131;
    }
    //--------------------------------------------Event8140
    public int Event8140(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 전투개시                           8141
        //1. 체력, 피로도 소 회복           8142
        Getting = "";
        Losing = "";
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                return 8141;
            case 1:
                Getting = "체력 회복 : " + (30 + RandomHP).ToString() + "\n" +
                    "피로도 회복 : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHP);
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8142;
        }

        return 8140;
    }
    public void Event8141(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
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
    //--------------------------------------------Event8150
    public int Event8150(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 전투                 8151
        //1. 이탈                 8152
        //2. 경험치 중          8153
        Getting = "";
        Losing = "";
        int RandomReward = Random.Range(-(StageAverageReward / 2), (StageAverageReward / 2) + 1);
        switch(ButtonType)
        {
            case 0:
                return 8151;
            case 1:
                return 8152;
            case 2:
                Getting = "경험치 획득 : " + ((StageAverageReward * 2) + RandomReward).ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount((StageAverageReward * 2) + RandomReward);
                UIMgr.GI_UI.ActiveGettingUI(0, true);
                return 8153;
        }
        return 8150;
    }
    public void Event8151(PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, BattleManager BattleMgr)
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
    //--------------------------------------------Event8160
    public int Event8160(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 피로도 회복                 8161
        //1. 탐색도 상승                 8162
        Getting = "";
        Losing = "";
        int RandomSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                Getting = "피로도 회복 : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8161;
            case 1:
                //1 ~ 20 까지중 랜덤으로
                int RandomIncrease = Random.Range(1, 20);
                Getting = "탐색도 상승 : " + RandomIncrease.ToString();
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandomIncrease;
                return 8162;
        }
        return 8160;
    }
    //--------------------------------------------Event8170
    public int Event8170(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 피로도, 체력 회복                 8171
        //1. 이탈                 8172
        Getting = "";
        Losing = "";
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                Getting = "체력 회복 : " + (30 + RandomHP).ToString() + "\n" +
                    "피로도 회복 : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHP);
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8171;
            case 1:
                return 8172;
        }
        return 8170;
    }
}
