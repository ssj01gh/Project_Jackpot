using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Burst;
using UnityEngine;
using UnityEngine.AI;

public enum EEquipState
{
    StateSTR,
    StateDUR,
    StateRES,
    StateSPD,
    StateLUK
}

public enum EIsEventEquip
{
    NormalEquip = 1,
    EventEquip
}
public enum EEquipTier
{
    TierZero,
    TierOne,
    TierTwo,
    TierThree,
    TierFour,
    TierFive,
    TierSix
}
public enum EEquipStateType
{
    StateSTR,
    StateDUR,
    StateRES,
    StateSPD,
    StateLUK,
    StateHP,
    StateSTA,
    StateNormal,
    StateStart
}
public enum EEquipType
{
    TypeWeapon,
    TypeArmor,
    TypeHelmet,
    TypeBoots,
    TypeAcc,
}
public enum EEquipMultiType
{
    MultiNull,
    MultiSteady,
    MultiBalanced,
    MultiVolatile,
}
public class EquipmentInfoManager : MonoSingleton<EquipmentInfoManager>
{
    [Header("EquipmentSlotSprite")]
    public float[] EquipmentSlotIDs;
    public Sprite[] EquipmentSlotSprites;
    [Header("PlayerEquipIncreaseState")]
    public EquipIncreaseSO[] PlayerEquipIncreaseSOs;
    [Header("PlayerSlots")]
    public EquipSlotSO[] PlayerEquipSlotSos;
    [Header("PlayerEquipDetails")]
    public PlayerEquipDetailSO[] PlayerEquipDetailSOs;
    [Header("PlayerEquipSprite")]
    public EquipSpriteSO[] PlayerEquipSpriteSOs;
    [Header("PlayerEventEquip")]
    public EquipmentSO[] PlayerEventEquipSOs;

    [Header("MonWeapon")]
    public EquipmentSO[] MonWeaponSOs;
    [Header("MonArmor")]
    public EquipmentSO[] MonArmorSOs;
    [Header("MonAnotherEquip")]
    public EquipmentSO[] MonAnotherEquipSOs;

    protected Dictionary<float, Sprite> EquipmentSlotSpriteStorage = new Dictionary<float, Sprite>();
    protected Dictionary<int, EquipmentSO> EquipmentStorage = new Dictionary<int, EquipmentSO>();
    protected Dictionary<int, EquipmentSO> MonEquipmentStorage = new Dictionary<int, EquipmentSO>();

    protected Dictionary<int, EquipIncreaseSO> EquipIncreaseState = new Dictionary<int, EquipIncreaseSO>();
    protected Dictionary<int, EquipSlotSO> EquipSlot = new Dictionary<int, EquipSlotSO>();
    protected Dictionary<int, PlayerEquipDetailSO> EquipDetail = new Dictionary<int, PlayerEquipDetailSO>();
    protected Dictionary<int, EquipSpriteSO> EquipSprite = new Dictionary<int, EquipSpriteSO>();
    protected Dictionary<int, EquipmentSO> PlayerEventEquip = new Dictionary<int, EquipmentSO>();

    protected string[] PlayerEquipTierName = new string[7]
    { "시작의 ", "저품질의 ", "평범한 ", "고품질의 ", "강화된 ", "마법의 ", "영원한 " };
    protected string[] PlayerEquipMultiTypeName = new string[4]
    { "","안정적인 ","균형적인 ","폭발적인 " };
    protected int[] BootsBuffInt = new int[7]
    { 2,40,2,1,1,1,2 };
    protected int[] AccessoriesBuffInt = new int[7]
    { 3,3,2,3,0,3,2 };
    //95 5 0 0 0
    protected int[,] EquipmentGamblingDetail =
    {
        { 95,5,0,0,0,0 }, //0레벨
        { 89,11,0,0,0,0 }, //1레벨
        { 80,19,1,0,0,0 }, //2레벨
        { 68,27,5,0,0,0 }, //3레벨
        { 53,36,11,0,0,0 }, //4레벨
        { 35,44,20,1,0,0 }, //5레벨
        { 14,54,27,5,0,0 }, //6레벨
        { 0,53,36,11,0,0 }, //7레벨
        { 0,35,44,20,1,0 }, //8레벨
        { 0,14,54,27,5,0 }, //9레벨
        { 0,0,53,36,11,0 }//10레벨
    };

    protected int[,] EquipmentGamblingDetail_Luck =
    {
        { 66,32,2,0,0,0 }, //0레벨
        { 63,34,3,0,0,0 }, //1레벨
        { 57,37,6,0,0,0 }, //2레벨
        { 47,39,12,2,0,0 }, //3레벨
        { 37,41,19,3,0,0 }, //4레벨
        { 25,41,27,7,0,0 }, //5레벨
        { 9,42,35,12,2,0 }, //6레벨
        { 0,37,41,19,3,0 }, //7레벨
        { 0,25,41,27,7,0 }, //8레벨
        { 0,9,42,35,12,2 }, //9레벨
        { 0,0,37,41,19,3 }//10레벨
    };

    protected int[] GamblingLevelUpCost = new int[10] { 30, 45, 70, 105, 160, 240, 360, 540, 810, 1215 };
    protected int[] GamblingGachaCost = new int[11] { 1, 2, 3, 5, 8, 12, 18, 27, 40, 60, 90 };

    protected static int IsEventEquipMultipleNum = 10000;
    protected static int TierMultipleNum = 1000;
    protected static int StateTypeMultipleNum = 100;
    protected static int TypeMultipleNum = 10;

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
        //-------------------------------------------------------------
        //InitPlayerIncreaseState
        for(int i = 0; i < PlayerEquipIncreaseSOs.Length; i++)
        {
            int SaveCode = (PlayerEquipIncreaseSOs[i].IsEventEquip * IsEventEquipMultipleNum) + 
                (PlayerEquipIncreaseSOs[i].EquipStateType * StateTypeMultipleNum) + (PlayerEquipIncreaseSOs[i].EquipType * TypeMultipleNum);
            if(!EquipIncreaseState.ContainsKey(SaveCode))
            {
                EquipIncreaseState.Add(SaveCode, PlayerEquipIncreaseSOs[i]);
            }
        }
        //InitPlayerEquipSlot
        for(int i = 0; i < PlayerEquipSlotSos.Length; i++)
        {
            int SaveCode = (PlayerEquipSlotSos[i].IsEventEquip * IsEventEquipMultipleNum) + 
                (PlayerEquipSlotSos[i].EquipTier * TierMultipleNum) + PlayerEquipSlotSos[i].EquipMultiType;
            if(!EquipSlot.ContainsKey(SaveCode))
            {
                EquipSlot.Add(SaveCode, PlayerEquipSlotSos[i]);
            }
        }
        //InitPlayerEquipDetail
        for(int i = 0; i < PlayerEquipDetailSOs.Length; i++)
        {
            int SaveCode = (PlayerEquipDetailSOs[i].IsEventEquip * IsEventEquipMultipleNum) + 
                (PlayerEquipDetailSOs[i].EquipStateType * StateTypeMultipleNum) + (PlayerEquipDetailSOs[i].EquipType * TypeMultipleNum);
            if(!EquipDetail.ContainsKey(SaveCode))
            {
                EquipDetail.Add(SaveCode, PlayerEquipDetailSOs[i]);
            }
        }
        //InitPlayerEquipSprite
        for(int i = 0; i < PlayerEquipSpriteSOs.Length; i++)
        {
            int SaveCode = (PlayerEquipSpriteSOs[i].IsEventEquip * IsEventEquipMultipleNum) + 
                (PlayerEquipSpriteSOs[i].EqupTier * TierMultipleNum) + (PlayerEquipSpriteSOs[i].EquipStateType * StateTypeMultipleNum) + 
                (PlayerEquipSpriteSOs[i].EquipType * TypeMultipleNum);
            if(!EquipSprite.ContainsKey(SaveCode))
            {
                EquipSprite.Add(SaveCode, PlayerEquipSpriteSOs[i]);
            }
        }
        for(int i = 0; i < PlayerEventEquipSOs.Length; i++)
        {
            int SaveCode = (PlayerEventEquipSOs[i].EquipmentType * IsEventEquipMultipleNum) +
                (PlayerEventEquipSOs[i].EquipmentTier * TierMultipleNum) + PlayerEventEquipSOs[i].EquipmentCode;
            if(!PlayerEventEquip.ContainsKey(SaveCode))
            {
                PlayerEventEquip.Add(SaveCode, PlayerEventEquipSOs[i]);
            }
        }
        //--------------------------------------------------------
        //InitMonWeapon
        for (int i = 0; i < MonWeaponSOs.Length; i++)
        {
            int i_EquipmentCode = MonWeaponSOs[i].EquipmentCode;
            if (!MonEquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                MonEquipmentStorage.Add(i_EquipmentCode, MonWeaponSOs[i]);
            }
        }
        //IntiMonArmor
        for (int i = 0; i < MonArmorSOs.Length; i++)
        {
            int i_EquipmentCode = MonArmorSOs[i].EquipmentCode;
            if (!MonEquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                MonEquipmentStorage.Add(i_EquipmentCode, MonArmorSOs[i]);
            }
        }
        //InitMonAnotherEquip
        for (int i = 0; i < MonAnotherEquipSOs.Length; i++)
        {
            int i_EquipmentCode = MonAnotherEquipSOs[i].EquipmentCode;
            if (!MonEquipmentStorage.ContainsKey(i_EquipmentCode))//해당 코드의 장비가 없다면
            {
                MonEquipmentStorage.Add(i_EquipmentCode, MonAnotherEquipSOs[i]);
            }
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
    //----------------------
    protected EquipIncreaseSO GetEquipIncreseInfo(int EquipEventType, int StateType, int EquipType)
    {
        int NeededCode = (EquipEventType * IsEventEquipMultipleNum) + (StateType * StateTypeMultipleNum) + (EquipType * TypeMultipleNum);
        return EquipIncreaseState[NeededCode];
    }

    protected EquipSlotSO GetEquipSlotInfo(int EquipEventType, int EquipTier,   int EquipStateType, int EquipType, int MultiType)
    {
        int NeededCode = 0;

        if(EquipStateType == (int)EEquipStateType.StateLUK)
        {//럭일때
            //EquipType이 신발이나 장신구가 아니라면 NeedCode를 바꿈
            if(EquipType != (int)EEquipType.TypeBoots && EquipType != (int)EEquipType.TypeAcc)
            {
                if (EquipTier != (int)EEquipTier.TierZero)//0티어는 행운이 없음
                    NeededCode = (EquipEventType * IsEventEquipMultipleNum) + (EquipTier * TierMultipleNum) + 
                        MultiType + 3;
                else
                    NeededCode = (EquipEventType * IsEventEquipMultipleNum) + (EquipTier * TierMultipleNum) + MultiType;

                if (MultiType == 0)//0이 나올수 없지만 혹시나 0이 나온다면 안정 타입으로
                    NeededCode += 1;
            }
        }
        else//럭이 아닐때//피로도, 일반 장비의 경우 그냥 1티어 전꺼를 리턴함
        {//EquipType이 신발이나 장신구라면 NeedCode를 바꾸지 않음
            if (EquipType != (int)EEquipType.TypeBoots && EquipType != (int)EEquipType.TypeAcc)
            {
                if(EquipStateType == (int)EEquipStateType.StateSTA || EquipStateType == (int)EEquipStateType.StateNormal)
                    NeededCode = (EquipEventType * IsEventEquipMultipleNum) + (EquipTier * TierMultipleNum) + MultiType;
                else
                {
                    if(EquipTier >= 1)
                        NeededCode = (EquipEventType * IsEventEquipMultipleNum) + ((EquipTier - 1) * TierMultipleNum) + MultiType;
                    else
                        NeededCode = (EquipEventType * IsEventEquipMultipleNum) + (EquipTier * TierMultipleNum) + MultiType;
                }

                if (MultiType == 0)//0이 나올수 없지만 혹시나 0이 나온다면 안정 타입으로
                    NeededCode += 1;
            }
        }
        return EquipSlot[NeededCode];
    }

    protected PlayerEquipDetailSO GetEquipDetailInfo(int EquipEventType, int StateType, int EquipType)
    {
        int NeededCode = (EquipEventType * IsEventEquipMultipleNum) + (StateType * StateTypeMultipleNum) + (EquipType * TypeMultipleNum);
        return EquipDetail[NeededCode];
    }

    protected EquipSpriteSO GetEquipSpriteInfo(int EquipEventType, int EquipTier, int StateType, int EquipType)
    {
        int NeededCode = (EquipEventType * IsEventEquipMultipleNum) + (EquipTier * TierMultipleNum) + (StateType * StateTypeMultipleNum) + (EquipType * TypeMultipleNum);
        return EquipSprite[NeededCode];
    }
    public bool CheckIsCorrectEquipCode(int EquipCode)
    {
        int EquipEventType = (EquipCode / IsEventEquipMultipleNum);
        int EquipTier = (EquipCode / TierMultipleNum) % 10;
        int EquipStateType = (EquipCode / StateTypeMultipleNum) % 10;
        int EquipType = (EquipCode / TypeMultipleNum) % 10;
        int EquipMultiType = (EquipCode % 10);

        int IncreaseInfoNum = (EquipEventType * IsEventEquipMultipleNum) + (EquipStateType * StateTypeMultipleNum) + (EquipType * TypeMultipleNum);
        int NormalSlotInfoNum = (EquipEventType * IsEventEquipMultipleNum) + (EquipTier * TierMultipleNum) + EquipMultiType;
        int LuckSlotInfoNum = (EquipEventType * IsEventEquipMultipleNum) + (EquipTier * TierMultipleNum) + EquipMultiType + 3;
        int DetailNum = (EquipEventType * IsEventEquipMultipleNum) + (EquipStateType * StateTypeMultipleNum) + (EquipType * TypeMultipleNum);
        int SpriteNum = (EquipEventType * IsEventEquipMultipleNum) + (EquipTier * TierMultipleNum) + (EquipStateType * StateTypeMultipleNum) + (EquipType * TypeMultipleNum);
        int EventNum = EquipCode;

        if (EquipEventType == (int)EIsEventEquip.NormalEquip)
        {
            if (!EquipIncreaseState.ContainsKey(IncreaseInfoNum))//없으면
                return false;

            if (!EquipSlot.ContainsKey(NormalSlotInfoNum) && !EquipSlot.ContainsKey(LuckSlotInfoNum))
                return false;

            if (!EquipDetail.ContainsKey(DetailNum))
                return false;

            if (!EquipSprite.ContainsKey(SpriteNum))
                return false;
        }
        else if (EquipEventType == (int)EIsEventEquip.EventEquip)
        {
            if (!PlayerEventEquip.ContainsKey(EventNum))
            {
                return false;
            }
        }
        else
            return false;

         return true;
    }
    //------------------------------------
    public EquipmentInfo GetPlayerEquipmentInfo(int EquipCode)//이것만 어떻게 바꾸면 될듯?
    {
        //EquipCode가 0일때(비어있을때)예외 처리도 해야될것 같은데?



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

        //신발과 장신구의 경우 안정적인, 이런거 빼고, 슬롯도 빼고
        int PlayerEquipEventType = (EquipCode / IsEventEquipMultipleNum);
        int PlayerEquipTier = (EquipCode / TierMultipleNum) % 10;
        int PlayerEquipStateType = (EquipCode / StateTypeMultipleNum) % 10;
        int PlayerEquipType = (EquipCode / TypeMultipleNum) % 10;
        int PlayerEquipMultiType = (EquipCode % 10);
        EquipmentInfo AssembleEquip = new EquipmentInfo();
        //보관하고 있는 코드중 하나라도 false(있지 않음이 뜬다면)null을 리턴하게
        if (EquipCode == 0 || !CheckIsCorrectEquipCode(EquipCode))
        {//옳지 않은 코드가 들어오면 이걸 리턴하게 하고 싶은데....//둘중 하나라도 true면 empty 반환
            AssembleEquip.EquipmentType = PlayerEventEquip[0].EquipmentType;
            AssembleEquip.EquipmentTier = PlayerEventEquip[0].EquipmentTier;
            AssembleEquip.EquipmentCode = PlayerEventEquip[0].EquipmentCode;
            AssembleEquip.EquipmentName = PlayerEventEquip[0].EquipmentName;
            AssembleEquip.SpendTiredness = PlayerEventEquip[0].SpendTiredness;
            AssembleEquip.EquipmentSlots = PlayerEventEquip[0].EquipmentSlots;
            AssembleEquip.EquipmentImage = PlayerEventEquip[0].EquipmentImage;
            AssembleEquip.AddSTRAmount = PlayerEventEquip[0].AddSTRAmount;
            AssembleEquip.AddDURAmount = PlayerEventEquip[0].AddDURAmount;
            AssembleEquip.AddRESAmount = PlayerEventEquip[0].AddRESAmount;
            AssembleEquip.AddSPDAmount = PlayerEventEquip[0].AddSPDAmount;
            AssembleEquip.AddLUKAmount = PlayerEventEquip[0].AddLUKAmount;
            AssembleEquip.AddHPAmount = PlayerEventEquip[0].AddHPAmount;
            AssembleEquip.AddTirednessAmount = PlayerEventEquip[0].AddTirednessAmount;
            AssembleEquip.EquipmentDetail = PlayerEventEquip[0].EquipmentDetail;

            return AssembleEquip;
        }
        
        if (PlayerEquipEventType == (int)EIsEventEquip.EventEquip)
        {
            //여기서 이벤트 장비는 적절하게 가공을 해야할듯
            //Type하고 Tier는 꼭 맞게 들어가 있어야함
            AssembleEquip.EquipmentType = PlayerEquipType;
            AssembleEquip.EquipmentTier = PlayerEquipTier;
            AssembleEquip.EquipmentCode = 0;
            AssembleEquip.EquipmentName = PlayerEventEquip[EquipCode].EquipmentName;
            AssembleEquip.SpendTiredness = PlayerEventEquip[EquipCode].SpendTiredness;
            AssembleEquip.EquipmentSlots = PlayerEventEquip[EquipCode].EquipmentSlots;
            AssembleEquip.EquipmentImage = PlayerEventEquip[EquipCode].EquipmentImage;
            AssembleEquip.AddSTRAmount = PlayerEventEquip[EquipCode].AddSTRAmount;
            AssembleEquip.AddDURAmount = PlayerEventEquip[EquipCode].AddDURAmount;
            AssembleEquip.AddRESAmount = PlayerEventEquip[EquipCode].AddRESAmount;
            AssembleEquip.AddSPDAmount = PlayerEventEquip[EquipCode].AddSPDAmount;
            AssembleEquip.AddLUKAmount = PlayerEventEquip[EquipCode].AddLUKAmount;
            AssembleEquip.AddHPAmount = PlayerEventEquip[EquipCode].AddHPAmount;
            AssembleEquip.AddTirednessAmount = PlayerEventEquip[EquipCode].AddTirednessAmount;
            AssembleEquip.EquipmentDetail = PlayerEventEquip[EquipCode].EquipmentDetail;

            return AssembleEquip;
        }

        //(number / (int)Mathf.Pow(10, position)) % 10;
        AssembleEquip.EquipmentType = PlayerEquipType;
        AssembleEquip.EquipmentTier = PlayerEquipTier;
        AssembleEquip.EquipmentCode = 0;
        AssembleEquip.EquipmentName = PlayerEquipTierName[PlayerEquipTier] + PlayerEquipMultiTypeName[PlayerEquipMultiType] +
            GetEquipDetailInfo(PlayerEquipEventType, PlayerEquipStateType, PlayerEquipType).EquipmentName;

        if (PlayerEquipTier < 1)//0티어
        {
            AssembleEquip.SpendTiredness = GetEquipIncreseInfo(PlayerEquipEventType, PlayerEquipStateType, PlayerEquipType).SpendTired;
            AssembleEquip.AddSTRAmount = 0;
            AssembleEquip.AddDURAmount = 0;
            AssembleEquip.AddRESAmount = 0;
            AssembleEquip.AddSPDAmount = 0;
            AssembleEquip.AddLUKAmount = 0;
        }
        else
        {
            AssembleEquip.SpendTiredness = (int)(GetEquipIncreseInfo(PlayerEquipEventType, PlayerEquipStateType, PlayerEquipType).SpendTired * PlayerEquipTier);
            AssembleEquip.AddSTRAmount = (GetEquipIncreseInfo(PlayerEquipEventType, PlayerEquipStateType, PlayerEquipType).IncreaseSTR * PlayerEquipTier);
            AssembleEquip.AddDURAmount = (GetEquipIncreseInfo(PlayerEquipEventType, PlayerEquipStateType, PlayerEquipType).IncreaseDUR * PlayerEquipTier);
            AssembleEquip.AddRESAmount = (GetEquipIncreseInfo(PlayerEquipEventType, PlayerEquipStateType, PlayerEquipType).IncreaseRES * PlayerEquipTier);
            AssembleEquip.AddSPDAmount = (GetEquipIncreseInfo(PlayerEquipEventType, PlayerEquipStateType, PlayerEquipType).IncreaseSPD * PlayerEquipTier);
            AssembleEquip.AddLUKAmount = (GetEquipIncreseInfo(PlayerEquipEventType, PlayerEquipStateType, PlayerEquipType).IncreaseLUK * PlayerEquipTier);
        }
        AssembleEquip.EquipmentSlots = GetEquipSlotInfo(PlayerEquipEventType, PlayerEquipTier, PlayerEquipStateType, PlayerEquipType, PlayerEquipMultiType).EquipmentSlots;
        AssembleEquip.EquipmentImage = GetEquipSpriteInfo(PlayerEquipEventType, PlayerEquipTier, PlayerEquipStateType, PlayerEquipType).EquipSprite;
        AssembleEquip.AddHPAmount = 0;
        AssembleEquip.AddTirednessAmount = 0;
        string BeforeDetailString = GetEquipDetailInfo(PlayerEquipEventType, PlayerEquipStateType, PlayerEquipType).EquipmentDetail;
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
                int RESA = 15 - AccessoriesBuffInt[(int)EEquipStateType.StateRES] * PlayerEquipTier;
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

    public EquipmentInfo GetMonEquipmentInfo(int EquipmentCode)
    {
        EquipmentInfo AssembleEquip = new EquipmentInfo();

        if (!MonEquipmentStorage.ContainsKey(EquipmentCode))//MonEquipmentStroage에 없으면 0번 장비(아무것도 아닌것)전달
        {
            EquipmentCode = 0;
        }

        AssembleEquip.EquipmentType = MonEquipmentStorage[EquipmentCode].EquipmentType;
        AssembleEquip.EquipmentTier = MonEquipmentStorage[EquipmentCode].EquipmentTier;
        AssembleEquip.EquipmentCode = MonEquipmentStorage[EquipmentCode].EquipmentCode;
        AssembleEquip.EquipmentName = MonEquipmentStorage[EquipmentCode].EquipmentName;
        AssembleEquip.SpendTiredness = MonEquipmentStorage[EquipmentCode].SpendTiredness;
        AssembleEquip.EquipmentSlots = MonEquipmentStorage[EquipmentCode].EquipmentSlots;
        AssembleEquip.EquipmentImage = MonEquipmentStorage[EquipmentCode].EquipmentImage;
        AssembleEquip.AddSTRAmount = MonEquipmentStorage[EquipmentCode].AddSTRAmount;
        AssembleEquip.AddDURAmount = MonEquipmentStorage[EquipmentCode].AddDURAmount;
        AssembleEquip.AddRESAmount = MonEquipmentStorage[EquipmentCode].AddRESAmount;
        AssembleEquip.AddSPDAmount = MonEquipmentStorage[EquipmentCode].AddSPDAmount;
        AssembleEquip.AddLUKAmount = MonEquipmentStorage[EquipmentCode].AddLUKAmount;
        AssembleEquip.AddHPAmount = MonEquipmentStorage[EquipmentCode].AddHPAmount;
        AssembleEquip.AddTirednessAmount = MonEquipmentStorage[EquipmentCode].AddTirednessAmount;
        //94071 분노 첫번째 특행은 따로 확인 해줘야함
        if(EquipmentCode == 94071)
        {
            string BeforeString = MonEquipmentStorage[EquipmentCode].EquipmentDetail;
            string WrathTag = "";
            int WrathBuffCount = 7 - JsonReadWriteManager.Instance.LkEv_Info.GreatDevilKillCount;
            if (JsonReadWriteManager.Instance.LkEv_Info.GreatDevilKillCount >= 6)
                WrathTag = "Dead";
            else
                WrathTag = "Alive";

            string Pattern = $@"\{{{WrathTag}\}}(.*?)\{{/{WrathTag}\}}";
            Match WrathMatch = Regex.Match(BeforeString, Pattern, RegexOptions.Singleline);
            if(WrathMatch.Success)
            {
                BeforeString = WrathMatch.Groups[1].Value;
            }
            AssembleEquip.EquipmentDetail = BeforeString.Replace("{WrathCount}", WrathBuffCount.ToString());
        }
        else
        {
            AssembleEquip.EquipmentDetail = MonEquipmentStorage[EquipmentCode].EquipmentDetail;
        }
        return AssembleEquip;
    }

    public int GetFixedTierRandomEquipmnet(int Tier)//정해진 티어에서 랜덤한 장비 리턴
    {
        //티어가 정해져 있다면 -> 천의 자리수는 정해진것이다
        //그렇다면 백의 자리 (0~7)(장비 성향), 십의 자리 (0~4)(장비종류), 일의 자리 (1~3(신발 혹은 장신구라면 0)) (곱성향)을 정한다.
        int RandEquipNum = 0;
        int EquipStateType = Random.Range((int)EEquipStateType.StateSTR, (int)EEquipStateType.StateNormal + 1);
        int EquipType = Random.Range((int)EEquipType.TypeWeapon, (int)EEquipType.TypeAcc + 1);
        int EquipMultiType = 0;
        if(EquipType != (int)EEquipType.TypeBoots && EquipType != (int)EEquipType.TypeAcc)
        {
            EquipMultiType = Random.Range((int)EEquipMultiType.MultiSteady, (int)EEquipMultiType.MultiVolatile + 1);
        }

        RandEquipNum = IsEventEquipMultipleNum + (Tier * TierMultipleNum) + (EquipStateType * StateTypeMultipleNum) + (EquipType * TypeMultipleNum) + EquipMultiType;

        return RandEquipNum;
    }

    public int GetFixedTierNTypeRandomEquipment(int Tier, EEquipType EquipType)
    {//티어와 장비 종류가 정해진 곳에서 랜덤
        //티어와 장비 종류가 정해 졌다 -> 천의 자리수와 십의 자리수가 정해졌다.
        //그렇다면 백의 자리 (0~7)(장비 성향), 일의 자리 (1~3(신발 혹은 장신구라면 0)) (곱성향)을 정한다.
        int RandEquipNum = 0;

        int EquipStateType = Random.Range((int)EEquipStateType.StateSTR, (int)EEquipStateType.StateNormal + 1);
        int EquipMultiType = 0;
        if (EquipType != EEquipType.TypeBoots && EquipType != EEquipType.TypeAcc)
        {
            EquipMultiType = Random.Range((int)EEquipMultiType.MultiSteady, (int)EEquipMultiType.MultiVolatile + 1);
        }
        RandEquipNum = IsEventEquipMultipleNum + (Tier * TierMultipleNum) + (EquipStateType * StateTypeMultipleNum) + ((int)EquipType * TypeMultipleNum) + EquipMultiType;

        return RandEquipNum;
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
        int EquipTier = 1;
        int EquipStateType = 0;
        int EquipType = 0;
        int EquipMultiType = 0;
        //티어 = 천의 자리 // 장비 성향 = 백의 자리 // 장비 종류 = 십의 자리 // 곱 성향 = 일의자리
        if (RandNum < TierOneNum)//여기에 들어가면 1티어 장비     0미만
            EquipTier = (int)EEquipTier.TierOne;
        else if (RandNum >= TierOneNum && RandNum < TierTwoNum)//여기 2티어    0이상 0미만
            EquipTier = (int)EEquipTier.TierTwo;
        else if (RandNum >= TierTwoNum && RandNum < TierThreeNum)//여기 3티어   0이상 53미만
            EquipTier = (int)EEquipTier.TierThree;
        else if (RandNum >= TierThreeNum && RandNum < TierFourNum)//여기 4티어  53이상 89 미만
            EquipTier = (int)EEquipTier.TierFour;
        else if (RandNum >= TierFourNum && RandNum < TierFiveNum)//여기 5티어   89이상 100미만
            EquipTier = (int)EEquipTier.TierFive;
        else if (RandNum >= TierFiveNum)//티어 6
            EquipTier = (int)EEquipTier.TierSix;

        EquipStateType = Random.Range((int)EEquipStateType.StateSTR, (int)EEquipStateType.StateNormal + 1);
        EquipType = Random.Range((int)EEquipType.TypeWeapon, (int)EEquipType.TypeAcc + 1);
        if (EquipType != (int)EEquipType.TypeBoots && EquipType != (int)EEquipType.TypeAcc)
        {
            EquipMultiType = Random.Range((int)EEquipMultiType.MultiSteady, (int)EEquipMultiType.MultiVolatile + 1);
        }

        int RandEquipNum = IsEventEquipMultipleNum + (EquipTier * TierMultipleNum) + (EquipStateType * StateTypeMultipleNum) + ((int)EquipType * TypeMultipleNum) + EquipMultiType;

        return RandEquipNum;
    }

    public int GetGamblingTierCode(int GamblingLevel)
    {
        int ResultTierCode = 1;
        int RandNum = Random.Range(0, 100);//0부터 99까지 하나가 나옴
        int TierOneNum = EquipmentGamblingDetail[GamblingLevel, 0];//0 <= RandNum < TierOneNum 까지 걸리면 1티어 장비
        int TierTwoNum = EquipmentGamblingDetail[GamblingLevel, 1] + TierOneNum;//TierOneNum <= RandNum < TierTwoNum 까지 걸리면 2티어//나머지 동일
        int TierThreeNum = EquipmentGamblingDetail[GamblingLevel, 2] + TierTwoNum;
        int TierFourNum = EquipmentGamblingDetail[GamblingLevel, 3] + TierThreeNum;
        int TierFiveNum = EquipmentGamblingDetail[GamblingLevel, 4] + TierFourNum;

        if (JsonReadWriteManager.Instance.E_Info.EarlyLuckLevel >= 7)
        {
            TierOneNum = EquipmentGamblingDetail_Luck[GamblingLevel, 0];
            TierTwoNum = EquipmentGamblingDetail_Luck[GamblingLevel, 1] + TierOneNum;
            TierThreeNum = EquipmentGamblingDetail_Luck[GamblingLevel, 2] + TierTwoNum;
            TierFourNum = EquipmentGamblingDetail_Luck[GamblingLevel, 3] + TierThreeNum;
            TierFiveNum = EquipmentGamblingDetail_Luck[GamblingLevel, 4] + TierFourNum;
        }

        if (RandNum < TierOneNum)//여기에 들어가면 1티어 장비     0미만
            ResultTierCode = (int)EEquipTier.TierOne;
        else if (RandNum >= TierOneNum && RandNum < TierTwoNum)//여기 2티어    0이상 0미만
            ResultTierCode = (int)EEquipTier.TierTwo;
        else if (RandNum >= TierTwoNum && RandNum < TierThreeNum)//여기 3티어   0이상 53미만
            ResultTierCode = (int)EEquipTier.TierThree;
        else if (RandNum >= TierThreeNum && RandNum < TierFourNum)//여기 4티어  53이상 89 미만
            ResultTierCode = (int)EEquipTier.TierFour;
        else if (RandNum >= TierFourNum && RandNum < TierFiveNum)//여기 5티어   89이상 100미만
            ResultTierCode = (int)EEquipTier.TierFive;
        else if (RandNum >= TierFiveNum)//티어 6
            ResultTierCode = (int)EEquipTier.TierSix;

        return ResultTierCode;
    }

    public int GetGamblingLevelPercent(int Level, int Tier)
    {
        if(JsonReadWriteManager.Instance.E_Info.EarlyLuckLevel >= 7)
        {
            return EquipmentGamblingDetail_Luck[Level, Tier];
        }
        else
        {
            return EquipmentGamblingDetail[Level, Tier];
        }
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
