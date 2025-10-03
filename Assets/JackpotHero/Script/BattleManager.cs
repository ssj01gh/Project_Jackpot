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
/*
public class ActivePlayerMonsterActionGuages
{
    public float ThisTurnPlayerGuage = 0;
    public float[] ThisTurnMonsterGuages = new float[3] { 0, 0, 0 };
}
*/
public class BattleManager : MonoBehaviour
{
    protected List<GameObject> BattleTurn = new List<GameObject>();
    //protected List<ActivePlayerMonsterActionGuages> ThisTurnPlayerMosnterGuage = new List<ActivePlayerMonsterActionGuages>();
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

    protected float NextPlayerActiveGauge = 0;

    protected bool IgnoreBuffProgressAtFirstBattleProgress = false;

    private const int CurrentFinalStage = 1;
    public float CurrentSpawnPatternReward { protected set; get; }

    protected List<string> SpawnMonstersID = new List<string>();
    protected GameObject SummonerMonster = null;
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

    public void InitCurrentBattleMonsters(bool IsBossBattle = false)//�����Ҷ� �ѹ��� �����
    {
        IgnoreBuffProgressAtFirstBattleProgress = true;
        CurrentTurnObject = null;
        CurrentSpawnPatternReward = 0;
        if(IsBossBattle == false)
        {
            MonMgr.SetSpawnPattern(PlayerMgr);//������ ���� ���� ����
        }
        else
        {//���� �����̶�� �ٸ��� //���������� ���� �״ٸ� ���� ���� ������ ������ x
            MonMgr.SetBossSpawn(PlayerMgr);
        }
        
        MonMgr.SpawnCurrentSpawnPatternMonster();//���� ���Ͽ� �°� ���� ����//ActiveMonster����
        //���ʹ� �� SpawnCurrentSpawnPatternMonster���� ���� ������ �� ����
        //���� ������ ������� �ٽ� ����
        PlayerMgr.GetPlayerInfo().SetInitBuffByMonsters(MonMgr.GetActiveMonsters());
        PlayerMgr.GetPlayerInfo().SetInitBuff();
        //�÷��̾��� ���¿� �´� ������ ���� ���Ѿ���
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    public void InitMonsterNPlayerActiveGuage()//�ʱ�ȭ
    {
        BattleTurn.Clear();
        PlayerActiveGauge = 0;
        NextPlayerActiveGauge = 0;
        for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
        {
            //MonsterActiveGauge[i] = 0;
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge = 0;
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge = 0;
        }

        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack != -1)//������ ���ߴٸ�
        {
            if (MonMgr.GetActiveMonsters().Count >= 1)
            {
                //BattleTurn.Add(MonMgr.GetActiveMonsters()[0]);
                for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
                {
                    //BattleTurn.Add(MonMgr.GetActiveMonsters()[i]);
                    //������ NextGauge�� 100+SPD�� �Ҵ��Ѵ� -> ������ ���� �߿� ��������� �ൿ
                    MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge
                        = 100 + MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentSPD;
                }
            }
        }
    }

    public void ProgressBattle()//�̰� ������ ��ӵǴ� ���� ���������� �ҷ������ߵ� �Լ���
    {
        //%1000�� ������ 200�̻�, 300�̸��� ����
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 >= 200 &&
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 < 300)
        {
            SoundManager.Instance.PlayBGM("BossBattleBGM");
        }
        else
        {
            SoundManager.Instance.PlayBGM("NormalBattleBGM");
        }

        List<int> RewardEXPs = MonMgr.CheckActiveMonstersRSurvive(PlayerMgr);//���� ������ ������ ���� ���� ����//������ Reward����
        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack == -1)
        {//-1�̶� ���� ������ �ƴϴ� -> ������ ȹ�� �ؾ� �Ѵ�.
            for (int i = 0; i < RewardEXPs.Count; i++)
            {
                CurrentSpawnPatternReward += RewardEXPs[i];
            }
        }

        if(SpawnMonstersID.Count >= 1)
        {
            MonMgr.SpawnMonsterBySummonMonster(SpawnMonstersID, SummonerMonster);
            SummonerMonster = null;
        }

        //���� ��� <- �̳��� �ٵ� ���� �༮�� ����� ���� ������ ���X
        if(CurrentTurnObject != null)
        {
            BuffProgress();
        }
        //���� ���
        MonMgr.SetActiveMonstersStatus();//���� Active���ִ� ���� ���� status ���
        PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//���� ����������� ���� ����
        PlayerMgr.GetPlayerInfo().SetDefeseResilienceBuff();
        foreach (GameObject Mon in MonMgr.GetActiveMonsters())
        {
            Mon.GetComponent<Monster>().SetMonsterVariousBuff();
        }
        //�ϰ�� �ϱ����� ���� ���� �༮�� �׾��� Ȯ��. ���� �༮�� �׾����� CurrentTurnObject�� �ʱ�ȭ�Ѵ�?
        if (CurrentTurnObject != null && CurrentTurnObject.tag == "Monster")
        {
            if (CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentHP <= 0)
            {
                //���Ͱ� ������ progressBattle�� �ٽ� �����Ѵ�.
                CurrentTurnObject = null;
                ProgressBattle();
                return;
            }
        }
        SetBattleTurn();//�÷��̾�� ������ SPD���� ������ ���� Turn�� ����->�Ƹ� ���⼭ Ȯ���ҵ�?
        DecideCurrentBattleTurn();//���⼭ ���� ������ �������� ����
        UIMgr.B_UI.SetBattleTurnUI(BattleTurn);//������ Turn�� ui�� ǥ��
        MonMgr.SetCurrentTargetMonster(null);//CurrentTarget�ʱ�ȭ
        UIMgr.B_UI.UnDisplayMonsterDetailUI();//������ �� ǥ�� UnActive//�� �������ͽ��� ����� �͵�;

        /*
        if (IgnoreBuffProgressAtFirstBattleProgress == true)
        {
            MonMgr.SetActiveMonstersStatus();//���� Active���ִ� ���� ���� status ���
            PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//���� ����������� ���� ����
        }//ó����

        SetBattleTurn();//�÷��̾�� ������ SPD���� ������ ���� Turn�� ����->�Ƹ� ���⼭ Ȯ���ҵ�?
        DecideCurrentBattleTurn();//���⼭ ���� ������ �������� ����
        UIMgr.B_UI.SetBattleTurnUI(BattleTurn);//������ Turn�� ui�� ǥ��
        MonMgr.SetCurrentTargetMonster(null);//CurrentTarget�ʱ�ȭ
        UIMgr.B_UI.UnDisplayMonsterDetailUI();//������ �� ǥ�� UnActive//�� �������ͽ��� ����� �͵�;

        //���⿡�� Ư�� ������ ����������� ����ҵ�(����ִ� ���� �ڽ��� �Ͽ�)
        if (IgnoreBuffProgressAtFirstBattleProgress == true)
        {
            IgnoreBuffProgressAtFirstBattleProgress = false;
            ActiveBuffEffectAtFirstTime();
        }
        else
        {
            BuffProgress();//�̰� ó���� �����ϰ��ؾ���//���⼭ ����Ʈ �߻���.....
                           //hit ����Ʈ�� ����� BattleUI���� ������ ������ �߻� �ؾ� �ҵ�?
            MonMgr.SetActiveMonstersStatus();//���� Active���ִ� ���� ���� status ���
            PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//���� ����������� ���� ����
            //ó�� ���ķδ� �Ĺݿ�
        }
        */


        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(), 
            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);//�÷��̾� ������ ����
        UIMgr.B_UI.SetPlayerBattleUI(PlayerMgr.GetPlayerInfo());
        UIMgr.B_UI.SetMonsterBattleUI(MonMgr.GetActiveMonsters());
        //UIMgr.B_UI.ActivePlayerShieldNBuffUI(PlayerMgr.GetPlayerInfo());//�̰� ���� ���Ͱ� �׾����� �ƴ��� Ȯ���ؾߵ�

        if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP <= 0)
        {
            //��������Ʈ�� �������� ��������Ʈ�� ����ȴٴ���?
            //ActiveMonster���δ� ����
            MonMgr.InActiveAllActiveMonster();
            //UIMgr.B_UI.ActivePlayerShieldNBuffUI(PlayerMgr.GetPlayerInfo());//�����ִ� Set~~�� �� ���ֱ� ����
            PlayerMgr.GetPlayerInfo().CalculateEarlyPoint();
            PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Defeat);
            UIMgr.B_UI.DefeatBattle(PlayerMgr.GetPlayerInfo());
            UIMgr.PlayerDefeat();//�÷��̾��� ���â, ����â, ���� �������� ����â �����͵�
            //�ٸ� �÷��̾� UI�� ������
            return;
        }
        if (MonMgr.GetActiveMonsters().Count <= 0)
        {
            Debug.Log("Winner");
            PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle);
            //������ ��밡 ������°��� Ȯ���� ����� �ִ°�? ���� ����//���� 200�̻� 300�̸�
            if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 >= 200 &&
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 < 300)
            {
                PlayerMgr.GetPlayerInfo().EndOfAction(true);//���⼭ ������ �� 0�̵�
                PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//���� ����������� ���� ����
                UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);//�÷��̾� ������ ����
                int RewardEXP = PlayerMgr.GetPlayerInfo().ReturnEXPByEXPMagnification((int)CurrentSpawnPatternReward);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RewardEXP, true);
                UIMgr.B_UI.VictoryBattle(RewardEXP);
            }
            else//������ �ƴ϶�� ����ϰ�
            {
                PlayerMgr.GetPlayerInfo().EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ//���⼭ ������ �� 0�̵�
                PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//���� ����������� ���� ����//VictoryButton
                UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);//������ �������Ϳ� ���� ���� ����
                                                                   //�޽��߿� ������ �����Ŷ�� �ٽ� �޽� �ൿ �������� ���ư�
                int RewardEXP = PlayerMgr.GetPlayerInfo().ReturnEXPByEXPMagnification((int)CurrentSpawnPatternReward);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RewardEXP, true);
                UIMgr.B_UI.VictoryBattle(RewardEXP);
            }
            return;
            //�� ������ ������ ������ ������.
        }

        if (CurrentState == (int)EBattleStates.PlayerTurn)
        {
            //�÷��̾��� ���̶�� �÷��̾��� �ൿ�� �����ϴ� ��ư�� ��Ÿ���� �Ѵ�.
            UIMgr.B_UI.ActiveBattleSelectionUI(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] >= 1,
                PlayerMgr.GetPlayerInfo().AttackAverageIncrease, PlayerMgr.GetPlayerInfo().DefenseAverageIncrease, PlayerMgr.GetPlayerInfo().RestAverageIncrease);//�ൿ ���� ��ư�� ��Ÿ��

            //�÷��̾��� ���� ������ ����ִ� ���͵��� ���� Ÿ�� �ʱ�ȭ
            MonMgr.SetActiveMonsterChainAttack(false, false);
        }
        else if (CurrentState == (int)EBattleStates.MonsterTurn)
        {
            //������ ���̶�� �ൿ ��ư�� ��Ÿ��
            UIMgr.B_UI.ActiveBattleSelectionUI_Mon();

            //������ ���� ������ �÷��̾��� ���� Ÿ�� �ʱ�ȭ
            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(false, false);
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
        SoundManager.Instance.PlayUISFX("UI_Button");
        SpawnMonstersID.Clear();
        bool IsTargetHasShield = false;
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

        //��Ȥ�� �ִٸ�//Ȯ���� �ɷȴٸ�//ActionButtonType�� Charm���� �ٲ�� �ҵ�?
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charm] >= 1)
        {
            //1~11���� ������
            int GetCharm = PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charm];
            int RandCharmNum = Random.Range(1, 11);
            if(GetCharm >= RandCharmNum)
            {
                ActionButtonType = "Charm";
            }
        }

        SetPlayerBattleStatus(ActionButtonType);
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charging] >= 1)
            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charging] = 0;

        if (ActionButtonType == "Attack")//�����̶�� BattleMgr���� ���� ����� ���� Ÿ���� ü���� ����
        {
            TargetObject = MonMgr.CheckActiveMonsterHaveProvocation().gameObject;
            //�����߿� ������ ������ �ִ� �༮�� �ִٸ� �׳༮���� CurrentTarget�� �ٲ����.

            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(true, true);
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] >= 1)
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower]--;

            //��ġ�� �� ���� ������
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.OverWhelmingPower] >= 1)
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
            if (MonMgr.CurrentTarget.MonsterBuff.BuffList[(int)EBuffType.ThornArmor] >= 1)
            {//���� �ִ� �������� ���� ��ȣ���� �� ������ => ���� ������ < ���� ��ȣ��
                if(BattleResultStatus.FinalResultAmount < MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint)
                {
                    float ReflectionDamage =
                        (MonMgr.CurrentTarget.GetMonsterCurrentStatus().MonsterCurrentShieldPoint + MonMgr.CurrentTarget.MonsterBuff.BuffList[(int)EBuffType.Defense])
                        - BattleResultStatus.FinalResultAmount;

                    PlayerMgr.GetPlayerInfo().PlayerDamage((int)ReflectionDamage);
                }
            }
            //���뺸����
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Exploitation] >= 1)
            {
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount((int)(BattleResultStatus.FinalResultAmount / 5));
            }
            MonMgr.CurrentTarget.MonsterDamage(BattleResultStatus.FinalResultAmount);
            if(MonMgr.CurrentTarget.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint >= 1)
            {//���尡 ����������
                IsTargetHasShield = true;
            }
        }
        else if (ActionButtonType == "Charm")
        {
            TargetObject = PlayerMgr.GetPlayerInfo().gameObject;
            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(true, true);
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] >= 1)
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower]--;

            PlayerMgr.GetPlayerInfo().PlayerDamage(BattleResultStatus.FinalResultAmount);
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount >= 1)
            {//���尡 ����������
                IsTargetHasShield = true;
            }
        }
        else if (ActionButtonType == "Defense")//����� ����� �÷��̾��� ��� ��ġ�� ����
        {
            TargetObject = null;

            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(true, false);
            PlayerMgr.GetPlayerInfo().PlayerGetShield(BattleResultStatus.FinalResultAmount);
        }
        else if (ActionButtonType == "Rest")//�޽��̶�� ����� �÷��̾��� �Ƿε��� ȸ����
        {
            TargetObject = null;

            MonMgr.SetAcitveMonsterMountainLord();

            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] >= 1)
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear]--;

            PlayerMgr.GetPlayerInfo().SetChainAttackBuff(true, false);
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
        //���⼭�� ������ü�� �÷��̾�
        UIMgr.B_UI.ActiveMainBattleUI(PlayerMgr.GetPlayerInfo().gameObject, MonMgr.CurrentTarget, ActionButtonType, BattleResultStatus, 
            PlayerMgr.GetPlayerInfo().gameObject.transform.position, IsTargetHasShield, ProgressBattle);
    }

    public void MonsterBattleActionSelectionButtonClick()
    {
        if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charm] >= 1)
        {
            int GetCharm = CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charm];
            int RandCharmNum = Random.Range(1, 11);
            if(GetCharm >= RandCharmNum)
            {
                CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState = (int)EMonsterActionState.Charm;
            }
        }

        CalculateMonsterAction();
        SoundManager.Instance.PlayUISFX("UI_Button");
        string CurrentBattleState;
        bool IsTargetHasShield = false;
        bool IsMonsterAttack = false;
        bool IsAlreadyDamageCalculate = false;
        SpawnMonstersID.Clear();
        TargetObject = null;
        switch (CurrentTurnObject.GetComponent<Monster>().MonsterCurrentState)
        {
            case (int)EMonsterActionState.Attack:
                //�������� �÷��̾ �������ִ� ���庸�� �۰ų� ���ٸ�
                CurrentBattleState = "Attack";
                TargetObject = PlayerMgr.GetPlayerInfo().gameObject;
                IsMonsterAttack = true;

                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] >= 1)
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] = 0;

                //�÷��̾ ���� ������ ������ �������
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ThornArmor] >= 1)
                {//������ < �÷��̾� ��ȣ�� �ϰ��
                    if (BattleResultStatus.FinalResultAmount < PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount)
                    {
                        float ReflectionDamage = (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount + PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Defense])
                            - BattleResultStatus.FinalResultAmount;
                        CurrentTurnObject.GetComponent<Monster>().MonsterDamage(ReflectionDamage);
                    }
                }
                //��Ż�� ������ ������
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Plunder] >= 1)
                {
                    int CurrentPlayerExperience = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience;
                    if (CurrentPlayerExperience >= BattleResultStatus.FinalResultAmount)//����ġ�� ��� ��
                    {
                        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-(int)BattleResultStatus.FinalResultAmount, true);
                        CurrentTurnObject.GetComponent<Monster>().AdditionalEXP += (int)(BattleResultStatus.FinalResultAmount * 0.7);
                    }
                    else//�ƴ϶�� ����ġ�� ��� ���� �� ü���� ����
                    {
                        CurrentTurnObject.GetComponent<Monster>().AdditionalEXP += (int)(CurrentPlayerExperience * 0.7);
                        float RemainDamange = BattleResultStatus.FinalResultAmount - CurrentPlayerExperience;
                        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-CurrentPlayerExperience, true);
                        PlayerMgr.GetPlayerInfo().PlayerDamage(RemainDamange);
                        IsAlreadyDamageCalculate = true;
                    }
                }

                if(IsAlreadyDamageCalculate == false)
                {
                    PlayerMgr.GetPlayerInfo().PlayerDamage((int)BattleResultStatus.FinalResultAmount);
                }
                if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ShieldAmount >= 1)
                {//���⿡ ������ ����_�� ����
                    //�ӽ÷� ª�ٸ���
                    /*
                    if(CurrentTurnObject.GetComponent<Monster>().MonsterName == "ShortLegBird")
                    {
                        CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.StrengthAdaptation] += 1;
                    }
                    */
                    IsTargetHasShield = true;
                }
                break;
            case (int)EMonsterActionState.Charm:
                CurrentBattleState = "Charm";
                TargetObject = CurrentTurnObject;
                IsMonsterAttack = true;

                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] >= 1)
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] = 0;

                CurrentTurnObject.GetComponent<Monster>().MonsterDamage((int)BattleResultStatus.FinalResultAmount);
                if(CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint >= 1)
                {
                    IsTargetHasShield = true;
                }
                break;
            case (int)EMonsterActionState.Defense:
                CurrentBattleState = "Defense";

                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] >= 1)
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Charging] = 0;

                CurrentTurnObject.GetComponent<Monster>().MonsterGetShield(BattleResultStatus.FinalResultAmount);
                break;
            case (int)EMonsterActionState.SpawnMonster:
                SpawnMonstersID = CurrentTurnObject.GetComponent<Monster>().GetSummonMonsters();
                SummonerMonster = CurrentTurnObject;
                //���⼭ ���� �Ͽ� � ���͸� �������� �����ϸ� �ɵ�?
                CurrentBattleState = "Another";
                break;
            case (int)EMonsterActionState.ApplyLuck:
                CurrentBattleState = "Luck";
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.Luck);
                break;
            case (int)EMonsterActionState.GivePoison:
                CurrentBattleState = "Poison";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Poison, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Poison));
                break;
            case (int)EMonsterActionState.GiveMisFortune:
                CurrentBattleState = "MisFortune";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Misfortune, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Misfortune));
                break;
            case (int)EMonsterActionState.GiveCurseOfDeath:
                CurrentBattleState = "CurseOfDeath";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.CurseOfDeath, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.CurseOfDeath));
                break;
            case (int)EMonsterActionState.ApplyThornArmor:
                CurrentBattleState = "ThornArmor";
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.ThornArmor);
                break;
            case (int)EMonsterActionState.GiveCower:
                CurrentBattleState = "Cower";
                PlayerMgr.GetPlayerInfo().ApplyBuff((int)EBuffType.Cower, CurrentTurnObject.GetComponent<Monster>().MonsterGiveBuff((int)EBuffType.Cower));
                break;
            case (int)EMonsterActionState.ApplyCopyStrength:
                //������ȭ + ���������� ���� ��ȭ
                CurrentBattleState = "CopyStrength";
                int STRPlusAmount = (int)(JsonReadWriteManager.Instance.GetEarlyState("STR") + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().StrengthLevel);
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.CopyStrength, STRPlusAmount);
                break;
            case (int)EMonsterActionState.ApplyCopyDurability:
                CurrentBattleState = "CopyDurability";
                int DURPlusAmount = (int)(JsonReadWriteManager.Instance.GetEarlyState("DUR") + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DurabilityLevel);
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.CopyDurability, DURPlusAmount);
                break;
            case (int)EMonsterActionState.ApplyCopySpeed:
                CurrentBattleState = "CopySpeed";
                int SPDPlusAmount = (int)(JsonReadWriteManager.Instance.GetEarlyState("SPD") + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SpeedLevel);
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.CopySpeed, SPDPlusAmount);
                break;
            case (int)EMonsterActionState.ApplyCopyLuck:
                CurrentBattleState = "CopyLuck";
                int LUKPlusAmount = (int)(JsonReadWriteManager.Instance.GetEarlyState("LUK") + PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().LuckLevel);
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.CopyLuck, LUKPlusAmount);
                break;
            case (int)EMonsterActionState.ApplyGreed:
                CurrentBattleState = "Greed";
                int GreedStackAmount = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience;
                CurrentTurnObject.GetComponent<Monster>().MonsterGetBuff((int)EBuffType.Greed, GreedStackAmount);
                break;
            case (int)EMonsterActionState.GiveEnvy:
                CurrentBattleState = "Envy";
                int EnvyStackAmount = (int)PlayerMgr.GetPlayerInfo().GetAllEquipTier();
                PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Envy] = EnvyStackAmount;
                break;
            case (int)EMonsterActionState.ConsumeGluttony:
                //Stack�� �ڱ� �ڽ��� �ִ� ü�º��� �������� ������
                int GluttonyMonMaxHP = (int)(CurrentTurnObject.GetComponent<Monster>().GetMonsterCurrentStatus().MonsterMaxHP);
                int GluttonyStack = CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Gluttony];
                if (GluttonyMonMaxHP >= GluttonyStack)
                {//ü���� �� ���ų� ������ -> ��ȭ ���� -> ���� ��ȯ
                    CurrentBattleState = "SurvantByGluttony";
                    int GluttonySummonMonHP = (int)(GluttonyStack * 0.2f);
                    int GluttonySummonMonStatus = (int)(GluttonyStack * 0.03f);

                    SpawnMonstersID = CurrentTurnObject.GetComponent<Monster>().
                        GetSummonMonsters(GluttonySummonMonHP, GluttonySummonMonStatus, GluttonySummonMonStatus, GluttonySummonMonStatus, GluttonySummonMonStatus);
                    SummonerMonster = CurrentTurnObject;
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Gluttony] = 0;
                }
                else
                {//ü���� �� ������ -> ��ȭ �Ұ��� -> ������
                    CurrentBattleState = "CantConsume";
                    CurrentTurnObject.GetComponent<Monster>().MonsterDamage(GluttonyStack / 2, true);
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Gluttony] = 0;
                }
                    //Stack�� �ڱ� �ڽ��� �ִ� ü�º��� �������� ���� ��ȯ
                break;
            default:
                CurrentBattleState = "Another";
                break;
        }

        MonMgr.SetActiveMonsterChainAttack(true, IsMonsterAttack, CurrentTurnObject.GetComponent<Monster>());

        CurrentTurnObject.GetComponent<Monster>().CheckEnemyBuff(PlayerMgr.GetPlayerInfo().PlayerBuff);
        CurrentTurnObject.GetComponent<Monster>().SetNextMonsterState();//������ �ൿ ���Ͽ� ���� ���� �ൿ ����
        UIMgr.EDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.MEDI_UI.InActiveEquipmentDetailInfoUI();
        //���⼱ ������ü�� ����
        UIMgr.B_UI.ActiveMainBattleUI(CurrentTurnObject.GetComponent<Monster>().gameObject, CurrentTurnObject.GetComponent<Monster>(), CurrentBattleState, BattleResultStatus,
            PlayerMgr.GetPlayerInfo().gameObject.transform.position, IsTargetHasShield, ProgressBattle);
    }
    public void PressVictoryButton()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 >= 200 &&
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails % 1000 < 300)//�ƴҶ��� �����϶�����
        {//���⼭ Ŭ�� ������ �����϶� ���� ������ �־�� �ҵ�?
            Debug.Log("Boss");
            //CurrentPlayerActionDetail�� �����ϴ°� ������ 0���� �ٲٱ�//CurrentFloor�� �ø���
            if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails / 1000 >= CurrentFinalStage)
            {//���� ����� �� ���� ũ�ų� ������ ���� �¸���
                UIMgr.B_UI.ClickVictoryButton();
                PlayerMgr.GetPlayerInfo().CalculateEarlyPoint();
                UIMgr.B_UI.WinGame(PlayerMgr.GetPlayerInfo());
            }
            else
            {
                PlayerMgr.GetPlayerInfo().WinBossBattle();
                UIMgr.B_UI.ClickVictoryButton();//�¸���ư �������� UI����
                UIMgr.BossBattleWinFade();//Fade�ؾ���//���⿡ SetUI�� �̰����� ����
                JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            }
        }
        else
        {
            UIMgr.B_UI.ClickVictoryButton();
            UIMgr.GI_UI.ActiveGettingUI();
            UIMgr.SetUI();
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        }
    }

    public void PressDefeatButton()//�̰������� �Ȱ����ϳ�
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        //���⼭ �ʱ� ��ȭ ����Ʈ�� �󸶳� ���� ����ؾ� �Ǵ°� �ƴϿ�?
        if (JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 2)
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
    protected void ActiveBuffEffectAtFirstTime()
    {
        if (CurrentTurnObject.tag == "Player")
        {
            if(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Petrification] > 0)
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Frost", PlayerMgr.GetPlayerInfo().gameObject.transform.position);
            }
            if(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] > 0)
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Fear", PlayerMgr.GetPlayerInfo().gameObject.transform.position);
            }
        }
        /*//���ʹ� ���� �ɸ��� �ʴ´�? -> �� �����ϴ� ��Ŀ������ �ٲ�� ������ �ɸ���, �׸��� ������ ȿ���� ���Ѵٴ���?
        else if(CurrentTurnObject.tag == "Monster")
        {
            Monster MonInfo = CurrentTurnObject.GetComponent<Monster>();
            if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.FrostBite] > 0)
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Frost", MonInfo.gameObject.transform.position);
            }
            if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.Fear] > 0)
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Fear", MonInfo.gameObject.transform.position);
            }
        }
        */
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
        if (CurrentTurnObject.tag == "Player" && MonMgr.GetActiveMonsters().Count > 0)
        {
            Vector3 PlayerBuffPos = PlayerMgr.GetPlayerInfo().gameObject.transform.position;
            PlayerBuffPos.y += 1.5f;
            if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[BuffsType] > 0)
            {
                switch (BuffsType)//1�� �پ���� �ʰų� �������� �ְų� ȸ����Ű�°͸�
                {
                    //��� , ������, ȭ��, ��, ������ ����, ���������, ������, �һ�(�̳��� �������� ���Ǿ� �ɰ� ������)
                    case (int)EBuffType.Resilience:
                        //PlayerRegenSTA
                        PlayerMgr.GetPlayerInfo().PlayerRegenSTA(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Resilience]);
                        break;
                    case (int)EBuffType.Burn:
                        float BurnDamage = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP / 20;
                        PlayerMgr.GetPlayerInfo().PlayerDamage((int)BurnDamage, true);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Burn", PlayerBuffPos);
                        break;
                    case (int)EBuffType.Poison:
                        PlayerMgr.GetPlayerInfo().PlayerDamage(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison], true);
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison] = PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Poison] / 2;
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Poison", PlayerBuffPos);
                        break;
                    case (int)EBuffType.CurseOfDeath:
                        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath] <= 1)//1���� ������ = ���� 0�̵ȴ� = �������� �Դ´�.
                        {//������� ���� CurseOfDeath�� ������ 0�ʰ� 1���� �� 1��
                            float CurseOfDeathDamage = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP * 9 / 10;
                            PlayerMgr.GetPlayerInfo().PlayerDamage((int)CurseOfDeathDamage, true);
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath] = 0;
                            EffectManager.Instance.ActiveEffect("BattleEffect_Buff_CurseOfDeath", PlayerBuffPos);
                        }
                        else
                        {//0�ʰ��̱⸸ �Ҷ��� --
                            SoundManager.Instance.PlaySFX("Buff_Consume");
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.CurseOfDeath]--;
                        }
                        break;
                    case (int)EBuffType.RegenArmor:
                        PlayerMgr.GetPlayerInfo().PlayerGetShield(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.RegenArmor]);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_RegenArmor", PlayerBuffPos);
                        break;
                    case (int)EBuffType.Weakness:
                        float WeaknessSpendSTA = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA / 20;
                        PlayerMgr.GetPlayerInfo().PlayerSpendSTA(WeaknessSpendSTA);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Weakness", PlayerBuffPos);
                        break;
                    case (int)EBuffType.Regeneration:
                        float RegenHPAmount = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP - PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP;
                        RegenHPAmount = RegenHPAmount / 20;
                        PlayerMgr.GetPlayerInfo().PlayerRegenHp((int)RegenHPAmount);
                        //BattleEffect_Buff_Regeneration
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Regeneration", PlayerBuffPos);
                        break;
                    case (int)EBuffType.Recharge:
                        float RegenSTAAmount = PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxSTA - PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentSTA;
                        RegenSTAAmount = RegenSTAAmount / 20;
                        PlayerMgr.GetPlayerInfo().PlayerRegenSTA(RegenSTAAmount);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_ReCharge", PlayerBuffPos);
                        break;
                    case (int)EBuffType.UnDead:
                        if (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP < 1)
                        {
                            PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP = 1;
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentHpRatio =
                                PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP / PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP;
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnDead] = 0;
                            EffectManager.Instance.ActiveEffect("BattleEffect_Buff_UnDead", PlayerBuffPos);
                        }
                        else
                        {//���� ������ �ƴ϶�� --
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.UnDead]--;
                        }
                        break;
                    case (int)EBuffType.Cower:
                        while(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] >= 3)
                        {
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] -= 3;
                            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Fear] += 1;
                        }
                        float DebuffSpendSTA = PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Cower] * 20;
                        PlayerMgr.GetPlayerInfo().PlayerSpendSTA(DebuffSpendSTA);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Cower", PlayerBuffPos);
                        //BattleEffect_Buff_Cower
                        //durl
                        break;
                    case (int)EBuffType.Fear:
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Fear", PlayerBuffPos);
                        break;
                    case (int)EBuffType.OverCharge:
                        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.OverCharge] <= 1)
                        {
                            PlayerMgr.GetPlayerInfo().PlayerDamage(99999);
                        }
                        break;
                }

                //��, ������ ����, �һ�, �����, ����, ȸ����, ���� Ÿ��, ����, ����, ���� ����
                //����_��, ����_��, �ݻ�, ���� ����
                if (BuffsType != (int)EBuffType.Poison && BuffsType != (int)EBuffType.CurseOfDeath &&
                    BuffsType != (int)EBuffType.UnDead && BuffsType != (int)EBuffType.Defenseless && 
                    BuffsType != (int)EBuffType.Defense && BuffsType != (int)EBuffType.Resilience &&
                    BuffsType != (int)EBuffType.ChainAttack && BuffsType != (int)EBuffType.Cower &&
                    BuffsType != (int)EBuffType.Fear && BuffsType != (int)EBuffType.Charging &&
                    BuffsType != (int)EBuffType.GoodKarma && BuffsType != (int)EBuffType.BadKarma &&
                    BuffsType != (int)EBuffType.Reflect && BuffsType != (int)EBuffType.Envy)//�� �� ������ ����, �һ�, �����, ����, ȸ���� ���� ���
                {
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[BuffsType]--;
                }
            }
        }
        else if (CurrentTurnObject.tag == "Monster")
        {
            Monster MonInfo = CurrentTurnObject.GetComponent<Monster>();
            if (MonInfo.MonsterBuff.BuffList[BuffsType] > 0)
            {
                switch (BuffsType)//1�� �پ���� �ʰų� �������� �ְų� ȸ����Ű�°͸�
                {
                    //��� , ������, ȭ��, ��, ������ ����, ���������, ������, �һ�(�̳��� �������� ���Ǿ� �ɰ� ������)
                    case (int)EBuffType.Burn:
                        float BurnDamage = MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP / 20;
                        MonInfo.MonsterDamage((int)BurnDamage);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Burn", MonInfo.gameObject.transform.position);
                        break;
                    case (int)EBuffType.Poison:
                        MonInfo.MonsterDamage(MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison]);
                        MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison] = MonInfo.MonsterBuff.BuffList[(int)EBuffType.Poison] / 2;
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Poison", MonInfo.gameObject.transform.position);
                        break;
                    case (int)EBuffType.CurseOfDeath:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath] <= 1)
                        {
                            float CurseOfDeathDamage = MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP * 9 / 10;
                            MonInfo.MonsterDamage((int)CurseOfDeathDamage);
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath] = 0;
                            EffectManager.Instance.ActiveEffect("BattleEffect_Buff_CurseOfDeath", MonInfo.gameObject.transform.position);
                        }
                        else
                        {
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath]--;
                        }
                        break;
                    case (int)EBuffType.RegenArmor:
                        MonInfo.MonsterGetShield(MonInfo.MonsterBuff.BuffList[(int)EBuffType.RegenArmor]);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_RegenArmor", MonInfo.gameObject.transform.position);
                        break;
                    case (int)EBuffType.Regeneration:
                        float RegenHPAmount = MonInfo.GetMonsterCurrentStatus().MonsterMaxHP - MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP;
                        RegenHPAmount = RegenHPAmount / 20;
                        MonInfo.MonsterRegenHP(RegenHPAmount);
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_Regeneration", MonInfo.gameObject.transform.position);
                        break;
                    case (int)EBuffType.UnDead:
                        if (MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP < 1)
                        {
                            MonInfo.GetMonsterCurrentStatus().MonsterCurrentHP = 1;
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.UnDead] = 0;
                            EffectManager.Instance.ActiveEffect("BattleEffect_Buff_UnDead", MonInfo.gameObject.transform.position);
                        }
                        else
                        {
                            MonInfo.MonsterBuff.BuffList[(int)EBuffType.UnDead]--;
                        }
                        break;
                    case (int)EBuffType.Survant:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.Survant] <= 1)
                        {
                            MonInfo.MonsterDamage(99999);
                        }
                        break;
                    case (int)EBuffType.SelfDestruct:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.SelfDestruct] <= 1)
                        {
                            MonInfo.MonsterDamage(99999);
                            PlayerMgr.GetPlayerInfo().PlayerDamage(50);
                        }
                        break;
                    case (int)EBuffType.OverCharge:
                        if (MonInfo.MonsterBuff.BuffList[(int)EBuffType.OverCharge] <= 1)
                        {
                            MonInfo.MonsterDamage(99999);
                        }
                        break;
                    case (int)EBuffType.Lust:
                        PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Charm] += 2;
                        break;
                }

                //��, ������ ����, �һ�, ����, ����Ÿ��, �걺, ����_��, ����_����, ����_�ӵ�, ����_���, ��� ����
                //����_��, ����_����, ����_�ӵ�, ����, �ݻ� ����
                //����, Ž��, ����, ����, ��Ž, �г� ����
                if (BuffsType != (int)EBuffType.Poison && BuffsType != (int)EBuffType.CurseOfDeath &&
                    BuffsType != (int)EBuffType.UnDead && BuffsType != (int)EBuffType.Defense &&
                    BuffsType != (int)EBuffType.ChainAttack && BuffsType != (int)EBuffType.MountainLord &&
                    BuffsType != (int)EBuffType.CopyStrength && BuffsType != (int)EBuffType.CopyDurability &&
                    BuffsType != (int)EBuffType.CopySpeed && BuffsType != (int)EBuffType.CopyLuck &&
                    BuffsType != (int)EBuffType.Consume && BuffsType != (int)EBuffType.StrengthAdaptation &&
                    BuffsType != (int)EBuffType.DurabilityAdaptation && BuffsType != (int)EBuffType.SpeedAdaptation &&
                    BuffsType != (int)EBuffType.Charging && BuffsType != (int)EBuffType.Reflect &&
                    BuffsType != (int)EBuffType.Pride && BuffsType != (int)EBuffType.Greed &&
                    BuffsType != (int)EBuffType.Envy && BuffsType != (int)EBuffType.Lust &&
                    BuffsType != (int)EBuffType.Gluttony && BuffsType != (int)EBuffType.Wrath)//�� �� ������ ����, �һ�, ������ ���� ���
                {
                    MonInfo.MonsterBuff.BuffList[BuffsType]--;
                }
            }
        }
    }
    public void SetBattleTurn()
    {
        //���⿡�� ù��° �� �������� �������� ��ƾ���.
        /*
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
        */
        //�������� �������� ù��°�� ��´�.//Current Gauge�� nextGauge�� ���� �ִ´�.
        //BattleTurn�� Count�� 2�� �ɶ� NextGauge�� CurrentGuage�� �����Ѵ�.
        BattleTurn.Clear();
        List<int> ChargedObject = new List<int>();

        PlayerActiveGauge = NextPlayerActiveGauge;
        if (PlayerActiveGauge >= 100)
        {
            ChargedObject.Add(-1);
        }
        for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
        {
            MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge =
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge;
            if (MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge >= 100)
            {
                ChargedObject.Add(i);
            }
        }
        //�ʱ� ���
        while (ChargedObject.Count >= 1)
        {
            //MostFastMonster�� Active������ Index��
            int MostFastObjectIndex = 0;
            for(int i = 1; i < ChargedObject.Count; i++)
            {
                if (ChargedObject[MostFastObjectIndex] == -1)
                {//���� �켱 ������ ���� ������Ʈ�� �÷��̾� �϶�
                    if (PlayerActiveGauge <=
                        MonMgr.GetActiveMonsters()[ChargedObject[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                    {//���Ͱ� �÷��̾� ���� �켱�̶�� MostFastObjectIndex�� ������
                        MostFastObjectIndex = i;
                    }
                }
                else
                {//���� �켱������ ���� ������Ʈ�� �÷��̾ �ƴҶ�
                    if (MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge <=
                        MonMgr.GetActiveMonsters()[ChargedObject[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                    {
                        MostFastObjectIndex = i;
                    }
                }
            }
            //������� ���� ó������ ������ �� �˻� �Ѱ��� //���� Gauge�� ���� ū ���� ������ ��
            if(ChargedObject[MostFastObjectIndex] == -1)
            {//Gauge�� ���� ū���� �÷��̾� �϶�
                PlayerActiveGauge -= 100;
                BattleTurn.Add(PlayerMgr.GetPlayerInfo().gameObject);
                RecordNextActiveGauge();
            }
            else
            {//���� ū���� �÷��̾ �ƴҶ�
                MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge -= 100;
                BattleTurn.Add(MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]]);
                RecordNextActiveGauge();
            }
            ChargedObject.RemoveAt(MostFastObjectIndex);//MostFastMonster�� ��ġ�� ���� RemoveAt�� �ƴϿ��� ���� ����
        }
        //������ ä���
        while(BattleTurn.Count < 6 )//TurnUI�� 6ĭ�̿��� Count = 6������ ������ �ɵ���
        {//6���� ������ While�� �۵���( 6�� �Ǹ� ���)
            //Speed���� ���� ActionGuage ���� �� �������� ���� 100�� �Ѿ ������Ʈ�� ����
            ChargedObject.Clear();
            PlayerActiveGauge += PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().TotalSPD;
            if (PlayerActiveGauge >= 100)
            {
                ChargedObject.Add(-1);
            }
            for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge +=
                    MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentSPD;
                if (MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge >= 100)
                {
                    ChargedObject.Add(i);
                }
            }
            //�� ���
            while (ChargedObject.Count >= 1)
            {
                //MostFastMonster�� Active������ Index��
                int MostFastObjectIndex = 0;
                for (int i = 1; i < ChargedObject.Count; i++)
                {
                    if (ChargedObject[MostFastObjectIndex] == -1)
                    {//���� �켱 ������ ���� ������Ʈ�� �÷��̾� �϶�
                        if (PlayerActiveGauge <=
                            MonMgr.GetActiveMonsters()[ChargedObject[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                        {//���Ͱ� �÷��̾� ���� �켱�̶�� MostFastObjectIndex�� ������
                            MostFastObjectIndex = i;
                        }
                    }
                    else
                    {//���� �켱������ ���� ������Ʈ�� �÷��̾ �ƴҶ�
                        if (MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge <=
                            MonMgr.GetActiveMonsters()[ChargedObject[i]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge)
                        {
                            MostFastObjectIndex = i;
                        }
                    }
                }
                //������� ���� ó������ ������ �� �˻� �Ѱ��� //���� Gauge�� ���� ū ���� ������ ��
                if (ChargedObject[MostFastObjectIndex] == -1)
                {//Gauge�� ���� ū���� �÷��̾� �϶�
                    PlayerActiveGauge -= 100;
                    BattleTurn.Add(PlayerMgr.GetPlayerInfo().gameObject);
                    RecordNextActiveGauge();
                }
                else
                {//���� ū���� �÷��̾ �ƴҶ�
                    MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge -= 100;
                    BattleTurn.Add(MonMgr.GetActiveMonsters()[ChargedObject[MostFastObjectIndex]]);
                    RecordNextActiveGauge();
                }
                ChargedObject.RemoveAt(MostFastObjectIndex);//MostFastMonster�� ��ġ�� ���� RemoveAt�� �ƴϿ��� ���� ����
            }
        }

        CurrentState = (int)EBattleStates.Idle;
    }

    protected void RecordNextActiveGauge()
    {
        if(BattleTurn.Count == 2)//->0,1�� ���� �Ѵٴ� ����
        {//BattleTurn 1�� �ش��ϴ� ���� �����Ŀ� ���� �°� ���� NextActionGauge�� 100�� ���� �ش�.
            NextPlayerActiveGauge = PlayerActiveGauge;
            //Debug.Log(NextPlayerActiveGauge);
            for (int i = 0; i < MonMgr.GetActiveMonsters().Count; i++)
            {
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge =
                MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentActionGauge;
                //Debug.Log(MonMgr.GetActiveMonsters()[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge);
            }

            if (BattleTurn[1].tag == "Player")
            {
                NextPlayerActiveGauge += 100;
            }
            else if (BattleTurn[1].tag == "Monster")
            {
                BattleTurn[1].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterNextActionGauge += 100;
            }
        }
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

            if(CurrentTurnObject != null)
            {
                if(CurrentTurnObject.tag == "Player" && CurrentTurnObject == BattleTurn[0])
                {//������ ������Ʈ�� �÷��̾��̰� ������ ������Ʈ�� �̹��Ͽ��� �ൿ�Ҷ�
                    foreach (GameObject Mon in MonMgr.GetActiveMonsters())
                    {
                        /*
                        if(Mon.GetComponent<Monster>().MonsterName == "ShortLegBird")
                        {
                            Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.SpeedAdaptation] += 1;
                        }
                        */
                    }
                }
            }
            //�� �� ������ CurrentTurnObject�� ������ ������Ʈ��
            //����ִ� ������ ������ ������ �ִ� ���Ͱ� �ִٸ�
            foreach(GameObject Mon in MonMgr.GetActiveMonsters())
            {
                if (Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Pride] >= 1)
                {
                    int PercentOfPride = Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Pride];
                    int RandPrideNum = Random.Range(1, 101);
                    if(PercentOfPride >= RandPrideNum)
                    {//���� ������ �ش� ������ ���� ��
                        BattleTurn[0] = Mon;
                    }
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
        BattleResultStatus.BaseAmountPlus = 0;
        switch (ActionButtonType)
        {
            case "Charm":
            case "Attack":
                EquipCode = P_Info.EquipWeaponCode;
                BattleResultStatus.BaseAmount = TP_Info.TotalSTR;
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ChainAttack] >= 1)
                {
                    BattleResultStatus.BaseAmountPlus += PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.ChainAttack];
                }
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
            BattleResultStatus.BaseAmountPlus += AllEquipTier;
        }
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.BloodFamiliy] >= 1)
        {
            int TenPercentHP = (int)(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP / 20);
            BattleResultStatus.BaseAmountPlus += TenPercentHP;
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
            //Debug.Log("���� : ���� = 1 : 2 => ���� : 0 ~ 20"  + " ���� : 20 ~ " + (60 + (int)TP_Info.TotalLUK + LuckBuffNum));
            //Luk�� �̳� �������°� ����ó��
            int RandNum;
            if (60 + (int)TP_Info.TotalLUK <= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)TP_Info.TotalLUK);
            }

            if(NegativeAmount == 0)//�������� �ϳ� �̾Ƽ� ����
            {
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
            else if(PositiveAmount == 0)//�������� �ϳ� �̾Ƽ� ����
            {
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //������ List���� �ϳ��� �̾Ƽ� ����
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)TP_Info.TotalLUK)
            {
                //������ List���� �ϳ��� �̾Ƽ� ����
                int PositiveRandNum = Random.Range(0, PositiveList.Count);
                BattleResultStatus.ResultMagnification.Add(PositiveList[PositiveRandNum]);
            }
        }
        //������ ���� ����
        BattleResultStatus.BuffMagnification = 1f;
        if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.PowerOfDeath] >= 1)
        {
            float HPRatio = (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().CurrentHP - 1) / (PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo().MaxHP - 1);
            float PowerOfDeathMagnification = Mathf.Lerp(3, 1, HPRatio);
            BattleResultStatus.BuffMagnification *= PowerOfDeathMagnification;
        }

        switch (ActionButtonType)
        {
            case "Charm":
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
        BattleResultStatus.FinalResultAmountPlus = 0f;
        switch(ActionButtonType)
        {
            case "Charm":
            case "Attack":
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.EXPPower] >= 1)//������ �� ���� ������
                {
                    BattleResultStatus.FinalResultAmountPlus += (int)(P_Info.Experience / 100f);
                }
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    BattleResultStatus.FinalResultAmountPlus += PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect];
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] = 0;
                }
                break;
            case "Defense":
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] = (int)(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] * 0.5f);
                }
                break;
            case "Rest":
                if (PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] = (int)(PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList[(int)EBuffType.Reflect] * 0.5f);
                }
                break;
        }
        //���� �߰� ��ġ // ���� �����ϰ� EXPMG �ʹ� ��ȭ ȿ���� ������ �޴� ��ġ��
        
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
            case (int)EMonsterActionState.Charm:
            case (int)EMonsterActionState.Attack:
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterWeaponCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentATK;
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ChainAttack] >= 1)
                {
                    BattleResultStatus.BaseAmountPlus += CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ChainAttack];
                }
                break;
            case (int)EMonsterActionState.Defense:
                MonEquipmentCode = CurrentTurnObject.GetComponent<Monster>().MonsterArmorCode;
                BattleResultStatus.BaseAmount = MC_Info.MonsterCurrentDUR;
                break;
        }
        EquipmentSO ESO_Info = EquipmentInfoManager.Instance.GetMonEquipmentInfo(MonEquipmentCode);

        if(BattleResultStatus.BaseAmount > 0)
        {
            //���� �������� ResultMagnification ����
            BattleResultStatus.ResultMagnification.Clear();
            SetMonsterBattleStatus(MC_Info, ESO_Info, CurrentState);
        }
    }

    protected void SetMonsterBattleStatus(MonsterCurrentStatus MC_Info, EquipmentSO ESO_Info, int CurrentState)
    {
        if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.BloodFamiliy] >= 1)
        {
            BattleResultStatus.BaseAmountPlus += (int)(MC_Info.MonsterCurrentHP / 10);
        }

        if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.PowerOfDeath] >= 1)
        {
            float HPRatio = (MC_Info.MonsterCurrentHP - 1) / (MC_Info.MonsterMaxHP - 1);
            float PowerOfDeathMagnification = Mathf.Lerp(3, 1, HPRatio);
            BattleResultStatus.BuffMagnification *= PowerOfDeathMagnification;
        }
        switch (CurrentState)
        {
            case (int)EMonsterActionState.Charm:
            case (int)EMonsterActionState.Attack:
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
        //BattleResultStatus.FinalResultPlus
        switch (CurrentState)
        {
            case (int)EMonsterActionState.Charm:
            case (int)EMonsterActionState.Attack:
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    BattleResultStatus.FinalResultAmountPlus += CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect];
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] = 0;
                }
                break;
            default:
                if (CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] >= 1)
                {
                    CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] =
                        (int)(CurrentTurnObject.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Reflect] * 0.5f);
                }
                break;
        }
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

            //Debug.Log("���� : ���� = 1 : 1 => ���� : 0 ~ 30" + " ���� : 30 ~ " + (60 + (int)MC_Info.MonsterCurrentLUK + LuckBuffNum));
            //Luk�� �̳� �������°� ����ó��
            int RandNum;
            if (60 + (int)MC_Info.MonsterCurrentLUK <= 1)
            {
                RandNum = 0;
            }
            else
            {
                RandNum = Random.Range(0, 60 + (int)MC_Info.MonsterCurrentLUK);
            }

            if (RandNum >= 0 && RandNum < MultiplyNum * NegativeAmount)
            {
                //������ List���� �ϳ��� �̾Ƽ� ����
                int NegativeRandNum = Random.Range(0, NegativeList.Count);
                BattleResultStatus.ResultMagnification.Add(NegativeList[NegativeRandNum]);
            }
            else if (RandNum >= MultiplyNum * NegativeAmount && RandNum < 60 + (int)MC_Info.MonsterCurrentLUK)
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
