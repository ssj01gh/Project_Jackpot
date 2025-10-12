using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBattleActionSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    public GameObject ActionPercentDetailUI;
    public TextMeshProUGUI ActionPercentDetailUIText;

    protected float UsefulInfo = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetUsefulInfo(float Info)
    {
        if(ActionPercentDetailUI.activeSelf == true)
            ActionPercentDetailUI.SetActive(false);
        UsefulInfo = Info;
    }

    public void InActiveActionPercentDetailUI()
    {
        ActionPercentDetailUI.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActionPercentDetailUI.SetActive(true);
        ActionPercentDetailUI.transform.position = eventData.pointerEnter.transform.position;

        if (UsefulInfo > 0)
            ActionPercentDetailUIText.text = "¾à " + "<color=green>+" + UsefulInfo.ToString("F1") +"%"+ "</color>";
        else if (UsefulInfo < 0)
            ActionPercentDetailUIText.text = "¾à " + "<color=red>" + UsefulInfo.ToString("F1") +"%" +"</color>";
        else
            ActionPercentDetailUIText.text = "¾à " + UsefulInfo.ToString("F1") +"%";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ActionPercentDetailUI.SetActive(false);
    }
}
