using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    private PlayerManager PlayerMgr;
    [SerializeField]
    private PlaySceneUIManager UIMgr;
    [SerializeField]
    private BattleManager BattleMgr;

    public EventSO[] CommonGameEvents;
    public EventSO[] LinkageGameEvents;
    public EventSO[] Stage01GameEvents;
    public EventSO[] Stage02GameEvents;
    public EventSO[] Stage03GameEvents;
    public EventSO[] Stage04GameEvents;

    public EventSO[] CommonFollowEvents;
    public EventSO[] LinkageFollowEvents;
    public EventSO[] Stage01FollowEvents;
    public EventSO[] Stage02FollowEvents;
    public EventSO[] Stage03FollowEvents;
    public EventSO[] Stage04FollowEvents;
    // Start is called before the first frame update

    Dictionary<int, List<EventSO>> EventStorage = new Dictionary<int, List<EventSO>>();
    Dictionary<int, EventSO> FollowEventStorage = new Dictionary<int, EventSO>();

    private const int BossTheme = 10;
    private const int EventTheme = 9;
    private readonly float[] StageAverageRward = new float[4] { 28, 101, 313, 816 };
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
        //CommonGameEvent
        foreach(EventSO Ev in CommonGameEvents)
        {
            int ThemeNum = Ev.EventCode / 1000;
            if(!EventStorage.ContainsKey(ThemeNum))
            {
                List<EventSO> EventList = new List<EventSO>();
                EventList.Add(Ev);
                EventStorage.Add(ThemeNum, EventList);
            }
            else
                EventStorage[ThemeNum].Add(Ev);
        }
        //CommonFollowEvent
        foreach(EventSO FoEv in CommonFollowEvents)
        {
            if (!FollowEventStorage.ContainsKey(FoEv.EventCode))
                FollowEventStorage.Add(FoEv.EventCode, FoEv);
        }
        //LinkageGameEvent
        foreach (EventSO Ev in LinkageGameEvents)
        {
            int ThemeNum = Ev.EventCode / 1000;
            if (!EventStorage.ContainsKey(ThemeNum))
            {
                List<EventSO> EventList = new List<EventSO>();
                EventList.Add(Ev);
                EventStorage.Add(ThemeNum, EventList);
            }
            else
                EventStorage[ThemeNum].Add(Ev);
        }
        //LinkageFollowEvent
        foreach (EventSO FoEv in LinkageFollowEvents)
        {
            if (!FollowEventStorage.ContainsKey(FoEv.EventCode))
                FollowEventStorage.Add(FoEv.EventCode, FoEv);
        }
        //Stage01GameEvent
        foreach (EventSO Ev in Stage01GameEvents)
        {
            int ThemeNum = Ev.EventCode / 1000;
            if (!EventStorage.ContainsKey(ThemeNum))
            {
                List<EventSO> EventList = new List<EventSO>();
                EventList.Add(Ev);
                EventStorage.Add(ThemeNum, EventList);
            }
            else
                EventStorage[ThemeNum].Add(Ev);
        }
        //Stage01FollowEvent
        foreach (EventSO FoEv in Stage01FollowEvents)
        {
            if (!FollowEventStorage.ContainsKey(FoEv.EventCode))
                FollowEventStorage.Add(FoEv.EventCode, FoEv);
        }
        //Stage02GameEvent
        foreach (EventSO Ev in Stage02GameEvents)
        {
            int ThemeNum = Ev.EventCode / 1000;
            if (!EventStorage.ContainsKey(ThemeNum))
            {
                List<EventSO> EventList = new List<EventSO>();
                EventList.Add(Ev);
                EventStorage.Add(ThemeNum, EventList);
            }
            else
                EventStorage[ThemeNum].Add(Ev);
        }
        //Stage02FollowEvent
        foreach (EventSO FoEv in Stage02FollowEvents)
        {
            if (!FollowEventStorage.ContainsKey(FoEv.EventCode))
                FollowEventStorage.Add(FoEv.EventCode, FoEv);
        }
        //Stage03GameEvent
        foreach (EventSO Ev in Stage03GameEvents)
        {
            int ThemeNum = Ev.EventCode / 1000;
            if (!EventStorage.ContainsKey(ThemeNum))
            {
                List<EventSO> EventList = new List<EventSO>();
                EventList.Add(Ev);
                EventStorage.Add(ThemeNum, EventList);
            }
            else
                EventStorage[ThemeNum].Add(Ev);
        }
        //Stage03FollowEvent
        foreach (EventSO FoEv in Stage03FollowEvents)
        {
            if (!FollowEventStorage.ContainsKey(FoEv.EventCode))
                FollowEventStorage.Add(FoEv.EventCode, FoEv);
        }
        //Stage04GameEvent
        foreach (EventSO Ev in Stage04GameEvents)
        {
            int ThemeNum = Ev.EventCode / 1000;
            if (!EventStorage.ContainsKey(ThemeNum))
            {
                List<EventSO> EventList = new List<EventSO>();
                EventList.Add(Ev);
                EventStorage.Add(ThemeNum, EventList);
            }
            else
                EventStorage[ThemeNum].Add(Ev);
        }
        //Stage04FollowEvent
        foreach (EventSO FoEv in Stage04FollowEvents)
        {
            if (!FollowEventStorage.ContainsKey(FoEv.EventCode))
                FollowEventStorage.Add(FoEv.EventCode, FoEv);
        }
    }

    public void SetCurrentEvent(bool IsBossEvent = false)
    {
        PlayerScript P_Info = PlayerMgr.GetPlayerInfo();

        int ThemeNum = P_Info.GetPlayerStateInfo().CurrentFloor;
        int DetailOfEvents = P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails;
        int CurrentFloorEventCount = 0;// = EventStorage[ThemeNum].Count;
        int CommonEventCount = EventStorage[EventTheme].Count;
        //���� �̺�Ʈ ��� -> ���� �̺�Ʈ�� �Ѱ��� �ɵ� ���� �̺�Ʈ �ڵ� = 10000
        if(IsBossEvent == true)
        {
            CurrentEvent = EventStorage[BossTheme][0];
            P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
            return;
        }

        //DetailOfEvents�� 1000 ~ �� �� 1�׸���� 1000~ 2�׸� ��� 2000~���̴�
        if (DetailOfEvents != 0)//�̺�Ʈ�� ������ ���·� ������ ���� ������
        {
            int FixedEvnetThemeNum = DetailOfEvents / 1000;
            for (int i = 0; i < EventStorage[FixedEvnetThemeNum].Count; i++)
            {
                if (EventStorage[FixedEvnetThemeNum][i].EventCode == DetailOfEvents)//��ġ�°� �ִٸ�
                {
                    CurrentEvent = EventStorage[FixedEvnetThemeNum][i];
                    CheckLinkageEvent();//���谡 �ִ� �̺�Ʈ��� ���谡 ������ ����� üũ�ؼ� CurrentEvent�� �ٲ���
                    P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
                    JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                    return;//�Լ� ����
                }
            }
            if (FollowEventStorage.ContainsKey(DetailOfEvents))
            {
                CurrentEvent = FollowEventStorage[DetailOfEvents];
                CheckLinkageEvent();//���谡 �ִ� �̺�Ʈ��� ���谡 ������ ����� üũ�ؼ� CurrentEvent�� �ٲ���
                P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
                JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                return;//�Լ� ����
            }
        }

        //���� �°Ÿ� �̺�Ʈ�� ���� �����ؾ� �Ǵ°�//���� �̺�Ʈ�� �ش� �� �̺�Ʈ �����ϳ�.
        int RandNum = Random.Range(0, CurrentFloorEventCount + CommonEventCount);
        if(RandNum >= 0 && RandNum < CurrentFloorEventCount)
        {//������ �̺�Ʈ�� ����
            RandNum = Random.Range(0, EventStorage[ThemeNum].Count);
            CurrentEvent = EventStorage[ThemeNum][RandNum];
            CheckLinkageEvent();
            P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
            return;//�Լ� ����
        }
        else if(RandNum >= CurrentFloorEventCount && RandNum < CurrentFloorEventCount + CommonEventCount)
        {//���� �̺�Ʈ�� ����
            RandNum = Random.Range(0, EventStorage[EventTheme].Count);
            CurrentEvent = EventStorage[EventTheme][RandNum];
            CheckLinkageEvent();
            P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
            return;//�Լ� ����
        }

        //����� �Ѿ�� �ƹ��͵� �ȵȰ�-- �׷��ϱ� ������ �߻��Ѱ�
        Debug.Log("ThereIsNoEvent");
    }

    protected void CheckLinkageEvent()//���� �̺�Ʈ üũ�ϴ��Լ�// ���� �̺�Ʈ ��κ��� ���⼭ ������ ����
    {
        switch(CurrentEvent.EventCode)
        {
            case 9020:
                if(JsonReadWriteManager.Instance.LkEv_Info.TalkingMonster == true)
                {
                    if(JsonReadWriteManager.Instance.LkEv_Info.TalkingDirtGolem == true)
                    {
                        int Rand = Random.Range(0, 2);
                        if(Rand == 0)
                        {//����� ���� ����
                            if (JsonReadWriteManager.Instance.LkEv_Info.TotoRepayFavor == true)
                            {//����� �������Ⱑ �Ϸ��ٸ� ������ ��������
                                FindNChangeEvent(8060);
                            }
                            else
                            {//�Ϸ� �ȉ�ٸ� ������ ���������
                                FindNChangeEvent(8030);
                            }
                        }
                        else
                        {//����� ���ֹ��� ��// ����� �ູ���� ��
                            if(JsonReadWriteManager.Instance.LkEv_Info.TotoBlessedSword == true)
                            {//����� �ູ���� ���� �Ϸ������� ������ ��������
                                FindNChangeEvent(8060);
                            }
                            else if(JsonReadWriteManager.Instance.LkEv_Info.TotoCursedSword == true)
                            {//������ ���ֹ��� ���� �Ϸ� ������ ����� �ູ���� ������ ������
                                FindNChangeEvent(8050);
                            }
                            else
                            {
                                int CursedSword = 17001;
                                int SmallCursedSword = 17002;
                                bool IsHaveCursedSword = false;
                                if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == CursedSword ||
                                    PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == SmallCursedSword)
                                    IsHaveCursedSword = true;
                                else
                                {
                                    for (int i = 0; i < PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory.Length; i++)
                                    {
                                        if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] == CursedSword ||
                                            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] == SmallCursedSword)
                                        {
                                            IsHaveCursedSword = true;
                                            break;
                                        }
                                    }
                                }

                                if (IsHaveCursedSword == true)
                                {//���ֹ�����, ���ְ� ������ ���� ������ �ִٸ�
                                    FindNChangeEvent(8040);
                                }
                                else
                                {//�Ȱ����� ������ --> ������ �������� �Ǵ� ������ �������� ������
                                    if (JsonReadWriteManager.Instance.LkEv_Info.TotoRepayFavor == true)
                                    {//����� �������Ⱑ �Ϸ��ٸ� ������ ��������
                                        FindNChangeEvent(8060);
                                    }
                                    else
                                    {//�Ϸ� �ȉ�ٸ� ������ ���������
                                        FindNChangeEvent(8030);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        FindNChangeEvent(8010);
                    }
                }
                break;
            case 9030:
                if(JsonReadWriteManager.Instance.LkEv_Info.RestInPeace == true)
                {
                    int Rand = Random.Range(0, 2);
                    if(Rand == 0)
                    {//���⿡ �ɷ��� ������ �����
                        FindNChangeEvent(8020);
                    }
                }
                break;
            case 9050:
                if (JsonReadWriteManager.Instance.LkEv_Info.OminousSword == true &&
                    JsonReadWriteManager.Instance.LkEv_Info.CleanOminousSword == false)
                {
                    FindNChangeEvent(8000);
                }
                break;
            case 9080:
                if(JsonReadWriteManager.Instance.LkEv_Info.TotoBlessedSword == true)
                {//8100����
                    FindNChangeEvent(8100);
                }
                else if(JsonReadWriteManager.Instance.LkEv_Info.OminousSword == true)
                {
                    //�ұ��� ���� ���� 8070
                    //���� ������ 8090����
                    int CursedSword = 17001;
                    int SmallCursedSword = 17002;
                    bool IsHaveCursedSword = false;
                    if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == CursedSword ||
                        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipWeaponCode == SmallCursedSword)
                        IsHaveCursedSword = true;
                    else
                    {
                        for (int i = 0; i < PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory.Length; i++)
                        {
                            if (PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] == CursedSword ||
                                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().EquipmentInventory[i] == SmallCursedSword)
                            {
                                IsHaveCursedSword = true;
                                break;
                            }
                        }
                    }

                    if(IsHaveCursedSword == false)
                        FindNChangeEvent(8090);
                    else if (JsonReadWriteManager.Instance.LkEv_Info.CleanOminousSword == true)//������ ���� ���� 8080����
                        FindNChangeEvent(8080);
                    else
                        FindNChangeEvent(8070);
                    
                }
                break;
        }
    }

    public void FindNChangeEvent(int EventC)
    {
        int EventTheme = EventC / 1000;
        for(int i = 0; i < EventStorage[EventTheme].Count; i++)
        {
            if (EventStorage[EventTheme][i].EventCode == EventC)
            {
                CurrentEvent = EventStorage[EventTheme][i];
                break;
            }
        }
    }

    protected void EndOfEvent()
    {
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        PlayerMgr.GetPlayerInfo().EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
    }

    public void PressEventSelectionButton(int SelectionType)//�̰� ���� �̺�Ʈ�� ��� �Լ���
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
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
        int CurrentEventTheme = CurrentEvent.EventCode / 1000;
        switch(CurrentEventTheme)
        {
            case 1:
                Stage01ThemeEvent(ButtonType);
                break;
            case 2:
                Stage02ThemeEvent(ButtonType);
                break;
            case 3:
                Stage03ThemeEvent(ButtonType);
                break;
            case 4:
                Stage04ThemeEvent(ButtonType);
                break;
            case 8:
                LinkageThemeEvent(ButtonType);
                break;
            case 9:
            case 10:
                CommonThemeEvent(ButtonType);
                break;
            default:
                break;
        }
        /*�̰� 2���� ���������� ����, EXP�� ��� ��´ٸ� ���X �׷��� ������ ����Ǹ� ��������
        UIMgr.E_UI.InActiveEventUI();//��ư �� �������� �̺�Ʈ�� ���� �Ѵ�.
        PlayerInfo.EndOfAction();//���⼭ Action, ActionDetail�� �ʱ�ȭ, ���� �ൿ �ʱ�ȭ
        */
    }

    protected void CommonThemeEvent(int ButtonType)
    {
        CommonEventDetailAction CommonEvents = new CommonEventDetailAction();
        int FollowEventCode = 0;
        int CurrentStageReward = (int)StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1];
        switch (CurrentEvent.EventCode)
        {
            case 9000:
                FollowEventCode = CommonEvents.Event9000(ButtonType, CurrentStageReward, PlayerMgr, UIMgr);
                break;
            case 9010:
                FollowEventCode = CommonEvents.Event9010(ButtonType, CurrentStageReward, PlayerMgr);
                break;
            case 9020:
                FollowEventCode = CommonEvents.Event9020(ButtonType, PlayerMgr);
                break;
            case 9030:
                FollowEventCode = CommonEvents.Event9030(ButtonType, CurrentStageReward, PlayerMgr, UIMgr);
                break;
            case 9040:
                FollowEventCode = CommonEvents.Event9040(ButtonType, CurrentStageReward, PlayerMgr);
                break;
            case 9050:
                FollowEventCode = CommonEvents.Event9050(ButtonType, CurrentStageReward, PlayerMgr, UIMgr);
                break;
            case 9060:
                FollowEventCode = CommonEvents.Event9060(ButtonType, CurrentStageReward, PlayerMgr, UIMgr);
                break;
            case 9070:
                FollowEventCode = CommonEvents.Event9070(ButtonType, PlayerMgr, UIMgr);
                break;
            case 9071:
            case 9072:
            case 9073:
            case 9074:
                FollowEventCode = CommonEvents.Event9071_4(ButtonType, CurrentEvent.EventCode, PlayerMgr, UIMgr);
                break;
            case 9080:
                FollowEventCode = CommonEvents.Event9080(ButtonType, PlayerMgr, UIMgr);
                break;
            case 10000:
                CommonEvents.Event10000(ButtonType, PlayerMgr);
                Debug.Log("Boss");
                EndOfEvent();
                break;
            case 9001:
            case 9002:
            case 9011:
            case 9021:
            case 9022:
            case 9023:
            case 9031:
            case 9032:
            case 9041:
            case 9051:
            case 9052:
            case 9061:
            case 9062:
            case 9063:
            case 9075:
            case 9076:
            case 9081:
            case 9082:
                CommonFollowEvent();
                break;
        }

        if (CurrentEvent.EventCode == FollowEventCode || FollowEventCode == 0)
            return;
        else if (FollowEventStorage.ContainsKey(FollowEventCode))
        {
            CurrentEvent = FollowEventStorage[FollowEventCode];
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
            UIMgr.E_UI.ActiveEventUI(this);
        }
    }
    protected void LinkageThemeEvent(int ButtonType)
    {
        LinkageEventDetailAction LinkageEvent = new LinkageEventDetailAction();
        int FollowEventCode = 0;
        int CurrentStageReward = (int)StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1];
        switch (CurrentEvent.EventCode)
        {
            case 8000:
                FollowEventCode = LinkageEvent.Event8000(ButtonType, CurrentStageReward, PlayerMgr, UIMgr);
                break;
            case 8010:
                FollowEventCode = LinkageEvent.Event8010(ButtonType, PlayerMgr);
                break;
            case 8020:
                FollowEventCode = LinkageEvent.Event8020(ButtonType, CurrentStageReward, PlayerMgr, UIMgr);
                break;
            case 8030:
                FollowEventCode = LinkageEvent.Event8030(ButtonType, PlayerMgr, UIMgr);
                break;
            case 8040:
                FollowEventCode = LinkageEvent.Event8040(ButtonType, PlayerMgr);
                break;
            case 8050:
                FollowEventCode = LinkageEvent.Event8050(ButtonType, PlayerMgr, UIMgr);
                break;
            case 8060:
                FollowEventCode = LinkageEvent.Event8060(ButtonType, PlayerMgr);
                break;
            case 8070:
                FollowEventCode = LinkageEvent.Event8070(ButtonType, PlayerMgr);
                break;
            case 8080:
                FollowEventCode = LinkageEvent.Event8080();
                break;
            case 8090:
                FollowEventCode = LinkageEvent.Event8090(ButtonType, PlayerMgr);
                break;
            case 8100:
                FollowEventCode = LinkageEvent.Event8100(ButtonType, PlayerMgr);
                break;
            case 8001:
            case 8002:
            case 8003:
            case 8004:
            case 8011:
            case 8012:
            case 8013:
            case 8021:
            case 8031:
            case 8041:
            case 8042:
            case 8051:
            case 8061:
            case 8062:
            case 8071:
            case 8081:
            case 8091:
            case 8101:
                CommonFollowEvent();
                break;
        }

        if (CurrentEvent.EventCode == FollowEventCode || FollowEventCode == 0)
            return;
        else if (FollowEventStorage.ContainsKey(FollowEventCode))
        {
            CurrentEvent = FollowEventStorage[FollowEventCode];
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
            UIMgr.E_UI.ActiveEventUI(this);
        }
    }
    protected void Stage01ThemeEvent(int ButtonType)
    {
        Stage01EventDetailAction Stage01Event = new Stage01EventDetailAction();
        int FollowEventCode = 0;
        int CurrentStageReward = (int)StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1];
        switch (CurrentEvent.EventCode)
        {
            case 1000:
                FollowEventCode = Stage01Event.Event1000(ButtonType, PlayerMgr);
                break;
            case 1010:
                FollowEventCode = Stage01Event.Event1010(ButtonType, PlayerMgr, UIMgr);
                break;
            case 1013:
                Stage01Event.Event1013(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 1020:
                FollowEventCode = Stage01Event.Event1020(ButtonType, PlayerMgr);
                break;
            case 1022:
            case 1023:
                Stage01Event.Event1022_3(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 1030:
                FollowEventCode = Stage01Event.Event1030(ButtonType, PlayerMgr);
                break;
            case 1032:
                Stage01Event.Event1032(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 1040:
                FollowEventCode = Stage01Event.Event1040(ButtonType, PlayerMgr);
                break;
            case 1050:
                FollowEventCode = Stage01Event.Event1050(ButtonType, PlayerMgr);
                break;
            case 1001:
            case 1002:
            case 1003:
            case 1011:
            case 1012:
            case 1021:
            case 1024:
            case 1031:
            case 1041:
            case 1042:
            case 1051:
            case 1052:
            case 1053:
                CommonFollowEvent();
                break;
        }

        if (CurrentEvent.EventCode == FollowEventCode || FollowEventCode == 0)
            return;
        else if (FollowEventStorage.ContainsKey(FollowEventCode))
        {
            CurrentEvent = FollowEventStorage[FollowEventCode];
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
            UIMgr.E_UI.ActiveEventUI(this);
        }
    }
    protected void Stage02ThemeEvent(int ButtonType)
    {

    }
    protected void Stage03ThemeEvent(int ButtonType)
    {

    }
    protected void Stage04ThemeEvent(int ButtonType)
    {

    }
    protected void CommonFollowEvent()
    {
        EndOfEvent();
    }
    //------------------------------------------Event1060
    /*
    protected void Event1060(int ButtonType)
    {
        //0. �ƹ��� ����
        //1. ���ֹ��� �� ȹ��, �̰� ���ý� �ٽ� �߻� x
        switch (ButtonType)
        {
            case 0:
                CurrentEvent = FollowEventStorage[1061];
                break;
            case 1:
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                //���ֹ��� �� �ڵ� = 17001
                int OminousSwordCode = 17001;
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(OminousSwordCode);
                UIMgr.GI_UI.ActiveGettingUI(OminousSwordCode);
                JsonReadWriteManager.Instance.LkEv_Info.OminousSword = true;

                CurrentEvent = FollowEventStorage[1062];
                break;
        }
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
        UIMgr.E_UI.ActiveEventUI(this);
    }
    */
    //------------------------------------------Event9020
    /*
    protected void Event9020(int ButtonType)
    {
        //0. �Ƿε� 300ȸ��
        //1. gk + 1
        //2. ü�� -30 �Ƿε� - 300 gk + 3
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300);
                CurrentEvent = FollowEventStorage[9021];
                break;
            case 1:
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                CurrentEvent = FollowEventStorage[9022];
                break;
            case 2:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                PlayerMgr.GetPlayerInfo().PlayerRegenHp(-30);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 3;
                JsonReadWriteManager.Instance.LkEv_Info.TalkingMonster = true;

                CurrentEvent = FollowEventStorage[9023];
                break;
        }
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
        UIMgr.E_UI.ActiveEventUI(this);
    }
    */
    //------------------------------------------Event9030
    /*
    protected void Event9030(int ButtonType)
    {
        //0. �������� ��� Ƽ�� ���, ����� ����, bk + 1
        //1. ����� ����
        //2. ����� ���� , �Ƿε� -300, gk + 1
        //3. �Ƿε� -300, gk + 2
        int RewardRange = 0;
        int RandomReward = 0;
        switch(ButtonType)
        {
            case 0:
                //��� ȹ��
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//�κ��丮�� ��á�ٸ�
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                int RandomEquipment = EquipmentInfoManager.Instance.GetFixedTierRandomEquipmnet(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor);
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipment);
                //����ġ ȹ��
                RewardRange = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1] / 4);
                RandomReward = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1]) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                //ī���� ���
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);

                CurrentEvent = FollowEventStorage[9031];
                break;
            case 1:
                //����ġ ȹ��
                RewardRange = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1] / 4);
                RandomReward = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1]) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);

                UIMgr.GI_UI.ActiveGettingUI(0, true);
                CurrentEvent = FollowEventStorage[9031];
                break;
            case 2:
                //����ġ ȹ��
                RewardRange = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1] / 4);
                RandomReward = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1]) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                //���׸��� �Ҹ�
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                //ī���� ���
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;

                UIMgr.GI_UI.ActiveGettingUI(0, true);
                JsonReadWriteManager.Instance.LkEv_Info.RestInPeace = true;
                CurrentEvent = FollowEventStorage[9032];
                break;
            case 3:
                //���׹̳��Ҹ�
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                //ī���� ���
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 2;

                JsonReadWriteManager.Instance.LkEv_Info.RestInPeace = true;
                CurrentEvent = FollowEventStorage[9032];
                break;
        }
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
        UIMgr.E_UI.ActiveEventUI(this);
    }
    */
    //------------------------------------------Event9040
    /*
    protected void Event9040(int ButtonType)
    {
        //0. �Ƿ� 900ȸ�� bk + 3
        //1. �Ƿ� 600ȸ�� bk + 1
        //2. �Ƿ� 300ȸ��
        //3. �Ƿ� 300ȸ��, -�������� ��� ����, gk + 1
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(900);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 3;
                break;
            case 1:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(600);
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                break;
            case 2:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300);
                break;
            case 3:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300);
                int RewardRange = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1] / 4);
                int RandomReward = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1]) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(-RandomReward, true);

                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;
                break;
        }
        CurrentEvent = FollowEventStorage[9041];
        PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
        UIMgr.E_UI.ActiveEventUI(this);
    }
    */
    //------------------------------------------Event101
    protected void Event101(int ButtonType)
    {
        //0. �������� �ʴ´�.
        //1. �� ��������. -> ��� ��´�// �κ��丮 �뷮�� �κ��丮 �ڵ尡 0�� ���� �ѳ� ������ Event�� ���ϵȴ�.
        //�� ��í ������ �°� ��� ȹ���Ѵ�.
        if(ButtonType == 1)
        {
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
    //------------------------------------------Event1101
}
