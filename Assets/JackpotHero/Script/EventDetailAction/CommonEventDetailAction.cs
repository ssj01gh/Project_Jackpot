using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEventDetailAction
{
    //-----------------------------------------Event9000
    public int Event9000(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. �������� + 1 Ƽ�� ���, bk + 3
        //1. �������� ��� ���� * 3, bk+ 3
        //2. �� ���� + 2, bk + 3
        //3. �����Ѵ�.
        switch (ButtonType)
        {
            case 0:
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return 9000;
                }
                int RandomEquipment = EquipmentInfoManager.Instance.GetFixedTierRandomEquipmnet(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor + 1);
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipment);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);

                return 9001;
            case 1:
                int RewardRange = (int)(StageAverageReward * 3 / 4);
                int RandomReward = (int)(StageAverageReward * 3) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                UIMgr.GI_UI.ActiveGettingUI(0, true);

                return 9001;
            case 2:
                PlayerMgr.GetPlayerInfo().UpgradePlayerSingleStatus("STR", 2);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;

                return 9001;
            case 3:
                return 9002;
        }
        //PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
        //UIMgr.E_UI.ActiveEventUI(this);
        return 9000;
    }
    //-------------------------------------------Event9010
    public int Event9010(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr)
    {
        //0. ü�� 90ȸ�� bk + 3
        //1. ü�� 60ȸ�� bk + 1
        //2. ü�� 30ȸ��
        //3. ü�� 30ȸ�� ����ġ - �������� ���� gk + 1
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(90);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                break;
            case 1:
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(60);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                break;
            case 2:
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(30);
                break;
            case 3:
                int RewardRange = (int)(StageAverageReward / 4);
                int RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);

                PlayerMgr.GetPlayerInfo().PlayerRegenHp(30);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                break;
        }
        SoundManager.Instance.PlaySFX("Buff_Healing");
        return 9011;
    }
    //---------------------------------------------Event9020
    public int Event9020(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. �Ƿε� 300ȸ��
        //1. gk + 1
        //2. ü�� -30 �Ƿε� - 300 gk + 3
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 9021;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                return 9022;
            case 2:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 3;
                JsonReadWriteManager.Instance.LkEv_Info.TalkingMonster = true;

                return 9023;
        }
        return 9020;
    }
    //------------------------------------------------Event9030
    public int Event9030(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. �������� ��� Ƽ�� ���, ����� ����, bk + 1
        //1. ����� ����
        //2. ����� ���� , �Ƿε� -300, gk + 1
        //3. �Ƿε� -300, gk + 2
        int RewardRange = 0;
        int RandomReward = 0;
        switch (ButtonType)
        {
            case 0:
                //��� ȹ��
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return 9030;
                }
                int RandomEquipment = EquipmentInfoManager.Instance.GetFixedTierRandomEquipmnet(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor);
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipment);
                //����ġ ȹ��
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                //ī���� ���
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);

                return 9031;
            case 1:
                //����ġ ȹ��
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);

                UIMgr.GI_UI.ActiveGettingUI(0, true);
                return 9031;
            case 2:
                //����ġ ȹ��
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                //���׸��� �Ҹ�
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                //ī���� ���
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;

                UIMgr.GI_UI.ActiveGettingUI(0, true);
                JsonReadWriteManager.Instance.LkEv_Info.RestInPeace = true;

                return 9032;
            case 3:
                //���׹̳��Ҹ�
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                //ī���� ���
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 2;

                JsonReadWriteManager.Instance.LkEv_Info.RestInPeace = true;

                return 9032;
        }
        return 9030;
    }
    //-----------------------------------------Event9040
    public int Event9040(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr)
    {
        //0. �Ƿ� 900ȸ�� bk + 3
        //1. �Ƿ� 600ȸ�� bk + 1
        //2. �Ƿ� 300ȸ��
        //3. �Ƿ� 300ȸ��, -�������� ��� ����, gk + 1
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(900);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                break;
            case 1:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(600);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                break;
            case 2:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300);
                break;
            case 3:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300);
                int RewardRange = (int)(StageAverageReward / 4);
                int RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);

                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                break;
        }
        SoundManager.Instance.PlaySFX("Buff_Healing");
        return 9041;
    }
    //---------------------------------------Event9050
    public int Event9050(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. �������� ��պ��� *3, bk + 3
        //1. bk - 1
        //2. -�������� ��պ���, bk - 3
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

                return 9051;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma -= 1;

                return 9052;
            case 2:
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma -= 2;

                return 9052;
        }
        return 9050;
    }
    //----------------------------------------------Event9060
    public int Event9060(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. -�������� ��� * 3
        //1. +�������� ��� 
        //2. -�������� ���, gk + 2
        int RewardRange = 0;
        int RandomReward = 0;
        switch (ButtonType)
        {
            case 0:
                RewardRange = (int)(StageAverageReward * 3 / 4);
                RandomReward = (int)(StageAverageReward * 3) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);

                return 9061;
            case 1:
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);

                UIMgr.GI_UI.ActiveGettingUI(0, true);

                return 9062;
            case 2:
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);

                return 9063;
        }
        return 9060;
    }
    //----------------------------------------------Event9070
    public int Event9070(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. 2Ƽ�� �̺�Ʈ -120����ġ
        //1. 3Ƽ�� �̺�Ʈ -300����ġ
        //2. 4Ƽ�� �̺�Ʈ -1050����ġ
        //3. 5Ƽ�� �̺�Ʈ -3000����ġ
        //4. ������.
        switch(ButtonType)
        {
            case 0:
                if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 120)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughEXP_ForgeEvent);
                    return 9070;
                }
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-120, true);
                return 9071;
            case 1:
                if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 300)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughEXP_ForgeEvent);
                    return 9070;
                }
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-300, true);
                return 9072;
            case 2:
                if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 1050)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughEXP_ForgeEvent);
                    return 9070;
                }
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-1050, true);
                return 9073;
            case 3:
                if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 3000)
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughEXP_ForgeEvent);
                    return 9070;
                }
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-3000, true);
                return 9074;
            case 4:
                return 9076;
        }
        return 9070;
    }

    public int Event9071_4(int ButtonType, int CurrentEventCode, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //Tier = 2 -> 9071 // 3-> 9072 // 4-> 9073 // 5-> 9074
        //0. 2Ƽ�� ����
        //1. 2Ƽ�� ����
        //2. 2Ƽ�� ����
        //3. 2Ƽ�� ����
        //4. 2Ƽ�� ��ű�
        if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
            return CurrentEventCode;
        }
        int RandomEquipmnetCode = 0;
        int CurrentEventEquipTier = (CurrentEventCode % 10) + 1;
        switch(ButtonType)
        {
            case 0:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipmentType.Weapon);
                break;
            case 1:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipmentType.Armor);
                break;
            case 2:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipmentType.Helmet);
                break;
            case 3:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipmentType.Boots);
                break;
            case 4:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipmentType.Accessories);
                break;
        }
        if (RandomEquipmnetCode == 0)
            return CurrentEventCode;

        PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipmnetCode);
        UIMgr.GI_UI.ActiveGettingUI(RandomEquipmnetCode);
        return 9075;
    }
    //-----------------------------------------Event9080
    public int Event9080(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. �ƹ��� ����
        //1. ���ֹ��� �� ȹ��, �̰� ���ý� �ٽ� �߻� x
        switch (ButtonType)
        {
            case 0:
                return 9081;
            case 1:
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return 9080;
                }
                //���ֹ��� �� �ڵ� = 17001
                int OminousSwordCode = 17001;
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(OminousSwordCode);
                UIMgr.GI_UI.ActiveGettingUI(OminousSwordCode);
                JsonReadWriteManager.Instance.LkEv_Info.OminousSword = true;

                return 9082;
        }
        return 9080;
    }
    //-------------------------------------------------Event10000
    public void Event10000(int ButtonType, PlayerManager PlayerMgr)//���������� -> Ŭ���ϸ� �ൿ��������, ���� Ȯ�� 100����
    {
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint = 99999;
                break;
        }
    }
}
