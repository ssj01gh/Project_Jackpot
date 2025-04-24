using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffImageUIContainer : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    [SerializeField]
    protected Image[] BuffImages;
    [SerializeField]
    protected TextMeshProUGUI[] BuffText;
    [SerializeField]
    protected GameObject BuffDetailInfoObject;
    [SerializeField]
    protected TextMeshProUGUI BuffDetailTitle;
    [SerializeField]
    protected TextMeshProUGUI BuffDetailText;

    protected int[] ActiveBuffTypeCode = new int[14];//�ϴ� 20�� ���� 20���̴ϱ�?
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitBuffImage()
    {
        gameObject.SetActive(false);
        for(int i = 0; i < BuffImages.Length; i++)
        {
            BuffText[i].text = "";
            BuffImages[i].gameObject.SetActive(false);
            ActiveBuffTypeCode[i] = -1;
        }
        BuffDetailInfoObject.SetActive(false);
    }

    public void ActiveBuffImage(int[] BuffList, Vector2 Position)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = Position;
        for(int i = 0; i < BuffList.Length; i++)
        {//i�� ������ �ڵ�
            if (BuffList[i] > 0)
            {
                for(int j = 0; j < BuffImages.Length; j++)
                {
                    if (BuffImages[j].gameObject.activeSelf == false)
                    {
                        BuffImages[j].gameObject.SetActive(true);
                        BuffImages[j].sprite = BuffInfoManager.Instance.GetBuffInfo(i).BuffImage;

                        if (i == (int)EBuffType.Defenseless)
                            BuffText[j].text = "";
                        else
                            BuffText[j].text = BuffList[i].ToString();

                        ActiveBuffTypeCode[j] = i;
                        break;
                    }
                }
            }
        }
    }

    public void ActiveBuffDetailUI(GameObject BuffObj)
    {
        for(int i = 0; i < BuffImages.Length; i++)
        {
            if (BuffImages[i].gameObject == BuffObj)//���콺�� �ö� ���� �̹����� �̰Ŷ��
            {
                BuffDetailInfoObject.SetActive(true);
                BuffDetailInfoObject.transform.position = BuffObj.transform.position;
                BuffDetailTitle.text = BuffInfoManager.Instance.GetBuffInfo(ActiveBuffTypeCode[i]).BuffName;
                BuffDetailText.text = BuffInfoManager.Instance.GetBuffInfo(ActiveBuffTypeCode[i]).BuffDetail;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //eventData.pointerEnter
        ActiveBuffDetailUI(eventData.pointerEnter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BuffDetailInfoObject.SetActive(false);
    }
}
