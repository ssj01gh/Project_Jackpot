using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

public class EarlyStrengthTextLink : MonoBehaviour
{
    public TextMeshProUGUI EarlyStrenghExplaneText;
    public GameObject DetailExplainObject;
    public TextMeshProUGUI DetailExplainText;

    private int CurrentLinkIndex = -1;
    private Coroutine _Corou;
    // Start is called before the first frame update
    void Start()
    {
        
        if (DetailExplainObject != null)
        {
            DetailExplainObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("DetailExplainObject is null at Start.");
        }
        //DetailExplainObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 mousePosition = Input.mousePosition;

        // 링크 인덱스를 찾음
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(EarlyStrenghExplaneText, mousePosition, Camera.main);

        if (linkIndex != CurrentLinkIndex)
        {
            // 기존 링크에서 마우스가 벗어났을 때
            if (CurrentLinkIndex != -1)
            {
                string oldID = EarlyStrenghExplaneText.textInfo.linkInfo[CurrentLinkIndex].GetLinkID();
                OnLinkExit(oldID);
            }

            // 새로운 링크에 마우스가 올라갔을 때
            if (linkIndex != -1)
            {
                string newID = EarlyStrenghExplaneText.textInfo.linkInfo[linkIndex].GetLinkID();
                OnLinkEnter(newID);
            }

            CurrentLinkIndex = linkIndex;
        }
        
    }

    private void OnLinkEnter(string id)
    {
        /*
         * DetailExplainText.text = "도달 최대 층수 (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
    ")\r\n일반 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillNormalMonster +
    ")\r\n엘리트 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillEliteMonster +
    ")\r\n남은 경험치 (" + PlayerInfo.GetPlayerStateInfo().Experience +
    ")\r\n선한 영향력 (" + PlayerInfo.GetPlayerStateInfo().GoodKarma + ")";
         */
        DetailExplainObject.SetActive(true);
        DetailExplainText.text = "";
        _Corou = StartCoroutine(Load(id));
        /*
        switch (id)
        {
            case "STR07":
                DetailExplainText.text =
                    "<size=40>넘치는 힘</size>" + "\r\n\r\n" +
                    "적을 공격시 적이 쓰러지면" + "\r\n" +
                    "초과한 데미지만큼" + "\r\n" +
                    "다른 적들에게 데미지를 줍니다.";
                break;
            case "DUR07":
                DetailExplainText.text =
                    "<size=40>가시 갑옷</size>" + "\r\n\r\n" +
                    "공격을 받은후 보호막이 남아있다면" + "\r\n" +
                    "남은 보호막 수치만큼 해당 적에게" + "\r\n" +
                    "데미지를 줍니다.";
                break;
            case "RES07":
                DetailExplainText.text =
                    "<size=40>상급 휴식</size>" + "\r\n\r\n" +
                    "전투중 피로도 회복시" + "\r\n" +
                    "피로도 회복량의 10% 만큼" + "\r\n" +
                    "체력을 회복합니다.";
                break;
            case "SPD07":
                DetailExplainText.text =
                    "<size=40>무방비</size>" + "\r\n\r\n" +
                    "적의 공격을 통해 데미지를 받을때" + "\r\n" +
                    "데미지를 2배로 받습니다." + "\r\n" +
                    "자신의 턴이 됬을때" + "\r\n" +
                    "효과가 종료됩니다.";
                break;
            case "HP07":
                DetailExplainText.text =
                    "<size=40>피의 일족</size>" + "\r\n\r\n" +
                    "전투중 모든 수치 계산시" + "\r\n" +
                    "최대 체력의 3% 만큼의" + "\r\n" +
                    "기초 수치가 추가됩니다.";
                break;
            case "STA07":
                DetailExplainText.text =
                    "<size=40>피로도 조절</size>" + "\r\n\r\n" +
                    "현재 피로도에 비례하여" + "\r\n" +
                    "피로도 사용량이" + "\r\n"+
                    "최대 90% 감소합니다.";
                break;
            case "EXP07":
                DetailExplainText.text =
                    "<size=40>착취</size>" + "\r\n\r\n" +
                    "전투중 공격으로 준 피해의" + "\r\n" +
                    "20%만큼 경험치를 수집합니다.";
                break;
            case "EXPMG07":
                DetailExplainText.text =
                    "<size=40>경험의 힘</size>" + "\r\n\r\n" +
                    "전투중 모든 수치 계산에" + "\r\n" +
                    "경험치의 10%만큼" + "\r\n" +
                    "최종 수치를 추가합니다.";
                break;
            case "EQUIP07":
                DetailExplainText.text =
                    "<size=40>웨폰 마스터</size>" + "\r\n\r\n" +
                    "전투중 모든 수치 계산에" + "\r\n" +
                    "자신이 가지고 있는" + "\r\n"+
                    "모든 장비의 티어 수치만큼의" + "\r\n"+
                    "기초 수치가 추가 됩니다.";
                break;
        }
        */
        //Debug.Log($"마우스 올라감: {id}");
    }

    private IEnumerator Load(string key)
    {
        yield return LocalizationSettings.InitializationOperation;

        string BuffDetailKey = "ESBD_" + key;
        var BuffDetailTable = LocalizationSettings.StringDatabase.GetTable("ES_BuffDetail");
        DetailExplainText.text = BuffDetailTable.GetEntry(BuffDetailKey).GetLocalizedString();
        /*
        string DetailTitleKey = "ESDT_" + key;
        var DetailTitleTable = LocalizationSettings.StringDatabase.GetTable("ES_DetailTitle");
        DetailTitleText.text = DetailTitleTable.GetEntry(DetailTitleKey).GetLocalizedString();

        //이거 키 넘어가나 확인해 봐야것는디
        string DetialTextKey = "EST_" + key;
        var DetailTextTable = LocalizationSettings.StringDatabase.GetTable("ES_DetailText");
        DetailText.text = DetailTextTable.GetEntry(DetialTextKey).GetLocalizedString();
        */
    }

    private void OnLinkExit(string id)
    {
        DetailExplainObject.SetActive(false);

        if(_Corou != null)
        {
            StopCoroutine(_Corou);
            _Corou = null;
        }
        // 예: 원래 색상 복구, 툴팁 숨기기 등
        //Debug.Log($"마우스 벗어남: {id}");
    }
}
