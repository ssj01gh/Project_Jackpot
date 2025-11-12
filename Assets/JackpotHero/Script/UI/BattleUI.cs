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
    public Canvas WorldCanvas;
    public GameObject UI_StartOfBattle;
    [Header("ActionSelectionUI")]
    public GameObject PlayerActionSelectionBattleUI;
    public Button PlayerAttackButton;
    public Button PlayerDefenseButton;
    public Button PlayerRegenSTAButton;
    public GameObject MonsterActionSelectionBattleUI;
    [Header("MainBattleUI")]
    public GameObject MainBattleUI;
    public GameObject[] HeroineSprites;//7번
    public GameObject BaseAmountObject;//1번
    public TextMeshProUGUI BaseAmountText;//1번
    public GameObject MagnificationObject;//2번
    public TextMeshProUGUI MagnificationText;//2번
    public GameObject BaseAmountCard;//3번
    public TextMeshProUGUI BaseAmountCardTitleText;//3번
    public TextMeshProUGUI BaseAmountCardDetailText;//3번
    public GameObject MagnificationCard;//3번
    public GameObject UpperMGLine;
    public GameObject LowerMGLine;
    public GameObject[] UpperMGCard;
    public GameObject[] UpperMGVirtualCard;
    public GameObject[] LowerMGCard;
    public GameObject[] LowerMGVirtualCard;
    public GameObject[] ActionTypeObject;//4번
    public GameObject FinalCalculateObject;//4번
    public TextMeshProUGUI FinalCalculateText;//4번
    public GameObject ClickTextObject;//5번
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
    public GameObject[] MonsterPrideIcons;
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

    protected int TotalOpenCard = 0;
    protected List<float> UpperMGList = new List<float>();
    protected List<float> LowwerMGList = new List<float>();
    protected int PositiveLink = 0;
    protected int MergeCompleteCardCount = 0;

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

    private Vector3 InitMGPosRotation = new Vector3(0, 0, 0);
    private Vector3 InitMGScale = new Vector3(1, 1, 1);
    private Color InitMGColor = new Color(1, 1, 1, 1);

    private Color TargetMGColor = new Color(1, 1, 1, 0);
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

    public void ActiveBattleUI()//전투를 시작할때 한번만
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

    public void ActiveBattleSelectionUI(bool IsFear, float AttackAver, float DefenseAver, float RestAver)//플레이어의 턴이 올때마다
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
    public void SetPlayerBattleUI(PlayerScript PlayerInfo)//쉴드랑 버프
    {
        //일단 다 끄고
        PlayerShield.SetActive(false);
        PlayerBrokenShieldObject.SetActive(false);
        PlayerBuffUI.InitBuffImage();

        if(PlayerInfo.BeforeShield <= 0 && PlayerInfo.GetPlayerStateInfo().ShieldAmount > 0)//활성화
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
        else if(PlayerInfo.BeforeShield > 0 && PlayerInfo.GetPlayerStateInfo().ShieldAmount <= 0)//비활성화
        {
            //부숴지는것처럼 생긴 쉴드 초기화
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
        //일단 다끄기
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
            MonsterPrideIcons[i].SetActive(false);
        }

        for (int i = 0; i < ActiveMonsters.Count; i++)
        {
            int index = i;
            Monster Mon = ActiveMonsters[i].GetComponent<Monster>();
            //SetCurrentActionIcon;
            Vector2 ActiveIconPosition = Mon.HpSliderPos.transform.position;
            ActiveIconPosition.x -= (Mon.HpsliderWidth / 100 / 2) + 0.2f;

            if (Mon.MonsterBuff.BuffList[(int)EBuffType.Pride] >= 1)
            {
                MonsterPrideIcons[i].SetActive(true);
                MonsterPrideIcons[i].transform.position = ActiveIconPosition;
            }
            else
            {
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
            }

            if(Mon.BeforeMonsterShield <= 0 && Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint > 0)//활성화
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
            else if(Mon.BeforeMonsterShield > 0 && Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint <= 0)//비활성화
            {
                //부숴지는것처럼 생긴 쉴드 초기화
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

    public void UnActiveMonsterBattleUI(GameObject Monster)//인수로 들어온 Monster에 해당하는 UI만 해제 가능?
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

    public void DisplayMonsterDetailUI(Monster Mon)//CurrentTarget이 바뀌였을때 불러와짐 -> 그 화살표도 같이 이동//죽은 얘를 가리키면 끄기
    {
        MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 100);
        MonsterEquipmentUI.SetActive(true);//자동적으로 ActiveAnimation
        MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(-100, 0.4f).SetEase(Ease.OutBack);

        MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, -125);
        MonsterStatusUI.SetActive(true);//자동적으로 Activeanimation
        MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(125, 0.4f).SetEase(Ease.OutBack);

        SelectionArrow.SetActive(true);
        Vector2 SelectionArrowPos = Mon.transform.position;
        SelectionArrowPos.y -= 0.5f;
        SelectionArrow.transform.position = SelectionArrowPos;

        EquipmentInfo MonWeapon = EquipmentInfoManager.Instance.GetMonEquipmentInfo(Mon.MonsterWeaponCode);
        EquipmentInfo MonArmor = EquipmentInfoManager.Instance.GetMonEquipmentInfo(Mon.MonsterArmorCode);

        List<EquipmentInfo> AnotherEquip = new List<EquipmentInfo>();
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

        //파랑이 긍정 빨강이 부정
        //힘
        if (Mon.GetMonsterCurrentBaseStatus("STR") < Mon.GetMonsterCurrentStatus().MonsterCurrentATK)
            MonsterSTRText.color = Color.blue;
        else if(Mon.GetMonsterCurrentBaseStatus("STR") > Mon.GetMonsterCurrentStatus().MonsterCurrentATK)
            MonsterSTRText.color = Color.red;
        else
            MonsterSTRText.color = Color.white;
        //내구
        if (Mon.GetMonsterCurrentBaseStatus("DUR") < Mon.GetMonsterCurrentStatus().MonsterCurrentDUR)
            MonsterDURText.color = Color.blue;
        else if (Mon.GetMonsterCurrentBaseStatus("DUR") > Mon.GetMonsterCurrentStatus().MonsterCurrentDUR)
            MonsterDURText.color = Color.red;
        else
            MonsterDURText.color = Color.white;
        //행운
        if (Mon.GetMonsterCurrentBaseStatus("LUK") < Mon.GetMonsterCurrentStatus().MonsterCurrentLUK)
            MonsterLUKText.color = Color.blue;
        else if (Mon.GetMonsterCurrentBaseStatus("LUK") > Mon.GetMonsterCurrentStatus().MonsterCurrentLUK)
            MonsterLUKText.color = Color.red;
        else
            MonsterLUKText.color = Color.white;
        //속도
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

    public void ActiveMainBattleUI(GameObject ActionObj, Monster CurrentTarget, string ActionString, BattleResultStates BattleResult, Vector2 PlayerPos, bool IsThereShield, Action CallBack)//메인 전투 시작
    {//(현재 차례인 오브젝트, 타겟이 된 몬스터(플레이어 턴일때), 오브젝트의 행동, 행동 결과, 모든 행동이 끝나고 실행할 함수)
        //필요한것은 Effect를 불러올 플레이어의 Vector와 몬스터의 Vector 굳이 Player와 Monster의 모든 정보를 안가져 와도 될듯?
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

        if(ActionString != "Attack" && ActionString != "Defense" &&  ActionString != "Rest" && ActionString != "Charm")
        {//Another일때
            BaseAmountObject.SetActive(false);
            MagnificationObject.SetActive(false);
        }
        else
        {
            //1번 활성화
            BaseAmountObject.SetActive(true);
            //BaseAmountObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 350);
            BaseAmountObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-180, 130);
            BaseAmountObject.GetComponent<RectTransform>().localScale = Vector2.one;
            BaseAmountText.text = "0";

            //2번 활성화
            MagnificationObject.SetActive(true);
            //MagnificationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 350);
            MagnificationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(180, 130);
            MagnificationObject.GetComponent<RectTransform>().localScale = Vector2.one;
            MagnificationText.text = "1.00";
        }
        //일단 3번들 다 비활성화 //밑의 코루틴에서 해결할듯
        BaseAmountCard.SetActive(false);//3번
        BaseAmountCardTitleText.text = "";//3번
        BaseAmountCardDetailText.text = "";//3번
        MagnificationCard.SetActive(false);//3번
        InitAllMGCard();//3번

        //0. Attack, 1. Defense, 2. Rest, 3. Another 4.Charm
        //행동 상태에 맞는 모양 활성화 //4번 활성화
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
            case "Charm":
                ActionTypeObject[4].SetActive(true);
                break;
            default:
                ActionTypeObject[3].SetActive(true);
                break;
        }
        Vector2 TargetPos = Vector2.zero;
        if (CurrentTarget != null)
            TargetPos = CurrentTarget.gameObject.transform.position;

        StartCoroutine(ProgressMainBattle_UI(ActionObj, ActionString, BattleResult, PlayerPos, TargetPos, IsThereShield, CallBack));
        //DoTween.OnComplete로 하면 코루틴으로 안하고 함수들키리 이벤트성을 연결가능하지 않나? 그게 더 안좋으려나?
    }
    IEnumerator ProgressMainBattle_UI(GameObject ActionObj, string ActionString, BattleResultStates BattleResult, Vector2 PlayerPos, Vector2 TargetPos, bool IsThereShield, Action CallBack)
    {
        AudioSource NumAudioSource = new AudioSource();
        CurrentMainBattlePhase = (int)EMainBattlePhase.Nothing;
        IsOpenCard = false;
        IsOpenAnimationComplete = false;
        //우선 기초 수치(현재 스텟)카드를 나타나게 한다.
        //BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80f);
        BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
        BaseAmountCard.SetActive(true);//3번

        ClickTextObject.SetActive(true);
        ClickTextObject.GetComponent<RectTransform>().DOKill();
        ClickTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250f);
        ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-245, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        switch (ActionString)
        {
            case "Charm":
            case "Attack":
                BaseAmountCardTitleText.text = "현재 힘";
                BaseAmountCardDetailText.text = ((int)BattleResult.BaseAmount).ToString();
                break;
            case "Defense":
                BaseAmountCardTitleText.text = "현재 내구";
                BaseAmountCardDetailText.text = ((int)BattleResult.BaseAmount).ToString();
                break;
            case "Rest":
                BaseAmountCardTitleText.text = "현재 회복";
                BaseAmountCardDetailText.text = ((int)BattleResult.BaseAmount).ToString();
                break;
            default:
                CurrentMainBattlePhase = (int)EMainBattlePhase.SpecialAction;
                BaseAmountCardTitleText.text = "특수 행동";
                BaseAmountCardDetailText.text = "";
                //특수 행동에 대한 설명?
                break;
        }
        //첫번째 카드를 클릭하고 애니메이션이 끝날때까지 대기
        IsAnimateComplete = false;
        if(CurrentMainBattlePhase != (int)EMainBattlePhase.SpecialAction)
        {
            while (true)
            {
                yield return null;
                if (IsAnimateComplete == true)
                {
                    //애니메이션이 끝나면 1번의 수 증가, 빠져나요기
                    int BaseAmount = 0;
                    if (BaseAmount != (int)BattleResult.BaseAmount)//값이 다를때
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
            //추가 기초 수치가 존재한다면 다시 활성화
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
                BaseAmountCardTitleText.text = "추가 기초 수치";
                BaseAmountCardDetailText.text = ((int)BattleResult.BaseAmountPlus).ToString();
            }
            //첫번째 카드를 클릭하고 애니메이션이 끝날때까지 대기
            while (BaseAmountCard.activeSelf == true)
            {
                yield return null;
                if (IsAnimateComplete)
                {
                    //애니메이션이 끝나면 1번의 수 증가, 빠져나요기//숫자가 한번에 변하는것이 아닌 조금씩 변화하면 더 좋을것 같기도 함
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

            //2번으로 향할 3번 활성화
            //이걸 BattleResult.Count만큼 반복 시켜야함
            float TotalMagnification = 1f;
            TotalOpenCard = 0;
            PositiveLink = 0;
            UpperMGList.Clear();
            LowwerMGList.Clear();

            //BattleResult.Count만큼 활성화 및 위 아래의 카드 개봉시 수치 나누기
            SetMGCardActive(BattleResult.ResultMagnification);
            //클릭할 수 있는 카드는 활성화 되어있음
            //각 카드를 누를때마다 PositiveLink가 증가할지 0으로 될지 결정됨
            //각 카드가 몇번째 카드인지 확인할 수 있어야함 (왼쪽 위 부터 1번)
            //개방된 카드가 ResultMagnification.count 이상이 되면 다음으로 넘어감
            while(true)
            {//개방된 카드가 ResultMagnification.count 이상이 되면 다음으로 넘어감
                //
                yield return null;
                if (TotalOpenCard >= BattleResult.ResultMagnification.Count)
                {//여기까지 오면 카드가 다 개방된거임
                    break;
                }

            }

            yield return new WaitForSeconds(0.3f);//잠깐 기다렸다가

            MergeCompleteCardCount = 0;
            //모든 Active상태의 Virtual카드가 날라가게
            MergeToMagnificationSlot();

            while (true)
            {
                yield return null;
                if (MergeCompleteCardCount >= BattleResult.ResultMagnification.Count)
                {
                    float BeforeMagnification = TotalMagnification;

                    for(int i = 0; i < BattleResult.ResultMagnification.Count; i++)
                    {
                        TotalMagnification *= BattleResult.ResultMagnification[i];
                    }

                    if (BeforeMagnification != TotalMagnification)//*1이 아닐때
                    {
                        NumAudioSource = SoundManager.Instance.PlaySFX("Increase_Number");
                    }
                    DOTween.To(() => BeforeMagnification, x =>
                    {
                        BeforeMagnification = x;
                        MagnificationText.text = BeforeMagnification.ToString("F2");
                    }, TotalMagnification, 0.3f).OnComplete(() => { SoundManager.Instance.StopSFX(NumAudioSource); });
                    break;
                }
            }
            /*
            while (RepeatCount < BattleResult.ResultMagnification.Count)
            {
                yield return null;
                //MagnificationCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                MagnificationCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80f);
                MagnificationCard.GetComponent<RectTransform>().localScale = Vector2.one;
                MagnificationCard.SetActive(true);
                MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);//?카드로 초기화

                IsOpenCard = false;
                IsOpenAnimationComplete = false;
                IsAnimateComplete = false;
                //ClickTextObject.SetActive(true);
                //ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                ClickTextObject.SetActive(true);
                ClickTextObject.GetComponent<RectTransform>().DOKill();
                ClickTextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250f);
                ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-245, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                //카드 공개의 50프로까지 진행될때까지 대기
                while (true)
                {
                    yield return null;
                    if (IsOpenCard == true)
                    {
                        //애니메이션이 50프로 까지 진행되면 카드에 결과 출력
                        if (BattleResult.ResultMagnification[RepeatCount] >= 1)//긍정이라면
                        {
                            PositiveLink++;
                        }
                        else
                        {
                            PositiveLink = 0;
                        }
                        MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(BattleResult.ResultMagnification[RepeatCount]);//0번째 결과 출력
                        MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).OnComplete(() =>
                        {
                            //MagnificationCard.GetComponent<RectTransform>().DORotate
                            PlayCardResultSound(PositiveLink);//계속 긍정이 되면 피치가 계속 올라감
                            IsOpenAnimationComplete = true;
                        });
                        break;
                    }
                }

                //숫자 카드가 다 나타날때 까지 대기
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

                //이쪽이 곱 카드 표시하는곳
                //카드 클릭 애니메이션이 진행될때까지 대기
                while (true)
                {
                    yield return null;
                    if (IsAnimateComplete == true)
                    {
                        //카드 클릭 애니메이션 재생이 끝나면 비활성화 후 결과값을 출력
                        MagnificationCard.SetActive(false);
                        float BeforeMagnification = TotalMagnification;
                        TotalMagnification *= BattleResult.ResultMagnification[RepeatCount];
                        if (BeforeMagnification != TotalMagnification)//*1이 아닐때
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
            */

            //여기 왔다는것은 장비에의한 곱이 완료된거임
            CurrentMainBattlePhase = (int)EMainBattlePhase.EquipMagnificationComplete;
            if (BattleResult.BuffMagnification != 1f)//버프에 의한 증감이 존재 할때
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
                BaseAmountCardTitleText.text = "버프 배율";
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
                    //애니메이션이 끝나면 1번의 수 증가, 빠져나요기//숫자가 한번에 변하는것이 아닌 조금씩 변화하면 더 좋을것 같기도 함
                    //MagnificationText.text = TotalMagnification.ToString("F2");
                    CurrentMainBattlePhase = (int)EMainBattlePhase.BuffMagnificationComplete;
                    break;
                }
            }

            //약간 기다린다
            yield return new WaitForSeconds(0.3f);
            //5번 실행
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

            //FinalCalculateObject가 끝날때까지 대기한다.
            while (true)
            {
                yield return null;
                if (IsAnimateComplete == true)
                {
                    CurrentMainBattlePhase = (int)EMainBattlePhase.MergeComplete;
                    break;
                }
            }
            //FinalResultAmountPlus가 1보다 크다면 AmountCard를 다시 활성화 한다.
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

                BaseAmountCardTitleText.text = "추가 최종 수치";
                BaseAmountCardDetailText.text = ((int)BattleResult.FinalResultAmountPlus).ToString();
            }

            while (BaseAmountCard.activeSelf == true)
            {
                yield return null;
                if (IsAnimateComplete)
                {
                    //애니메이션이 끝나면 1번의 수 증가, 빠져나요기//숫자가 한번에 변하는것이 아닌 조금씩 변화하면 더 좋을것 같기도 함
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
                //여기서 부터
            }
        }
        
        //약간 기다린다
        yield return new WaitForSeconds(0.3f);

        //행동한 주체( 몬스터 혹은 플레이어 머리 위쪽으로 이동한다)
        IsAnimateComplete = false;
        if (ActionObj.tag == "Player")
        {
            //플레이어 머리쪽으로 (들어온 PlayerActionTypePos쪽으로 이동)
            // 부모 RectTransform,월드 좌표 → 스크린 좌표, ,결과 anchoredPosition 
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
        //약간 기다린다
        yield return new WaitForSeconds(0.3f);
        IsAnimateComplete = false;
        //이거에 대한 예외사항은 나중에 계속 늘어날듯?
        //공격한 주체가 앞으로 갔다가 오는 거
        if (ActionObj.tag == "Player" && ActionString == "Attack")//플레이어일 경우 공격일때만
        {
            //여기에 들어왓을때 상대가 쉴드가 있다면 막힌거임-> 공격이 막힌 소리를 낸다.
            if(IsThereShield == true)//몬스터가 쉴드를 가지고 있을떄 -> 막힌거임
            {
                SoundManager.Instance.PlaySFX("Shield_Block");
            }
            else//안가지고 있을때 -> 뚫은거임 혹은 매혹상태거나
            {
                EffectManager.Instance.ActiveEffect("BattleEffect_Hit_Sward", TargetPos + new Vector2(0,0.5f));
            }
            //overwhelmingpower 이 있으면 사방으로 퍼져나가는 이펙트가 있으면 좋을듯?
            ActionObj.transform.DOPunchPosition(new Vector3(1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
            //PlayerZone.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, 0), 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { IsAnimateComplete = true; });
        }
        else if(ActionObj.tag == "Player" && ActionString == "Charm")
        {
            Vector2 ActionObjPos = ActionObj.transform.position;
            EffectManager.Instance.ActiveEffect("BattleEffect_Hit_Sward", ActionObjPos + new Vector2(0.5f, 0.5f));

            IsAnimateComplete = true;
        }
        else if (ActionObj.tag == "Player" && ActionString == "Rest")
        {
            if (BattleResult.FinalResultAmount != 0)
                SoundManager.Instance.PlaySFX("Buff_Healing");

            IsAnimateComplete = true;
        }
        else if (ActionObj.tag == "Monster")//몬스터일 경우 공격 + 특수 행동일때만
        {
            if (ActionString == "Attack")
            {
                if (IsThereShield == true && ActionString == "Attack")//플레이어가 쉴드를 가지고 있을떄 -> 막힌거임
                {
                    SoundManager.Instance.PlaySFX("Shield_Block");
                }
                else//플레이어가 안가지고 있을때 -> 뚫은거임
                {
                    EffectManager.Instance.ActiveEffect("BattleEffect_Hit_Mon_Sward", PlayerPos + new Vector2(0, 0.5f));//몬스터의 종류에 따라 달라지면 좋을지두
                }

                if(ActionObj.GetComponent<Monster>().IsHaveAttackAnimation == false)
                {
                    ActionObj.transform.DOPunchPosition(new Vector3(-1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
                }
                else
                {
                    ActionObj.GetComponent<Monster>().SetMonsterAnimation("Attack");
                }
            }
            else if (ActionString == "Charm")
            {
                Vector2 ActionObjPos = ActionObj.transform.position;
                EffectManager.Instance.ActiveEffect("BattleEffect_Hit_Mon_Sward", ActionObjPos + new Vector2(-0.5f, 0.5f));

                if (ActionObj.GetComponent<Monster>().IsHaveAttackAnimation == false)
                {
                    IsAnimateComplete = true;
                }
                else
                {
                    ActionObj.GetComponent<Monster>().SetMonsterAnimation("Attack");
                }
            }
            else if (ActionString == "Poison" || ActionString == "CurseOfDeath" || ActionString == "Burn")
            {
                SoundManager.Instance.PlaySFX("Buff_Consume");
                ActionObj.transform.DOPunchPosition(new Vector3(-1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
            }
            else if (ActionString == "MisFortune" || ActionString == "Envy" || ActionString == "Cower" || ActionString == "DefenseDebuff" || ActionString == "AttackDebuff" ||
                ActionString == "OverCharge" || ActionString == "GiveCharm" || ActionString == "Petrification")
            {
                SoundManager.Instance.PlaySFX("Buff_Forcing");
                ActionObj.transform.DOPunchPosition(new Vector3(-1, 0, 0), 0.2f, 1, 1).OnComplete(() => { IsAnimateComplete = true; });
            }
            else if(ActionString == "Greed" || ActionString == "Charging")
            {
                SoundManager.Instance.PlaySFX("Acquire_EXP");
                IsAnimateComplete = true;
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
            else
            {
                if(ActionObj.tag == "Monster" && ActionObj.GetComponent<Monster>().IsHaveAttackAnimation == true)
                {
                    if(ActionObj.GetComponent<Monster>().CheckmonsterAnimationEnd("Attack") == true)
                    {
                        IsAnimateComplete = true;
                    }
                    else
                    {
                        IsAnimateComplete = false;
                    }
                }
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

        //여기에서 소환?
        CallBack?.Invoke();
        Debug.Log("CoroutineEnd");
    }

    public void ClickAmountCard()
    {
        if(CurrentMainBattlePhase == (int)EMainBattlePhase.Nothing || CurrentMainBattlePhase == (int)EMainBattlePhase.BaseAmountComplete)//왼쪽으로 회전하면서
        {//기초 수치 도착 지점, //추가 기초 수치 도착지점
            //BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-400, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-180, 130), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
        }
        else if(CurrentMainBattlePhase == (int)EMainBattlePhase.EquipMagnificationComplete)
        {//장비에 의한 곱 완료 상태라면 -> 버프에 의한 추가 곱임
            //BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(400, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(180, 130), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
        }
        else if(CurrentMainBattlePhase == (int)EMainBattlePhase.MergeComplete || CurrentMainBattlePhase == (int)EMainBattlePhase.SpecialAction)
        {//합치는게 완료 상태 -> 최종 추가 수치
            //BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 130), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
        }

        /*
        if(IsOpenAnimationComplete == false)//추가 기초 수치
        {
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-400, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.SetActive(false);
        }
        else if(IsOpenAnimationComplete == true)//추가 최종 수치
        {
            BaseAmountCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 350), 0.5f);
            BaseAmountCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
            BaseAmountCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.SetActive(false);
        }
        */
    }

    public void ClickMagnificationCard(int CardNum)//여는거 연출
    {//UpperCard == 0 ~ 3 // LowwerCard == 4 ~ 7
        //클릭 텍스트를 띄워야 하려나? -> 이건 일단 나중에 하면 될듯?
        //일단 클릭되면 클릭된 당사자는 투명해지고 ButtonInteratable false로
        //, Virtual카드들이 뒤집히는 연출을함
        /*
        if(IsOpenCard == false)//아직 안열였다면//IsOpenCard를 true로 함 -> 코루틴에서 값에 맞는 카드로 바뀜
        {
            SoundManager.Instance.PlaySFX("ReverseCard_Open");
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
            MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).OnComplete(() => { IsOpenCard = true; });
        }
        else if(IsOpenCard == true && IsOpenAnimationComplete == true && IsAnimateComplete == false)//카드가 공개 되고, 공개 애니메이션이 끝났을때 클릭하면
        {//이게 날라가는거
            //MagnificationCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(400, 350), 0.5f);
            MagnificationCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(180, 130), 0.5f);
            MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
            MagnificationCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
        }
        */
        if(CardNum <= 3 && CardNum >= 0)
        {//UpperCard
            UpperMGCard[CardNum].GetComponent<Image>().color = TargetMGColor;
            UpperMGCard[CardNum].GetComponent<Button>().interactable = false;
            //TotalOpenCard++;

            SoundManager.Instance.PlaySFX("ReverseCard_Open");
            UpperMGVirtualCard[CardNum].SetActive(true);
            //뒤집는 애니메이션이 완료됬을때 카드를 
            UpperMGVirtualCard[CardNum].GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).OnComplete(() => 
            {
                if (UpperMGList[CardNum] >= 1)//긍정
                    PositiveLink++;
                else//부정
                    PositiveLink = 0;

                UpperMGVirtualCard[CardNum].GetComponent<Image>().sprite = 
                EquipmentInfoManager.Instance.GetEquipmentSlotSprite(UpperMGList[CardNum]);//클릭한 카드에 맞는 결과 출력
                UpperMGVirtualCard[CardNum].GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).OnComplete(() =>
                {
                    TotalOpenCard++;
                    PlayCardResultSound(PositiveLink);//계속 긍정이 되면 피치가 계속 올라감
                });
            });
        }
        else if(CardNum >= 4 && CardNum <= 7)
        {//LowerCard
            int FixedCardNum = CardNum - 4;
            LowerMGCard[FixedCardNum].GetComponent<Image>().color = TargetMGColor;
            LowerMGCard[FixedCardNum].GetComponent<Button>().interactable = false;

            SoundManager.Instance.PlaySFX("ReverseCard_Open");
            LowerMGVirtualCard[FixedCardNum].SetActive(true);
            //뒤집는 애니메이션이 완료됬을때 카드를 
            LowerMGVirtualCard[FixedCardNum].GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).OnComplete(() =>
            {
                if (LowwerMGList[FixedCardNum] >= 1)//긍정
                    PositiveLink++;
                else//부정
                    PositiveLink = 0;

                LowerMGVirtualCard[FixedCardNum].GetComponent<Image>().sprite =
                EquipmentInfoManager.Instance.GetEquipmentSlotSprite(LowwerMGList[FixedCardNum]);//클릭한 카드에 맞는 결과 출력
                LowerMGVirtualCard[FixedCardNum].GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).OnComplete(() =>
                {
                    TotalOpenCard++;
                    PlayCardResultSound(PositiveLink);//계속 긍정이 되면 피치가 계속 올라감
                });
            });
        }
    }

    protected void MergeToMagnificationSlot()//합쳐지는 연출//Active됬던 모든 Virtual카드
    {
        RectTransform CanvasRect = (RectTransform)WorldCanvas.transform;
        Vector3 TargetPivotWorldPos = MagnificationObject.transform.position;
        Vector2 CanvasLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect,
            RectTransformUtility.WorldToScreenPoint(WorldCanvas.worldCamera, TargetPivotWorldPos), WorldCanvas.worldCamera, out CanvasLocal);

        Vector3 World = CanvasRect.TransformPoint(CanvasLocal);

        foreach (GameObject UCard in UpperMGVirtualCard)
        {
            if(UCard.activeSelf == true)
            {
                Vector3 TargetPos = UCard.transform.parent.InverseTransformPoint(World);

                UCard.GetComponent<RectTransform>().DOAnchorPos(TargetPos, 0.5f);
                UCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
                UCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => 
                { 
                    MergeCompleteCardCount++;
                    UCard.SetActive(false);
                });
            }

        }
        foreach(GameObject LCard in LowerMGVirtualCard)
        {
            Vector3 TargetPos = LCard.transform.parent.InverseTransformPoint(World);

            LCard.GetComponent<RectTransform>().DOAnchorPos(TargetPos, 0.5f);
            LCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
            LCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => 
            { 
                MergeCompleteCardCount++;
                LCard.SetActive(false);
            });
        }
        /*
        if (IsOpenCard == true && IsOpenAnimationComplete == true && IsAnimateComplete == false)//카드가 공개 되고, 공개 애니메이션이 끝났을때 클릭하면
        {//이게 날라가는거
         //MagnificationCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(400, 350), 0.5f);
            MagnificationCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(180, 130), 0.5f);
            MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
            MagnificationCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.GetComponent<RectTransform>().DOKill();
            ClickTextObject.SetActive(false);
        }
        */
    }

    protected void InitAllMGCard()
    {//초기화 == 일단 카드들 다 비활성화, 초기 상태로 되될리기
        UpperMGLine.SetActive(false);
        LowerMGLine.SetActive(false);
        foreach (GameObject obj in UpperMGCard)
        {
            obj.SetActive(false);
            obj.GetComponent<Image>().color = InitMGColor;
            obj.GetComponent<Button>().interactable = true;
        }
        foreach (GameObject obj in UpperMGVirtualCard)
        {
            obj.SetActive(false);
            obj.GetComponent<RectTransform>().anchoredPosition = InitMGPosRotation;
            obj.GetComponent<RectTransform>().localScale = InitMGScale;
            obj.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);
        }
        foreach (GameObject obj in LowerMGCard)
        {
            obj.SetActive(false);
            obj.GetComponent<Image>().color = InitMGColor;
            obj.GetComponent<Button>().interactable = true;
        }
        foreach (GameObject obj in LowerMGVirtualCard)
        {
            obj.SetActive(false);
            obj.GetComponent<RectTransform>().anchoredPosition = InitMGPosRotation;
            obj.GetComponent<RectTransform>().localScale = InitMGScale;
            obj.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);
        }
        //MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);//?카드로 초기화
        //EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);
    }

    protected void SetMGCardActive(List<float> MagnificationList)
    {
        int ListCount = MagnificationList.Count;
        int UpperInt = 0;
        int LowwerInt = 0;
        switch (ListCount)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                UpperInt = ListCount;
                UpperMGLine.SetActive(true);
                break;
            case 5://3 2
                UpperInt = 3;
                LowwerInt = 2;
                UpperMGLine.SetActive(true);
                LowerMGLine.SetActive(true);
                break;
            case 6:// 4 2
                UpperInt = 4;
                LowwerInt = 2;
                UpperMGLine.SetActive(true);
                LowerMGLine.SetActive(true);
                break;
            case 7://4 3
                UpperInt = 4;
                LowwerInt = 3;
                UpperMGLine.SetActive(true);
                LowerMGLine.SetActive(true);
                break;
            case 8://4 4
                UpperInt = 4;
                LowwerInt = 4;
                UpperMGLine.SetActive(true);
                LowerMGLine.SetActive(true);
                break;
        }

        if(UpperInt != 0)
        {
            for (int i = 0; i < UpperInt; i++)
            {
                UpperMGCard[i].SetActive(true);
                UpperMGList.Add(MagnificationList[i]);
            }
        }
        if(LowwerInt != 0)
        {
            for (int i = 0; i < LowwerInt; i++)
            {
                LowerMGCard[i].SetActive(true);
                LowwerMGList.Add(MagnificationList[UpperInt + i]);
            }
        }
    }

    protected void PlayCardResultSound(int PositiveLink)
    {
        switch(PositiveLink)
        {
            case 0://부정
                SoundManager.Instance.PlaySFX("CardOpen_Negative");
                break;
            case 1://긍정 1연//도
                SoundManager.Instance.PlaySFX("CardOpen_Link01");
                break;
            case 2://긍정 2연//레
                SoundManager.Instance.PlaySFX("CardOpen_Link02");
                break;
            case 3://긍정 3연//미
                SoundManager.Instance.PlaySFX("CardOpen_Link03");
                break;
            case 4://긍정 4연//파
                SoundManager.Instance.PlaySFX("CardOpen_Link04");
                break;
            case 5://긍정 5연//솔
                SoundManager.Instance.PlaySFX("CardOpen_Link05");
                break;
            case 6://긍정 6연//라
                SoundManager.Instance.PlaySFX("CardOpen_Link06");
                break;
            case 7://긍정 7연//시
                SoundManager.Instance.PlaySFX("CardOpen_Link07");
                break;
            case 8://긍정 8연//도
                SoundManager.Instance.PlaySFX("CardOpen_Link08");
                break;
            default:
                break;
        }
    }

    public void VictoryBattle(int RewardExperience)//승리했을때
    {
        //행동선택 UI비활성화
        if (PlayerActionSelectionBattleUI.activeSelf == true)
        {
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.5f).OnComplete(() => { PlayerActionSelectionBattleUI.SetActive(false); });
        }
        //몬스터 턴 버튼 UI 비활성화
        if (MonsterActionSelectionBattleUI.activeSelf == true)
        {
            MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.5f).OnComplete(() => { MonsterActionSelectionBattleUI.SetActive(false); });
        }
        //턴 표시 UI비활성화
        if (DisplayTurnUI.activeSelf == true)
        {
            DisplayTurnUIImages[0].transform.parent.gameObject.GetComponent<RectTransform>().DOKill();
            DisplayTurnUI.GetComponent<CanvasGroup>().alpha = 1;
            DisplayTurnUI.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => { DisplayTurnUI.SetActive(false); });
        }
        //플래이어 버프 표시 비활성화
        PlayerBuffUI.InitBuffImage();
        //몬스터 선택 UI비활성화
        SelectionArrow.SetActive(false);
        //몬스터 장비 UI비활성화
        if (MonsterEquipmentUI.activeSelf == true)
        {
            MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -100);
            MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(100, 0.5f).OnComplete(() => { MonsterEquipmentUI.SetActive(false); });
        }
        //몬스터 스탯 UI비활성화
        if (MonsterStatusUI.activeSelf == true)
        {
            MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, 125);
            MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.5f).OnComplete(() => { MonsterStatusUI.SetActive(false); });
        }
        //몬스터 버프 표시 비활성화
        foreach (BuffImageUIContainer obj in MonsterBuffUI)
        {
            obj.InitBuffImage();
        }
        //몬스터 다음 행동 표시 비활성화
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
        //이게 이제 승리 했을때 올라오는거고
        VictoryUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -1080);
        VictoryUI.SetActive(true);
        VictoryUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);
        VictoryText.text = "획득 경험치 : " + RewardExperience;
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
        //행동선택 UI비활성화
        if (PlayerActionSelectionBattleUI.activeSelf == true)
        {
            PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.5f).OnComplete(() => { PlayerActionSelectionBattleUI.SetActive(false); });
        }
        //몬스터 턴 버튼 UI 비활성화
        if (MonsterActionSelectionBattleUI.activeSelf == true)
        {
            MonsterActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(-1080, 0.5f).OnComplete(() => { MonsterActionSelectionBattleUI.SetActive(false); });
        }
        //턴 표시 UI비활성화
        if (DisplayTurnUI.activeSelf == true)
        {
            DisplayTurnUIImages[0].transform.parent.gameObject.GetComponent<RectTransform>().DOKill();
            DisplayTurnUI.GetComponent<CanvasGroup>().alpha = 1;
            DisplayTurnUI.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => { DisplayTurnUI.SetActive(false); });
        }
        //플래이어 버프 표시 비활성화
        PlayerBuffUI.InitBuffImage();
        //몬스터 선택 UI비활성화
        SelectionArrow.SetActive(false);
        //몬스터 장비 UI비활성화
        if (MonsterEquipmentUI.activeSelf == true)
        {
            MonsterEquipmentUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -100);
            MonsterEquipmentUI.GetComponent<RectTransform>().DOAnchorPosY(100, 0.5f).OnComplete(() => { MonsterEquipmentUI.SetActive(false); });
        }
        //몬스터 스탯 UI비활성화
        if (MonsterStatusUI.activeSelf == true)
        {
            MonsterStatusUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-390, 125);
            MonsterStatusUI.GetComponent<RectTransform>().DOAnchorPosY(-125, 0.5f).OnComplete(() => { MonsterStatusUI.SetActive(false); });
        }
        //몬스터 버프 표시 비활성화
        foreach (BuffImageUIContainer obj in MonsterBuffUI)
        {
            obj.InitBuffImage();
        }
        //몬스터 다음 행동 표시 비활성화
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

        DefeatEventTitle.text = "도달 최대 층수 (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
            ")\r\n일반 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillNormalMonster +
            ")\r\n엘리트 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillEliteMonster +
            ")\r\n남은 경험치 (" + PlayerInfo.GetPlayerStateInfo().Experience +
            ")\r\n선한 영향력 (" + PlayerInfo.GetPlayerStateInfo().GoodKarma + ")";
        DefeatEventAmount.text = (int)(JsonReadWriteManager.Instance.E_Info.PlayerReachFloor * 2000) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().KillNormalMonster * 500) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().KillEliteMonster * 700) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().Experience) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().GoodKarma * 200);
        /*
        DefeatEventTitle.text = "도달 최대 층수 (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
            ")\r\n준 피해 (" + PlayerInfo.GetPlayerStateInfo().GiveDamage +
            ")\r\n받은 피해 (" + PlayerInfo.GetPlayerStateInfo().ReceiveDamage +
            ")\r\n최대 피해 (" + PlayerInfo.GetPlayerStateInfo().MostPowerfulDamage +
            ")\r\n남은 경험치 (" +PlayerInfo.GetPlayerStateInfo().Experience +")";
        DefeatEventAmount.text = (int)(JsonReadWriteManager.Instance.E_Info.PlayerReachFloor / 5) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().GiveDamage / 1000) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().ReceiveDamage / 500) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().MostPowerfulDamage / 100) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().Experience / 2000);
        */
        EquipSuccessionText.text = "소지한 장비중 랜덤한 " + (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIPSUC") + "개의 장비가 계승됩니다.";
        DeafeatEarlyPoint.text = "기초 강화 포인트 : " + JsonReadWriteManager.Instance.E_Info.PlayerEarlyPoint;
    }

    public void WinGame(PlayerScript PlayerInfo)//이게 여기있어도 되는지는 몰겠는데 안좋으면 나중에 옮기지 뭐
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
        //특정 조건에 따라 승리라는 제목도 바뀔수 있지 않을까? 일단 그대로 진행
        WinEventTitle.text = "도달 최대 층수 (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
            ")\r\n일반 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillNormalMonster +
            ")\r\n엘리트 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillEliteMonster +
            ")\r\n남은 경험치 (" + PlayerInfo.GetPlayerStateInfo().Experience +
            ")\r\n선한 영향력 (" + PlayerInfo.GetPlayerStateInfo().GoodKarma + ")";
        WinEventAmount.text = (int)(JsonReadWriteManager.Instance.E_Info.PlayerReachFloor * 2000) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().KillNormalMonster * 500) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().KillEliteMonster * 700) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().Experience) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().GoodKarma * 200);
        /*
        WinEventTitle.text = "도달 최대 층수 (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
            ")\r\n준 피해 (" + PlayerInfo.GetPlayerStateInfo().GiveDamage +
            ")\r\n받은 피해 (" + PlayerInfo.GetPlayerStateInfo().ReceiveDamage +
            ")\r\n최대 피해 (" + PlayerInfo.GetPlayerStateInfo().MostPowerfulDamage +
            ")\r\n남은 경험치 (" + PlayerInfo.GetPlayerStateInfo().Experience + ")";
        WinEventAmount.text = (int)(JsonReadWriteManager.Instance.E_Info.PlayerReachFloor / 5) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().GiveDamage / 1000) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().ReceiveDamage / 500) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().MostPowerfulDamage / 100) +
            "\r\n" + (int)(PlayerInfo.GetPlayerStateInfo().Experience / 2000);
        */
        WinEquipSuccessionText.text = "소지한 장비중 랜덤한 " + (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIPSUC") + "개의 장비가 계승됩니다.";
        WinEarlyPoint.text = "기초 강화 포인트 : " + JsonReadWriteManager.Instance.E_Info.PlayerEarlyPoint;
    }

    public void ClickDefeatButton()
    {

    }
}
