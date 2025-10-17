using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MGCardContainerAutoBalance : MonoBehaviour
{
    public GameObject UpperLine;
    public GameObject LowerLine;
    public GameObject[] UpperCard;
    public GameObject[] UpperVirtualCard;
    public GameObject[] LowerCard;
    public GameObject[] LowerVirtualCard;

    private Vector3 InitPosRotation = new Vector3(0, 0, 0);
    private Vector3 InitScale = new Vector3(1, 1, 1);
    private Color InitColor = new Color(1, 1, 1, 1);
    //Virtual카드들 비활성화 혹은 투명화 되어 놓다가
    //실제 클릭되는 카드들은 비활성화 -> Virtual카드들이 등장하게 -> Virtual 카드들이 뒤집어진다. -> Virtual 카드들만 이미지가 바뀜
    //활성화된 Virtual카드는 타이밍 맞게 정해진 위치로 회전, 이동
    public void InitAllCard()
    {//초기화 == 일단 카드들 다 비활성화, 초기 상태로 되될리기
        UpperLine.SetActive(false);
        LowerLine.SetActive(false);
        foreach (GameObject obj in UpperCard)
        {
            obj.SetActive(false);
            obj.GetComponent<Image>().color = InitColor;
        }
        foreach(GameObject obj in UpperVirtualCard)
        {
            obj.SetActive(false);
            obj.GetComponent<RectTransform>().anchoredPosition = InitPosRotation;
            obj.GetComponent<RectTransform>().localScale = InitScale;
            obj.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);
        }
        foreach (GameObject obj in LowerCard)
        {
            obj.SetActive(false);
            obj.GetComponent<Image>().color = InitColor;
        }
        foreach (GameObject obj in LowerVirtualCard)
        {
            obj.SetActive(false);
            obj.GetComponent<RectTransform>().anchoredPosition = InitPosRotation;
            obj.GetComponent<RectTransform>().localScale = InitScale;
            obj.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);
        }
        //MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);//?카드로 초기화
        //EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);
    }

    public void SetCardActive(int ActiveCardCount)
    {
        switch(ActiveCardCount)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                UpperLine.SetActive(true);
                for(int i = 0; i < ActiveCardCount; i++)
                {
                    UpperCard[i].SetActive(true);
                }
                break;
            case 5://3 2
                UpperLine.SetActive(true);
                LowerLine.SetActive(true);
                for(int i = 0; i < 3; i++)
                {
                    UpperCard[i].SetActive(true);
                }
                for(int i = 0; i < 2; i++)
                {
                    LowerCard[i].SetActive(true);
                }
                break;
            case 6:// 4 2
                UpperLine.SetActive(true);
                LowerLine.SetActive(true);
                for (int i = 0; i < 4; i++)
                {
                    UpperCard[i].SetActive(true);
                }
                for (int i = 0; i < 2; i++)
                {
                    LowerCard[i].SetActive(true);
                }
                break;
            case 7://4 3
                UpperLine.SetActive(true);
                LowerLine.SetActive(true);
                for (int i = 0; i < 3; i++)
                {
                    UpperCard[i].SetActive(true);
                }
                for (int i = 0; i < 3; i++)
                {
                    LowerCard[i].SetActive(true);
                }
                break;
            case 8://4 4
                UpperLine.SetActive(true);
                LowerLine.SetActive(true);
                for (int i = 0; i < 3; i++)
                {
                    UpperCard[i].SetActive(true);
                }
                for (int i = 0; i < 4; i++)
                {
                    LowerCard[i].SetActive(true);
                }
                break;
        }
    }
}
