using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EquipmentSlot//�� ���Կ��� n�� ������ ȿ���� ���� ����
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
}