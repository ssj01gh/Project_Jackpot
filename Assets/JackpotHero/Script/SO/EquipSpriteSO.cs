using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipSpriteSO", menuName = "SO/EquipSpriteSO")]
public class EquipSpriteSO : ScriptableObject
{
    public int IsEventEquip;
    public int EqupTier;
    public int EquipStateType;
    public int EquipType;
    public Sprite EquipSprite;
}
