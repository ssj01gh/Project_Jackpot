using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipmentUI : MonoBehaviour
{
    public Image WeaponImage;
    public Image ArmorImage;
    public Image HelmetImage;
    public Image BootsIamge;
    public Image AccessoriesImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEquipmentImage(PlayerInfo PInfo)
    {
        WeaponImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PInfo.EquipWeaponCode).EquipmentImage;
        ArmorImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PInfo.EquipArmorCode).EquipmentImage;
        HelmetImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PInfo.EquipHatCode).EquipmentImage;
        BootsIamge.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PInfo.EquipShoesCode).EquipmentImage;
        AccessoriesImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PInfo.EquipAccessoriesCode).EquipmentImage;
    }
}
