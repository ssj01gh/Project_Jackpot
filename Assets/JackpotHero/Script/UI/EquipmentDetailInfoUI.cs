using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        switch (id)
        {
            case "STRWE":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.DefenseDebuff).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.DefenseDebuff).BuffDetail;
                break;
            case "STRAR":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffDetail;
                break;
            case "STRHE":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Recharge).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Recharge).BuffDetail;
                break;
            case "STRBO":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffDetail;
                break;
            case "STRAC":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughFist).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughFist).BuffDetail;
                break;
            case "DURWE":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.AttackDebuff).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.AttackDebuff).BuffDetail;
                break;
            case "DURAR":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnbreakableArmor).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnbreakableArmor).BuffDetail;
                break;
            case "DURHE":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffDetail;
                break;
            case "DURAC":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughSkin).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughSkin).BuffDetail;
                break;
            case "RESWE01":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Poison).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Poison).BuffDetail;
                break;
            case "RESWE02":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Burn).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Burn).BuffDetail;
                break;
            case "RESAR":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Misfortune).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Misfortune).BuffDetail;
                break;
            case "RESHE":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffDetail;
                break;
            case "RESBO":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Slow).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Slow).BuffDetail;
                break;
            case "RESAC":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CurseOfDeath).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CurseOfDeath).BuffDetail;
                break;
            case "SPDWE":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ChainAttack).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ChainAttack).BuffDetail;
                break;
            case "SPDAR":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffDetail;
                break;
            case "SPDHE":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffDetail;
                break;
            case "SPDBO":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Haste).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Haste).BuffDetail;
                break;
            case "SPDAC":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CorruptSerum).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CorruptSerum).BuffDetail;
                break;
            case "LUKBO":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Luck).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Luck).BuffDetail;
                break;
            case "HPWE":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.LifeSteal).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.LifeSteal).BuffDetail;
                break;
            case "HPAR":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffDetail;
                break;
            case "HPHE":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Charm).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Charm).BuffDetail;
                break;
            case "HPBO":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffDetail;
                break;
            case "HPAC":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.PowerOfDeath).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.PowerOfDeath).BuffDetail;
                break;
            case "STABO":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffDetail;
                break;
            case "STAAC":
                BuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffName;
                BuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffDetail;
                break;
        }
        //Debug.Log($"마우스 올라감: {id}");
    }

    private void OnLinkExit(string id)
    {
        BuffDetailExplainObject.SetActive(false);
        // 예: 원래 색상 복구, 툴팁 숨기기 등
        //Debug.Log($"마우스 벗어남: {id}");
    }

    public void ActiveEquipmentDetailInfoUI(EquipmentInfo EquipInfo, bool IsPlayerEquipment)
    {
        gameObject.SetActive(true);

        foreach(GameObject obj in CardContainers)
        {
            obj.SetActive(false);
        }

        int EquipCode = (EquipInfo.EquipmentType * 10000) + (EquipInfo.EquipmentTier * 1000) + EquipInfo.EquipmentCode;
        if (EquipCode == 0)
            return;

        gameObject.GetComponent<RectTransform>().localScale = Vector2.zero;
        gameObject.GetComponent<RectTransform>().DOScale(Vector2.one, 0.5f).SetEase(Ease.OutBack);
        ClickEquipImage.sprite = EquipInfo.EquipmentImage;
        ClickEquipName.text = EquipInfo.EquipmentName;

        if(IsPlayerEquipment == true)
        {
            if (EquipInfo.EquipmentType == (int)EEquipType.TypeWeapon)
                ClickEquipSpendSTA.text = "공격시 피로도 : " + EquipInfo.SpendTiredness.ToString();
            else if (EquipInfo.EquipmentType == (int)EEquipType.TypeArmor)
                ClickEquipSpendSTA.text = "방어시 피로도 : " + EquipInfo.SpendTiredness.ToString();
            else if (EquipInfo.EquipmentType == (int)EEquipType.TypeHelmet)
                ClickEquipSpendSTA.text = "피로도 회복시 피로도는 사용되지 않습니다.";
            else if (EquipInfo.EquipmentType == (int)EEquipType.TypeBoots)
                ClickEquipSpendSTA.text = "신발은 피로도를 사용하지 않습니다.";
            else if (EquipInfo.EquipmentType == (int)EEquipType.TypeAcc)
                ClickEquipSpendSTA.text = "장신구는 피로도를 사용하지 않습니다.";
            else
                ClickEquipSpendSTA.text = "??시 피로도 : " + EquipInfo.SpendTiredness.ToString();

            ClickEquipAddSTR.text = EquipInfo.AddSTRAmount.ToString();
            ClickEquipAddDUR.text = EquipInfo.AddDURAmount.ToString();
            ClickEquipAddRES.text = EquipInfo.AddRESAmount.ToString();
            ClickEquipAddSPD.text = EquipInfo.AddSPDAmount.ToString();
            ClickEquipAddLUK.text = EquipInfo.AddLUKAmount.ToString();
        }
        else
        {
            EquipCode = EquipInfo.EquipmentCode;

            if(EquipCode >= 70000 && EquipCode < 80000)
            {
                ClickEquipSpendSTA.text = "해당 몬스터의 공격입니다.";
            }
            else if(EquipCode >= 80000 && EquipCode < 90000)
            {
                ClickEquipSpendSTA.text = "해당 몬스터의 방어입니다.";
            }
            else if(EquipCode >= 90000 && EquipCode < 100000)
            {
                ClickEquipSpendSTA.text = "해당 몬스터의 특수한 행동입니다.";
            }
            else
            {

            }
        }
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
