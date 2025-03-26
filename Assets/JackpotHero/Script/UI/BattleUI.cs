using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Net.NetworkInformation;
using System.Reflection;
[System.Serializable]
public class MonBrokenShield
{
    public List<GameObject> MonsterBrokenShieldPieces = new List<GameObject>();
}

public class BattleUI : MonoBehaviour
{
    public GameObject UI_StartOfBattle;
    [Header("ActionSelectionUI")]
    public GameObject PlayerActionSelectionBattleUI;
    public Button PlayerAttackButton;
    public Button PlayerDefenseButton;
    public Button PlayerRegenSTAButton;
    public GameObject MonsterActionSelectionBattleUI;
    [Header("MainBattleUI")]
    public GameObject MainBattleUI;
    public GameObject[] HeroineSprites;//7��
    public GameObject BaseAmountObject;//1��
    public TextMeshProUGUI BaseAmountText;//1��
    public GameObject MagnificationObject;//2��
    public TextMeshProUGUI MagnificationText;//2��
    public GameObject BaseAmountCard;//3��
    public TextMeshProUGUI BaseAmountCardTitleText;//3��
    public TextMeshProUGUI BaseAmountCardDetailText;//3��
    public GameObject MagnificationCard;//3��
    public GameObject[] ActionTypeObject;//4��
    public GameObject FinalCalculateObject;//4��
    public TextMeshProUGUI FinalCalculateText;//4��
    public GameObject ClickTextObject;//5��
    public GameObject PlayerZone;
    public GameObject MonsterZone;

    [Header("MonsterSprites")]
    public string[] MonsterName;
    public Sprite[] MonsterHeadSprites;
    public GameObject[] MonsterSprites;//8��
    //0. Normal # 1. ATK_Form # 2. DUR_Form # 3. RES_Form # 4. SPD_Form # 5. LUK_Form # 6. HP_Form # 7. STA_Form # 8. EXP_From # 9. EXPMG_Form # 10. EQUIP_Form
    [Header("BattleTurnUI")]
    public GameObject DisplayTurnUI;
    public Image[] DisplayTurnUIImages;
    public Sprite[] PlayerHeadSprites;
    [Header("MonsterSelectionUI")]
    public GameObject SelectionArrow;
    [Header("MonsterEquipDetailUI")]
    public GameObject MonsterEquipmentUI;
    public GameObject MonsterWeaponButton;
    public Image MonsterWeaponImage;
    public GameObject MonsterArmorButton;
    public Image MonsterArmorImage;
    public GameObject[] MonsterAnotherEquipmentButton;
    public Image[] MonsterAnotherEquipmentImage;
    [Header("MonsterStatusDetailUI")]
    public GameObject MonsterStatusUI;
    public TextMeshProUGUI MonsterSTRText;
    public TextMeshProUGUI MonsterDURText;
    public TextMeshProUGUI MonsterSPDText;
    public TextMeshProUGUI MonsterLUKText;
    [Header("PlayerBuffNShield")]
    public GameObject PlayerShield;
    public TextMeshProUGUI PlayerShieldText;
    public GameObject PlayerBrokenShieldObject;
    public GameObject[] PlayerBrokenShieldPiece;//0 = LeftPiece // 1 = RightPiece // 2 = ButtomPiece
    public BuffImageUIContainer PlayerBuffUI;
    [Header("MonsterBuffNShieldNHP")]
    public GameObject[] MonsterShield;
    public TextMeshProUGUI[] MonsterShieldText;
    public GameObject[] MonsterBrokenShieldObject;
    [SerializeField]
    public List<MonBrokenShield> MonsterBrokenShield = new List<MonBrokenShield>();
    public BuffImageUIContainer[] MonsterBuffUI;
    public Slider[] MonsterHPSlider;
    public TextMeshProUGUI[] MosnterHpText;
    public GameObject[] MonsterAttackIcons;
    public GameObject[] MonsterDefenseIcons;
    public GameObject[] MonsterAnotherIcons;
    [Header("VictoryUI")]
    public GameObject VictoryUI;
    public TextMeshProUGUI VictoryText;
    [Header("DefeatUI")]
    public GameObject DefeatUI;
    public TextMeshProUGUI DefeatEventTitle;
    public TextMeshProUGUI EquipSuccessionText;
    public TextMeshProUGUI DefeatEventAmount;
    public TextMeshProUGUI DeafeatEarlyPoint;
    [Header("WinGameUI")]
    public GameObject WinGameUI;
    public TextMeshProUGUI WinGameUITitleText;
    public TextMeshProUGUI WinEventTitle;
    public TextMeshProUGUI WinEquipSuccessionText;
    public TextMeshProUGUI WinEventAmount;
    public TextMeshProUGUI WinEarlyPoint;

    protected Dictionary<string, GameObject> MonSpritesStorage = new Dictionary<string, GameObject>();
    protected Dictionary<string, Sprite> MonHeadSpriteStorage = new Dictionary<string, Sprite>();
    // Start is called before the first frame update
    protected bool IsAnimateComplete = false;
    protected bool IsOpenCard = false;
    protected bool IsOpenAnimationComplete = false;

    protected int CurrentMainBattlePhase;
    protected enum EMainBattlePhase
    {
        Nothing,
        BaseAmountComplete,
        BasePlusAmountComplete,
        EquipMagnificationComplete,
        BuffMagnificationComplete,
        MergeComplete,
        FinalPlusAmountComplete,
    }

    protected Vector2[] PlayerBrokenShieldPieceInitPos = new Vector2[]
    {
        new Vector2(-26.25f, 31f),
        new Vector2(22.5f,37.25f),
        new Vector2(0,-37.5f)
    };

    protected Vector2[] PlayerBrokenShieldPieceMoveDist = new Vector2[]
    {
        new Vector2(-25f,25f),
        new Vector2(25f,25f),
        new Vector2(0,-35f)
    };

    protected Vector2[] MonsterBrokenShieldPieceInitPos = new Vector2[]
    {
        new Vector2(-8.76f, 9.41f),
        new Vector2(7.375f, 11.22f),
        new Vector2(-0.13f, -11.23f)
    };

    protected Vector2[] MonsterBrokenShieldPieceMoveDis = new Vector2[]
    {
        new Vector2(-8f,8f),
        new Vector2(8f,8f),
        new Vector2(0,-11f)
    };
    private void Awake()
    {
        for(int i = 0; i < MonsterName.Length; i++)
        {
            if (!MonSpritesStorage.ContainsKey(MonsterName[i]))
            {
                MonSpritesStorage.Add(MonsterName[i], MonsterSprites[i]);
            }
            if (!MonHeadSpriteStorage.ContainsKey(MonsterName[i]))
            {
                MonHeadSpriteStorage.Add(MonsterName[i], MonsterHeadSprites[i]);
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitBattleUI()
    {
        gameObject.SetActive(true);
        DisplayTurnUI.SetActive(false);
        UI_StartOfBattle.SetActive(false);
        PlayerActionSelectionBattleUI.SetActive(false);
        PlayerShield.SetActive(false);
        PlayerBrokenShieldObject.SetActive(false);
        foreach(GameObject obj in MonsterShield)
        {
            obj.SetActive(false);
        }
        foreach(GameObject obj in MonsterBrokenShieldObject)
        {
            obj.SetActive(false);
        }
        MonsterActionSelectionBattleUI.SetActive(false);
        MainBattleUI.SetActive(false);
        MonsterEquipmentUI.SetActive(false);
        MonsterStatusUI.SetActive(false);
        SelectionArrow.SetActive(false);
        VictoryUI.SetActive(false);
        DefeatUI.SetActive(false);
        WinGameUI.SetActive(false);
    }

    public void ActiveBattleUI()//������ �����Ҷ� �ѹ���
    {
        gameObject.SetActive(true);
        UI_StartOfBattle.SetActive(true);
    }

    public void ActiveBattleSelectionUI(bool IsFear)//�÷��̾��� ���� �ö�����
    {
        if (PlayerActionSelectionBattleUI.activeSelf == true)
            return;

        if(IsFear == true)
        {
            MainBattleUI.SetActive(false);
            PlayerActionSelectionBattleUI.SetActive(true);
            PlayerAttackButton.interactable = false;
            PlayerDefenseButton.interactable = false;
            PlayerRegenSTAButton.interactable = true;
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1080);
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
        }
        else
        {
            MainBattleUI.SetActive(false);
            PlayerActionSelectionBattleUI.SetActive(true);
            PlayerAttackButton.interactable = true;
            PlayerDefenseButton.interactable = true;
            PlayerRegenSTAButton.interactable = true;
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1080);
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
        }
    }

    public void ActiveBattleSelectionUI_Mon()
    {
        if (MonsterActionSelectionBattleUI.activeSelf == true)
            return;

        MainBattleUI.SetActive(false);
        MonsterActionSelectionBattleUI.SetActive(true);
        MonsterActionSelectionBattleUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1080);
        MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
    }
    public void SetPlayerBattleUI(PlayerScript PlayerInfo)//����� ����
    {
        //�ϴ� �� ����
        PlayerShield.SetActive(false);
        PlayerBrokenShieldObject.SetActive(false);
        PlayerBuffUI.InitBuffImage();

        if(PlayerInfo.BeforeShield <= 0 && PlayerInfo.GetPlayerStateInfo().ShieldAmount > 0)//Ȱ��ȭ
        {
            float ForUIBeforeShield = PlayerInfo.BeforeShield;
            DOTween.To(() => ForUIBeforeShield, x =>
            {
                ForUIBeforeShield = x;
                PlayerShieldText.text = ForUIBeforeShield.ToString("F0");
            }, PlayerInfo.GetPlayerStateInfo().ShieldAmount, 0.5f);
            //PlayerShieldText.text = ((int)PlayerInfo.GetPlayerStateInfo().ShieldAmount).ToString();
            PlayerShield.transform.position = PlayerInfo.ShieldUIPos.transform.position;
            Vector2 InitShieldPos = PlayerShield.GetComponent<RectTransform>().anchoredPosition;
            PlayerShield.GetComponent<RectTransform>().anchoredPosition = new Vector2(InitShieldPos.x, InitShieldPos.y - 50);
            PlayerShield.GetComponent<CanvasGroup>().alpha = 0;
            PlayerShield.SetActive(true);
            PlayerShield.GetComponent<RectTransform>().DOAnchorPos(InitShieldPos, 0.5f).SetEase(Ease.OutBack);
            PlayerShield.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }
        else if(PlayerInfo.BeforeShield > 0 && PlayerInfo.GetPlayerStateInfo().ShieldAmount <= 0)//��Ȱ��ȭ
        {
            //�ν����°�ó�� ���� ���� �ʱ�ȭ
            PlayerBrokenShieldObject.transform.position = PlayerInfo.ShieldUIPos.transform.position;
            PlayerBrokenShieldObject.SetActive(true);
            PlayerBrokenShieldObject.GetComponent<CanvasGroup>().alpha = 1;
            for (int i = 0; i < PlayerBrokenShieldPiece.Length; i++)
            {
                PlayerBrokenShieldPiece[i].GetComponent<RectTransform>().anchoredPosition = PlayerBrokenShieldPieceInitPos[i];
            }

            for (int i = 0; i < PlayerBrokenShieldPiece.Length; i++)
            {
                PlayerBrokenShieldPiece[i].GetComponent<RectTransform>().
                    DOAnchorPos(PlayerBrokenShieldPiece[i].GetComponent<RectTransform>().anchoredPosition + PlayerBrokenShieldPieceMoveDist[i], 0.5f);
            }
            PlayerBrokenShieldObject.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => { PlayerBrokenShieldObject.SetActive(false); });
        }
        else if(PlayerInfo.BeforeShield > 0 && PlayerInfo.GetPlayerStateInfo().ShieldAmount > 0)
        {
            float ForUIBeforeShield = PlayerInfo.BeforeShield;
            DOTween.To(() => ForUIBeforeShield, x =>
            {
                ForUIBeforeShield = x;
                PlayerShieldText.text = ForUIBeforeShield.ToString("F0");
            }, PlayerInfo.GetPlayerStateInfo().ShieldAmount, 0.5f);
            //PlayerShieldText.text = ((int)PlayerInfo.GetPlayerStateInfo().ShieldAmount).ToString();
            PlayerShield.transform.position = PlayerInfo.ShieldUIPos.transform.position;
            PlayerShield.SetActive(true);
        }

        PlayerBuffUI.ActiveBuffImage(PlayerInfo.PlayerBuff.BuffList, PlayerInfo.BuffUIPos.transform.position);
    }

    public void SetMonsterBattleUI(List<GameObject> ActiveMonsters)
    {
        //�ϴ� �ٲ���
        foreach (GameObject obj in MonsterShield)
        {
            obj.SetActive(false);
        }
        foreach(GameObject obj in MonsterBrokenShieldObject)
        {
            obj.SetActive(false);
        }
        foreach (Slider obj in MonsterHPSlider)
        {
            obj.gameObject.SetActive(false);
        }
        foreach (BuffImageUIContainer obj in MonsterBuffUI)
        {
            obj.InitBuffImage();
        }
        for (int i = 0; i < MonsterAttackIcons.Length; i++)
        {
            MonsterAttackIcons[i].SetActive(false);
            MonsterDefenseIcons[i].SetActive(false);
            MonsterAnotherIcons[i].SetActive(false);
        }

        for (int i = 0; i < ActiveMonsters.Count; i++)
        {
            int index = i;
            Monster Mon = ActiveMonsters[i].GetComponent<Monster>();
            //SetCurrentActionIcon;
            Vector2 ActiveIconPosition = Mon.HpSliderPos.transform.position;
            ActiveIconPosition.x -= (Mon.HpsliderWidth / 100 / 2) + 0.2f;
            switch (Mon.MonsterCurrentState)
            {
                case (int)EMonsterActionState.Attack:
                    MonsterAttackIcons[i].SetActive(true);
                    MonsterAttackIcons[i].transform.position = ActiveIconPosition;
                    break;
                case (int)EMonsterActionState.Defense:
                    MonsterDefenseIcons[i].SetActive(true);
                    MonsterDefenseIcons[i].transform.position = ActiveIconPosition;
                    break;
                default:
                    MonsterAnotherIcons[i].SetActive(true);
                    MonsterAnotherIcons[i].transform.position = ActiveIconPosition;
                    break;
            }

            if(Mon.BeforeMonsterShield <= 0 && Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint > 0)//Ȱ��ȭ
            {
                float ForUIBeforeShield = Mon.BeforeMonsterShield;
                DOTween.To(() => ForUIBeforeShield, x =>
                {
                    ForUIBeforeShield = x;
                    MonsterShieldText[index].text = ForUIBeforeShield.ToString("F0");
                }, Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint, 0.5f);
                //MonsterShieldText[i].text = ((int)Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint).ToString();
                MonsterShield[i].transform.position = Mon.MonsterShieldPos.transform.position;
                Vector2 InitShieldPos = MonsterShield[i].GetComponent<RectTransform>().anchoredPosition;
                MonsterShield[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(InitShieldPos.x, InitShieldPos.y - 50);
                MonsterShield[i].GetComponent<CanvasGroup>().alpha = 0;
                MonsterShield[i].SetActive(true);
                MonsterShield[i].GetComponent<RectTransform>().DOAnchorPos(InitShieldPos, 0.5f).SetEase(Ease.OutBack);
                MonsterShield[i].GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            }
            else if(Mon.BeforeMonsterShield > 0 && Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint <= 0)//��Ȱ��ȭ
            {
                //�ν����°�ó�� ���� ���� �ʱ�ȭ
                MonsterBrokenShieldObject[i].transform.position = Mon.MonsterShieldPos.transform.position;
                MonsterBrokenShieldObject[i].SetActive(true);
                MonsterBrokenShieldObject[i].GetComponent<CanvasGroup>().alpha = 1;
                for (int j = 0; j < MonsterBrokenShield[i].MonsterBrokenShieldPieces.Count; j++)
                {
                    MonsterBrokenShield[i].MonsterBrokenShieldPieces[j].GetComponent<RectTransform>().anchoredPosition = MonsterBrokenShieldPieceInitPos[j];
                }

                for (int j = 0; j < MonsterBrokenShield[i].MonsterBrokenShieldPieces.Count; j++)
                {
                    MonsterBrokenShield[i].MonsterBrokenShieldPieces[j].GetComponent<RectTransform>().
                        DOAnchorPos(MonsterBrokenShield[i].MonsterBrokenShieldPieces[j].GetComponent<RectTransform>().anchoredPosition + MonsterBrokenShieldPieceMoveDis[j], 0.5f);
                }
                MonsterBrokenShieldObject[i].GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() => { MonsterBrokenShieldObject[i].SetActive(false); });
            }
            else if(Mon.BeforeMonsterShield > 0 && Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint > 0)
            {
                
                float ForUIBeforeShield = Mon.BeforeMonsterShield;
                DOTween.To(() => ForUIBeforeShield, x =>
                {
                    ForUIBeforeShield = x;
                    MonsterShieldText[index].text = ForUIBeforeShield.ToString("F0");
                }, Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint, 0.5f);
                
                //MonsterShieldText[i].text = ((int)Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint).ToString();
                MonsterShield[i].transform.position = Mon.MonsterShieldPos.transform.position;
                MonsterShield[i].SetActive(true);
            }
            MonsterHPSlider[i].gameObject.SetActive(true);
            Vector2 MonsterHpSliderSize = MonsterHPSlider[i].GetComponent<RectTransform>().sizeDelta;
            MonsterHpSliderSize.x = Mon.HpsliderWidth;
            MonsterHPSlider[i].GetComponent<RectTransform>().sizeDelta = MonsterHpSliderSize;

            float BeforeMonsterHP = Mon.GetMonsterCurrentStatus().MonsterMaxHP * MonsterHPSlider[i].value;
            DOTween.To(() => BeforeMonsterHP, x =>
            {
                BeforeMonsterHP = x;
                MosnterHpText[index].text = BeforeMonsterHP.ToString("F0") + " / " + Mon.GetMonsterCurrentStatus().MonsterMaxHP.ToString();
            }, Mon.GetMonsterCurrentStatus().MonsterCurrentHP, 0.5f);
            MonsterHPSlider[i].DOValue(Mon.GetMonsterCurrentStatus().MonsterCurrentHP / Mon.GetMonsterCurrentStatus().MonsterMaxHP, 0.5f);
            MonsterHPSlider[i].transform.position = Mon.HpSliderPos.transform.position;

            //SetBuff
            Vector2 BuffPos = Mon.HpSliderPos.transform.position;
            BuffPos.y += Mon.BuffPosUpperHp;
            MonsterBuffUI[i].ActiveBuffImage(Mon.MonsterBuff.BuffList, BuffPos);
            /*
            MonsterBuffUI[i].SetActive(true);
            Vector2 BuffPos = Mon.HpSliderPos.transform.position;
            BuffPos.y += Mon.BuffPosUpperHp;
            MonsterBuffUI[i].transform.position = BuffPos;
            */
        }
    }

    public void UnActiveMonsterBattleUI(GameObject Monster)//�μ��� ���� Monster�� �ش��ϴ� UI�� ���� ����?
    {

    }
    
    public void InActiveBattleUI()
    {
        //
    }
    

    public void SetBattleTurnUI(List<GameObject> TurnList)
    {
        if(DisplayTurnUI.activeSelf == false)
        {
            DisplayTurnUI.GetComponent<CanvasGroup>().alpha = 0;
            DisplayTurnUI.SetActive(true);
            DisplayTurnUI.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
        }
        for(int i = 0; i < DisplayTurnUIImages.Length; i++)
        {
            GameObject obj = TurnList[i];
            if (obj.tag == "Player")
            {
                DisplayTurnUIImages[i].sprite = PlayerHeadSprites[obj.GetComponent<PlayerScript>().GetTotalPlayerStateInfo().CurrentForm];
            }
            else if(obj.tag == "Monster")
            {
                if (!MonHeadSpriteStorage.ContainsKey(obj.GetComponent<Monster>().MonsterName))//���ٸ� �н�
                    continue;

                DisplayTurnUIImages[i].sprite = MonHeadSpriteStorage[obj.GetComponent<Monster>().MonsterName];
            }
        }
    }

    public void DisplayMonsterDetailUI(Monster Mon)//CurrentTarget�� �ٲ���� �ҷ����� -> �� ȭ��ǥ�� ���� �̵�//���� �긦 ����Ű�� ����
    {
        MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 100);
        MonsterEquipmentUI.SetActive(true);//�ڵ������� ActiveAnimation
        MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(-100, 0.4f).SetEase(Ease.OutBack);

        MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, -125);
        MonsterStatusUI.SetActive(true);//�ڵ������� Activeanimation
        MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(125, 0.4f).SetEase(Ease.OutBack);

        SelectionArrow.SetActive(true);
        Vector2 SelectionArrowPos = Mon.transform.position;
        SelectionArrowPos.y -= 0.5f;
        SelectionArrow.transform.position = SelectionArrowPos;

        EquipmentSO MonWeapon = EquipmentInfoManager.Instance.GetMonEquipmentInfo(Mon.MonsterWeaponCode);
        EquipmentSO MonArmor = EquipmentInfoManager.Instance.GetMonEquipmentInfo(Mon.MonsterArmorCode);
        List<EquipmentSO> AnotherEquip = new List<EquipmentSO>();
        for(int i = 0; i < Mon.MonsterAnotherEquipmentCode.Length; i++)
        {
            AnotherEquip.Add(EquipmentInfoManager.Instance.GetMonEquipmentInfo(Mon.MonsterAnotherEquipmentCode[i]));
        }

        MonsterWeaponImage.sprite = MonWeapon.EquipmentImage;
        MonsterArmorImage.sprite = MonArmor.EquipmentImage;
        foreach(GameObject AnotherButton in MonsterAnotherEquipmentButton)
        {
            AnotherButton.SetActive(false);
        }

        for(int i = 0; i < AnotherEquip.Count; i++)
        {
            MonsterAnotherEquipmentButton[i].SetActive(true);
            MonsterAnotherEquipmentImage[i].sprite = AnotherEquip[i].EquipmentImage;
        }
        
        MonsterSTRText.text = Mon.GetMonsterCurrentStatus().MonsterCurrentATK.ToString();
        MonsterDURText.text = Mon.GetMonsterCurrentStatus().MonsterCurrentDUR.ToString();
        MonsterSPDText.text = Mon.GetMonsterCurrentStatus().MonsterCurrentSPD.ToString();
        MonsterLUKText.text = Mon.GetMonsterCurrentStatus().MonsterCurrentLUK.ToString();
    }

    public void UnDisplayMonsterDetailUI()
    {
        if(MonsterEquipmentUI.activeSelf == true)
        {
            MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -100);
            MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(100, 0.4f).OnComplete(() => { MonsterEquipmentUI.SetActive(false); });
        }
        if(MonsterStatusUI.activeSelf == true)
        {
            MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, 125);
            MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.4f).OnComplete(() => { MonsterStatusUI.SetActive(false); });
        }
        SelectionArrow.SetActive(false);
    }

    public void ActiveMainBattleUI(int PlayerForm, Monster CurrentTarget, string ActionString, BattleResultStates BattleResult, int CurrentBattleState, Action CallBack)//���� ���� ����
    {
        if (MainBattleUI.activeSelf == true)
            return;

        if(PlayerActionSelectionBattleUI.activeSelf == true)
        {
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => { PlayerActionSelectionBattleUI.SetActive(false); });
        }

        if(MonsterActionSelectionBattleUI.activeSelf == true)
        {
            MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => { MonsterActionSelectionBattleUI.SetActive(false); });
        }

        MainBattleUI.GetComponent<CanvasGroup>().alpha = 0f;
        MainBattleUI.SetActive(true);
        MainBattleUI.GetComponent<CanvasGroup>().DOFade(1, 1);

        //1�� Ȱ��ȭ
        BaseAmountObject.SetActive(true);
        BaseAmountObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 350);
        BaseAmountObject.GetComponent<RectTransform>().localScale = Vector2.one;
        BaseAmountText.text = "0";

        //2�� Ȱ��ȭ
        MagnificationObject.SetActive(true);
        MagnificationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 350);
        MagnificationObject.GetComponent<RectTransform>().localScale = Vector2.one;
        MagnificationText.text = "1.00";

        //�ϴ� 3���� �� ��Ȱ��ȭ //���� �ڷ�ƾ���� �ذ��ҵ�
        BaseAmountCard.SetActive(false);//3��
        BaseAmountCardTitleText.text = "";//3��
        BaseAmountCardDetailText.text = "";//3��
        MagnificationCard.SetActive(false);//3��

        //0. Attack, 1. Defense, 2. Rest, 3. Another
        //�ൿ ���¿� �´� ��� Ȱ��ȭ //4�� Ȱ��ȭ
        FinalCalculateObject.SetActive(true);
        FinalCalculateObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 350);
        FinalCalculateText.gameObject.SetActive(false);
        for (int i = 0; i < ActionTypeObject.Length; i++)
        {
            ActionTypeObject[i].SetActive(false);
        }
        switch(ActionString)
        {
            case "Attack":
                ActionTypeObject[0].SetActive(true);
                break;
            case "Defense":
                ActionTypeObject[1].SetActive(true);
                break;
            case "Rest":
                ActionTypeObject[2].SetActive(true);
                break;
            case "Another":
                ActionTypeObject[3].SetActive(true);
                break;
        }

        //�÷��̾��� ���¿� �´� ��������Ʈ Ȱ��ȭ//7�� Ȱ��ȭ
        foreach(GameObject Form in HeroineSprites)
        {
            Form.SetActive(false);
        }
        HeroineSprites[PlayerForm].SetActive(true);

        //������ ������ �´� ��������Ʈ Ȱ��ȭ//8�� Ȱ��ȭ
        //�÷��̾��� ���϶� �޽� Ȥ�� ����϶��� �ȳ�Ÿ����
        foreach (GameObject Mon in MonSpritesStorage.Values)//�ϴ� �ٲ�
        {
            Mon.SetActive(false);
        }
        //Ȱ��ȭ�Ұ͸� Ȱ��ȭ
        if(CurrentBattleState == (int)EBattleStates.PlayerTurn)
        {
            if(ActionString == "Attack")//�÷��̾��� �Ͽ��� Attack������ ���� Ȱ��ȭ
            {
                if (MonSpritesStorage.ContainsKey(CurrentTarget.MonsterName))//���� ���⼭ ������ ���� ������ monster�� ���� ��ü�ڰ� ������ Target�� ���� ����
                {
                    MonSpritesStorage[CurrentTarget.MonsterName].SetActive(true);
                }
            }
        }
        else if(CurrentBattleState == (int)EBattleStates.MonsterTurn)//������ �Ͽ����� �ƹ�ư Ȱ��ȭ
        {
            if (MonSpritesStorage.ContainsKey(CurrentTarget.MonsterName))//���� ���⼭ ������ ���� ������ monster�� ���� ��ü�ڰ� ������ Target�� ���� ����
            {
                MonSpritesStorage[CurrentTarget.MonsterName].SetActive(true);
            }
        }   

        StartCoroutine(ProgressMainBattle_UI(ActionString, BattleResult, CurrentBattleState, CallBack));
        //DoTween.OnComplete�� �ϸ� �ڷ�ƾ���� ���ϰ� �Լ���Ű�� �̺�Ʈ���� ���ᰡ������ �ʳ�? �װ� �� ����������?
    }
    IEnumerator ProgressMainBattle_UI(string ActionString, BattleResultStates BattleResult, int CurrentBattleState, Action CallBack)
    {
        CurrentMainBattlePhase = (int)EMainBattlePhase.Nothing;
        IsOpenCard = false;
        IsOpenAnimationComplete = false;
        //�켱 ���� ��ġ(���� ����)ī�带 ��Ÿ���� �Ѵ�.
        BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
        BaseAmountCard.SetActive(true);//3��
        ClickTextObject.SetActive(true);
        ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        switch (ActionString)
        {
            case "Attack":
                BaseAmountCardTitleText.text = "���� ��";
                /*
                int BaseAmount = 0;
                DOTween.To(() => BaseAmount, x =>
                {
                    BaseAmount = x;
                    BaseAmountCardDetailText.text = BaseAmount.ToString("");
                }, ((int)BattleResult.BaseAmount), 0.3f);
                */
                BaseAmountCardDetailText.text = ((int)BattleResult.BaseAmount).ToString();
                break;
            case "Defense":
                BaseAmountCardTitleText.text = "���� ����";
                BaseAmountCardDetailText.text = ((int)BattleResult.BaseAmount).ToString();
                break;
            case "Rest":
                BaseAmountCardTitleText.text = "���� ȸ��";
                BaseAmountCardDetailText.text = ((int)BattleResult.BaseAmount).ToString();
                break;
            case "Another":
                BaseAmountCardTitleText.text = "Ư�� �ൿ";
                BaseAmountCardDetailText.text = "";
                //Ư�� �ൿ�� ���� ����?
                break;
        }
        //ù��° ī�带 Ŭ���ϰ� �ִϸ��̼��� ���������� ���
        IsAnimateComplete = false;
        while (true)
        {
            yield return null;
            if (IsAnimateComplete == true)
            {
                //�ִϸ��̼��� ������ 1���� �� ����, ���������
                int BaseAmount = 0;
                DOTween.To(() => BaseAmount, x =>
                {
                    BaseAmount = x;
                    BaseAmountText.text = BaseAmount.ToString("F0");
                }, (int)BattleResult.BaseAmount, 0.3f);
                //BaseAmountText.text = ((int)BattleResult.BaseAmount).ToString();
                BaseAmountCard.SetActive(false);
                CurrentMainBattlePhase = (int)EMainBattlePhase.BaseAmountComplete;
                break;
            }
        }
        //�߰� ���� ��ġ�� �����Ѵٸ� �ٽ� Ȱ��ȭ
        if (BattleResult.BaseAmountPlus > 0)
        {
            IsAnimateComplete = false;
            BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
            BaseAmountCard.SetActive(true);
            ClickTextObject.SetActive(true);
            ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            BaseAmountCardTitleText.text = "�߰� ���� ��ġ";
            BaseAmountCardDetailText.text = ((int)BattleResult.BaseAmountPlus).ToString();
        }
        //ù��° ī�带 Ŭ���ϰ� �ִϸ��̼��� ���������� ���
        while (BaseAmountCard.activeSelf == true)
        {
            yield return null;
            if (IsAnimateComplete)
            {
                //�ִϸ��̼��� ������ 1���� �� ����, ���������//���ڰ� �ѹ��� ���ϴ°��� �ƴ� ���ݾ� ��ȭ�ϸ� �� ������ ���⵵ ��
                int BaseAmount = (int)BattleResult.BaseAmount;
                DOTween.To(() => BaseAmount, x =>
                {
                    BaseAmount = x;
                    BaseAmountText.text = BaseAmount.ToString("F0");
                }, (int)(BattleResult.BaseAmount + BattleResult.BaseAmountPlus), 0.3f);
                //BaseAmountText.text = ((int)(BattleResult.BaseAmount + BattleResult.BaseAmountPlus)).ToString();
                BaseAmountCard.SetActive(false);
                CurrentMainBattlePhase = (int)EMainBattlePhase.BasePlusAmountComplete;
                break;
            }
        }

        //2������ ���� 3�� Ȱ��ȭ
        //�̰� BattleResult.Count��ŭ �ݺ� ���Ѿ���
        float TotalMagnification = 1f;
        int RepeatCount = 0;
        while(RepeatCount < BattleResult.ResultMagnification.Count)
        {
            yield return null;
            MagnificationCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            MagnificationCard.GetComponent<RectTransform>().localScale = Vector2.one;
            MagnificationCard.SetActive(true);
            MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);//?ī��� �ʱ�ȭ
            IsOpenCard = false;
            IsOpenAnimationComplete = false;
            IsAnimateComplete = false;
            ClickTextObject.SetActive(true);
            ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            //ī�� ������ 50���α��� ����ɶ����� ���
            while (true)
            {
                yield return null;
                if (IsOpenCard == true)
                {
                    //�ִϸ��̼��� 50���� ���� ����Ǹ� ī�忡 ��� ���
                    MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(BattleResult.ResultMagnification[RepeatCount]);//0��° ��� ���
                    MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).OnComplete(() => { IsOpenAnimationComplete = true; });
                    break;
                }
            }

            //���� ī�尡 �� ��Ÿ���� ���� ���
            while(true)
            {
                yield return null;
                if(IsOpenAnimationComplete == true)
                {
                    ClickTextObject.SetActive(true);
                    ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                    break;
                }
            }

            //ī�� Ŭ�� �ִϸ��̼��� ����ɶ����� ���
            while (true)
            {
                yield return null;
                if (IsAnimateComplete == true)
                {
                    //ī�� Ŭ�� �ִϸ��̼� ����� ������ ��Ȱ��ȭ �� ������� ���
                    MagnificationCard.SetActive(false);
                    float BeforeMagnification = TotalMagnification;
                    TotalMagnification *= BattleResult.ResultMagnification[RepeatCount];
                    DOTween.To(() => BeforeMagnification, x =>
                    {
                        BeforeMagnification = x;
                        MagnificationText.text = BeforeMagnification.ToString("F2");
                    }, TotalMagnification, 0.3f);
                    //MagnificationText.text = TotalMagnification.ToString("F2");
                    RepeatCount++;
                    break;
                }
            }
        }
        //���� �Դٴ°��� ������� ���� �Ϸ�Ȱ���
        CurrentMainBattlePhase = (int)EMainBattlePhase.EquipMagnificationComplete;
        if (BattleResult.BuffMagnification != 1)//������ ���� ������ ���� �Ҷ�
        {
            IsAnimateComplete = false;
            BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
            BaseAmountCard.SetActive(true);
            ClickTextObject.SetActive(true);
            ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            BaseAmountCardTitleText.text = "���� ����";
            BaseAmountCardDetailText.text = BattleResult.BuffMagnification.ToString("F2");
        }

        while (BaseAmountCard.activeSelf == true)
        {
            yield return null;
            if (IsAnimateComplete)
            {
                BaseAmountCard.SetActive(false);
                float BeforeMagnification = TotalMagnification;
                TotalMagnification *= BattleResult.BuffMagnification;
                DOTween.To(() => BeforeMagnification, x =>
                {
                    BeforeMagnification = x;
                    MagnificationText.text = BeforeMagnification.ToString("F2");
                }, TotalMagnification, 0.3f);
                //�ִϸ��̼��� ������ 1���� �� ����, ���������//���ڰ� �ѹ��� ���ϴ°��� �ƴ� ���ݾ� ��ȭ�ϸ� �� ������ ���⵵ ��
                //MagnificationText.text = TotalMagnification.ToString("F2");
                CurrentMainBattlePhase = (int)EMainBattlePhase.BuffMagnificationComplete;
                break;
            }
        }

        //�ణ ��ٸ���
        yield return new WaitForSeconds(0.3f);
        //5�� ����
        IsAnimateComplete = false;
        BaseAmountObject.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.5f);
        BaseAmountObject.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f);
        MagnificationObject.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.5f);
        MagnificationObject.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });

        while (true)
        {
            yield return null;
            if(IsAnimateComplete == true)
            {
                IsAnimateComplete = false;
                BaseAmountObject.SetActive(false);
                MagnificationObject.SetActive(false);
                FinalCalculateText.gameObject.SetActive(true);
                FinalCalculateText.gameObject.GetComponent<RectTransform>().localScale = Vector2.zero;
                FinalCalculateText.gameObject.GetComponent<RectTransform>().DOScale(Vector2.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() => { IsAnimateComplete = true; });
                int BeforeFinalCalculate = 0;
                DOTween.To(() => BeforeFinalCalculate, x =>
                {
                    BeforeFinalCalculate = x;
                    FinalCalculateText.text = BeforeFinalCalculate.ToString("F0");
                }, (int)(BattleResult.FinalResultAmount - BattleResult.FinalResultAmountPlus), 0.5f);
                //FinalCalculateText.text = ((int)(BattleResult.FinalResultAmount - BattleResult.FinalResultAmountPlus)).ToString();
                break;
            }
        }

        //FinalCalculateObject�� ���������� ����Ѵ�.
        while(true)
        {
            yield return null;
            if (IsAnimateComplete == true)
            {
                CurrentMainBattlePhase = (int)EMainBattlePhase.MergeComplete;
                break;
            }
        }
        //FinalResultAmountPlus�� 1���� ũ�ٸ� AmountCard�� �ٽ� Ȱ��ȭ �Ѵ�.
        if(BattleResult.FinalResultAmountPlus >= 1)
        {
            IsAnimateComplete = false;
            BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
            BaseAmountCard.SetActive(true);

            ClickTextObject.SetActive(true);
            ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

            BaseAmountCardTitleText.text = "�߰� ���� ��ġ";
            BaseAmountCardDetailText.text = ((int)BattleResult.FinalResultAmountPlus).ToString();
        }

        while (BaseAmountCard.activeSelf == true)
        {
            yield return null;
            if (IsAnimateComplete)
            {
                //�ִϸ��̼��� ������ 1���� �� ����, ���������//���ڰ� �ѹ��� ���ϴ°��� �ƴ� ���ݾ� ��ȭ�ϸ� �� ������ ���⵵ ��
                CurrentMainBattlePhase = (int)EMainBattlePhase.FinalPlusAmountComplete;
                int BeforeFinalCalculate = (int)(BattleResult.FinalResultAmount - BattleResult.FinalResultAmountPlus);
                DOTween.To(() => BeforeFinalCalculate, x =>
                {
                    BeforeFinalCalculate = x;
                    FinalCalculateText.text = BeforeFinalCalculate.ToString("F0");
                }, (int)BattleResult.FinalResultAmount, 0.5f);
                //FinalCalculateText.text = ((int)BattleResult.FinalResultAmount).ToString();
                BaseAmountCard.SetActive(false);
                break;
            }
        }
        //�ణ ��ٸ���
        yield return new WaitForSeconds(0.3f);

        IsAnimateComplete = false;
        if (CurrentBattleState == (int)EBattleStates.PlayerTurn)
        {
            FinalCalculateObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-675, 350), 0.5f).OnComplete(() => { IsAnimateComplete = true; });
        }
        else if(CurrentBattleState == (int)EBattleStates.MonsterTurn)
        {
            FinalCalculateObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(675, 350), 0.5f).OnComplete(() => { IsAnimateComplete = true; });
        }

        while(true)
        {
            yield return null;
            if(IsAnimateComplete == true)
            {
                break;
            }
        }
        //�ణ ��ٸ���
        yield return new WaitForSeconds(0.3f);
        IsAnimateComplete = false;
        //�̰ſ� ���� ���ܻ����� ���߿� ��� �þ��?
        if (CurrentBattleState == (int)EBattleStates.PlayerTurn && ActionString == "Attack")//�÷��̾��� ��� �����϶���
        {
            PlayerZone.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, 0), 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { IsAnimateComplete = true; });
        }
        else if (CurrentBattleState == (int)EBattleStates.MonsterTurn && (ActionString == "Attack" || ActionString == "Another"))//������ ��� ���� + Ư�� �ൿ�϶���
        {
            MonsterZone.GetComponent<RectTransform>().DOAnchorPos(new Vector2(350, 0), 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { IsAnimateComplete = true; });
        }
        else
        {
            IsAnimateComplete = true;
        }

        while (true)
        {
            yield return null;
            if (IsAnimateComplete == true)
            {
                break;
            }
        }

        IsAnimateComplete = false;
        MainBattleUI.GetComponent<CanvasGroup>().DOFade(0f, 1f).OnComplete(() => { IsAnimateComplete = true; }); ;
        while(true)
        {
            yield return null;
            if(IsAnimateComplete == true)
            {
                MainBattleUI.SetActive(false);
                break;
            }
        }

        CallBack?.Invoke();
        Debug.Log("CoroutineEnd");
    }

    public void ClickAmountCard()
    {
        if(CurrentMainBattlePhase == (int)EMainBattlePhase.Nothing || CurrentMainBattlePhase == (int)EMainBattlePhase.BaseAmountComplete)//�������� ȸ���ϸ鼭
        {//���� ��ġ ���� ����, //�߰� ���� ��ġ ��������
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-400, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.SetActive(false);
        }
        else if(CurrentMainBattlePhase == (int)EMainBattlePhase.EquipMagnificationComplete)
        {//��� ���� �� �Ϸ� ���¶�� -> ������ ���� �߰� ����
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(400, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.SetActive(false);
        }
        else if(CurrentMainBattlePhase == (int)EMainBattlePhase.MergeComplete)
        {//��ġ�°� �Ϸ� ���� -> ���� �߰� ��ġ
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.SetActive(false);
        }

        /*
        if(IsOpenAnimationComplete == false)//�߰� ���� ��ġ
        {
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-400, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.SetActive(false);
        }
        else if(IsOpenAnimationComplete == true)//�߰� ���� ��ġ
        {
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.SetActive(false);
        }
        */
    }

    public void ClickMagnificationCard()
    {
        if(IsOpenCard == false)//���� �ȿ����ٸ�//IsOpenCard�� true�� �� -> �ڷ�ƾ���� ���� �´� ī��� �ٲ�
        {
            MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).OnComplete(() => { IsOpenCard = true; });
        }
        else if(IsOpenCard == true && IsOpenAnimationComplete == true && IsAnimateComplete == false)//ī�尡 ���� �ǰ�, ���� �ִϸ��̼��� �������� Ŭ���ϸ�
        {
            MagnificationCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(400, 350), 0.5f);
            MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
            MagnificationCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.SetActive(false);
        }
    }

    public void VictoryBattle(int RewardExperience)//�¸�������
    {
        //�ൿ���� UI��Ȱ��ȭ
        if (PlayerActionSelectionBattleUI.activeSelf == true)
        {
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => { PlayerActionSelectionBattleUI.SetActive(false); });
        }
        //���� �� ��ư UI ��Ȱ��ȭ
        if (MonsterActionSelectionBattleUI.activeSelf == true)
        {
            MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => { MonsterActionSelectionBattleUI.SetActive(false); });
        }
        //�� ǥ�� UI��Ȱ��ȭ
        if (DisplayTurnUI.activeSelf == true)
        {
            DisplayTurnUI.GetComponent<CanvasGroup>().alpha = 1;
            DisplayTurnUI.GetComponent<CanvasGroup>().DOFade(0f, 0.4f).OnComplete(() => { DisplayTurnUI.SetActive(false); });
        }
        //���� ���� UI��Ȱ��ȭ
        SelectionArrow.SetActive(false);
        //���� ��� UI��Ȱ��ȭ
        if (MonsterEquipmentUI.activeSelf == true)
        {
            MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -100);
            MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(100, 0.4f).OnComplete(() => { MonsterEquipmentUI.SetActive(false); });
        }
        //���� ���� UI��Ȱ��ȭ
        if (MonsterStatusUI.activeSelf == true)
        {
            MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, 125);
            MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.4f).OnComplete(() => { MonsterStatusUI.SetActive(false); });
        }
        //�̰� ���� �¸� ������ �ö���°Ű�
        VictoryUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -1080);
        VictoryUI.SetActive(true);
        VictoryUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
        VictoryText.text = "ȹ�� ����ġ : " + RewardExperience;
    }

    public void ClickVictoryButton()
    {
        if (VictoryUI.activeSelf == true)
        {
            VictoryUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 0);
            VictoryUI.GetComponent<RectTransform>().DOAnchorPosY(1080, 0.5f).OnComplete(() => { VictoryUI.SetActive(false); });
        }

    }

    public void DefeatBattle(PlayerScript PlayerInfo)
    {
        if (PlayerActionSelectionBattleUI.activeSelf == true)
        {
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => { PlayerActionSelectionBattleUI.SetActive(false); });
        }

        if (MonsterActionSelectionBattleUI.activeSelf == true)
        {
            MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => { MonsterActionSelectionBattleUI.SetActive(false); });
        }
        if (DisplayTurnUI.activeSelf == true)
        {
            DisplayTurnUI.GetComponent<CanvasGroup>().alpha = 1;
            DisplayTurnUI.GetComponent<CanvasGroup>().DOFade(0f, 0.4f).OnComplete(() => { DisplayTurnUI.SetActive(false); });
        }
        SelectionArrow.SetActive(false);
        if (MonsterEquipmentUI.activeSelf == true)
        {
            MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -100);
            MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(100, 0.4f).OnComplete(() => { MonsterEquipmentUI.SetActive(false); });
        }
        if (MonsterStatusUI.activeSelf == true)
        {
            MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, 125);
            MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.4f).OnComplete(() => { MonsterStatusUI.SetActive(false); });
        }

        DefeatUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -1080);
        DefeatUI.SetActive(true);
        DefeatUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
        DefeatEventTitle.text = "���� �ִ� ���� (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
            ")\r\n�� ���� (" + PlayerInfo.GetPlayerStateInfo().GiveDamage +
            ")\r\n���� ���� (" + PlayerInfo.GetPlayerStateInfo().ReceiveDamage +
            ")\r\n�ִ� ���� (" + PlayerInfo.GetPlayerStateInfo().MostPowerfulDamage +
            ")\r\n���� ����ġ (" +PlayerInfo.GetPlayerStateInfo().Experience +")";
        DefeatEventAmount.text = (int)(JsonReadWriteManager.Instance.E_Info.PlayerReachFloor / 5) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().GiveDamage / 1000) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().ReceiveDamage / 500) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().MostPowerfulDamage / 100) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().Experience / 2000);
        EquipSuccessionText.text = "������ ����� ������ " + (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIPSUC") + "���� ��� ��µ˴ϴ�.";
        DeafeatEarlyPoint.text = "���� ��ȭ ����Ʈ : " + JsonReadWriteManager.Instance.E_Info.PlayerEarlyPoint;
    }

    public void WinGame(PlayerScript PlayerInfo)//�̰� �����־ �Ǵ����� ���ڴµ� �������� ���߿� �ű��� ��
    {
        if (PlayerActionSelectionBattleUI.activeSelf == true)
        {
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => { PlayerActionSelectionBattleUI.SetActive(false); });
        }

        if (MonsterActionSelectionBattleUI.activeSelf == true)
        {
            MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => { MonsterActionSelectionBattleUI.SetActive(false); });
        }
        if (DisplayTurnUI.activeSelf == true)
        {
            DisplayTurnUI.GetComponent<CanvasGroup>().alpha = 1;
            DisplayTurnUI.GetComponent<CanvasGroup>().DOFade(0f, 0.4f).OnComplete(() => { DisplayTurnUI.SetActive(false); });
        }
        SelectionArrow.SetActive(false);
        if (MonsterEquipmentUI.activeSelf == true)
        {
            MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -100);
            MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(100, 0.4f).OnComplete(() => { MonsterEquipmentUI.SetActive(false); });
        }
        if (MonsterStatusUI.activeSelf == true)
        {
            MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, 125);
            MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.4f).OnComplete(() => { MonsterStatusUI.SetActive(false); });
        }

        WinGameUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -1080);
        WinGameUI.SetActive(true);
        WinGameUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
        //Ư�� ���ǿ� ���� �¸���� ���� �ٲ�� ���� ������? �ϴ� �״�� ����

        WinEventTitle.text = "���� �ִ� ���� (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
            ")\r\n�� ���� (" + PlayerInfo.GetPlayerStateInfo().GiveDamage +
            ")\r\n���� ���� (" + PlayerInfo.GetPlayerStateInfo().ReceiveDamage +
            ")\r\n�ִ� ���� (" + PlayerInfo.GetPlayerStateInfo().MostPowerfulDamage +
            ")\r\n���� ����ġ (" + PlayerInfo.GetPlayerStateInfo().Experience + ")";
        WinEventAmount.text = (int)(JsonReadWriteManager.Instance.E_Info.PlayerReachFloor / 5) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().GiveDamage / 1000) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().ReceiveDamage / 500) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().MostPowerfulDamage / 100) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().Experience / 2000);
        WinEquipSuccessionText.text = "������ ����� ������ " + (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIPSUC") + "���� ��� ��µ˴ϴ�.";
        WinEarlyPoint.text = "���� ��ȭ ����Ʈ : " + JsonReadWriteManager.Instance.E_Info.PlayerEarlyPoint;
    }

    public void ClickDefeatButton()
    {

    }
}
