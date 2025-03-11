using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public PlayerManager PlayerMgr;
    public PlaySceneUIManager UIMgr;
    public BattleManager BattleMgr;

    public EventSO[] GameEvents;
    // Start is called before the first frame update

    Dictionary<int, List<EventSO>> EventStorage = new Dictionary<int, List<EventSO>>();

    public EventSO CurrentEvent { protected set; get; }
    private void Awake()
    {
        InitEvent();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitEvent()
    {
        foreach(EventSO Ev in GameEvents)
        {
            int ThemeNum = Ev.EventCode / 100;
            if(!EventStorage.ContainsKey(ThemeNum))
            {
                List<EventSO> EventList = new List<EventSO>();
                EventList.Add(Ev);
                EventStorage.Add(ThemeNum, EventList);
            }
            else
            {
                EventStorage[ThemeNum].Add(Ev);
            }
        }
    }

    public void SetCurrentEvent()
    {
        PlayerScript P_Info = PlayerMgr.GetPlayerInfo();

        int ThemeNum = P_Info.GetPlayerStateInfo().CurrentFloor;
        int DetailOfEvents = P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails;

        //DetailOfEvents�� 101 ~ �� �� 1�׸���� 101~ 2�׸� ��� 201~���̴�//�̺�Ʈ�� ������ ���·� ������ ���� ������
        if (DetailOfEvents != 0)
        {
            int ThemeOfEvent = DetailOfEvents / 100;
            //PatternStorage[DetailOfEvents / 100];�̰� ���� ����� ������ ���� ������ �ϳ�// ���⼭ ã�ƾ���
            for (int i = 0; i < EventStorage[ThemeOfEvent].Count; i++)
            {
                if (EventStorage[ThemeOfEvent][i].EventCode == DetailOfEvents)//��ġ�°� �ִٸ�
                {
                    CurrentEvent = EventStorage[ThemeOfEvent][i];
                    P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
                    JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                    return;//�Լ� ����
                }
            }
        }

        //��� �׸��� �� ������ ������
        if (ThemeNum == -1)
        {
            List<EventSO> EventSOs = new List<EventSO>();
            foreach (int Key in EventStorage.Keys)
            {
                foreach (EventSO ESO in EventStorage[Key])
                {
                    EventSOs.Add(ESO);
                }
            }
            int Rand = Random.Range(0, EventSOs.Count);
            CurrentEvent = EventSOs[Rand];
        }
        else//-1�� �ƴҶ� ���� �׸� ���� �°� ����
        {
            int Rand = Random.Range(0, EventStorage[ThemeNum].Count);
            CurrentEvent = EventStorage[ThemeNum][Rand];
        }
        P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
        JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
    }

    protected void EndOfEvent()
    {
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        PlayerMgr.GetPlayerInfo().EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
    }

    public void PressEventSelectionButton(int SelectionType)//�̰� ���� �̺�Ʈ�� ��� �Լ���
    {
        OccurEventBySelection(SelectionType);//���⼭ �̺�Ʈ�� ���ÿ� �°� �̺�Ʈ�� �߻���
        /*
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        PlayerMgr.GetPlayerInfo().EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
        *///������ 2���� ���¿� �°� ������ �̺�Ʈ���� �߻��ؾ� �ҵ� -> ��� ���, EXP�� ��°� �������� ��Ʋ�� �����Ѵٸ� ��������
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        UIMgr.SetUI();
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();//����
        //StartCoroutine(CheckBackGroundMoveEnd(true));//���⿡ ���� ���� ���¿� �°� ui�� �����ǰ� �������� ��
        //SetUI�� ������ ���� CheckBackGroundMoveEnd �ڷ�ƾ��
        //UIMgr.SetUI(PlayerMgr);
    }

    public void OccurEventBySelection(int ButtonType)//PlaySceneManager���� ����
    {//�̰� ��ư�� ������
        switch(CurrentEvent.EventCode)
        {
            case 101:
                Event101(ButtonType);
                break;
            case 102:
                Event102(ButtonType);
                break;
            case 103:
                Event103(ButtonType);
                break;
            default:
                break;
        }
        /*�̰� 2���� ���������� ����, EXP�� ��� ��´ٸ� ���X �׷��� ������ ����Ǹ� ��������
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        PlayerInfo.EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
        */
    }



    //------------------------------------------Event101
    protected void Event101(int ButtonType)
    {
        //0. �������� �ʴ´�.
        //1. �� ��������. -> ��� ��´�// �κ��丮 �뷮�� �κ��丮 �ڵ尡 0�� ���� �ѳ� ������ Event�� ���ϵȴ�.
        //�� ��í ������ �°� ��� ȹ���Ѵ�.
        if(ButtonType == 1)
        {
            /*
            List<int> EmptyInventoryIndex = new List<int>();
            int InventoryAmount = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");//�̰� ��밡���� �κ��丮 ����
            for (int i = 0; i < InventoryAmount; i++)//��밡�� �κ��丮 ���� ��ŭ �ݺ�//0�����̸� 0~3���� �ݺ��� 5���� �̻��̸� 0~11����
            {
                if (P_Info.GetPlayerStateInfo().EquipmentInventory[i] == 0)//����ִٸ�
                {
                    EmptyInventoryIndex.Add(i);//���� ���� �κ��丮 list�� ����
                }
            }
            if(EmptyInventoryIndex.Count <= 0)//����ִ� inventory Slot�� ���ٸ�
            {
                //����ִ� inventory�� ���ٴ� ǥ�ø� �������
                //UI�� �κ��丮�� �־�� �ҵ�?
                UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                return;   
            }
            //������� �ʴٸ� EmptyInventoryIndex�� ���� �տ� �ִ� index�� ������ ����� �ڵ带 ����
            //�̷��� ��� �̱� ������ �´� ��� ������ �ϳ��� ������ ��.
            int RandomEquipment = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(P_Info.GetPlayerStateInfo().EquipmentGamblingLevel);
            P_Info.GetPlayerStateInfo().EquipmentInventory[EmptyInventoryIndex[0]] = RandomEquipment;
            UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);
            */
            if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
            {
                UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                return;
            }
            int RandomEquipment = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
            PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipment);
            UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);
            EndOfEvent();
        }
        else
        {
            EndOfEvent();
        }
    }
    //------------------------------------------Event102
    protected void Event102(int ButtonType)
    {
        //0. �������� �ʴ´�.
        //1. ���ݸ� ��������.-> �ణ�� EXP ȹ���Ѵ�. 100~ 150����?
        //2. �� ��������.��� ��´�// �κ��丮 �뷮�� �κ��丮 �ڵ尡 0�� ���� �ѳ� ������ Event�� ���ϵȴ�.
        //�� ��í ������ �°� ��� ȹ���Ѵ�.
        switch (ButtonType)
        {
            case 0:
                EndOfEvent();
                break;
            case 1:
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(Random.Range(100, 151));
                UIMgr.GI_UI.ActiveGettingUI();
                break;
            case 2:
                /*
                List<int> EmptyInventoryIndex = new List<int>();
                int InventoryAmount = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");//�̰� ��밡���� �κ��丮 ����
                for (int i = 0; i < InventoryAmount; i++)//��밡�� �κ��丮 ���� ��ŭ �ݺ�//0�����̸� 0~3���� �ݺ��� 5���� �̻��̸� 0~11����
                {
                    if (P_Info.GetPlayerStateInfo().EquipmentInventory[i] == 0)//����ִٸ�
                    {
                        EmptyInventoryIndex.Add(i);//���� ���� �κ��丮 list�� ����
                    }
                }

                if (EmptyInventoryIndex.Count <= 0)//����ִ� inventory Slot�� ���ٸ�
                {
                    //����ִ� inventory�� ���ٴ� ǥ�ø� �������
                    //UI�� �κ��丮�� �־�� �ҵ�?
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                //������� �ʴٸ� EmptyInventoryIndex�� ���� �տ� �ִ� index�� ������ ����� �ڵ带 ����
                //�̷��� ��� �̱� ������ �´� ��� ������ �ϳ��� ������ ��.
                int RandomEquipment = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(P_Info.GetPlayerStateInfo().EquipmentGamblingLevel);
                P_Info.GetPlayerStateInfo().EquipmentInventory[EmptyInventoryIndex[0]] = RandomEquipment;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);
                */
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                int RandomEquipment = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipment);
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);
                break;
        }
        EndOfEvent();
    }
    //------------------------------------------Event103
    protected void Event103(int ButtonType)
    {
        //0. �������� �ʴ´�.
        //1. ���ݸ� ��������.//exp ȹ�� 100 ~ 150����
        //2. �� ��������.//��� ȹ��
        //3. �ֺ��� �ƹ��� ���� ���캻��.//���� ����
        switch (ButtonType)
        {
            case 0:
                EndOfEvent();
                break;
            case 1:
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(Random.Range(100, 151));
                UIMgr.GI_UI.ActiveGettingUI();
                EndOfEvent();
                break;
            case 2:
                /*
                List<int> EmptyInventoryIndex = new List<int>();
                int InventoryAmount = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");//�̰� ��밡���� �κ��丮 ����
                for (int i = 0; i < InventoryAmount; i++)//��밡�� �κ��丮 ���� ��ŭ �ݺ�//0�����̸� 0~3���� �ݺ��� 5���� �̻��̸� 0~11����
                {
                    if (P_Info.GetPlayerStateInfo().EquipmentInventory[i] == 0)//����ִٸ�
                    {
                        EmptyInventoryIndex.Add(i);//���� ���� �κ��丮 list�� ����
                    }
                }
                if (EmptyInventoryIndex.Count <= 0)//����ִ� inventory Slot�� ���ٸ�
                {
                    //����ִ� inventory�� ���ٴ� ǥ�ø� �������
                    //UI�� �κ��丮�� �־�� �ҵ�?
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                //������� �ʴٸ� EmptyInventoryIndex�� ���� �տ� �ִ� index�� ������ ����� �ڵ带 ����
                //�̷��� ��� �̱� ������ �´� ��� ������ �ϳ��� ������ ��.
                int RandomEquipment = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(P_Info.GetPlayerStateInfo().EquipmentGamblingLevel);
                P_Info.GetPlayerStateInfo().EquipmentInventory[EmptyInventoryIndex[0]] = RandomEquipment;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);
                */
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                int RandomEquipment = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentGamblingLevel);
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipment);
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);
                EndOfEvent();
                break;
            case 3:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = 0;
                UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
                BattleMgr.InitCurrentBattleMonsters();
                BattleMgr.InitMonsterNPlayerActiveGuage();
                BattleMgr.ProgressBattle();
                break;
        }
    }
}
