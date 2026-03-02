using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaEquipDictionaryUI : MonoBehaviour
{
    [Header("StateCard")]
    public GameObject StateCardStorage;
    public GameObject[] StateCards;
    [Header("EquipTypeCard")]
    public GameObject EquipTypeCardStorage;
    public GameObject[] EquipTypeCards;
    [Header("HighLight")]
    public GameObject[] CardHighLight;
    public GameObject[] LineHighLight;
    [Header("EquipInfo01")]
    public GameObject SelectionEquipInfo01;
    public Image SelectionEquipImage01;
    public TextMeshProUGUI STRAmountText01;
    public TextMeshProUGUI DURAmountText01;
    public TextMeshProUGUI RESAmountText01;
    public TextMeshProUGUI SPDAmountText01;
    public TextMeshProUGUI LUKAmountText01;
    public TextMeshProUGUI SpendSTAText01;
    public TextMeshProUGUI EquipEffectConditionText01;
    public TextMeshProUGUI EquipEffectText01;
    [Header("EquipInfo02")]
    public GameObject SelectionEquipInfo02;
    public Image SelectionEquipImage02;
    public TextMeshProUGUI STRAmountText02;
    public TextMeshProUGUI DURAmountText02;
    public TextMeshProUGUI RESAmountText02;
    public TextMeshProUGUI SPDAmountText02;
    public TextMeshProUGUI LUKAmountText02;
    public TextMeshProUGUI SpendSTAText02;
    public TextMeshProUGUI EquipEffectConditionText02;
    public TextMeshProUGUI EquipEffectText02;
    [Header("BuffDetailInfo")]
    public GameObject BuffDetailUI;
    public TextMeshProUGUI BuffDetailTitleText;
    public TextMeshProUGUI BuffDetailText;
    // Start is called before the first frame update
    protected enum EStateTypeCard
    {
        STR,
        DUR,
        RES,
        SPD,
        LUK,
        HP,
        STA,
        NOR
    }
    protected enum EEquipTypeCard
    {
        Weapon,
        Armor,
        Helmet,
        Boots,
        Acc
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveGachaEquipDictionary()
    {
        //왼쪽에서 오른쪽으로(만약 이미 DoTween중이라면 이 함수의 호출을 무시
        //카드 하이라이트를 STR과 Weapon위치에 활성화, 위치 시키기
        //줄 하이라이트를 하나 끄고(1번) 하나를 연결 시키기(0번)
        //STR과 Weapon에 맞는 정보를 EquipInfo01에 넣기
        //EquipInfo02는 비활성화
        //BuffDetail 비활성화
        //애니메이션으로 활성화
        if (DOTween.IsTweening(gameObject.GetComponent<RectTransform>()) == true)
            return;

        SoundManager.Instance.PlayUISFX("UI_Button");
        gameObject.SetActive(true);

        CardHighLight[0].GetComponent<RectTransform>().anchoredPosition = CalculatePosForHighLight(StateCards[(int)EStateTypeCard.STR], StateCardStorage);
        CardHighLight[0].SetActive(true);
        CardHighLight[1].GetComponent<RectTransform>().anchoredPosition = CalculatePosForHighLight(EquipTypeCards[(int)EEquipTypeCard.Weapon], EquipTypeCardStorage);
        CardHighLight[1].SetActive(true);

        LineHighLight[0].SetActive(false);
        LineHighLight[1].SetActive(false);
        //SetHightLightLine();

        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1920f, 0f);
        gameObject.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.5f);
    }

    public void InActiveGachaEquipDictionay()
    {
        //왼쪽으로 애니메이션을 보냄
        //만약 이미 DoTween중이라면 추가 호출을 무시함
        if (DOTween.IsTweening(gameObject.GetComponent<RectTransform>()) == true)
            return;

        SoundManager.Instance.PlayUISFX("UI_Button");
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        gameObject.GetComponent<RectTransform>().DOAnchorPosX(-1920f, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    protected Vector2 CalculatePosForHighLight(GameObject ObjectA, GameObject ObjectB)
    {
        return ObjectA.GetComponent<RectTransform>().anchoredPosition + ObjectB.GetComponent<RectTransform>().anchoredPosition;
    }

    protected void SetHightLightLine(Vector2 PosA, Vector2 PosB)
    {
        int ActiveLineNum;
        int InActiveLineNum;
        if (LineHighLight[0] == false)
        {
            ActiveLineNum = 0;
            InActiveLineNum = 1;
        }
        else
        {
            ActiveLineNum = 1;
            InActiveLineNum = 0;
        }
        float HighLightLineWidth = Mathf.Pow(Mathf.Pow(PosA.x - PosB.x, 2f) + Mathf.Pow(PosA.y - PosB.y, 2f), 0.5f) - 84f;
        Vector2 LinePos;
        LinePos.x = (PosA.x + PosB.x) / 2;
        LinePos.y = (PosA.y + PosB.y) / 2;
        float LineRotationZ = Mathf.Atan2(PosA.y - PosB.y, PosA.x - PosB.x);

        LineHighLight[ActiveLineNum].GetComponent<RectTransform>().sizeDelta = new Vector2(HighLightLineWidth, 10);
        LineHighLight[ActiveLineNum].GetComponent<RectTransform>().anchoredPosition = LinePos;
        LineHighLight[ActiveLineNum].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, LineRotationZ);

        LineHighLight[ActiveLineNum].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        LineHighLight[ActiveLineNum].SetActive(true);
        LineHighLight[ActiveLineNum].GetComponent<Image>().DOFade(1f, 0.5f);

        if (LineHighLight[InActiveLineNum].activeSelf == true)
        {
            LineHighLight[InActiveLineNum].GetComponent<Image>().color = Color.white;
            LineHighLight[InActiveLineNum].GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() =>
            {
                LineHighLight[InActiveLineNum].SetActive(false);
            });
        }
    }
}
