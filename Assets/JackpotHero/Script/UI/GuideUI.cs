using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum EGuideMessage
{
    AttackGuideMessage,
    NotEnoughSTAMessage
}

public class GuideUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI GuideMessageText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveGuideMessageUI(int MessageType)
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -115);
        gameObject.SetActive(true);
        gameObject.GetComponent<RectTransform>().DOAnchorPosY(115, 0.5f).SetEase(Ease.OutBack);
        StopAllCoroutines();
        switch (MessageType)
        {
            case (int)EGuideMessage.AttackGuideMessage:
                GuideMessageText.text = "공격의 목표가 지정되지 않았습니다.\r\n순서 표시창의 얼굴 아이콘을 클릭하거나 공격하고 싶은 적을 클릭해 목표를 지정해 주세요.";
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughSTAMessage:
                GuideMessageText.text = "피로도가 부족합니다.\r\n피로도 회복을 통해 피로도를 회복해 주세요.";
                StartCoroutine(CountSeconds());
                break;
            default:
                break;
        }
    }

    IEnumerator CountSeconds()
    {
        yield return new WaitForSeconds(10f);
        if(gameObject.activeSelf == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 115);
            gameObject.GetComponent<RectTransform>().DOAnchorPosY(-115, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}
