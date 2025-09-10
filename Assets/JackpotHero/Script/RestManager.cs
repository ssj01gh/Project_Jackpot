using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpGradeAfterStatus
{
    public int AfterSTR = 0;
    public int AfterDUR = 0;
    public int AfterRES = 0;
    public int AfterSPD = 0;
    public int AfterLUK = 0;
    public int AfterLevel = 0;
    public int NeededEXP = 0;
}

public class RestManager : MonoBehaviour
{
    [SerializeField]
    private PlayerManager PlayerMgr;
    [SerializeField]
    private PlaySceneUIManager UIMgr;
    [SerializeField]
    private BattleManager BattleMgr;
    

    public Slider TimeCountSlider;
    // Start is called before the first frame update
    public List<bool> IsPeacefulRest { protected set; get; } = new List<bool>();
    protected int MaxRestTime = 0;
    protected int CurrentRestTime = 0;

    public UpGradeAfterStatus AfterStatus { protected set; get; } = new UpGradeAfterStatus();


    protected int BeforeLevel;
    /*
    protected int[] LevelEXP = new int[100] 
    { }
    */
    protected const int LevelUpgradeBasePoint = 10;
    protected const float LevelUpIncreaseRatio = 1.2f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActiveRestActionSelection()
    {
        UIMgr.R_UI.ActiveRestActionSelection();
    }
    public void PlayButtonSoundInRestUI()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
    }

    //------------------------------Rest

    public void PressRestActionRest()//�޽� ����â���� �޽��� ������ �޽� �ð� ����â Ȱ��ȭ
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.R_UI.ActiveRestTimeSelectionUI(PlayerMgr.GetPlayerInfo());
    }
    public void SetRestMgrRestResult()//�޽��� �ð��� �����ϰ�
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        SetRestResult();
        UIMgr.R_UI.ActiveLeftTimeObject(IsPeacefulRest, this);
    }
    protected void SetRestResult()//����� �����Ѵ�. <- �޾ƾ� �ϴ� ���� �޽� Ƚ��, �޽� ����Ƽ
    {
        //VeryBad -> 0 ~ 49, Bad -> 0 ~ 24, Good -> 0 ~ 9, VeryGood -> 0 ~ 4, Perfect -> Ȯ�� ����
        int RestQuality = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails;
        MaxRestTime = (int)TimeCountSlider.value;
        CurrentRestTime = 0;
        IsPeacefulRest.Clear();
        int MonAttackRange = 5;

        switch (RestQuality)
        {
            case (int)EPlayerRestQuality.VeryBad:
                MonAttackRange = 55;
                break;
            case (int)EPlayerRestQuality.Bad:
                MonAttackRange = 35;
                break;
            case (int)EPlayerRestQuality.Good:
                MonAttackRange = 20;
                break;
            case (int)EPlayerRestQuality.VeryGood:
                MonAttackRange = 10;
                break;
            case (int)EPlayerRestQuality.Perfect:
                MonAttackRange = 5;
                break;
        }

        for (int i = 0; i < MaxRestTime; i++)
        {
            int RandNum = Random.Range(0, 100);
            if (RandNum < MonAttackRange)// 50�̻� ������ �佺 //25�̻� ������ �佺
            {//���� �ɸ��� ���� ������
                IsPeacefulRest.Add(false);
            }
            else
            {
                IsPeacefulRest.Add(true);
            }
        }
    }

    public void StartRestCheck()
    {
        StartCoroutine(RestCheckCoroutine());
    }

    IEnumerator RestCheckCoroutine()
    {
        yield return null;
        while(CurrentRestTime < MaxRestTime)
        {
            if (IsPeacefulRest[CurrentRestTime] == true)//��ȭ�Ӱ� �������ٸ�
            {
                UIMgr.R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1)) / MaxRestTime, false);
                while(true)
                {
                    yield return null;
                    if(UIMgr.R_UI.FillAmountAnimEnd == true)//1���� ���������� ���⼭ °��?//ȸ���ϴ� ����Ʈ�� ������?
                    {
                        EffectManager.Instance.ActiveEffect("BattleEffect_Buff_RegenHP", PlayerMgr.GetPlayerInfo().gameObject.transform.position + new Vector3(0, 0.5f));
                        SoundManager.Instance.PlaySFX("Rest_TickTack");
                        break;
                    }
                }
                PlayerMgr.GetPlayerInfo().RecoverHPNSTAByRest(0.1f);
                UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
                    PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);
                CurrentRestTime++;
            }
            else//������ �Ͼ�ٸ�
            {
                //�ð踦 ���� ���������� �ϰ� ���ݽ���
                //DurationTime�� �߰��� ������ ���� ������ �ƴ� 0.3 ~ 0.8���� �������� �ֵ���
                //1 -> 1��, 0.5 -> 0.5��
                float RandAmount = Random.Range(0.2f, 0.8f);
                UIMgr.R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1) + RandAmount) / MaxRestTime, true, RandAmount);
                while(true)
                {
                    yield return null;
                    if(UIMgr.R_UI.FillAmountAnimEnd == true)
                    {
                        break;
                    }
                }
                UIMgr.R_UI.InActiveLeftTimeObject();
                //�޽� UI�� ����
                PlayerMgr.GetPlayerInfo().SetIsSuddenAttackAndRestQuality();
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = 0;

                PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle_Battle);
                BattleMgr.InitCurrentBattleMonsters();
                BattleMgr.InitMonsterNPlayerActiveGuage();
                BattleMgr.ProgressBattle();
                //PlaySceneMgr.SuddenAttackByMonsterInRest(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails);
                //�ٷ� ���� ���� �ɵ�? ���� �ð��� �����̰� ��¥�� �ٽ� �޽� �������� ���ư�����
                break;
            }
            yield return null;
        }
        //�߰��� ������ �ȳ����� �޽��� ��������
        if(CurrentRestTime >= MaxRestTime)
        {
            //ȸ���� ������ �ٽ� �޽� �ൿ ����â�� ���;���
            //Player�� ������ Json�� �����ϰ�
            Debug.Log("FullRest");
            UIMgr.R_UI.InActiveLeftTimeObject();
            UIMgr.R_UI.ActiveRestActionSelection();
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        }
        Debug.Log("RestEnd");
    }

    //---------------------------------PlayerUpgradeFunc
    public void PressPlayerUpgradeButton()
    {
        InitUpgradeAfterStatus();
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.R_UI.ActivePlayerUpGradeUI(PlayerMgr.GetPlayerInfo());
    }
    protected void InitUpgradeAfterStatus()
    {
        AfterStatus.AfterSTR = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().StrengthLevel;
        AfterStatus.AfterDUR = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().DurabilityLevel;
        AfterStatus.AfterRES = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().ResilienceLevel;
        AfterStatus.AfterSPD = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SpeedLevel;
        AfterStatus.AfterLUK = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().LuckLevel;
        AfterStatus.AfterLevel = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Level;
        AfterStatus.NeededEXP = 0;
        BeforeLevel = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Level;
    }

    public void PressPlayerUpGradePlusButton(string ButtonType)
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerUpgradePlusButtonClick(ButtonType);
        UIMgr.R_UI.PlayerUpgradePLUSMINUSButtonClick(PlayerMgr.GetPlayerInfo(), AfterStatus);
    }
    public void PressPlayerUpGradeMinusButton(string ButtonType)
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerUpgradeMinusButtonClick(ButtonType);
        UIMgr.R_UI.PlayerUpgradePLUSMINUSButtonClick(PlayerMgr.GetPlayerInfo(), AfterStatus);
    }

    protected void PlayerUpgradePlusButtonClick(string UpGradeType)
    {
        AfterStatus.AfterLevel++;
        switch(UpGradeType)
        {
            case "STR":
                AfterStatus.AfterSTR++;
                break;
            case "DUR":
                AfterStatus.AfterDUR++;
                break;
            case "RES":
                AfterStatus.AfterRES++;
                break;
            case "SPD":
                AfterStatus.AfterSPD++;
                break;
            case "LUK":
                AfterStatus.AfterLUK++;
                break;
        }
        CalculateNeededEXP(true);
    }

    protected void PlayerUpgradeMinusButtonClick(string UpGradeType)
    {
        AfterStatus.AfterLevel--;
        switch (UpGradeType)
        {
            case "STR":
                AfterStatus.AfterSTR--;
                break;
            case "DUR":
                AfterStatus.AfterDUR--;
                break;
            case "RES":
                AfterStatus.AfterRES--;
                break;
            case "SPD":
                AfterStatus.AfterSPD--;
                break;
            case "LUK":
                AfterStatus.AfterLUK--;
                break;
        }
        CalculateNeededEXP(false);
    }

    protected void CalculateNeededEXP(bool IsPlus)
    {
        //AfterStatus.NeededEXP += Mathf.CeilToInt(LevelUpgradeBasePoint * Mathf.Pow(LevelUpIncreaseRatio, i));
        if(IsPlus == true)
        {
            AfterStatus.NeededEXP += Mathf.CeilToInt(LevelUpgradeBasePoint * Mathf.Pow(LevelUpIncreaseRatio, AfterStatus.AfterLevel - 1));
        }
        else if(IsPlus == false)
        {
            AfterStatus.NeededEXP -= Mathf.CeilToInt(LevelUpgradeBasePoint * Mathf.Pow(LevelUpIncreaseRatio, AfterStatus.AfterLevel));
        }
    }

    public void PlayerUpgradeOKButton()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < AfterStatus.NeededEXP)
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughEXP_PlayerUpgrade);
            return;
        }
        //������� ���� ����ġ�� ����Ѱ���
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-AfterStatus.NeededEXP, true);//����ġ�� ���̰ų� �ø���
        PlayerMgr.GetPlayerInfo().UpgradePlayerStatus(AfterStatus);//������ ���׷��̵�
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//Json�� ����

        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);//UI�� �ִ� ���� ����
        UIMgr.R_UI.ActiveRestActionSelection();//�޽� ����â ����
    }
    //---------------------------------------------------EquipGatchaFunc
    public void ActiveEquipGambling()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.EDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.MEDI_UI.InActiveEquipmentDetailInfoUI();
        UIMgr.R_UI.ActivePlayerEquipMg();
    }
    public void InActiveEquipGambling()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        UIMgr.R_UI.ActiveRestActionSelection();//�޽� ����â ����
        //�� â�� �����ٴ°� �÷��̾��� ���Ȱ� ��� ������ ���ɼ��� ����
        PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//��� ��ȭ�� ���� ���� ��ȭ����
        UIMgr.PE_UI.SetEquipmentImage(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//��� ��ȭ�� ���� ���ǥ�� ui ��ȭ ����
        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);
        //��� ��ȭ�� ���� ����ǥ�� ui ��ȭ ����
    }
    //-----------------------------------------------------RestEndButton
    public void PressRestEndButton()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int) EPlayerCurrentState.SelectAction;
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = 0;

        UIMgr.PressRestEnd();
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //StartCoroutine(CheckBackGroundMoveEnd(true));
    }
}
