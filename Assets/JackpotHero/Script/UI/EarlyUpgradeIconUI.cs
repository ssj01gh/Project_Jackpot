using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

public class EarlyUpgradeIconUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject[] EarlyUpgradeIcons;
    public TextMeshProUGUI[] EarlyUpgradeText;
    public GameObject EarlyUpgradeDetail;
    public TextMeshProUGUI EarlyUpgradeDetailText;

    protected enum EEarlyUpgradeIcon
    {
        STR,
        DUR,
        RES,
        SPD,
        LUK,
        HP,
        STA,
        EXP,
        EXPMG,
        EQUIP
    }
    // Start is called before the first frame update
    void Start()
    {
        ActiveEarlyUpgradeIcon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveEarlyUpgradeIcon()
    {
        for(int i = 0; i < EarlyUpgradeIcons.Length; i++)
        {
            EarlyUpgradeIcons[i].SetActive(false);
        }
        EarlyUpgradeDetail.SetActive(false);

        if (JsonReadWriteManager.Instance.E_Info.EarlyStrengthLevel >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.STR].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.STR].text = JsonReadWriteManager.Instance.E_Info.EarlyStrengthLevel.ToString();
        }

        if (JsonReadWriteManager.Instance.E_Info.EarlyDurabilityLevel >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.DUR].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.DUR].text = JsonReadWriteManager.Instance.E_Info.EarlyDurabilityLevel.ToString();
        }

        if (JsonReadWriteManager.Instance.E_Info.EarlyResilienceLevel >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.RES].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.RES].text = JsonReadWriteManager.Instance.E_Info.EarlyResilienceLevel.ToString();
        }

        if(JsonReadWriteManager.Instance.E_Info.EarlySpeedLevel >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.SPD].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.SPD].text = JsonReadWriteManager.Instance.E_Info.EarlySpeedLevel.ToString();
        }

        if(JsonReadWriteManager.Instance.E_Info.EarlyLuckLevel >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.LUK].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.LUK].text = JsonReadWriteManager.Instance.E_Info.EarlyLuckLevel.ToString();
        }

        if (JsonReadWriteManager.Instance.E_Info.EarlyHpLevel >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.HP].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.HP].text = JsonReadWriteManager.Instance.E_Info.EarlyHpLevel.ToString();
        }

        if (JsonReadWriteManager.Instance.E_Info.EarlyTirednessLevel >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.STA].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.STA].text = JsonReadWriteManager.Instance.E_Info.EarlyTirednessLevel.ToString();
        }

        if (JsonReadWriteManager.Instance.E_Info.EarlyExperience >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.EXP].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.EXP].text = JsonReadWriteManager.Instance.E_Info.EarlyExperience.ToString();
        }

        if (JsonReadWriteManager.Instance.E_Info.EarlyExperienceMagnification >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.EXPMG].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.EXPMG].text = JsonReadWriteManager.Instance.E_Info.EarlyExperienceMagnification.ToString();
        }

        if (JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel >= 1)
        {
            EarlyUpgradeIcons[(int)EEarlyUpgradeIcon.EQUIP].SetActive(true);
            EarlyUpgradeText[(int)EEarlyUpgradeIcon.EQUIP].text = JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel.ToString();
        }
    }

    public void InActiveAllIcon()
    {
        for (int i = 0; i < EarlyUpgradeIcons.Length; i++)
        {
            EarlyUpgradeIcons[i].SetActive(false);
        }
        EarlyUpgradeDetail.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //eventData.pointerEnter
        //ActiveBuffDetailUI(eventData.pointerEnter);
        for (int i = 0; i < EarlyUpgradeIcons.Length; i++)
        {
            if (EarlyUpgradeIcons[i].gameObject == eventData.pointerEnter)//마우스가 올라간 버프 이미지가 이거라면
            {
                ActiveEarlyUpgradeDetailUI(i, eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition);
                return;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EarlyUpgradeDetail.SetActive(false);
    }

    protected void ActiveEarlyUpgradeDetailUI(int IconNum, Vector3 TargetPos)
    {
        string EU_Key = GetKeyString(IconNum);
        if (EU_Key == "")
            return;

        EarlyUpgradeDetail.SetActive(true);
        EarlyUpgradeDetail.GetComponent<RectTransform>().anchoredPosition = TargetPos;
        StartCoroutine(LoadEarlyUpgradeText(EU_Key));
    }

    protected string GetKeyString(int IconNum)
    {
        string ReturnKey = "";
        int UpgradeLevel = 0;
        switch (IconNum)
        {
            case (int)EEarlyUpgradeIcon.STR:
                ReturnKey = "EU_STR_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EarlyStrengthLevel;
                break;
            case (int)EEarlyUpgradeIcon.DUR:
                ReturnKey = "EU_DUR_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EarlyDurabilityLevel;
                break;
            case (int)EEarlyUpgradeIcon.RES:
                ReturnKey = "EU_RES_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EarlyResilienceLevel;
                break;
            case (int)EEarlyUpgradeIcon.SPD:
                ReturnKey = "EU_SPD_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EarlySpeedLevel;
                break;
            case (int)EEarlyUpgradeIcon.LUK:
                ReturnKey = "EU_LUK_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EarlyLuckLevel ;
                break;
            case (int)EEarlyUpgradeIcon.HP:
                ReturnKey = "EU_HP_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EarlyHpLevel;
                break;
            case (int)EEarlyUpgradeIcon.STA:
                ReturnKey = "EU_STA_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EarlyTirednessLevel;
                break;
            case (int)EEarlyUpgradeIcon.EXP:
                ReturnKey = "EU_EXP_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EarlyExperience;
                break;
            case (int)EEarlyUpgradeIcon.EXPMG:
                ReturnKey = "EU_EXPMG_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EarlyExperienceMagnification;
                break;
            case (int)EEarlyUpgradeIcon.EQUIP:
                ReturnKey = "EU_EQUIP_";
                UpgradeLevel = JsonReadWriteManager.Instance.E_Info.EquipmentSuccessionLevel;
                break;
        }

        switch(UpgradeLevel)
        {
            case 1:
                ReturnKey += "01";
                break;
            case 2:
                ReturnKey += "02";
                break;
            case 3:
                ReturnKey += "03";
                break;
            case 4:
                ReturnKey += "04";
                break;
            case 5:
            case 6:
                ReturnKey += "0506";
                break;
            case 7:
                ReturnKey += "07";
                break;
        }
        return ReturnKey;
    }

    private IEnumerator LoadEarlyUpgradeText(string EarlyUpgradeKey)
    {
        yield return LocalizationSettings.InitializationOperation;

        var TutorialTable = LocalizationSettings.StringDatabase.GetTable("EarlyUpgrade_PlayScene");
        EarlyUpgradeDetailText.text = TutorialTable.GetEntry(EarlyUpgradeKey).GetLocalizedString();
    }
}
