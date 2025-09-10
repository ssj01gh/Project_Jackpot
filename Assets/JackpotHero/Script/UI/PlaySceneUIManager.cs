using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlaySceneUIManager : MonoBehaviour
{
    public PlayerManager PlayerMgr;
    public MonsterManager MonMgr;

    //��...... UI ���� ��©��.... ���� �ϳ��ϳ� �� �־�� �ǳ�....
    public PlayerEquipmentUI PE_UI;
    public PlayerStateInfoUI PSI_UI;
    public CurrentStageProgressUI CSP_UI;
    public EquipmentDetailInfoUI EDI_UI;
    public EquipmentDetailInfoUI MEDI_UI;
    public BackGroundUI BG_UI;
    public OptionUI OP_UI;

    public BattleUI B_UI;
    public GettingItenUIScript GI_UI;
    public GuideUI G_UI;
    public GameObject ActionSelectionUI;
    public EventUIScript E_UI;
    public GameObject RestSelectionUI;
    public GameObject FadeUI;
    public RestUIScript R_UI;
    // Start is called before the first frame update
    private void Awake()
    {
        B_UI.InitBattleUI();
        E_UI.gameObject.SetActive(false);
        RestSelectionUI.SetActive(false);
        BG_UI.SetRestBackGround(false);
        FadeUI.SetActive(false);
        OP_UI.OptionInActive();
    }
    void Start()//start���� �ϸ� ���� ���ϰ� ����
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void SetCurrentStateUI(int PlayerAction, int DetailEvnet)
    {
        //Detial Event = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails;
        //B_UI.InitBattleUI();//BattleUI�� ��Ȱ��ȭ//�̰Ÿ� �ع����� �ִϸ��̼��� ����Ǵٰ� �����µ�?
        //E_UI.gameObject.SetActive(false);
        //EventUI.SetActive(false);
        //RestUI.SetActive(false);
        switch(PlayerAction)
        {
            case (int)EPlayerCurrentState.SelectAction:
                ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 0);
                ActionSelectionUI.SetActive(true);
                ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(-400, 0.5f).SetEase(Ease.OutBack);
                SoundManager.Instance.PlayBGM("BaseBGM");//���丮 �㶧 �ٲ�°� �ڿ��������? (�̰� SelectionAction�� ���� ���� ����)
                break;
            case (int)EPlayerCurrentState.Battle:
                if (ActionSelectionUI.activeSelf == true)
                {
                    ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
                    ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.5f).OnComplete(() => { ActionSelectionUI.SetActive(false); });
                }
                if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack != -1)
                    BG_UI.SetRestBackGround(true);//�̰͵� ��� �־����
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
                if(RestSelectionUI.activeSelf == true)
                {
                    RestSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
                    RestSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.3f).OnComplete(() => { RestSelectionUI.SetActive(false); });
                }
                //���̵� �ƿ� �� ��ٰ� ������ ���� ���� ������ ��׶��带 �ٲ�
                FadeUI.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
                FadeUI.SetActive(true);
                FadeUI.GetComponent<Image>().DOFade(1, 0.5f).OnComplete(() => 
                { 
                    BG_UI.SetRestBackGround(true);
                    SoundManager.Instance.PlaySFX("Rest_Ready_Cancel");
                    PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Rest);
                    DOVirtual.DelayedCall(1f, () => 
                    { FadeUI.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() => 
                        { 
                            R_UI.ActiveRestActionSelection(); 
                            FadeUI.SetActive(false);
                            SoundManager.Instance.PlayBGM("RestBGM");
                        }); 
                    }); 
                });
                //���߿��� �ޱ׷� �Ÿ��� �Ҹ��� ���ٴ��� �ϸ�ɵ�
                //��׶��带 �ٲ��Ŀ� ���̵� ��
                //RestActionSelection Ȱ��ȭ
                //�޽Ŀ��� �ൿ ���ù�ư�� Ȱ��ȭ
                break;
            case (int)EPlayerCurrentState.Boss_Event:
                if (ActionSelectionUI.activeSelf == true)
                {
                    ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
                    ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.5f).OnComplete(() => { ActionSelectionUI.SetActive(false); });
                }
                break;
            case (int)EPlayerCurrentState.Boss_Battle:
                if (ActionSelectionUI.activeSelf == true)
                {
                    ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
                    ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.5f).OnComplete(() => { ActionSelectionUI.SetActive(false); });
                }
                break;
        }
    }
    public void SetUI()
    {
        PE_UI.SetEquipmentImage(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo(),
            PlayerMgr.GetPlayerInfo().PlayerBuff.BuffList);
        CSP_UI.SetCurrentStegeUI(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        EDI_UI.gameObject.SetActive(false);
        MEDI_UI.gameObject.SetActive(false);
        SetCurrentStateUI(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction, PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails);
    }

    public void RestButtonClick()//
    {
        ActionSelectionUI.GetComponent<RectTransform>().DOKill();
        RestSelectionUI.GetComponent<RectTransform>().DOKill();

        ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
        ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.3f).OnComplete(() => { ActionSelectionUI.SetActive(false); });

        //RestSelectionUI���� �Ƿε��� �����ϸ� �ȳ� �ż����� ���ų� ��Ȱ��ȭ -> �ȳ� �ż��� ���°� �� ������?
        RestSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 0);
        RestSelectionUI.gameObject.SetActive(true);
        RestSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(-400, 0.3f).SetEase(Ease.OutBack);
    }

    public void NotRestButtonClick()
    {
        RestSelectionUI.GetComponent<RectTransform>().DOKill();
        ActionSelectionUI.GetComponent<RectTransform>().DOKill();

        RestSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
        RestSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(400, 0.3f).OnComplete(() => { RestSelectionUI.SetActive(false); });

        ActionSelectionUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 0);
        ActionSelectionUI.gameObject.SetActive(true);
        ActionSelectionUI.GetComponent<RectTransform>().DOAnchorPosX(-400, 0.3f).SetEase(Ease.OutBack);
    }
    public void EquipmentButtonClick()
    {
        GameObject ClickedButton = EventSystem.current.currentSelectedGameObject;
        int EquipCode;
        switch (ClickedButton.name)
        {
            case "Weapon":
                EquipCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
                break;
            case "Armor":
                EquipCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
                break;
            case "Hat":
                EquipCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
                break;
            case "Shoes":
                EquipCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
                break;
            case "Accessories":
                EquipCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode;
                break;
            default:
                return;
        }

        if(EquipCode == 0)//����ִ� ĭ�� Ŭ���ߴٸ�
        {
            return;
        }
        EDI_UI.gameObject.transform.position = ClickedButton.gameObject.transform.position;
        EDI_UI.ActiveEquipmentDetailInfoUI(EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(EquipCode), true);
    }

    public void MonEquipmentButtonClick()
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
                if (MonMgr.CurrentTarget.MonsterAnotherEquipmentCode.Length >= 1)
                    EquipCode = MonMgr.CurrentTarget.MonsterAnotherEquipmentCode[0];
                else
                    EquipCode = 0;
                break;
            case "AnotherEquip02":
                if (MonMgr.CurrentTarget.MonsterAnotherEquipmentCode.Length >= 2)
                    EquipCode = MonMgr.CurrentTarget.MonsterAnotherEquipmentCode[1];
                else
                    EquipCode = 0;
                break;
            case "AnotherEquip03":
                if (MonMgr.CurrentTarget.MonsterAnotherEquipmentCode.Length >= 3)
                    EquipCode = MonMgr.CurrentTarget.MonsterAnotherEquipmentCode[2];
                else
                    EquipCode = 0;
                break;
            default:
                return;
        }

        MEDI_UI.gameObject.transform.position = ClickedButton.gameObject.transform.position;
        MEDI_UI.ActiveEquipmentDetailInfoUI(EquipmentInfoManager.Instance.GetMonEquipmentInfo(EquipCode), false);
    }

    public void PlayerDefeat()//���ų� ���ӿ��� �̱�ų�
    {
        PE_UI.gameObject.GetComponent<RectTransform>().DOAnchorPosY(100, 0.5f).OnComplete(() => { PE_UI.gameObject.SetActive(false); });
        PSI_UI.gameObject.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.5f).OnComplete(() => { PSI_UI.gameObject.SetActive(false); });
        CSP_UI.gameObject.GetComponent<RectTransform>().DOAnchorPosY(130, 0.5f).OnComplete(() => { CSP_UI.gameObject.SetActive(false); });
        SoundManager.Instance.PlayBGM("DefeatBGM");
    }
    //-------------------------------PressRestTimeUI

    public void PressRestEnd()
    {
        R_UI.InActiveRestActionSelection();
        FadeUI.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        FadeUI.SetActive(true);
        FadeUI.GetComponent<Image>().DOFade(1, 0.5f).
        OnComplete(() =>
            {
                BG_UI.SetRestBackGround(false);
                SoundManager.Instance.PlaySFX("Rest_Ready_Cancel");
                PlayerMgr.GetPlayerInfo().SetPlayerAnimation((int)EPlayerAnimationState.Idle);
                DOVirtual.DelayedCall(1f, () =>
                {
                    FadeUI.GetComponent<Image>().DOFade(0, 0.5f).
                    OnComplete(() =>
                    {
                        SetUI();
                        FadeUI.SetActive(false);
                    });
                });
            });
    }
    //-------------------BossBattleWinFad
    public void BossBattleWinFade()
    {
        FadeUI.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        FadeUI.SetActive(true);
        FadeUI.GetComponent<Image>().DOFade(1, 0.5f).OnComplete(() =>
        {
            BG_UI.SetBackGroundSprite(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor);
            DOVirtual.DelayedCall(1f, () =>
            {
                FadeUI.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() =>
                {
                    SetUI();
                    FadeUI.SetActive(false);
                });
            });
        });
    }
}
