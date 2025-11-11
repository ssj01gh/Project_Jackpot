using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.AI;

public enum EEquipmentType
{
    Weapon = 1,
    Armor,
    Helmet,
    Boots,
    Accessories,
}

public enum EEquipState
{
    StateSTR,
    StateDUR,
    StateRES,
    StateSPD,
    StateLUK
}
public class EquipmentInfoManager : MonoSingleton<EquipmentInfoManager>
{
    [Header("EquipmentSlotSprite")]
    public float[] EquipmentSlotIDs;
    public Sprite[] EquipmentSlotSprites;
    [Header("PlayerWeapon")]
    public EquipmentSO[] PlayerWeaponSOs;
    [Header("PlayerArmor")]
    public EquipmentSO[] PlayerArmorSOs;
    [Header("PlayerHelmet")]
    public EquipmentSO[] PlayerHelmetSOs;
    [Header("PlayerBoots")]
    public EquipmentSO[] PlayerBootsSOs;
    [Header("PlayerAsseccories")]
    public EquipmentSO[] PlayerAccessoriesSOs;
    [Header("PlayerEquipIncreaseState")]
    public EquipIncreaseSO[] PlayerEquipIncreaseSOs;
    [Header("PlayerSlots")]
    public EquipSlotSO[] PlayerEquipSlotSos;
    [Header("PlayerEquipDetails")]
    public PlayerEquipDetailSO[] PlayerEquipDetailSOs;
    [Header("PlayerEquipSprite")]
    public EquipSpriteSO[] PlayerEquipSpriteSOs;

    [Header("MonWeapon")]
    public EquipmentSO[] MonWeaponSOs;
    [Header("MonArmor")]
    public EquipmentSO[] MonArmorSOs;
    [Header("MonAnotherEquip")]
    public EquipmentSO[] MonAnotherEquipSOs;

    protected Dictionary<float, Sprite> EquipmentSlotSpriteStorage = new Dictionary<float, Sprite>();
    protected Dictionary<int, EquipmentSO> EquipmentStorage = new Dictionary<int, EquipmentSO>();
    protected Dictionary<int, EquipmentSO> MonEquipmentStroage = new Dictionary<int, EquipmentSO>();

    protected List<int> TierZeroEquipmentCodes = new List<int>();
    protected List<int> TierOneEquipmentCodes = new List<int>();
    protected List<int> TierTwoEquipmentCodes = new List<int>();
    protected List<int> TierThreeEquipmentCodes = new List<int>();
    protected List<int> TierFourEquipmentCodes = new List<int>();
    protected List<int> TierFiveEquipmentCodes = new List<int>();
    protected List<int> TierSixEquipmnetCodes = new List<int>();

    protected List<int> EventEquipmentCodes = new List<int>();

    protected Dictionary<int, EquipIncreaseSO> EquipIncreaseState = new Dictionary<int, EquipIncreaseSO>();
    protected Dictionary<int, EquipSlotSO> EquipSlot = new Dictionary<int, EquipSlotSO>();
    protected Dictionary<int, PlayerEquipDetailSO> EquipDetail = new Dictionary<int, PlayerEquipDetailSO>();
    protected Dictionary<int, EquipSpriteSO> EquipSprite = new Dictionary<int, EquipSpriteSO>();

    protected string[] PlayerEquipTierName = new string[7]
    { "시작의", "저품질의", "평범한", "고품질의", "강화된", "마법의", "영원한" };
    protected string[] PlayerEquipMultiTypeName = new string[3]
    { "안정적인","균형적인","폭발적인" };
    protected int[] BootsBuffInt = new int[7]
    { 2,40,2,1,1,1,2 };
    protected int[] AccessoriesBuffInt = new int[7]
    { 3,3,2,3,0,3,2 };
    //95 5 0 0 0
    protected int[,] EquipmentGamblingDetail =
    {{95,5,0,0,0 }, //0레벨
        { 89,11,0,0,0}, //1레벨
        { 80,19,1,0,0}, //2레벨
        { 68,27,5,0,0}, //3레벨
        { 53,36,11,0,0}, //4레벨
        { 35,44,20,1,0}, //5레벨
        { 14,54,27,5,0}, //6레벨
        { 0,53,36,11,0}, //7레벨
        { 0,35,44,20,1}, //8레벨
        { 0,14,54,27,5}, //9레벨
        { 0,0,53,36,11} };//10레벨

    protected int[] GamblingLevelUpCost = new int[10] { 30, 45, 70, 105, 160, 240, 360, 540, 810, 1215 };
    protected int[] GamblingGachaCost = new int[11] { 1, 2, 3, 5, 8, 12, 18, 27, 40, 60, 90 };
    protected override void Awake()
    {
        base.Awake();
        InitEquipmentStorage();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitEquipmentStorage()
    {
        //InitEquipmentSlotSprite
        for(int i = 0; i < EquipmentSlotSprites.Length; i++)
        {
            if (!EquipmentSlotSpriteStorage.ContainsKey(EquipmentSlotIDs[i]))
            {
                EquipmentSlotSpriteStorage.Add(EquipmentSlotIDs[i], EquipmentSlotSprites[i]);
            }
        }
        //InitPlayerWeapon
        for(int i = 0; i < PlayerWeaponSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerWeaponSOs[i].EquipmentType * 10000) + (PlayerWeaponSOs[i].EquipmentTier * 1000) + PlayerWeaponSOs[i].EquipmentCode;
            InitTierEquipmnetCode(i_EquipmentCode, PlayerWeaponSOs[i].EquipmentTier);
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerWeaponSOs[i]);
            }
        }
        //InitPlayerArmor
        for (int i = 0; i < PlayerArmorSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerArmorSOs[i].EquipmentType * 10000) + (PlayerArmorSOs[i].EquipmentTier * 1000) + PlayerArmorSOs[i].EquipmentCode;
            InitTierEquipmnetCode(i_EquipmentCode, PlayerArmorSOs[i].EquipmentTier);
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerArmorSOs[i]);
            }
        }
        //InitPlayerHelmet
        for (int i = 0; i < PlayerHelmetSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerHelmetSOs[i].EquipmentType * 10000) + (PlayerHelmetSOs[i].EquipmentTier * 1000) + PlayerHelmetSOs[i].EquipmentCode;
            InitTierEquipmnetCode(i_EquipmentCode, PlayerHelmetSOs[i].EquipmentTier);
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerHelmetSOs[i]);
            }
        }
        //InitPlayerBoots
        for (int i = 0; i < PlayerBootsSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerBootsSOs[i].EquipmentType * 10000) + (PlayerBootsSOs[i].EquipmentTier * 1000) + PlayerBootsSOs[i].EquipmentCode;
            InitTierEquipmnetCode(i_EquipmentCode, PlayerBootsSOs[i].EquipmentTier);
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerBootsSOs[i]);
            }
        }
        //InitPlayerAccessories
        for (int i = 0; i < PlayerAccessoriesSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerAccessoriesSOs[i].EquipmentType * 10000) + (PlayerAccessoriesSOs[i].EquipmentTier * 1000) + PlayerAccessoriesSOs[i].EquipmentCode;
            InitTierEquipmnetCode(i_EquipmentCode, PlayerAccessoriesSOs[i].EquipmentTier);
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerAccessoriesSOs[i]);
            }
        }
        //-------------------------------------------------------------
        //InitPlayerIncreaseState
        for(int i = 0; i < PlayerEquipIncreaseSOs.Length; i++)
        {
            int SaveCode = (PlayerEquipIncreaseSOs[i].EquipStateType * 10) + PlayerEquipIncreaseSOs[i].EquipType;
            if(!EquipIncreaseState.ContainsKey(SaveCode))
            {
                EquipIncreaseState.Add(SaveCode, PlayerEquipIncreaseSOs[i]);
            }
        }
        //InitPlayerEquipSlot
        for(int i = 0; i < PlayerEquipSlotSos.Length; i++)
        {
            int SaveCode = (PlayerEquipSlotSos[i].EquipTier * 10) + PlayerEquipSlotSos[i].EquipMultiType;
            if(!EquipSlot.ContainsKey(SaveCode))
            {
                EquipSlot.Add(SaveCode, PlayerEquipSlotSos[i]);
            }
        }
        //InitPlayerEquipDetail
        for(int i = 0; i < PlayerEquipDetailSOs.Length; i++)
        {
            int SaveCode = (PlayerEquipDetailSOs[i].EquipStateType * 10) + PlayerEquipDetailSOs[i].EquipType;
            if(!EquipDetail.ContainsKey(SaveCode))
            {
                EquipDetail.Add(SaveCode, PlayerEquipDetailSOs[i]);
            }
        }
        //InitPlayerEquipSprite
        for(int i = 0; i < PlayerEquipSpriteSOs.Length; i++)
        {
            int SaveCode = (PlayerEquipSpriteSOs[i].EqupTier * 100) + (PlayerEquipSpriteSOs[i].EquipStateType * 10) + PlayerEquipSpriteSOs[i].EquipType;
            if(!EquipSprite.ContainsKey(SaveCode))
            {
                EquipSprite.Add(SaveCode, PlayerEquipSpriteSOs[i]);
            }
        }
        //--------------------------------------------------------
        //InitMonWeapon
        for (int i = 0; i < MonWeaponSOs.Length; i++)
        {
            int i_EquipmentCode = MonWeaponSOs[i].EquipmentCode;
            if (!MonEquipmentStroage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                MonEquipmentStroage.Add(i_EquipmentCode, MonWeaponSOs[i]);
            }
        }
        //IntiMonArmor
        for (int i = 0; i < MonArmorSOs.Length; i++)
        {
            int i_EquipmentCode = MonArmorSOs[i].EquipmentCode;
            if (!MonEquipmentStroage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                MonEquipmentStroage.Add(i_EquipmentCode, MonArmorSOs[i]);
            }
        }
        //InitMonAnotherEquip
        for (int i = 0; i < MonAnotherEquipSOs.Length; i++)
        {
            int i_EquipmentCode = MonAnotherEquipSOs[i].EquipmentCode;
            if (!MonEquipmentStroage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                MonEquipmentStroage.Add(i_EquipmentCode, MonAnotherEquipSOs[i]);
            }
        }
    }

    public void InitTierEquipmnetCode(int EquipmentCode , int EquipmentTier)
    {
        switch(EquipmentTier)
        {
            case 0:
                TierZeroEquipmentCodes.Add(EquipmentCode);
                break;
            case 1:
                TierOneEquipmentCodes.Add(EquipmentCode);
                break;
            case 2:
                TierTwoEquipmentCodes.Add(EquipmentCode);
                break;
            case 3:
                TierThreeEquipmentCodes.Add(EquipmentCode);
                break;
            case 4:
                TierFourEquipmentCodes.Add(EquipmentCode);
                break;
            case 5:
                TierFiveEquipmentCodes.Add(EquipmentCode);
                break;
            case 6:
                TierSixEquipmnetCodes.Add(EquipmentCode);
                break;
            case 7:
                EventEquipmentCodes.Add(EquipmentTier);
                break;
        }
    }
    public Sprite GetEquipmentSlotSprite(float SlotAmount)
    {
        if(!EquipmentSlotSpriteStorage.ContainsKey(SlotAmount))//없다면
        {
            return EquipmentSlotSpriteStorage[-1];
        }
        return EquipmentSlotSpriteStorage[SlotAmount];
    }

    protected EquipIncreaseSO GetEquipIncreseInfo(int StateType, int EquipType)
    {
        int NeededCode = (StateType * 10) + EquipType;
        return EquipIncreaseState[NeededCode];
    }

    protected EquipSlotSO GetEquipSlotInfo(int EquipTier, int MultiType)
    {
        int NeededCode = (EquipTier * 10) + MultiType;
        return EquipSlot[NeededCode];
    }

    protected PlayerEquipDetailSO GetEquipDetailInfo(int StateType, int EquipType)
    {
        int NeededCode = (StateType * 10) + EquipType;
        return EquipDetail[NeededCode];
    }

    protected EquipSpriteSO GetEquipSpriteInfo(int EquipTier, int StateType, int EquipType)
    {
        int NeededCode = (EquipTier * 100) + (StateType * 10) + EquipType;
        return EquipSprite[NeededCode];
    }

    public EquipmentSO GetPlayerEquipmentInfo(int EquipCode)//이것만 어떻게 바꾸면 될듯?
    {
        //여기에서 EquipmentCode가 들어오는 정보대로 EquipmentSO를 맞춰서 retrun 해주면 될듯함

        //IncreaseState => StateType * 10 + EquipType
        //PlayerEqupSlot => EquipTier * 10 + EquipMultiType
        //PlayerEquipDetailSo => StateType * 10 + EquipType
        //EquipSprite => EquipTier * 100 + StateType * 10 + EquipType
        /*
        public int EquipmentType;//장비의 종류 -> 십의 자리                          //
        public int EquipmentTier;//장비의 티어 -> 천의 자리                          //
        public int EquipmentCode;//장비코드? 이건 굳이? ->이건 0으로            //
        public string EquipmentName;//장비 이름 = 장비 티어 종류 + 곱 성향 + 장비이름-> PlayerEquipDetailSo에서        //
        public float SpendTiredness;//사용하는 피로도, 주어진 정보에 티어 곱하기, (0티어 제외)-> EqupIncreaseSO에서     //
        public EquipmentSlot[] EquipmentSlots; // 슬롯 ->EqupSlotSo에서                 //
        public Sprite EquipmentImage;//장비 이미지 -> EquipSpriteSo에서                //
        public int AddSTRAmount;//변화하는 힘 수치 , 주어진 정보에 티어 곱하기 스탯 관련은 다 똑같이->EqupIncreaseSO에서
        public int AddDURAmount;//변화하는 내구 수치 ->EqupIncreaseSO에서
        public int AddRESAmount;//변화하는 회복 수치 ->EqupIncreaseSO에서
        public int AddSPDAmount;//변화하는 속도 수치 ->EqupIncreaseSO에서
        public int AddLUKAmount;//변화하는 행운 수치 ->EqupIncreaseSO에서
        public float AddHPAmount;//변화하는 체력 수치 -> 0으로
        public float AddTirednessAmount;//변화하는 피로도 수치 -> 0으로
        [TextArea(10, 20)]
        public string EquipmentDetail;//장비 상세 설명 -> EquipDetailSo에서
         * */
        int PlayerEquipTier = (EquipCode / 1000);
        int PlayerEquipStateType = (EquipCode / 100) % 10;
        int PlayerEquipType = (EquipCode / 10) % 10;
        int PlayerEquipMultiType = (EquipCode % 10);

        EquipmentSO AssembleEquip = new EquipmentSO();
        //(number / (int)Mathf.Pow(10, position)) % 10;
        AssembleEquip.EquipmentType = ((EquipCode / 10) % 10);
        AssembleEquip.EquipmentTier = (EquipCode / 1000);
        AssembleEquip.EquipmentCode = 0;
        AssembleEquip.EquipmentName = PlayerEquipTierName[PlayerEquipTier] + " " + PlayerEquipMultiTypeName[PlayerEquipMultiType] + " " +
            GetEquipDetailInfo(PlayerEquipStateType, PlayerEquipType).EquipmentName;
        if (PlayerEquipTier < 1)//0티어
        {
            AssembleEquip.SpendTiredness = GetEquipIncreseInfo(PlayerEquipStateType, PlayerEquipType).SpendTired;
            AssembleEquip.AddSTRAmount = 0;
            AssembleEquip.AddDURAmount = 0;
            AssembleEquip.AddRESAmount = 0;
            AssembleEquip.AddSPDAmount = 0;
            AssembleEquip.AddLUKAmount = 0;
        }
        else
        {
            AssembleEquip.SpendTiredness = (int)(GetEquipIncreseInfo(PlayerEquipStateType, PlayerEquipType).SpendTired * PlayerEquipTier);
            AssembleEquip.AddSTRAmount = (GetEquipIncreseInfo(PlayerEquipStateType, PlayerEquipType).IncreaseSTR * PlayerEquipTier);
            AssembleEquip.AddDURAmount = (GetEquipIncreseInfo(PlayerEquipStateType, PlayerEquipType).IncreaseDUR * PlayerEquipTier);
            AssembleEquip.AddRESAmount = (GetEquipIncreseInfo(PlayerEquipStateType, PlayerEquipType).IncreaseRES * PlayerEquipTier);
            AssembleEquip.AddSPDAmount = (GetEquipIncreseInfo(PlayerEquipStateType, PlayerEquipType).IncreaseSPD * PlayerEquipTier);
            AssembleEquip.AddLUKAmount = (GetEquipIncreseInfo(PlayerEquipStateType, PlayerEquipType).IncreaseLUK * PlayerEquipTier);
        }
        AssembleEquip.EquipmentSlots = GetEquipSlotInfo(PlayerEquipTier, PlayerEquipMultiType).EquipmentSlots;
        AssembleEquip.EquipmentImage = GetEquipSpriteInfo(PlayerEquipTier, PlayerEquipStateType, PlayerEquipType).EquipSprite;
        AssembleEquip.AddHPAmount = 0;
        AssembleEquip.AddTirednessAmount = 0;
        string BeforeDetailString = GetEquipDetailInfo(PlayerEquipStateType, PlayerEquipType).EquipmentDetail;
        //AssembleEquip.EquipmentDetail = BeforeDetailString.Replace()
        switch(PlayerEquipStateType)
        {
            case (int)EEquipStateType.StateSTR:
                int STRB = BootsBuffInt[(int)EEquipStateType.StateSTR] * PlayerEquipTier;
                int STRA = AccessoriesBuffInt[(int)EEquipStateType.StateSTR] * PlayerEquipTier;
                AssembleEquip.EquipmentDetail = BeforeDetailString.Replace("{STRB}", STRB.ToString()).Replace("{STRA}", STRA.ToString());
                break;
            case (int)EEquipStateType.StateDUR:
                int DURB = BootsBuffInt[(int)EEquipStateType.StateDUR] * PlayerEquipTier;
                int DURA = AccessoriesBuffInt[(int)EEquipStateType.StateDUR] * PlayerEquipTier;
                AssembleEquip.EquipmentDetail = BeforeDetailString.Replace("{DURB}", DURB.ToString()).Replace("{DURA}", DURA.ToString());
                break;
            case (int)EEquipStateType.StateRES:
                int RESB = BootsBuffInt[(int)EEquipStateType.StateRES] * PlayerEquipTier;
                int RESA = AccessoriesBuffInt[(int)EEquipStateType.StateRES] * PlayerEquipTier;
                AssembleEquip.EquipmentDetail = BeforeDetailString.Replace("{RESB}", RESB.ToString()).Replace("{RESA}", RESA.ToString());
                break;
            case (int)EEquipStateType.StateSPD:
                int SPDB = BootsBuffInt[(int)EEquipStateType.StateSPD] * PlayerEquipTier;
                int SPDA = AccessoriesBuffInt[(int)EEquipStateType.StateSPD] * PlayerEquipTier;
                AssembleEquip.EquipmentDetail = BeforeDetailString.Replace("{SPDB}", SPDB.ToString()).Replace("{SPDA}", SPDA.ToString());
                break;
            case (int)EEquipStateType.StateLUK:
                int LUKB = BootsBuffInt[(int)EEquipStateType.StateLUK] * PlayerEquipTier;
                AssembleEquip.EquipmentDetail = BeforeDetailString.Replace("{LUKB}", LUKB.ToString());
                break;
            case (int)EEquipStateType.StateHP:
                int HPB = BootsBuffInt[(int)EEquipStateType.StateHP] * PlayerEquipTier;
                int HPA = AccessoriesBuffInt[(int)EEquipStateType.StateHP] * PlayerEquipTier;
                AssembleEquip.EquipmentDetail = BeforeDetailString.Replace("{HPB}", HPB.ToString()).Replace("{HPA}", HPA.ToString());
                break;
            case (int)EEquipStateType.StateSTA:
                int STAB = BootsBuffInt[(int)EEquipStateType.StateSTA] * PlayerEquipTier;
                int STAA = AccessoriesBuffInt[(int)EEquipStateType.StateSTA] * PlayerEquipTier;
                AssembleEquip.EquipmentDetail = BeforeDetailString.Replace("{STAB}", STAB.ToString()).Replace("{STAA}", STAA.ToString());
                break;
            case (int)EEquipStateType.StateNormal:
            case (int)EEquipStateType.StateStart:
                AssembleEquip.EquipmentDetail = BeforeDetailString;
                break;
        }

        return AssembleEquip;
        /*
        if (!EquipmentStorage.ContainsKey(EquipCode))//EquipmentStorage에 없으면 0번 장비(아무것도 아닌것)전달
        {
            return EquipmentStorage[0];
        }
        return EquipmentStorage[EquipCode];
        */
    }

    public EquipmentSO GetMonEquipmentInfo(int EquipmentCode)
    {
        if (!MonEquipmentStroage.ContainsKey(EquipmentCode))//MonEquipmentStroage에 없으면 0번 장비(아무것도 아닌것)전달
        {
            return MonEquipmentStroage[0];
        }
        return MonEquipmentStroage[EquipmentCode];
    }

    public int GetFixedTierRandomEquipmnet(int Tier)//정해진 티어에서 랜덤한 장비 리턴
    {
        int RandNum = 0;
        switch(Tier)
        {
            case 0://0티어 장비
                RandNum = Random.Range(0, TierZeroEquipmentCodes.Count);
                return TierZeroEquipmentCodes[RandNum];
            case 1://1티어 장비
                RandNum = Random.Range(0, TierOneEquipmentCodes.Count);
                return TierOneEquipmentCodes[RandNum];
            case 2://2티어 장비
                RandNum = Random.Range(0, TierTwoEquipmentCodes.Count);
                return TierTwoEquipmentCodes[RandNum];
            case 3://3티어 장비
                RandNum = Random.Range(0, TierThreeEquipmentCodes.Count);
                return TierThreeEquipmentCodes[RandNum];
            case 4://4티어 장비
                RandNum = Random.Range(0, TierFourEquipmentCodes.Count);
                return TierFourEquipmentCodes[RandNum];
            case 5://5티어 장비
                RandNum = Random.Range(0, TierFiveEquipmentCodes.Count);
                return TierFiveEquipmentCodes[RandNum];
            case 6://6티어 장비
                RandNum = Random.Range(0, TierSixEquipmnetCodes.Count);
                return TierSixEquipmnetCodes[RandNum];
            case 7://이벤트 장비
                RandNum = Random.Range(0, EventEquipmentCodes.Count);
                return EventEquipmentCodes[RandNum];
        }
        return 0;
    }

    public int GetFixedTierNTypeRandomEquipment(int Tier, EEquipmentType Type)
    {
        List<int> ApplyEquipment = new List<int>();
        int TypeNum = 0;
        switch(Type)
        {
            case EEquipmentType.Weapon:
                TypeNum = (int)EEquipmentType.Weapon;
                break;
            case EEquipmentType.Armor:
                TypeNum = (int)EEquipmentType.Armor;
                break;
            case EEquipmentType.Helmet:
                TypeNum = (int)EEquipmentType.Helmet;
                break;
            case EEquipmentType.Boots:
                TypeNum = (int)EEquipmentType.Boots;
                break;
            case EEquipmentType.Accessories:
                TypeNum = (int)EEquipmentType.Accessories;
                break;
        }
        switch(Tier)
        {
            case 0://0티어
                for(int i = 0; i < TierZeroEquipmentCodes.Count; i++)
                {
                    if ((TierZeroEquipmentCodes[i] / 10000) == TypeNum)
                        ApplyEquipment.Add(TierZeroEquipmentCodes[i]);
                }
                break;
            case 1://1티어
                for (int i = 0; i < TierOneEquipmentCodes.Count; i++)
                {
                    if ((TierOneEquipmentCodes[i] / 10000) == TypeNum)
                        ApplyEquipment.Add(TierOneEquipmentCodes[i]);
                }
                break;
            case 2://2티어
                for (int i = 0; i < TierTwoEquipmentCodes.Count; i++)
                {
                    if ((TierTwoEquipmentCodes[i] / 10000) == TypeNum)
                        ApplyEquipment.Add(TierTwoEquipmentCodes[i]);
                }
                break;
            case 3://3티어
                for (int i = 0; i < TierThreeEquipmentCodes.Count; i++)
                {
                    if ((TierThreeEquipmentCodes[i] / 10000) == TypeNum)
                        ApplyEquipment.Add(TierThreeEquipmentCodes[i]);
                }
                break;
            case 4://4티어
                for (int i = 0; i < TierFourEquipmentCodes.Count; i++)
                {
                    if ((TierFourEquipmentCodes[i] / 10000) == TypeNum)
                        ApplyEquipment.Add(TierFourEquipmentCodes[i]);
                }
                break;
            case 5://5티어
                for (int i = 0; i < TierFiveEquipmentCodes.Count; i++)
                {
                    if ((TierFiveEquipmentCodes[i] / 10000) == TypeNum)
                        ApplyEquipment.Add(TierFiveEquipmentCodes[i]);
                }
                break;
            case 6://6티어
                for (int i = 0; i < TierSixEquipmnetCodes.Count; i++)
                {
                    if ((TierSixEquipmnetCodes[i] / 10000) == TypeNum)
                        ApplyEquipment.Add(TierSixEquipmnetCodes[i]);
                }
                break;
            case 7://이벤트 장비
                for (int i = 0; i < EventEquipmentCodes.Count; i++)
                {
                    if ((EventEquipmentCodes[i] / 10000) == TypeNum)
                        ApplyEquipment.Add(EventEquipmentCodes[i]);
                }
                break;
        }
        if(ApplyEquipment.Count == 0)
            return 0;
        else
        {
            int RandNum = Random.Range(0, ApplyEquipment.Count);
            return ApplyEquipment[RandNum];
        }
    }

    public int GetGamblingEquipmentCode(int GamblingLevel)//플레이어의 겜블링 레벨에 맞춰서 랜덤한 장비의 코드를 리턴함//이게 보통 장비를 얻는것임
    {//그러니까 여기서 Early강화에 따른 1티어 상승된 장비의 획득이 확률적으로 일어나야함
        int RandNum = Random.Range(0, 100);//0부터 99까지 하나가 나옴
        int TierOneNum = EquipmentGamblingDetail[GamblingLevel, 0];//0 <= RandNum < TierOneNum 까지 걸리면 1티어 장비
        int TierTwoNum = EquipmentGamblingDetail[GamblingLevel, 1] + TierOneNum;//TierOneNum <= RandNum < TierTwoNum 까지 걸리면 2티어//나머지 동일
        int TierThreeNum = EquipmentGamblingDetail[GamblingLevel, 2] + TierTwoNum;
        int TierFourNum = EquipmentGamblingDetail[GamblingLevel, 3] + TierThreeNum;
        int TierFiveNum = EquipmentGamblingDetail[GamblingLevel, 4] + TierFourNum;

        //만약 JsonReadWriteManager의 E_info.Luk의 레벨이 7이상이라면 5퍼센트 확률로 발생
        if (JsonReadWriteManager.Instance.E_Info.EarlyLuckLevel >= 7)
        {
            int RandLuk = Random.Range(0, 99);
            if(RandLuk < 5)//장비 1티어 상승 당첨 -> 1티어라면 2티어로....5티어 라면 6티어로
            {
                if (RandNum < TierOneNum)//1티어는 2티어로
                    RandNum = TierOneNum + 1;
                else if (RandNum >= TierOneNum && RandNum < TierTwoNum)//2티어는 3티어로
                    RandNum = TierTwoNum + 1;
                else if (RandNum >= TierTwoNum && RandNum < TierThreeNum)//3티어는 4티어로
                    RandNum = TierThreeNum + 1;
                else if (RandNum >= TierThreeNum && RandNum < TierFourNum)//4티어는 5티어로
                    RandNum = TierFourNum + 1;
                else if (RandNum >= TierFourNum && RandNum < TierFiveNum)//5티어는 6티어로
                    RandNum = TierFiveNum + 1;
            }
        }

        List<int> ApplicableEquipmentCode = new List<int>();
        //장비의 티어는 0X000의 X에 따라 결정된다. 
        //(12345 / 1000) = 12 -> 12 % 10 = 2 -> 2티어 장비인거임 //티어에 따른 저장도 따로 해놓을까? 그게 나을수도 있을것 같은데?
        if (RandNum < TierOneNum)//여기에 들어가면 1티어 장비
        {
            int RandTierNum = Random.Range(0, TierOneEquipmentCodes.Count);//티어가 1인 장비의 코드중 하나
            return TierOneEquipmentCodes[RandTierNum];//티어가 1인 장비의 코드중 하나를 리턴함
        }
        else if(RandNum >= TierOneNum && RandNum < TierTwoNum)
        {
            int RandTierNum = Random.Range(0, TierTwoEquipmentCodes.Count);
            return TierTwoEquipmentCodes[RandTierNum];//티어가 2인 장비의 코드중 하나를 리턴함
        }
        else if(RandNum >= TierTwoNum && RandNum < TierThreeNum)
        {
            int RandTierNum = Random.Range(0, TierThreeEquipmentCodes.Count);
            return TierThreeEquipmentCodes[RandTierNum];//티어가 3인 장비의 코드중 하나를 리턴함
        }
        else if(RandNum >= TierThreeNum && RandNum < TierFourNum)
        {
            int RandTierNum = Random.Range(0, TierFourEquipmentCodes.Count);
            return TierFourEquipmentCodes[RandTierNum];//티어가 4인 장비의 코드중 하나를 리턴함
        }
        else if(RandNum >= TierFourNum && RandNum < TierFiveNum)
        {
            int RandTierNum = Random.Range(0, TierFiveEquipmentCodes.Count);
            return TierFiveEquipmentCodes[RandTierNum];
        }
        else if(RandNum >= TierFiveNum)//티어 6
        {
            int RandTierNum = Random.Range(0, TierSixEquipmnetCodes.Count);
            return TierSixEquipmnetCodes[RandTierNum];
        }
        return 0;
    }

    public int GetGamblingLevelPercent(int Level, int Tier)
    {
        return EquipmentGamblingDetail[Level, Tier];
    }

    public int GetGamblingLevelUPCost(int Level)
    {
        if(Level >= GamblingLevelUpCost.Length)
        {
            return 0;
        }
        return GamblingLevelUpCost[Level];
    }

    public int GetGamblingGachaCost(int Level)
    {
        if (Level >= GamblingGachaCost.Length)
        {
            return 0;
        }
        return GamblingGachaCost[Level];
    }
}
