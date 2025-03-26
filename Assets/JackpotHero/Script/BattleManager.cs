using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public enum EBattleStates
{
    Idle,
    PlayerTurn,
    MonsterTurn,
}

public class BattleResultStates
{
    public float BaseAmount;
    public float BaseAmountPlus;
    public List<float> ResultMagnification = new List<float>();
    public float BuffMagnification;
    public float FinalResultAmountPlus;
    public float FinalResultAmount;
}

public class BattleManager : MonoBehaviour
{
    protected List<GameObject> BattleTurn = new List<GameObject>();
    [SerializeField]
    private PlayerManager PlayerMgr;
    [SerializeField]
    private MonsterManager MonMgr;
    [SerializeField]
    private PlaySceneUIManager UIMgr;
    // Start is called before the first frame update
    public GameObject CurrentTurnObject { protected set; get; }
    public BattleResultStates BattleResultStatus { protected set; get; } = new BattleResultStates();
    public int CurrentState { protected set; get; } = (int)EBattleStates.Idle;

    protected GameObject TargetObject = null;
    protected float PlayerActiveGauge = 0;

    protected bool IgnoreBuffProgressAtFirstBattleProgress = false;
    //protected List<float> MonsterActiveGuage = new List<float>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.P))
        {
            foreach(GameObject obj in BattleTurn)
            {
                Debug.Log(obj.name);
            }
        }
        */
    }
    public List<GameObject> GetBattleTurn()
    {
        return BattleTurn;
    }

    public void InitCurrentBattleMonsters()//�����Ҷ� �ѹ��� �����
    {
        IgnoreBuffProgressAtFirstBattleProgress = true;
        MonMgr.SetSpawnPattern(PlayerMgr);//������ ���� ���� ����
        MonMgr.SpawnCurrentSpawnPatternMonster();//���� ���Ͽ� �°� ���� ����//ActiveMonster����
        //���� ������ ������� �ٽ� ����
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    public void InitMonsterNPlayerActiveGuage()//�ʱ�ȭ
    {
        BattleTurn.Clear();
        PlayerActiveGauge = 0;
        for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
        {
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge = 0;
        }

        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack != -1)//������ ���ߴٸ�
        {
            if (MonMgr.GetActiveMonsters().Count >= 1)
            {
                //SetBattleTurn���� ù��°���� ����� ������
                //ActiveMonster[0]�� �ѹ� ���� ������ �ٽ� �־����
                BattleTurn.Add(MonMgr.GetActiveMonsters()[0]);
            }
            for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                BattleTurn.Add(MonMgr.GetActiveMonsters()[i]);
            }
        }
    }

    public void ProgressBattle()//�̰� ������ ��ӵǴ� ���� ���������� �ҷ������ߵ� �Լ���
    {
        //���⿡�� Ư�� ������ ����������� ����ҵ�(�ϴ��̴ϱ�)
        if(IgnoreBuffProgressAtFirstBattleProgress == true)
            IgnoreBuffProgressAtFirstBattleProgress = false;
        else
            BuffProgress();//�̰� ó���� �����ϰ��ؾ���

        MonMgr.CheckActiveMonstersRSurvive();//���� ������ ������ ���� ���� ����
        SetBattleTurn();//�÷��̾�� ������ SPD���� ������ ���� Turn�� ����->�Ƹ� ���⼭ Ȯ���ҵ�?
        DecideCurrentBattleTurn();//���⼭ ���� ������ �������� ����
        UIMgr.B_UI.SetBattleTurnUI(BattleTurn);//������ Turn�� ui�� ǥ��
        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//�÷��̾� ������ ����
        MonMgr.SetCurrentTargetMonster(null);//CurrentTarget�ʱ�ȭ
        UIMgr.B_UI.UnDisplayMonsterDetailUI();//������ �� ǥ�� UnActive//�� �������ͽ��� ����� �͵�;
        UIMgr.B_UI.SetPlayerBattleUI(PlayerMgr.GetPlayerInfo());
        UIMgr.B_UI.SetMonsterBattleUI(MonMgr.GetActiveMonsters());
        //UIMgr.B_UI.ActivePlayerShieldNBuffUI(PlayerMgr.GetPlayerInfo());//�̰� ���� ���Ͱ� �׾����� �ƴ��� Ȯ���ؾߵ�

        if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP <= 0)
        {
            //��������Ʈ�� �������� ��������Ʈ�� ����ȴٴ���?
            //ActiveMonster���δ� ����
            MonMgr.InActiveAllActiveMonster();
            //UIMgr.B_UI.ActivePlayerShieldNBuffUI(PlayerMgr.GetPlayerInfo());//�����ִ� Set~~�� �� ���ֱ� ����
            PlayerMgr.GetPlayerInfo().DefeatFromBattle();
            UIMgr.B_UI.DefeatBattle(PlayerMgr.GetPlayerInfo());
            UIMgr.PlayerDefeat();//�÷��̾��� ���â, ����â, ���� �������� ����â �����͵�
            //�ٸ� �÷��̾� UI�� ������
            return;
        }
        if (MonMgr.GetActiveMonsters().Count <= 0)
        {
            Debug.Log("Winner");
            //������ ��밡 ������°��� Ȯ���� ����� �ִ°�?//������������ �������� 199�̷������� �Ѵ�? %100�� ������ 99��� �����ΰ���
            //%100�� ���� 90�̻��̸� �����ΰ���. ������ 1���� �������� ������
            if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 100 >= 90)
            {
                //PlayerMgr.GetPlayerInfo().DefeatFromBattle();
                //UIMgr.B_UI.WinGame(PlayerMgr.GetPlayerInfo());
                //UIMgr.PlayerDefeat();
            }
            else//������ �ƴ϶�� ����ϰ�
            {
                PlayerMgr.GetPlayerInfo().EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
                                                        //�޽��߿� ������ �����Ŷ�� �ٽ� �޽� �ൿ �������� ���ư�
                int RewardEXP = PlayerMgr.GetPlayerInfo().ReturnEXPByEXPMagnification((int)MonMgr.CurrentSpawnPatternReward);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RewardEXP, true);
                UIMgr.B_UI.VictoryBattle(RewardEXP);
            }
            return;
            //�� ������ ������ ������ ������.
        }


        if (CurrentState == (int)EBattleStates.PlayerTurn)
        {
            //�÷��̾��� ���̶�� �÷��̾��� �ൿ�� �����ϴ� ��ư�� ��Ÿ���� �Ѵ�.
            UIMgr.B_UI.ActiveBattleSelectionUI(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] >= 1);//�ൿ ���� ��ư�� ��Ÿ��
        }
        else if (CurrentState == (int)EBattleStates.MonsterTurn)
        {
            //������ ���̶�� �ൿ ��ư�� ��Ÿ��
            UIMgr.B_UI.ActiveBattleSelectionUI_Mon();
            /*
            MonMgr.SetCurrentTargetMonster(BattleMgr.CurrentTurnObject.GetComponent<Monster>());
            BattleButtonClick("Monster");
            */
        }
    }

    //
    public void PlayerBattleActionSelectionButtonClick(string ActionButtonType)//���� ��ư�� �ൿ�� ���� �޶���
    {
        //������ ��� ���� ��밡 ������ �־����
        if (ActionButtonType == "Attack" && MonMgr.CurrentTarget == null)
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.AttackGuideMessage);
            return;
        }
        if (PlayerMgr.GetPlayerInfo().SpendSTA(ActionButtonType) == false)//�Ƿε��� �����ϸ�
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughSTAMessage_Battle);
            return;
        }
        //�ൿ�ϱ����� �����ؾ� �Ұ͵��� ���⼭ �ҵ�?//���Ⱑ �ƴ϶� �� �������� �����ؾ���//DecideTurn����
        //PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount = 0;
        SetPlayerBattleStatus(ActionButtonType);

        if (ActionButtonType == "Attack")//�����̶�� BattleMgr���� ���� ����� ���� Ÿ���� ü���� ����
        {
            PlayerMgr.GetPlayerInfo().PlayerRecordGiveDamage(BattleResultStatus.FinalResultAmount);
            TargetObject = MonMgr.CurrentTarget.gameObject;
            //��ġ�� �� ���� ������
            if(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.OverWhelmingPower] >= 1)
            {//ü�� + ��ȣ�� ���� �������� ������
                if (MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentHP + MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint
                    < BattleResultStatus.FinalResultAmount)
                {
                    List<GameObject> UnTargetMonsters = new List<GameObject>();
                    foreach(GameObject obj in MonMgr.GetActiveMonsters())
                    {
                        if(obj != MonMgr.CurrentTarget.gameObject)
                        {
                            UnTargetMonsters.Add(obj);
                        }
                    }
                    //������ ���Ϳ� �� ������ ���
                    float LastDamage = 
                        (BattleResultStatus.FinalResultAmount - MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentHP) / UnTargetMonsters.Count;
                    foreach(GameObject Mon in UnTargetMonsters)
                    {
                        Mon.GetComponent<Monster>().MonsterDamage((int)LastDamage);
                    }
                }
            }
            //���� ���ð��� ������
            if (MonMgr.CurrentTarget.MonsterBuff.BuffList[(int)EBuffType.ThronArmor] >= 1)
            {//���� �ִ� �������� ���� ��ȣ���� �� ������ => ���� ������ < ���� ��ȣ��
                if(BattleResultStatus.FinalResultAmount < MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint)
                {
                    float ReflectionDamage =
                        MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint - BattleResultStatus.FinalResultAmount;

                    PlayerMgr.GetPlayerInfo().PlayerDamage((int)ReflectionDamage);
                }
            }
            //��Ż ������
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Rapine] >= 1)
            {
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount((int)(BattleResultStatus.FinalResultAmount / 5));
            }
            MonMgr.CurrentTarget.MonsterDamage(BattleResultStatus.FinalResultAmount);
        }
        else if (ActionButtonType == "Defense")//����� ����� �÷��̾��� ��� ��ġ�� ����
        {
            TargetObject = null;
            PlayerMgr.GetPlayerInfo().PlayerGetShield(BattleResultStatus.FinalResultAmount);
        }
        else if (ActionButtonType == "Rest")//�޽��̶�� ����� �÷��̾��� �Ƿε��� ȸ����
        {
            TargetObject = null;
            PlayerMgr.GetPlayerInfo().PlayerRegenSTA(BattleResultStatus.FinalResultAmount);
            //����޽� ������
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.AdvancedRest] >= 1)
            {
                PlayerMgr.GetPlayerInfo().PlayerRegenHp((int)(BattleResultStatus.FinalResultAmount / 10));
            }
        }
        //�� �Լ��� �����Ű�� ���� BattleMgr���� �÷��̾� �ൿ�� ���� ������� ����� -> �̰� BattleUI�� �� ���� �޾Ƽ� �۵��ϸ� �ɵ�?
        //���� ������ ��ġ�� �������� MainBattle�� Ȱ��ȭ ��Ŵ
        UIMgr.EDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.MEDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentForm, MonMgr.CurrentTarget, ActionButtonType, BattleResultStatus, CurrentState, ProgressBattle);
    }

    public void MonsterBattleActionSelectionButtonClick()
    {
        CalculateMonsterAction();
        string CurrentBattleState;
        switch (CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState)
        {
            case (int)EMonsterActionState.Attack:
                //�������� �÷��̾ �������ִ� ���庸�� �۰ų� ���ٸ�
                CurrentBattleState = "Attack";
                TargetObject = PlayerMgr.GetPlayerInfo().gameObject;
                //�÷��̾ ���� ������ ������ �������
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ThronArmor] >= 1)
                {//������ < �÷��̾� ��ȣ�� �ϰ��
                    if (BattleResultStatus.FinalResultAmount < PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount)
                    {
                        float ReflectionDamage = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount - BattleResultStatus.FinalResultAmount;
                        CurrentTurnObject.GetComponent<Monster>().MonsterDamage(ReflectionDamage);
                    }
                }
                //��Ż�� ������ ������
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Rapine] >= 1)
                {
                    PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-(int)(BattleResultStatus.FinalResultAmount / 5), true);
                }
                PlayerMgr.GetPlayerInfo().PlayerDamage(BattleResultStatus.FinalResultAmount);
                break;
            case (int)EMonsterActionState.Defense:
                CurrentBattleState = "Defense";
                TargetObject = null;
                CurrentTurnObject.GetComponent<Monster>().MonsterGetShield(BattleResultStatus.FinalResultAmount);
                break;
            default:
                CurrentBattleState = "Another";
                break;
        }

        CurrentTurnObject.GetComponent<Monster>().SetNextMonsterState();//������ �ൿ ���Ͽ� ���� ���� �ൿ ����
        UIMgr.EDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.MEDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentForm, CurrentTurnObject.GetComponent<Monster>(), CurrentBattleState, BattleResultStatus, CurrentState, ProgressBattle);
    }
    public void PressVictoryButton()
    {
        UIMgr.B_UI.ClickVictoryButton();
        UIMgr.GI_UI.ActiveGettingUI();
        UIMgr.SetUI();
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
    }

    public void PressDefeatButton()//�̰������� �Ȱ����ϳ�
    {
        if(JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 2)
        {//2�̻��̸� ��� �����ϰ� �κ��丮�� �ִ´� -> 
            //�ʱ�ȭ->�ϱ� ���� ��� ���� ������ ��� �־���� �۾��ϸ� �ɵ�?
            //JsonManager�� �ִ� ���� ������ ������ �����Ҷ� ���ŉ����� ������ ����
            SetEquipSuccession();
        }
        else
        {
            JsonReadWriteManager.Instance.InitPlayerInfo(true);//�ʱ�ȭ
            JsonReadWriteManager.Instance.InitEarlyStrengthenInfo(true);//ReachFloor�� EarlyPoint�� �����ϰ� �ʱ�ȭ��Ŵ
        }
        LoadingScene.Instance.LoadAnotherScene("TitleScene");
        //�ʱ�ȭ JsonManager�� P_Info �ʱ�ȭ
    }

    protected void SetEquipSuccession()
    {
        JsonReadWriteManager.Instance.InitPlayerInfo(true);//�ʱ�ȭ
        JsonReadWriteManager.Instance.InitEarlyStrengthenInfo(true);//ReachFloor�� EarlyPoint�� �����ϰ� �ʱ�ȭ��Ŵ
        List<int> InPossessionEquip = new List<int>();
        //����ϰ��ִ� ��� �ڵ� ����
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode);
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode);
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode);
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode);
        InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode);
        //�κ��丮�� �ִ� ��� �ڵ�����
        for(int i = 0; i < (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP"); i++)
        {
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)
            {
                InPossessionEquip.Add(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]);
            }
        }
        //�� ���������� ��� ������ �°� n���� ������ ��� ����
        for(int i = 0; i < (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIPSUC"); i++)
        {
            if (InPossessionEquip.Count <= 0)
                break;

            int RandNum = Random.Range(0, InPossessionEquip.Count);
            JsonReadWriteManager.Instance.P_Info.EquipmentInventory[i] = InPossessionEquip[RandNum];
            InPossessionEquip.RemoveAt(RandNum);
        }

    }

    public void PressTurnUIImage(int ButtonNum)//���� ĭ���� ���� ��ư �������� ���
    {
        GameObject obj;
        obj = GetBattleTurn()[ButtonNum];

        if (obj.tag == "Monster")
        {
            MonMgr.SetCurrentTargetMonster(obj.GetComponent<Monster>());
        }
    }
    protected void BuffProgress()
    {
        //�� �÷��̾�� ���Ϳ� ������ �����ϸ� ������ ���� 1�� ���ҽ�Ű�� �װſ� ���缭 �������� �ְų� �ؾ���
        for(int i = 0; i < (int)EBuffType.CountOfBuff; i++)
        {
            BuffCalculate(i);
        }
    }
    protected void BuffCalculate(int BuffsType)
    {
        //�÷��̾��� ���� ���
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[BuffsType] > 0)
        {
            switch (BuffsType)//1�� �پ���� �ʰų� �������� �ְų� ȸ����Ű�°͸�
            {
                //��� , ������, ȭ��, ��, ������ ����, ���������, ������, �һ�(�̳��� �������� ���Ǿ� �ɰ� ������)
                case (int)EBuffType.HealingFactor:
                    float LostHP = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP - PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP;
                    PlayerMgr.GetPlayerInfo().PlayerRegenHp((int)(LostHP / 20));
                    break;
                case (int)EBuffType.ReCharge:
                    float LostSTA = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxSTA - PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA;
                    PlayerMgr.GetPlayerInfo().PlayerRegenSTA((int)(LostSTA / 20));
                    break;
                case (int)EBuffType.Burn:
                    float BurnDamage = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP / 20;
                    PlayerMgr.GetPlayerInfo().PlayerDamage((int)BurnDamage);
                    break;
                case (int)EBuffType.Poison:
                    PlayerMgr.GetPlayerInfo().PlayerDamage(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison]);
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison] = PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison] / 2;
                    break;
                case (int)EBuffType.CurseOfDeath:
                    if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath] <= 1)//1���� ������ = ���� 0�̵ȴ� = �������� �Դ´�.
                    {//������� ���� CurseOfDeath�� ������ 0�ʰ� 1���� �� 1��
                        float CurseOfDeathDamage = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP * 9 / 10;
                        PlayerMgr.GetPlayerInfo().PlayerDamage((int)CurseOfDeathDamage);
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath] = 0;
                    }
                    else
                    {//0�ʰ��̱⸸ �Ҷ��� --
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath]--;
                    }
                    break;
                case (int)EBuffType.RegenArmor:
                    PlayerMgr.GetPlayerInfo().PlayerGetShield(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.RegenArmor]);
                    break;
                case (int)EBuffType.Weakness:
                    float WeaknessSpendSTA = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA / 20;
                    PlayerMgr.GetPlayerInfo().PlayerSpendSTA(WeaknessSpendSTA);
                    break;
                case (int)EBuffType.UnDead:
                    if(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP < 1)
                    {
                        PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP = 1;
                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentHpRatio =
                            PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP / PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP;
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnDead] = 0;
                    }
                    else
                    {//���� ������ �ƴ϶�� --
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnDead]--;
                    }
                    break;
            }

            if(BuffsType != (int)EBuffType.Poison && BuffsType != (int)EBuffType.CurseOfDeath &&
                BuffsType != (int)EBuffType.UnDead && BuffsType != (int)EBuffType.Defenseless)//�� �� ������ ����, �һ�, ������ ���� ���
            {
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[BuffsType]--;
            }
        }

        foreach(GameObject Mon in MonMgr.GetActiveMonsters())
        {
            Monster MonInfo = Mon.GetComponent<Monster>();
            if (MonInfo.MonsterBuff.BuffList[BuffsType] > 0)
            {
                switch (BuffsType)//1�� �پ���� �ʰų� �������� �ְų� ȸ����Ű�°͸�
                {
                    //��� , ������, ȭ��, ��, ������ ����, ���������, ������, �һ�(�̳��� �������� ���Ǿ� �ɰ� ������)
                    case (int)EBuffType.HealingFactor:
                        float LostHP = MonInfo.GetMonsterCurrentStatus().MonsterMaxHP - MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP;
                        MonInfo.MonsterRegenHP((int)(LostHP / 20));
                        break;
                    case (int)EBuffType.Burn:
                        float BurnDamage = MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP / 20;
                        MonInfo.MonsterDamage((int)BurnDamage);
                        break;
                    case (int)EBuffType.Poison:
                        MonInfo.MonsterDamage(MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison]);
                        MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison] = MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison] / 2;
                        break;
                    case (int)EBuffType.CurseOfDeath:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath] <= 1)
                        {
                            float CurseOfDeathDamage = MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP * 9 / 10;
                            MonInfo.MonsterDamage((int)CurseOfDeathDamage);
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath] = 0;
                        }
                        else
                        {
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath]--;
                        }
                        break;
                    case (int)EBuffType.RegenArmor:
                        MonInfo.MonsterGetShield(MonInfo.MonsterBuff.BuffList[(int)EBuffType.RegenArmor]);
                        break;
                    case (int)EBuffType.UnDead:
                        if(MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP < 1)
                        {
                            MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP = 1;
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.UnDead] = 0;
                        }
                        else
                        {
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.UnDead]--;
                        }
                        break;
                }

                if (BuffsType != (int)EBuffType.Poison && BuffsType != (int)EBuffType.CurseOfDeath &&
                    BuffsType != (int)EBuffType.UnDead)//�� �� ������ ����, �һ�� ���� ���
                {
                    MonInfo.MonsterBuff.BuffList[BuffsType]--;
                }
            }
        }
    }
    public void SetBattleTurn()
    {
        if (BattleTurn.Count >= 1)
            BattleTurn.RemoveAt(0);//ù��°���� �����.(�ֳ��ϸ� ���⿡ ũ�Ⱑ 1�̻��� BattleTurn�� ���Դٴ°��� �̹� 0��°�� �ִ� ������Ʈ�� ���� ������

        for(int i = BattleTurn.Count - 1; i >= 0; i--)
        {
            if (BattleTurn[i].tag == "Monster")
            {
                if (BattleTurn[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentHP <= 0)
                {
                    Debug.Log("Empty");
                    BattleTurn.RemoveAt(i);
                }
            }
        }
        float PlayerSPD = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().TotalSPD;

        float[] MonsterSPD = new float[MonMgr.GetActiveMonsters().Count];
        for (int i = 0; i < MonsterSPD.Length; i++)
        {
            MonsterSPD[i] = MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentSPD;
        }

        while(BattleTurn.Count < 9)
        {
            PlayerActiveGauge += PlayerSPD;
            //ActiveMonster�� ������ �ٿ��� MonsterActiveGuage�� ������ �״�� ������ �ȴ�.
            //���� index1�� ���Ͱ� ������ ������ index2���� ���Ͱ� index1�� ���� MonsterActiveGuage�� �̾�ް� �Ǵ� ������ �����.
            for(int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge += MonsterSPD[i];
            }


            if(PlayerActiveGauge >= 100)
            {
                PlayerActiveGauge -= 100;
                BattleTurn.Add(PlayerMgr.GetPlayerInfo().gameObject);
            }

            List<int> ChargedMonster = new List<int>();
            for(int i = 0; i < MonsterSPD.Length; i++)
            {
                if (MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge >= 100)
                {
                    ChargedMonster.Add(i);
                    //MonsterActiveGuage[i] -= 100;
                    //BattleTurn.Enqueue(MonMgr.GetActiveMonsters()[i]);
                }
            }

            while(ChargedMonster.Count > 0)
            {
                float FastestMonster = MonMgr.GetActiveMonsters()[ChargedMonster[0]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge;
                int ChargedMonsterIndex = 0;
                for(int i = 0; i < ChargedMonster.Count; i++)
                {
                    if(FastestMonster < MonMgr.GetActiveMonsters()[ChargedMonster[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                    {
                        FastestMonster = MonMgr.GetActiveMonsters()[ChargedMonster[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge;
                        ChargedMonsterIndex = i;
                    }
                }
                //����� ������ FastestMonster�� �����Ȱ���
                MonMgr.GetActiveMonsters()[ChargedMonster[ChargedMonsterIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge -= 100;
                BattleTurn.Add(MonMgr.GetActiveMonsters()[ChargedMonster[ChargedMonsterIndex]]);
                ChargedMonster.RemoveAt(ChargedMonsterIndex);
            }
        }
        CurrentState = (int)EBattleStates.Idle;
    }

    /*
    protected void RecordPlayerNMonsterBeforeShield()//DecideCurrentBattleTurn �� ����Ǳ� ���� ����Ǿ���
    {//���ͳ� �÷��̾ �¾������� Ȯ���ؾ� ���ٵ�?
        PlayerMgr.GetPlayerInfo().RecordBeforeShield();
        foreach (GameObject Mon in MonMgr.GetActiveMonsters())
        {
            Mon.GetComponent<Monster>().RecordMonsterBeforeShield();
        }
    }
    */

    protected void DecideCurrentBattleTurn()
    {
        if(CurrentState == (int)EBattleStates.Idle)
        {
            if(CurrentTurnObject != null)
            {
                if (CurrentTurnObject != PlayerMgr.GetPlayerInfo().gameObject)//���� �Ͽ� �ִ��� �÷��̾ �ƴҶ�
                {
                    //(TargetObject != null && TargetObject != PlayerMgr.GetPlayerInfo().gameObject)
                    if(TargetObject != null)//Null�� �ƴҶ��� �� Ÿ���� ���� �ƴϿ��� ���Ű���
                    {
                        if (TargetObject != PlayerMgr.GetPlayerInfo().gameObject)
                        {
                            PlayerMgr.GetPlayerInfo().RecordBeforeShield();
                        }
                    }
                    else//Ÿ���� ���� �������� �ƴϴϱ� ���Ű���
                    {
                        PlayerMgr.GetPlayerInfo().RecordBeforeShield();
                    }
                }
                foreach (GameObject Mon in MonMgr.GetActiveMonsters())
                {
                    if(CurrentTurnObject != Mon)//���� ���̿����� �ش� ���Ͱ� �ƴѳ�鸸 
                    {
                        //(TargetObject != null && TargetObject != Mon)
                        if(TargetObject != null)
                        {
                            if(TargetObject != Mon)
                            {
                                //���⿡ �ش��Ѵ� ���͵��� RegenArmor�� ������ Record�� ���� �ʴ´�?
                                if (Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.RegenArmor] <= 0)
                                    Mon.GetComponent<Monster>().RecordMonsterBeforeShield();
                            }
                        }
                        else
                        {
                            //���⿡ �ش��Ѵ� ���͵��� RegenArmor�� ������ Record�� ���� �ʴ´�?
                            if (Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.RegenArmor] <= 0)
                                Mon.GetComponent<Monster>().RecordMonsterBeforeShield();
                        }
                    }
                }
            }
            else
            {
                PlayerMgr.GetPlayerInfo().RecordBeforeShield();
                foreach (GameObject Mon in MonMgr.GetActiveMonsters())
                {
                    Mon.GetComponent<Monster>().RecordMonsterBeforeShield();
                }
            }

            CurrentTurnObject = BattleTurn[0];
            //BattleTurn.RemoveAt(0);
            if (CurrentTurnObject.tag == "Player")
            {
                CurrentState = (int)EBattleStates.PlayerTurn;
                ActionOfStartPlayerTurn();
            }
            else if (CurrentTurnObject.tag == "Monster")
            {
                CurrentState = (int)EBattleStates.MonsterTurn;
                ActionOfStartMonsterTurn(CurrentTurnObject);
            }
        }
    }



    protected void ActionOfStartPlayerTurn()//�÷��̾����� �Ǿ�����
    {
        //������ �ϼ��� ���δٴ���, ü���� �����Ѵٴ��� �ϴ°͵�
        PlayerMgr.GetPlayerInfo().RecordBeforeShield();
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnbreakableArmor] >= 1)
        {
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount = 
                (int)(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount * 0.5f);
        }
        else
        {
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount = 0;
        }
        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Defenseless] = 0;
    }

    protected void ActionOfStartMonsterTurn(GameObject Monster)
    {
        Monster.GetComponent<Monster>().RecordMonsterBeforeShield();
        if (Monster.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.UnbreakableArmor] >= 1)
        {
            Monster.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint =
                (int)(Monster.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint * 0.5f);
        }
        else
        {
            Monster.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint = 0;
        }
        Monster.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Defenseless] = 0;
    }

    //CalculatePlayerAction
    protected void SetPlayerBattleStatus(string ActionButtonType)
    {
        PlayerInfo P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        TotalPlayerState TP_Info = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo();
        float AllEquipTier = PlayerMgr.GetPlayerInfo().GetAllEquipTier();

        int EquipCode = 0;
        switch (ActionButtonType)
        {
            case "Attack":
                EquipCode = P_Info.EquipWeaponCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalSTR;
                break;
            case "Defense":
                EquipCode = P_Info.EquipArmorCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalDUR;
                break;
            case "Rest":
                EquipCode = P_Info.EquipHatCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalRES;
                break;
        }
        EquipmentSO Equipment_Info = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(EquipCode);


        //���� �߰� ��ġ // ���� �����ϰ� EQUIP �ʹ� ��ȭ ȿ���� ������ �޴� ��ġ��
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.WeaponMaster] >= 1)//���������� ������ �������϶�
        {
            //�÷��̾ ���� �ִ� ������ ������ ��� ������ Ƽ���� ��
            BattleResultStatus.BaseAmountPlus = AllEquipTier;
        }
        else
        {
            BattleResultStatus.BaseAmountPlus = 0f;
        }
        //������//������ slot�� �������� �����ؾ���, luck�� ������ �޾Ƽ� ������
        //�� ������ ���� ������Ʈ ����ŭ
        BattleResultStatus.ResultMagnification.Clear();
        for (int i = 0; i < Equipment_Info.EquipmentSlots.Length; i++)
        {
            List<float> NegativeList = new List<float>();
            List<float> PositiveList = new List<float>();
            float WholeSlotStateAmount = Equipment_Info.EquipmentSlots[i].SlotState.Length;
            float MultiplyNum = 60 / WholeSlotStateAmount;
            float NegativeAmount = 0f;
            float PositiveAmount = 0f;
            for (int j = 0; j < Equipment_Info.EquipmentSlots[i].SlotState.Length; j++)
            {
                if (Equipment_Info.EquipmentSlots[i].IsPositive[j] == true)
                {
                    PositiveAmount++;
                    PositiveList.Add(Equipment_Info.EquipmentSlots[i].SlotState[j]);
                }
                else
                {
                    NegativeAmount++;
                    NegativeList.Add(Equipment_Info.EquipmentSlots[i].SlotState[j]);
                }
            }
            int LuckBuffNum = 0;
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Luck] >= 1)
            {
                LuckBuffNum += 30;
            }
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Misfortune] >= 1)
            {
                LuckBuffNum -= 30;
            }

            //Debug.Log("���� : ���� = 1 : 2 => ���� : 0 ~ 20"  + " ���� : 20 ~ " + (60 + (int)TP_Info.TotalLUK + LuckBuffNum));
            //Luk�� �̳� �������°� ����ó��
            int RandNum;
            if (60 + (int)TP_Info.TotalLUK + LuckBuffNum<= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)TP_Info.TotalLUK + LuckBuffNum);
            }

            if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //������ List���� �ϳ��� �̾Ƽ� ����
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)TP_Info.TotalLUK + LuckBuffNum)
            {
                //������ List���� �ϳ��� �̾Ƽ� ����
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
        }
        //������ ���� ����
        BattleResultStatus.BuffMagnification = 1f;
        switch (ActionButtonType)
        {
            case "Attack":
                //����
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.AttackDebuff] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 0.5f;
                }
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ToughFist] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 2f;
                }
                break;
            case "Defense":
                //���
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.DefenseDebuff] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 0.5f;
                }
                //��� ������
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ToughSkin] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 2f;
                }
                break;
            case "Rest":
                break;
        }
        //���� �߰� ��ġ // ���� �����ϰ� EXPMG �ʹ� ��ȭ ȿ���� ������ �޴� ��ġ��
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.EXPPower] >= 1)//������ �� ���� ������
        {
            BattleResultStatus.FinalResultAmountPlus = (int)(P_Info.Experience / 100f);
        }
        else
        {
            BattleResultStatus.FinalResultAmountPlus = 0f;
        }
        //���� ��ġ -> (���� ��ġ + ���� �߰� ��ġ) * ������ ���� ���� 1 * ������ ���� ���� 2 ...... * ������ ���� ���� n + ���� �߰� ��ġ;
        float FinalMultiplyNum = 1f;
        for (int i = 0; i < BattleResultStatus.ResultMagnification.Count; i++)
        {
            FinalMultiplyNum *= BattleResultStatus.ResultMagnification[i];
        }
        BattleResultStatus.FinalResultAmount =
            (int)(((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * (FinalMultiplyNum * BattleResultStatus.BuffMagnification)) + BattleResultStatus.FinalResultAmountPlus);
        //BattleResultStatus.FinalResultAmount = (int)(((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * FinalMultiplyNum) + BattleResultStatus.FinalResultAmountPlus);
    }

    public void CalculateMonsterAction()
    {
        MonsterCurrentStatus MC_Info = CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus();
        int CurrentState = CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState;
        int MonEquipmentCode = 0;
        BattleResultStatus.BaseAmount = 0f;
        BattleResultStatus.BaseAmountPlus = 0f;
        BattleResultStatus.ResultMagnification.Clear();
        BattleResultStatus.ResultMagnification.Add(1);//Ȥ�ø� ���� ����
        BattleResultStatus.BuffMagnification = 1f;
        BattleResultStatus.FinalResultAmountPlus = 0f;
        BattleResultStatus.FinalResultAmount = 0f;

        switch (CurrentState)
        {
            case (int)EMonsterActionState.Attack:
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterWeaponCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentATK;
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.AttackDebuff] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 0.5f;
                }
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ToughFist] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 2f;
                }
                break;
            case (int)EMonsterActionState.Defense:
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterArmorCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentDUR;
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.DefenseDebuff] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 0.5f;
                }
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ToughSkin] >= 1)
                {
                    BattleResultStatus.BuffMagnification *= 2f;
                }
                break;
        }
        EquipmentSO ESO_Info = EquipmentInfoManager.Instance.GetMonEquipmentInfo(MonEquipmentCode);

        if(BattleResultStatus.BaseAmount > 0)
        {
            //���� �������� ResultMagnification ����
            BattleResultStatus.ResultMagnification.Clear();
            SetMonsterBattleStatus(MC_Info, ESO_Info);
        }
    }

    protected void SetMonsterBattleStatus(MonsterCurrentStatus MC_Info, EquipmentSO ESO_Info)
    {
        //BattleResult�� ����� ����
        for (int i = 0; i < ESO_Info.EquipmentSlots.Length; i++)
        {
            List<float> NegativeList = new List<float>();
            List<float> PositiveList = new List<float>();
            float WholeSlotStateAmount = ESO_Info.EquipmentSlots[i].SlotState.Length;
            float MultiplyNum = 60 / WholeSlotStateAmount;
            float NegativeAmount = 0f;
            float PositiveAmount = 0f;
            for (int j = 0; j < ESO_Info.EquipmentSlots[i].SlotState.Length; j++)
            {
                if (ESO_Info.EquipmentSlots[i].IsPositive[j] == true)
                {
                    PositiveAmount++;
                    PositiveList.Add(ESO_Info.EquipmentSlots[i].SlotState[j]);
                }
                else
                {
                    NegativeAmount++;
                    NegativeList.Add(ESO_Info.EquipmentSlots[i].SlotState[j]);
                }
            }

            int LuckBuffNum = 0;
            if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Luck] >= 1)
            {
                LuckBuffNum += 30;
            }
            if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Misfortune] >= 1)
            {
                LuckBuffNum -= 30;
            }
            Debug.Log("���� : ���� = 1 : 1 => ���� : 0 ~ 30" + " ���� : 30 ~ " + (60 + (int)MC_Info.MonsterCurrentLUK + LuckBuffNum));
            //Luk�� �̳� �������°� ����ó��
            int RandNum;
            if (60 + (int)MC_Info.MonsterCurrentLUK + LuckBuffNum <= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)MC_Info.MonsterCurrentLUK +LuckBuffNum);
            }

            if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //������ List���� �ϳ��� �̾Ƽ� ����
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)MC_Info.MonsterCurrentLUK + LuckBuffNum)
            {
                //������ List���� �ϳ��� �̾Ƽ� ����
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
        }

        float FinalMultiplyNum = 1f;
        for (int i = 0; i < BattleResultStatus.ResultMagnification.Count; i++)
        {
            FinalMultiplyNum *= BattleResultStatus.ResultMagnification[i];
        }
        BattleResultStatus.FinalResultAmount =
            (int)(((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * (FinalMultiplyNum * BattleResultStatus.BuffMagnification)) + BattleResultStatus.FinalResultAmountPlus);
        //BattleResultStatus.FinalResultAmount = (int)(((BattleResultStatus.BaseAmount + BattleResultStatus.BaseAmountPlus) * FinalMultiplyNum) + BattleResultStatus.FinalResultAmountPlus);
    }
}
