using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class EventManager : MonoBehaviour
{
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

    public void SetCurrentEvent(PlayerScript P_Info)
    {
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
    }

    public void OccurEventBySelection(int ButtonType, PlayerScript PlayerInfo, PlaySceneUIManager UIMgr)//PlaySceneManager���� ����
    {//�̰� ��ư�� ������
        switch(CurrentEvent.EventCode)
        {
            case 101:
                Event101(ButtonType, PlayerInfo, UIMgr);
                break;
            case 102:
                Event102(ButtonType, PlayerInfo, UIMgr);
                break;
            case 103:
                Event103(ButtonType, PlayerInfo, UIMgr);
                break;
            default:
                break;
        }
        /*�̰� 2���� ���������� ����, EXP�� ��� ��´ٸ� ���X �׷��� ������ ����Ǹ� ��������
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        PlayerInfo.EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
        */
    }

    protected void EndOfEvent(PlayerScript PlayerInfo, PlaySceneUIManager UIMgr)
    {
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        PlayerInfo.EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
    }

    //------------------------------------------Event101
    protected void Event101(int ButtonType, PlayerScript P_Info, PlaySceneUIManager UIMgr)
    {
        //0. �������� �ʴ´�.
        //1. �� ��������. -> ��� ��´�// �κ��丮 �뷮�� �κ��丮 �ڵ尡 0�� ���� �ѳ� ������ Event�� ���ϵȴ�.
        //�� ��í ������ �°� ��� ȹ���Ѵ�.
        if(ButtonType == 1)
        {
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

            EndOfEvent(P_Info, UIMgr);
        }
    }
    //------------------------------------------Event102
    protected void Event102(int ButtonType, PlayerScript P_Info, PlaySceneUIManager UIMgr)
    {
        //0. �������� �ʴ´�.
        //1. ���ݸ� ��������.-> �ణ�� EXP ȹ���Ѵ�. 100~ 150����?
        //2. �� ��������.��� ��´�// �κ��丮 �뷮�� �κ��丮 �ڵ尡 0�� ���� �ѳ� ������ Event�� ���ϵȴ�.
        //�� ��í ������ �°� ��� ȹ���Ѵ�.
        switch (ButtonType)
        {
            case 1:
                P_Info.SetPlayerEXPAmount(Random.Range(100, 151));
                UIMgr.GI_UI.ActiveGettingUI();
                break;
            case 2:
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
                break;
        }
        EndOfEvent(P_Info, UIMgr);
    }
    //------------------------------------------Event103
    protected void Event103(int ButtonType, PlayerScript P_Info, PlaySceneUIManager UIMgr)
    {
        //0. �������� �ʴ´�.
        //1. ���ݸ� ��������.//exp ȹ�� 100 ~ 150����
        //2. �� ��������.//��� ȹ��
        //3. �ֺ��� �ƹ��� ���� ���캻��.//���� ����
        switch (ButtonType)
        {
            case 1:
                P_Info.SetPlayerEXPAmount(Random.Range(100, 151));
                UIMgr.GI_UI.ActiveGettingUI();
                EndOfEvent(P_Info, UIMgr);
                break;
            case 2:
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
                EndOfEvent(P_Info, UIMgr);
                break;
            case 3:
                P_Info.GetPlayerStateInfo().CurrentPlayerAction = (int)EPlayerCurrentState.Battle;
                P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = 0;
                UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
                break;
        }
    }
}
