using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class RestUIScript : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("RestActionSelection")]
    public GameObject RestActionSelection;
    [Header("RestTimeSelection")]
    public GameObject RestTimeSelectionUI;
    public TextMeshProUGUI RestTimeTitleText;
    public TextMeshProUGUI PercentOfMonsterAttackText;
    public Slider RestCountSlider;
    [Header("LeftTimeObject")]
    public GameObject LeftTimeObject;
    public Image LeftTimeSlider;
    public TextMeshProUGUI LeftTimeText;
    [Header("PlayerUpgrade")]
    public GameObject PlayerUpgradeUI;
    public TextMeshProUGUI STRBeforeText;
    public TextMeshProUGUI STRAfterText;
    public Button STRMinusButton;
    public TextMeshProUGUI DURBeforeText;
    public TextMeshProUGUI DURAfterText;
    public Button DURMinusButton;
    public TextMeshProUGUI RESBeforeText;
    public TextMeshProUGUI RESAfterText;
    public Button RESMinusButton;
    public TextMeshProUGUI SPDBeforeText;
    public TextMeshProUGUI SPDAfterText;
    public Button SPDMinusButton;
    public TextMeshProUGUI LUKBeforeText;
    public TextMeshProUGUI LUKAfterText;
    public Button LUKMinusButton;
    public TextMeshProUGUI NeededEXPText;
    public Button UpgradeOKButton;
    [Header("PlayerEquipManagement")]
    public PlayerEquipMgUI PlayerEquipMgObject;



    public bool FillAmountAnimEnd { protected set; get; } = false;
    void Start()
    {
        RestActionSelection.SetActive(false);
        RestTimeSelectionUI.SetActive(false);
        LeftTimeObject.SetActive(false);
        PlayerUpgradeUI.SetActive(false);
        PlayerEquipMgObject.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //-----------------------RestActionSelection
    public void ActiveRestActionSelection()
    {
        InActiveRestTimeSelectionUI();//�̰� �����ٴ°Ŵ� ������ �� off
        InActiveLeftTimeObject();
        InActivePlayerUpGradeUI();
        InActivePlayerEquipMg();
        RestActionSelection.GetComponent<RectTransform>().anchoredPosition = new Vector2(480, 0);
        RestActionSelection.SetActive(true);
        RestActionSelection.GetComponent<RectTransform>().DOAnchorPosX(-480, 0.3f).SetEase(Ease.OutBack);
    }

    public void InActiveRestActionSelection()
    {
        if(RestActionSelection.activeSelf == true)
        {
            RestActionSelection.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480, 0);
            RestActionSelection.GetComponent<RectTransform>().DOAnchorPosX(480, 0.3f).OnComplete(() => { RestActionSelection.SetActive(false); });
        }
    }

    //-------------------------------RestTimeSelection
    public void ActiveRestTimeSelectionUI(PlayerScript PlayerInfo)
    {
        InActiveRestActionSelection();//�̰� �����ٴ°Ŵ� ������ �� off
        InActiveLeftTimeObject();
        InActivePlayerUpGradeUI();
        InActivePlayerEquipMg();
        //�߿����� ����Ƽ�� ���� Ȯ��ǥ��, ������ �޶���
        switch (PlayerInfo.GetPlayerStateInfo().CurrentPlayerActionDetails)
        {
            case (int)EPlayerRestQuality.VeryBad:
                RestTimeTitleText.text = "�ν��� �޽�";
                PercentOfMonsterAttackText.text = "50%";
                break;
            case (int)EPlayerRestQuality.Bad:
                RestTimeTitleText.text = "������ �޽�";
                PercentOfMonsterAttackText.text = "25%";
                break;
            case (int)EPlayerRestQuality.Good:
                RestTimeTitleText.text = "����� �޽�";
                PercentOfMonsterAttackText.text = "10%";
                break;
            case (int)EPlayerRestQuality.VeryGood:
                RestTimeTitleText.text = "������ �޽�";
                PercentOfMonsterAttackText.text = "5%";
                break;
            case (int)EPlayerRestQuality.Perfect:
                RestTimeTitleText.text = "�Ϻ��� �޽�";
                PercentOfMonsterAttackText.text = "0%";
                break;
        }

        RestTimeSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 0);
        RestTimeSelectionUI.SetActive(true);
        RestTimeSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(-400, 0.3f).SetEase(Ease.OutBack);
    }

    public void InActiveRestTimeSelectionUI()
    {
        if (RestTimeSelectionUI.activeSelf == true)
        {
            RestTimeSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
            RestTimeSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.3f).OnComplete(() => { RestTimeSelectionUI.SetActive(false); });
        }
    }

    //--------------------------LeftTimeObject
    public void ActiveLeftTimeObject(List<bool> RestResult, RestManager RestMgr)//->
    {
        InActiveRestActionSelection();
        InActiveRestTimeSelectionUI();
        InActivePlayerUpGradeUI();
        InActivePlayerEquipMg();

        LeftTimeText.text = RestResult.Count.ToString();
        LeftTimeSlider.fillAmount = 1;

        LeftTimeObject.GetComponent<CanvasGroup>().alpha = 0;
        LeftTimeObject.SetActive(true);
        LeftTimeObject.GetComponent<CanvasGroup>().DOFade(1, 0.3f).OnComplete(() => { RestMgr.StartRestCheck(this); });
    }

    public void InActiveLeftTimeObject()
    {
        if(LeftTimeObject.activeSelf == true)
        {
            LeftTimeObject.GetComponent<CanvasGroup>().alpha = 1;
            LeftTimeObject.GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(() => { LeftTimeObject.SetActive(false); });
        }
    }

    public void SetLeftTimeTextNSlider(int LeftNum, float SliderValue, bool IsSuddenAttack, float DurationTime = 1)//Text�� ���� Num�� Image fillAmount�� ������ SliderValue�� ����
    {
        FillAmountAnimEnd = false;
        if(IsSuddenAttack == false)
        {
            LeftTimeSlider.DOFillAmount(SliderValue, DurationTime).SetEase(Ease.Linear).OnComplete(() => { LeftTimeText.text = LeftNum.ToString(); FillAmountAnimEnd = true; });//Slidervalue���� 1�ʵ��� �����Ѵ�.
        }
        else if(IsSuddenAttack == true)
        {
            LeftTimeSlider.DOFillAmount(SliderValue, DurationTime).SetEase(Ease.Linear).OnComplete(() => { FillAmountAnimEnd = true; });//0.5��
        }
    }
    //----------------------------PlayerUpgradeUI
    public void ActivePlayerUpGradeUI(PlayerScript PlayerInfo)//�ʱ�
    {
        InActiveRestActionSelection();//�̰� �����ٴ°Ŵ� ������ �� off
        InActiveRestTimeSelectionUI();
        InActiveLeftTimeObject();
        InActivePlayerEquipMg();

        //STR
        if (PlayerInfo.GetPlayerStateInfo().StrengthLevel <= 0)
        {
            STRMinusButton.interactable = false;
        }
        else
        {
            STRMinusButton.interactable = true;
        }
        STRBeforeText.text = PlayerInfo.GetPlayerStateInfo().StrengthLevel.ToString();
        STRAfterText.text = PlayerInfo.GetPlayerStateInfo().StrengthLevel.ToString();
        //DUR
        if (PlayerInfo.GetPlayerStateInfo().DurabilityLevel <= 0)
        {
            DURMinusButton.interactable = false;
        }
        else
        {
            DURMinusButton.interactable = true;
        }
        DURBeforeText.text = PlayerInfo.GetPlayerStateInfo().DurabilityLevel.ToString();
        DURAfterText.text = PlayerInfo.GetPlayerStateInfo().DurabilityLevel.ToString();
        //RES
        if (PlayerInfo.GetPlayerStateInfo().ResilienceLevel <= 0)
        {
            RESMinusButton.interactable = false;
        }
        else
        {
            RESMinusButton.interactable = true;
        }
        RESBeforeText.text = PlayerInfo.GetPlayerStateInfo().ResilienceLevel.ToString();
        RESAfterText.text = PlayerInfo.GetPlayerStateInfo().ResilienceLevel.ToString();
        //SPD
        if (PlayerInfo.GetPlayerStateInfo().SpeedLevel <= 0)
        {
            SPDMinusButton.interactable = false;
        }
        else
        {
            SPDMinusButton.interactable = true;
        }
        SPDBeforeText.text = PlayerInfo.GetPlayerStateInfo().SpeedLevel.ToString();
        SPDAfterText.text = PlayerInfo.GetPlayerStateInfo().SpeedLevel.ToString();
        //LUK
        if (PlayerInfo.GetPlayerStateInfo().LuckLevel <= 0)
        {
            LUKMinusButton.interactable = false;
        }
        else
        {
            LUKMinusButton.interactable = true;
        }
        LUKBeforeText.text = PlayerInfo.GetPlayerStateInfo().LuckLevel.ToString();
        LUKAfterText.text = PlayerInfo.GetPlayerStateInfo().LuckLevel.ToString();
        //EXP
        NeededEXPText.text = "0";

        PlayerUpgradeUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 0);
        PlayerUpgradeUI.SetActive(true);
        PlayerUpgradeUI.GetComponent<RectTransform>().DOAnchorPosX(-400, 0.3f).SetEase(Ease.OutBack);
    }
    public void InActivePlayerUpGradeUI()
    {
        if (PlayerUpgradeUI.activeSelf == true)
        {
            PlayerUpgradeUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
            PlayerUpgradeUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.3f).OnComplete(() => { PlayerUpgradeUI.SetActive(false); });
        }
    }
    public void PlayerUpgradePLUSMINUSButtonClick(PlayerScript PlayerInfo, UpGradeAfterStatus AfterStatus)
    {
        if (AfterStatus.AfterSTR <= 0)
        {
            STRMinusButton.interactable = false;
        }
        else
        {
            STRMinusButton.interactable = true;
        }
        STRBeforeText.text = PlayerInfo.GetPlayerStateInfo().StrengthLevel.ToString();
        STRAfterText.text = AfterStatus.AfterSTR.ToString();
        //DUR
        if (AfterStatus.AfterDUR <= 0)
        {
            DURMinusButton.interactable = false;
        }
        else
        {
            DURMinusButton.interactable = true;
        }
        DURBeforeText.text = PlayerInfo.GetPlayerStateInfo().DurabilityLevel.ToString();
        DURAfterText.text = AfterStatus.AfterDUR.ToString();
        //RES
        if (AfterStatus.AfterRES <= 0)
        {
            RESMinusButton.interactable = false;
        }
        else
        {
            RESMinusButton.interactable = true;
        }
        RESBeforeText.text = PlayerInfo.GetPlayerStateInfo().ResilienceLevel.ToString();
        RESAfterText.text = AfterStatus.AfterRES.ToString();
        //SPD
        if (AfterStatus.AfterSPD <= 0)
        {
            SPDMinusButton.interactable = false;
        }
        else
        {
            SPDMinusButton.interactable = true;
        }
        SPDBeforeText.text = PlayerInfo.GetPlayerStateInfo().SpeedLevel.ToString();
        SPDAfterText.text = AfterStatus.AfterSPD.ToString();
        //LUK
        if (AfterStatus.AfterLUK <= 0)
        {
            LUKMinusButton.interactable = false;
        }
        else
        {
            LUKMinusButton.interactable = true;
        }
        LUKBeforeText.text = PlayerInfo.GetPlayerStateInfo().LuckLevel.ToString();
        LUKAfterText.text = AfterStatus.AfterLUK.ToString();
        //EXP
        NeededEXPText.text = AfterStatus.NeededEXP.ToString();
    }
    //----------------------------PlayerEquipMg
    public void ActivePlayerEquipMg()
    {
        InActiveRestTimeSelectionUI();//�̰� �����ٴ°Ŵ� ������ �� off
        InActiveLeftTimeObject();
        InActivePlayerUpGradeUI();
        InActiveRestActionSelection();

        
        PlayerEquipMgObject.ActivePlayerEquipMg();
    }

    public void InActivePlayerEquipMg()
    {
        if(PlayerEquipMgObject.gameObject.activeSelf == true)
        {
            PlayerEquipMgObject.InActivePlayerEquipMg();
        }
    }
}
