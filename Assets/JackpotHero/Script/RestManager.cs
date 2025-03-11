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
    private PlaySceneManager PlaySceneMgr;
    

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
    //------------------------------RestFunc
    public void SetRestResult(int RestQuality)//����� �����Ѵ�. <- �޾ƾ� �ϴ� ���� �޽� Ƚ��, �޽� ����Ƽ
    {
        //VeryBad -> 0 ~ 49, Bad -> 0 ~ 24, Good -> 0 ~ 9, VeryGood -> 0 ~ 4, Perfect -> Ȯ�� ����
        MaxRestTime = (int)TimeCountSlider.value;
        CurrentRestTime = 0;
        IsPeacefulRest.Clear();
        int MonAttackRange = 0;

        switch (RestQuality)
        {
            case (int)EPlayerRestQuality.VeryBad:
                MonAttackRange = 50;
                break;
            case (int)EPlayerRestQuality.Bad:
                MonAttackRange = 25;
                break;
            case (int)EPlayerRestQuality.Good:
                MonAttackRange = 10;
                break;
            case (int)EPlayerRestQuality.VeryGood:
                MonAttackRange = 5;
                break;
            case (int)EPlayerRestQuality.Perfect:
                MonAttackRange = 0;
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

    public void StartRestCheck(RestUIScript R_UI)
    {
        StartCoroutine(RestCheckCoroutine(R_UI));
    }

    IEnumerator RestCheckCoroutine(RestUIScript R_UI)
    {
        yield return null;
        while(CurrentRestTime < MaxRestTime)
        {
            if (IsPeacefulRest[CurrentRestTime] == true)//��ȭ�Ӱ� �������ٸ�
            {
                R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1)) / MaxRestTime, false);
                while(true)
                {
                    yield return null;
                    if(R_UI.FillAmountAnimEnd == true)
                    {
                        break;
                    }
                }
                PlayerMgr.GetPlayerInfo().RecoverHPNSTAByRest(0.1f);
                UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
                CurrentRestTime++;
            }
            else//������ �Ͼ�ٸ�
            {
                //�ð踦 ���� ���������� �ϰ� ���ݽ���
                //DurationTime�� �߰��� ������ ���� ������ �ƴ� 0.3 ~ 0.8���� �������� �ֵ���
                //1 -> 1��, 0.5 -> 0.5��
                float RandAmount = Random.Range(0.2f, 0.8f);
                R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1) + RandAmount) / MaxRestTime, true, RandAmount);
                while(true)
                {
                    yield return null;
                    if(R_UI.FillAmountAnimEnd == true)
                    {
                        break;
                    }
                }
                R_UI.InActiveLeftTimeObject();
                //�޽� UI�� ����
                PlaySceneMgr.SuddenAttackByMonsterInRest(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails);
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
            R_UI.InActiveLeftTimeObject();
            R_UI.ActiveRestActionSelection();
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();
        }
        Debug.Log("RestEnd");
    }

    //---------------------------------PlayerUpgradeFunc
    public void InitUpgradeAfterStatus()
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

    public void PlayerUpgradePlusButtonClick(string UpGradeType)
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

    public void PlayerUpgradeMinusButtonClick(string UpGradeType)
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
        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < AfterStatus.NeededEXP)
        {
            UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughEXP_PlayerUpgrade);
            return;
        }
        //������� ���� ����ġ�� ����Ѱ���
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-AfterStatus.NeededEXP, true);//����ġ�� ���̰ų� �ø���
        PlayerMgr.GetPlayerInfo().UpgradePlayerStatus(AfterStatus);//������ ���׷��̵�
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//Json�� ����

        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//UI�� �ִ� ���� ����
        UIMgr.R_UI.ActiveRestActionSelection();//�޽� ����â ����
    }
    //---------------------------------------------------EquipGatchaFunc
    public void InActiveEquipGambling()
    {
        UIMgr.R_UI.ActiveRestActionSelection();//�޽� ����â ����
        //�� â�� �����ٴ°� �÷��̾��� ���Ȱ� ��� ������ ���ɼ��� ����
        PlayerMgr.GetPlayerInfo().SetPlayerTotalStatus();//��� ��ȭ�� ���� ���� ��ȭ����
        UIMgr.PE_UI.SetEquipmentImage(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());//��� ��ȭ�� ���� ���ǥ�� ui ��ȭ ����
        UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //��� ��ȭ�� ���� ����ǥ�� ui ��ȭ ����
    }
}
