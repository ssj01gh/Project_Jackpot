using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class EventUIScript : MonoBehaviour
{
    public TextMeshProUGUI EventTitle;
    public Image UIEventImage;
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
        UIEventImage.sprite = EventMgr.CurrentEvent.EventImage;
        EventDetail.text = EventMgr.CurrentEvent.EventDetail;
        if(EventMgr.Getting == "")
        {
            //{Getting}
            EventDetail.text = Regex.Replace(EventDetail.text, @".*\{Getting\}.*\n?", EventMgr.Getting, RegexOptions.Multiline);
        }
        else
        {
            EventDetail.text = Regex.Replace(EventDetail.text, "{Getting}", EventMgr.Getting);
        }
        if (EventMgr.Losing == "")
        {
            //{Losing}
            EventDetail.text = Regex.Replace(EventDetail.text, @".*\{Losing\}.*\n?", EventMgr.Losing, RegexOptions.Multiline);
        }
        else
        {
            EventDetail.text = Regex.Replace(EventDetail.text, "{Losing}", EventMgr.Losing);
        }

        foreach (GameObject Obj in EventSelectionButtons)
        {
            Obj.SetActive(false);
            Obj.GetComponent<Button>().interactable = false;
        }
        for(int i = 0; i < EventMgr.CurrentEvent.EventSelectionDetail.Count; i++)
        {
            EventSelectionButtons[i].SetActive(true);
            EventSelectionButtons[i].GetComponent<Button>().interactable = true;
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
            for(int i = 0; i < EventSelectionButtons.Length; i++)
            {
                if (EventSelectionButtons[i].activeSelf == true)
                {
                    EventSelectionButtons[i].GetComponent<Button>().interactable = false;
                }
            }
            gameObject.GetComponent<CanvasGroup>().alpha = 1;
            gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
        }

    }

}
