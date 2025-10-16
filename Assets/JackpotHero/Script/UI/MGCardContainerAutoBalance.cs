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

    //Virtual카드들 비활성화 혹은 투명화 되어 놓다가
    //합쳐지는 타이밍이 되면//혹은 다 뒤집는 타이밍 되면
    //실제 클릭되는 카드들은 비활성화 -> Virtual카드들이 등장하게
    public void InitAllCard()
    {

        //EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);
    }

    public void SetCardActive(int ActiveCardCount)
    {
        UpperLine.SetActive(false);
        LowerLine.SetActive(false);
    }
}
