using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySceneUIManager : MonoBehaviour
{
    public PlayerEquipmentUI PE_UI;
    public PlayerStateInfoUI PSI_UI;
    public CurrentStageProgressUI CSP_UI;
    public EquipmentDetailInfoUI EDI_UI;
    public EquipmentDetailInfoUI MEDI_UI;
    public BackGroundUI BG_UI;
    public BattleUI B_UI;
    public GuideUI G_UI;
    public GameObject ActionSelectionUI;
    public GameObject EventUI;
    public GameObject RestUI;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void SetCurrentStateUI(int PlayerAction, int DetailEvnet)
    {
        //Detial Event = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails;
        B_UI.InitBattleUI();
        //EventUI.SetActive(false);
        //RestUI.SetActive(false);
        switch(PlayerAction)
        {
            case (int)EPlayerCurrentState.SelectAction:
                ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 0);
                ActionSelectionUI.SetActive(true);
                ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(-400, 0.5f).SetEase(Ease.OutBack);
                break;
            case (int)EPlayerCurrentState.Battle:
                if (ActionSelectionUI.activeSelf == true)
                {
                    ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
                    ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.5f).OnComplete(() => { ActionSelectionUI.SetActive(false); });
                }
                B_UI.ActiveBattleUI();
                break;
            case (int)EPlayerCurrentState.OtherEvent:
                if (ActionSelectionUI.activeSelf == true)
                {
                    ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
                    ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.5f).OnComplete(() => { ActionSelectionUI.SetActive(false); });
                }
                //EventUI.SetActive(true);
                break;
            case (int)EPlayerCurrentState.Rest:
                if (ActionSelectionUI.activeSelf == true)
                {
                    ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
                    ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.5f).OnComplete(() => { ActionSelectionUI.SetActive(false); });
                }
                //RestUI.SetActive(true);
                break;
        }
    }

    public void SetUI(PlayerManager PlayerMgr)
    {
        PE_UI.SetEquipmentImage(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        CSP_UI.SetCurrentStegeUI(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        EDI_UI.gameObject.SetActive(false);
        MEDI_UI.gameObject.SetActive(false);
        SetCurrentStateUI(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction, PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails);
    }

    /*
    public void SetActionSelectionUIActive(bool IsActive)
    {
        ActionSelectionUI.SetActive(IsActive);
    }
    */
    public void ForResearchButtonClick()
    {
        ActionSelectionUI.SetActive(false);
        BG_UI.MoveBackGround();
    }

    public bool IsBGMoveEnd()
    {
        return BG_UI.GetIsMoveEnd();
    }
    public void EquipmentButtonClick(PlayerManager PMgr)
    {
        GameObject ClickedButton = EventSystem.current.currentSelectedGameObject;
        int EquipCode;
        switch (ClickedButton.name)
        {
            case "Weapon":
                EquipCode = PMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
                break;
            case "Armor":
                EquipCode = PMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
                break;
            case "Hat":
                EquipCode = PMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
                break;
            case "Shoes":
                EquipCode = PMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
                break;
            case "Accessories":
                EquipCode = PMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode;
                break;
            default:
                return;
        }
        EDI_UI.gameObject.transform.position = ClickedButton.gameObject.transform.position;
        EDI_UI.ActiveEquipmentDetailInfoUI(EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(EquipCode), true);
    }

    public void MonEquipmentButtonClick(MonsterManager MonMgr)
    {
        GameObject ClickedButton = EventSystem.current.currentSelectedGameObject;
        int EquipCode;
        switch(ClickedButton.name)
        {
            case "Weapon":
                EquipCode = MonMgr.CurrentTarget.MonsterWeaponCode;
                break;
            case "Armor":
                EquipCode = MonMgr.CurrentTarget.MonsterArmorCode;
                break;
            case "AnotherEquip01":
                EquipCode = MonMgr.CurrentTarget.MonsterWeaponCode;
                break;
            case "AnotherEquip02":
                EquipCode = MonMgr.CurrentTarget.MonsterWeaponCode;
                break;
            case "AnotherEquip03":
                EquipCode = MonMgr.CurrentTarget.MonsterWeaponCode;
                break;
            default:
                return;
        }

        MEDI_UI.gameObject.transform.position = ClickedButton.gameObject.transform.position;
        MEDI_UI.ActiveEquipmentDetailInfoUI(EquipmentInfoManager.Instance.GetMonEquipmentInfo(EquipCode), false);
    }

    public void PlayerDefeat()
    {
        PE_UI.gameObject.GetComponent<RectTransform>().DOAnchorPosY(100, 0.5f).OnComplete(() => { PE_UI.gameObject.SetActive(false); });
        PSI_UI.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.5f).OnComplete(() => { PSI_UI.gameObject.SetActive(false); });
        CSP_UI.gameObject.GetComponent<RectTransform>().DOAnchorPosY(130, 0.5f).OnComplete(() => { CSP_UI.gameObject.SetActive(false); });
    }
}
