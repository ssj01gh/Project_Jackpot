using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    private PlayerManager PlayerMgr;
    [SerializeField]
    private PlaySceneUIManager UIMgr;
    [SerializeField]
    private BattleManager BattleMgr;

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

        //DetailOfEvents는 101 ~ 로 됨 1테마라면 101~ 2테마 라면 201~식이다//이벤트가 결정된 상태로 게임을 껏다 켰으면
        if (DetailOfEvents != 0)
        {
            int ThemeOfEvent = DetailOfEvents / 100;
            //PatternStorage[DetailOfEvents / 100];이게 이제 저장된 적들의 스폰 패턴중 하나// 여기서 찾아야함
            for (int i = 0; i < EventStorage[ThemeOfEvent].Count; i++)
            {
                if (EventStorage[ThemeOfEvent][i].EventCode == DetailOfEvents)//겹치는게 있다면
                {
                    CurrentEvent = EventStorage[ThemeOfEvent][i];
                    P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
                    JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
                    return;//함수 종료
                }
            }
        }

        //모든 테마를 다 포함한 랜덤값
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
        else//-1이 아닐때 들어온 테마 값에 맞게 결정
        {
            int Rand = Random.Range(0, EventStorage[ThemeNum].Count);
            CurrentEvent = EventStorage[ThemeNum][Rand];
        }
        P_Info.GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentEvent.EventCode;
        JsonReadWriteManager.Instance.SavePlayerInfo(P_Info.GetPlayerStateInfo());
    }

    protected void EndOfEvent()
    {
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        PlayerMgr.GetPlayerInfo().EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화
    }

    public void PressEventSelectionButton(int SelectionType)//이게 이제 이벤트의 결과 함수임
    {
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
        /*이거 2개는 공통적이지 않음, EXP나 장비를 얻는다면 상관X 그러나 전투로 연결되면 대참사임
        UIMgr.E_UI.InActiveEventUI();//버튼 이 눌렸으니 이벤트를 종료 한다.
        PlayerInfo.EndOfAction();//여기서 Action, ActionDetail이 초기화, 현재 행동 초기화
        */
    }



    //------------------------------------------Event101
    protected void Event101(int ButtonType)
    {
        //0. 가져가지 않는다.
        //1. 다 가져간다. -> 장비를 얻는다// 인벤토리 용량에 인벤토리 코드가 0인 놈이 한놈도 없으면 Event는 리턴된다.
        //내 갓챠 레벨에 맞게 장비를 획득한다.
        if(ButtonType == 1)
        {
            /*
            List<int> EmptyInventoryIndex = new List<int>();
            int InventoryAmount = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");//이게 사용가능한 인벤토리 갯수
            for (int i = 0; i < InventoryAmount; i++)//사용가능 인벤토리 갯수 만큼 반복//0레벨이면 0~3까지 반복함 5레벨 이상이면 0~11까지
            {
                if (P_Info.GetPlayerStateInfo().EquipmentInventory[i] == 0)//비어있다면
                {
                    EmptyInventoryIndex.Add(i);//접근 가능 인벤토리 list에 저장
                }
            }
            if(EmptyInventoryIndex.Count <= 0)//비어있는 inventory Slot이 없다면
            {
                //비어있는 inventory가 없다는 표시를 해줘야함
                //UI에 인벤토리도 넣어야 할듯?
                UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                return;   
            }
            //비어있지 않다면 EmptyInventoryIndex의 가장 앞에 있는 index로 랜덤한 장비의 코드를 넣음
            //이러면 장비 뽑기 레벨에 맞는 장비 레벨중 하나가 리턴이 됨.
            int RandomEquipment = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(P_Info.GetPlayerStateInfo().EquipmentGamblingLevel);
            P_Info.GetPlayerStateInfo().EquipmentInventory[EmptyInventoryIndex[0]] = RandomEquipment;
            UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);
            */
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
                /*
                List<int> EmptyInventoryIndex = new List<int>();
                int InventoryAmount = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");//이게 사용가능한 인벤토리 갯수
                for (int i = 0; i < InventoryAmount; i++)//사용가능 인벤토리 갯수 만큼 반복//0레벨이면 0~3까지 반복함 5레벨 이상이면 0~11까지
                {
                    if (P_Info.GetPlayerStateInfo().EquipmentInventory[i] == 0)//비어있다면
                    {
                        EmptyInventoryIndex.Add(i);//접근 가능 인벤토리 list에 저장
                    }
                }

                if (EmptyInventoryIndex.Count <= 0)//비어있는 inventory Slot이 없다면
                {
                    //비어있는 inventory가 없다는 표시를 해줘야함
                    //UI에 인벤토리도 넣어야 할듯?
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                //비어있지 않다면 EmptyInventoryIndex의 가장 앞에 있는 index로 랜덤한 장비의 코드를 넣음
                //이러면 장비 뽑기 레벨에 맞는 장비 레벨중 하나가 리턴이 됨.
                int RandomEquipment = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(P_Info.GetPlayerStateInfo().EquipmentGamblingLevel);
                P_Info.GetPlayerStateInfo().EquipmentInventory[EmptyInventoryIndex[0]] = RandomEquipment;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);
                */
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
                /*
                List<int> EmptyInventoryIndex = new List<int>();
                int InventoryAmount = (int)JsonReadWriteManager.Instance.GetEarlyState("EQUIP");//이게 사용가능한 인벤토리 갯수
                for (int i = 0; i < InventoryAmount; i++)//사용가능 인벤토리 갯수 만큼 반복//0레벨이면 0~3까지 반복함 5레벨 이상이면 0~11까지
                {
                    if (P_Info.GetPlayerStateInfo().EquipmentInventory[i] == 0)//비어있다면
                    {
                        EmptyInventoryIndex.Add(i);//접근 가능 인벤토리 list에 저장
                    }
                }
                if (EmptyInventoryIndex.Count <= 0)//비어있는 inventory Slot이 없다면
                {
                    //비어있는 inventory가 없다는 표시를 해줘야함
                    //UI에 인벤토리도 넣어야 할듯?
                    UIMgr.G_UI.ActiveGuideMessageUI((int)EGuideMessage.NotEnoughInventoryMessage);
                    return;
                }
                //비어있지 않다면 EmptyInventoryIndex의 가장 앞에 있는 index로 랜덤한 장비의 코드를 넣음
                //이러면 장비 뽑기 레벨에 맞는 장비 레벨중 하나가 리턴이 됨.
                int RandomEquipment = EquipmentInfoManager.Instance.GetGamblingEquipmentCode(P_Info.GetPlayerStateInfo().EquipmentGamblingLevel);
                P_Info.GetPlayerStateInfo().EquipmentInventory[EmptyInventoryIndex[0]] = RandomEquipment;
                UIMgr.GI_UI.ActiveGettingUI(RandomEquipment);
                */
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
}
