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
        //0. �ƹ��� ����
        //1. 50% ü�� +30 50% ü�� -50
        switch(ButtonType)
        {
            case 0:
                return 1001;
            case 1:
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//����
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30);
                    SoundManager.Instance.PlaySFX("Buff_Consume");
                    return 1002;
                }
                else if(Rand == 1)
                {//����
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(30);
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
        //0. �ƹ��� ����
        //1. 50% ����ġ 56/ 50% ����
        switch(ButtonType)
        {
            case 0:
                return 1011;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//�������
                    int RewardRange = (int)(Stage01AverageReward / 4);
                    PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(Stage01AverageReward + Random.Range(-RewardRange, RewardRange +1));

                    UIMgr.GI_UI.ActiveGettingUI(0, true);
                    return 1012;
                }
                else if(Rand == 1)
                {//��� ����
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
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-------------------------------------------------Event1020
    public int Event1020(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. ������ 2���� �������� ��� 3 �ο� 50% // ���� 50%
        //1. �ּ���� ����
        //2. �ƹ��� ����
        switch(ButtonType)
        {
            case 0:
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//��ġ�� ����
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                    JsonReadWriteManager.Instance.LkEv_Info.PowwersCeremony = 2;
                    return 1021;
                }
                else
                {//��ġ�� ����
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
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-------------------------------------------------Event1030
    public int Event1030(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. ��, ����, �ӵ� �� ���� ������ 1����, �Ƿε� -300
        //1. ����
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
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
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        BattleMgr.InitCurrentBattleMonsters();
        BattleMgr.InitMonsterNPlayerActiveGuage();
        BattleMgr.ProgressBattle();
    }
    //-------------------------------------------------Event1040
    public int Event1040(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. �Ƿε� 300ȸ��
        //2. Ž�� ��ġ ����
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 1041;
            case 1:
                //1 ~ 7 ������ ��������
                int RandomIncrease = Random.Range(1, 8);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint += RandomIncrease;
                return 1042;
        }
        return 1040;
    }
    //-------------------------------------------------Event1050
    public int Event1050(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 50% ü�� �Ҹ�30 // 50% �Ƿε� ȸ�� 300
        //1. �Ƿε� �Ҹ� 300
        switch(ButtonType)
        {
            case 0:
                int Rand = Random.Range(0, 2);
                if(Rand == 0)
                {//�ε���
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30);
                    return 1051;
                }
                else
                {//�Ⱥε���
                    PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300);
                    SoundManager.Instance.PlaySFX("Buff_Healing");
                    return 1052;
                }
            case 1:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                return 1053;
        }
        return 1050;
    }
}
