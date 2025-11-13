using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EquipSlotSO", menuName = "SO/EquipSlotSO")]
public class EquipSlotSO : ScriptableObject
{
    public int IsEventEquip;
    public int EquipTier;
    public int EquipMultiType;
    public EquipmentSlot[] EquipmentSlots;
}
