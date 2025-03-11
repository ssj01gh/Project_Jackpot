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
        //일단 다끄기
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
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//비어있지 않을때
            {
                //코드에 맞는 이미지를 넣음
                InventoryItemImage[i].gameObject.SetActive(true);
                InventoryItemImage[i].sprite = 
                    EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]).EquipmentImage;
                //TierNum = (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] / 1000) % 10
                //ex ) (21002 / 1000) = 21 -> 21 % 10 = 1; -> 1티어
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
        if(eventData.pointerEnter != null && eventData.pointerEnter.tag == "InventorySlot")//여기서 들어오는 놈이 몇번째 Slot인지 알아야함
        {//1번 슬롯 에서 12번 슬롯까지 겹치는거 검사?
            Vector2 ClickedUIPos = eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition;
            for (int i = 0; i < InventorySlots.Length; i++)
            {
                if(Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                {
                    if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0 && InventoryLockImage[i].activeSelf == false)//비어있지 않을때//잠겨있지 않을때
                    {
                        IsClickedInventorySlot = true;
                        //들고 있는 아이템의 정보 저장
                        CurrentClickedSlotIndex = i;
                        CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i];
                        //마우스를 따라다니는 이미지를 마우스 위치에 위치시키고 이미지를 바꿈
                        MouseFollowImage.gameObject.SetActive(true);
                        MouseFollowImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                        MoveUI(eventData);
                        //클릭한 슬롯의 이미지를 잠시 꺼둠
                        InventoryItemImage[i].gameObject.SetActive(false);
                        //티어 텍스트도 잠시 꺼둠
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

    public void OnPointerUp(PointerEventData eventData)//여기에서 플레이어에 있는 정보의 업데이트를 때리고 싶은데.... Json에 있는 정보 저장은 따로 되는것임
    {
        
        if (IsClickedInventorySlot == true && eventData.pointerEnter != null)
        {
            IsClickedInventorySlot = false;
            Vector2 ClickedUIPos = eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition;
            if (eventData.pointerEnter.tag == "InventorySlot")//마우스를 놓은 곳이 슬롯일때
            {
                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                    {
                        if (InventoryLockImage[i].activeSelf == true)//잠겨 있을때
                        {
                            //원래 위치로
                            InventoryItemImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                            TierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                        }
                        else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//비어있지 않을떄//잠겨 있지 않으면서 비어있지 않을때
                        {
                            //떨어뜨리는 곳의 번호와 장비코드를 저장
                            DropDownSlotIndex = i;
                            DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex];
                            //떨어뜨린 곳의 슬롯에 집었던 장비를 덮어 쓰기
                            InventoryItemImage[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                            TierTexts[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex] = CurrentBringItemCode;
                            //장비를 집었던 슬롯에 떨어뜨린 곳의 장비를 덮어 쓰기
                            InventoryItemImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                            InventoryItemImage[CurrentClickedSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                            TierTexts[CurrentClickedSlotIndex].text = GetTierText(DropDownItemCode);
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = DropDownItemCode;
                        }
                        else//비어있을떄
                        {
                            //떨어뜨린 곳의 슬롯에 집었던 장비를 덮어 쓰기
                            InventoryItemImage[i].gameObject.SetActive(true);
                            InventoryItemImage[i].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                            TierTexts[i].text = GetTierText(CurrentBringItemCode);
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = CurrentBringItemCode;
                            //장비를 잡았던 슬롯을 비우기
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                            TierTexts[CurrentClickedSlotIndex].text = "";
                        }
                        break;
                    }
                }
            }
            else if(eventData.pointerEnter.name == "TrashCan")//마우스를 놓은 곳이 쓰레기 통일때
            {
                //장비를 잡았던 슬롯 비우기
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                TierTexts[CurrentClickedSlotIndex].text = "";
            }
            else//슬롯도 쓰레기 통도 아닐떄
            {//원래 자리로 원위치
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
        return ((EquipmnetCode / 1000) % 10).ToString() + "티어";
    }
}