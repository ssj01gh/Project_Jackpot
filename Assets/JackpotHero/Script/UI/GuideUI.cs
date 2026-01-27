using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public enum EGuideMessage
{
    AttackGuideMessage,
    NotEnoughSTAMessage_Battle,
    NotEnoughInventoryMessage,
    NotEnoughSTAMessage_RestQuality,
    NotEnoughEXP_PlayerUpgrade,
    NotEnoughEXP_ForgeEvent,
    NoEnoughEnergy_FoggedForest,
}

public class GuideUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI GuideMessageText;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActiveGuideMessageUI(int MessageType)
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -115);
        gameObject.SetActive(true);
        gameObject.GetComponent<RectTransform>().DOAnchorPosY(115, 0.5f);
        StopAllCoroutines();
        switch (MessageType)
        {
            case (int)EGuideMessage.AttackGuideMessage:
                //GuideMessageText.text = "공격의 목표가 지정되지 않았습니다.\r\n순서 표시창의 얼굴 아이콘을 클릭하거나 공격하고 싶은 적을 클릭해 목표를 지정해 주세요.";
                StartCoroutine(LoadGuideMessage("PS_GM_MissTarget"));
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughSTAMessage_Battle:
                //GuideMessageText.text = "피로도가 부족합니다.\r\n피로도 회복을 통해 피로도를 회복해 주세요.";
                StartCoroutine(LoadGuideMessage("PS_GM_BattleNoSTA"));
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughInventoryMessage:
                //GuideMessageText.text = "장비를 보관할 인벤토리가 부족합니다.\r\n인벤토리속 장비를 처분해서 인벤토리의 공간을 확보해 주세요.";
                StartCoroutine(LoadGuideMessage("PS_GM_NoInven"));
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughSTAMessage_RestQuality:
                //GuideMessageText.text = "피로도가 부족합니다.\r\n낮은 품질의 야영지를 설치해 주세요.";
                StartCoroutine(LoadGuideMessage("PS_GM_CampingNoSTA"));
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughEXP_PlayerUpgrade:
                //GuideMessageText.text = "플레이어를 강화할 경험치가 부족합니다.";
                StartCoroutine(LoadGuideMessage("PS_GM_LevelUPNoEXP"));
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NotEnoughEXP_ForgeEvent:
                //GuideMessageText.text = "경험치가 부족합니다.";
                StartCoroutine(LoadGuideMessage("PS_GM_NoEXP"));
                StartCoroutine(CountSeconds());
                break;
            case (int)EGuideMessage.NoEnoughEnergy_FoggedForest:
                //GuideMessageText.text = "안개 낀 숲을 탐색할 피로도가 부족합니다.";
                StartCoroutine(LoadGuideMessage("PS_GM_Fog_NoSTA"));
                StartCoroutine(CountSeconds());
                break;
            default:
                break;
        }
    }
    IEnumerator LoadGuideMessage(string GuideMessageKey)
    {
        yield return LocalizationSettings.InitializationOperation;

        var TutorialTable = LocalizationSettings.StringDatabase.GetTable("PlaySceneShortText");
        GuideMessageText.text = TutorialTable.GetEntry(GuideMessageKey).GetLocalizedString();
    }
    IEnumerator CountSeconds()
    {
        yield return new WaitForSeconds(5f);
        if(gameObject.activeSelf == true)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 115);
            gameObject.GetComponent<RectTransform>().DOAnchorPosY(-115, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}
