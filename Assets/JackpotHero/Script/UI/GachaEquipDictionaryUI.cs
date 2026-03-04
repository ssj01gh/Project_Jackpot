using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
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
    public TextMeshProUGUI EquipEffectText02;
    [Header("BuffDetailInfo")]
    public GameObject BuffDetailUI;
    public TextMeshProUGUI BuffDetailTitleText;
    public TextMeshProUGUI BuffDetailText;

    private int EquipCurrentLinkIndex01 = -1;
    private int EquipCurrentLinkIndex02 = -1;

    private int CurrentSelectionStateType = 0;
    private int CurrentSelectionEquipType = 0;
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
        Vector3 mousePosition = Input.mousePosition;
        //EquipDetail_EquipDetailText
        //EquipDetail_InvenDetailText

        // 링크 인덱스를 찾음
        if(SelectionEquipInfo01.activeSelf == true)
        {
            int EquipLinkIndex01 = TMP_TextUtilities.FindIntersectingLink(EquipEffectText01, mousePosition, Camera.main);
            if (EquipLinkIndex01 != EquipCurrentLinkIndex01)
            {
                // 기존 링크에서 마우스가 벗어났을 때
                if (EquipCurrentLinkIndex01 != -1)
                {
                    string oldID = EquipEffectText01.textInfo.linkInfo[EquipCurrentLinkIndex01].GetLinkID();
                    OnLinkExit(oldID);
                }

                // 새로운 링크에 마우스가 올라갔을 때
                if (EquipLinkIndex01 != -1)
                {
                    string newID = EquipEffectText01.textInfo.linkInfo[EquipLinkIndex01].GetLinkID();

                    TMP_LinkInfo LinkInfo = EquipEffectText01.textInfo.linkInfo[EquipLinkIndex01];
                    int FirstCharIndex = LinkInfo.linkTextfirstCharacterIndex;
                    int LastCharIndex = FirstCharIndex + LinkInfo.linkTextLength - 1;
                    Vector3 BottomLeft = EquipEffectText01.textInfo.characterInfo[FirstCharIndex].bottomLeft;
                    Vector3 TopRight = EquipEffectText01.textInfo.characterInfo[LastCharIndex].topRight;
                    Vector3 CenterPos = (EquipEffectText01.transform.TransformPoint(BottomLeft) + EquipEffectText01.transform.TransformPoint(TopRight)) * 0.5f;

                    OnLinkEnter(newID, CenterPos);
                }
                EquipCurrentLinkIndex01 = EquipLinkIndex01;
            }
        }
        if(SelectionEquipInfo02.activeSelf == true)
        {
            int EquipLinkIndex02 = TMP_TextUtilities.FindIntersectingLink(EquipEffectText02, mousePosition, Camera.main);
            if (EquipLinkIndex02 != EquipCurrentLinkIndex02)
            {
                // 기존 링크에서 마우스가 벗어났을 때
                if (EquipCurrentLinkIndex02 != -1)
                {
                    string oldID = EquipEffectText02.textInfo.linkInfo[EquipCurrentLinkIndex02].GetLinkID();
                    OnLinkExit(oldID);
                }

                // 새로운 링크에 마우스가 올라갔을 때
                if (EquipLinkIndex02 != -1)
                {
                    string newID = EquipEffectText02.textInfo.linkInfo[EquipLinkIndex02].GetLinkID();

                    TMP_LinkInfo LinkInfo = EquipEffectText02.textInfo.linkInfo[EquipLinkIndex02];
                    int FirstCharIndex = LinkInfo.linkTextfirstCharacterIndex;
                    int LastCharIndex = FirstCharIndex + LinkInfo.linkTextLength - 1;
                    Vector3 BottomLeft = EquipEffectText02.textInfo.characterInfo[FirstCharIndex].bottomLeft;
                    Vector3 TopRight = EquipEffectText02.textInfo.characterInfo[LastCharIndex].topRight;
                    Vector3 CenterPos = (EquipEffectText02.transform.TransformPoint(BottomLeft) + EquipEffectText02.transform.TransformPoint(TopRight)) * 0.5f;

                    OnLinkEnter(newID, CenterPos);
                }
                EquipCurrentLinkIndex02 = EquipLinkIndex02;
            }
        }
    }

    public void ActiveGachaEquipDictionary()
    {
        //Debug.Log("IsClickingTips");
        //왼쪽에서 오른쪽으로(만약 이미 DoTween중이라면 이 함수의 호출을 무시
        //카드 하이라이트를 STR과 Weapon위치에 활성화, 위치 시키기
        //줄 하이라이트를 하나 끄고(1번) 하나를 연결 시키기(0번)
        //STR과 Weapon에 맞는 정보를 EquipInfo01에 넣기
        //EquipInfo02는 비활성화
        //BuffDetail 비활성화
        //애니메이션으로 활성화
        if (DOTween.IsTweening(gameObject.GetComponent<RectTransform>()) == true)
            return;

        if (gameObject.activeSelf == true)
            return;

        CurrentSelectionStateType = (int)EStateTypeCard.STR;
        CurrentSelectionEquipType = (int)EEquipTypeCard.Weapon;

        SoundManager.Instance.PlayUISFX("UI_Button");
        gameObject.SetActive(true);

        CardHighLight[0].GetComponent<RectTransform>().anchoredPosition = CalculatePosForHighLight(StateCards[CurrentSelectionStateType], StateCardStorage);
        CardHighLight[0].SetActive(true);
        CardHighLight[1].GetComponent<RectTransform>().anchoredPosition = CalculatePosForHighLight(EquipTypeCards[CurrentSelectionEquipType], EquipTypeCardStorage);
        CardHighLight[1].SetActive(true);

        LineHighLight[0].SetActive(false);
        LineHighLight[1].SetActive(false);
        SetHightLightLine(CalculatePosForHighLight(StateCards[CurrentSelectionStateType], StateCardStorage), CalculatePosForHighLight(EquipTypeCards[CurrentSelectionEquipType], EquipTypeCardStorage));

        SelectionEquipInfo01.SetActive(false);
        SelectionEquipInfo02.SetActive(false);
        SetSelectionEquipInfo(CurrentSelectionStateType, CurrentSelectionEquipType);

        BuffDetailUI.SetActive(false);

        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1920f, 0f);
        gameObject.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.5f);
    }

    public void InActiveGachaEquipDictionay()
    {
        //왼쪽으로 애니메이션을 보냄
        //만약 이미 DoTween중이라면 추가 호출을 무시함
        if (DOTween.IsTweening(gameObject.GetComponent<RectTransform>()) == true)
            return;

        if (gameObject.activeSelf == false)
            return;

        SoundManager.Instance.PlayUISFX("UI_Button");
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        gameObject.GetComponent<RectTransform>().DOAnchorPosX(-1920f, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void ClickStateTypeCard(int StateTypeNum)
    {
        if (CurrentSelectionStateType == StateTypeNum)
            return;

        CurrentSelectionStateType = StateTypeNum;

        SoundManager.Instance.PlayUISFX("UI_Button");
        CardHighLight[0].GetComponent<RectTransform>().anchoredPosition = CalculatePosForHighLight(StateCards[CurrentSelectionStateType], StateCardStorage);
        SetHightLightLine(CalculatePosForHighLight(StateCards[CurrentSelectionStateType], StateCardStorage), CalculatePosForHighLight(EquipTypeCards[CurrentSelectionEquipType], EquipTypeCardStorage));
        SetSelectionEquipInfo(CurrentSelectionStateType, CurrentSelectionEquipType);
    }

    public void ClickEquipTypeCard(int EquipTypeNum)
    {
        if (CurrentSelectionEquipType == EquipTypeNum)
            return;

        CurrentSelectionEquipType = EquipTypeNum;

        SoundManager.Instance.PlayUISFX("UI_Button");
        CardHighLight[1].GetComponent<RectTransform>().anchoredPosition = CalculatePosForHighLight(EquipTypeCards[CurrentSelectionEquipType], EquipTypeCardStorage);
        SetHightLightLine(CalculatePosForHighLight(StateCards[CurrentSelectionStateType], StateCardStorage), CalculatePosForHighLight(EquipTypeCards[CurrentSelectionEquipType], EquipTypeCardStorage));
        SetSelectionEquipInfo(CurrentSelectionStateType, CurrentSelectionEquipType);
    }

    protected Vector2 CalculatePosForHighLight(GameObject ObjectA, GameObject ObjectB)
    {
        return ObjectA.GetComponent<RectTransform>().anchoredPosition + ObjectB.GetComponent<RectTransform>().anchoredPosition;
    }

    protected void SetHightLightLine(Vector2 PosA, Vector2 PosB)
    {
        int ActiveLineNum;
        int InActiveLineNum;
        if (LineHighLight[0].activeSelf == false)//나타 나야 할 애의 DoTween 애니메이션을 중지 시킨다.
        {
            ActiveLineNum = 0;
            InActiveLineNum = 1;
        }
        else
        {
            ActiveLineNum = 1;
            InActiveLineNum = 0;
        }
        DOTween.Kill(LineHighLight[ActiveLineNum].GetComponent<Image>());
        float HighLightLineWidth = Mathf.Pow(Mathf.Pow(PosA.x - PosB.x, 2f) + Mathf.Pow(PosA.y - PosB.y, 2f), 0.5f) - 84f;
        Vector2 LinePos;
        LinePos.x = (PosA.x + PosB.x) / 2;
        LinePos.y = (PosA.y + PosB.y) / 2;
        float LineRotationZ = Mathf.Atan2(PosA.y - PosB.y, PosA.x - PosB.x) * Mathf.Rad2Deg;

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

    protected void SetSelectionEquipInfo(int StateType, int EquipType)
    {
        //여기서 스탯과 스테미나 사용량을 T * n식으로 넣어야함
        EquipmentInfo DictionaryEquipInfo = EquipmentInfoManager.Instance.GetDictionaryEquipmentInfo(StateType, EquipType);
        if (SelectionEquipInfo01.activeSelf == false)
        {//01번을 활성화 시키는 작업, 02번이 켜져있다면 비활성화 시키는 작업
            //1번의 DoTween을 kill
            DOTween.Kill(SelectionEquipInfo01.GetComponent<CanvasGroup>());
            SelectionEquipImage01.sprite = DictionaryEquipInfo.EquipmentImage;
            STRAmountText01.text = "0";
            DURAmountText01.text = "0";
            RESAmountText01.text = "0";
            SPDAmountText01.text = "0";
            LUKAmountText01.text = "0";
            if (DictionaryEquipInfo.AddSTRAmount != 0)
                STRAmountText01.text = "T×" + DictionaryEquipInfo.AddSTRAmount;
            if(DictionaryEquipInfo.AddDURAmount != 0)
                DURAmountText01.text = "T×" + DictionaryEquipInfo.AddDURAmount;
            if(DictionaryEquipInfo.AddRESAmount != 0)
                RESAmountText01.text = "T×" + DictionaryEquipInfo.AddRESAmount;
            if(DictionaryEquipInfo.AddSPDAmount != 0)
                SPDAmountText01.text = "T×" + DictionaryEquipInfo.AddSPDAmount;
            if(DictionaryEquipInfo.AddLUKAmount != 0)
                LUKAmountText01.text = "T×" + DictionaryEquipInfo.AddLUKAmount;

            StartCoroutine(LoadEquipSpendSTA(EquipType, (int)DictionaryEquipInfo.SpendTiredness, true));
            EquipEffectText01.text = DictionaryEquipInfo.EquipmentDetail;

            SelectionEquipInfo01.SetActive(true);
            SelectionEquipInfo01.GetComponent<CanvasGroup>().alpha = 0;
            SelectionEquipInfo01.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);

            if(SelectionEquipInfo02.activeSelf == true)
            {
                SelectionEquipInfo02.GetComponent<CanvasGroup>().alpha = 1;
                SelectionEquipInfo02.GetComponent<CanvasGroup>().DOFade(0f, 0.2f).OnComplete(() =>
                {
                    SelectionEquipInfo02.SetActive(false);
                });
            }
        }
        else
        {
            //2번의 DoTween을 kill
            DOTween.Kill(SelectionEquipInfo02.GetComponent<CanvasGroup>());
            SelectionEquipImage02.sprite = DictionaryEquipInfo.EquipmentImage;
            STRAmountText02.text = "0";
            DURAmountText02.text = "0";
            RESAmountText02.text = "0";
            SPDAmountText02.text = "0";
            LUKAmountText02.text = "0";
            if (DictionaryEquipInfo.AddSTRAmount != 0)
                STRAmountText02.text = "T×" + DictionaryEquipInfo.AddSTRAmount;
            if (DictionaryEquipInfo.AddDURAmount != 0)
                DURAmountText02.text = "T×" + DictionaryEquipInfo.AddDURAmount;
            if (DictionaryEquipInfo.AddRESAmount != 0)
                RESAmountText02.text = "T×" + DictionaryEquipInfo.AddRESAmount;
            if (DictionaryEquipInfo.AddSPDAmount != 0)
                SPDAmountText02.text = "T×" + DictionaryEquipInfo.AddSPDAmount;
            if (DictionaryEquipInfo.AddLUKAmount != 0)
                LUKAmountText02.text = "T×" + DictionaryEquipInfo.AddLUKAmount;

            StartCoroutine(LoadEquipSpendSTA(EquipType, (int)DictionaryEquipInfo.SpendTiredness, false));
            EquipEffectText02.text = DictionaryEquipInfo.EquipmentDetail;

            SelectionEquipInfo02.SetActive(true);
            SelectionEquipInfo02.GetComponent<CanvasGroup>().alpha = 0;
            SelectionEquipInfo02.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);

            if (SelectionEquipInfo01.activeSelf == true)
            {
                SelectionEquipInfo01.GetComponent<CanvasGroup>().alpha = 1;
                SelectionEquipInfo01.GetComponent<CanvasGroup>().DOFade(0f, 0.2f).OnComplete(() =>
                {
                    SelectionEquipInfo01.SetActive(false);
                });
            }
        }
    }

    private IEnumerator LoadEquipSpendSTA(int EquipType, int SpendSTAAmount, bool IsFirstInfo)
    {
        yield return LocalizationSettings.InitializationOperation;

        string EquipSTATableKey = "";
        if (EquipType == (int)EEquipType.TypeWeapon)
            EquipSTATableKey = "PS_SC_WeaponSTA";
        else if (EquipType == (int)EEquipType.TypeArmor)
            EquipSTATableKey = "PS_SC_ArmorSTA";
        else if (EquipType == (int)EEquipType.TypeHelmet)
            EquipSTATableKey = "PS_SC_HelmetSTA";
        else if (EquipType == (int)EEquipType.TypeBoots)
            EquipSTATableKey = "PS_SC_BootsSTA";
        else if (EquipType == (int)EEquipType.TypeAcc)
            EquipSTATableKey = "PS_SC_AccSTA";

        if (EquipSTATableKey == "")
            EquipSTATableKey = "PS_SC_EquipSTAError";

        var DictionayEquipSTA = LocalizationSettings.StringDatabase.GetTable("PlaySceneShortText");

        if(IsFirstInfo == true)
        {//첫번째 Info에 기록
            SpendSTAText01.text = DictionayEquipSTA.GetEntry(EquipSTATableKey).GetLocalizedString();
            if (EquipType == (int)EEquipType.TypeWeapon)
                SpendSTAText01.text += "T×"+SpendSTAAmount.ToString();
            else if (EquipType == (int)EEquipType.TypeArmor)
                SpendSTAText01.text += "T×" + SpendSTAAmount.ToString();
        }
        else
        {//두번째 Info에 기록
            SpendSTAText02.text = DictionayEquipSTA.GetEntry(EquipSTATableKey).GetLocalizedString();
            if (EquipType == (int)EEquipType.TypeWeapon)
                SpendSTAText02.text += "T×" + SpendSTAAmount.ToString();
            else if (EquipType == (int)EEquipType.TypeArmor)
                SpendSTAText02.text += "T×" + SpendSTAAmount.ToString();
        }
    }

    private void OnLinkEnter(string id, Vector2 Pos)
    {
        BuffDetailUI.transform.position = Pos;
        BuffDetailUI.SetActive(true);
        switch (id)
        {
            case "STRWE":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.DefenseDebuff).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.DefenseDebuff).BuffDetail;
                break;
            case "STRAR":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffDetail;
                break;
            case "STRHE":
            case "ForestBracelet02":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Recharge).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Recharge).BuffDetail;
                break;
            case "STRBO":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffDetail;
                break;
            case "STRAC":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughFist).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughFist).BuffDetail;
                break;
            case "DURWE":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.AttackDebuff).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.AttackDebuff).BuffDetail;
                break;
            case "DURAR":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnbreakableArmor).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnbreakableArmor).BuffDetail;
                break;
            case "DURHE":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffDetail;
                break;
            case "DURAC":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughSkin).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughSkin).BuffDetail;
                break;
            case "RESWE01":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Poison).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Poison).BuffDetail;
                break;
            case "RESWE02":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Burn).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Burn).BuffDetail;
                break;
            case "RESAR":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Misfortune).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Misfortune).BuffDetail;
                break;
            case "RESHE":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffDetail;
                break;
            case "RESBO":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Slow).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Slow).BuffDetail;
                break;
            case "RESAC":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CurseOfDeath).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CurseOfDeath).BuffDetail;
                break;
            case "SPDWE":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ChainAttack).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ChainAttack).BuffDetail;
                break;
            case "SPDAR":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffDetail;
                break;
            case "SPDHE":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffDetail;
                break;
            case "SPDBO":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Haste).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Haste).BuffDetail;
                break;
            case "SPDAC":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CorruptSerum).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CorruptSerum).BuffDetail;
                break;
            case "LUKBO":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Luck).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Luck).BuffDetail;
                break;
            case "HPWE":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.LifeSteal).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.LifeSteal).BuffDetail;
                break;
            case "HPAR":
            case "ForestBracelet01":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffDetail;
                break;
            case "HPHE":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Charm).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Charm).BuffDetail;
                break;
            case "HPBO":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffDetail;
                break;
            case "HPAC":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.PowerOfDeath).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.PowerOfDeath).BuffDetail;
                break;
            case "STABO":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffDetail;
                break;
            case "STAAC":
                BuffDetailTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffDetail;
                break;
        }
    }

    private void OnLinkExit(string id)
    {
        BuffDetailUI.SetActive(false);
        // 예: 원래 색상 복구, 툴팁 숨기기 등
        //Debug.Log($"마우스 벗어남: {id}");
    }
}
