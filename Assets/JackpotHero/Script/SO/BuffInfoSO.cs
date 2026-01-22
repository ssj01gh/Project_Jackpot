using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuffSOInfo
{
    public Sprite BuffImage;
    public string BuffName;
    public string BuffDetail;
}


[CreateAssetMenu(fileName = "BuffSO", menuName = "SO/BuffSO")]
public class BuffInfoSO : ScriptableObject
{
    public Sprite BuffImage;
    public string BuffName;
    [TextArea(10, 20)]
    public string BuffDetail;

    public string BuffNameEN;
    [TextArea(10, 20)]
    public string BuffDetailEN;

    public string BuffNameJA;
    [TextArea(10, 20)]
    public string BuffDetailJA;
}
