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
    public TextMeshProUGUI WinEventTitle;
    public TextMeshProUGUI WinEquipSuccessionText;
    public TextMeshProUGUI WinEventAmount;
    public TextMeshProUGUI WinEarlyPoint;

    //protected Dictionary<string, GameObject> MonSpritesStorage = new Dictionary<string, GameObject>();
    //protected Dictionary<string, Sprite> MonHeadSpriteStorage = new Dictionary<string, Sprite>();
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
        SpecialAction
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
        PlayerBuffUI.InitBuffImage();
        foreach (GameObject obj in MonsterShield)
        {
            obj.SetActive(false);
        }
        foreach(GameObject obj in MonsterBrokenShieldObject)
        {
            obj.SetActive(false);
        }
        foreach(BuffImageUIContainer obj in MonsterBuffUI)
        {
            obj.InitBuffImage();
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

        UI_StartOfBattle.GetComponent<CanvasGroup>().alpha = 0;
        UI_StartOfBattle.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        UI_StartOfBattle.SetActive(true);
        UI_StartOfBattle.GetComponent<CanvasGroup>().DOFade(1, 0.3f).OnComplete(() => 
        {
            UI_StartOfBattle.GetComponent<RectTransform>().DOAnchorPosY(640, 0.3f).SetEase(Ease.InBack).OnComplete(() => 
            {
                UI_StartOfBattle.SetActive(false);
            });
        });
    }

    public void ActiveBattleSelectionUI(bool IsFear, float AttackAver, float DefenseAver, float RestAver)//�÷��̾��� ���� �ö�����
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
            PlayerRegenSTAButton.GetComponent<PlayerBattleActionSelection>().GetUsefulInfo(RestAver);

            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -1080);
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
        }
        else
        {
            MainBattleUI.SetActive(false);
            PlayerActionSelectionBattleUI.SetActive(true);
            PlayerAttackButton.interactable = true;
            PlayerAttackButton.GetComponent<PlayerBattleActionSelection>().GetUsefulInfo(AttackAver);
            PlayerDefenseButton.interactable = true;
            PlayerDefenseButton.GetComponent<PlayerBattleActionSelection>().GetUsefulInfo(DefenseAver);
            PlayerRegenSTAButton.interactable = true;
            PlayerRegenSTAButton.GetComponent<PlayerBattleActionSelection>().GetUsefulInfo(RestAver);
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
            SoundManager.Instance.PlaySFX("Shield_Appear");
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
            SoundManager.Instance.PlaySFX("Shield_Disappear");
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
                SoundManager.Instance.PlaySFX("Shield_Appear");
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
                SoundManager.Instance.PlaySFX("Shield_Disappear");
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

        DisplayTurnUIImages[0].transform.parent.gameObject.GetComponent<RectTransform>().DOKill();
        DisplayTurnUIImages[0].transform.parent.gameObject.GetComponent<RectTransform>().localScale = Vector2.one;
        DisplayTurnUIImages[0].transform.parent.gameObject.GetComponent<RectTransform>().DOScale(1.05f, 0.3f).
            SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        for (int i = 0; i < DisplayTurnUIImages.Length; i++)
        {
            GameObject obj = TurnList[i];
            if (obj.tag == "Player")
            {
                DisplayTurnUIImages[i].sprite = PlayerHeadSprites[obj.GetComponent<PlayerScript>().GetTotalPlayerStateInfo().CurrentForm];
            }
            else if(obj.tag == "Monster")
            {
                DisplayTurnUIImages[i].sprite = obj.GetComponent<Monster>().MonsterHead;
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

        //�Ķ��� ���� ������ ����
        //��
        if (Mon.GetMonsterCurrentBaseStatus("STR") < Mon.GetMonsterCurrentStatus().MonsterCurrentATK)
            MonsterSTRText.color = Color.blue;
        else if(Mon.GetMonsterCurrentBaseStatus("STR") > Mon.GetMonsterCurrentStatus().MonsterCurrentATK)
            MonsterSTRText.color = Color.red;
        else
            MonsterSTRText.color = Color.white;
        //����
        if (Mon.GetMonsterCurrentBaseStatus("DUR") < Mon.GetMonsterCurrentStatus().MonsterCurrentDUR)
            MonsterDURText.color = Color.blue;
        else if (Mon.GetMonsterCurrentBaseStatus("DUR") > Mon.GetMonsterCurrentStatus().MonsterCurrentDUR)
            MonsterDURText.color = Color.red;
        else
            MonsterDURText.color = Color.white;
        //���
        if (Mon.GetMonsterCurrentBaseStatus("LUK") < Mon.GetMonsterCurrentStatus().MonsterCurrentLUK)
            MonsterLUKText.color = Color.blue;
        else if (Mon.GetMonsterCurrentBaseStatus("LUK") > Mon.GetMonsterCurrentStatus().MonsterCurrentLUK)
            MonsterLUKText.color = Color.red;
        else
            MonsterLUKText.color = Color.white;
        //�ӵ�
        if (Mon.GetMonsterCurrentBaseStatus("SPD") < Mon.GetMonsterCurrentStatus().MonsterCurrentSPD)
            MonsterSPDText.color = Color.blue;
        else if (Mon.GetMonsterCurrentBaseStatus("SPD") > Mon.GetMonsterCurrentStatus().MonsterCurrentSPD)
            MonsterSPDText.color = Color.red;
        else
            MonsterSPDText.color = Color.white;
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

    public void ActiveMainBattleUI(GameObject ActionObj, Monster CurrentTarget, string ActionString, BattleResultStates BattleResult, Vector2 PlayerPos, bool IsThereShield, Action CallBack)//���� ���� ����
    {//(���� ������ ������Ʈ, Ÿ���� �� ����(�÷��̾� ���϶�), ������Ʈ�� �ൿ, �ൿ ���, ��� �ൿ�� ������ ������ �Լ�)
        //�ʿ��Ѱ��� Effect�� �ҷ��� �÷��̾��� Vector�� ������ Vector ���� Player�� Monster�� ��� ������ �Ȱ��� �͵� �ɵ�?
        if (MainBattleUI.activeSelf == true)
            return;

        if(PlayerActionSelectionBattleUI.activeSelf == true)
        {
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => 
            { 
                PlayerActionSelectionBattleUI.SetActive(false);
                PlayerActionSelectionBattleUI.GetComponent<PlayerBattleActionSelection>().InActiveActionPercentDetailUI();
            });
        }

        if(MonsterActionSelectionBattleUI.activeSelf == true)
        {
            MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.4f).OnComplete(() => { MonsterActionSelectionBattleUI.SetActive(false); });
        }

        MainBattleUI.GetComponent<CanvasGroup>().alpha = 0f;
        MainBattleUI.SetActive(true);
        MainBattleUI.GetComponent<CanvasGroup>().DOFade(1, 1);

        if(ActionString != "Attack" && ActionString != "Defense" &&  ActionString != "Rest")
        {
            BaseAmountObject.SetActive(false);
            MagnificationObject.SetActive(false);
        }
        else
        {
            //1�� Ȱ��ȭ
            BaseAmountObject.SetActive(true);
            //BaseAmountObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 350);
            BaseAmountObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-180, 130);
            BaseAmountObject.GetComponent<RectTransform>().localScale = Vector2.one;
            BaseAmountText.text = "0";

            //2�� Ȱ��ȭ
            MagnificationObject.SetActive(true);
            //MagnificationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 350);
            MagnificationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(180, 130);
            MagnificationObject.GetComponent<RectTransform>().localScale = Vector2.one;
            MagnificationText.text = "1.00";
        }
        //�ϴ� 3���� �� ��Ȱ��ȭ //���� �ڷ�ƾ���� �ذ��ҵ�
        BaseAmountCard.SetActive(false);//3��
        BaseAmountCardTitleText.text = "";//3��
        BaseAmountCardDetailText.text = "";//3��
        MagnificationCard.SetActive(false);//3��

        //0. Attack, 1. Defense, 2. Rest, 3. Another
        //�ൿ ���¿� �´� ��� Ȱ��ȭ //4�� Ȱ��ȭ
        FinalCalculateObject.SetActive(true);
        //FinalCalculateObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 350);
        FinalCalculateObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 130);
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
            default:
                ActionTypeObject[3].SetActive(true);
                break;
        }
        Vector2 TargetPos = Vector2.zero;
        if (CurrentTarget != null)
            TargetPos = CurrentTarget.gameObject.transform.position;

        StartCoroutine(ProgressMainBattle_UI(ActionObj, ActionString, BattleResult, PlayerPos, TargetPos, IsThereShield, CallBack));
        //DoTween.OnComplete�� �ϸ� �ڷ�ƾ���� ���ϰ� �Լ���Ű�� �̺�Ʈ���� ���ᰡ������ �ʳ�? �װ� �� ����������?
    }
    IEnumerator ProgressMainBattle_UI(GameObject ActionObj, string ActionString, BattleResultStates BattleResult, Vector2 PlayerPos, Vector2 TargetPos, bool IsThereShield, Action CallBack)
    {
        AudioSource NumAudioSource = new AudioSource();
        CurrentMainBattlePhase = (int)EMainBattlePhase.Nothing;
        IsOpenCard = false;
        IsOpenAnimationComplete = false;
        //�켱 ���� ��ġ(���� ����)ī�带 ��Ÿ���� �Ѵ�.
        //BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80f);
        BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
        BaseAmountCard.SetActive(true);//3��

        ClickTextObject.SetActive(true);
        ClickTextObject.GetComponent<RectTransform>().DOKill();
        ClickTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250f);
        ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-245, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        switch (ActionString)
        {
            case "Attack":
                BaseAmountCardTitleText.text = "���� ��";
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
            default:
                CurrentMainBattlePhase = (int)EMainBattlePhase.SpecialAction;
                BaseAmountCardTitleText.text = "Ư�� �ൿ";
                BaseAmountCardDetailText.text = "";
                //Ư�� �ൿ�� ���� ����?
                break;
        }
        //ù��° ī�带 Ŭ���ϰ� �ִϸ��̼��� ���������� ���
        IsAnimateComplete = false;
        if(CurrentMainBattlePhase != (int)EMainBattlePhase.SpecialAction)
        {
            while (true)
            {
                yield return null;
                if (IsAnimateComplete == true)
                {
                    //�ִϸ��̼��� ������ 1���� �� ����, ���������
                    int BaseAmount = 0;
                    if (BaseAmount != (int)BattleResult.BaseAmount)//���� �ٸ���
                    {
                        NumAudioSource = SoundManager.Instance.PlaySFX("Increase_Number");
                    }
                    DOTween.To(() => BaseAmount, x =>
                    {
                        BaseAmount = x;
                        BaseAmountText.text = BaseAmount.ToString("F0");
                    }, (int)BattleResult.BaseAmount, 0.3f).OnComplete(() => { SoundManager.Instance.StopSFX(NumAudioSource); });
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
                //BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80f);
                BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
                BaseAmountCard.SetActive(true);

                ClickTextObject.SetActive(true);
                ClickTextObject.GetComponent<RectTransform>().DOKill();
                ClickTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250f);
                ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-245, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
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
                    if (BaseAmount != (int)(BattleResult.BaseAmount + BattleResult.BaseAmountPlus))
                    {
                        NumAudioSource = SoundManager.Instance.PlaySFX("Increase_Number");
                    }
                    DOTween.To(() => BaseAmount, x =>
                    {
                        BaseAmount = x;
                        BaseAmountText.text = BaseAmount.ToString("F0");
                    }, (int)(BattleResult.BaseAmount + BattleResult.BaseAmountPlus), 0.3f).OnComplete(() => { SoundManager.Instance.StopSFX(NumAudioSource); });
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
            int PositiveLink = 0;
            while (RepeatCount < BattleResult.ResultMagnification.Count)
            {
                yield return null;
                //MagnificationCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                MagnificationCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80f);
                MagnificationCard.GetComponent<RectTransform>().localScale = Vector2.one;
                MagnificationCard.SetActive(true);
                MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);//?ī��� �ʱ�ȭ

                IsOpenCard = false;
                IsOpenAnimationComplete = false;
                IsAnimateComplete = false;
                //ClickTextObject.SetActive(true);
                //ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                ClickTextObject.SetActive(true);
                ClickTextObject.GetComponent<RectTransform>().DOKill();
                ClickTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250f);
                ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-245, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                //ī�� ������ 50���α��� ����ɶ����� ���
                while (true)
                {
                    yield return null;
                    if (IsOpenCard == true)
                    {
                        //�ִϸ��̼��� 50���� ���� ����Ǹ� ī�忡 ��� ���
                        if (BattleResult.ResultMagnification[RepeatCount] >= 1)//�����̶��
                        {
                            PositiveLink++;
                        }
                        else
                        {
                            PositiveLink = 0;
                        }
                        MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(BattleResult.ResultMagnification[RepeatCount]);//0��° ��� ���
                        MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).OnComplete(() =>
                        {
                            //MagnificationCard.GetComponent<RectTransform>().DORotate
                            PlayCardResultSound(PositiveLink);//��� ������ �Ǹ� ��ġ�� ��� �ö�
                            IsOpenAnimationComplete = true;
                        });
                        break;
                    }
                }

                //���� ī�尡 �� ��Ÿ���� ���� ���
                while (true)
                {
                    yield return null;
                    if (IsOpenAnimationComplete == true)
                    {
                        //ClickTextObject.SetActive(true);
                        //ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                        ClickTextObject.SetActive(true);
                        ClickTextObject.GetComponent<RectTransform>().DOKill();
                        ClickTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250f);
                        ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-245, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
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
                        if (BeforeMagnification != TotalMagnification)//*1�� �ƴҶ�
                        {
                            NumAudioSource = SoundManager.Instance.PlaySFX("Increase_Number");
                        }
                        DOTween.To(() => BeforeMagnification, x =>
                        {
                            BeforeMagnification = x;
                            MagnificationText.text = BeforeMagnification.ToString("F2");
                        }, TotalMagnification, 0.3f).OnComplete(() => { SoundManager.Instance.StopSFX(NumAudioSource); });
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
                //BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80f);
                BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
                BaseAmountCard.SetActive(true);

                //ClickTextObject.SetActive(true);
                //ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                ClickTextObject.SetActive(true);
                ClickTextObject.GetComponent<RectTransform>().DOKill();
                ClickTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250f);
                ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-245, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
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
                    if (BeforeMagnification != TotalMagnification)
                    {
                        NumAudioSource = SoundManager.Instance.PlaySFX("Increase_Number");
                    }
                    DOTween.To(() => BeforeMagnification, x =>
                    {
                        BeforeMagnification = x;
                        MagnificationText.text = BeforeMagnification.ToString("F2");
                    }, TotalMagnification, 0.3f).OnComplete(() => { SoundManager.Instance.StopSFX(NumAudioSource); });
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
                if (IsAnimateComplete == true)
                {
                    IsAnimateComplete = false;
                    BaseAmountObject.SetActive(false);
                    MagnificationObject.SetActive(false);
                    FinalCalculateText.gameObject.SetActive(true);
                    FinalCalculateText.gameObject.GetComponent<RectTransform>().localScale = Vector2.zero;
                    FinalCalculateText.gameObject.GetComponent<RectTransform>().DOScale(Vector2.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() => { IsAnimateComplete = true; });
                    int BeforeFinalCalculate = 0;
                    if (BeforeFinalCalculate != (int)(BattleResult.FinalResultAmount - BattleResult.FinalResultAmountPlus))
                    {
                        NumAudioSource = SoundManager.Instance.PlaySFX("Increase_Number");
                    }
                    DOTween.To(() => BeforeFinalCalculate, x =>
                    {
                        BeforeFinalCalculate = x;
                        FinalCalculateText.text = BeforeFinalCalculate.ToString("F0");
                    }, (int)(BattleResult.FinalResultAmount - BattleResult.FinalResultAmountPlus), 0.5f).OnComplete(() => { SoundManager.Instance.StopSFX(NumAudioSource); });
                    //FinalCalculateText.text = ((int)(BattleResult.FinalResultAmount - BattleResult.FinalResultAmountPlus)).ToString();
                    break;
                }
            }

            //FinalCalculateObject�� ���������� ����Ѵ�.
            while (true)
            {
                yield return null;
                if (IsAnimateComplete == true)
                {
                    CurrentMainBattlePhase = (int)EMainBattlePhase.MergeComplete;
                    break;
                }
            }
            //FinalResultAmountPlus�� 1���� ũ�ٸ� AmountCard�� �ٽ� Ȱ��ȭ �Ѵ�.
            if (BattleResult.FinalResultAmountPlus >= 1)
            {
                IsAnimateComplete = false;
                //BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80f);
                BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
                BaseAmountCard.SetActive(true);

                //ClickTextObject.SetActive(true);
                //ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                ClickTextObject.SetActive(true);
                ClickTextObject.GetComponent<RectTransform>().DOKill();
                ClickTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250f);
                ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-245, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

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
                    if (BeforeFinalCalculate != (int)BattleResult.FinalResultAmount)
                    {
                        NumAudioSource = SoundManager.Instance.PlaySFX("Increase_Number");
                    }
                    DOTween.To(() => BeforeFinalCalculate, x =>
                    {
                        BeforeFinalCalculate = x;
                        FinalCalculateText.text = BeforeFinalCalculate.ToString("F0");
                    }, (int)BattleResult.FinalResultAmount, 0.5f).OnComplete(() => { SoundManager.Instance.StopSFX(NumAudioSource); });
                    //FinalCalculateText.text = ((int)BattleResult.FinalResultAmount).ToString();
                    BaseAmountCard.SetActive(false);
                    break;
                }
            }
        }
        else if(CurrentMainBattlePhase == (int)EMainBattlePhase.SpecialAction)
        {
            while(true)
            {
                yield return null;
                if(IsAnimateComplete == true)
                {
                    //Debug.Log("IsHere?");
                    BaseAmountCard.SetActive(false);
                    CurrentMainBattlePhase = (int)EMainBattlePhase.FinalPlusAmountComplete;
                    break;
                }
                //���⼭ ����
            }
        }
        
        //�ణ ��ٸ���
        yield return new WaitForSeconds(0.3f);

        //�ൿ�� ��ü( ���� Ȥ�� �÷��̾� �Ӹ� �������� �̵��Ѵ�)
        IsAnimateComplete = false;
        if (ActionObj.tag == "Player")
        {
            //�÷��̾� �Ӹ������� (���� PlayerActionTypePos������ �̵�)
            // �θ� RectTransform,���� ��ǥ �� ��ũ�� ��ǥ, ,��� anchoredPosition 
            Vector2 AnchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(FinalCalculateObject.transform.parent as RectTransform,
                Camera.main.WorldToScreenPoint(ActionObj.GetComponent<PlayerScript>().ActionTypePos.transform.position),
                Camera.main, out AnchoredPos);

            FinalCalculateObject.GetComponent<RectTransform>().DOAnchorPos(AnchoredPos, 0.5f).
                OnComplete(() => { IsAnimateComplete = true; });
        }
        else if(ActionObj.tag == "Monster")
        {
            Vector2 AnchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(FinalCalculateObject.transform.parent as RectTransform,
                Camera.main.WorldToScreenPoint(ActionObj.GetComponent<Monster>().GetMonActionTypePos()),
                Camera.main, out AnchoredPos);

            FinalCalculateObject.GetComponent<RectTransform>().DOAnchorPos(AnchoredPos, 0.5f).
                OnComplete(() => { IsAnimateComplete = true; });
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
        //������ ��ü�� ������ ���ٰ� ���� ��
        if (ActionObj.tag == "Player" && ActionString == "Attack")//�÷��̾��� ��� �����϶���
        {
            //���⿡ �������� ��밡 ���尡 �ִٸ� ��������-> ������ ���� �Ҹ��� ����.
            if(IsThereShield == true)//���Ͱ� ���带 ������ ������ -> ��������
            {
                SoundManager.Instance.PlaySFX("Shield_Block");
            }
            else//�Ȱ����� ������ -> ��������
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Hit_Sward", TargetPos + new Vector2(0,0.5f));
            }
            //overwhelmingpower �� ������ ������� ���������� ����Ʈ�� ������ ������?
            ActionObj.transform.DOPunchPosition(new Vector3(1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
            //PlayerZone.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, 0), 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { IsAnimateComplete = true; });
        }
        else if(ActionObj.tag == "Player" && ActionString == "Rest")
        {
            if(BattleResult.FinalResultAmount != 0)
                SoundManager.Instance.PlaySFX("Buff_Healing");

            IsAnimateComplete = true;
        }
        else if (ActionObj.tag == "Monster")//������ ��� ���� + Ư�� �ൿ�϶���
        {
            if (ActionString == "Attack")
            {
                if (IsThereShield == true)//�÷��̾ ���带 ������ ������ -> ��������
                {
                    SoundManager.Instance.PlaySFX("Shield_Block");
                }
                else//�÷��̾ �Ȱ����� ������ -> ��������
                {
                    EffectManager.Instance.ActiveEffect("BattleEffect_Hit_Mon_Sward", PlayerPos + new Vector2(0, 0.5f));//������ ������ ���� �޶����� ��������
                }
                ActionObj.transform.DOPunchPosition(new Vector3(-1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
            }
            else if(ActionString == "Poison")
            {
                SoundManager.Instance.PlaySFX("Buff_Consume");
                ActionObj.transform.DOPunchPosition(new Vector3(-1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
            }
            else if(ActionString == "MisFortune")
            {
                SoundManager.Instance.PlaySFX("Buff_Forcing");
                ActionObj.transform.DOPunchPosition(new Vector3(-1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
            }
            else if(ActionString == "CurseOfDeath")
            {
                SoundManager.Instance.PlaySFX("Buff_Consume");
                ActionObj.transform.DOPunchPosition(new Vector3(-1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
            }
            else if(ActionString == "Cower")
            {
                SoundManager.Instance.PlaySFX("Buff_Forcing");
                ActionObj.transform.DOPunchPosition(new Vector3(-1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
            }
            else
            {
                IsAnimateComplete = true;
            }
            //MonsterZone.GetComponent<RectTransform>().DOAnchorPos(new Vector2(350, 0), 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { IsAnimateComplete = true; });
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
        MainBattleUI.GetComponent<CanvasGroup>().DOFade(0f, 0.2f).OnComplete(() => { IsAnimateComplete = true; }); ;
        while(true)
        {
            yield return null;
            if(IsAnimateComplete == true)
            {
                MainBattleUI.SetActive(false);
                break;
            }
        }

        //���⿡�� ��ȯ?
        CallBack?.Invoke();
        Debug.Log("CoroutineEnd");
    }

    public void ClickAmountCard()
    {
        if(CurrentMainBattlePhase == (int)EMainBattlePhase.Nothing || CurrentMainBattlePhase == (int)EMainBattlePhase.BaseAmountComplete)//�������� ȸ���ϸ鼭
        {//���� ��ġ ���� ����, //�߰� ���� ��ġ ��������
            //BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-400, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-180, 130), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
        }
        else if(CurrentMainBattlePhase == (int)EMainBattlePhase.EquipMagnificationComplete)
        {//��� ���� �� �Ϸ� ���¶�� -> ������ ���� �߰� ����
            //BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(400, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(180, 130), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
        }
        else if(CurrentMainBattlePhase == (int)EMainBattlePhase.MergeComplete || CurrentMainBattlePhase == (int)EMainBattlePhase.SpecialAction)
        {//��ġ�°� �Ϸ� ���� -> ���� �߰� ��ġ
            //BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 130), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.GetComponent<RectTransform>().DOKill();
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
            SoundManager.Instance.PlaySFX("ReverseCard_Open");
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
            MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).OnComplete(() => { IsOpenCard = true; });
        }
        else if(IsOpenCard == true && IsOpenAnimationComplete == true && IsAnimateComplete == false)//ī�尡 ���� �ǰ�, ���� �ִϸ��̼��� �������� Ŭ���ϸ�
        {
            //MagnificationCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(400, 350), 0.5f);
            MagnificationCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(180, 130), 0.5f);
            MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
            MagnificationCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
        }
    }

    protected void PlayCardResultSound(int PositiveLink)
    {
        switch(PositiveLink)
        {
            case 0://����
                SoundManager.Instance.PlaySFX("CardOpen_Negative");
                break;
            case 1://���� 1��//��
                SoundManager.Instance.PlaySFX("CardOpen_Link01");
                break;
            case 2://���� 2��//��
                SoundManager.Instance.PlaySFX("CardOpen_Link02");
                break;
            case 3://���� 3��//��
                SoundManager.Instance.PlaySFX("CardOpen_Link03");
                break;
            case 4://���� 4��//��
                SoundManager.Instance.PlaySFX("CardOpen_Link04");
                break;
            case 5://���� 5��//��
                SoundManager.Instance.PlaySFX("CardOpen_Link05");
                break;
            case 6://���� 6��//��
                SoundManager.Instance.PlaySFX("CardOpen_Link06");
                break;
            case 7://���� 7��//��
                SoundManager.Instance.PlaySFX("CardOpen_Link07");
                break;
            case 8://���� 8��//��
                SoundManager.Instance.PlaySFX("CardOpen_Link08");
                break;
            default:
                break;
        }
    }

    public void VictoryBattle(int RewardExperience)//�¸�������
    {
        //�ൿ���� UI��Ȱ��ȭ
        if (PlayerActionSelectionBattleUI.activeSelf == true)
        {
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.5f).OnComplete(() => { PlayerActionSelectionBattleUI.SetActive(false); });
        }
        //���� �� ��ư UI ��Ȱ��ȭ
        if (MonsterActionSelectionBattleUI.activeSelf == true)
        {
            MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.5f).OnComplete(() => { MonsterActionSelectionBattleUI.SetActive(false); });
        }
        //�� ǥ�� UI��Ȱ��ȭ
        if (DisplayTurnUI.activeSelf == true)
        {
            DisplayTurnUIImages[0].transform.parent.gameObject.GetComponent<RectTransform>().DOKill();
            DisplayTurnUI.GetComponent<CanvasGroup>().alpha = 1;
            DisplayTurnUI.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => { DisplayTurnUI.SetActive(false); });
        }
        //�÷��̾� ���� ǥ�� ��Ȱ��ȭ
        PlayerBuffUI.InitBuffImage();
        //���� ���� UI��Ȱ��ȭ
        SelectionArrow.SetActive(false);
        //���� ��� UI��Ȱ��ȭ
        if (MonsterEquipmentUI.activeSelf == true)
        {
            MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -100);
            MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(100, 0.5f).OnComplete(() => { MonsterEquipmentUI.SetActive(false); });
        }
        //���� ���� UI��Ȱ��ȭ
        if (MonsterStatusUI.activeSelf == true)
        {
            MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, 125);
            MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.5f).OnComplete(() => { MonsterStatusUI.SetActive(false); });
        }
        //���� ���� ǥ�� ��Ȱ��ȭ
        foreach (BuffImageUIContainer obj in MonsterBuffUI)
        {
            obj.InitBuffImage();
        }
        //���� ���� �ൿ ǥ�� ��Ȱ��ȭ
        for(int i = 0; i < MonsterAttackIcons.Length; i++)
        {
            if (MonsterAttackIcons.Length > i && MonsterAttackIcons[i].activeSelf == true)
            {
                MonsterAttackIcons[i].SetActive(false);
            }
            if(MonsterDefenseIcons.Length > i && MonsterDefenseIcons[i].activeSelf == true)
            {
                MonsterDefenseIcons[i].SetActive(false);
            }
            if(MonsterAnotherIcons.Length > i && MonsterAnotherIcons[i].activeSelf == true)
            {
                MonsterAnotherIcons[i].SetActive(false);
            }
        }
        //�̰� ���� �¸� ������ �ö���°Ű�
        VictoryUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -1080);
        VictoryUI.SetActive(true);
        VictoryUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);
        VictoryText.text = "ȹ�� ����ġ : " + RewardExperience;
        SoundManager.Instance.PlayBGM("BaseBGM");
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
        //�ൿ���� UI��Ȱ��ȭ
        if (PlayerActionSelectionBattleUI.activeSelf == true)
        {
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.5f).OnComplete(() => { PlayerActionSelectionBattleUI.SetActive(false); });
        }
        //���� �� ��ư UI ��Ȱ��ȭ
        if (MonsterActionSelectionBattleUI.activeSelf == true)
        {
            MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.5f).OnComplete(() => { MonsterActionSelectionBattleUI.SetActive(false); });
        }
        //�� ǥ�� UI��Ȱ��ȭ
        if (DisplayTurnUI.activeSelf == true)
        {
            DisplayTurnUIImages[0].transform.parent.gameObject.GetComponent<RectTransform>().DOKill();
            DisplayTurnUI.GetComponent<CanvasGroup>().alpha = 1;
            DisplayTurnUI.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => { DisplayTurnUI.SetActive(false); });
        }
        //�÷��̾� ���� ǥ�� ��Ȱ��ȭ
        PlayerBuffUI.InitBuffImage();
        //���� ���� UI��Ȱ��ȭ
        SelectionArrow.SetActive(false);
        //���� ��� UI��Ȱ��ȭ
        if (MonsterEquipmentUI.activeSelf == true)
        {
            MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -100);
            MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(100, 0.5f).OnComplete(() => { MonsterEquipmentUI.SetActive(false); });
        }
        //���� ���� UI��Ȱ��ȭ
        if (MonsterStatusUI.activeSelf == true)
        {
            MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, 125);
            MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.5f).OnComplete(() => { MonsterStatusUI.SetActive(false); });
        }
        //���� ���� ǥ�� ��Ȱ��ȭ
        foreach (BuffImageUIContainer obj in MonsterBuffUI)
        {
            obj.InitBuffImage();
        }
        //���� ���� �ൿ ǥ�� ��Ȱ��ȭ
        for (int i = 0; i < MonsterAttackIcons.Length; i++)
        {
            if (MonsterAttackIcons.Length > i && MonsterAttackIcons[i].activeSelf == true)
            {
                MonsterAttackIcons[i].SetActive(false);
            }
            if (MonsterDefenseIcons.Length > i && MonsterDefenseIcons[i].activeSelf == true)
            {
                MonsterDefenseIcons[i].SetActive(false);
            }
            if (MonsterAnotherIcons.Length > i && MonsterAnotherIcons[i].activeSelf == true)
            {
                MonsterAnotherIcons[i].SetActive(false);
            }
        }

        DefeatUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -1080);
        DefeatUI.SetActive(true);
        DefeatUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);

        DefeatEventTitle.text = "���� �ִ� ���� (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
            ")\r\n�Ϲ� ���� (" + PlayerInfo.GetPlayerStateInfo().KillNormalMonster +
            ")\r\n����Ʈ ���� (" + PlayerInfo.GetPlayerStateInfo().KillEliteMonster +
            ")\r\n���� ����ġ (" + PlayerInfo.GetPlayerStateInfo().Experience +
            ")\r\n���� ����� (" + PlayerInfo.GetPlayerStateInfo().GoodKarma + ")";
        DefeatEventAmount.text = (int)(JsonReadWriteManager.Instance.E_Info.PlayerReachFloor * 2000) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().KillNormalMonster * 500) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().KillEliteMonster * 700) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().Experience) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().GoodKarma * 200);
        /*
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
        */
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
            ")\r\n�Ϲ� ���� (" + PlayerInfo.GetPlayerStateInfo().KillNormalMonster +
            ")\r\n����Ʈ ���� (" + PlayerInfo.GetPlayerStateInfo().KillEliteMonster +
            ")\r\n���� ����ġ (" + PlayerInfo.GetPlayerStateInfo().Experience +
            ")\r\n���� ����� (" + PlayerInfo.GetPlayerStateInfo().GoodKarma + ")";
        WinEventAmount.text = (int)(JsonReadWriteManager.Instance.E_Info.PlayerReachFloor * 2000) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().KillNormalMonster * 500) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().KillEliteMonster * 700) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().Experience) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().GoodKarma * 200);
        /*
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
        */
        WinEquipSuccessionText.text = "������ ����� ������ " + (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIPSUC") + "���� ��� ��µ˴ϴ�.";
        WinEarlyPoint.text = "���� ��ȭ ����Ʈ : " + JsonReadWriteManager.Instance.E_Info.PlayerEarlyPoint;
    }

    public void ClickDefeatButton()
    {

    }
}
