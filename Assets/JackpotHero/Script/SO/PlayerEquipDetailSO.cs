using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipDetailSO", menuName = "SO/EquipDetailSO")]
public class PlayerEquipDetailSO : ScriptableObject
{
    public int EquipStateType;
    public int EquipType;
    public string EquipmentName;
    [TextArea(10, 20)]
    public string EquipmentDetail;
}
