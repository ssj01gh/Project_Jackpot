using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EarlyStrengthTextLink : MonoBehaviour
{
    public TextMeshProUGUI EarlyStrenghExplaneText;
    public GameObject DetailExplainObject;
    public TextMeshProUGUI DetailExplainText;

    private int CurrentLinkIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        
        if (DetailExplainObject != null)
        {
            DetailExplainObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DetailExplainObject is null at Start.");
        }
        //DetailExplainObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 mousePosition = Input.mousePosition;

        // ��ũ �ε����� ã��
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(EarlyStrenghExplaneText, mousePosition, Camera.main);

        if (linkIndex != CurrentLinkIndex)
        {
            // ���� ��ũ���� ���콺�� ����� ��
            if (CurrentLinkIndex != -1)
            {
                string oldID = EarlyStrenghExplaneText.textInfo.linkInfo[CurrentLinkIndex].GetLinkID();
                OnLinkExit(oldID);
            }

            // ���ο� ��ũ�� ���콺�� �ö��� ��
            if (linkIndex != -1)
            {
                string newID = EarlyStrenghExplaneText.textInfo.linkInfo[linkIndex].GetLinkID();
                OnLinkEnter(newID);
            }

            CurrentLinkIndex = linkIndex;
        }
        
    }

    private void OnLinkEnter(string id)
    {
        /*
         * DetailExplainText.text = "���� �ִ� ���� (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
    ")\r\n�Ϲ� ���� (" + PlayerInfo.GetPlayerStateInfo().KillNormalMonster +
    ")\r\n����Ʈ ���� (" + PlayerInfo.GetPlayerStateInfo().KillEliteMonster +
    ")\r\n���� ����ġ (" + PlayerInfo.GetPlayerStateInfo().Experience +
    ")\r\n���� ����� (" + PlayerInfo.GetPlayerStateInfo().GoodKarma + ")";
         */
        DetailExplainObject.SetActive(true);
        switch (id)
        {
            case "STR07":
                DetailExplainText.text =
                    "<size=40>��ġ�� ��</size>" + "\r\n\r\n" +
                    "���� ���ݽ� ���� ��������" + "\r\n" +
                    "�ʰ��� ��������ŭ" + "\r\n" +
                    "�ٸ� ���鿡�� �������� �ݴϴ�.";
                break;
            case "DUR07":
                DetailExplainText.text =
                    "<size=40>���� ����</size>" + "\r\n\r\n" +
                    "������ ������ ��ȣ���� �����ִٸ�" + "\r\n" +
                    "���� ��ȣ�� ��ġ��ŭ �ش� ������" + "\r\n" +
                    "�������� �ݴϴ�.";
                break;
            case "RES07":
                DetailExplainText.text =
                    "<size=40>��� �޽�</size>" + "\r\n\r\n" +
                    "������ �Ƿε� ȸ����" + "\r\n" +
                    "�Ƿε� ȸ������ 10% ��ŭ" + "\r\n" +
                    "ü���� ȸ���մϴ�.";
                break;
            case "SPD07":
                DetailExplainText.text =
                    "<size=40>�����</size>" + "\r\n\r\n" +
                    "���� ������ ���� �������� ������" + "\r\n" +
                    "�������� 2��� �޽��ϴ�." + "\r\n" +
                    "�ڽ��� ���� ������" + "\r\n" +
                    "ȿ���� ����˴ϴ�.";
                break;
            case "HP07":
                DetailExplainText.text =
                    "<size=40>���� ����</size>" + "\r\n\r\n" +
                    "������ ��� ��ġ ����" + "\r\n" +
                    "�ִ� ü���� 5% ��ŭ��" + "\r\n" +
                    "���� ��ġ�� �߰��˴ϴ�.";
                break;
            case "STA07":
                DetailExplainText.text =
                    "<size=40>�Ƿε� ����</size>" + "\r\n\r\n" +
                    "���� �Ƿε��� ����Ͽ�" + "\r\n" +
                    "�Ƿε� ��뷮��" + "\r\n"+
                    "�ִ� 90% �����մϴ�.";
                break;
            case "EXP07":
                DetailExplainText.text =
                    "<size=40>����</size>" + "\r\n\r\n" +
                    "������ �������� �� ������" + "\r\n" +
                    "20%��ŭ ����ġ�� �����մϴ�.";
                break;
            case "EXPMG07":
                DetailExplainText.text =
                    "<size=40>������ ��</size>" + "\r\n\r\n" +
                    "������ ��� ��ġ ��꿡" + "\r\n" +
                    "����ġ�� 10%��ŭ" + "\r\n" +
                    "���� ��ġ�� �߰��մϴ�.";
                break;
            case "EQUIP07":
                DetailExplainText.text =
                    "<size=40>���� ������</size>" + "\r\n\r\n" +
                    "������ ��� ��ġ ��꿡" + "\r\n" +
                    "�ڽ��� ������ �ִ�" + "\r\n"+
                    "��� ����� Ƽ�� ��ġ��ŭ��" + "\r\n"+
                    "���� ��ġ�� �߰� �˴ϴ�.";
                break;
        }
        //Debug.Log($"���콺 �ö�: {id}");
    }

    private void OnLinkExit(string id)
    {
        DetailExplainObject.SetActive(false);
        // ��: ���� ���� ����, ���� ����� ��
        //Debug.Log($"���콺 ���: {id}");
    }
}
