using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventSO", menuName = "SO/EventSO")]
public class EventSO : ScriptableObject
{
    public int EventCode;
    public string EventTitle;
    public Sprite EventImage;
    [TextArea(10,20)]
    public string EventDetail;
    public string[] EventSelectionDetail;
}
