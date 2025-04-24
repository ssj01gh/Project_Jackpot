using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailInfoUI : MonoBehaviour
{
    public Image ClickEquipImage;
    public TextMeshProUGUI ClickEquipName;
    public TextMeshProUGUI ClickEquipSpendSTA;

    public TextMeshProUGUI ClickEquipAddSTR;
    public TextMeshProUGUI ClickEquipAddDUR;
    public TextMeshProUGUI ClickEquipAddRES;
    public TextMeshProUGUI ClickEquipAddSPD;
    public TextMeshProUGUI ClickEquipAddLUK;

    public GameObject[] CardContainers; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveEquipmentDetailInfoUI(EquipmentSO EquipInfo, bool IsPlayerEquipment)
    {
        gameObject.SetActive(true);

        foreach(GameObject obj in CardContainers)
        {
            obj.SetActive(false);
        }

        int EquipCode = (EquipInfo.EquipmentType * 10000) + (EquipInfo.EquipmentTier * 1000) + EquipInfo.EquipmentCode;
        if (EquipCode == 0)
            return;

        gameObject.GetComponent<RectTransform>().localScale = Vector2.zero;
        gameObject.GetComponent<RectTransform>().DOScale(Vector2.one, 0.5f).SetEase(Ease.OutBack);
        ClickEquipImage.sprite = EquipInfo.EquipmentImage;
        ClickEquipName.text = EquipInfo.EquipmentName;

        if(IsPlayerEquipment == true)
        {
            

            if (EquipCode >= 10000 && EquipCode < 20000)
                ClickEquipSpendSTA.text = "���ݽ� �Ƿε� : " + EquipInfo.SpendTiredness.ToString();
            else if (EquipCode >= 20000 && EquipCode < 30000)
                ClickEquipSpendSTA.text = "���� �Ƿε� : " + EquipInfo.SpendTiredness.ToString();
            else if (EquipCode >= 30000 && EquipCode < 40000)
                ClickEquipSpendSTA.text = "�Ƿε� ȸ���� �Ƿε��� ������ �ʽ��ϴ�.";
            else if (EquipCode >= 40000 && EquipCode < 50000)
                ClickEquipSpendSTA.text = "�Ź��� �Ƿε��� ������� �ʽ��ϴ�.";
            else if (EquipCode >= 50000 && EquipCode < 60000)
                ClickEquipSpendSTA.text = "��ű��� �Ƿε��� ������� �ʽ��ϴ�.";
            else
                ClickEquipSpendSTA.text = "??�� �Ƿε� : " + EquipInfo.SpendTiredness.ToString();

            ClickEquipAddSTR.text = EquipInfo.AddSTRAmount.ToString();
            ClickEquipAddDUR.text = EquipInfo.AddDURAmount.ToString();
            ClickEquipAddRES.text = EquipInfo.AddRESAmount.ToString();
            ClickEquipAddSPD.text = EquipInfo.AddSPDAmount.ToString();
            ClickEquipAddLUK.text = EquipInfo.AddLUKAmount.ToString();
        }
        else
        {
            EquipCode = EquipInfo.EquipmentCode;

            if(EquipCode >= 70000 && EquipCode < 80000)
            {
                ClickEquipSpendSTA.text = "�ش� ������ �����Դϴ�.";
            }
            else if(EquipCode >= 80000 && EquipCode < 90000)
            {
                ClickEquipSpendSTA.text = "�ش� ������ ����Դϴ�.";
            }
            else if(EquipCode >= 90000 && EquipCode < 100000)
            {
                ClickEquipSpendSTA.text = "�ش� ������ Ư���� �ൿ�Դϴ�.";
            }
            else
            {

            }
        }
        for(int i = 0; i < EquipInfo.EquipmentSlots.Length; i++)
        {
            //Ȱ��ȭ
            CardContainers[i].SetActive(true);
            CardContainers[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(EquipInfo.EquipmentSlots[i].SlotState);
        }
    }

    public void InActiveEquipmentDetailInfoUI()
    {
        if (gameObject.activeSelf == false)
            return;

        SoundManager.Instance.PlayUISFX("UI_Button");
        gameObject.GetComponent<RectTransform>().localScale = Vector2.one;
        gameObject.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() => { gameObject.SetActive(false); });
    }
}
