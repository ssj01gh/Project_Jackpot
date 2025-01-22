using UnityEngine;

[CreateAssetMenu(fileName = "EarlySO", menuName = "SO/EarlySO")]
public class EarlyStrengthenDetailSO : ScriptableObject
{
    public string EarlyDetailTitle;
    [TextArea]
    public string DetailText;
}
