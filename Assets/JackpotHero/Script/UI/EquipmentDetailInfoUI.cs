using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class EquipmentDetailInfoUI : MonoBehaviour
{
    public Image ClickEquipImage;
    public TextMeshProUGUI ClickEquipName;
    public TextMeshProUGUI ClickEquipSpendSTA;

    public TextMeshProUGUI ClickEquipAddSTR;
    public TextMeshProUGUI ClickEquipAddDUR;
    public TextMeshProUGUI ClickEquipAddRES;
    public TextMeshProUGUI ClickEquipAddSPD;
    public TextMeshProUGUI ClickEquipAddLUK;

    public TextMeshProUGUI ClickEquipDetailText;

    public GameObject ClickEquipCardContainer;
    public GameObject[] CardContainers;

    public GameObject ClickEquipCardButtonDownArrow;
    public GameObject ClickEquipCardButtonUpArrow;

    public TextMeshProUGUI TextWithLink;//링크 있는 텍스트
    public GameObject BuffDetailExplainObject;//설명 나올 오브젝트
    public TextMeshProUGUI BuffDetailExplainTitleText;//설명 제목
    public TextMeshProUGUI BuffDetailExplainDetailText;//설명 상세
    // Start is called before the first frame update

    private int CurrentLinkIndex = -1;
    void Start()
    {
        if (BuffDetailExplainObject != null)
        {
            BuffDetailExplainObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;

        // 링크 인덱스를 찾음
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(TextWithLink, mousePosition, Camera.main);

        if (linkIndex != CurrentLinkIndex)
        {
            // 기존 링크에서 마우스가 벗어났을 때
            if (CurrentLinkIndex != -1)
            {
                string oldID = TextWithLink.textInfo.linkInfo[CurrentLinkIndex].GetLinkID();
                OnLinkExit(oldID);
            }

            // 새로운 링크에 마우스가 올라갔을 때
            if (linkIndex != -1)
            {
                string newID = TextWithLink.textInfo.linkInfo[linkIndex].GetLinkID();
                OnLinkEnter(newID);
            }

            CurrentLinkIndex = linkIndex;
        }
        
    }
    private void OnLinkEnter(string id)
    {
        /*
         * DetailExplainText.text = "도달 최대 층수 (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
    ")\r\n일반 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillNormalMonster +
    ")\r\n엘리트 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillEliteMonster +
    ")\r\n남은 경험치 (" + PlayerInfo.GetPlayerStateInfo().Experience +
    ")\r\n선한 영향력 (" + PlayerInfo.GetPlayerStateInfo().GoodKarma + ")";
         */
        BuffDetailExplainObject.SetActive(true);
        int BuffTypeNum = 0;
        switch (id)
        {
            case "MDefenseDebuff":
            case "STRWE":
                BuffTypeNum = (int)EBuffType.DefenseDebuff;
                break;
            case "MUnDead":
            case "STRAR":
            case "HPBO":
                BuffTypeNum = (int)EBuffType.UnDead;
                break;
            case "STRHE":
            case "ForestBracelet02":
                BuffTypeNum = (int)EBuffType.Recharge;
                break;
            case "MOverCharge":
            case "STRBO":
            case "SPDHE":
                BuffTypeNum = (int)EBuffType.OverCharge;
                break;
            case "MToughFist":
            case "STRAC":
                BuffTypeNum = (int)EBuffType.ToughFist;
                break;
            case "MAttackDebuff":
            case "DURWE":
                BuffTypeNum = (int)EBuffType.AttackDebuff;
                break;
            case "MUnbreakableArmor":
            case "DURAR":
                BuffTypeNum = (int)EBuffType.UnbreakableArmor;
                break;
            case "MRegenArmor":
            case "DURHE":
            case "SPDAR":
                BuffTypeNum = (int)EBuffType.RegenArmor;
                break;
            case "MToughSkin":
            case "DURAC":
                BuffTypeNum = (int)EBuffType.ToughSkin;
                break;
            case "RESWE01":
            case "MPoison":
                BuffTypeNum = (int)EBuffType.Poison;
                break;
            case "MBurn":
            case "RESWE02":
                BuffTypeNum = (int)EBuffType.Burn;
                break;
            case "MMisfortune":
            case "RESAR":
                BuffTypeNum = (int)EBuffType.Misfortune;
                break;
            case "MRegeneration":
            case "RESHE":
            case "HPAR":
            case "ForestBracelet01":
                BuffTypeNum = (int)EBuffType.Regeneration;
                break;
            case "MSlow":
            case "RESBO":
                BuffTypeNum = (int)EBuffType.Slow;
                break;
            case "RESAC":
            case "MCurseOfDeath":
                BuffTypeNum = (int)EBuffType.CurseOfDeath;
                break;
            case "SPDWE":
                BuffTypeNum = (int)EBuffType.ChainAttack;
                break;
            case "SPDBO":
                BuffTypeNum = (int)EBuffType.Haste;
                break;
            case "SPDAC":
                BuffTypeNum = (int)EBuffType.CorruptSerum;
                break;
            case "LUKBO":
            case "MLuck":
                BuffTypeNum = (int)EBuffType.Luck;
                break;
            case "HPWE":
                BuffTypeNum = (int)EBuffType.LifeSteal;
                break;
            case "MCharm":
            case "HPHE":
                BuffTypeNum = (int)EBuffType.Charm;
                break;
            case "MPowerOfDeath":
            case "HPAC":
                BuffTypeNum = (int)EBuffType.PowerOfDeath;
                break;
            case "MPetrification":
            case "STABO":
            case "STAAC":
                BuffTypeNum = (int)EBuffType.Petrification;
                break;
            case "MThornArmor":
                BuffTypeNum = (int)EBuffType.ThornArmor;
                break;
            case "MPlunder":
                BuffTypeNum = (int)EBuffType.Plunder;
                break;
            case "MCower":
                BuffTypeNum = (int)EBuffType.Cower;
                break;
            case "MMountainLord":
                BuffTypeNum = (int)EBuffType.MountainLord;
                break;
            case "MSelfDestruct":
                BuffTypeNum = (int)EBuffType.SelfDestruct;
                break;
            case "MCopyStrength":
                BuffTypeNum = (int)EBuffType.CopyStrength;
                break;
            case "MCopyDurability":
                BuffTypeNum = (int)EBuffType.CopyDurability;
                break;
            case "MCopySpeed":
                BuffTypeNum = (int)EBuffType.CopySpeed;
                break;
            case "MCopyLuck":
                BuffTypeNum = (int)EBuffType.CopyLuck;
                break;
            case "MConsume":
                BuffTypeNum = (int)EBuffType.Consume;
                break;
            case "MWeakness":
                BuffTypeNum = (int)EBuffType.Weakness;
                break;
            case "MSpeedAdaptation":
                BuffTypeNum = (int)EBuffType.SpeedAdaptation;
                break;
            case "MStrengthAdaptation":
                BuffTypeNum = (int)EBuffType.StrengthAdaptation;
                break;
            case "MDurabilityAdaptation":
                BuffTypeNum = (int)EBuffType.DurabilityAdaptation;
                break;
            case "MPride":
                BuffTypeNum = (int)EBuffType.Pride;
                break;
            case "MReflect":
                BuffTypeNum = (int)EBuffType.Reflect;
                break;
            case "MGreed":
                BuffTypeNum = (int)EBuffType.Greed;
                break;
            case "MEnvy":
                BuffTypeNum = (int)EBuffType.Envy;
                break;
            case "MLust":
                BuffTypeNum = (int)EBuffType.Lust;
                break;
            case "MGluttony":
                BuffTypeNum = (int)EBuffType.Gluttony;
                break;
            case "MSloth":
                BuffTypeNum = (int)EBuffType.Sloth;
                break;
            case "MWrath":
                BuffTypeNum = (int)EBuffType.Wrath;
                break;
            case "MCharging":
                BuffTypeNum = (int)EBuffType.Charging;
                break;
        }
        BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo(BuffTypeNum).BuffName;
        BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo(BuffTypeNum).BuffDetail;
        //Debug.Log($"마우스 올라감: {id}");
    }

    private void OnLinkExit(string id)
    {
        BuffDetailExplainObject.SetActive(false);
        // 예: 원래 색상 복구, 툴팁 숨기기 등
        //Debug.Log($"마우스 벗어남: {id}");
    }

    public void ActiveEquipmentDetailInfoUI(EquipmentInfo EquipInfo, int EquipmentCode, bool IsPlayerEquipment)
    {
        //여기는 마지막에 불러와지는 함수여서..... 그냥 async로 바꾸고 받아 와도 될것 같기두.....
        //이 함수를 async로 만들고 EquipInfo가 아니라 코드를 받아오고
        //여기서 EquipmentInfo에 대한 정보를 받는다?
        gameObject.SetActive(true);

        foreach(GameObject obj in CardContainers)
        {
            obj.SetActive(false);
        }

        //int EquipCode = (EquipInfo.EquipmentType * 10000) + (EquipInfo.EquipmentTier * 1000) + EquipInfo.EquipmentCode;
        if (EquipmentCode == 0)
            return;

        gameObject.GetComponent<RectTransform>().localScale = Vector2.zero;
        gameObject.GetComponent<RectTransform>().DOScale(Vector2.one, 0.5f).SetEase(Ease.OutBack);
        ClickEquipImage.sprite = EquipInfo.EquipmentImage;
        ClickEquipName.text = EquipInfo.EquipmentName;
        ClickEquipSpendSTA.text = "";

        StartCoroutine(LoadEquipSpendSTA(EquipInfo.EquipmentType, (int)EquipInfo.SpendTiredness, EquipInfo.EquipmentCode));
        //장비의 추가 스탯
        ClickEquipAddSTR.text = EquipInfo.AddSTRAmount.ToString();
        ClickEquipAddDUR.text = EquipInfo.AddDURAmount.ToString();
        ClickEquipAddRES.text = EquipInfo.AddRESAmount.ToString();
        ClickEquipAddSPD.text = EquipInfo.AddSPDAmount.ToString();
        ClickEquipAddLUK.text = EquipInfo.AddLUKAmount.ToString();
        //장비의 설명창
        ClickEquipDetailText.text = EquipInfo.EquipmentDetail.ToString();
        //이 밑에꺼는 나중에 버튼 클릭하면 열리게 설정만 해놓고
        //얘의 부모를 꺼놓음
        ClickEquipCardButtonDownArrow.SetActive(true);
        ClickEquipCardButtonUpArrow.SetActive(false);
        ClickEquipCardContainer.SetActive(false);
        for (int i = 0; i < EquipInfo.EquipmentSlots.Length; i++)
        {
            //활성화
            CardContainers[i].SetActive(true);
            CardContainers[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(EquipInfo.EquipmentSlots[i].SlotState);
        }
    }

    private IEnumerator LoadEquipSpendSTA(int EquipType, int SpendSTAAmount, int EquipCode)
    {
        yield return LocalizationSettings.InitializationOperation;

        string EquipSTATableKey = "";

        if (EquipCode >= 70000 && EquipCode < 80000)
            EquipSTATableKey = "PS_SC_MonWeaponSTA";
        else if (EquipCode >= 80000 && EquipCode < 90000)
            EquipSTATableKey = "PS_SC_MonArmorSTA";
        else if (EquipCode >= 90000 && EquipCode < 100000)
            EquipSTATableKey = "PS_SC_MonAccSTA";
        else
        {
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
        }

        if (EquipSTATableKey == "")
            EquipSTATableKey = "PS_SC_EquipSTAError";

        var TutorialTable = LocalizationSettings.StringDatabase.GetTable("PlaySceneShortText");
        ClickEquipSpendSTA.text = TutorialTable.GetEntry(EquipSTATableKey).GetLocalizedString();

        if(EquipCode == 0)
        {//여기에서 사용하는 STA 기록
            if (EquipType == (int)EEquipType.TypeWeapon)
                ClickEquipSpendSTA.text += SpendSTAAmount.ToString();
            else if (EquipType == (int)EEquipType.TypeArmor)
                ClickEquipSpendSTA.text += SpendSTAAmount.ToString();
        }
    }

    public void InActiveEquipmentDetailInfoUI()
    {
        if (gameObject.activeSelf == false)
            return;

        SoundManager.Instance.PlayUISFX("UI_Button");
        SetClickEquipCardContainer(true);
        gameObject.GetComponent<RectTransform>().localScale = Vector2.one;
        gameObject.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => { gameObject.SetActive(false); });
    }

    public void SetClickEquipCardContainer(bool IsInActiveEquipmentDetailInfo = false)
    {
        if(IsInActiveEquipmentDetailInfo == false)
            SoundManager.Instance.PlayUISFX("UI_Button");

        if (ClickEquipCardContainer.activeSelf == true || IsInActiveEquipmentDetailInfo == true)
        {//켜져있을때는 끄고
            ClickEquipCardButtonDownArrow.SetActive(true);
            ClickEquipCardButtonUpArrow.SetActive(false);
            ClickEquipCardContainer.SetActive(false);
        }
        else if(ClickEquipCardContainer.activeSelf == false)
        {//꺼져있을때는 키고
            ClickEquipCardButtonDownArrow.SetActive(false);
            ClickEquipCardButtonUpArrow.SetActive(true);
            ClickEquipCardContainer.SetActive(true);
        }
    }
}
