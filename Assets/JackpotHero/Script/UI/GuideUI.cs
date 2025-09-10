using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum EGuideMessage
{
    AttackGuideMessage,
    NotEnoughSTAMessage_Battle,
    NotEnoughInventoryMessage,
    NotEnoughSTAMessage_RestQuality,
    NotEnoughEXP_PlayerUpgrade,
    NotEnoughEXP_ForgeEvent,
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
        gameObject.GetComponent<RectTransform>().DOAnchorPosY(115, 0.5f);
        StopAllCoroutines();
        switch (MessageType)
        {
            case (int)EGuideMessage.AttackGuideMessage:
                GuideMessageText.text = "������ ��ǥ�� �������� �ʾҽ��ϴ�.\r\n���� ǥ��â�� �� �������� Ŭ���ϰų� �����ϰ� ���� ���� Ŭ���� ��ǥ�� ������ �ּ���.";
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughSTAMessage_Battle:
                GuideMessageText.text = "�Ƿε��� �����մϴ�.\r\n�Ƿε� ȸ���� ���� �Ƿε��� ȸ���� �ּ���.";
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughInventoryMessage:
                GuideMessageText.text = "��� ������ �κ��丮�� �����մϴ�.\r\n�κ��丮�� ��� ó���ؼ� �κ��丮�� ������ Ȯ���� �ּ���.";
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughSTAMessage_RestQuality:
                GuideMessageText.text = "�Ƿε��� �����մϴ�.\r\n���� ǰ���� �߿����� ��ġ�� �ּ���.";
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughEXP_PlayerUpgrade:
                GuideMessageText.text = "�÷��̾ ��ȭ�� ����ġ�� �����մϴ�.";
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughEXP_ForgeEvent:
                GuideMessageText.text = "����ġ�� �����մϴ�.";
                StartCoroutine(CountSeconds());
                break;
            default:
                break;
        }
    }

    IEnumerator CountSeconds()
    {
        yield return new WaitForSeconds(5f);
        if(gameObject.activeSelf == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 115);
            gameObject.GetComponent<RectTransform>().DOAnchorPosY(-115, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}
