using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GettingItenUIScript : MonoBehaviour
{
    public GameObject EXPObject;
    public GameObject HelmetObject;
    public GameObject WeaponObject;
    public GameObject ArmorObject;
    public GameObject BootsObject;
    public GameObject JewelryObject;

    protected Vector3[] PathPoint = new Vector3[] { new Vector3(0, 0, 0), new Vector3(-350, -200, 0), new Vector3(-700, 0, 0) };
    // Start is called before the first frame update
    void Start()
    {
        InitGettingUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitGettingUI()
    {
        EXPObject.SetActive(false);
        HelmetObject.SetActive(false);
        WeaponObject.SetActive(false);
        ArmorObject.SetActive(false);
        BootsObject.SetActive(false);
        JewelryObject.SetActive(false);
    }

    public void ActiveGettingUI(int EquipmentCode = 0)
    {
        string GettingType = "EXP";
        switch(EquipmentCode / 10000)
        {
            case 0://°æÇèÄ¡
                GettingType = "EXP";
                break;
            case 1://¹«±â
                GettingType = "Weapon";
                break;
            case 2://°©¿Ê
                GettingType = "Armor";
                break;
            case 3://Çï¸Ë
                GettingType = "Helmet";
                break;
            case 4://ºÎÃ÷
                GettingType = "Boots";
                break;
            case 5://¾Ç¼¼»ç¸®
                GettingType = "Jewelry";
                break;
        }
        switch(GettingType)
        {
            case "EXP":
                EXPObject.SetActive(true);
                EXPObject.GetComponent<RectTransform>().localScale = Vector3.one;
                EXPObject.GetComponent<RectTransform>().DOLocalPath(PathPoint, 0.5f, PathType.CatmullRom);
                EXPObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f).OnComplete(() => { EXPObject.SetActive(false); });
                //³ªÁß¿¡ ³¡³ª¸é ´Ù¸¥ ÀÌÆåÆ®/ »ç¿îµåµµ
                break;
            case "Weapon":
                WeaponObject.SetActive(true);
                WeaponObject.GetComponent<RectTransform>().localScale = Vector3.one;
                WeaponObject.GetComponent<RectTransform>().DOLocalPath(PathPoint, 0.5f, PathType.CatmullRom);
                WeaponObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f).OnComplete(() => { EXPObject.SetActive(false); });
                break;
            case "Armor":
                ArmorObject.SetActive(true);
                ArmorObject.GetComponent<RectTransform>().localScale = Vector3.one;
                ArmorObject.GetComponent<RectTransform>().DOLocalPath(PathPoint, 0.5f, PathType.CatmullRom);
                ArmorObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f).OnComplete(() => { EXPObject.SetActive(false); });
                break;
            case "Helmet":
                HelmetObject.SetActive(true);
                HelmetObject.GetComponent<RectTransform>().localScale = Vector3.one;
                HelmetObject.GetComponent<RectTransform>().DOLocalPath(PathPoint, 0.5f, PathType.CatmullRom);
                HelmetObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f).OnComplete(() => { EXPObject.SetActive(false); });
                break;
            case "Boots":
                BootsObject.SetActive(true);
                BootsObject.GetComponent<RectTransform>().localScale = Vector3.one;
                BootsObject.GetComponent<RectTransform>().DOLocalPath(PathPoint, 0.5f, PathType.CatmullRom);
                BootsObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f).OnComplete(() => { EXPObject.SetActive(false); });
                break;
            case "Jewelry":
                JewelryObject.SetActive(true);
                JewelryObject.GetComponent<RectTransform>().localScale = Vector3.one;
                JewelryObject.GetComponent<RectTransform>().DOLocalPath(PathPoint, 0.5f, PathType.CatmullRom);
                JewelryObject.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f).OnComplete(() => { EXPObject.SetActive(false); });
                break;
        }
    }
}
