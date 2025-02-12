using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class EquipmentInfoManager : MonoSingleton<EquipmentInfoManager>
{
    [Header("EquipmentSlotSprite")]
    public float[] EquipmentSlotIDs;
    public Sprite[] EquipmentSlotSprites;
    [Header("PlayerWeapon")]
    public EquipmentSO[] PlayerWeaponSOs;
    [Header("PlayerArmor")]
    public EquipmentSO[] PlayerArmorSOs;
    [Header("PlayerHelmet")]
    public EquipmentSO[] PlayerHelmetSOs;
    [Header("PlayerBoots")]
    public EquipmentSO[] PlayerBootsSOs;
    [Header("PlayerAsseccories")]
    public EquipmentSO[] PlayerAccessoriesSOs;

    [Header("MonWeapon")]
    public EquipmentSO[] MonWeaponSOs;
    [Header("MonArmor")]
    public EquipmentSO[] MonArmorSOs;
    [Header("MonAnotherEquip")]
    public EquipmentSO[] MonAnotherEquipSOs;

    protected Dictionary<float, Sprite> EquipmentSlotSpriteStorage = new Dictionary<float, Sprite>();
    protected Dictionary<int, EquipmentSO> EquipmentStorage = new Dictionary<int, EquipmentSO>();

    protected Dictionary<int, EquipmentSO> MonEquipmentStroage = new Dictionary<int, EquipmentSO>();
    protected override void Awake()
    {
        base.Awake();
        InitEquipmentStorage();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitEquipmentStorage()
    {
        //InitEquipmentSlotSprite
        for(int i = 0; i < EquipmentSlotSprites.Length; i++)
        {
            if (!EquipmentSlotSpriteStorage.ContainsKey(EquipmentSlotIDs[i]))
            {
                EquipmentSlotSpriteStorage.Add(EquipmentSlotIDs[i], EquipmentSlotSprites[i]);
            }
        }
        //InitPlayerWeapon
        for(int i = 0; i < PlayerWeaponSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerWeaponSOs[i].EquipmentType * 10000) + (PlayerWeaponSOs[i].EquipmentTier * 1000) + PlayerWeaponSOs[i].EquipmentCode;
            if(!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerWeaponSOs[i]);
            }
        }
        //InitPlayerArmor
        for (int i = 0; i < PlayerArmorSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerArmorSOs[i].EquipmentType * 10000) + (PlayerArmorSOs[i].EquipmentTier * 1000) + PlayerArmorSOs[i].EquipmentCode;
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerArmorSOs[i]);
            }
        }
        //InitPlayerHelmet
        for (int i = 0; i < PlayerHelmetSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerHelmetSOs[i].EquipmentType * 10000) + (PlayerHelmetSOs[i].EquipmentTier * 1000) + PlayerHelmetSOs[i].EquipmentCode;
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerHelmetSOs[i]);
            }
        }
        //InitPlayerBoots
        for (int i = 0; i < PlayerBootsSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerBootsSOs[i].EquipmentType * 10000) + (PlayerBootsSOs[i].EquipmentTier * 1000) + PlayerBootsSOs[i].EquipmentCode;
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerBootsSOs[i]);
            }
        }
        //InitPlayerAccessories
        for (int i = 0; i < PlayerAccessoriesSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerAccessoriesSOs[i].EquipmentType * 10000) + (PlayerAccessoriesSOs[i].EquipmentTier * 1000) + PlayerAccessoriesSOs[i].EquipmentCode;
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerAccessoriesSOs[i]);
            }
        }
        //InitMonWeapon
        for (int i = 0; i < MonWeaponSOs.Length; i++)
        {
            int i_EquipmentCode = MonWeaponSOs[i].EquipmentCode;
            if (!MonEquipmentStroage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                MonEquipmentStroage.Add(i_EquipmentCode, MonWeaponSOs[i]);
            }
        }
        //IntiMonArmor
        for (int i = 0; i < MonArmorSOs.Length; i++)
        {
            int i_EquipmentCode = MonArmorSOs[i].EquipmentCode;
            if (!MonEquipmentStroage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                MonEquipmentStroage.Add(i_EquipmentCode, MonArmorSOs[i]);
            }
        }
        //InitMonAnotherEquip
        for (int i = 0; i < MonAnotherEquipSOs.Length; i++)
        {
            int i_EquipmentCode = MonAnotherEquipSOs[i].EquipmentCode;
            if (!MonEquipmentStroage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                MonEquipmentStroage.Add(i_EquipmentCode, MonAnotherEquipSOs[i]);
            }
        }
    }
    public Sprite GetEquipmentSlotSprite(float SlotAmount)
    {
        if(!EquipmentSlotSpriteStorage.ContainsKey(SlotAmount))//없다면
        {
            return EquipmentSlotSpriteStorage[-1];
        }
        return EquipmentSlotSpriteStorage[SlotAmount];
    }
    
    public EquipmentSO GetPlayerEquipmentInfo(int EquipmentCode)
    {
        if (!EquipmentStorage.ContainsKey(EquipmentCode))//EquipmentStorage에 없으면 0번 장비(아무것도 아닌것)전달
        {
            return EquipmentStorage[0];
        }
        return EquipmentStorage[EquipmentCode];
    }
    public EquipmentSO GetMonEquipmentInfo(int EquipmentCode)
    {
        if (!MonEquipmentStroage.ContainsKey(EquipmentCode))//MonEquipmentStroage에 없으면 0번 장비(아무것도 아닌것)전달
        {
            return MonEquipmentStroage[0];
        }
        return MonEquipmentStroage[EquipmentCode];
    }
}
