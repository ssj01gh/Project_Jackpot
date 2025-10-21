using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class LinkageEventDetailAction
{
    //---------------------------------Event8000
    public int Event8000(int ButtonType, int StageAverageReward,PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        Debug.Log("IsInHere");
        //0. 평균보상 * 3, bk + 1
        //1. bk - 1
        //2. -평균보상 bk - 2
        //3. 저주가 옅어진 검//플레이어가 장착하고 있거나 인벤토리에 잇는 검을 바꿔야함
        int RewardRange = 0;
        int RandomReward = 0;
        switch (ButtonType)
        {
            case 0:
                RewardRange = (int)(StageAverageReward * 3 / 4);
                RandomReward = (int)(StageAverageReward * 3) + Random.Range(-RewardRange, RewardRange + 1);
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
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma -= 2;

                return 8002;
            case 3:
                int SmallCursedSword = 17002;
                int CursedSword = 17001;
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
                UIMgr.GI_UI.ActiveGettingUI(SmallCursedSword);
                JsonReadWriteManager.Instance.LkEv_Info.CleanOminousSword = true;
                return 8003;
        }
        return 8000;
    }
    //-------------------------------------------------Event8010
    public int Event8010(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 체력15, 피로도150 회복
        //1. gk+1
        //2. 피로도 -300 gk + 2
        int RandomSTA = Random.Range(-150, 151);
        int RandomHP = Random.Range(-15, 16);
        switch(ButtonType)
        {
            case 0:
                int RandomNum = Random.Range(0, 2);
                if(RandomNum == 0)
                {
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHP);
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                }
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8011;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                return 8012;
            case 2:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 2;
                JsonReadWriteManager.Instance.LkEv_Info.TalkingDirtGolem = true;
                return 8013;
        }
        return 8010;
    }
    //----------------------------------------------------Event8020
    public int Event8020(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. 스테이지 보상 * 2
        switch(ButtonType)
        {
            case 0:
                int RewardRange = (int)(StageAverageReward * 2 / 4);
                int RandomReward = (int)(StageAverageReward * 2) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                UIMgr.GI_UI.ActiveGettingUI(0, true);
                JsonReadWriteManager.Instance.LkEv_Info.RestInPeace = false;

                return 8021;
        }
        return 8020;
    }
    //---------------------------------------------------Event8030
    public int Event8030(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0.신성이 깃던검 17003
        int HolySword = 17003;
        if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
            return 8030;
        }
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(HolySword);
                UIMgr.GI_UI.ActiveGettingUI(HolySword);
                JsonReadWriteManager.Instance.LkEv_Info.TotoRepayFavor = true;
                return 8031;
        }
        return 8030;
    }
    //-------------------------------------------------Event8040
    public int Event8040(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. -저주 받은검, 혹은 - 저주가 옅어진 검
        //1. 아무일 X
        int SmallCursedSword = 17002;
        int CursedSword = 17001;
        switch (ButtonType)
        {
            case 0:
                if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == CursedSword ||
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == SmallCursedSword)
                {
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = 0;
                }
                else
                {
                    for(int i = 0; i < PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory.Length; i++)
                    {
                        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] == CursedSword ||
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] == SmallCursedSword)
                        {
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = 0;
                            break;
                        }
                    }
                }
                JsonReadWriteManager.Instance.LkEv_Info.TotoCursedSword = true;
                return 8041;
            case 1:
                return 8042;
        }
        return 8040;
    }
    //---------------------------------------Event8050
    public int Event8050(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        int BlessedSword = 17004;
        if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
            return 8050;
        }
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(BlessedSword);
                UIMgr.GI_UI.ActiveGettingUI(BlessedSword);
                JsonReadWriteManager.Instance.LkEv_Info.TotoBlessedSword = true;
                return 8051;
        }
        return 8051;
    }
    //-----------------------------------------Event8060
    public int Event8060(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 체력 30, 피로도 300회복
        //1. gk + 3
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
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
    public int Event8070(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 체력 -10 피로도 -100
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                int RandNum = Random.Range(0, 2);
                if(RandNum == 0)
                {
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                }
                else
                {
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
    public int Event8090(int ButtonType, PlayerManager PlayerMgr)
    {
        //체력 -15, 피로도-150
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30 - RandomHP);
                return 8091;
        }
        return 8090;
    }
    //--------------------------------------------Event8100
    public int Event8100(int ButtonType, PlayerManager PlayerMgr)
    {
        int RandomHP = Random.Range(-15, 16);
        int RandomSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                int RandNum = Random.Range(0, 2);
                if(RandNum == 0)
                {
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomHP);
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                }
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8101;
        }
        return 8100;
    }
}
