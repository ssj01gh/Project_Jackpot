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
        float DetectNextFloorRatio = ((float)PInfo.DetectNextFloorPoint / (250f + PInfo.DetectNextFloorPoint)) * 100f;
        CurrentStageText.text = (PInfo.CurrentFloor).ToString() +"Ãþ";
        CurrentStageProgressText.text = "´ÙÀ½Ãþ ¹ß°ß È®·ü : " + DetectNextFloorRatio.ToString("F1") + "%";
    }
}
