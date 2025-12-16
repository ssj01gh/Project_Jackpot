using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[System.Serializable]
public class EarlyStrengthenSO
{
    public string ESCode;
    public EarlyStrengthenDetailSO ESSOData;
}

public class EarlyStrengthenUI : MonoBehaviour
{
    public TutorialManager TutorialMgr;
    public EarlyStrengthenSO[] EarlyStrengthDatas;
    [Header("ESUI")]
    public TextMeshProUGUI PointRemain;
    public TextMeshProUGUI DetailTitleText;
    public TextMeshProUGUI DetailText;
    public GameObject ButtonOutlineObject;

    [Header("Level0Button")]
    public Button[] LevelZeros;
    [Header("ATK")]
    public Button ATKMinusButton;
    public Button ATKPlusButton;
    public Button[] ATKLevels;
    [Header("DUR")]
    public Button DURMinusButton;
    public Button DURPlusButton;
    public Button[] DURLevels;
    [Header("SPD")]
    public Button SPDMinusButton;
    public Button SPDPlusButton;
    public Button[] SPDLevels;
    [Header("RES")]
    public Button RESMinusButton;
    public Button RESPlusButton;
    public Button[] RESLevels;
    [Header("LUK")]
    public Button LUKMinusButton;
    public Button LUKPlusButton;
    public Button[] LUKLevels;
    [Header("HP")]
    public Button HPMinusButton;
    public Button HPPlusButton;
    public Button[] HPLevels;
    [Header("STA")]
    public Button STAMinusButton;
    public Button STAPlusButton;
    public Button[] StaLevels;
    [Header("EXP")]
    public Button EXPMinusButton;
    public Button EXPPlusButton;
    public Button[] EXPLevels;
    [Header("EXPMG")]
    public Button EXPMGMinusButton;
    public Button EXPMGPlusButton;
    public Button[] EXPMGLevels;
    [Header("EQUIP")]
    public Button EQUIPMinusButton;
    public Button EQUIPPlusButton;
    public Button[] EQUIPLevels;

    protected EarlyStrengthenInfo EarlyInfo;
    protected Dictionary<string, EarlyStrengthenDetailSO> ESDictionary = new Dictionary<string, EarlyStrengthenDetailSO>();

    protected const int MaxLevel = 7;
    protected Color UnActiveGrayColor = new Color(0.25f, 0.25f, 0.25f);
    void Start()
    {
        InitEarlyData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitEarlyData()
    {
        
        foreach(EarlyStrengthenSO ESSO in EarlyStrengthDatas)
        {
            if(!ESDictionary.ContainsKey(ESSO.ESCode))
            {
                ESDictionary.Add(ESSO.ESCode, ESSO.ESSOData);
            }
        }
        
    }

    public void EarlyStrengthenActive()
    {
        //Debug.Log(JsonReadWriteManager.Instance.E_Info.PlayerEarlyPoint);
        EarlyInfo = JsonReadWriteManager.Instance.GetCopyEarlyInfo();
        DetailTitleText.text = "";
        DetailText.text = "";

        SetEarlyStrengthenUI();
        //UI조정
        //gameObject.SetActive(true);
        DetailTitleText.text = ESDictionary["ATK00"].EarlyDetailTitle;
        DetailText.text = ESDictionary["ATK00"].DetailText;
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1080);
        gameObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f).OnComplete(() => { StrengthenActiveAnimationEnd(); });
        //gameObject.GetComponent<Animator>().SetInteger("EarlyStrengthenState", 1);
    }

    public void StrengthenActiveAnimationEnd()
    {
        if (ESDictionary.ContainsKey("ATK00"))
        {
            //DetailTitleText.text = ESDictionary["ATK00"].EarlyDetailTitle;
            //DetailText.text = ESDictionary["ATK00"].DetailText;
            if (LevelZeros.Length > 0)
            {
                ButtonOutlineObject.transform.position = LevelZeros[0].transform.position;
            }
            else
            {
                ButtonOutlineObject.SetActive(false);
            }
        }
        if(JsonReadWriteManager.Instance.T_Info.TitleEarlyStrengthen == false)
        {
            TutorialMgr.SetLinkedTutorialNStartTutorial("Tutorial/Title");
            JsonReadWriteManager.Instance.T_Info.TitleEarlyStrengthen = true;
        }
    }

    public void EarlyStrengthenInActive()
    {
        if (gameObject.activeSelf == false)
            return;

        SoundManager.Instance.PlayUISFX("UI_Button");
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        gameObject.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.5f);//.OnComplete(() => { gameObject.SetActive(false); });
    }

    public void EarlyStrengthenSetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    public void SetEarlyStrengthenUI()
    {
        //UI조정
        PointRemain.text = EarlyInfo.PlayerEarlyPoint.ToString();
        for(int i = 0; i < LevelZeros.Length; i++)
        {
            LevelZeros[i].interactable = true;
            LevelZeros[i].gameObject.GetComponent<Image>().color = Color.white;
        }
        //ATK
        if (EarlyInfo.EarlyStrengthLevel <= 0)
            ATKMinusButton.interactable = false;
        else
            ATKMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EarlyStrengthLevel >= MaxLevel)
            ATKPlusButton.interactable = false;
        else
            ATKPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EarlyStrengthLevel; i++)
            ATKLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EarlyStrengthLevel; i < MaxLevel; i++)
            ATKLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
        //DUR
        if (EarlyInfo.EarlyDurabilityLevel <= 0)
            DURMinusButton.interactable = false;
        else
            DURMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EarlyDurabilityLevel >= MaxLevel)
            DURPlusButton.interactable = false;
        else
            DURPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EarlyDurabilityLevel; i++)
            DURLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EarlyDurabilityLevel; i < MaxLevel; i++)
            DURLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
        //SPD
        if (EarlyInfo.EarlySpeedLevel <= 0)
            SPDMinusButton.interactable = false;
        else
            SPDMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EarlySpeedLevel >= MaxLevel)
            SPDPlusButton.interactable = false;
        else
            SPDPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EarlySpeedLevel; i++)
            SPDLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EarlySpeedLevel; i < MaxLevel; i++)
            SPDLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
        //RES
        if (EarlyInfo.EarlyResilienceLevel <= 0)
            RESMinusButton.interactable = false;
        else
            RESMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EarlyResilienceLevel >= MaxLevel)
            RESPlusButton.interactable = false;
        else
            RESPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EarlyResilienceLevel; i++)
            RESLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EarlyResilienceLevel; i < MaxLevel; i++)
            RESLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
        //LUK
        if (EarlyInfo.EarlyLuckLevel <= 0)
            LUKMinusButton.interactable = false;
        else
            LUKMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EarlyLuckLevel >= MaxLevel)
            LUKPlusButton.interactable = false;
        else
            LUKPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EarlyLuckLevel; i++)
            LUKLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EarlyLuckLevel; i < MaxLevel; i++)
            LUKLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
        //HP
        if (EarlyInfo.EarlyHpLevel <= 0)
            HPMinusButton.interactable = false;
        else
            HPMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EarlyHpLevel >= MaxLevel)
            HPPlusButton.interactable = false;
        else
            HPPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EarlyHpLevel; i++)
            HPLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EarlyHpLevel; i < MaxLevel; i++)
            HPLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
        //STA
        if (EarlyInfo.EarlyTirednessLevel <= 0)
            STAMinusButton.interactable = false;
        else
            STAMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EarlyTirednessLevel >= MaxLevel)
            STAPlusButton.interactable = false;
        else
            STAPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EarlyTirednessLevel; i++)
            StaLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EarlyTirednessLevel; i < MaxLevel; i++)
            StaLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
        //EXP
        if (EarlyInfo.EarlyExperience <= 0)
            EXPMinusButton.interactable = false;
        else
            EXPMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EarlyExperience >= MaxLevel)
            EXPPlusButton.interactable = false;
        else
            EXPPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EarlyExperience; i++)
            EXPLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EarlyExperience; i < MaxLevel; i++)
            EXPLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
        //EXPMG
        if (EarlyInfo.EarlyExperienceMagnification <= 0)
            EXPMGMinusButton.interactable = false;
        else
            EXPMGMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EarlyExperienceMagnification >= MaxLevel)
            EXPMGPlusButton.interactable = false;
        else
            EXPMGPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EarlyExperienceMagnification; i++)
            EXPMGLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EarlyExperienceMagnification; i < MaxLevel; i++)
            EXPMGLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
        //EQUIP
        
        if (EarlyInfo.EquipmentSuccessionLevel <= 0)
            EQUIPMinusButton.interactable = false;
        else
            EQUIPMinusButton.interactable = true;

        if (EarlyInfo.PlayerEarlyPoint <= 0 || EarlyInfo.EquipmentSuccessionLevel >= MaxLevel)
            EQUIPPlusButton.interactable = false;
        else
            EQUIPPlusButton.interactable = true;

        for (int i = 0; i < EarlyInfo.EquipmentSuccessionLevel; i++)
            EQUIPLevels[i].gameObject.GetComponent<Image>().color = Color.white;

        for (int i = EarlyInfo.EquipmentSuccessionLevel; i < MaxLevel; i++)
            EQUIPLevels[i].gameObject.GetComponent<Image>().color = UnActiveGrayColor;
    }

    //Type : ATK, DUR, RES, SPD, LUK, HP, STA, EXP, EXPMG, EQUIP
    public void PlusButtonClick(string ButtonType)
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        //1. 플러스 버튼을 누른다.
        //2. 플러스 버튼의 종류에 따라 맞는 강화 효과의 레벨이 올라간다.
        //3. 남은 강화 포인트는 1줄어든다.
        //4. UI를 갱신 한다.
        switch (ButtonType)
        {
            case "ATK":
                if(EarlyInfo.EarlyStrengthLevel < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EarlyStrengthLevel++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
            case "DUR":
                if (EarlyInfo.EarlyDurabilityLevel < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EarlyDurabilityLevel++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
            case "RES":
                if (EarlyInfo.EarlyResilienceLevel < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EarlyResilienceLevel++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
            case "SPD":
                if (EarlyInfo.EarlySpeedLevel < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EarlySpeedLevel++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
            case "LUK":
                if (EarlyInfo.EarlyLuckLevel < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EarlyLuckLevel++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
            case "HP":
                if (EarlyInfo.EarlyHpLevel < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EarlyHpLevel++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
            case "STA":
                if (EarlyInfo.EarlyTirednessLevel < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EarlyTirednessLevel++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
            case "EXP":
                if(EarlyInfo.EarlyExperience < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EarlyExperience++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
            case "EXPMG":
                if (EarlyInfo.EarlyExperienceMagnification < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EarlyExperienceMagnification++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
            case "EQUIP":
                if (EarlyInfo.EquipmentSuccessionLevel < MaxLevel && EarlyInfo.PlayerEarlyPoint > 0)
                {
                    EarlyInfo.EquipmentSuccessionLevel++;
                    EarlyInfo.PlayerEarlyPoint--;
                }
                break;
        }
        SetEarlyStrengthenUI();
    }

    public void MinusButtonClick(string ButtonType)
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        //1. 마이너스 버튼을 누른다.
        //2. 마이너스 버튼의 종류에 따라 맞는 강화 효과의 레벨이 내려간다.
        //3. 남은 강화 포인트는 1늘어든다.
        //4. UI를 갱신 한다.
        switch (ButtonType)
        {
            case "ATK":
                if (EarlyInfo.EarlyStrengthLevel > 0)
                {
                    EarlyInfo.EarlyStrengthLevel--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
            case "DUR":
                if (EarlyInfo.EarlyDurabilityLevel > 0)
                {
                    EarlyInfo.EarlyDurabilityLevel--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
            case "RES":
                if (EarlyInfo.EarlyResilienceLevel > 0)
                {
                    EarlyInfo.EarlyResilienceLevel--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
            case "SPD":
                if (EarlyInfo.EarlySpeedLevel > 0)
                {
                    EarlyInfo.EarlySpeedLevel--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
            case "LUK":
                if (EarlyInfo.EarlyLuckLevel > 0)
                {
                    EarlyInfo.EarlyLuckLevel--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
            case "HP":
                if (EarlyInfo.EarlyHpLevel > 0)
                {
                    EarlyInfo.EarlyHpLevel--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
            case "STA":
                if (EarlyInfo.EarlyStrengthLevel > 0)
                {
                    EarlyInfo.EarlyTirednessLevel--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
            case "EXP":
                if (EarlyInfo.EarlyExperience > 0)
                {
                    EarlyInfo.EarlyExperience--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
            case "EXPMG":
                if (EarlyInfo.EarlyExperienceMagnification > 0)
                {
                    EarlyInfo.EarlyExperienceMagnification--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
            case "EQUIP":
                if (EarlyInfo.EquipmentSuccessionLevel > 0)
                {
                    EarlyInfo.EquipmentSuccessionLevel--;
                    EarlyInfo.PlayerEarlyPoint++;
                }
                break;
        }
        SetEarlyStrengthenUI();
    }

    public void LevelButtonClick(string ButtonCode)
    {
        if (!ESDictionary.ContainsKey(ButtonCode))
            return;

        SoundManager.Instance.PlayUISFX("UI_Button");
        GameObject ClickButton = EventSystem.current.currentSelectedGameObject;
        ButtonOutlineObject.transform.position = ClickButton.transform.position;

        DetailTitleText.text = ESDictionary[ButtonCode].EarlyDetailTitle;
        DetailText.text = ESDictionary[ButtonCode].DetailText;
    }

    public void LoadPlayScene()//여기서 바뀐 EarlyData를 JsonManager에 넘겨야함
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        JsonReadWriteManager.Instance.SaveEarlyInfo(EarlyInfo);
        //JsonReadWriteManager.Instance.E_Info = EarlyInfo;//JsonReadWriteManager에 값 복사
        JsonReadWriteManager.Instance.InitPlayerInfo(true);//새로 시작하는거니까 PlayInfo초기값으로 변경시키기
        JsonReadWriteManager.Instance.InitLinkageEventInfo(true);
        LoadingScene.Instance.LoadAnotherScene("PlayScene");
    }
}
