using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffSO", menuName = "SO/BuffSO")]
public class BuffInfoSO : ScriptableObject
{
    public Sprite BuffImage;
    public string BuffName;
    [TextArea]
    public string BuffDetail;
}
