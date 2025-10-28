using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipIncreaseStateSO", menuName = "SO/EquipIncreaseStateSO")]
public class EquipIncreaseSO : ScriptableObject
{
    public int EquipStateType;
    public int EquipType;
    public int IncreaseSTR;
    public int IncreaseDUR;
    public int IncreaseRES;
    public int IncreaseSPD;
    public int IncreaseLUK;
    public int SpendTired;
}