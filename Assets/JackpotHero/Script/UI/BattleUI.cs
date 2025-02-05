using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public GameObject UI_StartOfBattle;
    [Header("ActionSelectionUI")]
    public GameObject PlayerActionSelectionBattleUI;
    [Header("MainBattleUI")]
    public GameObject MainBattleUI;
    public GameObject DisplayTurnUI;
    public Image[] DisplayTurnUIImages;
    public Sprite[] PlayerHeadSprites;
    [Header("PlayerSprites")]
    public GameObject[] HeroineSprites;
    //0. Normal # 1. ATK_Form # 2. DUR_Form # 3. RES_Form # 4. SPD_Form # 5. LUK_Form # 6. HP_Form # 7. STA_Form # 8. EXP_From # 9. EXPMG_Form # 10. EQUIP_Form
    [Header("MonsterSprites")]
    public string[] MonsterName;
    public Sprite[] MonsterHeadSprites;
    public GameObject[] MonsterSprites;
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

    protected Dictionary<string, GameObject> MonSpritesStorage = new Dictionary<string, GameObject>();
    protected Dictionary<string, Sprite> MonHeadSpriteStorage = new Dictionary<string, Sprite>();
    // Start is called before the first frame update
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
        MainBattleUI.SetActive(false);
        MonsterEquipmentUI.SetActive(false);
        MonsterStatusUI.SetActive(false);
        SelectionArrow.SetActive(false);
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
        PlayerActionSelectionBattleUI.GetComponent<Animator>().SetInteger("BattleSelectionState", 1);
    }

    public void SetBattleShieldNBuffUI(PlayerScript PlayerInfo, List<GameObject> ActiveMonsters)
    {
        //PlayerUI
        PlayerShield.SetActive(false);
        PlayerBuffUI.SetActive(false);
        if (PlayerInfo.GetPlayerStateInfo().ShieldAmount >= 1)
        {
            PlayerShield.SetActive(true);
            PlayerShieldText.text = ((int)PlayerInfo.GetPlayerStateInfo().ShieldAmount).ToString();
            PlayerShield.transform.position = PlayerInfo.ShieldUIPos.transform.position;
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

        for(int i = 0; i < ActiveMonsters.Count; i++)
        {
            Monster Mon = ActiveMonsters[i].GetComponent<Monster>();
            
            //SetShield
            if (ActiveMonsters[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint >= 1)
            {
                MonsterShield[i].SetActive(true);
                MonsterShieldText[i].text = ((int)ActiveMonsters[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentShieldPoint).ToString();
                MonsterShield[i].transform.position = ActiveMonsters[i].GetComponent<Monster>().MonsterShieldPos.transform.position;
            }
            
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

    public void SetBattleTurnUI(Queue<GameObject> TurnQue)
    {
        DisplayTurnUI.SetActive(true);
        Queue<GameObject> CopyQue = new Queue<GameObject>(TurnQue);
        for(int i = 0; i < DisplayTurnUIImages.Length; i++)
        {
            GameObject obj = CopyQue.Dequeue();
            if(obj.tag == "Player")
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

    public void DisplayMonsterDetailUI(Monster Mon)//CurrentTarget이 바뀌였을때 불러와짐 -> 그 화살표도 같이 이동
    {
        MonsterEquipmentUI.SetActive(true);//자동적으로 ActiveAnimation
        MonsterStatusUI.SetActive(true);//자동적으로 Activeanimation

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
    }

    public void ActiveMainBattleUI(int PlayerForm, Monster CurrentTarget)
    {
        if (MainBattleUI.activeSelf == true)
            return;

        if(PlayerActionSelectionBattleUI.activeSelf == true)
            PlayerActionSelectionBattleUI.GetComponent<Animator>().SetInteger("BattleSelectionState", 2);

        MainBattleUI.SetActive(true);
        //플레이어의 상태에 맞는 스프라이트 활성화
        foreach(GameObject Form in HeroineSprites)
        {
            Form.SetActive(false);
        }
        HeroineSprites[PlayerForm].SetActive(true);

        //몬스터의 종류에 맞는 스프라이트 활성화
        foreach(GameObject Mon in MonSpritesStorage.Values)
        {
            Mon.SetActive(false);
        }
        if (MonSpritesStorage.ContainsKey(CurrentTarget.MonsterName))//지금 여기서 오류가 나는 이유는 monster가 공격 주체자가 됬을때 Target이 없기 때문
        {
            MonSpritesStorage[CurrentTarget.MonsterName].SetActive(true);
        }

    }
}
