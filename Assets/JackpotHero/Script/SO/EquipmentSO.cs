using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EquipmentInfo
{
    public int EquipmentType;
    public int EquipmentTier;
    public int EquipmentCode;
    public string EquipmentName;
    public float SpendTiredness;
    public EquipmentSlot[] EquipmentSlots;
    public Sprite EquipmentImage;
    public int AddSTRAmount;
    public int AddDURAmount;
    public int AddRESAmount;
    public int AddSPDAmount;
    public int AddLUKAmount;
    public float AddHPAmount;
    public float AddTirednessAmount;
    public string EquipmentDetail;
}

[System.Serializable]
public class EquipmentSlot//한 슬롯에는 n개 까지의 효과가 들어갈수 있음
{
    public bool[] IsPositive;
    public float[] SlotState;
}

[CreateAssetMenu(fileName = "EquipmentSO", menuName = "SO/EquipmentSO")]
public class EquipmentSO : ScriptableObject
{
    public int EquipmentType;
    public int EquipmentTier;
    public int EquipmentCode;
    public string EquipmentName;
    public float SpendTiredness;
    public EquipmentSlot[] EquipmentSlots;
    public Sprite EquipmentImage;
    public int AddSTRAmount;
    public int AddDURAmount;
    public int AddRESAmount;
    public int AddSPDAmount;
    public int AddLUKAmount;
    public float AddHPAmount;
    public float AddTirednessAmount;
    [TextArea(10, 20)]
    public string EquipmentDetail;
}