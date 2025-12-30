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
        CurrentStageText.text = (PInfo.CurrentFloor).ToString() + "½ºÅ×ÀÌÁö";
        if (PInfo.CurrentFloor == 4)
        {
            CurrentStageProgressText.text = "Å½»öµµ : <color=red>???</color>";
        }
        else
        {
            if (PInfo.DetectNextFloorPoint < 100)
            {
                CurrentStageProgressText.text = "Å½»öµµ : " + PInfo.DetectNextFloorPoint + " / 100";
            }
            else
            {
                CurrentStageProgressText.text = "Å½»öµµ : <color=red>100 / 100</color>";
            }
        }
    }
}
