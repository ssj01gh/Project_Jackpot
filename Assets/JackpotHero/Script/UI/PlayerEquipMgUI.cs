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
    public TextMeshProUGUI EquipDetail_EquipDetailText;
    public TextMeshProUGUI EquipDetail_EquipSTRText;
    public TextMeshProUGUI EquipDetail_EquipDURText;
    public TextMeshProUGUI EquipDetail_EquipRESText;
    public TextMeshProUGUI EquipDetail_EquipSPDText;
    public TextMeshProUGUI EquipDetail_EquipLUKText;
    public GameObject[] EquipDetail_EquipCardContainer;
    [Header("EquipMg_EquipDetailInfo_Inven")]
    public Image EquipDetail_InvenImage;
    public TextMeshProUGUI EquipDetail_InvenTierText;
    public TextMeshProUGUI EquipDetail_InvenNameText;
    public TextMeshProUGUI EquipDetail_InvenDetailText;
    public TextMeshProUGUI EquipDetail_InvenSTRText;
    public TextMeshProUGUI EquipDetail_InvenDURText;
    public TextMeshProUGUI EquipDetail_InvenRESText;
    public TextMeshProUGUI EquipDetail_InvenSPDText;
    public TextMeshProUGUI EquipDetail_InvenLUKText;
    public GameObject[] EquipDetail_InvenCardContainer;
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
            return;//�� ���� ������ ���� ������
        }
        gameObject.SetActive(true);
        //SetPlayerEquip
        //�ٲ��ٰ� �ش�Ǵ°Ÿ� Ű��
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
        PlayerEquip.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1460, 890);
        PlayerEquip.SetActive(true);
        PlayerEquip.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-460, 190), 0.5f);
        //SetEquipInventory
        SetInventory(true);
        //SetEquipDetailInfo_Equip//Detail�� �ʱ���´� ����ִ� ����//�Ŀ� �÷��̾��� ���ۿ� ���� ���� ǥ��
        EquipDetail_EquipImage.gameObject.SetActive(false);
        EquipDetail_EquipTierText.text = "";
        EquipDetail_EquipNameText.text = "";
        EquipDetail_EquipDetailText.text = "";
        EquipDetail_EquipSTRText.text = "";
        EquipDetail_EquipDURText.text = "";
        EquipDetail_EquipRESText.text = "";
        EquipDetail_EquipSPDText.text = "";
        EquipDetail_EquipLUKText.text = "";
        foreach(GameObject Obj in EquipDetail_EquipCardContainer)
        {
            Obj.SetActive(false);
        }
        EquipDetailInfo_Equip.GetComponent<RectTransform>().anchoredPosition = new Vector2(-710, -850);
        EquipDetailInfo_Equip.SetActive(true);
        EquipDetailInfo_Equip.GetComponent<RectTransform>().DOAnchorPosY(-350, 0.5f);
        //SetEquipDetailInfo_Inven//Detail�� �ʱ���´� ����ִ� ����//�Ŀ� �÷��̾��� ���ۿ� ���� ���� ǥ��
        EquipDetail_InvenImage.gameObject.SetActive(false);
        EquipDetail_InvenTierText.text = "";
        EquipDetail_InvenNameText.text = "";
        EquipDetail_InvenDetailText.text = "";
        EquipDetail_InvenSTRText.text = "";
        EquipDetail_InvenDURText.text = "";
        EquipDetail_InvenRESText.text = "";
        EquipDetail_InvenSPDText.text = "";
        EquipDetail_InvenLUKText.text = "";
        foreach (GameObject Obj in EquipDetail_InvenCardContainer)
        {
            Obj.SetActive(false);
        }
        EquipDetailInfo_Inven.GetComponent<RectTransform>().anchoredPosition = new Vector2(-210, -730);
        EquipDetailInfo_Inven.SetActive(true);
        EquipDetailInfo_Inven.GetComponent<RectTransform>().DOAnchorPosY(-350, 0.5f);
        //SetEquipGambling
        SetGambling(true);

    }

    public void InActivePlayerEquipMg()// ��Ȱ��ȭ ������ ����
    {
        if(PlayerEquip.activeSelf == true)
        {
            PlayerEquip.GetComponent<RectTransform>().anchoredPosition = new Vector2(-460, 190);
            PlayerEquip.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1460, 890), 0.5f).OnComplete(() => { PlayerEquip.SetActive(false); });
        }
        if(EquipInventory.activeSelf == true)
        {
            EquipInventory.GetComponent<RectTransform>().anchoredPosition = new Vector2(500, 190);
            EquipInventory.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1420, 890), 0.5f).OnComplete(() => { EquipInventory.SetActive(false); });
        }
        if(EquipDetailInfo_Equip.activeSelf == true)
        {
            EquipDetailInfo_Equip.GetComponent<RectTransform>().anchoredPosition = new Vector2(-710, -350);
            EquipDetailInfo_Equip.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-710, -850), 0.5f).OnComplete(() => { EquipDetailInfo_Equip.SetActive(false); });
        }
        if(EquipDetailInfo_Inven.activeSelf == true)
        {
            EquipDetailInfo_Inven.GetComponent<RectTransform>().anchoredPosition = new Vector2(-210, -350);
            EquipDetailInfo_Inven.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-210, -730), 0.5f).OnComplete(() => { EquipDetailInfo_Inven.SetActive(false); });
        }
        if(EquipGambling.activeSelf == true)
        {
            EquipGambling.GetComponent<RectTransform>().anchoredPosition = new Vector2(500, -350);
            EquipGambling.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1420, -730), 0.5f).OnComplete(() => { EquipGambling.SetActive(false); gameObject.SetActive(false); });
        }

        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    protected string GetTierText(int Code)
    {
        return (Code / 1000) % 10 + "Ƽ��";
    }

    protected void SetGambling(bool IsActive = false)//->�̰Ŵ� ���� �ൿ �ɶ����� ��� ������Ʈ �ؾ��ҵ�?
    {//�ϴ��� ��� ���� ������(OnPointerUp�϶� �ѹ��ؾ���)(��� �̰� ������ �׷���)
        EXPAmountText.text = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience.ToString();
        //�̱� ������ ��ư
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel >= 10)
        {//������ ���
            EquipGamblingLevelUpButton.interactable = false;
            EquipGamblingLevelUPButtonText.text = "MaxLevel";
        }
        else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 
            EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel))
        {//����ġ�� ������ ���
            EquipGamblingLevelUpButton.interactable = false;
            EquipGamblingLevelUPButtonText.text = "����ġ ����\r\n" +
                "�ʿ� : " + EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel) + "����ġ";
        }
        else//�������� �������
        {
            EquipGamblingLevelUpButton.interactable = true;
            EquipGamblingLevelUPButtonText.text = "��� �̱� ��ȭ\r\n" + 
                EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel) + " ����ġ";
        }

        //�̱� ��ư
        if(PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)
        {//�κ��丮�� ��á�ٸ�
            EquipGamblingButton.interactable = false;
            EquipGamblingButtonText.text = "�κ��丮\r\n���� ����";
        }
        else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 
            EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel))
        {//����ġ�� �����Ұ��
            EquipGamblingButton.interactable = false;
            EquipGamblingButtonText.text = "����ġ ����\r\n" +
                "�ʿ� : " + EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel) + "����ġ";
        }
        else
        {
            EquipGamblingButton.interactable = true;
            EquipGamblingButtonText.text = "��� �̱�\r\n" + 
                EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel) + " ����ġ";
        }


        //EquipGamblingPercentTexts
        for (int i = 0; i < EquipGamblingPercentTexts.Length; i++)
        {
            EquipGamblingPercentTexts[i].text = EquipmentInfoManager.Instance.GetGamblingLevelPercent(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel, i) + "%";
        }

        if (IsActive == true)
        {
            EquipGambling.GetComponent<RectTransform>().anchoredPosition = new Vector2(1420, -730);
            EquipGambling.SetActive(true);
            EquipGambling.GetComponent<RectTransform>().DOAnchorPos(new Vector2(500, -350), 0.5f);
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
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//������� ������
            {
                //�ڵ忡 �´� �̹����� ����
                InventorySlotsImage[i].gameObject.SetActive(true);
                InventorySlotsImage[i].sprite =
                    EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]).EquipmentImage;
                InventorySlotTierTexts[i].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]);
            }
        }
        if(IsActive == true)
        {
            EquipInventory.GetComponent<RectTransform>().anchoredPosition = new Vector2(1420, 890);
            EquipInventory.SetActive(true);
            EquipInventory.GetComponent<RectTransform>().DOAnchorPos(new Vector2(500, 190), 0.5f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerEquipIndex = -1;
        if (eventData.pointerEnter != null && eventData.pointerEnter.tag == "InventorySlot")//���⼭ ������ ���� ���° Slot���� �˾ƾ���
        {
            Vector2 ClickedUIPos = eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition;
            for(int i = 0; i <PlayerEquipSlots.Length; i++)
            {
                if(Vector2.Distance(ClickedUIPos, PlayerEquipSlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                {
                    switch (i)
                    {
                        case (int)EPlayerEquip.Helmet:
                            if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode == 0)//����ִٸ�
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
                            break;
                        case (int)EPlayerEquip.Armor:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode == 0)//����ִٸ�
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
                            break;
                        case (int)EPlayerEquip.Boots:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode == 0)//����ִٸ�
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
                            break;
                        case (int)EPlayerEquip.Weapon:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == 0)//����ִٸ�
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
                            break;
                        case (int)EPlayerEquip.Accessories:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode == 0)//����ִٸ�
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
                    //���ĭ ������ Ŭ��
                    break;
                }
            }
            
            if(PlayerEquipIndex == -1)//���� ���â�߿� Ŭ���� ������ ���ٸ� �κ��丮�� �˻�
            {
                //�κ��丮 �������� �˻�
                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                    {
                        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0 && LockObjects[i].activeSelf == false)//������� ������//������� ������
                        {
                            SoundManager.Instance.PlayUISFX("Item_PickUp");
                            IsClickedInventorySlot = true;
                            //��� �ִ� �������� ���� ����
                            CurrentClickedSlotIndex = i;
                            CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i];
                            //���콺�� ����ٴϴ� �̹����� ���콺 ��ġ�� ��ġ��Ű�� �̹����� �ٲ�
                            MouseFollowImage.gameObject.SetActive(true);
                            MouseFollowImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                            MoveUI(eventData);
                            //Ŭ���� ������ �̹����� ��� ����
                            InventorySlotsImage[i].gameObject.SetActive(false);
                            //Ƽ�� �ؽ�Ʈ�� ��� ����
                            InventorySlotTierTexts[i].text = "";
                        }
                        //�κ��丮ĭ ������ Ŭ��
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

            if (PlayerEquipIndex != -1)//��� ĭ���� Ŭ��������//���ĭ���� �κ��丮 OR ���ĭ
            {
                if (eventData.pointerEnter.tag == "InventorySlot")//���콺�� ���� ���� �����϶�
                {
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    bool IsEquipmentSlot = true;
                    //�κ��丮 ĭ���� �˻�
                    for (int i = 0; i < InventorySlots.Length; i++)
                    {
                        if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                        {
                            //���⿡ �ѹ��̶� ���ٴ°� = ���콺�� �� ������ ��� ������ �ƴ�
                            IsEquipmentSlot = false;
                            if (LockObjects[i].activeSelf == true)//��� ������
                            {
                                //���� ��ġ��
                                PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                                PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                            }
                            else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//������� ������//��� ���� �����鼭 ������� ������
                            {
                                if(CurrentBringItemCode / 10000 == 
                                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] / 10000)//������� �ʾ����� ����� Ÿ���� ���ٸ�
                                {//�ٲٱ�
                                    //����߸��� ���� index��ȣ�� ��� �ڵ带 ����
                                    DropDownSlotIndex = i;
                                    DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i];
                                    //����߸� ���� ���Կ� ������ ��� ���� ����
                                    InventorySlotsImage[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                    InventorySlotTierTexts[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex] = CurrentBringItemCode;
                                    //������ ���Կ� ����߸����� ��� �ֱ�
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
                                else//���� �ʴٸ�
                                {//���� ��ġ��
                                    PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                                    PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                                }
                            }
                            else//����ִٸ�
                            {
                                //����ִ� ���� �ְ�
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = CurrentBringItemCode;
                                InventorySlotsImage[i].gameObject.SetActive(true);
                                InventorySlotsImage[i].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                InventorySlotTierTexts[i].text = GetTierText(CurrentBringItemCode);
                                //��� ĭ�� ����
                                //�̹� UI�����δ� ����� ����
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

                    if(IsEquipmentSlot == true)//��� �����̿��ٸ�
                    {//���� �ڸ���
                        PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                        PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                    }
                }
                else if (eventData.pointerEnter.name == "TrashCan")//���콺�� ���� ���� ������ ���϶�
                {
                    SoundManager.Instance.PlayUISFX("Item_Remove");
                    //����
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
                else//���� �ƴҶ�
                {//���� �ڸ���
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                    PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                }
            }
            else//Ŭ��(PointDown)�� ������ �κ��丮 �����϶�//�κ��丮���� ���ĭ OR �κ��丮��
            {
                if (eventData.pointerEnter.tag == "InventorySlot")//���콺�� ���� ���� �����϶�
                {
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    bool IsEquipmentSlot = true;
                    //�κ��丮 ĭ���� �˻�
                    for (int i = 0; i < InventorySlots.Length; i++)
                    {
                        if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                        {
                            //���⿡ �ѹ��̶� ���ٴ°� = ���콺�� �� ������ ��� ������ �ƴ�
                            IsEquipmentSlot = false;
                            if (LockObjects[i].activeSelf == true)//��� ������
                            {
                                //���� ��ġ��
                                InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                            }
                            else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//������� ������//��� ���� �����鼭 ������� ������
                            {
                                //����߸��� ���� ��ȣ�� ����ڵ带 ����
                                DropDownSlotIndex = i;
                                DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex];
                                //����߸� ���� ���Կ� ������ ��� ���� ����
                                InventorySlotsImage[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                InventorySlotTierTexts[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex] = CurrentBringItemCode;
                                //��� ������ ���Կ� ����߸� ���� ��� ���� ����
                                InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                InventorySlotsImage[CurrentClickedSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                                InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(DropDownItemCode);
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = DropDownItemCode;
                            }
                            else//����ִٸ�
                            {
                                //����ִ� ���� �ְ�
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = CurrentBringItemCode;
                                InventorySlotsImage[i].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                InventorySlotTierTexts[i].text = GetTierText(CurrentBringItemCode);
                                //���� ĭ�� ����
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                            }
                            break;
                        }
                    }

                    if (IsEquipmentSlot == true)//��� �����̿��ٸ�
                    {
                        for(int i = 0; i < PlayerEquipSlots.Length; i++)
                        {
                            if(Vector2.Distance(ClickedUIPos, PlayerEquipSlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                            {//���ĭ���� Ŭ���� ��������
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

                                if(DropDownItemCode == 0)//����ִٸ�//�°� �־�� �ϴµ�.....
                                {
                                    if(CurrentBringItemCode / 10000 == 1 && DropDownSlotIndex == (int)EPlayerEquip.Weapon)
                                    {//CurrentBringItemCode�� ���ڸ��� 1 : ���� -> 1�̶�� �ε��� 3�� ĭ�϶� ok -> �ֱ�
                                        //�ֱ�
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //���� ĭ�� ����
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if(CurrentBringItemCode / 10000 == 2 && DropDownSlotIndex == (int)EPlayerEquip.Armor)
                                    {//CurrentBringItemCode�� ���ڸ��� 2 : ���� -> 2��� �ε��� 1�� ĭ�϶� ok -> �ֱ�
                                        //�ֱ�
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //���� ĭ�� ����
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if(CurrentBringItemCode / 10000 == 3 && DropDownSlotIndex == (int)EPlayerEquip.Helmet)
                                    {//CurrentBringItemCode�� ���ڸ��� 3 : ���� -> 3�̶�� �ε��� 0�� ĭ�϶� ok -> �ֱ�
                                        //�ֱ�
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //���� ĭ�� ����
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if(CurrentBringItemCode / 10000 == 4 && DropDownSlotIndex == (int)EPlayerEquip.Boots)
                                    {//CurrentBringItemCode�� ���ڸ��� 4 : �Ź� -> 4��� �ε��� 2�� ĭ�϶� ok -> �ֱ�
                                        //�ֱ�
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //���� ĭ�� ����
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if(CurrentBringItemCode / 10000 == 5 && DropDownSlotIndex == (int)EPlayerEquip.Accessories)
                                    {//CurrentBringItemCode�� ���ڸ��� 5 : ��ű� -> 5��� �ε��� 4�� ĭ �϶� ok -> �ֱ�
                                        //�ֱ�
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //���� ĭ�� ����
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else
                                    {//�������� �ش����� �ʴ´ٸ� ���� �ڸ���
                                        InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                        InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                                    }
                                }
                                else if(CurrentBringItemCode / 10000 == DropDownItemCode / 10000)//���� Ÿ���� ����� ��ü
                                {//�ٲٱ�
                                    //�κ��丮 ��� -> ���� ���
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
                                    //���� ��� -> �κ��丮 ���
                                    InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                    InventorySlotsImage[CurrentClickedSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(DropDownItemCode);
                                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = DropDownItemCode;
                                }
                                else
                                {//���� �ƴϸ� ���� �ڸ���
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
                else if (eventData.pointerEnter.name == "TrashCan")//���콺�� ���� ���� ������ ���϶�
                {
                    //��� ��Ҵ� ���� ����
                    SoundManager.Instance.PlayUISFX("Item_Remove");
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = "";
                }
                else//���� �ƴҶ�
                {//���� �ڸ���
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

    private void DisplayEquipDetailInfo(bool IsEquipedEquipment)//��� Ŭ���ϸ� �ڼ��� ������ ����Ѵ�.
    {//true�� ���ĭ�� Ŭ���ѰŰ�, false�� �κ��丮�� Ŭ���Ѱ���
        if(IsEquipedEquipment == true)//���ĭ�� �ִ� ������ Ŭ��
        {//-> ������ Detail�� ����ϸ� ��
            //CurrentBringItemCode -> 0�϶��� �� ����
            EquipmentSO EquipedEquipmentInfo = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode);
            EquipDetail_EquipImage.gameObject.SetActive(true);
            EquipDetail_EquipImage.sprite = EquipedEquipmentInfo.EquipmentImage;
            EquipDetail_EquipTierText.text = GetTierText(CurrentBringItemCode);
            EquipDetail_EquipNameText.text = EquipedEquipmentInfo.EquipmentName;

            if (CurrentBringItemCode >= 10000 && CurrentBringItemCode < 20000)
                EquipDetail_EquipDetailText.text = "���ݽ� �Ƿε� : " + EquipedEquipmentInfo.SpendTiredness.ToString();
            else if (CurrentBringItemCode >= 20000 && CurrentBringItemCode < 30000)
                EquipDetail_EquipDetailText.text = "���� �Ƿε� : " + EquipedEquipmentInfo.SpendTiredness.ToString();
            else if (CurrentBringItemCode >= 30000 && CurrentBringItemCode < 40000)
                EquipDetail_EquipDetailText.text = "�Ƿε� ȸ���� �Ƿε��� ������ �ʽ��ϴ�.";
            else if (CurrentBringItemCode >= 40000 && CurrentBringItemCode < 50000)
                EquipDetail_EquipDetailText.text = "�Ź��� �Ƿε��� ������� �ʽ��ϴ�.";
            else if (CurrentBringItemCode >= 50000 && CurrentBringItemCode < 60000)
                EquipDetail_EquipDetailText.text = "��ű��� �Ƿε��� ������� �ʽ��ϴ�.";
            else
                EquipDetail_EquipDetailText.text = "";

            EquipDetail_EquipSTRText.text = EquipedEquipmentInfo.AddSTRAmount.ToString();
            EquipDetail_EquipDURText.text = EquipedEquipmentInfo.AddDURAmount.ToString();
            EquipDetail_EquipRESText.text = EquipedEquipmentInfo.AddRESAmount.ToString();
            EquipDetail_EquipSPDText.text = EquipedEquipmentInfo.AddSPDAmount.ToString();
            EquipDetail_EquipLUKText.text = EquipedEquipmentInfo.AddLUKAmount.ToString();

            //Ȱ��ȭ �ϱ����� �ѹ� �� �ʱ�ȭ
            foreach(GameObject obj in EquipDetail_EquipCardContainer)
                obj.SetActive(false);
            for (int i = 0; i < EquipedEquipmentInfo.EquipmentSlots.Length; i++)
            {
                //Ȱ��ȭ
                EquipDetail_EquipCardContainer[i].SetActive(true);
                EquipDetail_EquipCardContainer[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(EquipedEquipmentInfo.EquipmentSlots[i].SlotState);
            }
            //�������� ����â�� �ƿ� ����
            EquipDetail_InvenImage.gameObject.SetActive(false);
            EquipDetail_InvenTierText.text = "";
            EquipDetail_InvenNameText.text = "";
            EquipDetail_InvenDetailText.text = "";
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
        else//�κ��丮�� �ִ� ������ Ŭ��
        {//-> ���� Detail, ������ Detail�� ��� �ؾߵ�
         //���� Ÿ���� ��� ������ �������� ��� x
            //�κ��� ��� ǥ��
            EquipmentSO UnEquipedEquipment = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode);
            int EquipedEquipmentCode;
            EquipDetail_InvenImage.gameObject.SetActive(true);
            EquipDetail_InvenImage.sprite = UnEquipedEquipment.EquipmentImage;
            EquipDetail_InvenTierText.text = GetTierText(CurrentBringItemCode);
            EquipDetail_InvenNameText.text = UnEquipedEquipment.EquipmentName;

            if (CurrentBringItemCode >= 10000 && CurrentBringItemCode < 20000)
            {
                EquipDetail_InvenDetailText.text = "���ݽ� �Ƿε� : " + UnEquipedEquipment.SpendTiredness.ToString();
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
            }
            else if (CurrentBringItemCode >= 20000 && CurrentBringItemCode < 30000)
            {
                EquipDetail_InvenDetailText.text = "���� �Ƿε� : " + UnEquipedEquipment.SpendTiredness.ToString();
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
            }
            else if (CurrentBringItemCode >= 30000 && CurrentBringItemCode < 40000)
            {
                EquipDetail_InvenDetailText.text = "�Ƿε� ȸ���� �Ƿε��� ������ �ʽ��ϴ�.";
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
            }
            else if (CurrentBringItemCode >= 40000 && CurrentBringItemCode < 50000)
            {
                EquipDetail_InvenDetailText.text = "�Ź��� �Ƿε��� ������� �ʽ��ϴ�.";
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
            }
            else if (CurrentBringItemCode >= 50000 && CurrentBringItemCode < 60000)
            {
                EquipDetail_InvenDetailText.text = "��ű��� �Ƿε��� ������� �ʽ��ϴ�.";
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode;
            }
            else
            {
                EquipDetail_InvenDetailText.text = "";
                EquipedEquipmentCode = 0;
            }

            EquipDetail_InvenSTRText.text = UnEquipedEquipment.AddSTRAmount.ToString();
            EquipDetail_InvenDURText.text = UnEquipedEquipment.AddDURAmount.ToString();
            EquipDetail_InvenRESText.text = UnEquipedEquipment.AddRESAmount.ToString();
            EquipDetail_InvenSPDText.text = UnEquipedEquipment.AddSPDAmount.ToString();
            EquipDetail_InvenLUKText.text = UnEquipedEquipment.AddLUKAmount.ToString();

            //Ȱ��ȭ �ϱ����� �ѹ� �� �ʱ�ȭ
            foreach (GameObject obj in EquipDetail_InvenCardContainer)
                obj.SetActive(false);
            for (int i = 0; i < UnEquipedEquipment.EquipmentSlots.Length; i++)
            {
                //Ȱ��ȭ
                EquipDetail_InvenCardContainer[i].SetActive(true);
                EquipDetail_InvenCardContainer[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(UnEquipedEquipment.EquipmentSlots[i].SlotState);
            }
            //���� Ÿ���� ��� �ְų� Ȥ�� ���ų�
            EquipmentSO EquipedEquipmentInfo = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(EquipedEquipmentCode);
            if (EquipedEquipmentCode == 0)//���� �ִ� ��� ������
            {
                //�ƿ� ����
                EquipDetail_EquipImage.gameObject.SetActive(false);
                EquipDetail_EquipTierText.text = "";
                EquipDetail_EquipNameText.text = "";
                EquipDetail_EquipDetailText.text = "";
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
            else//���� �ִ� ��� ������
            {
                EquipDetail_EquipImage.gameObject.SetActive(true);
                EquipDetail_EquipImage.sprite = EquipedEquipmentInfo.EquipmentImage;
                EquipDetail_EquipTierText.text = GetTierText(EquipedEquipmentCode);
                EquipDetail_EquipNameText.text = EquipedEquipmentInfo.EquipmentName;

                if (CurrentBringItemCode >= 10000 && CurrentBringItemCode < 20000)
                    EquipDetail_EquipDetailText.text = "���ݽ� �Ƿε� : " + EquipedEquipmentInfo.SpendTiredness.ToString();
                else if (CurrentBringItemCode >= 20000 && CurrentBringItemCode < 30000)
                    EquipDetail_EquipDetailText.text = "���� �Ƿε� : " + EquipedEquipmentInfo.SpendTiredness.ToString();
                else if (CurrentBringItemCode >= 30000 && CurrentBringItemCode < 40000)
                    EquipDetail_EquipDetailText.text = "�Ƿε� ȸ���� �Ƿε��� ������ �ʽ��ϴ�.";
                else if (CurrentBringItemCode >= 40000 && CurrentBringItemCode < 50000)
                    EquipDetail_EquipDetailText.text = "�Ź��� �Ƿε��� ������� �ʽ��ϴ�.";
                else if (CurrentBringItemCode >= 50000 && CurrentBringItemCode < 60000)
                    EquipDetail_EquipDetailText.text = "��ű��� �Ƿε��� ������� �ʽ��ϴ�.";
                else
                    EquipDetail_EquipDetailText.text = "";

                EquipDetail_EquipSTRText.text = EquipedEquipmentInfo.AddSTRAmount.ToString();
                EquipDetail_EquipDURText.text = EquipedEquipmentInfo.AddDURAmount.ToString();
                EquipDetail_EquipRESText.text = EquipedEquipmentInfo.AddRESAmount.ToString();
                EquipDetail_EquipSPDText.text = EquipedEquipmentInfo.AddSPDAmount.ToString();
                EquipDetail_EquipLUKText.text = EquipedEquipmentInfo.AddLUKAmount.ToString();

                //Ȱ��ȭ �ϱ����� �ѹ� �� �ʱ�ȭ
                foreach (GameObject obj in EquipDetail_EquipCardContainer)
                    obj.SetActive(false);
                for (int i = 0; i < EquipedEquipmentInfo.EquipmentSlots.Length; i++)
                {
                    //Ȱ��ȭ
                    EquipDetail_EquipCardContainer[i].SetActive(true);
                    EquipDetail_EquipCardContainer[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(EquipedEquipmentInfo.EquipmentSlots[i].SlotState);
                }
            }
        }
    }

    public void PressEquipGamblingLevelUPButton()
    {
        //����ġ�� �پ���. //����ġ�� ���� �پ������ ������ �´� ����ġ�� �پ��
        //�÷��̾��� �̱� ������ ������.
        //SetGambling���� ui�� ������Ʈ�Ѵ�.
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel), true);
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel++;
        SetGambling();
    }

    public void PressEquipGamblingGachaButton()//ActiveGacha
    {
        //����ġ�� �پ���.
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel), true);
        //�ʱ�ȭ
        EquipGachaObject.SetActive(true);
        EquipGachaEquipmentObject.SetActive(true);//��� ���߰� �ִ� ĸ���� ��� �̹����� ������ �ִ� ������Ʈ
        EquipGachaCapsule.GetComponent<RectTransform>().localScale = Vector3.one;
        EquipGachaCapsule.GetComponent<CanvasGroup>().alpha = 1;
        EquipGachaCapsule.SetActive(true);
        EquipGachaEquipmentObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 750);
        ClickButton.SetActive(false);
        GetEquipClickButton.SetActive(false);

        for(int i = 0; i < EquipGachaLight.Length; i++)
        {
            EquipGachaLight[i].GetComponent<Image>().color = GachaTierLightColor[i];
            EquipGachaLight[i].GetComponent<SpriteOutline>().color = GachaTierLightColor[i];
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
        
        //����� ������ ����� �̹����� �κ��丮�� �̸� �־��, ��¥�� ������Ʈ ���� ������ UI(�κ��丮)������ �Ⱥ��̴ϱ�
        //GetGamblingEquipmentCode <- gambling ������ �´� ��� �ڵ� ��ȯ
        GachaResultEquipCode = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
        EquipGachaResultImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(GachaResultEquipCode).EquipmentImage;
        //�̰� ���߿� Ȱ��ȭ
        PlayerMgr.GetPlayerInfo().PutEquipmentToInven(GachaResultEquipCode);
        //������� ������ Ŭ����ư Ȱ��ȭ
        EquipGachaEquipmentObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0.7f).SetEase(Ease.OutBounce).
            OnComplete(() => { ActiveGachaClickButton(); });
        //��í ���ⰰ���� ������ �����ٵ�.....
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
        //�� ��ư�� ������ ��ư�� ��Ȱ��ȭ
        int EquipmentTier = (GachaResultEquipCode / 1000) % 10;
        // 1Ƽ�� ����� 0������ Ȱ��ȭ , ���// 2Ƽ�� �����  1������ Ȱ��ȭ, �Ķ���
        // 3Ƽ�� ����� 2������ Ȱ��ȭ, �����// 4Ƽ�� ����� 3������ Ȱ��ȭ, �����
        // 5Ƽ�� ����� 4������ Ȱ��ȭ, 
        //�̸� ������ GachaResultEquipCode�� Ƽ� �ľ��� �װͿ� �°� ����
        // 1Ƽ�� ���� ���� ���� �����Ƿ� 1�� �ٷ� Ȱ��ȭ
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
        //�Ʒ����� ��� �ش��ϴ� Ƽ� ���ö����� �ݺ��Ǿ���
        while (true)
        {
            yield return null;
            IsAnimationEnd = false;
            yield return new WaitForSeconds(0.3f);

            if (EquipmentTier <= CurrentEffectLevel)//1Ƽ����
            {
                break;
            }
            else//1Ƽ�� �̻��϶�
            {
                EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(3f, 3f), 0.3f).SetEase(Ease.Linear).
                    OnComplete(() =>
                    { 
                        ContinueOfEquipGacha(CurrentEffectLevel++); 
                        EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(1f, 1f), 0.3f).
                        OnComplete(() => { IsAnimationEnd = true; }); 
                    });//ContinueOfGacha�� int���� �����Ŀ� ++����
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
        //���� �߰� �ǰ�, �� ���� �ٲ�
        for(int i = 0; i < CurrentEffectLevel + 1; i++)
        {
            EquipGachaLight[i].GetComponent<Image>().color = GachaTierLightColor[CurrentEffectLevel];
            EquipGachaLight[i].GetComponent<SpriteOutline>().color = GachaTierLightColor[CurrentEffectLevel];
            if (EquipGachaLight[i].activeSelf == false)
            {//false�� Ű�°���
                EquipGachaLight[i].GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
                EquipGachaLight[i].GetComponent<RectTransform>().eulerAngles =
                    GachaLightRot[i] + new Vector3(0, 0, Random.Range(-GachaLightZVariation, GachaLightZVariation));
                EquipGachaLight[i].SetActive(true);
                PlayEquipGachaLightSound(CurrentEffectLevel);
                Debug.Log("AAAAAA");
                EquipGachaLight[i].GetComponent<RectTransform>().DOScaleY(1, 0.3f).SetEase(Ease.OutExpo);
            }
            else//�̹� �����ִ� light�� outline�̹����� ���� �ȵ�, regenerate?
            {
                EquipGachaLight[i].GetComponent<SpriteOutline>().Regenerate();
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
                    EquipGachaLight[i].GetComponent<SpriteOutline>().color = GachaTierLightColor[i];
                    if (EquipGachaLight[i].activeSelf == true)
                        EquipGachaLight[i].GetComponent<SpriteOutline>().Regenerate();
                    //obj.GetComponent<SpriteOutline>().Regenerate();
                    EquipGachaLight[i].SetActive(false);
                };
                EquipGachaCapsule.GetComponent<CanvasGroup>().DOFade(0, 0.5f).
                OnComplete(() => 
                { 
                    GetEquipClickButton.SetActive(true);
                }); });
    }

    public void PressGetEquipClickButton()
    {
        EquipGachaObject.SetActive(false);
        SetGambling();//�̰ɷ� Gambling UI�� ������Ʈ �ϰ�
        SetInventory();//�κ��丮�� ������Ʈ �ؾ���
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }
}
