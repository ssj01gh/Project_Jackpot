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
                GuideMessageText.text = "������ ��ǥ�� �������� �ʾҽ��ϴ�.\r\n���� ǥ��â�� �� �������� Ŭ���ϰų� �����ϰ� ���� ���� Ŭ���� ��ǥ�� ������ �ּ���.";
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughSTAMessage:
                GuideMessageText.text = "�Ƿε��� �����մϴ�.\r\n�Ƿε� ȸ���� ���� �Ƿε��� ȸ���� �ּ���.";
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
