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


    public bool FillAmountAnimEnd { protected set; get; } = false;
    void Start()
    {
        RestActionSelection.SetActive(false);
        RestTimeSelectionUI.SetActive(false);
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

    public void SetLeftTimeTextNSlider(int LeftNum, float SliderValue)//Text�� ���� Num�� Image fillAmount�� ������ SliderValue�� ����
    {
        FillAmountAnimEnd = false;
        LeftTimeSlider.DOFillAmount(SliderValue, 1).SetEase(Ease.Linear).OnComplete(() => { LeftTimeText.text = LeftNum.ToString(); FillAmountAnimEnd = true; });//Slidervalue���� 1�ʵ��� �����Ѵ�.
        //������ �Ŀ���
    }
}
