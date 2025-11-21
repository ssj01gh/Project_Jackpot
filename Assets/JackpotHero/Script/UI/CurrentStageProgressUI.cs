using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentStageProgressUI : MonoBehaviour
{
    public TextMeshProUGUI CurrentStageText;
    public TextMeshProUGUI CurrentStageProgressText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentStegeUI(PlayerInfo PInfo)
    {
        CurrentStageText.text = (PInfo.CurrentFloor).ToString() +"스테이지";
        if(PInfo.DetectNextFloorPoint < 100)
        {
            CurrentStageProgressText.text = "탐색도 : " + PInfo.DetectNextFloorPoint + " / 100";
        }
        else
        {
            CurrentStageProgressText.text = "탐색도 : <color=red>100 / 100</color>";
        }
    }
}
