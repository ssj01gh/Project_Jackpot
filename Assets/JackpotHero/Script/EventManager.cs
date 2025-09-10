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
        //보스 이벤트 라면 -> 보스 이벤트는 한개면 될듯 보스 이벤트 코드 = 10000
        if(IsBossEvent == true)
        {
            CurrentEvent = EventStorage[BossTheme][0];
            P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
            return;
        }

        //DetailOfEvents는 1000 ~ 로 됨 1테마라면 1000~ 2테마 라면 2000~식이다
        if (DetailOfEvents != 0)//이벤트가 결정된 상태로 게임을 껏다 켰으면
        {
            int FixedEvnetThemeNum = DetailOfEvents / 1000;
            for (int i = 0; i < EventStorage[FixedEvnetThemeNum].Count; i++)
            {
                if (EventStorage[FixedEvnetThemeNum][i].EventCode == DetailOfEvents)//겹치는게 있다면
                {
                    CurrentEvent = EventStorage[FixedEvnetThemeNum][i];
                    CheckLinkageEvent();//연계가 있는 이벤트라면 연계가 어디까지 됬는지 체크해서 CurrentEvent를 바꿔줌
                    P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
                    JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                    return;//함수 종료
                }
            }
            if (FollowEventStorage.ContainsKey(DetailOfEvents))
            {
                CurrentEvent = FollowEventStorage[DetailOfEvents];
                CheckLinkageEvent();//연계가 있는 이벤트라면 연계가 어디까지 됬는지 체크해서 CurrentEvent를 바꿔줌
                P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
                JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                return;//함수 종료
            }
        }

        //여기 온거면 이벤트를 새로 결정해야 되는거//공통 이벤트와 해당 층 이벤트 둘중하나.
        int RandNum = Random.Range(0, CurrentFloorEventCount + CommonEventCount);
        if(RandNum >= 0 && RandNum < CurrentFloorEventCount)
        {//현재층 이벤트로 결정
            RandNum = Random.Range(0, EventStorage[ThemeNum].Count);
            CurrentEvent = EventStorage[ThemeNum][RandNum];
            CheckLinkageEvent();
            P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
            return;//함수 종료
        }
        else if(RandNum >= CurrentFloorEventCount && RandNum < CurrentFloorEventCount + CommonEventCount)
        {//공통 이벤트로 결정
            RandNum = Random.Range(0, EventStorage[EventTheme].Count);
            CurrentEvent = EventStorage[EventTheme][RandNum];
            CheckLinkageEvent();
            P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
            return;//함수 종료
        }

        //여기로 넘어가면 아무것도 안된거-- 그러니까 오류가 발생한거
        Debug.Log("ThereIsNoEvent");
    }

    protected void CheckLinkageEvent()//연계 이벤트 체크하는함수// 연계 이벤트 대부분은 여기서 결정될 예정
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
                        {//토토와 은혜 갚기
                            if (JsonReadWriteManager.Instance.LkEv_Info.TotoRepayFavor == true)
                            {//토토와 은혜갚기가 완료됬다면 토토의 지원으로
                                FindNChangeEvent(8060);
                            }
                            else
                            {//완료 안됬다면 토토의 은혜갚기로
                                FindNChangeEvent(8030);
                            }
                        }
                        else
                        {//토토와 저주받은 검// 토토와 축복받은 검
                            if(JsonReadWriteManager.Instance.LkEv_Info.TotoBlessedSword == true)
                            {//토토와 축복받은 검을 완료했으면 토토의 지원으로
                                FindNChangeEvent(8060);
                            }
                            else if(JsonReadWriteManager.Instance.LkEv_Info.TotoCursedSword == true)
                            {//토토의 저주받은 검을 완료 했으면 토토와 축복받은 검으로 가야함
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
                                {//저주받은검, 저주가 옅어진 검을 가지고 있다면
                                    FindNChangeEvent(8040);
                                }
                                else
                                {//안가지고 있으면 --> 토토의 은혜갚기 또는 토토의 지원으로 가야함
                                    if (JsonReadWriteManager.Instance.LkEv_Info.TotoRepayFavor == true)
                                    {//토토와 은혜갚기가 완료됬다면 토토의 지원으로
                                        FindNChangeEvent(8060);
                                    }
                                    else
                                    {//완료 안됬다면 토토의 은혜갚기로
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
                    {//여기에 걸려야 고인의 감사로
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
                {//8100으로
                    FindNChangeEvent(8100);
                }
                else if(JsonReadWriteManager.Instance.LkEv_Info.OminousSword == true)
                {
                    //불길한 검의 저주 8070
                    //검이 없으면 8090으로
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
                    else if (JsonReadWriteManager.Instance.LkEv_Info.CleanOminousSword == true)//옅어진 검의 저주 8080으로
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
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        PlayerMgr.GetPlayerInfo().EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화
    }

    public void PressEventSelectionButton(int SelectionType)//이게 이제 이벤트의 결과 함수임
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        OccurEventBySelection(SelectionType);//여기서 이벤트의 선택에 맞게 이벤트가 발생함
        /*
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        PlayerMgr.GetPlayerInfo().EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화
        *///위에꺼 2개는 상태에 맞게 각각의 이벤트에서 발생해야 할듯 -> 장비를 얻고, EXP를 얻는건 괜찮지만 배틀로 가야한다면 대참사임
        JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        UIMgr.SetUI();
        //JsonReadWriteManager.Instance.P_Info = PlayerMgr.GetPlayerInfo().GetPlayerStateInfo();//갱신
        //StartCoroutine(CheckBackGroundMoveEnd(true));//여기에 들어가면 현재 상태에 맞게 ui가 생성되고 없어지고 함
        //SetUI와 전투를 위한 CheckBackGroundMoveEnd 코루틴임
        //UIMgr.SetUI(PlayerMgr);
    }

    public void OccurEventBySelection(int ButtonType)//PlaySceneManager에서 전달
    {//이건 버튼이 눌린거
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
        /*이거 2개는 공통적이지 않음, EXP나 장비를 얻는다면 상관X 그러나 전투로 연결되면 대참사임
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        PlayerInfo.EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화
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
        //0. 아무일 없음
        //1. 저주받은 검 획득, 이거 선택시 다시 발생 x
        switch (ButtonType)
        {
            case 0:
                CurrentEvent = FollowEventStorage[1061];
                break;
            case 1:
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                //저주받은 검 코드 = 17001
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
        //0. 피로도 300회복
        //1. gk + 1
        //2. 체력 -30 피로도 - 300 gk + 3
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
        //0. 스테이지 평균 티어 장비, 평균적 보상, bk + 1
        //1. 평균적 보상
        //2. 평균적 보상 , 피로도 -300, gk + 1
        //3. 피로도 -300, gk + 2
        int RewardRange = 0;
        int RandomReward = 0;
        switch(ButtonType)
        {
            case 0:
                //장비 획득
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
                {
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                int RandomEquipment = EquipmentInfoManager.Instance.GetFixedTierRandomEquipmnet(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor);
                PlayerMgr.GetPlayerInfo().PutEquipmentToInven(RandomEquipment);
                //경험치 획득
                RewardRange = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1] / 4);
                RandomReward = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1]) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                //카르마 계산
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().BadKarma += 1;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);

                CurrentEvent = FollowEventStorage[9031];
                break;
            case 1:
                //경험치 획득
                RewardRange = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1] / 4);
                RandomReward = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1]) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);

                UIMgr.GI_UI.ActiveGettingUI(0, true);
                CurrentEvent = FollowEventStorage[9031];
                break;
            case 2:
                //경험치 획득
                RewardRange = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1] / 4);
                RandomReward = (int)(StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1]) + Random.Range(-RewardRange, RewardRange + 1);
                PlayerMgr.GetPlayerInfo().SetPlayerEXPAmount(RandomReward);
                //스테마나 소모
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                //카르마 계산
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().GoodKarma += 1;

                UIMgr.GI_UI.ActiveGettingUI(0, true);
                JsonReadWriteManager.Instance.LkEv_Info.RestInPeace = true;
                CurrentEvent = FollowEventStorage[9032];
                break;
            case 3:
                //스테미나소모
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300);
                //카르마 계산
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
        //0. 피로 900회복 bk + 3
        //1. 피로 600회복 bk + 1
        //2. 피로 300회복
        //3. 피로 300회복, -스테이지 평균 보상, gk + 1
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
        //0. 가져가지 않는다.
        //1. 다 가져간다. -> 장비를 얻는다// 인벤토리 용량에 인벤토리 코드가 0인 놈이 한놈도 없으면 Event는 리턴된다.
        //내 갓챠 레벨에 맞게 장비를 획득한다.
        if(ButtonType == 1)
        {
            if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
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
        //0. 가져가지 않는다.
        //1. 조금만 가져간다.-> 약간의 EXP 획득한다. 100~ 150사이?
        //2. 다 가져간다.장비를 얻는다// 인벤토리 용량에 인벤토리 코드가 0인 놈이 한놈도 없으면 Event는 리턴된다.
        //내 갓챠 레벨에 맞게 장비를 획득한다.
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
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
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
        //0. 가져가지 않는다.
        //1. 조금만 가져간다.//exp 획득 100 ~ 150사이
        //2. 다 가져간다.//장비 획득
        //3. 주변에 아무도 없나 살펴본다.//전투 시작
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
                if (PlayerMgr.GetPlayerInfo().IsInventoryFull() == true)//인벤토리가 꽉찼다면
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
                UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
                BattleMgr.InitCurrentBattleMonsters();
                BattleMgr.InitMonsterNPlayerActiveGuage();
                BattleMgr.ProgressBattle();
                break;
        }
    }
    //------------------------------------------Event1101
}
