using JetBrains.Annotations;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    private PlayerManager PlayerMgr;
    [SerializeField]
    private PlaySceneUIManager UIMgr;
    [SerializeField]
    private BattleManager BattleMgr;

    //public EventSO[] CommonGameEvents;
    //public EventSO[] LinkageGameEvents;
    //public EventSO[] Stage01GameEvents;
    //public EventSO[] Stage02GameEvents;
    //public EventSO[] Stage03GameEvents;
    //public EventSO[] Stage04GameEvents;
    //
    //public EventSO[] CommonFollowEvents;
    //public EventSO[] LinkageFollowEvents;
    //public EventSO[] Stage01FollowEvents;
    //public EventSO[] Stage02FollowEvents;
    //public EventSO[] Stage03FollowEvents;
    //public EventSO[] Stage04FollowEvents;

    [Header("EventCodes")]
    public int[] StartEventCodes;
    public int[] FollowEventCodes;

    // Start is called before the first frame update

    //Dictionary<int, List<EventSO>> EventStorage = new Dictionary<int, List<EventSO>>();
    //Dictionary<int, EventSO> FollowEventStorage = new Dictionary<int, EventSO>();
    Dictionary<int, List<int>> EventCodeStorage = new Dictionary<int, List<int>>();
    HashSet<int> FollowEventCodeStorage = new HashSet<int>();

    private AsyncOperationHandle<EventSO> _Handle;
    private const int BossTheme = 10;
    private const int EventTheme = 9;
    private readonly float[] StageAverageRward = new float[4] { 28, 101, 313, 816 };
    public EventSOInfo CurrentEvent { protected set; get; } = new EventSOInfo();
    protected EventSO CurrentKeyEvent;
    //protected EventSO CurrentKeyEvent = new EventSO();

    [HideInInspector]
    public string Getting = "";
    [HideInInspector]
    public string Losing = "";
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
        /*
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
        */

        foreach (int EventCode in StartEventCodes)
        {
            int ThemeNum = EventCode / 1000;
            if (!EventCodeStorage.ContainsKey(ThemeNum))
            {
                List<int> EventCodeList = new List<int>();
                EventCodeList.Add(EventCode);
                EventCodeStorage.Add(ThemeNum, EventCodeList);
            }
            else
                EventCodeStorage[ThemeNum].Add(EventCode);
        }

        foreach(int FollowEventCode in FollowEventCodes)
        {
            FollowEventCodeStorage.Add(FollowEventCode);
        }
    }
    public async void SetAddresableEvent(bool IsBossEvent = false)
    {
        PlayerScript P_Info = PlayerMgr.GetPlayerInfo();
        int DetailOfEvents = P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails;
        int ThemeNum = DetailOfEvents / 1000;
        FreeEventHandle();

        //보스 이벤트로 적용
        if (IsBossEvent == true)
        {
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(10000));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoBossEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
            StartCoroutine(LoadEvent());
            //UIMgr.E_UI.ActiveEventUI(this);
            return;
        }
        //말하는 몬스터를 한번도 본적이 없다면
        if (JsonReadWriteManager.Instance.LkEv_Info.IsMeetTalkingMonster == false)
        {
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(9020));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoTalkingMonsterEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.LkEv_Info.IsMeetTalkingMonster = true;
            JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
            StartCoroutine(LoadEvent());
            return;
        }
        if (DetailOfEvents != 0)//이벤트가 결정된 상태로 게임을 껏다 켰으면
        {//이벤트가 있는지 없는지 확인해야하는데.....
            //이벤트 전체에서 있는지 확인 -> 이벤트 결과에서 있는지 확인.....
            //시작 이벤트 전체 = EventCodeStorage[스테이지][코드]-> 스테이지가 정해지면 거기에서 랜덤으로 뽑는것 도 가능
            //연계 이벤트 전체 = FollowEventCodeStroage[코드] -> 있는지 바로 검사 가능
            for (int i = 0; i < EventCodeStorage[ThemeNum].Count; i++)
            {
                if (EventCodeStorage[ThemeNum][i] == DetailOfEvents)//겹치는게 있다면
                {
                    int EC = CheckLinkageAddresableEvent(DetailOfEvents);//여기서 연계이벤트가 있다면 코드가 바뀜
                    _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(EC));
                    await _Handle.Task;//불러 와질 때까지 비동기 대기
                    if (_Handle.Status != AsyncOperationStatus.Succeeded)
                    {
                        //Debug.LogError("NoEvent");
                        return;
                    }
                    CurrentKeyEvent = _Handle.Result;
                    P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
                    JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                    StartCoroutine(LoadEvent());
                    return;//함수 종료
                }
            }
            if (FollowEventCodes.Contains(DetailOfEvents))
            {
                int EC = CheckLinkageAddresableEvent(DetailOfEvents);//여기서 연계이벤트가 있다면 코드가 바뀜
                _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(EC));
                await _Handle.Task;//불러 와질 때까지 비동기 대기
                if (_Handle.Status != AsyncOperationStatus.Succeeded)
                {
                    //Debug.LogError("NoEvent");
                    return;
                }
                CurrentKeyEvent = _Handle.Result;
                P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
                JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                StartCoroutine(LoadEvent());
                return;//함수 종료
            }
        }

        if (P_Info.GetPlayerStateInfo().CurrentFloor == 4)
        {
            int Stage04EventCount = JsonReadWriteManager.Instance.LkEv_Info.Stage04EventCount;
            int EC = EventCodeStorage[4][Stage04EventCount];
            //0 -> 4000//1 -> 4010//2 -> 4020//3 -> 4030//4 -> 4040//5 -> 4050//6 -> 4060//7 -> 4070//
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(EC));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
            StartCoroutine(LoadEvent());
            return;//함수 종료
        }
        else
        {
            int CurrentFloor = P_Info.GetPlayerStateInfo().CurrentFloor;
            //현재 있는 스테이지의 이벤트 개수
            int CurrentFloorEventCount = EventCodeStorage[CurrentFloor].Count;
            //공통 이벤트 개수(9로 시작하는 것들)
            int CommonEventCount = EventCodeStorage[EventTheme].Count;

            int RandNum = Random.Range(0, CurrentFloorEventCount + CommonEventCount);
            if (RandNum >= 0 && RandNum < CurrentFloorEventCount)
            {//현재층 이벤트로 결정
                RandNum = Random.Range(0, EventCodeStorage[CurrentFloor].Count);
                int EC = CheckLinkageAddresableEvent(EventCodeStorage[CurrentFloor][RandNum]);
                _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(EC));
                await _Handle.Task;//불러 와질 때까지 비동기 대기
                if (_Handle.Status != AsyncOperationStatus.Succeeded)
                {
                    //Debug.LogError("NoEvent");
                    return;
                }
                CurrentKeyEvent = _Handle.Result;
                P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
                JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                StartCoroutine(LoadEvent());
                return;//함수 종료
            }
            else if (RandNum >= CurrentFloorEventCount && RandNum < CurrentFloorEventCount + CommonEventCount)
            {//공통 이벤트로 결정
                RandNum = Random.Range(0, EventCodeStorage[EventTheme].Count);
                int EC = CheckLinkageAddresableEvent(EventCodeStorage[EventTheme][RandNum]);
                _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(EC));
                await _Handle.Task;//불러 와질 때까지 비동기 대기
                if (_Handle.Status != AsyncOperationStatus.Succeeded)
                {
                    //Debug.LogError("NoEvent");
                    return;
                }
                CurrentKeyEvent = _Handle.Result;
                P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
                JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                StartCoroutine(LoadEvent());
                return;//함수 종료
            }
        }
    }
    protected string GetAdressByEventCode(int EventCode)
    {
        return "Event/Event" + EventCode.ToString();
    }
    protected void FreeEventHandle()
    {
        if (_Handle.IsValid())//살아있는 핸들이라면 해방
        {
            Addressables.Release(_Handle);
        }
    }
    protected int CheckLinkageAddresableEvent(int CurrentEventCode)
    {
        int ReturnEventCode = CurrentEventCode;
        switch(CurrentEventCode)
        {
            case 9020:
                if (JsonReadWriteManager.Instance.LkEv_Info.TalkingMonster == true)
                {
                    if (JsonReadWriteManager.Instance.LkEv_Info.TalkingDirtGolem == true)
                    {
                        int Rand = Random.Range(0, 2);
                        if (Rand == 0)
                        {//토토와 은혜 갚기
                            if (JsonReadWriteManager.Instance.LkEv_Info.TotoRepayFavor == true)
                            {//토토와 은혜갚기가 완료됬다면 토토의 지원으로
                                ReturnEventCode =FindNChangeAddresableEvent(9020, 8060);
                            }
                            else
                            {//완료 안됬다면 토토의 은혜갚기로
                                ReturnEventCode = FindNChangeAddresableEvent(9020, 8030);
                            }
                        }
                        else
                        {//토토와 저주받은 검// 토토와 축복받은 검
                            if (JsonReadWriteManager.Instance.LkEv_Info.TotoBlessedSword == true)
                            {//토토와 축복받은 검을 완료했으면 토토의 지원으로
                                ReturnEventCode = FindNChangeAddresableEvent(9020, 8060);
                            }
                            else if (JsonReadWriteManager.Instance.LkEv_Info.TotoCursedSword == true)
                            {//토토의 저주받은 검을 완료 했으면 토토와 축복받은 검으로 가야함
                                ReturnEventCode = FindNChangeAddresableEvent(9020,8050);
                            }
                            else
                            {
                                int CursedSword = 23000;
                                int SmallCursedSword = 24001;
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
                                    ReturnEventCode = FindNChangeAddresableEvent(9020, 8040);
                                }
                                else
                                {//안가지고 있으면 --> 토토의 은혜갚기 또는 토토의 지원으로 가야함
                                    if (JsonReadWriteManager.Instance.LkEv_Info.TotoRepayFavor == true)
                                    {//토토와 은혜갚기가 완료됬다면 토토의 지원으로
                                        ReturnEventCode = FindNChangeAddresableEvent(9020, 8060);
                                    }
                                    else
                                    {//완료 안됬다면 토토의 은혜갚기로
                                        ReturnEventCode = FindNChangeAddresableEvent(9020, 8030);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ReturnEventCode = FindNChangeAddresableEvent(9020, 8010);
                    }
                }
                break;
            case 9030:
                if (JsonReadWriteManager.Instance.LkEv_Info.RestInPeace == true)
                {
                    int Rand = Random.Range(0, 2);
                    if (Rand == 0)
                    {//여기에 걸려야 고인의 감사로
                        ReturnEventCode = FindNChangeAddresableEvent(9030, 8020);
                    }
                }
                break;
            case 9050:
                if (JsonReadWriteManager.Instance.LkEv_Info.OminousSword == true &&
                    JsonReadWriteManager.Instance.LkEv_Info.CleanOminousSword == false &&
                    JsonReadWriteManager.Instance.LkEv_Info.TotoCursedSword == false)
                {//여기에서 이미 토토에게 검이 정화되러 가지고 갔으면.....
                    //+토토와 저주받은 검이 == false 일때 바꿔야함
                    ReturnEventCode = FindNChangeAddresableEvent(9050, 8000);
                }
                break;
            case 9080:
                if (JsonReadWriteManager.Instance.LkEv_Info.TotoBlessedSword == true)
                {//8100으로
                    ReturnEventCode = FindNChangeAddresableEvent(9080, 8100);
                }
                else if (JsonReadWriteManager.Instance.LkEv_Info.OminousSword == true)
                {
                    //불길한 검의 저주 8070
                    //검이 없으면 8090으로
                    int CursedSword = 23000;
                    int SmallCursedSword = 24001;
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

                    if (IsHaveCursedSword == false)
                        ReturnEventCode = FindNChangeAddresableEvent(9080, 8090);
                    else if (JsonReadWriteManager.Instance.LkEv_Info.CleanOminousSword == true)//옅어진 검의 저주 8080으로
                        ReturnEventCode = FindNChangeAddresableEvent(9080, 8080);
                    else
                        ReturnEventCode = FindNChangeAddresableEvent(9080, 8070);

                }
                break;
            case 2000:
                if (JsonReadWriteManager.Instance.LkEv_Info.ForestBracelet == true)
                {
                    ReturnEventCode = FindNChangeAddresableEvent(2000, 8110);
                }
                break;
            case 2040:
                if (JsonReadWriteManager.Instance.LkEv_Info.ML_GKPerson == true ||
                    JsonReadWriteManager.Instance.LkEv_Info.ML_NorPerson == true)
                {//선 혹은 중립일때
                    ReturnEventCode = FindNChangeAddresableEvent(2040, 8120);
                }
                else if (JsonReadWriteManager.Instance.LkEv_Info.ML_BKPerson == true)
                {//악일때
                    ReturnEventCode = FindNChangeAddresableEvent(2040, 8130);
                }
                break;
            case 3020:
                if (JsonReadWriteManager.Instance.LkEv_Info.Lab_Security == true)
                {
                    ReturnEventCode = FindNChangeAddresableEvent(3020, 8140);
                }
                break;
            case 3040:
                if (JsonReadWriteManager.Instance.LkEv_Info.Lab_Sphere == true)
                {
                    ReturnEventCode = FindNChangeAddresableEvent(3040, 8150);
                }
                break;
            case 2050:
                if (JsonReadWriteManager.Instance.LkEv_Info.IsMeetTalkingGiant == true)
                {
                    ReturnEventCode = FindNChangeAddresableEvent(2050, 8160);
                }
                break;
            case 3060:
                if (JsonReadWriteManager.Instance.LkEv_Info.IsMeetTalkingDopple == true)
                {
                    ReturnEventCode = FindNChangeAddresableEvent(3060, 8170);
                }
                break;
        }
        return ReturnEventCode;
    }
    protected int FindNChangeAddresableEvent(int CurrentEventCode ,int ChangeEventCode)
    {
        int EventTheme = ChangeEventCode / 1000;
        for (int i = 0; i < EventCodeStorage[EventTheme].Count; i++)
        {
            if (EventCodeStorage[EventTheme][i] == ChangeEventCode)
            {
                return EventCodeStorage[EventTheme][i];
            }
        }
        return CurrentEventCode;
    }
    /*
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
                                int CursedSword = 23000;
                                int SmallCursedSword = 24001;
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
                    JsonReadWriteManager.Instance.LkEv_Info.CleanOminousSword == false &&
                    JsonReadWriteManager.Instance.LkEv_Info.TotoCursedSword)
                {//여기에서 이미 토토에게 검이 정화되러 가지고 갔으면.....
                    //+토토와 저주받은 검이 == false 일때 바꿔야함
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
            case 2000:
                if(JsonReadWriteManager.Instance.LkEv_Info.ForestBracelet == true)
                {
                    FindNChangeEvent(8110);
                }
                break;
            case 2040:
                if(JsonReadWriteManager.Instance.LkEv_Info.ML_GKPerson == true || 
                    JsonReadWriteManager.Instance.LkEv_Info.ML_NorPerson == true)
                {//선 혹은 중립일때
                    FindNChangeEvent(8120);
                }
                else if(JsonReadWriteManager.Instance.LkEv_Info.ML_BKPerson == true)
                {//악일때
                    FindNChangeEvent(8130);
                }
                break;
            case 3020:
                if(JsonReadWriteManager.Instance.LkEv_Info.Lab_Security == true)
                {
                    FindNChangeEvent(8140);
                }
                break;
            case 3040:
                if(JsonReadWriteManager.Instance.LkEv_Info.Lab_Sphere == true)
                {
                    FindNChangeEvent(8150);
                }
                break;
            case 2050:
                if(JsonReadWriteManager.Instance.LkEv_Info.IsMeetTalkingGiant == true)
                {
                    FindNChangeEvent(8160);
                }
                break;
            case 3060:
                if(JsonReadWriteManager.Instance.LkEv_Info.IsMeetTalkingDopple == true)
                {
                    FindNChangeEvent(8170);
                }
                break;
        }
    }
    */
    /*
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
    */
    protected void EndOfEvent()
    {
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        PlayerMgr.GetPlayerInfo().EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화
        FreeEventHandle();
    }

    public void PressEventSelectionButton(int SelectionType)//이게 이제 이벤트의 결과 함수임
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        //Debug.Log("ClickEventButton");
        OccurEventBySelection(SelectionType);//여기서 이벤트의 선택에 맞게 이벤트가 발생함
        /*
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        PlayerMgr.GetPlayerInfo().EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화
        *///위에꺼 2개는 상태에 맞게 각각의 이벤트에서 발생해야 할듯 -> 장비를 얻고, EXP를 얻는건 괜찮지만 배틀로 가야한다면 대참사임
          //이거 밑어꺼를 각 ThemeEvent 아래쪽에 박아야 할듯? 그래야 비동기 진행이랑 충돌 안할듯함
        //JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
        //UIMgr.SetUI();
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

    protected async void CommonThemeEvent(int ButtonType)
    {
        CommonEventDetailAction CommonEvents = new CommonEventDetailAction();
        int FollowEventCode = 0;
        int CurrentStageReward = (int)StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1];
        switch (CurrentEvent.EventCode)
        {
            case 9000:
                FollowEventCode = CommonEvents.Event9000(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 9010:
                FollowEventCode = CommonEvents.Event9010(ButtonType, CurrentStageReward, PlayerMgr, ref Getting, ref Losing);
                break;
            case 9020:
                FollowEventCode = CommonEvents.Event9020(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 9030:
                FollowEventCode = CommonEvents.Event9030(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 9040:
                FollowEventCode = CommonEvents.Event9040(ButtonType, CurrentStageReward, PlayerMgr, ref Getting, ref Losing);
                break;
            case 9050:
                FollowEventCode = CommonEvents.Event9050(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 9060:
                FollowEventCode = CommonEvents.Event9060(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
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
                FollowEventCode = CommonEvents.Event9080(ButtonType, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 10000:
                CommonEvents.Event10000(ButtonType, PlayerMgr);
                //Debug.Log("Boss");
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
        {
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            UIMgr.SetUI();
            return;
        }
        else if (FollowEventCodeStorage.Contains(FollowEventCode))
        {
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(FollowEventCode));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            StartCoroutine(LoadEvent(true));
        }
    }
    protected async void LinkageThemeEvent(int ButtonType)
    {
        LinkageEventDetailAction LinkageEvent = new LinkageEventDetailAction();
        int FollowEventCode = 0;
        int CurrentStageReward = (int)StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1];
        switch (CurrentEvent.EventCode)
        {
            case 8000:
                FollowEventCode = LinkageEvent.Event8000(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 8010:
                FollowEventCode = LinkageEvent.Event8010(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 8020:
                FollowEventCode = LinkageEvent.Event8020(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 8030:
                FollowEventCode = LinkageEvent.Event8030(ButtonType, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 8040:
                FollowEventCode = LinkageEvent.Event8040(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 8050:
                FollowEventCode = LinkageEvent.Event8050(ButtonType, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 8060:
                FollowEventCode = LinkageEvent.Event8060(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 8070:
                FollowEventCode = LinkageEvent.Event8070(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 8080:
                FollowEventCode = LinkageEvent.Event8080();
                break;
            case 8090:
                FollowEventCode = LinkageEvent.Event8090(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 8100:
                FollowEventCode = LinkageEvent.Event8100(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 8110:
                FollowEventCode = LinkageEvent.Event8110(PlayerMgr, ref Getting, ref Losing);
                break;
            case 8120:
                FollowEventCode = LinkageEvent.Event8120();
                break;
            case 8130:
                FollowEventCode = LinkageEvent.Event8130();
                break;
            case 8140:
                FollowEventCode = LinkageEvent.Event8140(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 8141:
                LinkageEvent.Event8141(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 8150:
                FollowEventCode = LinkageEvent.Event8150(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 8151:
                LinkageEvent.Event8151(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 8160:
                FollowEventCode = LinkageEvent.Event8160(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 8170:
                FollowEventCode = LinkageEvent.Event8170(ButtonType, PlayerMgr, ref Getting, ref Losing);
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
            case 8111:
            case 8121:
            case 8131:
            case 8142:
            case 8152:
            case 8153:
            case 8161:
            case 8162:
            case 8171:
            case 8172:
                CommonFollowEvent();
                break;
        }

        if (CurrentEvent.EventCode == FollowEventCode || FollowEventCode == 0)
        {
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            UIMgr.SetUI();
            return;
        }
        else if (FollowEventCodeStorage.Contains(FollowEventCode))
        {
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(FollowEventCode));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            StartCoroutine(LoadEvent(true));
        }
    }
    protected async void Stage01ThemeEvent(int ButtonType)
    {
        Stage01EventDetailAction Stage01Event = new Stage01EventDetailAction();
        int FollowEventCode = 0;
        int CurrentStageReward = (int)StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1];
        switch (CurrentEvent.EventCode)
        {
            case 1000:
                FollowEventCode = Stage01Event.Event1000(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 1010:
                FollowEventCode = Stage01Event.Event1010(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 1013:
                Stage01Event.Event1013(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 1020:
                FollowEventCode = Stage01Event.Event1020(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 1022:
            case 1023:
                Stage01Event.Event1022_3(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 1030:
                FollowEventCode = Stage01Event.Event1030(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 1032:
                Stage01Event.Event1032(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 1040:
                FollowEventCode = Stage01Event.Event1040(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 1050:
                FollowEventCode = Stage01Event.Event1050(ButtonType, PlayerMgr, ref Getting, ref Losing);
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
        {
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            UIMgr.SetUI();
            return;
        }
        else if (FollowEventCodeStorage.Contains(FollowEventCode))
        {
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(FollowEventCode));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            StartCoroutine(LoadEvent(true));
        }
    }
    protected async void Stage02ThemeEvent(int ButtonType)
    {
        Stage02EventDetailAction Stage02Event = new Stage02EventDetailAction();
        int FollowEventCode = 0;
        int CurrentStageReward = (int)StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1];
        switch (CurrentEvent.EventCode)
        {
            case 2000:
                FollowEventCode = Stage02Event.Event2000(ButtonType, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 2001:
                FollowEventCode = Stage02Event.Event2001(ButtonType, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 2002:
                FollowEventCode = Stage02Event.Event2002(ButtonType, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 2003:
                FollowEventCode = Stage02Event.Event2003(ButtonType, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 2004:
                FollowEventCode = Stage02Event.Event2004(ButtonType, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 2006:
                FollowEventCode = Stage02Event.Event2006(ButtonType);
                break;
            case 2010:
                FollowEventCode = Stage02Event.Event2010(ButtonType, ref Getting, ref Losing);
                break;
            case 2020:
                FollowEventCode = Stage02Event.Event2020(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 2021:
                FollowEventCode = Stage02Event.Event2021(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 2022:
                FollowEventCode = Stage02Event.Event2022(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 2025:
                Stage02Event.Event2025(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 2030:
                FollowEventCode = Stage02Event.Event2030(PlayerMgr, ref Getting, ref Losing);
                break;
            case 2032:
                Stage02Event.Event2032(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 2040:
                FollowEventCode = Stage02Event.Event2040(PlayerMgr, ref Getting, ref Losing);
                break;
            case 2043:
                Stage02Event.Event2043(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 2050:
                FollowEventCode = Stage02Event.Event2050(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 2051:
            case 2054:
                Stage02Event.Event2051(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 2005:
            case 2007:
            case 2011:
            case 2012:
            case 2013:
            case 2023:
            case 2024:
            case 2031:
            case 2041:
            case 2042:
            case 2052:
            case 2053:
                CommonFollowEvent();
                break;
        }
        if (CurrentEvent.EventCode == FollowEventCode || FollowEventCode == 0)
        {
            if(FollowEventCode == 2022)
            {
                _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(2022));
                await _Handle.Task;//불러 와질 때까지 비동기 대기
                if (_Handle.Status != AsyncOperationStatus.Succeeded)
                {
                    //Debug.LogError("NoEvent");
                    return;
                }
                CurrentKeyEvent = _Handle.Result;
                PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
                JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
                StartCoroutine(LoadEvent(true));
            }
            else
            {
                JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
                UIMgr.SetUI();
                return;
            }
            return;
        }
        else if (FollowEventCodeStorage.Contains(FollowEventCode))
        {
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(FollowEventCode));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            StartCoroutine(LoadEvent(true));
        }
        else if (FollowEventCode == 2000)
        {
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(2000));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            StartCoroutine(LoadEvent(true));
        }
    }
    protected async void Stage03ThemeEvent(int ButtonType)
    {
        Stage03EventDetailAction Stage03Event = new Stage03EventDetailAction();
        int FollowEventCode = 0;
        int CurrentStageReward = (int)StageAverageRward[PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor - 1];

        switch(CurrentEvent.EventCode)
        {
            case 3000:
                FollowEventCode = Stage03Event.Event3000(ButtonType, ref Getting, ref Losing);
                break;
            case 3010:
                FollowEventCode = Stage03Event.Event3010(ButtonType, PlayerMgr, ref Getting, ref Losing);
                break;
            case 3020:
                FollowEventCode = Stage03Event.Event3020();
                break;
            case 3021:
                Stage03Event.Event3021(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 3030:
                FollowEventCode = Stage03Event.Event3030(ButtonType, PlayerMgr);
                break;
            case 3031:
                Stage03Event.Event3031(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 3040:
                FollowEventCode = Stage03Event.Event3040(ButtonType);
                break;
            case 3041:
                Stage03Event.Event3041(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 3050:
                FollowEventCode = Stage03Event.Event3050(ButtonType, ref Getting, ref Losing);
                break;
            case 3060:
                FollowEventCode = Stage03Event.Event3060(ButtonType, CurrentStageReward, PlayerMgr, UIMgr, ref Getting, ref Losing);
                break;
            case 3001:
            case 3002:
            case 3003:
            case 3011:
            case 3012:
            case 3013:
            case 3014:
            case 3032:
            case 3042:
            case 3051:
            case 3052:
            case 3061:
            case 3062:
            case 3063:
                CommonFollowEvent();
                break;
        }

        if (CurrentEvent.EventCode == FollowEventCode || FollowEventCode == 0)
        {
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            UIMgr.SetUI();
            return;
        }
        else if (FollowEventCodeStorage.Contains(FollowEventCode))
        {
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(FollowEventCode));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            StartCoroutine(LoadEvent(true));
        }
    }
    protected async void Stage04ThemeEvent(int ButtonType)
    {
        Stage04EventDetailAction Stage04Event = new Stage04EventDetailAction();
        int FollowEventCode = 0;
        Getting = "";
        Losing = "";
        switch (CurrentEvent.EventCode)
        {
            case 4000:
                FollowEventCode = Stage04Event.Event4000();
                break;
            case 4010:
                FollowEventCode = Stage04Event.Event4010(ButtonType);
                break;
            case 4011:
                Stage04Event.Event4011(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 4020:
                FollowEventCode = Stage04Event.Event4020(ButtonType);
                break;
            case 4021:
                Stage04Event.Event4021(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 4030:
                FollowEventCode = Stage04Event.Event4030(ButtonType);
                break;
            case 4031:
                Stage04Event.Event4031(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 4040:
                FollowEventCode = Stage04Event.Event4040(ButtonType);
                break;
            case 4041:
                Stage04Event.Event4041(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 4050:
                FollowEventCode = Stage04Event.Event4050(ButtonType);
                break;
            case 4051:
                Stage04Event.Event4051(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 4060:
                FollowEventCode = Stage04Event.Event4060(ButtonType);
                break;
            case 4061:
                Stage04Event.Event4061(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 4070:
                FollowEventCode = Stage04Event.Event4070(ButtonType);
                break;
            case 4071:
            case 4072:
                Stage04Event.Event4071_2(PlayerMgr, UIMgr, BattleMgr);
                break;
            case 4001:
            case 4012:
                CommonFollowEvent();
                break;
        }

        if (CurrentEvent.EventCode == FollowEventCode || FollowEventCode == 0)
        {
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            UIMgr.SetUI();
            return;
        }
        else if (FollowEventCodeStorage.Contains(FollowEventCode))
        {
            _Handle = Addressables.LoadAssetAsync<EventSO>(GetAdressByEventCode(FollowEventCode));
            await _Handle.Task;//불러 와질 때까지 비동기 대기
            if (_Handle.Status != AsyncOperationStatus.Succeeded)
            {
                //Debug.LogError("NoEvent");
                return;
            }
            CurrentKeyEvent = _Handle.Result;
            PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentKeyEvent.EventCode;
            JsonReadWriteManager.Instance.SavePlayerInfo(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
            StartCoroutine(LoadEvent(true));
        }
    }
    protected void CommonFollowEvent()
    {
        EndOfEvent();
    }

    private IEnumerator LoadEvent(bool IsUseSetUI = false)
    {
        //CurrentEvent에 이미 코드가 들어가 있음..... 하나씩 딴거로 바꾼다?
        //CurrentEvent = CurrentKeyEvent;
        CurrentEvent.EventCode = CurrentKeyEvent.EventCode;
        CurrentEvent.EventImage = CurrentKeyEvent.EventImage;
        CurrentEvent.EventTitle = CurrentKeyEvent.EventTitle;
        CurrentEvent.EventDetail = CurrentKeyEvent.EventDetail;
        CurrentEvent.EventSelectionDetail.Clear();
        for(int i = 0; i < CurrentKeyEvent.EventSelectionDetail.Length; i++)
        {
            CurrentEvent.EventSelectionDetail.Add(CurrentKeyEvent.EventSelectionDetail[i]);
        }

        yield return LocalizationSettings.InitializationOperation;

        var EventTitleTable = LocalizationSettings.StringDatabase.GetTable("EventTitleText");
        var EventDetailTable = LocalizationSettings.StringDatabase.GetTable("EventDetailText");
        var EventSelectionDetailTable = LocalizationSettings.StringDatabase.GetTable("EventSelectionDetailText");

        CurrentEvent.EventTitle = EventTitleTable.GetEntry(CurrentEvent.EventTitle)?.GetLocalizedString();
        CurrentEvent.EventDetail = EventDetailTable.GetEntry(CurrentEvent.EventDetail)?.GetLocalizedString();
        for(int i = 0; i < CurrentEvent.EventSelectionDetail.Count; i++)
        {
            string EvetnSelectionDetailKey = CurrentEvent.EventSelectionDetail[i];
            CurrentEvent.EventSelectionDetail[i] = EventSelectionDetailTable.GetEntry(EvetnSelectionDetailKey)?.GetLocalizedString();
        }

        UIMgr.E_UI.ActiveEventUI(this);
        if(IsUseSetUI == true)
            UIMgr.SetUI();
    }

    public void ChangeLangueWhenEventUIOpen()
    {
        if(UIMgr.E_UI.gameObject.activeSelf == true && CurrentKeyEvent.EventCode == PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails)
        {//이거 두개가 같을때 = 옳은 이벤트가 켜져있을때 라고 해야하려나
            StartCoroutine(LoadEvent());
        }
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
    //------------------------------------------Event1101
}
