using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailCardContainerUI : MonoBehaviour
{
    public GameObject[] Cards;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveCards(float[] SpriteAmounts)
    {
        foreach(GameObject obj in Cards)
        {
            obj.SetActive(false);
        }
        for(int i = 0; i < SpriteAmounts.Length; i++)
        {
            Cards[i].SetActive(true);
            Cards[i].GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(SpriteAmounts[i]);
        }
    }
}
