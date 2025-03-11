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

public class BattleUI : MonoBehaviour
{
    public GameObject UI_StartOfBattle;
    [Header("ActionSelectionUI")]
    public GameObject PlayerActionSelectionBattleUI;
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
    public GameObject[] ActionTypeObject;//4번
    public GameObject FinalCalculateObject;//4번
    public TextMeshProUGUI FinalCalculateText;//4번
    public GameObject ClickTextObject;//5번
    public GameObject PlayerZone;
    public GameObject MonsterZone;

    [Header("MonsterSprites")]
    public string[] MonsterName;
    public Sprite[] MonsterHeadSprites;
    public GameObject[] MonsterSprites;//8번
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
    public GameObject PlayerBuffUI;
    [Header("MonsterBuffNShieldNHP")]
    public GameObject[] MonsterShield;
    public TextMeshProUGUI[] MonsterShieldText;
    public GameObject[] MonsterBuffUI;
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
    public TextMeshProUGUI DefeatEventAmount;
    public TextMeshProUGUI DeafeatEarlyPoint;

    protected Dictionary<string, GameObject> MonSpritesStorage = new Dictionary<string, GameObject>();
    protected Dictionary<string, Sprite> MonHeadSpriteStorage = new Dictionary<string, Sprite>();
    // Start is called before the first frame update
    protected bool IsAnimateComplete = false;
    protected bool IsOpenCard = false;
    protected bool IsOpenAnimationComplete = false;
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
        foreach(GameObject obj in MonsterShield)
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
    }

    public void ActiveBattleUI()//전투를 시작할때 한번만
    {
        gameObject.SetActive(true);
        UI_StartOfBattle.SetActive(true);
    }

    public void ActiveBattleSelectionUI()//플레이어의 턴이 올때마다
    {
        if (PlayerActionSelectionBattleUI.activeSelf == true)
            return;

        MainBattleUI.SetActive(false);
        PlayerActionSelectionBattleUI.SetActive(true);
        PlayerActionSelectionBattleUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,- 1080);
        PlayerActionSelectionBattleUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
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

    public void SetBattleShieldNBuffUI(PlayerScript PlayerInfo, List<GameObject> ActiveMonsters)
    {
        //PlayerUI
        PlayerShield.SetActive(false);
        PlayerBuffUI.SetActive(false);
        //true일때는 안들어 와야하나?
        if (PlayerInfo.GetPlayerStateInfo().ShieldAmount > 0)//연속해서 턴있을때 방패가 안나타나는 이유는 바로 없어지기 때문임. 이것도 어떻게 되면 좋겠는데...
        {
            PlayerShieldText.text = (PlayerInfo.GetPlayerStateInfo().ShieldAmount).ToString("F1");
            PlayerShield.transform.position = PlayerInfo.ShieldUIPos.transform.position;
            Vector2 InitShieldPos = PlayerShield.GetComponent<RectTransform>().anchoredPosition;
            PlayerShield.GetComponent<RectTransform>().anchoredPosition = new Vector2(InitShieldPos.x, InitShieldPos.y - 50);
            PlayerShield.GetComponent<CanvasGroup>().alpha = 0;
            PlayerShield.SetActive(true);
            PlayerShield.GetComponent<RectTransform>().DOAnchorPos(InitShieldPos, 0.5f).SetEase(Ease.OutBack);
            PlayerShield.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }
        PlayerBuffUI.transform.position = PlayerInfo.BuffUIPos.transform.position;

        //MonsterUI
        foreach(GameObject obj in MonsterShield)
        {
            obj.SetActive(false);
        }
        foreach (Slider obj in MonsterHPSlider)
        {
            obj.gameObject.SetActive(false);
        }
        foreach (GameObject obj in MonsterBuffUI)
        {
            obj.SetActive(false);
        }
        for(int i = 0; i < MonsterAttackIcons.Length; i++)
        {
            MonsterAttackIcons[i].SetActive(false);
            MonsterDefenseIcons[i].SetActive(false);
            MonsterAnotherIcons[i].SetActive(false);
        }

        for(int i = 0; i < ActiveMonsters.Count; i++)
        {
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
            
            //SetShield
            if (Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint > 0)
            {
                MonsterShieldText[i].text = (Mon.GetMonsterCurrentStatus().MonsterCurrentShieldPoint).ToString("F1");
                MonsterShield[i].transform.position = Mon.MonsterShieldPos.transform.position;
                Vector2 InitShieldPos = MonsterShield[i].GetComponent<RectTransform>().anchoredPosition;
                MonsterShield[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(InitShieldPos.x, InitShieldPos.y - 50);
                MonsterShield[i].GetComponent<CanvasGroup>().alpha = 0;
                MonsterShield[i].SetActive(true);
                MonsterShield[i].GetComponent<RectTransform>().DOAnchorPos(InitShieldPos, 0.5f).SetEase(Ease.OutBack);
                MonsterShield[i].GetComponent<CanvasGroup>().DOFade(1, 0.5f);
            }
            /*
            else
            {
                if (MonsterShield[i].activeSelf == true)
                {
                    Vector2 InitShieldPos = MonsterShield[i].GetComponent<RectTransform>().anchoredPosition;
                    MonsterShield[i].GetComponent<CanvasGroup>().alpha = 1;
                    MonsterShield[i].GetComponent<RectTransform>().DOAnchorPosY(InitShieldPos.y - 50, 0.5f);
                    MonsterShield[i].GetComponent<CanvasGroup>().DOFade(0, 0.5f);
                }
            }
            */
            
            //SetHp
            MonsterHPSlider[i].gameObject.SetActive(true);
            Vector2 MonsterHpSliderSize = MonsterHPSlider[i].GetComponent<RectTransform>().sizeDelta;
            MonsterHpSliderSize.x = Mon.HpsliderWidth;
            MonsterHPSlider[i].GetComponent<RectTransform>().sizeDelta = MonsterHpSliderSize;
            MonsterHPSlider[i].value = Mon.GetMonsterCurrentStatus().MonsterCurrentHP / Mon.GetMonsterCurrentStatus().MonsterMaxHP;
            MosnterHpText[i].text = ((int)Mon.GetMonsterCurrentStatus().MonsterCurrentHP).ToString() + " / " + ((int)Mon.GetMonsterCurrentStatus().MonsterMaxHP).ToString();
            MonsterHPSlider[i].transform.position = Mon.HpSliderPos.transform.position;

            //SetBuff
            /*
            MonsterBuffUI[i].SetActive(true);
            Vector2 BuffPos = Mon.HpSliderPos.transform.position;
            BuffPos.y += Mon.BuffPosUpperHp;
            MonsterBuffUI[i].transform.position = BuffPos;
            */
        }
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
                if (!MonHeadSpriteStorage.ContainsKey(obj.GetComponent<Monster>().MonsterName))//없다면 패스
                    continue;

                DisplayTurnUIImages[i].sprite = MonHeadSpriteStorage[obj.GetComponent<Monster>().MonsterName];
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

    public void ActiveMainBattleUI(int PlayerForm, Monster CurrentTarget, string ActionString, BattleResultStates BattleResult, int CurrentBattleState, Action CallBack)//메인 전투 시작
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

        //1번 활성화
        BaseAmountObject.SetActive(true);
        BaseAmountObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 350);
        BaseAmountObject.GetComponent<RectTransform>().localScale = Vector2.one;
        BaseAmountText.text = "0";

        //2번 활성화
        MagnificationObject.SetActive(true);
        MagnificationObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(400, 350);
        MagnificationObject.GetComponent<RectTransform>().localScale = Vector2.one;
        MagnificationText.text = "1.00";

        //일단 3번들 다 비활성화 //밑의 코루틴에서 해결할듯
        BaseAmountCard.SetActive(false);//3번
        BaseAmountCardTitleText.text = "";//3번
        BaseAmountCardDetailText.text = "";//3번
        MagnificationCard.SetActive(false);//3번

        //0. Attack, 1. Defense, 2. Rest, 3. Another
        //행동 상태에 맞는 모양 활성화 //4번 활성화
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

        //플레이어의 상태에 맞는 스프라이트 활성화//7번 활성화
        foreach(GameObject Form in HeroineSprites)
        {
            Form.SetActive(false);
        }
        HeroineSprites[PlayerForm].SetActive(true);

        //몬스터의 종류에 맞는 스프라이트 활성화//8번 활성화
        //플레이어의 턴일때 휴식 혹은 방어일때는 안나타나게
        if(!(CurrentBattleState == (int)EBattleStates.PlayerTurn && (ActionString == "Defense" || ActionString == "Rest")))
        {
            foreach (GameObject Mon in MonSpritesStorage.Values)
            {
                Mon.SetActive(false);
            }
            if (MonSpritesStorage.ContainsKey(CurrentTarget.MonsterName))//지금 여기서 오류가 나는 이유는 monster가 공격 주체자가 됬을때 Target이 없기 때문
            {
                MonSpritesStorage[CurrentTarget.MonsterName].SetActive(true);
            }
        }
        

        StartCoroutine(ProgressMainBattle_UI(ActionString, BattleResult, CurrentBattleState, CallBack));
        //DoTween.OnComplete로 하면 코루틴으로 안하고 함수들키리 이벤트성을 연결가능하지 않나? 그게 더 안좋으려나?
    }
    IEnumerator ProgressMainBattle_UI(string ActionString, BattleResultStates BattleResult, int CurrentBattleState, Action CallBack)
    {
        IsOpenCard = false;
        IsOpenAnimationComplete = false;
        //우선 기초 수치(현재 스텟)카드를 나타나게 한다.
        BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
        BaseAmountCard.SetActive(true);//3번
        ClickTextObject.SetActive(true);
        ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        switch (ActionString)
        {
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
            case "Another":
                BaseAmountCardTitleText.text = "특수 행동";
                BaseAmountCardDetailText.text = "";
                //특수 행동에 대한 설명?
                break;
        }
        //첫번째 카드를 클릭하고 애니메이션이 끝날때까지 대기
        IsAnimateComplete = false;
        while (true)
        {
            yield return null;
            if (IsAnimateComplete == true)
            {
                //애니메이션이 끝나면 1번의 수 증가, 빠져나요기
                BaseAmountText.text = ((int)BattleResult.BaseAmount).ToString();
                BaseAmountCard.SetActive(false);
                break;
            }
        }
        //추가 기초 수치가 존재한다면 다시 활성화
        if (BattleResult.BaseAmountPlus >= 1)
        {
            IsAnimateComplete = false;
            BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
            BaseAmountCard.SetActive(true);
            ClickTextObject.SetActive(true);
            ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
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
                BaseAmountText.text = ((int)(BattleResult.BaseAmount + BattleResult.BaseAmountPlus)).ToString();
                BaseAmountCard.SetActive(false);
                break;
            }
        }

        //2번으로 향할 3번 활성화
        //이걸 BattleResult.Count만큼 반복 시켜야함
        float TotalMagnification = 1f;
        int RepeatCount = 0;
        while(RepeatCount < BattleResult.ResultMagnification.Count)
        {
            yield return null;
            MagnificationCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            MagnificationCard.GetComponent<RectTransform>().localScale = Vector2.one;
            MagnificationCard.SetActive(true);
            MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(-1);//?카드로 초기화
            IsOpenCard = false;
            IsOpenAnimationComplete = false;
            IsAnimateComplete = false;
            ClickTextObject.SetActive(true);
            ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            //카드 공개의 50프로까지 진행될때까지 대기
            while (true)
            {
                yield return null;
                if (IsOpenCard == true)
                {
                    //애니메이션이 50프로 까지 진행되면 카드에 결과 출력
                    MagnificationCard.GetComponent<Image>().sprite = EquipmentInfoManager.Instance.GetEquipmentSlotSprite(BattleResult.ResultMagnification[RepeatCount]);//0번째 결과 출력
                    MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).OnComplete(() => { IsOpenAnimationComplete = true; });
                    break;
                }
            }

            //숫자 카드가 다 나타날때 까지 대기
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

            //카드 클릭 애니메이션이 진행될때까지 대기
            while (true)
            {
                yield return null;
                if (IsAnimateComplete == true)
                {
                    //카드 클릭 애니메이션 재생이 끝나면 비활성화 후 결과값을 출력
                    MagnificationCard.SetActive(false);
                    TotalMagnification *= BattleResult.ResultMagnification[RepeatCount];
                    MagnificationText.text = TotalMagnification.ToString("F2");
                    RepeatCount++;
                    break;
                }
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
            if(IsAnimateComplete == true)
            {
                IsAnimateComplete = false;
                BaseAmountObject.SetActive(false);
                MagnificationObject.SetActive(false);
                FinalCalculateText.gameObject.SetActive(true);
                FinalCalculateText.gameObject.GetComponent<RectTransform>().localScale = Vector2.zero;
                FinalCalculateText.gameObject.GetComponent<RectTransform>().DOScale(Vector2.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() => { IsAnimateComplete = true; });
                FinalCalculateText.text = ((int)(BattleResult.FinalResultAmount - BattleResult.FinalResultAmountPlus)).ToString();
                break;
            }
        }

        //FinalCalculateObject가 끝날때까지 대기한다.
        while(true)
        {
            yield return null;
            if (IsAnimateComplete == true)
                break;
        }
        //FinalResultAmountPlus가 1보다 크다면 AmountCard를 다시 활성화 한다.
        if(BattleResult.FinalResultAmountPlus >= 1)
        {
            IsAnimateComplete = false;
            BaseAmountCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            BaseAmountCard.GetComponent<RectTransform>().localScale = Vector2.one;
            BaseAmountCard.SetActive(true);

            ClickTextObject.SetActive(true);
            ClickTextObject.GetComponent<RectTransform>().DOAnchorPosY(-240, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

            BaseAmountCardTitleText.text = "추가 최종 수치";
            BaseAmountCardDetailText.text = ((int)BattleResult.FinalResultAmountPlus).ToString();
        }

        while (BaseAmountCard.activeSelf == true)
        {
            yield return null;
            if (IsAnimateComplete)
            {
                //애니메이션이 끝나면 1번의 수 증가, 빠져나요기//숫자가 한번에 변하는것이 아닌 조금씩 변화하면 더 좋을것 같기도 함
                FinalCalculateText.text = ((int)BattleResult.FinalResultAmount).ToString();
                BaseAmountCard.SetActive(false);
                break;
            }
        }
        //약간 기다린다
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
        //약간 기다린다
        yield return new WaitForSeconds(0.3f);
        IsAnimateComplete = false;
        //이거에 대한 예외사항은 나중에 계속 늘어날듯?
        if (CurrentBattleState == (int)EBattleStates.PlayerTurn && ActionString == "Attack")//플레이어일 경우 공격일때만
        {
            PlayerZone.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-350, 0), 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { IsAnimateComplete = true; });
        }
        else if (CurrentBattleState == (int)EBattleStates.MonsterTurn && (ActionString == "Attack" || ActionString == "Another"))//몬스터일 경우 공격 + 특수 행동일때만
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
    }

    public void ClickMagnificationCard()
    {
        if(IsOpenCard == false)//아직 안열였다면//IsOpenCard를 true로 함 -> 코루틴에서 값에 맞는 카드로 바뀜
        {
            MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).OnComplete(() => { IsOpenCard = true; });
        }
        else if(IsOpenCard == true && IsOpenAnimationComplete == true && IsAnimateComplete == false)//카드가 공개 되고, 공개 애니메이션이 끝났을때 클릭하면
        {
            MagnificationCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(400, 350), 0.5f);
            MagnificationCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);
            MagnificationCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() => { IsAnimateComplete = true; });
            ClickTextObject.SetActive(false);
        }
    }

    public void VictoryBattle(int RewardExperience)//승리했을때
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
        //이게 이제 승리 했을때 올라오는거고
        VictoryUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, -1080);
        VictoryUI.SetActive(true);
        VictoryUI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.4f).SetEase(Ease.OutBack);
        VictoryText.text = "획득 경험치 : " + RewardExperience;
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
        DeafeatEarlyPoint.text = "기초 강화 포인트 : " + JsonReadWriteManager.Instance.E_Info.PlayerEarlyPoint;
    }

    public void ClickDefeatButton()
    {

    }
}
