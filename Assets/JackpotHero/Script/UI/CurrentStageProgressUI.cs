using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class CurrentStageProgressUI : MonoBehaviour
{
    public TextMeshProUGUI CurrentStageText;
    public TextMeshProUGUI CurrentStageProgressText;
    // Start is called before the first frame update
    private Coroutine CSTextCor;
    private string LanKey;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentStegeUI(PlayerInfo PInfo)
    {
        if(CSTextCor != null)
        {
            StopCoroutine(CSTextCor);
            CSTextCor = null;
        }
        LanKey = "";
        CurrentStageText.text = "";
        switch (PInfo.CurrentFloor)
        {
            case 1:
                LanKey = "PS_SCStage01";
                break;
            case 2:
                LanKey = "PS_SCStage02";
                break;
            case 3:
                LanKey = "PS_SCStage03";
                break;
            case 4:
                LanKey = "PS_SCStage04";
                break;
            default:
                LanKey = "PS_SCStageError";
                break;
        }
        LanKey = "PS_SCStage01";
        StartCoroutine(Load(LanKey));
        //PlaySceneShortText

        if (PInfo.CurrentFloor == 4)
        {
            CurrentStageProgressText.text = "<color=red>???</color>";
        }
        else
        {
            if (PInfo.DetectNextFloorPoint < 100)
            {
                CurrentStageProgressText.text = PInfo.DetectNextFloorPoint + " / 100";
            }
            else
            {
                CurrentStageProgressText.text = "<color=red>100 / 100</color>";
            }
        }
    }

    private IEnumerator Load(string key)
    {
        yield return LocalizationSettings.InitializationOperation;

        var BuffDetailTable = LocalizationSettings.StringDatabase.GetTable("PlaySceneShortText");
        CurrentStageText.text = BuffDetailTable.GetEntry(key).GetLocalizedString();
    }
}
