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
        //0. ��պ��� * 3, bk + 1
        //1. bk - 1
        //2. -��պ��� bk - 2
        //3. ���ְ� ������ ��//�÷��̾ �����ϰ� �ְų� �κ��丮�� �մ� ���� �ٲ����
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
        //0. ü��15, �Ƿε�150 ȸ��
        //1. gk+1
        //2. �Ƿε� -300 gk + 2
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(15);
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(150);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8011;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                return 8012;
            case 2:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 2;
                JsonReadWriteManager.Instance.LkEv_Info.TalkingDirtGolem = true;
                return 8013;
        }
        return 8010;
    }
    //----------------------------------------------------Event8020
    public int Event8020(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. �������� ���� * 2
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
        //0.�ż��� ����� 17003
        int HolySword = 17003;
        if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
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
        //0. -���� ������, Ȥ�� - ���ְ� ������ ��
        //1. �ƹ��� X
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
        if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
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
        //0. ü�� 30, �Ƿε� 300ȸ��
        //1. gk + 3
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(15);
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(150);
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
        //0. ü�� -10 �Ƿε� -100
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(100);
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(-10);
                return 8071;
        }
        return 8070;
    }
    //--------------------------------------------Event8080
    public int Event8080()
    {
        //�ƹ��� ����
        return 8081;
    }
    //--------------------------------------------Event8090
    public int Event8090(int ButtonType, PlayerManager PlayerMgr)
    {
        //ü�� -15, �Ƿε�-150
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(150);
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(-15);
                return 8091;
        }
        return 8090;
    }
    //--------------------------------------------Event8100
    public int Event8100(int ButtonType, PlayerManager PlayerMgr)
    {
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(5);
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(50);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 8101;
        }
        return 8100;
    }
}
