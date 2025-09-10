using UnityEngine;

[CreateAssetMenu(fileName = "EarlySO", menuName = "SO/EarlySO")]
public class EarlyStrengthenDetailSO : ScriptableObject
{
    public string EarlyDetailTitle;
    [TextArea(5, 20)]
    public string DetailText;
}
