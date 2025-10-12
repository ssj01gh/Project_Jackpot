using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventUIScript : MonoBehaviour
{
    public TextMeshProUGUI EventTitle;
    public Image EventImage;
    public TextMeshProUGUI EventDetail;
    public GameObject[] EventSelectionButtons;
    public TextMeshProUGUI[] EventSelectionTexts;
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveEventUI(EventManager EventMgr)
    {
        EventTitle.text = EventMgr.CurrentEvent.EventTitle;
        EventImage.sprite = EventMgr.CurrentEvent.EventImage;
        EventDetail.text = EventMgr.CurrentEvent.EventDetail;
        foreach(GameObject Obj in EventSelectionButtons)
        {
            Obj.SetActive(false);
        }
        for(int i = 0; i < EventMgr.CurrentEvent.EventSelectionDetail.Length; i++)
        {
            EventSelectionButtons[i].SetActive(true);
            EventSelectionTexts[i].text = EventMgr.CurrentEvent.EventSelectionDetail[i];
        }
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        gameObject.SetActive(true);
        gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
    }

    public void InActiveEventUI()
    {
        if(gameObject.activeSelf == true)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 1;
            gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
        }

    }

}
