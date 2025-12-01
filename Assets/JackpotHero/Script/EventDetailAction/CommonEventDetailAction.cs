using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEventDetailAction
{
    //-----------------------------------------Event9000
    public int Event9000(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 스테이지 + 1 티어 장비, bk + 3
        //1. 스테이지 평균 보상 * 3, bk+ 3
        //2. 힘 레벨 + 2, bk + 3
        //3. 거절한다.
        Getting = "";
        Losing = "";
        switch (ButtonType)
        {
            case 0:
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return 9000;
                }
                int RandomEquipment = EquipmentInfoManager.Instance.GetFixedTierRandomEquipmnet(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor + 1);
                Getting = "장비 획득 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(RandomEquipment).EquipmentName;
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipment);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);

                return 9001;
            case 1:
                int RewardRange = (int)(StageAverageReward * 3 / 4);
                int RandomReward = (int)(StageAverageReward * 3) + Random.Range(-RewardRange, RewardRange + 1);
                Getting = "경험치 획득 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                UIMgr.GI_UI.ActiveGettingUI(0, true);

                return 9001;
            case 2:
                Getting = "영구적으로 힘 기초 능력치 2 증가";
                JsonReadWriteManager.Instance.LkEv_Info.TradeWithDevil += 2;
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
    public int Event9010(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 체력 90회복 bk + 3
        //1. 체력 60회복 bk + 1
        //2. 체력 30회복
        //3. 체력 30회복 경험치 - 스테이지 보상 gk + 1
        Getting = "";
        Losing = "";
        int RandomAverage = Random.Range(-15, 16);//-15 ~ 15
        switch (ButtonType)
        {
            case 0:
                Getting = "체력 회복 : " + (90 + RandomAverage).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(90 + RandomAverage);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                break;
            case 1:
                Getting = "체력 회복 : " + (60 + RandomAverage).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(60 + RandomAverage);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                break;
            case 2:
                Getting = "체력 회복 : " + (30 + RandomAverage).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomAverage);
                break;
            case 3:
                int RewardRange = (int)(StageAverageReward / 4);
                int RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);

                Getting = "체력 회복 : " + (30 + RandomAverage).ToString();
                Losing = "경험치 소모 : " + RandomReward.ToString();

                PlayerMgr.GetPlayerInfo().PlayerRegenHp(30 + RandomAverage);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                break;
        }
        SoundManager.Instance.PlaySFX("Buff_Healing");
        return 9011;
    }
    //---------------------------------------------Event9020
    public int Event9020(int ButtonType, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 피로도 300회복
        //1. gk + 1
        //2. 체력 -30 피로도 - 300 gk + 3
        Getting = "";
        Losing = "";
        int RandomAverSTA = Random.Range(-150, 151);
        int RandomAverHP = Random.Range(-15, 16);
        switch (ButtonType)
        {
            case 0:
                Getting = "피로도 회복 : " + (300 + RandomAverSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomAverSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 9021;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                return 9022;
            case 2:
                Losing = "체력 소모 : " + (30 - RandomAverHP).ToString() + "\n" + "피로도 소모 : " + (300 + RandomAverSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomAverSTA);
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30 + RandomAverHP);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 3;
                JsonReadWriteManager.Instance.LkEv_Info.TalkingMonster = true;

                return 9023;
        }
        return 9020;
    }
    //------------------------------------------------Event9030
    public int Event9030(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 스테이지 평균 티어 장비, 평균적 보상, bk + 1
        //1. 평균적 보상
        //2. 평균적 보상 , 피로도 -300, gk + 1
        //3. 피로도 -300, gk + 2
        Getting = "";
        Losing = "";
        int RewardRange = 0;
        int RandomReward = 0;
        int RandomSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                //장비 획득
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return 9030;
                }
                int RandomEquipment = EquipmentInfoManager.Instance.GetFixedTierRandomEquipmnet(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor);
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipment);
                //경험치 획득
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                Getting = "장비 획득 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(RandomEquipment).EquipmentName +
                    "\n" + "경험치 획득 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                //카르마 계산
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);

                return 9031;
            case 1:
                //경험치 획득
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                Getting = "경험치 획득 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);

                UIMgr.GI_UI.ActiveGettingUI(0, true);
                return 9031;
            case 2:
                //경험치 획득
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                Getting = "경험치 획득 : " + RandomReward.ToString();
                Losing = "피로도 소모 : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                //스테마나 소모
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                //카르마 계산
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;

                UIMgr.GI_UI.ActiveGettingUI(0, true);
                JsonReadWriteManager.Instance.LkEv_Info.RestInPeace = true;

                return 9032;
            case 3:
                Losing = "피로도 소모 : " + (300 + RandomSTA).ToString();
                //스테미나소모
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                //카르마 계산
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 2;

                JsonReadWriteManager.Instance.LkEv_Info.RestInPeace = true;

                return 9032;
        }
        return 9030;
    }
    //-----------------------------------------Event9040
    public int Event9040(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, ref string Getting, ref string Losing)
    {
        //0. 피로 900회복 bk + 3
        //1. 피로 600회복 bk + 1
        //2. 피로 300회복
        //3. 피로 300회복, -스테이지 평균 보상, gk + 1
        Getting = "";
        Losing = "";
        int RandomSTA = Random.Range(-150, 151);
        switch (ButtonType)
        {
            case 0:
                Getting = "피로도 회복 : " + (900 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(900 + RandomSTA);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                break;
            case 1:
                Getting = "피로도 회복 : " + (600 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(600 + RandomSTA);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                break;
            case 2:
                Getting = "피로도 회복 : " + (300 + RandomSTA).ToString();
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                break;
            case 3:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                int RewardRange = (int)(StageAverageReward / 4);
                int RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                Getting = "피로도 회복 : " + (300 + RandomSTA).ToString();
                Losing = "경험치 소모 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);

                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                break;
        }
        SoundManager.Instance.PlaySFX("Buff_Healing");
        return 9041;
    }
    //---------------------------------------Event9050
    public int Event9050(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 스테이지 평균보상 *3, bk + 3
        //1. bk - 1
        //2. -스테이지 평균보상, bk - 3
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

                return 9051;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma -= 1;

                return 9052;
            case 2:
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                Losing = "경험치 소모 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma -= 2;

                return 9052;
        }
        return 9050;
    }
    //----------------------------------------------Event9060
    public int Event9060(int ButtonType, int StageAverageReward, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. -스테이지 평균 * 3
        //1. +스테이지 평균 
        //2. -스테이지 평균, gk + 2
        Getting = "";
        Losing = "";
        int RewardRange = 0;
        int RandomReward = 0;
        switch (ButtonType)
        {
            case 0:
                RewardRange = (int)(StageAverageReward * 3 / 4);
                RandomReward = (int)(StageAverageReward * 3) + Random.Range(-RewardRange, RewardRange + 1);
                Losing = "경험치 소모 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);

                return 9061;
            case 1:
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                Getting = "경험치 획득 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);

                UIMgr.GI_UI.ActiveGettingUI(0, true);

                return 9062;
            case 2:
                RewardRange = (int)(StageAverageReward / 4);
                RandomReward = StageAverageReward + Random.Range(-RewardRange, RewardRange + 1);
                Losing = "경험치 소모 : " + RandomReward.ToString();
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);

                return 9063;
        }
        return 9060;
    }
    //----------------------------------------------Event9070
    public int Event9070(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr)
    {
        //0. 2티어 이벤트 -120경험치
        //1. 3티어 이벤트 -300경험치
        //2. 4티어 이벤트 -1050경험치
        //3. 5티어 이벤트 -3000경험치
        //4. 떠난다.
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
        //0. 2티어 무기
        //1. 2티어 갑옷
        //2. 2티어 투구
        //3. 2티어 부츠
        //4. 2티어 장신구
        if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
            return CurrentEventCode;
        }
        int RandomEquipmnetCode = 0;
        int CurrentEventEquipTier = (CurrentEventCode % 10) + 1;
        switch(ButtonType)
        {
            case 0:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipType.TypeWeapon);
                break;
            case 1:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipType.TypeArmor);
                break;
            case 2:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipType.TypeHelmet);
                break;
            case 3:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipType.TypeBoots);
                break;
            case 4:
                RandomEquipmnetCode = EquipmentInfoManager.Instance.GetFixedTierNTypeRandomEquipment(CurrentEventEquipTier, EEquipType.TypeAcc);
                break;
        }
        if (RandomEquipmnetCode == 0)
            return CurrentEventCode;

        PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipmnetCode);
        UIMgr.GI_UI.ActiveGettingUI(RandomEquipmnetCode);
        return 9075;
    }
    //-----------------------------------------Event9080
    public int Event9080(int ButtonType, PlayerManager PlayerMgr, PlaySceneUIManager UIMgr, ref string Getting, ref string Losing)
    {
        //0. 아무일 없음
        //1. 저주받은 검 획득, 이거 선택시 다시 발생 x
        Getting = "";
        Losing = "";
        switch (ButtonType)
        {
            case 0:
                return 9081;
            case 1:
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return 9080;
                }

                int OminousSwordCode = 23000;
                Getting = "장비 획득 : " + EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(OminousSwordCode).EquipmentName;
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(OminousSwordCode);
                UIMgr.GI_UI.ActiveGettingUI(OminousSwordCode);
                JsonReadWriteManager.Instance.LkEv_Info.OminousSword = true;

                return 9082;
        }
        return 9080;
    }
    //-------------------------------------------------Event10000
    public void Event10000(int ButtonType, PlayerManager PlayerMgr)//보스조우임 -> 클릭하면 행동선택으로, 보스 확률 100으로
    {
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint = 99999;
                break;
        }
    }
}
