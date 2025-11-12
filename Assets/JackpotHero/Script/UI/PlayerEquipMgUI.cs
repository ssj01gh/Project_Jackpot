using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerEquipMgUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public PlayerManager PlayerMgr;
    [Header("EquipManagement")]
    public GameObject PlayerEquip;
    public GameObject EquipInventory;
    public GameObject EquipDetailInfo_Equip;
    public GameObject EquipDetailInfo_Inven;
    public GameObject EquipGambling;
    [Header("EquipMg_PlayerEquip")]
    public GameObject[] PlayerEquipSlots;
    public Image[] PlayerEquipImages;
    public TextMeshProUGUI[] PlayerEquipTierText;
    [Header("EquipMg_EquipInventory")]
    public GameObject[] InventorySlots;
    public Image[] InventorySlotsImage;
    public GameObject[] LockObjects;
    public TextMeshProUGUI[] InventorySlotTierTexts;
    [Header("EquipMg_EquipDetailInfo_Equip")]
    public Image EquipDetail_EquipImage;
    public TextMeshProUGUI EquipDetail_EquipTierText;
    public TextMeshProUGUI EquipDetail_EquipNameText;
    public TextMeshProUGUI EquipDetail_EquipSTAText;
    public TextMeshProUGUI EquipDetail_EquipSTRText;
    public TextMeshProUGUI EquipDetail_EquipDURText;
    public TextMeshProUGUI EquipDetail_EquipRESText;
    public TextMeshProUGUI EquipDetail_EquipSPDText;
    public TextMeshProUGUI EquipDetail_EquipLUKText;
    public GameObject[] EquipDetail_EquipCardContainer;
    public TextMeshProUGUI EquipDetail_EquipDetailText;
    [Header("EquipMg_EquipDetailInfo_Inven")]
    public Image EquipDetail_InvenImage;
    public TextMeshProUGUI EquipDetail_InvenTierText;
    public TextMeshProUGUI EquipDetail_InvenNameText;
    public TextMeshProUGUI EquipDetail_InvenSTAText;
    public TextMeshProUGUI EquipDetail_InvenSTRText;
    public TextMeshProUGUI EquipDetail_InvenDURText;
    public TextMeshProUGUI EquipDetail_InvenRESText;
    public TextMeshProUGUI EquipDetail_InvenSPDText;
    public TextMeshProUGUI EquipDetail_InvenLUKText;
    public GameObject[] EquipDetail_InvenCardContainer;
    public TextMeshProUGUI EquipDetail_InvenDetailText;
    [Header("EqupMg_EquipGambling")]
    public TextMeshProUGUI EXPAmountText;
    public Button EquipGamblingButton;
    public TextMeshProUGUI EquipGamblingButtonText;
    public Button EquipGamblingLevelUpButton;
    public TextMeshProUGUI EquipGamblingLevelUPButtonText;
    public TextMeshProUGUI[] EquipGamblingPercentTexts;
    [Header("EquipMg_MouseFollowImage")]
    public Image MouseFollowImage;
    [Header("EquipMg_EquipGacha")]
    public GameObject EquipGachaObject;
    public GameObject EquipGachaEquipmentObject;
    public GameObject EquipGachaCapsule;
    public GameObject ClickButton;
    public GameObject GetEquipClickButton;
    public Image EquipGachaResultImage;
    public GameObject[] EquipGachaLight;


    protected Color InventoryActiveColor = new Color(0.28f, 0.19f, 0.1f, 1f);
    protected Color InventoryUnActiveColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);

    protected Color[] GachaTierLightColor = new Color[]
    {
        new Color(1f,1f,1f),
        new Color(0f,0.84f,1f),
        new Color(0.87f,0f,1f),
        new Color(1,0.92f,0f),
        new Color(0.24f,1f,0f),
        new Color(1f,0.71f,0.11f)
    };

    protected Vector3[] GachaLightRot = new Vector3[] 
    {
        new Vector3(0,0,0),
        new Vector3(0,0,-60),
        new Vector3(0,0,-120),
        new Vector3(0,0,-180),
        new Vector3(0,0,-240),
        new Vector3(0,0,-300)
    };

    //플레이어 장비창 좌표들
    protected Vector2 PlayerEquipInitPos = new Vector2(-1460, 740);
    protected Vector2 PlayerEquipTargetPos = new Vector2(-460,340);
    //플레이어 장비 상세 정보 좌표들
    protected Vector2 EquipedEquipDetailInitPos = new Vector2(-1210, -730);
    protected Vector2 EquipedEquipDetailTargetPos = new Vector2(-710,-50);
    //플레이어가 클릭한 장비 상세 정보 좌표들
    protected Vector2 ClickedInvenDetailInitPos = new Vector2(-710,-730);
    protected Vector2 ClickedInvenDetailTargetPos = new Vector2(-210,-50);
    //인벤토리 좌표들
    protected Vector2 InvenInitPos = new Vector2(1420, 890);
    protected Vector2 InvenTargetPos = new Vector2(500, 190);
    //갓챠창 좌표들
    protected Vector2 GamblingInitPos = new Vector2(1420, -730);
    protected Vector2 GamblingTargetPos = new Vector2(500, -350);
    //요 위의 좌표들에 정보 입력

    protected const float GachaLightZVariation = 10f;

    protected bool IsClickedInventorySlot = false;

    protected int PlayerEquipIndex = -1;

    protected int CurrentClickedSlotIndex = 0;
    protected int CurrentBringItemCode = 0;
    protected int DropDownSlotIndex = 0;
    protected int DropDownItemCode = 0;

    protected int GachaResultEquipCode = 0;
    protected enum EPlayerEquip
    {
        Helmet,
        Armor,
        Boots,
        Weapon,
        Accessories
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerEquip.SetActive(false);
        EquipInventory.SetActive(false);
        EquipDetailInfo_Equip.SetActive(false);
        EquipDetailInfo_Inven.SetActive(false);
        EquipGambling.SetActive(false);
        MouseFollowImage.gameObject.SetActive(false);
        EquipGachaObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivePlayerEquipMg()
    {
        if(PlayerEquip.activeSelf == true && EquipInventory.activeSelf == true && EquipDetailInfo_Equip.activeSelf == true &&
            EquipDetailInfo_Inven.activeSelf == true && EquipGambling.activeSelf == true)
        {
            return;//다 켜져 있으면 리턴 때리기
        }
        gameObject.SetActive(true);
        //SetPlayerEquip
        //다껏다가 해당되는거만 키기
        for(int i = 0; i < PlayerEquipImages.Length; i++)
        {
            PlayerEquipImages[i].gameObject.SetActive(false);
            PlayerEquipTierText[i].text = "";
        }
        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Helmet].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Helmet].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Helmet].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode);
        }
        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Armor].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Armor].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Armor].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode);
        }
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Boots].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Boots].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Boots].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode);
        }
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Weapon].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Weapon].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Weapon].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode);
        }
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Accessories].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Accessories].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Accessories].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode);
        }
        PlayerEquip.GetComponent<RectTransform>().anchoredPosition = PlayerEquipInitPos;
        PlayerEquip.SetActive(true);
        PlayerEquip.GetComponent<RectTransform>().DOAnchorPos(PlayerEquipTargetPos, 0.5f);
        //SetEquipInventory
        SetInventory(true);
        //SetEquipDetailInfo_Equip//Detail의 초기상태는 비어있는 거임//후에 플레이어의 조작에 따라 정보 표시
        EquipDetail_EquipImage.gameObject.SetActive(false);
        EquipDetail_EquipTierText.text = "";
        EquipDetail_EquipNameText.text = "";
        EquipDetail_EquipSTAText.text = "";
        EquipDetail_EquipSTRText.text = "";
        EquipDetail_EquipDURText.text = "";
        EquipDetail_EquipRESText.text = "";
        EquipDetail_EquipSPDText.text = "";
        EquipDetail_EquipLUKText.text = "";
        foreach(GameObject Obj in EquipDetail_EquipCardContainer)
        {
            Obj.SetActive(false);
        }
        EquipDetail_EquipDetailText.text = "";
        EquipDetailInfo_Equip.GetComponent<RectTransform>().anchoredPosition = EquipedEquipDetailInitPos;
        EquipDetailInfo_Equip.SetActive(true);
        EquipDetailInfo_Equip.GetComponent<RectTransform>().DOAnchorPos(EquipedEquipDetailTargetPos, 0.5f);
        //SetEquipDetailInfo_Inven//Detail의 초기상태는 비어있는 거임//후에 플레이어의 조작에 따라 정보 표시
        EquipDetail_InvenImage.gameObject.SetActive(false);
        EquipDetail_InvenTierText.text = "";
        EquipDetail_InvenNameText.text = "";
        EquipDetail_InvenSTAText.text = "";
        EquipDetail_InvenSTRText.text = "";
        EquipDetail_InvenDURText.text = "";
        EquipDetail_InvenRESText.text = "";
        EquipDetail_InvenSPDText.text = "";
        EquipDetail_InvenLUKText.text = "";
        foreach (GameObject Obj in EquipDetail_InvenCardContainer)
        {
            Obj.SetActive(false);
        }
        EquipDetail_InvenDetailText.text = "";
        EquipDetailInfo_Inven.GetComponent<RectTransform>().anchoredPosition = ClickedInvenDetailInitPos;
        EquipDetailInfo_Inven.SetActive(true);
        EquipDetailInfo_Inven.GetComponent<RectTransform>().DOAnchorPos(ClickedInvenDetailTargetPos, 0.5f);
        //SetEquipGambling
        SetGambling(true);

    }

    public void InActivePlayerEquipMg()// 비활성화 됬을때 저장
    {
        if(PlayerEquip.activeSelf == true)
        {
            PlayerEquip.GetComponent<RectTransform>().anchoredPosition = PlayerEquipTargetPos;
            PlayerEquip.GetComponent<RectTransform>().DOAnchorPos(PlayerEquipInitPos, 0.5f).OnComplete(() => { PlayerEquip.SetActive(false); });
        }
        if(EquipInventory.activeSelf == true)
        {
            EquipInventory.GetComponent<RectTransform>().anchoredPosition = InvenTargetPos;
            EquipInventory.GetComponent<RectTransform>().DOAnchorPos(InvenInitPos, 0.5f).OnComplete(() => { EquipInventory.SetActive(false); });
        }
        if(EquipDetailInfo_Equip.activeSelf == true)
        {
            EquipDetailInfo_Equip.GetComponent<RectTransform>().anchoredPosition = EquipedEquipDetailTargetPos;
            EquipDetailInfo_Equip.GetComponent<RectTransform>().DOAnchorPos(EquipedEquipDetailInitPos, 0.5f).OnComplete(() => { EquipDetailInfo_Equip.SetActive(false); });
        }
        if(EquipDetailInfo_Inven.activeSelf == true)
        {
            EquipDetailInfo_Inven.GetComponent<RectTransform>().anchoredPosition = ClickedInvenDetailTargetPos;
            EquipDetailInfo_Inven.GetComponent<RectTransform>().DOAnchorPos(ClickedInvenDetailInitPos, 0.5f).OnComplete(() => { EquipDetailInfo_Inven.SetActive(false); });
        }
        if(EquipGambling.activeSelf == true)
        {
            EquipGambling.GetComponent<RectTransform>().anchoredPosition = GamblingTargetPos;
            EquipGambling.GetComponent<RectTransform>().DOAnchorPos(GamblingInitPos, 0.5f).OnComplete(() => { EquipGambling.SetActive(false); gameObject.SetActive(false); });
        }

        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    protected string GetTierText(int Code)
    {
        if ((Code / 1000) % 10 == 7)
        {
            switch (Code)
            {
                case 17001:
                    return "3티어";
                case 17002:
                    return "4티어";
                case 17003:
                    return "5티어";
                case 17004:
                    return "6티어";
            }
            return "1티어";
        }
        else
            return ((Code / 1000) % 10).ToString() + "티어";
    }

    protected void SetGambling(bool IsActive = false)//->이거는 무언가 행동 될때마다 계속 업데이트 해야할듯?
    {//일단은 장비를 집고 놨을때(OnPointerUp일때 한번해야함)(장비 뽑고 나서도 그렇고)
        EXPAmountText.text = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience.ToString();
        //뽑기 레벨업 버튼
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel >= 10)
        {//만랩일 경우
            EquipGamblingLevelUpButton.interactable = false;
            EquipGamblingLevelUPButtonText.text = "MaxLevel";
        }
        else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 
            EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel))
        {//경험치가 부족할 경우
            EquipGamblingLevelUpButton.interactable = false;
            EquipGamblingLevelUPButtonText.text = "경험치 부족\r\n" +
                "필요 : " + EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel) + "경험치";
        }
        else//만랩보다 작을경우
        {
            EquipGamblingLevelUpButton.interactable = true;
            EquipGamblingLevelUPButtonText.text = "장비 뽑기 강화\r\n" + 
                EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel) + " 경험치";
        }

        //뽑기 버튼
        if(PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)
        {//인벤토리가 꽉찼다면
            EquipGamblingButton.interactable = false;
            EquipGamblingButtonText.text = "인벤토리\r\n공간 부족";
        }
        else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 
            EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel))
        {//경험치가 부족할경우
            EquipGamblingButton.interactable = false;
            EquipGamblingButtonText.text = "경험치 부족\r\n" +
                "필요 : " + EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel) + "경험치";
        }
        else
        {
            EquipGamblingButton.interactable = true;
            EquipGamblingButtonText.text = "장비 뽑기\r\n" + 
                EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel) + " 경험치";
        }


        //EquipGamblingPercentTexts
        for (int i = 0; i < EquipGamblingPercentTexts.Length; i++)
        {
            EquipGamblingPercentTexts[i].text = EquipmentInfoManager.Instance.GetGamblingLevelPercent(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel, i) + "%";
        }

        if (IsActive == true)
        {
            EquipGambling.GetComponent<RectTransform>().anchoredPosition = GamblingInitPos;
            EquipGambling.SetActive(true);
            EquipGambling.GetComponent<RectTransform>().DOAnchorPos(GamblingTargetPos, 0.5f);
        }
    }

    protected void SetInventory(bool IsActive = false)
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            InventorySlots[i].GetComponent<Image>().color = InventoryUnActiveColor;
            InventorySlotsImage[i].gameObject.SetActive(false);
            LockObjects[i].SetActive(true);
            InventorySlotTierTexts[i].text = "";
        }

        int CanUseInventory = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");
        for (int i = 0; i < CanUseInventory; i++)
        {
            InventorySlots[i].GetComponent<Image>().color = InventoryActiveColor;
            LockObjects[i].SetActive(false);
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//비어있지 않을때
            {
                //코드에 맞는 이미지를 넣음
                InventorySlotsImage[i].gameObject.SetActive(true);
                InventorySlotsImage[i].sprite =
                    EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]).EquipmentImage;
                InventorySlotTierTexts[i].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]);
            }
        }
        if(IsActive == true)
        {
            EquipInventory.GetComponent<RectTransform>().anchoredPosition = InvenInitPos;
            EquipInventory.SetActive(true);
            EquipInventory.GetComponent<RectTransform>().DOAnchorPos(InvenTargetPos, 0.5f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerEquipIndex = -1;
        if (eventData.pointerEnter != null && eventData.pointerEnter.tag == "InventorySlot")//여기서 들어오는 놈이 몇번째 Slot인지 알아야함
        {
            Vector2 ClickedUIPos = eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition;
            for(int i = 0; i <PlayerEquipSlots.Length; i++)
            {
                if(Vector2.Distance(ClickedUIPos, PlayerEquipSlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                {
                    switch (i)
                    {
                        case (int)EPlayerEquip.Helmet:
                            if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
                            break;
                        case (int)EPlayerEquip.Armor:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
                            break;
                        case (int)EPlayerEquip.Boots:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
                            break;
                        case (int)EPlayerEquip.Weapon:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
                            break;
                        case (int)EPlayerEquip.Accessories:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode;
                            break;
                    }
                    SoundManager.Instance.PlayUISFX("Item_PickUp");
                    PlayerEquipIndex = i;
                    IsClickedInventorySlot = true;

                    MouseFollowImage.gameObject.SetActive(true);
                    Debug.Log(CurrentBringItemCode);
                    MouseFollowImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                    MoveUI(eventData);
                    PlayerEquipImages[i].gameObject.SetActive(false);
                    PlayerEquipTierText[i].text = "";
                    DisplayEquipDetailInfo(true);
                    //장비칸 아이템 클릭
                    break;
                }
            }
            
            if(PlayerEquipIndex == -1)//만약 장비창중에 클릭한 슬롯이 없다면 인벤토리도 검사
            {
                //인벤토리 슬롯인지 검사
                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                    {
                        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0 && LockObjects[i].activeSelf == false)//비어있지 않을때//잠겨있지 않을때
                        {
                            SoundManager.Instance.PlayUISFX("Item_PickUp");
                            IsClickedInventorySlot = true;
                            //들고 있는 아이템의 정보 저장
                            CurrentClickedSlotIndex = i;
                            CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i];
                            //마우스를 따라다니는 이미지를 마우스 위치에 위치시키고 이미지를 바꿈
                            MouseFollowImage.gameObject.SetActive(true);
                            MouseFollowImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                            MoveUI(eventData);
                            //클릭한 슬롯의 이미지를 잠시 꺼둠
                            InventorySlotsImage[i].gameObject.SetActive(false);
                            //티어 텍스트도 잠시 꺼둠
                            InventorySlotTierTexts[i].text = "";
                        }
                        //인벤토리칸 아이템 클릭
                        DisplayEquipDetailInfo(false);
                        break;
                    }
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsClickedInventorySlot == true)
        {
            MoveUI(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsClickedInventorySlot == true && eventData.pointerEnter != null)
        {
            IsClickedInventorySlot = false;
            Vector2 ClickedUIPos = eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition;

            if (PlayerEquipIndex != -1)//장비 칸에서 클릭했을때//장비칸에서 인벤토리 OR 장비칸
            {
                if (eventData.pointerEnter.tag == "InventorySlot")//마우스를 놓은 곳이 슬롯일때
                {
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    bool IsEquipmentSlot = true;
                    //인벤토리 칸인지 검사
                    for (int i = 0; i < InventorySlots.Length; i++)
                    {
                        if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                        {
                            //여기에 한번이라도 들어갔다는것 = 마우스를 땐 슬롯이 장비 슬롯이 아님
                            IsEquipmentSlot = false;
                            if (LockObjects[i].activeSelf == true)//잠겨 있을때
                            {
                                //원래 위치로
                                PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                                PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                            }
                            else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//비어있지 않을떄//잠겨 있지 않으면서 비어있지 않을때
                            {
                                if(CurrentBringItemCode / 10000 == 
                                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] / 10000)//비어있지 않았을때 장비의 타입이 같다면
                                {//바꾸기
                                    //떨어뜨리는 곳의 index번호와 장비 코드를 저장
                                    DropDownSlotIndex = i;
                                    DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i];
                                    //떨어뜨린 곳의 슬롯에 집었던 장비 덮어 쓰기
                                    InventorySlotsImage[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                    InventorySlotTierTexts[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex] = CurrentBringItemCode;
                                    //집었던 슬롯에 떨어뜨린곳의 장비 넣기
                                    PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                                    PlayerEquipImages[PlayerEquipIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                                    PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(DropDownItemCode);
                                    switch(PlayerEquipIndex)
                                    {
                                        case (int)EPlayerEquip.Helmet:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = DropDownItemCode;
                                            break;
                                        case (int)EPlayerEquip.Armor:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = DropDownItemCode;
                                            break;
                                        case (int)EPlayerEquip.Boots:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = DropDownItemCode;
                                            break;
                                        case (int)EPlayerEquip.Weapon:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = DropDownItemCode;
                                            break;
                                        case (int)EPlayerEquip.Accessories:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = DropDownItemCode;
                                            break;
                                    }
                                }
                                else//같지 않다면
                                {//원래 위치로
                                    PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                                    PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                                }
                            }
                            else//비어있다면
                            {
                                //비어있는 곳에 넣고
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = CurrentBringItemCode;
                                InventorySlotsImage[i].gameObject.SetActive(true);
                                InventorySlotsImage[i].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                InventorySlotTierTexts[i].text = GetTierText(CurrentBringItemCode);
                                //장비 칸은 비우고
                                //이미 UI상으로는 비워져 있음
                                switch (PlayerEquipIndex)
                                {
                                    case (int)EPlayerEquip.Helmet:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = 0;
                                        break;
                                    case (int)EPlayerEquip.Armor:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = 0;
                                        break;
                                    case (int)EPlayerEquip.Boots:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = 0;
                                        break;
                                    case (int)EPlayerEquip.Weapon:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = 0;
                                        break;
                                    case (int)EPlayerEquip.Accessories:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = 0;
                                        break;
                                }
                            }
                            break;
                        }
                    }

                    if(IsEquipmentSlot == true)//장비 슬롯이엿다면
                    {//원래 자리로
                        PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                        PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                    }
                }
                else if (eventData.pointerEnter.name == "TrashCan")//마우스를 놓은 곳이 쓰레기 통일때
                {
                    SoundManager.Instance.PlayUISFX("Item_Remove");
                    //비우기
                    switch (PlayerEquipIndex)
                    {
                        case (int)EPlayerEquip.Helmet:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = 0;
                            break;
                        case (int)EPlayerEquip.Armor:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = 0;
                            break;
                        case (int)EPlayerEquip.Boots:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = 0;
                            break;
                        case (int)EPlayerEquip.Weapon:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = 0;
                            break;
                        case (int)EPlayerEquip.Accessories:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = 0;
                            break;
                    }
                }
                else//뭣도 아닐때
                {//원래 자리로
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                    PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                }
            }
            else//클릭(PointDown)한 슬롯이 인벤토리 슬롯일때//인벤토리에서 장비칸 OR 인벤토리로
            {
                if (eventData.pointerEnter.tag == "InventorySlot")//마우스를 놓은 곳이 슬롯일때
                {
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    bool IsEquipmentSlot = true;
                    //인벤토리 칸인지 검사
                    for (int i = 0; i < InventorySlots.Length; i++)
                    {
                        if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                        {
                            //여기에 한번이라도 들어갔다는것 = 마우스를 땐 슬롯이 장비 슬롯이 아님
                            IsEquipmentSlot = false;
                            if (LockObjects[i].activeSelf == true)//잠겨 있을때
                            {
                                //원래 위치로
                                InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                            }
                            else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//비어있지 않을떄//잠겨 있지 않으면서 비어있지 않을때
                            {
                                //떨어뜨리는 곳의 번호와 장비코드를 저장
                                DropDownSlotIndex = i;
                                DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex];
                                //떨어뜨린 곳의 슬롯에 집었던 장비를 덮어 쓰기
                                InventorySlotsImage[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                InventorySlotTierTexts[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex] = CurrentBringItemCode;
                                //장비를 집었던 슬롯에 떨어뜨린 곳의 장비를 덮어 쓰기
                                InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                InventorySlotsImage[CurrentClickedSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                                InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(DropDownItemCode);
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = DropDownItemCode;
                            }
                            else//비어있다면
                            {
                                //비어있는 곳에 넣고
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = CurrentBringItemCode;
                                InventorySlotsImage[i].gameObject.SetActive(true);
                                InventorySlotsImage[i].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                InventorySlotTierTexts[i].text = GetTierText(CurrentBringItemCode);
                                //원래 칸은 비우고
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                            }
                            break;
                        }
                    }

                    if (IsEquipmentSlot == true)//장비 슬롯이엿다면
                    {
                        for(int i = 0; i < PlayerEquipSlots.Length; i++)
                        {
                            if(Vector2.Distance(ClickedUIPos, PlayerEquipSlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                            {//장비칸에서 클릭이 때졌을때
                                DropDownSlotIndex = i;
                                switch (i)
                                {
                                    case (int)EPlayerEquip.Helmet:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
                                        break;
                                    case (int)EPlayerEquip.Armor:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
                                        break;
                                    case (int)EPlayerEquip.Boots:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
                                        break;
                                    case (int)EPlayerEquip.Weapon:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
                                        break;
                                    case (int)EPlayerEquip.Accessories:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode;
                                        break;
                                }

                                if(DropDownItemCode == 0)//비어있다면//맞게 넣어야 하는디.....
                                {
                                    if(CurrentBringItemCode / 10000 == 1 && DropDownSlotIndex == (int)EPlayerEquip.Weapon)
                                    {//CurrentBringItemCode의 앞자리가 1 : 무기 -> 1이라면 인덱스 3번 칸일때 ok -> 넣기
                                        //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if(CurrentBringItemCode / 10000 == 2 && DropDownSlotIndex == (int)EPlayerEquip.Armor)
                                    {//CurrentBringItemCode의 앞자리가 2 : 갑옷 -> 2라면 인덱스 1번 칸일때 ok -> 넣기
                                        //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if(CurrentBringItemCode / 10000 == 3 && DropDownSlotIndex == (int)EPlayerEquip.Helmet)
                                    {//CurrentBringItemCode의 앞자리가 3 : 투구 -> 3이라면 인덱스 0번 칸일때 ok -> 넣기
                                        //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if(CurrentBringItemCode / 10000 == 4 && DropDownSlotIndex == (int)EPlayerEquip.Boots)
                                    {//CurrentBringItemCode의 앞자리가 4 : 신발 -> 4라면 인덱스 2번 칸일때 ok -> 넣기
                                        //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if(CurrentBringItemCode / 10000 == 5 && DropDownSlotIndex == (int)EPlayerEquip.Accessories)
                                    {//CurrentBringItemCode의 앞자리가 5 : 장신구 -> 5라면 인덱스 4번 칸 일때 ok -> 넣기
                                        //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else
                                    {//위에꺼에 해당하지 않는다면 원래 자리로
                                        InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                        InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                                    }
                                }
                                else if(CurrentBringItemCode / 10000 == DropDownItemCode / 10000)//같은 타입의 장비라면 교체
                                {//바꾸기
                                    //인벤토리 장비 -> 착용 장비
                                    PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                    PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                    PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                    switch(DropDownSlotIndex)
                                    {
                                        case (int)EPlayerEquip.Helmet:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = CurrentBringItemCode;
                                            break;
                                        case (int)EPlayerEquip.Armor:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = CurrentBringItemCode;
                                            break;
                                        case (int)EPlayerEquip.Boots:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = CurrentBringItemCode;
                                            break;
                                        case (int)EPlayerEquip.Weapon:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = CurrentBringItemCode;
                                            break;
                                        case (int)EPlayerEquip.Accessories:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = CurrentBringItemCode;
                                            break;
                                    }
                                    //착용 장비 -> 인벤토리 장비
                                    InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                    InventorySlotsImage[CurrentClickedSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(DropDownItemCode);
                                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = DropDownItemCode;
                                }
                                else
                                {//뭣도 아니면 원래 자리로
                                    InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                                }
                                break;
                            }
                        }
                        /*
                        PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                        PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                        */
                    }
                }
                else if (eventData.pointerEnter.name == "TrashCan")//마우스를 놓은 곳이 쓰레기 통일때
                {
                    //장비를 잡았던 슬롯 비우기
                    SoundManager.Instance.PlayUISFX("Item_Remove");
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = "";
                }
                else//뭣도 아닐때
                {//원래 자리로
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                }
            }
            MouseFollowImage.gameObject.SetActive(false);
            CurrentBringItemCode = 0;
            CurrentClickedSlotIndex = 0;
            DropDownItemCode = 0;
            DropDownSlotIndex = 0;
            SetGambling();
        }
    }

    private void MoveUI(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameObject.GetComponentInParent<Canvas>().transform as RectTransform,
            eventData.position,
            Camera.main,
        out localPoint
        );

        MouseFollowImage.transform.position = gameObject.GetComponentInParent<Canvas>().transform.TransformPoint(localPoint);
    }

    private void DisplayEquipDetailInfo(bool IsEquipedEquipment)//장비를 클릭하면 자세한 정보를 출력한다.
    {//true면 장비칸을 클릭한거고, false면 인벤토리를 클릭한거임
        if(IsEquipedEquipment == true)//장비칸에 있는 아이템 클릭
        {//-> 오른쪽 Detail만 출력하면 됨
            //CurrentBringItemCode -> 0일때는 안 들어옴
            EquipmentInfo EquipedEquipmentInfo = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode);
            EquipDetail_EquipImage.gameObject.SetActive(true);
            EquipDetail_EquipImage.sprite = EquipedEquipmentInfo.EquipmentImage;
            EquipDetail_EquipTierText.text = GetTierText(CurrentBringItemCode);
            EquipDetail_EquipNameText.text = EquipedEquipmentInfo.EquipmentName;

            if (CurrentBringItemCode >= 10000 && CurrentBringItemCode < 20000)
                EquipDetail_EquipSTAText.text = "공격시 피로도 : " + EquipedEquipmentInfo.SpendTiredness.ToString();
            else if (CurrentBringItemCode >= 20000 && CurrentBringItemCode < 30000)
                EquipDetail_EquipSTAText.text = "방어시 피로도 : " + EquipedEquipmentInfo.SpendTiredness.ToString();
            else if (CurrentBringItemCode >= 30000 && CurrentBringItemCode < 40000)
                EquipDetail_EquipSTAText.text = "피로도 회복시 피로도는 사용되지 않습니다.";
            else if (CurrentBringItemCode >= 40000 && CurrentBringItemCode < 50000)
                EquipDetail_EquipSTAText.text = "신발은 피로도를 사용하지 않습니다.";
            else if (CurrentBringItemCode >= 50000 && CurrentBringItemCode < 60000)
                EquipDetail_EquipSTAText.text = "장신구는 피로도를 사용하지 않습니다.";
            else
                EquipDetail_EquipSTAText.text = "";

            EquipDetail_EquipSTRText.text = EquipedEquipmentInfo.AddSTRAmount.ToString();
            EquipDetail_EquipDURText.text = EquipedEquipmentInfo.AddDURAmount.ToString();
            EquipDetail_EquipRESText.text = EquipedEquipmentInfo.AddRESAmount.ToString();
            EquipDetail_EquipSPDText.text = EquipedEquipmentInfo.AddSPDAmount.ToString();
            EquipDetail_EquipLUKText.text = EquipedEquipmentInfo.AddLUKAmount.ToString();
            EquipDetail_EquipDetailText.text = EquipedEquipmentInfo.EquipmentDetail.ToString();

            //활성화 하기전에 한번 다 초기화
            foreach(GameObject obj in EquipDetail_EquipCardContainer)
                obj.SetActive(false);
            for (int i = 0; i < EquipedEquipmentInfo.EquipmentSlots.Length; i++)
            {
                //활성화
                EquipDetail_EquipCardContainer[i].SetActive(true);
                EquipDetail_EquipCardContainer[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(EquipedEquipmentInfo.EquipmentSlots[i].SlotState);
            }
            //오른쪽의 정보창은 아예 비우기
            EquipDetail_InvenImage.gameObject.SetActive(false);
            EquipDetail_InvenTierText.text = "";
            EquipDetail_InvenNameText.text = "";
            EquipDetail_InvenSTAText.text = "";
            EquipDetail_InvenSTRText.text = "";
            EquipDetail_InvenDURText.text = "";
            EquipDetail_InvenRESText.text = "";
            EquipDetail_InvenSPDText.text = "";
            EquipDetail_InvenLUKText.text = "";
            foreach (GameObject Obj in EquipDetail_InvenCardContainer)
            {
                Obj.SetActive(false);
            }
        }
        else//인벤토리에 있는 아이템 클릭
        {//-> 왼쪽 Detail, 오른쪽 Detail다 출력 해야됨
         //같은 타입의 장비가 없으면 오른쪽은 출력 x
         //인벤의 장비 표시
            EquipmentInfo UnEquipedEquipment = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode);
            int EquipedEquipmentCode;
            EquipDetail_InvenImage.gameObject.SetActive(true);
            EquipDetail_InvenImage.sprite = UnEquipedEquipment.EquipmentImage;
            EquipDetail_InvenTierText.text = GetTierText(CurrentBringItemCode);
            EquipDetail_InvenNameText.text = UnEquipedEquipment.EquipmentName;

            if (CurrentBringItemCode >= 10000 && CurrentBringItemCode < 20000)
            {
                EquipDetail_InvenSTAText.text = "공격시 피로도 : " + UnEquipedEquipment.SpendTiredness.ToString();
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
            }
            else if (CurrentBringItemCode >= 20000 && CurrentBringItemCode < 30000)
            {
                EquipDetail_InvenSTAText.text = "방어시 피로도 : " + UnEquipedEquipment.SpendTiredness.ToString();
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
            }
            else if (CurrentBringItemCode >= 30000 && CurrentBringItemCode < 40000)
            {
                EquipDetail_InvenSTAText.text = "피로도 회복시 피로도는 사용되지 않습니다.";
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
            }
            else if (CurrentBringItemCode >= 40000 && CurrentBringItemCode < 50000)
            {
                EquipDetail_InvenSTAText.text = "신발은 피로도를 사용하지 않습니다.";
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
            }
            else if (CurrentBringItemCode >= 50000 && CurrentBringItemCode < 60000)
            {
                EquipDetail_InvenSTAText.text = "장신구는 피로도를 사용하지 않습니다.";
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode;
            }
            else
            {
                EquipDetail_InvenSTAText.text = "";
                EquipedEquipmentCode = 0;
            }

            EquipDetail_InvenSTRText.text = UnEquipedEquipment.AddSTRAmount.ToString();
            EquipDetail_InvenDURText.text = UnEquipedEquipment.AddDURAmount.ToString();
            EquipDetail_InvenRESText.text = UnEquipedEquipment.AddRESAmount.ToString();
            EquipDetail_InvenSPDText.text = UnEquipedEquipment.AddSPDAmount.ToString();
            EquipDetail_InvenLUKText.text = UnEquipedEquipment.AddLUKAmount.ToString();
            EquipDetail_InvenDetailText.text = UnEquipedEquipment.EquipmentDetail.ToString();

            //활성화 하기전에 한번 다 초기화
            foreach (GameObject obj in EquipDetail_InvenCardContainer)
                obj.SetActive(false);
            for (int i = 0; i < UnEquipedEquipment.EquipmentSlots.Length; i++)
            {
                //활성화
                EquipDetail_InvenCardContainer[i].SetActive(true);
                EquipDetail_InvenCardContainer[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(UnEquipedEquipment.EquipmentSlots[i].SlotState);
            }
            //같은 타입의 장비가 있거나 혹은 없거나
            EquipmentInfo EquipedEquipmentInfo = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(EquipedEquipmentCode);
            if (EquipedEquipmentCode == 0)//끼고 있는 장비가 없을때
            {
                //아예 비우기
                EquipDetail_EquipImage.gameObject.SetActive(false);
                EquipDetail_EquipTierText.text = "";
                EquipDetail_EquipNameText.text = "";
                EquipDetail_EquipSTAText.text = "";
                EquipDetail_EquipSTRText.text = "";
                EquipDetail_EquipDURText.text = "";
                EquipDetail_EquipRESText.text = "";
                EquipDetail_EquipSPDText.text = "";
                EquipDetail_EquipLUKText.text = "";
                foreach (GameObject Obj in EquipDetail_EquipCardContainer)
                {
                    Obj.SetActive(false);
                }
            }
            else//끼고 있는 장비가 있을때
            {
                EquipDetail_EquipImage.gameObject.SetActive(true);
                EquipDetail_EquipImage.sprite = EquipedEquipmentInfo.EquipmentImage;
                EquipDetail_EquipTierText.text = GetTierText(EquipedEquipmentCode);
                EquipDetail_EquipNameText.text = EquipedEquipmentInfo.EquipmentName;

                if (CurrentBringItemCode >= 10000 && CurrentBringItemCode < 20000)
                    EquipDetail_EquipSTAText.text = "공격시 피로도 : " + EquipedEquipmentInfo.SpendTiredness.ToString();
                else if (CurrentBringItemCode >= 20000 && CurrentBringItemCode < 30000)
                    EquipDetail_EquipSTAText.text = "방어시 피로도 : " + EquipedEquipmentInfo.SpendTiredness.ToString();
                else if (CurrentBringItemCode >= 30000 && CurrentBringItemCode < 40000)
                    EquipDetail_EquipSTAText.text = "피로도 회복시 피로도는 사용되지 않습니다.";
                else if (CurrentBringItemCode >= 40000 && CurrentBringItemCode < 50000)
                    EquipDetail_EquipSTAText.text = "신발은 피로도를 사용하지 않습니다.";
                else if (CurrentBringItemCode >= 50000 && CurrentBringItemCode < 60000)
                    EquipDetail_EquipSTAText.text = "장신구는 피로도를 사용하지 않습니다.";
                else
                    EquipDetail_EquipSTAText.text = "";

                EquipDetail_EquipSTRText.text = EquipedEquipmentInfo.AddSTRAmount.ToString();
                EquipDetail_EquipDURText.text = EquipedEquipmentInfo.AddDURAmount.ToString();
                EquipDetail_EquipRESText.text = EquipedEquipmentInfo.AddRESAmount.ToString();
                EquipDetail_EquipSPDText.text = EquipedEquipmentInfo.AddSPDAmount.ToString();
                EquipDetail_EquipLUKText.text = EquipedEquipmentInfo.AddLUKAmount.ToString();

                //활성화 하기전에 한번 다 초기화
                foreach (GameObject obj in EquipDetail_EquipCardContainer)
                    obj.SetActive(false);
                for (int i = 0; i < EquipedEquipmentInfo.EquipmentSlots.Length; i++)
                {
                    //활성화
                    EquipDetail_EquipCardContainer[i].SetActive(true);
                    EquipDetail_EquipCardContainer[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(EquipedEquipmentInfo.EquipmentSlots[i].SlotState);
                }
            }
        }
    }

    public void PressEquipGamblingLevelUPButton()
    {
        //경험치가 줄어든다. //경험치가 먼저 줄어들어야지 레벨에 맞는 경험치가 줄어듬
        //플레이어의 뽑기 레벨이 오른다.
        //SetGambling으로 ui를 업데이트한다.
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel), true);
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel++;
        SetGambling();
    }

    public void PressEquipGamblingGachaButton()//ActiveGacha
    {
        //경험치가 줄어든다.
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel), true);
        //초기화
        EquipGachaObject.SetActive(true);
        EquipGachaEquipmentObject.SetActive(true);//장비를 감추고 있는 캡슐과 장비 이미지를 가지고 있는 오브젝트
        EquipGachaCapsule.GetComponent<RectTransform>().localScale = Vector3.one;
        EquipGachaCapsule.GetComponent<Image>().color = Color.white;
        EquipGachaCapsule.SetActive(true);
        EquipGachaEquipmentObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 750);
        ClickButton.SetActive(false);
        GetEquipClickButton.SetActive(false);

        for(int i = 0; i < EquipGachaLight.Length; i++)
        {
            EquipGachaLight[i].GetComponent<Image>().color = GachaTierLightColor[i];
            //EquipGachaLightOutline[i].color = GachaTierLightColor[0];
            EquipGachaLight[i].SetActive(false);
        }
        /*
        foreach (GameObject obj in EquipGachaLight)
        {
            //obj.GetComponent<Image>().color = GachaTierLightColor[0];
            //obj.GetComponent<SpriteOutline>().color = GachaTierLightColor[0];
            obj.SetActive(false);
        }
        */
        
        //결과로 나오는 장비의 이미지와 인벤토리에 미리 넣어둠, 어짜피 업데이트 하지 않으면 UI(인벤토리)에서는 안보이니까
        //GetGamblingEquipmentCode <- gambling 레벨에 맞는 장비 코드 반환
        GachaResultEquipCode = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
        EquipGachaResultImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(GachaResultEquipCode).EquipmentImage;
        //이건 나중에 활성화
        PlayerMgr.GetPlayerInfo().PutEquipmentToInven(GachaResultEquipCode);
        //띄용띄용이 끝나면 클릭버튼 활성화
        EquipGachaEquipmentObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0.7f).SetEase(Ease.OutBounce).
            OnComplete(() => { ActiveGachaClickButton(); });

    }

    private void ActiveGachaClickButton()
    {
        ClickButton.SetActive(true);
        ClickButton.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -5f);
        ClickButton.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 5), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void PressGachaClickButton()
    {
        ClickButton.GetComponent<RectTransform>().DOKill();
        ClickButton.SetActive(false);
        //이 버튼을 누르면 버튼은 비활성화
        int EquipmentTier = GachaResultEquipCode / 1000;
        // 1티어 장비라면 0번까지 활성화 , 흰색// 2티어 장비라면  1번까지 활성화, 파란색
        // 3티어 장비라면 2번까지 활성화, 보라색// 4티어 장비라면 3번까지 활성화, 노란색
        // 5티어 장비라면 4번까지 활성화, 
        //미리 저장한 GachaResultEquipCode로 티어를 파악해 그것에 맞게 연출
        // 1티어 보다 낮은 장비는 없으므로 1은 바로 활성화
        EquipGachaLight[0].GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
        EquipGachaLight[0].GetComponent<RectTransform>().eulerAngles = 
            GachaLightRot[0] + new Vector3(0, 0, Random.Range(-GachaLightZVariation, GachaLightZVariation));
        EquipGachaLight[0].SetActive(true);
        PlayEquipGachaLightSound(0);
        EquipGachaLight[0].GetComponent<RectTransform>().DOScaleY(1, 0.3f).SetEase(Ease.OutExpo).OnComplete(() => { StartCoroutine(EquipGachaCoroutine(EquipmentTier)); });
    }

    IEnumerator EquipGachaCoroutine(int EquipmentTier)
    {
        int CurrentEffectLevel = 1;
        bool IsAnimationEnd = false;
        //아래꺼가 계속 해당하는 티어가 나올때까지 반복되야함
        while (true)
        {
            yield return null;
            IsAnimationEnd = false;
            yield return new WaitForSeconds(0.3f);

            if (EquipmentTier <= CurrentEffectLevel)//1티어라면
            {
                break;
            }
            else//1티어 이상일때
            {
                EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(3f, 3f), 0.3f).SetEase(Ease.Linear).
                    OnComplete(() =>
                    { 
                        ContinueOfEquipGacha(CurrentEffectLevel++); 
                        EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(1.5f, 1.5f), 0.3f).
                        OnComplete(() => 
                        { 
                            IsAnimationEnd = true;
                            //EquipGachaCapsule.GetComponent<Image>().color = GachaTierLightColor[CurrentEffectLevel-1];
                            }); 
                    });//ContinueOfGacha에 int값을 전달후에 ++가됨
            }

            while(true)
            {
                if(IsAnimationEnd == true)
                {
                    break;
                }
                yield return null;
            }
        }
        SoundManager.Instance.PlaySFX("EquipGacha_Result");
        EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(3f, 3f), 0.3f).SetEase(Ease.Linear).OnComplete(() => { EndOfEquipGacha(); });
    }
    protected void ContinueOfEquipGacha(int CurrentEffectLevel)
    {
        //빛이 추가 되고, 빛 색깔도 바뀜
        for(int i = 0; i < CurrentEffectLevel + 1; i++)
        {
            EquipGachaLight[i].GetComponent<Image>().color = GachaTierLightColor[CurrentEffectLevel];
            if (EquipGachaLight[i].activeSelf == false)
            {//false면 키는거임
                EquipGachaLight[i].GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
                EquipGachaLight[i].GetComponent<RectTransform>().eulerAngles =
                    GachaLightRot[i] + new Vector3(0, 0, Random.Range(-GachaLightZVariation, GachaLightZVariation));
                EquipGachaLight[i].SetActive(true);
                PlayEquipGachaLightSound(CurrentEffectLevel);
                //Debug.Log("AAAAAA");
                EquipGachaLight[i].GetComponent<RectTransform>().DOScaleY(1, 0.3f).SetEase(Ease.OutExpo);
            }
        }
    }

    protected void PlayEquipGachaLightSound(int CurrentEffectLevel)
    {
        switch(CurrentEffectLevel)
        {
            case 0:
                SoundManager.Instance.PlaySFX("EquipGacha_TierOne");
                break;
            case 1:
                SoundManager.Instance.PlaySFX("EquipGacha_TierTwo");
                break;
            case 2:
                SoundManager.Instance.PlaySFX("EquipGacha_TierThree");
                break;
            case 3:
                SoundManager.Instance.PlaySFX("EquipGacha_TierFour");
                break;
            case 4:
                SoundManager.Instance.PlaySFX("EquipGacha_TierFive");
                break;
            case 5:
                SoundManager.Instance.PlaySFX("EquipGacha_TierSix");
                break;
        }
    }

    protected void EndOfEquipGacha()
    {

        EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(7f, 7f), 0.8f).
            OnComplete(() => 
            {
                for(int i = 0; i < EquipGachaLight.Length; i++)
                {
                    EquipGachaLight[i].GetComponent<Image>().color = GachaTierLightColor[i];
                    //obj.GetComponent<SpriteOutline>().Regenerate();
                    EquipGachaLight[i].SetActive(false);
                };
                EquipGachaCapsule.GetComponent<Image>().DOFade(0, 0.5f).
                OnComplete(() => 
                { 
                    GetEquipClickButton.SetActive(true);
                }); });
    }

    public void PressGetEquipClickButton()
    {
        EquipGachaObject.SetActive(false);
        SetGambling();//이걸로 Gambling UI를 업데이트 하고
        SetInventory();//인벤토리도 업데이트 해야함
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }
}
