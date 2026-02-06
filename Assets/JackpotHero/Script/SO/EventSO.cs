using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventSOInfo
{
    public int EventCode;
    public string EventTitle;
    public Sprite EventImage;
    public string EventDetail;
    public List<string> EventSelectionDetail = new List<string>();
}

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
