using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class PlayerEquipMgUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public PlayerManager PlayerMgr;
    [Header("EquipManagement")]
    public GameObject PlayerEquip;
    public GameObject EquipInventory;
    public GameObject EquipDetailInfo_Equip;
    public GameObject EquipDetailInfo_Inven;
    public GameObject EquipGambling;
    [Header("EquipMg_PlayerEquip")]
    public GameObject[] PlayerEquipSlots;
    public Image[] PlayerEquipImages;
    public TextMeshProUGUI[] PlayerEquipTierText;
    [Header("EquipMg_EquipInventory")]
    public GameObject[] InventorySlots;
    public Image[] InventorySlotsImage;
    public GameObject[] LockObjects;
    public TextMeshProUGUI[] InventorySlotTierTexts;
    [Header("EquipMg_EquipDetailInfo_Equip")]
    public Image EquipDetail_EquipImage;
    public TextMeshProUGUI EquipDetail_EquipTierText;
    public TextMeshProUGUI EquipDetail_EquipNameText;
    public TextMeshProUGUI EquipDetail_EquipSTAText;
    public TextMeshProUGUI EquipDetail_EquipSTRText;
    public TextMeshProUGUI EquipDetail_EquipDURText;
    public TextMeshProUGUI EquipDetail_EquipRESText;
    public TextMeshProUGUI EquipDetail_EquipSPDText;
    public TextMeshProUGUI EquipDetail_EquipLUKText;
    public GameObject[] EquipDetail_EquipCardContainer;
    public TextMeshProUGUI EquipDetail_EquipDetailText;
    [Header("EquipMg_EquipDetailInfo_Inven")]
    public Image EquipDetail_InvenImage;
    public TextMeshProUGUI EquipDetail_InvenTierText;
    public TextMeshProUGUI EquipDetail_InvenNameText;
    public TextMeshProUGUI EquipDetail_InvenSTAText;
    public TextMeshProUGUI EquipDetail_InvenSTRText;
    public TextMeshProUGUI EquipDetail_InvenDURText;
    public TextMeshProUGUI EquipDetail_InvenRESText;
    public TextMeshProUGUI EquipDetail_InvenSPDText;
    public TextMeshProUGUI EquipDetail_InvenLUKText;
    public GameObject[] EquipDetail_InvenCardContainer;
    public TextMeshProUGUI EquipDetail_InvenDetailText;
    [Header("EqupMg_EquipGambling")]
    public TextMeshProUGUI EXPAmountText;
    public Button EquipGamblingButton;
    public TextMeshProUGUI EquipGamblingButtonText;
    public Button EquipGamblingLevelUpButton;
    public TextMeshProUGUI EquipGamblingLevelUPButtonText;
    public TextMeshProUGUI[] EquipGamblingPercentTexts;
    [Header("EquipMg_MouseFollowImage")]
    public Image MouseFollowImage;
    [Header("EquipMg_EquipGacha")]
    public GameObject EquipGachaObject;
    public GameObject EquipGachaEquipmentObject;
    public GameObject EquipGachaCapsule;
    public GameObject ClickButton;
    public GameObject GetEquipClickButton;
    public Image EquipGachaResultImage;
    public GameObject LightStorage;
    public GameObject[] EquipGachaLight;
    public Image EquipGachaTrianglePlate;
    public GameObject EquipGachaTriangleBlindObject;
    public GameObject EquipGachaIcon_TierGem;
    public GameObject EquipGachaIcon_StateType;
    public GameObject EquipGachaIcon_EquipType;
    public GameObject EquipGachaIcon_MultiType;
    public GameObject FinalEquipGachaCapsule;
    public GameObject FinalEquipGachaButton;
    public Image FinalEquipImage;
    [Header("EquipGacha_SelectCard")]
    public GameObject GachaCardSelectObject;
    public TextMeshProUGUI CardSelectTitle;
    public GameObject GachaCardStorage;
    public GameObject[] GachaSelectCards;
    public GameObject GachaCardHighligh;
    public GameObject GachaVirtualCard;
    public GameObject GachaConfirmButton;
    public GameObject GachaAgainButton;
    public TextMeshProUGUI RemainGachaText;
    [Header("EquipGacha_Sprites")]
    public Sprite[] GachaTrianglePlateSprites;
    public Sprite[] GachaCardSprites;
    public Sprite[] GachaTierGemSprites;
    public Sprite[] GachaIconSprites;
    [Header("Equip_EquipBuffDetail")]
    public GameObject EquipBuffDetailExplainObject;//설명 나올 오브젝트
    public TextMeshProUGUI EquipBuffDetailExplainTitleText;//설명 제목
    public TextMeshProUGUI EquipBuffDetailExplainDetailText;//설명 상세
    [Header("Inven_EquipBuffDetail")]
    public GameObject InvenBuffDetailExplainObject;//설명 나올 오브젝트
    public TextMeshProUGUI InvenBuffDetailExplainTitleText;//설명 제목
    public TextMeshProUGUI InvenBuffDetailExplainDetailText;//설명 상세

    protected Color InventoryActiveColor = new Color(0.28f, 0.19f, 0.1f, 1f);
    protected Color InventoryUnActiveColor = new Color(0.78f, 0.78f, 0.78f, 0.5f);

    protected Color[] GachaTierLightColor = new Color[]
    {
        new Color(1f,1f,1f),//255,255,255
        new Color(0f,0.84f,1f),//0,214,255
        new Color(0.87f,0f,1f),//221,0,255
        new Color(1,0.92f,0f),//255,234,0
        new Color(0.24f,1f,0f),//61,255,0
        new Color(1f,0.71f,0.11f)//255,181,28
    };

    protected Vector3[] GachaLightRot = new Vector3[] 
    {
        new Vector3(0,0,0),
        new Vector3(0,0,-60),
        new Vector3(0,0,-120),
        new Vector3(0,0,-180),
        new Vector3(0,0,-240),
        new Vector3(0,0,-300)
    };

    //플레이어 장비창 좌표들
    protected Vector2 PlayerEquipInitPos = new Vector2(-1460, 740);
    protected Vector2 PlayerEquipTargetPos = new Vector2(-460,340);
    //플레이어 장비 상세 정보 좌표들
    protected Vector2 EquipedEquipDetailInitPos = new Vector2(-1210, -730);
    protected Vector2 EquipedEquipDetailTargetPos = new Vector2(-710,-50);
    //플레이어가 클릭한 장비 상세 정보 좌표들
    protected Vector2 ClickedInvenDetailInitPos = new Vector2(-710,-730);
    protected Vector2 ClickedInvenDetailTargetPos = new Vector2(-210,-50);
    //인벤토리 좌표들
    protected Vector2 InvenInitPos = new Vector2(1420, 890);
    protected Vector2 InvenTargetPos = new Vector2(500, 190);
    //갓챠창 좌표들
    protected Vector2 GamblingInitPos = new Vector2(1420, -730);
    protected Vector2 GamblingTargetPos = new Vector2(500, -350);
    //요 위의 좌표들에 정보 입력

    //갓챠창 enum
    protected enum ETriangleState
    {
        ZeroLightOn,
        OneLightOn,
        TwoLightOn,
        ThreeLightOn,
        FourLightOn,
    }
    protected enum EGachaTierGem
    {
        OneTierGem,
        TwoTierGem,
        ThreeTierGem,
        FourTierGem,
        FiveTierGem,
        SixTierGem
    }
    protected enum EGachaIconNCard
    {
        StateType_STR,
        StateType_DUR,
        StateType_RES,
        StateType_SPD,
        StateType_LUK,
        StateType_HP,
        StateType_STA,
        StateType_Normal,
        EquipType_Weapon,
        EquipType_Armor,
        EquipType_Helmet,
        EquipType_Shoes,
        EquipType_Acc,
        MultyType_Non,
        MultyType_01,
        MultyType_02,
        MultyType_03,
        OnlyForCard_BackCard
    }

    protected enum EGachaPhase
    {
        GemPhase,
        StatePhase,
        TypePhase,
        MultiplePhase,
        EndPhase
    }

    protected const float GachaLightZVariation = 10f;

    protected bool IsClickedInventorySlot = false;

    protected int PlayerEquipIndex = -1;

    protected int CurrentClickedSlotIndex = 0;
    protected int CurrentBringItemCode = 0;
    protected int DropDownSlotIndex = 0;
    protected int DropDownItemCode = 0;

    protected int GachaTierNum = 0;
    protected int GachaStateTypeNum = 0;
    protected int GachaEquipTypeNum = 0;
    protected int GachaMultipleTypeNum = 0;
    protected int GachaResultEquipCode = 0;
    protected int CurrentGachaPhase = 0;
    protected List<int> RemainCardResult = new List<int>();
    protected int RemainGachaCount = 0;
    protected int SelectedGachaCardNum;

    private int EquipCurrentLinkIndex = -1;
    private int InvenCurrentLinkIndex = -1;
    protected enum EPlayerEquip
    {
        Helmet,
        Armor,
        Boots,
        Weapon,
        Accessories
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerEquip.SetActive(false);
        EquipInventory.SetActive(false);
        EquipDetailInfo_Equip.SetActive(false);
        EquipDetailInfo_Inven.SetActive(false);
        EquipGambling.SetActive(false);
        MouseFollowImage.gameObject.SetActive(false);
        EquipGachaObject.SetActive(false);
        if (EquipBuffDetailExplainObject != null)
        {
            EquipBuffDetailExplainObject.SetActive(false);
        }
        if (InvenBuffDetailExplainObject != null)
        {
            InvenBuffDetailExplainObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        //EquipDetail_EquipDetailText
        //EquipDetail_InvenDetailText
        
        // 링크 인덱스를 찾음
        int EquiplinkIndex = TMP_TextUtilities.FindIntersectingLink(EquipDetail_EquipDetailText, mousePosition, Camera.main);
        int InvenLinkIndex = TMP_TextUtilities.FindIntersectingLink(EquipDetail_InvenDetailText, mousePosition, Camera.main);

        if (EquiplinkIndex != EquipCurrentLinkIndex)
        {
            // 기존 링크에서 마우스가 벗어났을 때
            if (EquipCurrentLinkIndex != -1)
            {
                string oldID = EquipDetail_EquipDetailText.textInfo.linkInfo[EquipCurrentLinkIndex].GetLinkID();
                OnLinkExit(oldID, 0);
            }

            // 새로운 링크에 마우스가 올라갔을 때
            if (EquiplinkIndex != -1)
            {
                string newID = EquipDetail_EquipDetailText.textInfo.linkInfo[EquiplinkIndex].GetLinkID();
                OnLinkEnter(newID, 0);
            }
            EquipCurrentLinkIndex = EquiplinkIndex;
        }

        if (InvenLinkIndex != InvenCurrentLinkIndex)
        {
            // 기존 링크에서 마우스가 벗어났을 때
            if (InvenCurrentLinkIndex != -1)
            {
                string oldID = EquipDetail_InvenDetailText.textInfo.linkInfo[InvenCurrentLinkIndex].GetLinkID();
                OnLinkExit(oldID, 1);
            }

            // 새로운 링크에 마우스가 올라갔을 때
            if (InvenLinkIndex != -1)
            {
                string newID = EquipDetail_InvenDetailText.textInfo.linkInfo[InvenLinkIndex].GetLinkID();
                OnLinkEnter(newID, 1);
            }
            InvenCurrentLinkIndex = InvenLinkIndex;
        }

    }
    private void OnLinkEnter(string id, int DetailNum)
    {
        /*
         * DetailExplainText.text = "도달 최대 층수 (" + JsonReadWriteManager.Instance.E_Info.PlayerReachFloor +
    ")\r\n일반 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillNormalMonster +
    ")\r\n엘리트 몬스터 (" + PlayerInfo.GetPlayerStateInfo().KillEliteMonster +
    ")\r\n남은 경험치 (" + PlayerInfo.GetPlayerStateInfo().Experience +
    ")\r\n선한 영향력 (" + PlayerInfo.GetPlayerStateInfo().GoodKarma + ")";
         */
        if (DetailNum == 0)
            EquipBuffDetailExplainObject.SetActive(true);
        else
            InvenBuffDetailExplainObject.SetActive(true);

        switch (id)
        {
            case "STRWE":
                if(DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.DefenseDebuff).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.DefenseDebuff).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.DefenseDebuff).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.DefenseDebuff).BuffDetail;
                }
                break;
            case "STRAR":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffDetail;
                }
                break;
            case "STRHE":
            case "ForestBracelet02":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Recharge).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Recharge).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Recharge).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Recharge).BuffDetail;
                }
                break;
            case "STRBO":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffDetail;
                }
                break;
            case "STRAC":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughFist).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughFist).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughFist).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughFist).BuffDetail;
                }
                break;
            case "DURWE":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.AttackDebuff).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.AttackDebuff).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.AttackDebuff).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.AttackDebuff).BuffDetail;
                }
                break;
            case "DURAR":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnbreakableArmor).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnbreakableArmor).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnbreakableArmor).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnbreakableArmor).BuffDetail;
                }
                break;
            case "DURHE":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffDetail;
                }
                break;
            case "DURAC":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughSkin).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughSkin).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughSkin).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ToughSkin).BuffDetail;
                }
                break;
            case "RESWE01":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Poison).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Poison).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Poison).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Poison).BuffDetail;
                }
                break;
            case "RESWE02":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Burn).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Burn).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Burn).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Burn).BuffDetail;
                }
                break;
            case "RESAR":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Misfortune).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Misfortune).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Misfortune).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Misfortune).BuffDetail;
                }
                break;
            case "RESHE":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffDetail;
                }
                break;
            case "RESBO":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Slow).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Slow).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Slow).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Slow).BuffDetail;
                }
                break;
            case "RESAC":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CurseOfDeath).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CurseOfDeath).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CurseOfDeath).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CurseOfDeath).BuffDetail;
                }
                break;
            case "SPDWE":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ChainAttack).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ChainAttack).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ChainAttack).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.ChainAttack).BuffDetail;
                }
                break;
            case "SPDAR":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.RegenArmor).BuffDetail;
                }
                break;
            case "SPDHE":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.OverCharge).BuffDetail;
                }
                break;
            case "SPDBO":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Haste).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Haste).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Haste).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Haste).BuffDetail;
                }
                break;
            case "SPDAC":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CorruptSerum).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CorruptSerum).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CorruptSerum).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.CorruptSerum).BuffDetail;
                }
                break;
            case "LUKBO":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Luck).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Luck).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Luck).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Luck).BuffDetail;
                }
                break;
            case "HPWE":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.LifeSteal).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.LifeSteal).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.LifeSteal).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.LifeSteal).BuffDetail;
                }
                break;
            case "HPAR":
            case "ForestBracelet01":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Regeneration).BuffDetail;
                }
                break;
            case "HPHE":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Charm).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Charm).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Charm).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Charm).BuffDetail;
                }
                break;
            case "HPBO":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.UnDead).BuffDetail;
                }
                break;
            case "HPAC":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.PowerOfDeath).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.PowerOfDeath).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.PowerOfDeath).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.PowerOfDeath).BuffDetail;
                }
                break;
            case "STABO":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffDetail;
                }
                break;
            case "STAAC":
                if (DetailNum == 0)
                {
                    EquipBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffName;
                    EquipBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffDetail;
                }
                else
                {
                    InvenBuffDetailExplainTitleText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffName;
                    InvenBuffDetailExplainDetailText.text = BuffInfoManager.Instance.GetBuffInfo((int)EBuffType.Petrification).BuffDetail;
                }
                break;
        }
        
        //Debug.Log($"마우스 올라감: {id}");
    }

    private void OnLinkExit(string id, int DetailNum)
    {
        if(DetailNum == 0)
            EquipBuffDetailExplainObject.SetActive(false);
        else 
            InvenBuffDetailExplainObject.SetActive(false);
        // 예: 원래 색상 복구, 툴팁 숨기기 등
        //Debug.Log($"마우스 벗어남: {id}");
    }

    public void ActivePlayerEquipMg()
    {
        if(PlayerEquip.activeSelf == true && EquipInventory.activeSelf == true && EquipDetailInfo_Equip.activeSelf == true &&
            EquipDetailInfo_Inven.activeSelf == true && EquipGambling.activeSelf == true)
        {
            return;//다 켜져 있으면 리턴 때리기
        }
        gameObject.SetActive(true);
        //SetPlayerEquip
        //다껏다가 해당되는거만 키기
        for(int i = 0; i < PlayerEquipImages.Length; i++)
        {
            PlayerEquipImages[i].gameObject.SetActive(false);
            PlayerEquipTierText[i].text = "";
        }
        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Helmet].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Helmet].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Helmet].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode);
        }
        if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Armor].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Armor].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Armor].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode);
        }
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Boots].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Boots].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Boots].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode);
        }
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Weapon].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Weapon].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Weapon].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode);
        }
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode != 0)
        {
            PlayerEquipImages[(int)EPlayerEquip.Accessories].gameObject.SetActive(true);
            PlayerEquipImages[(int)EPlayerEquip.Accessories].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode).EquipmentImage;
            PlayerEquipTierText[(int)EPlayerEquip.Accessories].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode);
        }
        PlayerEquip.GetComponent<RectTransform>().anchoredPosition = PlayerEquipInitPos;
        PlayerEquip.SetActive(true);
        PlayerEquip.GetComponent<RectTransform>().DOAnchorPos(PlayerEquipTargetPos, 0.5f);
        //SetEquipInventory
        SetInventory(true);
        //SetEquipDetailInfo_Equip//Detail의 초기상태는 비어있는 거임//후에 플레이어의 조작에 따라 정보 표시
        EquipDetail_EquipImage.gameObject.SetActive(false);
        EquipDetail_EquipTierText.text = "";
        EquipDetail_EquipNameText.text = "";
        EquipDetail_EquipSTAText.text = "";
        EquipDetail_EquipSTRText.text = "";
        EquipDetail_EquipDURText.text = "";
        EquipDetail_EquipRESText.text = "";
        EquipDetail_EquipSPDText.text = "";
        EquipDetail_EquipLUKText.text = "";
        foreach(GameObject Obj in EquipDetail_EquipCardContainer)
        {
            Obj.SetActive(false);
        }
        EquipDetail_EquipDetailText.text = "";
        EquipDetailInfo_Equip.GetComponent<RectTransform>().anchoredPosition = EquipedEquipDetailInitPos;
        EquipDetailInfo_Equip.SetActive(true);
        EquipDetailInfo_Equip.GetComponent<RectTransform>().DOAnchorPos(EquipedEquipDetailTargetPos, 0.5f);
        //SetEquipDetailInfo_Inven//Detail의 초기상태는 비어있는 거임//후에 플레이어의 조작에 따라 정보 표시
        EquipDetail_InvenImage.gameObject.SetActive(false);
        EquipDetail_InvenTierText.text = "";
        EquipDetail_InvenNameText.text = "";
        EquipDetail_InvenSTAText.text = "";
        EquipDetail_InvenSTRText.text = "";
        EquipDetail_InvenDURText.text = "";
        EquipDetail_InvenRESText.text = "";
        EquipDetail_InvenSPDText.text = "";
        EquipDetail_InvenLUKText.text = "";
        foreach (GameObject Obj in EquipDetail_InvenCardContainer)
        {
            Obj.SetActive(false);
        }
        EquipDetail_InvenDetailText.text = "";
        EquipDetailInfo_Inven.GetComponent<RectTransform>().anchoredPosition = ClickedInvenDetailInitPos;
        EquipDetailInfo_Inven.SetActive(true);
        EquipDetailInfo_Inven.GetComponent<RectTransform>().DOAnchorPos(ClickedInvenDetailTargetPos, 0.5f);
        //SetEquipGambling
        SetGambling(true);

    }

    public void InActivePlayerEquipMg()// 비활성화 됬을때 저장
    {
        if(PlayerEquip.activeSelf == true)
        {
            PlayerEquip.GetComponent<RectTransform>().anchoredPosition = PlayerEquipTargetPos;
            PlayerEquip.GetComponent<RectTransform>().DOAnchorPos(PlayerEquipInitPos, 0.5f).OnComplete(() => { PlayerEquip.SetActive(false); });
        }
        if(EquipInventory.activeSelf == true)
        {
            EquipInventory.GetComponent<RectTransform>().anchoredPosition = InvenTargetPos;
            EquipInventory.GetComponent<RectTransform>().DOAnchorPos(InvenInitPos, 0.5f).OnComplete(() => { EquipInventory.SetActive(false); });
        }
        if(EquipDetailInfo_Equip.activeSelf == true)
        {
            EquipDetailInfo_Equip.GetComponent<RectTransform>().anchoredPosition = EquipedEquipDetailTargetPos;
            EquipDetailInfo_Equip.GetComponent<RectTransform>().DOAnchorPos(EquipedEquipDetailInitPos, 0.5f).OnComplete(() => { EquipDetailInfo_Equip.SetActive(false); });
        }
        if(EquipDetailInfo_Inven.activeSelf == true)
        {
            EquipDetailInfo_Inven.GetComponent<RectTransform>().anchoredPosition = ClickedInvenDetailTargetPos;
            EquipDetailInfo_Inven.GetComponent<RectTransform>().DOAnchorPos(ClickedInvenDetailInitPos, 0.5f).OnComplete(() => { EquipDetailInfo_Inven.SetActive(false); });
        }
        if(EquipGambling.activeSelf == true)
        {
            EquipGambling.GetComponent<RectTransform>().anchoredPosition = GamblingTargetPos;
            EquipGambling.GetComponent<RectTransform>().DOAnchorPos(GamblingInitPos, 0.5f).OnComplete(() => { EquipGambling.SetActive(false); gameObject.SetActive(false); });
        }

        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    protected string GetTierText(int Code)
    {
           return ((Code / 1000) % 10).ToString() + "T";
    }

    protected void SetGambling(bool IsActive = false)//->이거는 무언가 행동 될때마다 계속 업데이트 해야할듯?
    {//일단은 장비를 집고 놨을때(OnPointerUp일때 한번해야함)(장비 뽑고 나서도 그렇고)
        EXPAmountText.text = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience.ToString();
        //뽑기 레벨업 버튼
        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel >= 10)
        {//만랩일 경우
            EquipGamblingLevelUpButton.interactable = false;
            EquipGamblingLevelUPButtonText.text = "Max Level";
        }
        else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 
            EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel))
        {//경험치가 부족할 경우
            EquipGamblingLevelUpButton.interactable = false;
            EquipGamblingLevelUPButtonText.text = "";
            if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                EquipGamblingLevelUPButtonText.text = "Not enough EXP\r\nRequired EXP : ";
            else if(JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                EquipGamblingLevelUPButtonText.text = "EXP不足\r\n必要EXP : ";
            else
                EquipGamblingLevelUPButtonText.text = "EXP 부족\r\n필요 EXP : ";

            if (EquipGamblingLevelUPButtonText.text != "")
            {
                EquipGamblingLevelUPButtonText.text += EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
            }
        }
        else//만랩보다 작을경우
        {
            EquipGamblingLevelUpButton.interactable = true;
            EquipGamblingLevelUPButtonText.text = "";
            if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                EquipGamblingLevelUPButtonText.text = "Equipment Gacha Enhancement\r\nEXP : ";
            else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                EquipGamblingLevelUPButtonText.text = "装備ガチャ強化\r\nEXP : ";
            else
                EquipGamblingLevelUPButtonText.text = "장비 뽑기 강화\r\nEXP : ";

            if (EquipGamblingLevelUPButtonText.text != "")
            {
                EquipGamblingLevelUPButtonText.text += EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
            }
        }

        //뽑기 버튼
        if(PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)
        {//인벤토리가 꽉찼다면
            EquipGamblingButton.interactable = false;
            EquipGamblingButtonText.text = "";
            if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                EquipGamblingButtonText.text = "Not enough\r\nInventory";
            else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                EquipGamblingButtonText.text = "インベントリ\r\n不足";
            else
                EquipGamblingButtonText.text = "인벤토리\r\n공간 부족";
        }
        else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().Experience < 
            EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel))
        {//경험치가 부족할경우
            EquipGamblingButton.interactable = false;
            EquipGamblingButtonText.text = "";
            if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                EquipGamblingButtonText.text = "Not enough EXP\r\nRequired EXP : ";
            else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                EquipGamblingButtonText.text = "EXP不足\r\n必要EXP : ";
            else
                EquipGamblingButtonText.text = "EXP 부족\r\n필요 EXP : ";

            if (EquipGamblingButtonText.text != "")
            {
                EquipGamblingButtonText.text += EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
            }
        }
        else
        {
            EquipGamblingButton.interactable = true;
            EquipGamblingButtonText.text = "";
            if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
                EquipGamblingButtonText.text = "Equipment Gacha\r\nEXP : ";
            else if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
                EquipGamblingButtonText.text = "装備ガチャ\r\nEXP : ";
            else
                EquipGamblingButtonText.text = "장비 뽑기\r\nEXP : ";

            if (EquipGamblingButtonText.text != "")
            {
                EquipGamblingButtonText.text += EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
            }
        }


        //EquipGamblingPercentTexts
        for (int i = 0; i < EquipGamblingPercentTexts.Length; i++)
        {
            EquipGamblingPercentTexts[i].text = EquipmentInfoManager.Instance.GetGamblingLevelPercent(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel, i) + "%";
        }

        if (IsActive == true)
        {
            EquipGambling.GetComponent<RectTransform>().anchoredPosition = GamblingInitPos;
            EquipGambling.SetActive(true);
            EquipGambling.GetComponent<RectTransform>().DOAnchorPos(GamblingTargetPos, 0.5f);
        }
    }

    protected void SetInventory(bool IsActive = false)
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            InventorySlots[i].GetComponent<Image>().color = InventoryUnActiveColor;
            InventorySlotsImage[i].gameObject.SetActive(false);
            LockObjects[i].SetActive(true);
            InventorySlotTierTexts[i].text = "";
        }

        int CanUseInventory = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");
        for (int i = 0; i < CanUseInventory; i++)
        {
            InventorySlots[i].GetComponent<Image>().color = InventoryActiveColor;
            LockObjects[i].SetActive(false);
            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//비어있지 않을때
            {
                //코드에 맞는 이미지를 넣음
                InventorySlotsImage[i].gameObject.SetActive(true);
                InventorySlotsImage[i].sprite =
                    EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]).EquipmentImage;
                InventorySlotTierTexts[i].text = GetTierText(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i]);
            }
        }
        if(IsActive == true)
        {
            EquipInventory.GetComponent<RectTransform>().anchoredPosition = InvenInitPos;
            EquipInventory.SetActive(true);
            EquipInventory.GetComponent<RectTransform>().DOAnchorPos(InvenTargetPos, 0.5f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerEquipIndex = -1;
        if (eventData.pointerEnter != null && eventData.pointerEnter.tag == "InventorySlot")//여기서 들어오는 놈이 몇번째 Slot인지 알아야함
        {
            Vector2 ClickedUIPos = eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition;
            for(int i = 0; i <PlayerEquipSlots.Length; i++)
            {
                if(Vector2.Distance(ClickedUIPos, PlayerEquipSlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                {
                    switch (i)
                    {
                        case (int)EPlayerEquip.Helmet:
                            if(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
                            break;
                        case (int)EPlayerEquip.Armor:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
                            break;
                        case (int)EPlayerEquip.Boots:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
                            break;
                        case (int)EPlayerEquip.Weapon:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
                            break;
                        case (int)EPlayerEquip.Accessories:
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode == 0)//비어있다면
                                return;
                            else
                                CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode;
                            break;
                    }
                    SoundManager.Instance.PlayUISFX("Item_PickUp");
                    PlayerEquipIndex = i;
                    IsClickedInventorySlot = true;

                    MouseFollowImage.gameObject.SetActive(true);
                    Debug.Log(CurrentBringItemCode);
                    MouseFollowImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                    MoveUI(eventData);
                    PlayerEquipImages[i].gameObject.SetActive(false);
                    PlayerEquipTierText[i].text = "";
                    DisplayEquipDetailInfo(true);
                    //장비칸 아이템 클릭
                    break;
                }
            }
            
            if(PlayerEquipIndex == -1)//만약 장비창중에 클릭한 슬롯이 없다면 인벤토리도 검사
            {
                //인벤토리 슬롯인지 검사
                for (int i = 0; i < InventorySlots.Length; i++)
                {
                    if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                    {
                        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0 && LockObjects[i].activeSelf == false)//비어있지 않을때//잠겨있지 않을때
                        {
                            SoundManager.Instance.PlayUISFX("Item_PickUp");
                            IsClickedInventorySlot = true;
                            //들고 있는 아이템의 정보 저장
                            CurrentClickedSlotIndex = i;
                            CurrentBringItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i];
                            //마우스를 따라다니는 이미지를 마우스 위치에 위치시키고 이미지를 바꿈
                            MouseFollowImage.gameObject.SetActive(true);
                            MouseFollowImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                            MoveUI(eventData);
                            //클릭한 슬롯의 이미지를 잠시 꺼둠
                            InventorySlotsImage[i].gameObject.SetActive(false);
                            //티어 텍스트도 잠시 꺼둠
                            InventorySlotTierTexts[i].text = "";
                        }
                        //인벤토리칸 아이템 클릭
                        DisplayEquipDetailInfo(false);
                        break;
                    }
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsClickedInventorySlot == true)
        {
            MoveUI(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsClickedInventorySlot == true && eventData.pointerEnter != null)
        {
            IsClickedInventorySlot = false;
            DropDownItemCode = 0;
            Vector2 ClickedUIPos = eventData.pointerEnter.GetComponent<RectTransform>().anchoredPosition;

            if (PlayerEquipIndex != -1)//장비 칸에서 클릭했을때//장비칸에서 인벤토리 OR 장비칸
            {
                if (eventData.pointerEnter.tag == "InventorySlot")//마우스를 놓은 곳이 슬롯일때
                {
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    bool IsEquipmentSlot = true;
                    //인벤토리 칸인지 검사
                    for (int i = 0; i < InventorySlots.Length; i++)
                    {
                        if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                        {
                            //여기에 한번이라도 들어갔다는것 = 마우스를 땐 슬롯이 장비 슬롯이 아님
                            IsEquipmentSlot = false;
                            if (LockObjects[i].activeSelf == true)//잠겨 있을때
                            {
                                //원래 위치로
                                PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                                PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                            }
                            else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//비어있지 않을떄//잠겨 있지 않으면서 비어있지 않을때
                            {
                                if((CurrentBringItemCode / 10) % 10 == 
                                    (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] / 10) % 10)//비어있지 않았을때 장비의 타입이 같다면
                                {//바꾸기
                                    //떨어뜨리는 곳의 index번호와 장비 코드를 저장
                                    DropDownSlotIndex = i;
                                    DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i];
                                    //떨어뜨린 곳의 슬롯에 집었던 장비 덮어 쓰기
                                    InventorySlotsImage[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                    InventorySlotTierTexts[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex] = CurrentBringItemCode;
                                    //집었던 슬롯에 떨어뜨린곳의 장비 넣기
                                    PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                                    PlayerEquipImages[PlayerEquipIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                                    PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(DropDownItemCode);
                                    switch(PlayerEquipIndex)
                                    {
                                        case (int)EPlayerEquip.Helmet:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = DropDownItemCode;
                                            break;
                                        case (int)EPlayerEquip.Armor:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = DropDownItemCode;
                                            break;
                                        case (int)EPlayerEquip.Boots:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = DropDownItemCode;
                                            break;
                                        case (int)EPlayerEquip.Weapon:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = DropDownItemCode;
                                            break;
                                        case (int)EPlayerEquip.Accessories:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = DropDownItemCode;
                                            break;
                                    }
                                }
                                else//같지 않다면
                                {//원래 위치로
                                    PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                                    PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                                }
                            }
                            else//비어있다면
                            {
                                //비어있는 곳에 넣고
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = CurrentBringItemCode;
                                InventorySlotsImage[i].gameObject.SetActive(true);
                                InventorySlotsImage[i].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                InventorySlotTierTexts[i].text = GetTierText(CurrentBringItemCode);
                                //장비 칸은 비우고
                                //이미 UI상으로는 비워져 있음
                                switch (PlayerEquipIndex)
                                {
                                    case (int)EPlayerEquip.Helmet:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = 0;
                                        break;
                                    case (int)EPlayerEquip.Armor:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = 0;
                                        break;
                                    case (int)EPlayerEquip.Boots:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = 0;
                                        break;
                                    case (int)EPlayerEquip.Weapon:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = 0;
                                        break;
                                    case (int)EPlayerEquip.Accessories:
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = 0;
                                        break;
                                }
                            }
                            break;
                        }
                    }

                    if(IsEquipmentSlot == true)//장비 슬롯이엿다면
                    {//원래 자리로
                        PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                        PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                    }
                }
                else if (eventData.pointerEnter.name == "TrashCan")//마우스를 놓은 곳이 쓰레기 통일때
                {
                    SoundManager.Instance.PlayUISFX("Item_Remove");
                    //비우기
                    switch (PlayerEquipIndex)
                    {
                        case (int)EPlayerEquip.Helmet:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = 0;
                            break;
                        case (int)EPlayerEquip.Armor:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = 0;
                            break;
                        case (int)EPlayerEquip.Boots:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = 0;
                            break;
                        case (int)EPlayerEquip.Weapon:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = 0;
                            break;
                        case (int)EPlayerEquip.Accessories:
                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = 0;
                            break;
                    }
                }
                else//뭣도 아닐때
                {//원래 자리로
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                    PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                }
            }
            else//클릭(PointDown)한 슬롯이 인벤토리 슬롯일때//인벤토리에서 장비칸 OR 인벤토리로
            {
                if (eventData.pointerEnter.tag == "InventorySlot")//마우스를 놓은 곳이 슬롯일때
                {
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    bool IsEquipmentSlot = true;
                    //인벤토리 칸인지 검사
                    for (int i = 0; i < InventorySlots.Length; i++)
                    {
                        if (Vector2.Distance(ClickedUIPos, InventorySlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                        {
                            //여기에 한번이라도 들어갔다는것 = 마우스를 땐 슬롯이 장비 슬롯이 아님
                            IsEquipmentSlot = false;
                            if (LockObjects[i].activeSelf == true)//잠겨 있을때
                            {
                                //원래 위치로
                                InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                            }
                            else if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] != 0)//비어있지 않을떄//잠겨 있지 않으면서 비어있지 않을때
                            {
                                //떨어뜨리는 곳의 번호와 장비코드를 저장
                                DropDownSlotIndex = i;
                                DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex];
                                //떨어뜨린 곳의 슬롯에 집었던 장비를 덮어 쓰기
                                InventorySlotsImage[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                InventorySlotTierTexts[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[DropDownSlotIndex] = CurrentBringItemCode;
                                //장비를 집었던 슬롯에 떨어뜨린 곳의 장비를 덮어 쓰기
                                InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                InventorySlotsImage[CurrentClickedSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                                InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(DropDownItemCode);
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = DropDownItemCode;
                            }
                            else//비어있다면
                            {
                                //비어있는 곳에 넣고
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] = CurrentBringItemCode;
                                InventorySlotsImage[i].gameObject.SetActive(true);
                                InventorySlotsImage[i].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                InventorySlotTierTexts[i].text = GetTierText(CurrentBringItemCode);
                                //원래 칸은 비우고
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                            }
                            break;
                        }
                    }

                    if (IsEquipmentSlot == true)//장비 슬롯이엿다면
                    {
                        for(int i = 0; i < PlayerEquipSlots.Length; i++)
                        {
                            if(Vector2.Distance(ClickedUIPos, PlayerEquipSlots[i].GetComponent<RectTransform>().anchoredPosition) <= 10)
                            {//장비칸에서 클릭이 때졌을때
                                DropDownSlotIndex = i;
                                switch (i)
                                {
                                    case (int)EPlayerEquip.Helmet:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
                                        break;
                                    case (int)EPlayerEquip.Armor:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
                                        break;
                                    case (int)EPlayerEquip.Boots:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
                                        break;
                                    case (int)EPlayerEquip.Weapon:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
                                        break;
                                    case (int)EPlayerEquip.Accessories:
                                        DropDownItemCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode;
                                        break;
                                }

                                if(DropDownItemCode == 0)//비어있다면//맞게 넣어야 하는디.....
                                {
                                    if ((CurrentBringItemCode / 10) % 10 == (int)EEquipType.TypeWeapon && DropDownSlotIndex == (int)EPlayerEquip.Weapon)
                                    {//CurrentBringItemCode의 앞자리가 1 : 무기 -> 1이라면 인덱스 3번 칸일때 ok -> 넣기
                                        //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if((CurrentBringItemCode / 10) % 10 == (int)EEquipType.TypeArmor && DropDownSlotIndex == (int)EPlayerEquip.Armor)
                                    {//CurrentBringItemCode의 앞자리가 2 : 갑옷 -> 2라면 인덱스 1번 칸일때 ok -> 넣기
                                        //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if((CurrentBringItemCode / 10) % 10 == (int)EEquipType.TypeHelmet && DropDownSlotIndex == (int)EPlayerEquip.Helmet)
                                    {//CurrentBringItemCode의 앞자리가 3 : 투구 -> 3이라면 인덱스 0번 칸일때 ok -> 넣기
                                        //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if((CurrentBringItemCode / 10) % 10 == (int)EEquipType.TypeBoots && DropDownSlotIndex == (int)EPlayerEquip.Boots)
                                    {//CurrentBringItemCode의 앞자리가 4 : 신발 -> 4라면 인덱스 2번 칸일때 ok -> 넣기
                                        //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else if((CurrentBringItemCode / 10) % 10 == (int)EEquipType.TypeAcc && DropDownSlotIndex == (int)EPlayerEquip.Accessories)
                                    {//CurrentBringItemCode의 앞자리가 5 : 장신구 -> 5라면 인덱스 4번 칸 일때 ok -> 넣기
                                     //넣기
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = CurrentBringItemCode;
                                        PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                        PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                        PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                        //원래 칸은 비우고
                                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                                    }
                                    else
                                    {//위에꺼에 해당하지 않는다면 원래 자리로
                                        InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                        InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                                    }
                                }
                                else if((CurrentBringItemCode / 10) % 10 == (DropDownItemCode / 10) % 10)//같은 타입의 장비라면 교체
                                {//바꾸기
                                    //인벤토리 장비 -> 착용 장비
                                    PlayerEquipImages[DropDownSlotIndex].gameObject.SetActive(true);
                                    PlayerEquipImages[DropDownSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode).EquipmentImage;
                                    PlayerEquipTierText[DropDownSlotIndex].text = GetTierText(CurrentBringItemCode);
                                    switch(DropDownSlotIndex)
                                    {
                                        case (int)EPlayerEquip.Helmet:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode = CurrentBringItemCode;
                                            break;
                                        case (int)EPlayerEquip.Armor:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode = CurrentBringItemCode;
                                            break;
                                        case (int)EPlayerEquip.Boots:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode = CurrentBringItemCode;
                                            break;
                                        case (int)EPlayerEquip.Weapon:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode = CurrentBringItemCode;
                                            break;
                                        case (int)EPlayerEquip.Accessories:
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode = CurrentBringItemCode;
                                            break;
                                    }
                                    //착용 장비 -> 인벤토리 장비
                                    InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                    InventorySlotsImage[CurrentClickedSlotIndex].sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(DropDownItemCode).EquipmentImage;
                                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(DropDownItemCode);
                                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = DropDownItemCode;
                                }
                                else
                                {//뭣도 아니면 원래 자리로
                                    InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                                }
                                break;
                            }
                        }
                        /*
                        PlayerEquipImages[PlayerEquipIndex].gameObject.SetActive(true);
                        PlayerEquipTierText[PlayerEquipIndex].text = GetTierText(CurrentBringItemCode);
                        */
                    }
                }
                else if (eventData.pointerEnter.name == "TrashCan")//마우스를 놓은 곳이 쓰레기 통일때
                {
                    //장비를 잡았던 슬롯 비우기
                    SoundManager.Instance.PlayUISFX("Item_Remove");
                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[CurrentClickedSlotIndex] = 0;
                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = "";
                }
                else//뭣도 아닐때
                {//원래 자리로
                    SoundManager.Instance.PlayUISFX("Item_PutDown");
                    InventorySlotsImage[CurrentClickedSlotIndex].gameObject.SetActive(true);
                    InventorySlotTierTexts[CurrentClickedSlotIndex].text = GetTierText(CurrentBringItemCode);
                }
            }
            MouseFollowImage.gameObject.SetActive(false);
            CurrentBringItemCode = 0;
            CurrentClickedSlotIndex = 0;
            DropDownItemCode = 0;
            DropDownSlotIndex = 0;
            SetGambling();
        }
    }

    private void MoveUI(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameObject.GetComponentInParent<Canvas>().transform as RectTransform,
            eventData.position,
            Camera.main,
        out localPoint
        );

        MouseFollowImage.transform.position = gameObject.GetComponentInParent<Canvas>().transform.TransformPoint(localPoint);
    }

    private void DisplayEquipDetailInfo(bool IsEquipedEquipment)//장비를 클릭하면 자세한 정보를 출력한다.
    {//true면 장비칸을 클릭한거고, false면 인벤토리를 클릭한거임
        if(IsEquipedEquipment == true)//장비칸에 있는 아이템 클릭
        {//-> 오른쪽 Detail만 출력하면 됨
            //CurrentBringItemCode -> 0일때는 안 들어옴
            EquipmentInfo EquipedEquipmentInfo = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode);
            EquipDetail_EquipImage.gameObject.SetActive(true);
            EquipDetail_EquipImage.sprite = EquipedEquipmentInfo.EquipmentImage;
            EquipDetail_EquipTierText.text = GetTierText(CurrentBringItemCode);
            EquipDetail_EquipNameText.text = EquipedEquipmentInfo.EquipmentName;

            int EquipType = (CurrentBringItemCode / 10) % 10;
            StartCoroutine(LoadEquipSpendSTA(EquipType, (int)EquipedEquipmentInfo.SpendTiredness, true));
            /*
            if (EquipType == (int)EEquipType.TypeWeapon)
                EquipDetail_EquipSTAText.text = "공격시 피로도 : " + EquipedEquipmentInfo.SpendTiredness.ToString();
            else if (EquipType == (int)EEquipType.TypeArmor)
                EquipDetail_EquipSTAText.text = "방어시 피로도 : " + EquipedEquipmentInfo.SpendTiredness.ToString();
            else if (EquipType == (int)EEquipType.TypeHelmet)
                EquipDetail_EquipSTAText.text = "피로도 회복시 피로도는 사용되지 않습니다.";
            else if (EquipType == (int)EEquipType.TypeBoots)
                EquipDetail_EquipSTAText.text = "신발은 피로도를 사용하지 않습니다.";
            else if (EquipType == (int)EEquipType.TypeAcc)
                EquipDetail_EquipSTAText.text = "장신구는 피로도를 사용하지 않습니다.";
            else
                EquipDetail_EquipSTAText.text = "";
            */

            EquipDetail_EquipSTRText.text = EquipedEquipmentInfo.AddSTRAmount.ToString();
            EquipDetail_EquipDURText.text = EquipedEquipmentInfo.AddDURAmount.ToString();
            EquipDetail_EquipRESText.text = EquipedEquipmentInfo.AddRESAmount.ToString();
            EquipDetail_EquipSPDText.text = EquipedEquipmentInfo.AddSPDAmount.ToString();
            EquipDetail_EquipLUKText.text = EquipedEquipmentInfo.AddLUKAmount.ToString();
            EquipDetail_EquipDetailText.text = EquipedEquipmentInfo.EquipmentDetail.ToString();

            //활성화 하기전에 한번 다 초기화
            foreach(GameObject obj in EquipDetail_EquipCardContainer)
                obj.SetActive(false);
            for (int i = 0; i < EquipedEquipmentInfo.EquipmentSlots.Length; i++)
            {
                //활성화
                EquipDetail_EquipCardContainer[i].SetActive(true);
                EquipDetail_EquipCardContainer[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(EquipedEquipmentInfo.EquipmentSlots[i].SlotState);
            }
            //오른쪽의 정보창은 아예 비우기
            EquipDetail_InvenImage.gameObject.SetActive(false);
            EquipDetail_InvenTierText.text = "";
            EquipDetail_InvenNameText.text = "";
            EquipDetail_InvenSTAText.text = "";
            EquipDetail_InvenSTRText.text = "";
            EquipDetail_InvenDURText.text = "";
            EquipDetail_InvenRESText.text = "";
            EquipDetail_InvenSPDText.text = "";
            EquipDetail_InvenLUKText.text = "";
            EquipDetail_InvenDetailText.text = "";
            foreach (GameObject Obj in EquipDetail_InvenCardContainer)
            {
                Obj.SetActive(false);
            }
        }
        else//인벤토리에 있는 아이템 클릭
        {//-> 왼쪽 Detail, 오른쪽 Detail다 출력 해야됨
         //같은 타입의 장비가 없으면 오른쪽은 출력 x
         //인벤의 장비 표시
            if(CurrentBringItemCode == 0 || CurrentBringItemCode == -1)
            {
                return;
            }
            EquipmentInfo UnEquipedEquipment = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(CurrentBringItemCode);
            int EquipedEquipmentCode;
            EquipDetail_InvenImage.gameObject.SetActive(true);
            EquipDetail_InvenImage.sprite = UnEquipedEquipment.EquipmentImage;
            EquipDetail_InvenTierText.text = GetTierText(CurrentBringItemCode);
            EquipDetail_InvenNameText.text = UnEquipedEquipment.EquipmentName;

            int EquipType = (CurrentBringItemCode / 10) % 10;
            StartCoroutine(LoadEquipSpendSTA(EquipType, (int)UnEquipedEquipment.SpendTiredness, false));
            //장비의 종류 10의 자리
            if (EquipType == (int)EEquipType.TypeWeapon)//무기
            {
                //EquipDetail_InvenSTAText.text = "공격시 피로도 : " + UnEquipedEquipment.SpendTiredness.ToString();
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode;
            }
            else if (EquipType == (int)EEquipType.TypeArmor)//방어구
            {
                //EquipDetail_InvenSTAText.text = "방어시 피로도 : " + UnEquipedEquipment.SpendTiredness.ToString();
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipArmorCode;
            }
            else if (EquipType == (int)EEquipType.TypeHelmet)//투구
            {
                //EquipDetail_InvenSTAText.text = "피로도 회복시 피로도는 사용되지 않습니다.";
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipHatCode;
            }
            else if (EquipType == (int)EEquipType.TypeBoots)//신발
            {
                //EquipDetail_InvenSTAText.text = "신발은 피로도를 사용하지 않습니다.";
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipShoesCode;
            }
            else if (EquipType == (int)EEquipType.TypeAcc)//장신구
            {
                //EquipDetail_InvenSTAText.text = "장신구는 피로도를 사용하지 않습니다.";
                EquipedEquipmentCode = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipAccessoriesCode;
            }
            else
            {
                //EquipDetail_InvenSTAText.text = "";
                EquipedEquipmentCode = 0;
            }

            EquipDetail_InvenSTRText.text = UnEquipedEquipment.AddSTRAmount.ToString();
            EquipDetail_InvenDURText.text = UnEquipedEquipment.AddDURAmount.ToString();
            EquipDetail_InvenRESText.text = UnEquipedEquipment.AddRESAmount.ToString();
            EquipDetail_InvenSPDText.text = UnEquipedEquipment.AddSPDAmount.ToString();
            EquipDetail_InvenLUKText.text = UnEquipedEquipment.AddLUKAmount.ToString();
            EquipDetail_InvenDetailText.text = UnEquipedEquipment.EquipmentDetail.ToString();

            //활성화 하기전에 한번 다 초기화
            foreach (GameObject obj in EquipDetail_InvenCardContainer)
                obj.SetActive(false);
            for (int i = 0; i < UnEquipedEquipment.EquipmentSlots.Length; i++)
            {
                //활성화
                EquipDetail_InvenCardContainer[i].SetActive(true);
                EquipDetail_InvenCardContainer[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(UnEquipedEquipment.EquipmentSlots[i].SlotState);
            }
            //같은 타입의 장비가 있거나 혹은 없거나
            EquipmentInfo EquipedEquipmentInfo = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(EquipedEquipmentCode);
            if (EquipedEquipmentCode == 0)//끼고 있는 장비가 없을때
            {
                //아예 비우기
                EquipDetail_EquipImage.gameObject.SetActive(false);
                EquipDetail_EquipTierText.text = "";
                EquipDetail_EquipNameText.text = "";
                EquipDetail_EquipSTAText.text = "";
                EquipDetail_EquipSTRText.text = "";
                EquipDetail_EquipDURText.text = "";
                EquipDetail_EquipRESText.text = "";
                EquipDetail_EquipSPDText.text = "";
                EquipDetail_EquipLUKText.text = "";
                EquipDetail_EquipDetailText.text = "";
                foreach (GameObject Obj in EquipDetail_EquipCardContainer)
                {
                    Obj.SetActive(false);
                }
            }
            else//끼고 있는 장비가 있을때
            {
                EquipDetail_EquipImage.gameObject.SetActive(true);
                EquipDetail_EquipImage.sprite = EquipedEquipmentInfo.EquipmentImage;
                EquipDetail_EquipTierText.text = GetTierText(EquipedEquipmentCode);
                EquipDetail_EquipNameText.text = EquipedEquipmentInfo.EquipmentName;
                StartCoroutine(LoadEquipSpendSTA(EquipType, (int)EquipedEquipmentInfo.SpendTiredness, true));
                /*
                if (EquipType == (int)EEquipType.TypeWeapon)
                    EquipDetail_EquipSTAText.text = "공격시 피로도 : " + EquipedEquipmentInfo.SpendTiredness.ToString();
                else if (EquipType == (int)EEquipType.TypeArmor)
                    EquipDetail_EquipSTAText.text = "방어시 피로도 : " + EquipedEquipmentInfo.SpendTiredness.ToString();
                else if (EquipType == (int)EEquipType.TypeHelmet)
                    EquipDetail_EquipSTAText.text = "피로도 회복시 피로도는 사용되지 않습니다.";
                else if (EquipType == (int)EEquipType.TypeBoots)
                    EquipDetail_EquipSTAText.text = "신발은 피로도를 사용하지 않습니다.";
                else if (EquipType == (int)EEquipType.TypeAcc)
                    EquipDetail_EquipSTAText.text = "장신구는 피로도를 사용하지 않습니다.";
                else
                    EquipDetail_EquipSTAText.text = "";
                */

                EquipDetail_EquipSTRText.text = EquipedEquipmentInfo.AddSTRAmount.ToString();
                EquipDetail_EquipDURText.text = EquipedEquipmentInfo.AddDURAmount.ToString();
                EquipDetail_EquipRESText.text = EquipedEquipmentInfo.AddRESAmount.ToString();
                EquipDetail_EquipSPDText.text = EquipedEquipmentInfo.AddSPDAmount.ToString();
                EquipDetail_EquipLUKText.text = EquipedEquipmentInfo.AddLUKAmount.ToString();
                EquipDetail_EquipDetailText.text = EquipedEquipmentInfo.EquipmentDetail.ToString();

                //활성화 하기전에 한번 다 초기화
                foreach (GameObject obj in EquipDetail_EquipCardContainer)
                    obj.SetActive(false);
                for (int i = 0; i < EquipedEquipmentInfo.EquipmentSlots.Length; i++)
                {
                    //활성화
                    EquipDetail_EquipCardContainer[i].SetActive(true);
                    EquipDetail_EquipCardContainer[i].GetComponent<EquipmentDetailCardContainerUI>().SetActiveCards(EquipedEquipmentInfo.EquipmentSlots[i].SlotState);
                }
            }
        }
    }

    private IEnumerator LoadEquipSpendSTA(int EquipType, int SpendSTAAmount, bool IsEquipedEquipment)
    {
        yield return LocalizationSettings.InitializationOperation;

        string EquipSTATableKey = "";

        if (EquipType == (int)EEquipType.TypeWeapon)
            EquipSTATableKey = "PS_SC_WeaponSTA";
        else if (EquipType == (int)EEquipType.TypeArmor)
            EquipSTATableKey = "PS_SC_ArmorSTA";
        else if (EquipType == (int)EEquipType.TypeHelmet)
            EquipSTATableKey = "PS_SC_HelmetSTA";
        else if (EquipType == (int)EEquipType.TypeBoots)
            EquipSTATableKey = "PS_SC_BootsSTA";
        else if (EquipType == (int)EEquipType.TypeAcc)
            EquipSTATableKey = "PS_SC_AccSTA";

        if (EquipSTATableKey == "")
            EquipSTATableKey = "PS_SC_EquipSTAError";

        var EquipmentTable = LocalizationSettings.StringDatabase.GetTable("PlaySceneShortText");

        if(IsEquipedEquipment == true)
        {//끼고 있는 장비
            EquipDetail_EquipSTAText.text = EquipmentTable.GetEntry(EquipSTATableKey).GetLocalizedString();
            if(EquipType == (int)EEquipType.TypeWeapon || EquipType == (int)EEquipType.TypeArmor)
            {
                EquipDetail_EquipSTAText.text += SpendSTAAmount.ToString();
            }
        }
        else
        {//인벤토리 속 장비
            EquipDetail_InvenSTAText.text = EquipmentTable.GetEntry(EquipSTATableKey).GetLocalizedString();
            if (EquipType == (int)EEquipType.TypeWeapon || EquipType == (int)EEquipType.TypeArmor)
            {
                EquipDetail_InvenSTAText.text += SpendSTAAmount.ToString();
            }
        }
    }

    public void PressEquipGamblingLevelUPButton()
    {
        //경험치가 줄어든다. //경험치가 먼저 줄어들어야지 레벨에 맞는 경험치가 줄어듬
        //플레이어의 뽑기 레벨이 오른다.
        //SetGambling으로 ui를 업데이트한다.
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-EquipmentInfoManager.Instance.GetGamblingLevelUPCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel), true);
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel++;
        SetGambling();
    }

    public void PressEquipGamblingGachaButton()//ActiveGacha
    {//
        //경험치가 줄어든다.
        SoundManager.Instance.PlayUISFX("UI_Button");
        PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-EquipmentInfoManager.Instance.GetGamblingGachaCost(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel), true);
        //초기화
        //삼격형 판 초기화
        EquipGachaTrianglePlate.gameObject.SetActive(true);//삼각형 판
        EquipGachaTrianglePlate.sprite = GachaTrianglePlateSprites[(int)ETriangleState.ZeroLightOn];
        EquipGachaIcon_TierGem.SetActive(false);
        EquipGachaIcon_StateType.SetActive(false);
        EquipGachaIcon_EquipType.SetActive(false);
        EquipGachaIcon_MultiType.SetActive(false);
        EquipGachaTriangleBlindObject.SetActive(true);
        FinalEquipGachaCapsule.SetActive(false);
        FinalEquipGachaButton.SetActive(false);
        FinalEquipImage.gameObject.SetActive(false);
        //티어 보석 뽑는 UI 초기화
        EquipGachaObject.SetActive(true);
        EquipGachaEquipmentObject.SetActive(true);//장비를 감추고 있는 캡슐과 장비 이미지를 가지고 있는 오브젝트
        EquipGachaCapsule.GetComponent<RectTransform>().localScale = Vector3.one;
        EquipGachaCapsule.GetComponent<Image>().color = Color.white;
        EquipGachaCapsule.SetActive(true);
        EquipGachaEquipmentObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 750);
        ClickButton.SetActive(false);
        GetEquipClickButton.SetActive(false);
        //장비 성향, 장비 종류, 곱 성향 뽑는 UI 초기화(일단은 부모만 끄기)
        GachaCardSelectObject.SetActive(false);

        for(int i = 0; i < EquipGachaLight.Length; i++)
        {
            EquipGachaLight[i].GetComponent<Image>().color = GachaTierLightColor[i];
            //EquipGachaLightOutline[i].color = GachaTierLightColor[0];
            EquipGachaLight[i].SetActive(false);
        }
        
        //결과로 나오는 장비의 이미지와 인벤토리에 미리 넣어둠, 어짜피 업데이트 하지 않으면 UI(인벤토리)에서는 안보이니까
        //GetGamblingEquipmentCode <- gambling 레벨에 맞는 장비 코드 반환
        //이제 이거 필요 X 코드는 차근차근 완성되는 형태임
        //여기 아래부터는 나중에 불러와야할 애들임
        //GachaResultEquipCode = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
        //EquipGachaResultImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(GachaResultEquipCode).EquipmentImage;
        //이건 나중에 활성화
        //PlayerMgr.GetPlayerInfo().PutEquipmentToInven(GachaResultEquipCode);
        //띄용띄용이 끝나면 클릭버튼 활성화
        GachaTierNum = EquipmentInfoManager.Instance.GetGamblingTierCode(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
        EquipGachaResultImage.sprite = GachaTierGemSprites[GachaTierNum - 1];//GachaTierNum는 최소 1이 나옴
        EquipGachaEquipmentObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0.7f).SetEase(Ease.OutBounce).
            OnComplete(() => { ActiveGachaClickButton(); });

    }

    private void ActiveGachaClickButton()
    {
        ClickButton.SetActive(true);
        ClickButton.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, -5f);
        ClickButton.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 5), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void PressGachaClickButton()
    {
        ClickButton.GetComponent<RectTransform>().DOKill();
        ClickButton.SetActive(false);
        //이 버튼을 누르면 버튼은 비활성화
        // 1티어 장비라면 0번까지 활성화 , 흰색// 2티어 장비라면  1번까지 활성화, 파란색
        // 3티어 장비라면 2번까지 활성화, 보라색// 4티어 장비라면 3번까지 활성화, 노란색
        // 5티어 장비라면 4번까지 활성화, 
        //미리 저장한 GachaResultEquipCode로 티어를 파악해 그것에 맞게 연출
        // 1티어 보다 낮은 장비는 없으므로 1은 바로 활성화
        CurrentGachaPhase = (int)EGachaPhase.GemPhase;
        LightStorage.SetActive(true);
        EquipGachaLight[0].GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
        EquipGachaLight[0].GetComponent<RectTransform>().eulerAngles = 
            GachaLightRot[0] + new Vector3(0, 0, Random.Range(-GachaLightZVariation, GachaLightZVariation));
        EquipGachaLight[0].SetActive(true);
        PlayEquipGachaLightSound(0);
        EquipGachaLight[0].GetComponent<RectTransform>().DOScaleY(1, 0.3f).SetEase(Ease.OutExpo).OnComplete(() => { StartCoroutine(EquipGachaCoroutine(GachaTierNum)); });
    }

    IEnumerator EquipGachaCoroutine(int EquipmentTier)
    {
        int CurrentEffectLevel = 1;
        bool IsAnimationEnd = false;
        //아래꺼가 계속 해당하는 티어가 나올때까지 반복되야함
        while (true)
        {
            yield return null;
            IsAnimationEnd = false;
            yield return new WaitForSeconds(0.3f);

            if (EquipmentTier <= CurrentEffectLevel)//1티어라면
            {
                break;
            }
            else//1티어 이상일때
            {
                EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(3f, 3f), 0.3f).SetEase(Ease.Linear).
                    OnComplete(() =>
                    { 
                        ContinueOfEquipGacha(CurrentEffectLevel++); 
                        EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(1.5f, 1.5f), 0.3f).
                        OnComplete(() => 
                        { 
                            IsAnimationEnd = true;
                            //EquipGachaCapsule.GetComponent<Image>().color = GachaTierLightColor[CurrentEffectLevel-1];
                            }); 
                    });//ContinueOfGacha에 int값을 전달후에 ++가됨
            }

            while(true)
            {
                if(IsAnimationEnd == true)
                {
                    break;
                }
                yield return null;
            }
        }
        SoundManager.Instance.PlaySFX("EquipGacha_Result");
        EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(3f, 3f), 0.3f).SetEase(Ease.Linear).OnComplete(() => { EquipGachaOpenTierGem(); });
    }
    protected void ContinueOfEquipGacha(int CurrentEffectLevel)
    {
        //빛이 추가 되고, 빛 색깔도 바뀜
        for(int i = 0; i < CurrentEffectLevel + 1; i++)
        {
            EquipGachaLight[i].GetComponent<Image>().color = GachaTierLightColor[CurrentEffectLevel];
            if (EquipGachaLight[i].activeSelf == false)
            {//false면 키는거임
                EquipGachaLight[i].GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
                EquipGachaLight[i].GetComponent<RectTransform>().eulerAngles =
                    GachaLightRot[i] + new Vector3(0, 0, Random.Range(-GachaLightZVariation, GachaLightZVariation));
                EquipGachaLight[i].SetActive(true);
                PlayEquipGachaLightSound(CurrentEffectLevel);
                //Debug.Log("AAAAAA");
                EquipGachaLight[i].GetComponent<RectTransform>().DOScaleY(1, 0.3f).SetEase(Ease.OutExpo);
            }
        }
    }

    protected void PlayEquipGachaLightSound(int CurrentEffectLevel)
    {
        switch(CurrentEffectLevel)
        {
            case 0:
                SoundManager.Instance.PlaySFX("EquipGacha_TierOne");
                break;
            case 1:
                SoundManager.Instance.PlaySFX("EquipGacha_TierTwo");
                break;
            case 2:
                SoundManager.Instance.PlaySFX("EquipGacha_TierThree");
                break;
            case 3:
                SoundManager.Instance.PlaySFX("EquipGacha_TierFour");
                break;
            case 4:
                SoundManager.Instance.PlaySFX("EquipGacha_TierFive");
                break;
            case 5:
                SoundManager.Instance.PlaySFX("EquipGacha_TierSix");
                break;
        }
    }
    protected void EquipGachaOpenTierGem()
    {
        EquipGachaCapsule.GetComponent<RectTransform>().DOScale(new Vector2(7f, 7f), 0.8f).
            OnComplete(() => 
            {
                for(int i = 0; i < EquipGachaLight.Length; i++)
                {
                    EquipGachaLight[i].GetComponent<Image>().color = GachaTierLightColor[i];
                    //obj.GetComponent<SpriteOutline>().Regenerate();
                    EquipGachaLight[i].SetActive(false);
                };
                EquipGachaCapsule.GetComponent<Image>().DOFade(0, 0.5f).
                OnComplete(() => 
                {
                    LightStorage.SetActive(false);
                    GetEquipClickButton.SetActive(true);
                }); });
    }
    public void EquipGachaPhaseEndButton()//이게 이제 확정 버튼 같은거
    {
        //1페이지 끝(보석이 공개 되고 그 보석이 가운데에 박히면서 플레이트가 바뀌어야함)
        //2페이즈 끝(장비 성향이 공개후 확정 되고 위쪽에 박히면서 플레이트가 바뀌어야함)
        //3페이즈 끝(장비 종류가 공개후 확정 되고 왼쪽에 박히면서 플레이트가 바뀌어야함)
        //4페이즈 끝(곱 성향이 공개후 확정 되고 오른쪽에 박히면서 플레이트가 바뀌어야함)
        GachaConfirmButton.SetActive(false);
        GachaAgainButton.SetActive(false);
        RemainGachaText.gameObject.SetActive(false);
        switch (CurrentGachaPhase)
        {
            case (int)EGachaPhase.GemPhase://1페이즈끝
                EquipGachaPhaseOneEnd();
                break;
            case (int)EGachaPhase.StatePhase://2페이즈끝
                EquipGachaPhaseTwoEnd();
                break;
            case (int)EGachaPhase.TypePhase://3페이즈끝
                EquipGachaPhaseThreeEnd();
                break;
            case (int)EGachaPhase.MultiplePhase://4페이즈끝
                EquipGachaPhaseFourEnd();
                break;
            case (int)EGachaPhase.EndPhase://5페이즈끝(모든 요소가 함쳐진후 장비 공개됬을때)
                EquipGachaEndPhaseEnd();
                break;
        }
    }
    public void EquipGachaAgainButton()
    {
        //활성화된 카드를 모았다가 다시 펼쳐야함
        //그 과정에서 카드의 순서는 다시 랜덤이 됨
        //다시뽑기 횟수를 차감해야함
        //1. 다시뽑기를 누르면 텍스트와 버튼들이 사라지고 카드가 다시 뒤집힘, 하이라이트도 없앰
        //2. 카드 뒤집기가 완료되면 중간으로 모음
        //3. 조금뒤 다시 펼침
        SoundManager.Instance.PlayUISFX("UI_Button");

        if(RemainGachaCount > 0)
            RemainGachaCount--;

        GachaConfirmButton.SetActive(false);
        GachaAgainButton.SetActive(false);
        RemainGachaText.gameObject.SetActive(false);
        GachaCardHighligh.SetActive(false);
        ReReverseAllOtherGachaCard().OnComplete(() =>//여기에 들어오면 
        {
            DecideGachaCardResult();//결과 바꾸기
            SoundManager.Instance.PlaySFX("ReverseCard_Open");
            GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = 20f;
            DOTween.To(() => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing,   // getter
             x => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = x, // setter
            -168f,                          // 목표 값
            0.5f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    SoundManager.Instance.PlaySFX("ReverseCard_Open");
                    GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = -168f;
                    DOTween.To(() => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing,   // getter
                     x => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = x, // setter
                    20f,                          // 목표 값
                    0.5f).OnComplete(() =>
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            GachaSelectCards[i].GetComponent<Button>().interactable = true;
                        }
                    });
                });
            });
        });
    }
    protected void EquipGachaPhaseOneEnd()
    {//(보석이 공개 되고 그 보석이 가운데에 박히면서 플레이트가 바뀌어야함)
        //EquipmentImage를 이동후에 없애면서 초기화, Plate에 있는 이미지를 그 이미지로 바꾼다.
        //그리고 다음 페이지로 자동 이동
        GetEquipClickButton.SetActive(false);
        EquipGachaResultImage.rectTransform.DOAnchorPosY(-115f, 0.5f).OnComplete(() => 
        {
            EquipGachaIcon_TierGem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -115f);
            EquipGachaIcon_TierGem.SetActive(true);
            EquipGachaIcon_TierGem.GetComponent<Image>().sprite = GachaTierGemSprites[GachaTierNum - 1];
            EquipGachaTrianglePlate.sprite = GachaTrianglePlateSprites[(int)ETriangleState.OneLightOn];
            EquipGachaResultImage.rectTransform.position = Vector3.zero;
            EquipGachaEquipmentObject.SetActive(false);
            DOVirtual.DelayedCall(0.5f, () =>
            {
                //여기에서 다음꺼_ 장비 성향 뽑기를 진행 해야함
                EquipGachaPhaseTwoStart();
            });
        });
    }
    protected void EquipGachaPhaseTwoStart()
    {
        CurrentGachaPhase = (int)EGachaPhase.StatePhase;
        RemainGachaCount = 3;
        DecideGachaCardResult();
        GachaCardSelectObject.SetActive(true);
        CardSelectTitle.text = "장비 성향";
        GachaConfirmButton.SetActive(false);
        GachaAgainButton.SetActive(false);
        RemainGachaText.gameObject.SetActive(false);
        GachaCardHighligh.SetActive(false);
        GachaVirtualCard.SetActive(false);
        for(int i = 0; i < GachaSelectCards.Length; i++)
        {
            GachaSelectCards[i].GetComponent<Button>().interactable = false;
            GachaSelectCards[i].SetActive(false);
        }
        for (int i = 0; i < 8; i++)
        {
            GachaSelectCards[i].SetActive(true);
            GachaSelectCards[i].GetComponent<Image>().sprite = GachaCardSprites[(int)EGachaIconNCard.OnlyForCard_BackCard];
        }
        //카드가 등장할때도 애니메이션이 있으면 좋을것 같은데?//Horizontal layout group의 spacing 을 -168에서 20으로 쫙 펼침
        SoundManager.Instance.PlaySFX("ReverseCard_Open");
        GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = -168f;
        DOTween.To(() => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing,   // getter
         x => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = x, // setter
        20f,                          // 목표 값
        0.5f).OnComplete(() =>
        {
            for (int i = 0; i < 8; i++)
            {
                GachaSelectCards[i].GetComponent<Button>().interactable = true;
            }
        });
    }
    protected void EquipGachaPhaseTwoEnd()
    {//확정.
        //스테이트 타입은 0~7까지//얻은 타입을 저장
        //0. 날라가야 하는 버츄얼 카드를 해당 스프라이트로 교체후 활성화
        //1. 해당 카드가 위치까지 날라가서(960,0) 비활성화, 삼각판 위의 해당 icon을 활성화 시킴
        //2. 카드를 원래 위치로 
        SoundManager.Instance.PlayUISFX("UI_Button");
        GachaStateTypeNum = RemainCardResult[SelectedGachaCardNum];//이건 0~7까지 자연스럽게 들어가게됨 나머지는 -몇을 해줘야 옳은 코드가 됨
        GachaCardHighligh.SetActive(false);
        GachaVirtualCard.GetComponent<Image>().sprite = GachaCardSprites[RemainCardResult[SelectedGachaCardNum]];
        GachaVirtualCard.GetComponent<RectTransform>().anchoredPosition = GachaSelectCards[SelectedGachaCardNum].GetComponent<RectTransform>().anchoredPosition;
        GachaVirtualCard.GetComponent<RectTransform>().localScale = Vector2.one;
        GachaVirtualCard.SetActive(true);
        GachaVirtualCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(960, 0), 0.5f);
        GachaVirtualCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
        GachaVirtualCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() =>
        {
            EquipGachaTrianglePlate.sprite = GachaTrianglePlateSprites[(int)ETriangleState.TwoLightOn];
            EquipGachaIcon_StateType.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 145f);
            EquipGachaIcon_StateType.GetComponent<Image>().sprite = GachaIconSprites[RemainCardResult[SelectedGachaCardNum]];
            EquipGachaIcon_StateType.SetActive(true);
            GachaVirtualCard.SetActive(false);
            //다시 뒤집고 모으기
            ReReverseAllOtherGachaCard().OnComplete(() =>
            {
                SoundManager.Instance.PlaySFX("ReverseCard_Open");
                GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = 20f;
                DOTween.To(() => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing,   // getter
                 x => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = x, // setter
                -168f,                          // 목표 값
                0.5f).OnComplete(() => { EquipGachaPhaseThreeStart(); });
            });
        });
    }
    protected void EquipGachaPhaseThreeStart()
    {
        CurrentGachaPhase = (int)EGachaPhase.TypePhase;
        DecideGachaCardResult();
        GachaCardSelectObject.SetActive(true);
        CardSelectTitle.text = "장비 종류";
        GachaConfirmButton.SetActive(false);
        GachaAgainButton.SetActive(false);
        RemainGachaText.gameObject.SetActive(false);
        GachaCardHighligh.SetActive(false);
        GachaVirtualCard.SetActive(false);
        for (int i = 0; i < GachaSelectCards.Length; i++)
        {
            GachaSelectCards[i].GetComponent<Button>().interactable = false;
            GachaSelectCards[i].SetActive(false);
        }
        for (int i = 0; i < 5; i++)
        {
            GachaSelectCards[i].SetActive(true);
            GachaSelectCards[i].GetComponent<Image>().sprite = GachaCardSprites[(int)EGachaIconNCard.OnlyForCard_BackCard];
        }
        //카드가 등장할때도 애니메이션이 있으면 좋을것 같은데?//Horizontal layout group의 spacing 을 -168에서 20으로 쫙 펼침
        SoundManager.Instance.PlaySFX("ReverseCard_Open");
        GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = -168f;
        DOTween.To(() => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing,   // getter
         x => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = x, // setter
        20f,                          // 목표 값
        0.5f).OnComplete(() =>
        {
            for (int i = 0; i < 5; i++)
            {
                GachaSelectCards[i].GetComponent<Button>().interactable = true;
            }
        });
    }
    protected void EquipGachaPhaseThreeEnd()
    {
        //720 -385
        //스테이트 타입은 0~7까지//얻은 타입을 저장
        //0. 날라가야 하는 버츄얼 카드를 해당 스프라이트로 교체후 활성화
        //1. 해당 카드가 위치까지 날라가서(960,0) 비활성화, 삼각판 위의 해당 icon을 활성화 시킴
        //2. 카드를 원래 위치로 
        SoundManager.Instance.PlayUISFX("UI_Button");
        GachaEquipTypeNum = RemainCardResult[SelectedGachaCardNum] - 8;//이건 0~7까지 자연스럽게 들어가게됨 나머지는 -몇을 해줘야 옳은 코드가 됨
        GachaCardHighligh.SetActive(false);
        GachaVirtualCard.GetComponent<Image>().sprite = GachaCardSprites[RemainCardResult[SelectedGachaCardNum]];
        GachaVirtualCard.GetComponent<RectTransform>().anchoredPosition = GachaSelectCards[SelectedGachaCardNum].GetComponent<RectTransform>().anchoredPosition;
        GachaVirtualCard.GetComponent<RectTransform>().localScale = Vector2.one;
        GachaVirtualCard.SetActive(true);
        GachaVirtualCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(720, -385), 0.5f);
        GachaVirtualCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
        GachaVirtualCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() =>
        {
            EquipGachaTrianglePlate.sprite = GachaTrianglePlateSprites[(int)ETriangleState.ThreeLightOn];
            EquipGachaIcon_EquipType.GetComponent<RectTransform>().anchoredPosition = new Vector2(-240f, -240f);
            EquipGachaIcon_EquipType.GetComponent<Image>().sprite = GachaIconSprites[RemainCardResult[SelectedGachaCardNum]];
            EquipGachaIcon_EquipType.SetActive(true);
            GachaVirtualCard.SetActive(false);
            //다시 뒤집고 모으기
            ReReverseAllOtherGachaCard().OnComplete(() =>
            {
                SoundManager.Instance.PlaySFX("ReverseCard_Open");
                GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = 20f;
                DOTween.To(() => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing,   // getter
                 x => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = x, // setter
                -168f,                          // 목표 값
                0.5f).OnComplete(() => { EquipGachaPhaseFourStart(); });
            });
        });
    }
    protected void EquipGachaPhaseFourStart()
    {
        CurrentGachaPhase = (int)EGachaPhase.MultiplePhase;
        //여기는 만약 전에 뽑은 장비의 종류가 신발 혹은 장신구였으면 Non카드 하나만 나옴
        DecideGachaCardResult(GachaEquipTypeNum == (int)EEquipType.TypeBoots || GachaEquipTypeNum == (int)EEquipType.TypeAcc);
        GachaCardSelectObject.SetActive(true);
        CardSelectTitle.text = "곱 성향";
        GachaConfirmButton.SetActive(false);
        GachaAgainButton.SetActive(false);
        RemainGachaText.gameObject.SetActive(false);
        GachaCardHighligh.SetActive(false);
        GachaVirtualCard.SetActive(false);
        for (int i = 0; i < GachaSelectCards.Length; i++)
        {
            GachaSelectCards[i].GetComponent<Button>().interactable = false;
            GachaSelectCards[i].SetActive(false);
        }
        for (int i = 0; i < RemainCardResult.Count; i++)
        {
            GachaSelectCards[i].SetActive(true);
            GachaSelectCards[i].GetComponent<Image>().sprite = GachaCardSprites[(int)EGachaIconNCard.OnlyForCard_BackCard];
        }
        //카드가 등장할때도 애니메이션이 있으면 좋을것 같은데?//Horizontal layout group의 spacing 을 -168에서 20으로 쫙 펼침
        if(GachaEquipTypeNum == (int)EEquipType.TypeBoots || GachaEquipTypeNum == (int)EEquipType.TypeAcc)
        {
            GachaAgainButton.GetComponent<Button>().interactable = false;
            GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = 20;
            for (int i = 0; i < RemainCardResult.Count; i++)
            {
                GachaSelectCards[i].GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            SoundManager.Instance.PlaySFX("ReverseCard_Open");
            GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = -168f;
            DOTween.To(() => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing,   // getter
             x => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = x, // setter
            20f,                          // 목표 값
            0.5f).OnComplete(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    GachaSelectCards[i].GetComponent<Button>().interactable = true;
                }
            });
        }
    }
    protected void EquipGachaPhaseFourEnd()
    {
        //1200,-385
        SoundManager.Instance.PlayUISFX("UI_Button");
        GachaMultipleTypeNum = RemainCardResult[SelectedGachaCardNum] - 13;
        GachaCardHighligh.SetActive(false);
        GachaVirtualCard.GetComponent<Image>().sprite = GachaCardSprites[RemainCardResult[SelectedGachaCardNum]];
        GachaVirtualCard.GetComponent<RectTransform>().anchoredPosition = GachaSelectCards[SelectedGachaCardNum].GetComponent<RectTransform>().anchoredPosition;
        GachaVirtualCard.GetComponent<RectTransform>().localScale = Vector2.one;
        GachaVirtualCard.SetActive(true);
        GachaVirtualCard.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1200, -385), 0.5f);
        GachaVirtualCard.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 1080), 0.5f, RotateMode.FastBeyond360);//.SetEase(Ease.OutQuad);
        GachaVirtualCard.GetComponent<RectTransform>().DOScale(Vector2.zero, 0.5f).OnComplete(() =>
        {
            EquipGachaTrianglePlate.sprite = GachaTrianglePlateSprites[(int)ETriangleState.FourLightOn];
            EquipGachaIcon_MultiType.GetComponent<RectTransform>().anchoredPosition = new Vector2(240f, -240f);
            EquipGachaIcon_MultiType.GetComponent<Image>().sprite = GachaIconSprites[RemainCardResult[SelectedGachaCardNum]];
            EquipGachaIcon_MultiType.SetActive(true);
            GachaVirtualCard.SetActive(false);
            //재료가 옳은 칸에 다 들어가면.... blind 끄고, 카드도 끄고
            ReReverseAllOtherGachaCard().OnComplete(() =>
            {
                if (GachaEquipTypeNum == (int)EEquipType.TypeBoots || GachaEquipTypeNum == (int)EEquipType.TypeAcc)
                {
                    GachaCardSelectObject.SetActive(false);
                    EquipGachaTriangleBlindObject.SetActive(false);
                    EquipGachaEndPhaseStart();
                }
                else
                {
                    SoundManager.Instance.PlaySFX("ReverseCard_Open");
                    GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = 20f;
                    DOTween.To(() => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing,   // getter
                     x => GachaCardStorage.GetComponent<HorizontalLayoutGroup>().spacing = x, // setter
                    -168f,                          // 목표 값
                    0.5f).OnComplete(() =>
                    {
                        GachaCardSelectObject.SetActive(false);
                        EquipGachaTriangleBlindObject.SetActive(false);
                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            EquipGachaEndPhaseStart();
                        });
                    });
                }
            });
        });
    }
    protected void EquipGachaEndPhaseStart()
    {
        //판에 올라가 있는 아이콘이 모임... 가까이 갈수록 느려지게(0,-115로)..... 키이잉~쾅(그냥 GachaResult를 쓰면 될듯)? 소리가 있으면 좋겠는데?
        //1. 합쳐지면서 중간에서 판의 중간에서 빛이 점점 커진다.
        //2. 빛(흰색의 원)이 엄청 커진다음에는 페이드 인이 되면서 장비가 모습을 드러낸다.(삼각형 판에 있는 icon들은 모두 없어진 상태여야한다.)
        //스케일이 0~9까지 커졌다가 30까지 커지고 페이드 인
        //3. 장비를 클릭하면 획득한다.
        SoundManager.Instance.PlaySFX("EquipGacha_Result");
        CurrentGachaPhase = (int)EGachaPhase.EndPhase;
        EquipGachaIcon_StateType.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -115f), 0.5f).SetEase(Ease.OutCubic);
        EquipGachaIcon_EquipType.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -115f), 0.5f).SetEase(Ease.OutCubic);
        EquipGachaIcon_MultiType.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -115f), 0.5f).SetEase(Ease.OutCubic);
        FinalEquipGachaCapsule.transform.localScale = Vector3.zero;
        FinalEquipGachaCapsule.GetComponent<Image>().color = Color.white;
        FinalEquipGachaCapsule.SetActive(true);
        FinalEquipGachaCapsule.transform.DOScale(new Vector3(9f, 9f, 9f), 0.5f).OnComplete(() =>
        {
            EquipGachaIcon_TierGem.SetActive(false);
            EquipGachaIcon_StateType.SetActive(false);
            EquipGachaIcon_EquipType.SetActive(false);
            EquipGachaIcon_MultiType.SetActive(false);
            GachaResultEquipCode = 10000 + (GachaTierNum * 1000) + (GachaStateTypeNum * 100) + (GachaEquipTypeNum * 10) +GachaMultipleTypeNum;
            FinalEquipImage.sprite = EquipmentInfoManager.Instance.GetPlayerEquipmentInfo(GachaResultEquipCode).EquipmentImage;
            FinalEquipImage.gameObject.SetActive(true);
            FinalEquipGachaButton.SetActive(true);
            FinalEquipGachaButton.GetComponent<Button>().interactable = false;

            FinalEquipGachaCapsule.transform.DOScale(new Vector3(30f, 30f, 30f), 0.3f).OnComplete(() =>
            {//화면을 덮을 정도로 커짐
                FinalEquipGachaCapsule.GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() =>
                {//페이드 인
                    FinalEquipGachaCapsule.SetActive(false);
                    FinalEquipGachaButton.GetComponent<Button>().interactable = true;
                });
            });
        });
    }
    protected void EquipGachaEndPhaseEnd()
    {
        PlayerMgr.GetPlayerInfo().PutEquipmentToInven(GachaResultEquipCode);//인벤토리에 넣고
        PressGetEquipClickButton();
    }
    protected void DecideGachaCardResult(bool IsShoesOrAcc = false)
    {
        RemainCardResult.Clear();
        switch(CurrentGachaPhase)
        {
            case (int)EGachaPhase.StatePhase://0~7
                for(int i = (int)EGachaIconNCard.StateType_STR; i < (int)EGachaIconNCard.StateType_Normal + 1; i++)
                    RemainCardResult.Add(i);
                break;
            case (int)EGachaPhase.TypePhase://8~12(0~4)
                for (int i = (int)EGachaIconNCard.EquipType_Weapon; i < (int)EGachaIconNCard.EquipType_Acc + 1; i++)
                    RemainCardResult.Add(i);
                break;
            case (int)EGachaPhase.MultiplePhase://13~16(0~3)
                if(IsShoesOrAcc == true)
                {
                    RemainCardResult.Add((int)EGachaIconNCard.MultyType_Non);
                }
                else
                {
                    for (int i = (int)EGachaIconNCard.MultyType_01; i < (int)EGachaIconNCard.MultyType_03 + 1; i++)
                        RemainCardResult.Add(i);
                }
                break;
            default:
                break;
        }

        for(int i = RemainCardResult.Count - 1; i > 0; i-- )
        {
            int j = Random.Range(0, i + 1);

            (RemainCardResult[i], RemainCardResult[j]) = (RemainCardResult[j], RemainCardResult[i]);
        }
    }
    public void PressGachaCard(int GachaCardNum)//이건 카드(버튼)에 연결//0~7까지 가능
    {
        //SetEase에서 90도 까지 카드 넘어갈때 속도 점점 느려지게 해야할듯//outcubic
        //
        //클릭하면 넘어가는 애니메이션 발생, 화면에 보이지 않는 각도가 되면 sprite 변경
        //LowerMGVirtualCard[FixedCardNum].GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).OnComplete(() =>
        //이게 안 보일때까지 되집는 식
        //LowerMGVirtualCard[FixedCardNum].GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).OnComplete(() =>
        //다른 카드들도 다 똑같이 해야함
        //모든 카드들도 interatable 비활성화
        //다 뒤집고 나니까 이게 구분이 안감.... 구분가게 하는방법..... 외각선?
        SelectedGachaCardNum = GachaCardNum;
        for (int i = 0; i < GachaSelectCards.Length; i++)
        {
            GachaSelectCards[i].GetComponent<Button>().interactable = false;
        }
        SoundManager.Instance.PlaySFX("ReverseCard_Open");
        GachaSelectCards[GachaCardNum].GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            GachaSelectCards[GachaCardNum].GetComponent<Image>().sprite = GachaCardSprites[RemainCardResult[GachaCardNum]];
            GachaSelectCards[GachaCardNum].GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).SetEase(Ease.InCirc).OnComplete(() =>
            {
                GachaCardHighligh.SetActive(true);
                GachaCardHighligh.GetComponent<RectTransform>().anchoredPosition = GachaSelectCards[GachaCardNum].GetComponent<RectTransform>().anchoredPosition;
                DOVirtual.DelayedCall(0.3f, () =>
                {
                    //다른카드들 다 뒤집기
                    ReverseAllOtherGachaCard(GachaCardNum);
                });
            });
        });
    }
    protected void ReverseAllOtherGachaCard(int SelectionCard)
    {
        //SelectionCard말고 다 뒤집기
        Sequence Seq = DOTween.Sequence();
        for (int i = 0; i < GachaSelectCards.Length; i++)
        {
            if (i == SelectionCard)
                continue;

            if (GachaSelectCards[i].activeSelf == false)
                continue;

            int Num = i;
            Seq.AppendCallback(() =>
            {
                ReverseGachaCard(Num);
            });
            Seq.AppendInterval(0.2f); // 0.2초 간격
        }
        Seq.OnComplete(() =>
        {
            GachaConfirmButton.SetActive(true);
            GachaAgainButton.SetActive(true);
            RemainGachaText.gameObject.SetActive(true);
            RemainGachaText.text = "남은 횟수 : " + RemainGachaCount + "회";
            if (RemainGachaCount > 0 && RemainCardResult.Count >= 2)
                GachaAgainButton.GetComponent<Button>().interactable = true;
            else
                GachaAgainButton.GetComponent<Button>().interactable = false;
            Seq.Kill();
        });
    }
    protected void ReverseGachaCard(int ReverseCardNum)
    {
        SoundManager.Instance.PlaySFX("ReverseCard_Open");
        GachaSelectCards[ReverseCardNum].GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            GachaSelectCards[ReverseCardNum].GetComponent<Image>().sprite = GachaCardSprites[RemainCardResult[ReverseCardNum]];
            GachaSelectCards[ReverseCardNum].GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).SetEase(Ease.InCirc);
        });
    }
    protected Sequence ReReverseAllOtherGachaCard()
    {
        Sequence Seq = DOTween.Sequence();
        for(int i = 0; i < GachaSelectCards.Length; i++)
        {
            //활성화 안된 놈들은 무시
            if (GachaSelectCards[i].activeSelf == false)
                continue;

            int Num = i;
            Seq.AppendCallback(() =>
            {
                ReReverseGachaCard(Num);
            });
            Seq.AppendInterval(0.2f); // 0.2초 간격
        }
        Seq.AppendInterval(0.5f);
        return Seq;
    }
    protected void ReReverseGachaCard(int ReverseCardNum)
    {
        SoundManager.Instance.PlaySFX("ReverseCard_Open");
        GachaSelectCards[ReverseCardNum].GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, -90, 0), 0.2f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            GachaSelectCards[ReverseCardNum].GetComponent<Image>().sprite = GachaCardSprites[(int)EGachaIconNCard.OnlyForCard_BackCard];
            GachaSelectCards[ReverseCardNum].GetComponent<RectTransform>().DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).SetEase(Ease.InCirc);
        });
    }
    protected void PressGetEquipClickButton()//이건 마지막에 클릭되야함?
    {
        EquipGachaObject.SetActive(false);
        SetGambling();//이걸로 Gambling UI를 업데이트 하고
        SetInventory();//인벤토리도 업데이트 해야함
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
    }
}
