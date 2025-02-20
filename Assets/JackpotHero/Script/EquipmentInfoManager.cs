using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.AI;

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

    [Header("MonWeapon")]
    public EquipmentSO[] MonWeaponSOs;
    [Header("MonArmor")]
    public EquipmentSO[] MonArmorSOs;
    [Header("MonAnotherEquip")]
    public EquipmentSO[] MonAnotherEquipSOs;

    protected Dictionary<float, Sprite> EquipmentSlotSpriteStorage = new Dictionary<float, Sprite>();
    protected Dictionary<int, EquipmentSO> EquipmentStorage = new Dictionary<int, EquipmentSO>();
    protected Dictionary<int, EquipmentSO> MonEquipmentStroage = new Dictionary<int, EquipmentSO>();

    protected List<int> TierOneEquipmentCodes = new List<int>();
    protected List<int> TierTwoEquipmentCodes = new List<int>();
    protected List<int> TierThreeEquipmentCodes = new List<int>();
    protected List<int> TierFourEquipmentCodes = new List<int>();
    protected List<int> TierFiveEquipmentCodes = new List<int>();
    protected List<int> TierSixEquipmnetCodes = new List<int>();

    protected int[,] EquipmentGamblingDetail =
    {{95,5,0,0,0 }, //0����
        { 89,11,0,0,0}, //1����
        { 80,20,1,0,0}, //2����
        { 68,27,5,0,0}, //3����
        { 53,36,11,0,0}, //4����
        { 35,44,20,1,0}, //5����
        { 14,54,27,5,0}, //6����
        { 0,53,36,11,0}, //7����
        { 0,35,44,20,1}, //8����
        { 0,14,54,27,5}, //9����
        { 0,0,53,36,11} };//10����
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
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//�ش� �ڵ��� ��� ���ٸ�
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerWeaponSOs[i]);
            }
        }
        //InitPlayerArmor
        for (int i = 0; i < PlayerArmorSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerArmorSOs[i].EquipmentType * 10000) + (PlayerArmorSOs[i].EquipmentTier * 1000) + PlayerArmorSOs[i].EquipmentCode;
            InitTierEquipmnetCode(i_EquipmentCode, PlayerArmorSOs[i].EquipmentTier);
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//�ش� �ڵ��� ��� ���ٸ�
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerArmorSOs[i]);
            }
        }
        //InitPlayerHelmet
        for (int i = 0; i < PlayerHelmetSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerHelmetSOs[i].EquipmentType * 10000) + (PlayerHelmetSOs[i].EquipmentTier * 1000) + PlayerHelmetSOs[i].EquipmentCode;
            InitTierEquipmnetCode(i_EquipmentCode, PlayerHelmetSOs[i].EquipmentTier);
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//�ش� �ڵ��� ��� ���ٸ�
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerHelmetSOs[i]);
            }
        }
        //InitPlayerBoots
        for (int i = 0; i < PlayerBootsSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerBootsSOs[i].EquipmentType * 10000) + (PlayerBootsSOs[i].EquipmentTier * 1000) + PlayerBootsSOs[i].EquipmentCode;
            InitTierEquipmnetCode(i_EquipmentCode, PlayerBootsSOs[i].EquipmentTier);
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//�ش� �ڵ��� ��� ���ٸ�
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerBootsSOs[i]);
            }
        }
        //InitPlayerAccessories
        for (int i = 0; i < PlayerAccessoriesSOs.Length; i++)
        {
            int i_EquipmentCode = (PlayerAccessoriesSOs[i].EquipmentType * 10000) + (PlayerAccessoriesSOs[i].EquipmentTier * 1000) + PlayerAccessoriesSOs[i].EquipmentCode;
            InitTierEquipmnetCode(i_EquipmentCode, PlayerAccessoriesSOs[i].EquipmentTier);
            if (!EquipmentStorage.ContainsKey(i_EquipmentCode))//�ش� �ڵ��� ��� ���ٸ�
            {
                EquipmentStorage.Add(i_EquipmentCode, PlayerAccessoriesSOs[i]);
            }
        }
        //InitMonWeapon
        for (int i = 0; i < MonWeaponSOs.Length; i++)
        {
            int i_EquipmentCode = MonWeaponSOs[i].EquipmentCode;
            if (!MonEquipmentStroage.ContainsKey(i_EquipmentCode))//�ش� �ڵ��� ��� ���ٸ�
            {
                MonEquipmentStroage.Add(i_EquipmentCode, MonWeaponSOs[i]);
            }
        }
        //IntiMonArmor
        for (int i = 0; i < MonArmorSOs.Length; i++)
        {
            int i_EquipmentCode = MonArmorSOs[i].EquipmentCode;
            if (!MonEquipmentStroage.ContainsKey(i_EquipmentCode))//�ش� �ڵ��� ��� ���ٸ�
            {
                MonEquipmentStroage.Add(i_EquipmentCode, MonArmorSOs[i]);
            }
        }
        //InitMonAnotherEquip
        for (int i = 0; i < MonAnotherEquipSOs.Length; i++)
        {
            int i_EquipmentCode = MonAnotherEquipSOs[i].EquipmentCode;
            if (!MonEquipmentStroage.ContainsKey(i_EquipmentCode))//�ش� �ڵ��� ��� ���ٸ�
            {
                MonEquipmentStroage.Add(i_EquipmentCode, MonAnotherEquipSOs[i]);
            }
        }
    }

    public void InitTierEquipmnetCode(int EquipmentCode , int EquipmentTier)
    {
        switch(EquipmentTier)
        {
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
        }
    }
    public Sprite GetEquipmentSlotSprite(float SlotAmount)
    {
        if(!EquipmentSlotSpriteStorage.ContainsKey(SlotAmount))//���ٸ�
        {
            return EquipmentSlotSpriteStorage[-1];
        }
        return EquipmentSlotSpriteStorage[SlotAmount];
    }
    
    public EquipmentSO GetPlayerEquipmentInfo(int EquipmentCode)
    {
        if (!EquipmentStorage.ContainsKey(EquipmentCode))//EquipmentStorage�� ������ 0�� ���(�ƹ��͵� �ƴѰ�)����
        {
            return EquipmentStorage[0];
        }
        return EquipmentStorage[EquipmentCode];
    }
    public EquipmentSO GetMonEquipmentInfo(int EquipmentCode)
    {
        if (!MonEquipmentStroage.ContainsKey(EquipmentCode))//MonEquipmentStroage�� ������ 0�� ���(�ƹ��͵� �ƴѰ�)����
        {
            return MonEquipmentStroage[0];
        }
        return MonEquipmentStroage[EquipmentCode];
    }

    public int GetGamblingEquipmentCode(int GamblingLevel)//�÷��̾��� �׺� ������ ���缭 ������ ����� �ڵ带 ������//�̰� ���� ��� ��°���
    {//�׷��ϱ� ���⼭ Early��ȭ�� ���� 1Ƽ�� ��µ� ����� ȹ���� Ȯ�������� �Ͼ����
        int RandNum = Random.Range(0, 100);//0���� 99���� �ϳ��� ����
        int TierOneNum = EquipmentGamblingDetail[GamblingLevel, 0];//0 <= RandNum < TierOneNum ���� �ɸ��� 1Ƽ�� ���
        int TierTwoNum = EquipmentGamblingDetail[GamblingLevel, 1] + TierOneNum;//TierOneNum <= RandNum < TierTwoNum ���� �ɸ��� 2Ƽ��//������ ����
        int TierThreeNum = EquipmentGamblingDetail[GamblingLevel, 2] + TierTwoNum;
        int TierFourNum = EquipmentGamblingDetail[GamblingLevel, 3] + TierThreeNum;
        int TierFiveNum = EquipmentGamblingDetail[GamblingLevel, 4] + TierFourNum;

        //���� JsonReadWriteManager�� E_info.Luk�� ������ 7�̻��̶�� 5�ۼ�Ʈ Ȯ���� �߻�
        if (JsonReadWriteManager.Instance.E_Info.EarlyLuckLevel >= 7)
        {
            int RandLuk = Random.Range(0, 99);
            if(RandLuk < 5)//��� 1Ƽ�� ��� ��÷ -> 1Ƽ���� 2Ƽ���....5Ƽ�� ��� 6Ƽ���
            {
                if (RandNum < TierOneNum)//1Ƽ��� 2Ƽ���
                    RandNum = TierOneNum;
                else if (RandNum >= TierOneNum && RandNum < TierTwoNum)//2Ƽ��� 3Ƽ���
                    RandNum = TierTwoNum;
                else if (RandNum >= TierTwoNum && RandNum < TierThreeNum)//3Ƽ��� 4Ƽ���
                    RandNum = TierThreeNum;
                else if (RandNum >= TierThreeNum && RandNum < TierFourNum)//4Ƽ��� 5Ƽ���
                    RandNum = TierFourNum;
                else if (RandNum >= TierFourNum && RandNum < TierFiveNum)//5Ƽ��� 6Ƽ���
                    RandNum = TierFiveNum;
            }
        }

        List<int> ApplicableEquipmentCode = new List<int>();
        //����� Ƽ��� 0X000�� X�� ���� �����ȴ�. 
        //(12345 / 1000) = 12 -> 12 % 10 = 2 -> 2Ƽ�� ����ΰ��� //Ƽ� ���� ���嵵 ���� �س�����? �װ� �������� ������ ������?
        if (RandNum < TierOneNum)//���⿡ ���� 1Ƽ�� ���
        {
            int RandTierNum = Random.Range(0, TierOneEquipmentCodes.Count);//Ƽ� 1�� ����� �ڵ��� �ϳ�
            return TierOneEquipmentCodes[RandTierNum];//Ƽ� 1�� ����� �ڵ��� �ϳ��� ������
        }
        else if(RandNum >= TierOneNum && RandNum < TierTwoNum)
        {
            int RandTierNum = Random.Range(0, TierTwoEquipmentCodes.Count);
            return TierTwoEquipmentCodes[RandTierNum];//Ƽ� 2�� ����� �ڵ��� �ϳ��� ������
        }
        else if(RandNum >= TierTwoNum && RandNum < TierThreeNum)
        {
            int RandTierNum = Random.Range(0, TierThreeEquipmentCodes.Count);
            return TierThreeEquipmentCodes[RandTierNum];//Ƽ� 3�� ����� �ڵ��� �ϳ��� ������
        }
        else if(RandNum >= TierThreeNum && RandNum < TierFourNum)
        {
            int RandTierNum = Random.Range(0, TierFourEquipmentCodes.Count);
            return TierFourEquipmentCodes[RandTierNum];//Ƽ� 4�� ����� �ڵ��� �ϳ��� ������
        }
        else if(RandNum >= TierFourNum && RandNum < TierFiveNum)
        {
            int RandTierNum = Random.Range(0, TierFiveEquipmentCodes.Count);
            return TierFiveEquipmentCodes[RandTierNum];
        }
        else if(RandNum >= TierFiveNum)//Ƽ�� 6
        {
            int RandTierNum = Random.Range(0, TierSixEquipmnetCodes.Count);
            return TierSixEquipmnetCodes[RandTierNum];
        }
        return 0;
    }
}
