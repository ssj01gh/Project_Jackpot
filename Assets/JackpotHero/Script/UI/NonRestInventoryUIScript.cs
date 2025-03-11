using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class NonRestInventoryUIScript : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    public PlayerManager PlayerMgr;
    public GameObject InventoryPanel;
    public GameObject[] InventorySlots;
    public Image[] InventoryItemImage;
    public GameObject[] InventoryLockImage;
    public TextMeshProUGUI[] TierTexts;
    public Image MouseFollowImage;

    protected Color ActiveColor = new Color(0.28f, 0.19f, 0.1f, 1f);
    protected Color UnActiveColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);

    protected int CurrentClickedSlotIndex;
    protected int CurrentBringItemCode;

    protected int DropDownSlotIndex;
    protected int DropDownItemCode;

    protected bool IsClickedInventorySlot = false;
    void Start()
    {
        InitNonRestInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitNonRestInventory()
    {
        InventoryPanel.SetActive(false);
    }

    public void OpenNonRestInventory()
    {
        if (InventoryPanel.activeSelf == true)
            return;

        InventoryPanel.GetComponent<RectTransform>().localScale = Vector2.zero;
        InventoryPanel.SetActive(true);
        InventoryPanel.GetComponent<RectTransform>().DOScale(Vector2.one, 0.5f).SetEase(Ease.OutBack);
        //�ϴ� �ٲ���
        MouseFollowImage.gameObject.SetActive(false);
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            InventorySlots[i].GetComponent<Image>().color = UnActiveColor;
            InventoryItemImage[i].gameObject.SetActive(false);
            InventoryLockImage[i].SetActive(true);
            TierTexts[i].text = "";
        }

        int CanUseInventory = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");
        for(int i = 0; i < CanUseInventory; i++)
        {
            InventorySlots[i].GetComponent<Image>().color = ActiveColor;
            InventoryLockImage[i].SetActive(false);
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//������� ������
            {
                //�ڵ忡 �´� �̹����� ����
                InventoryItemImage[i].gameObject.SetActive(true);
                InventoryItemImage[i].sprite = 
                    EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]).EquipmentImage;
                //TierNum = (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] / 1000) % 10
                //ex ) (21002 / 1000) = 21 -> 21 % 10 = 1; -> 1Ƽ��
                TierTexts[i].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]);
            }
        }
    }

    public void CloseNonRestInventory()
    {
        if (InventoryPanel.activeSelf == false)
            return;

        InventoryPanel.GetComponent<RectTransform>().localScale = Vector2.one;
        InventoryPanel.SetActive(true);
        InventoryPanel.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.3f).OnComplete(() => { InventoryPanel.SetActive(false); });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.pointerEnter != null && eventData.pointerEnter.tag == "InventorySlot")//���⼭ ������ ���� ���° Slot���� �˾ƾ���
        {//1�� ���� ���� 12�� ���Ա��� ��ġ�°� �˻�?
            Vector2 ClickedUIPos = eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition;
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if(Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                {
                    if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0 && InventoryLockImage[i].activeSelf == false)//������� ������//������� ������
                    {
                        IsClickedInventorySlot = true;
                        //��� �ִ� �������� ���� ����
                        CurrentClickedSlotIndex = i;
                        CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i];
                        //���콺�� ����ٴϴ� �̹����� ���콺 ��ġ�� ��ġ��Ű�� �̹����� �ٲ�
                        MouseFollowImage.gameObject.SetActive(true);
                        MouseFollowImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                        MoveUI(eventData);
                        //Ŭ���� ������ �̹����� ��� ����
                        InventoryItemImage[i].gameObject.SetActive(false);
                        //Ƽ�� �ؽ�Ʈ�� ��� ����
                        TierTexts[i].text = "";
                    }
                    break;
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(IsClickedInventorySlot == true)
        {
            MoveUI(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)//���⿡�� �÷��̾ �ִ� ������ ������Ʈ�� ������ ������.... Json�� �ִ� ���� ������ ���� �Ǵ°���
    {
        
        if (IsClickedInventorySlot == true && eventData.pointerEnter != null)
        {
            IsClickedInventorySlot = false;
            Vector2 ClickedUIPos = eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition;
            if (eventData.pointerEnter.tag == "InventorySlot")//���콺�� ���� ���� �����϶�
            {
                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                    {
                        if (InventoryLockImage[i].activeSelf == true)//��� ������
                        {
                            //���� ��ġ��
                            InventoryItemImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                            TierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                        }
                        else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//������� ������//��� ���� �����鼭 ������� ������
                        {
                            //����߸��� ���� ��ȣ�� ����ڵ带 ����
                            DropDownSlotIndex = i;
                            DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex];
                            //����߸� ���� ���Կ� ������ ��� ���� ����
                            InventoryItemImage[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                            TierTexts[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex] = CurrentBringItemCode;
                            //��� ������ ���Կ� ����߸� ���� ��� ���� ����
                            InventoryItemImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                            InventoryItemImage[CurrentClickedSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                            TierTexts[CurrentClickedSlotIndex].text = GetTierText(DropDownItemCode);
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = DropDownItemCode;
                        }
                        else//���������
                        {
                            //����߸� ���� ���Կ� ������ ��� ���� ����
                            InventoryItemImage[i].gameObject.SetActive(true);
                            InventoryItemImage[i].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                            TierTexts[i].text = GetTierText(CurrentBringItemCode);
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = CurrentBringItemCode;
                            //��� ��Ҵ� ������ ����
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                            TierTexts[CurrentClickedSlotIndex].text = "";
                        }
                        break;
                    }
                }
            }
            else if(eventData.pointerEnter.name == "TrashCan")//���콺�� ���� ���� ������ ���϶�
            {
                //��� ��Ҵ� ���� ����
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                TierTexts[CurrentClickedSlotIndex].text = "";
            }
            else//���Ե� ������ �뵵 �ƴҋ�
            {//���� �ڸ��� ����ġ
                InventoryItemImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                TierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
            }
            MouseFollowImage.gameObject.SetActive(false);
            CurrentBringItemCode = 0;
            CurrentClickedSlotIndex = 0;
            //Debug.Log(eventData.pointerEnter.name);
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

    private string GetTierText(int EquipmnetCode)
    {
        return ((EquipmnetCode / 1000) % 10).ToString() + "Ƽ��";
    }
}