using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class MonsterManager : MonoBehaviour
{
    public GameObject[] MonsterSpawnPoint;
    [Header("MonsterInfo")]
    public GameObject[] MonsterPrefabs;
    public MonSpawnPattern[] Tier01SpawnPatternSO;
    public MonSpawnPattern[] Tier02SpawnPatternSO;
    public MonSpawnPattern[] Tier03SpawnPatternSO;
    public MonSpawnPattern[] BossSpawnPatternSO;
    public MonSpawnPattern[] EventSpawnPatternSO;

    protected Dictionary<string, List<GameObject>> MonsterStorage = new Dictionary<string, List<GameObject>>();
    protected Dictionary<int, List<MonSpawnPattern>> Tier01PatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected Dictionary<int, List<MonSpawnPattern>> Tier02PatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected Dictionary<int, List<MonSpawnPattern>> Tier03PatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected Dictionary<int, List<MonSpawnPattern>> BossPatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected Dictionary<int, List<MonSpawnPattern>> EventPatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected List<GameObject> ActiveMonsters = new List<GameObject>();

    protected MonSpawnPattern CurrentSpawnPattern;
    //public float CurrentSpawnPatternReward { protected set; get; }
    protected const int InAdvanceAmount = 3;
    public Monster CurrentTarget { protected set; get; }
    public event System.Action<Monster> CurrentTargetChange;//배틀 UI에서 현재 몬스터 표시할려고 만든 event

    protected void Awake()
    {
        InitMonster();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitMonster()
    {
        for(int i = 0; i < MonsterPrefabs.Length; i++)
        {
            //해당 이름을 가진 몬스터가 존재하지 않는다면
            if(!MonsterStorage.ContainsKey(MonsterPrefabs[i].GetComponent<Monster>().MonsterName))
            {
                List<GameObject> MonsterList = new List<GameObject>();
                for(int j = 0; j < InAdvanceAmount; j++)
                {
                    GameObject obj = GameObject.Instantiate(MonsterPrefabs[i]);
                    obj.SetActive(false);
                    obj.transform.SetParent(gameObject.transform);
                    MonsterList.Add(obj);
                }
                MonsterStorage.Add(MonsterPrefabs[i].GetComponent<Monster>().MonsterName, MonsterList);
            }
        }
        //SaveTier01SpawnPattern
        foreach(MonSpawnPattern MSPattern in Tier01SpawnPatternSO)
        {
            int PatternThemeNum = MSPattern.SpawnPatternID / 1000;
            //저장된 테마가 없을경우
            if(!Tier01PatternStorage.ContainsKey(PatternThemeNum))
            {
                List<MonSpawnPattern> MonTier01SpawnList = new List<MonSpawnPattern>();
                MonTier01SpawnList.Add(MSPattern);
                Tier01PatternStorage.Add(PatternThemeNum, MonTier01SpawnList);
            }
            else//저장된 테마가 있을경우
            {
                Tier01PatternStorage[PatternThemeNum].Add(MSPattern);
            }
        }
        //SaveTier02SpawnPattern
        foreach (MonSpawnPattern MSPattern in Tier02SpawnPatternSO)
        {
            int PatternThemeNum = MSPattern.SpawnPatternID / 1000;
            //저장된 테마가 없을경우
            if(!Tier02PatternStorage.ContainsKey(PatternThemeNum))
            {
                List<MonSpawnPattern> MonTier02SpawnList = new List<MonSpawnPattern>();
                MonTier02SpawnList.Add(MSPattern);
                Tier02PatternStorage.Add(PatternThemeNum, MonTier02SpawnList);
            }
            else
            {
                Tier02PatternStorage[PatternThemeNum].Add(MSPattern);
            }
        }
        foreach(MonSpawnPattern MSPattern in Tier03SpawnPatternSO)
        {
            int PatternThemeNum = MSPattern.SpawnPatternID / 1000;
            //저장된 테마가 없을경우
            if (!Tier03PatternStorage.ContainsKey(PatternThemeNum))
            {
                List<MonSpawnPattern> MonTier03SpawnList = new List<MonSpawnPattern>();
                MonTier03SpawnList.Add(MSPattern);
                Tier03PatternStorage.Add(PatternThemeNum, MonTier03SpawnList);
            }
            else
            {
                Tier03PatternStorage[PatternThemeNum].Add(MSPattern);
            }
        }
        //SaveBossSpawnPattern
        foreach(MonSpawnPattern MSPattern in BossSpawnPatternSO)
        {
            int PatternThemeNum = MSPattern.SpawnPatternID / 1000;
            if(!BossPatternStorage.ContainsKey(PatternThemeNum))
            {
                List<MonSpawnPattern> MonBossSpawnList = new List<MonSpawnPattern>();
                MonBossSpawnList.Add(MSPattern);
                BossPatternStorage.Add(PatternThemeNum, MonBossSpawnList);
            }
            else
            {
                BossPatternStorage[PatternThemeNum].Add(MSPattern);
            }
        }
        foreach(MonSpawnPattern MSPattern in EventSpawnPatternSO)
        {
            int PatternThemeNum = MSPattern.SpawnPatternID / 1000;
            if (!EventPatternStorage.ContainsKey(PatternThemeNum))
            {
                List<MonSpawnPattern> MonBossSpawnList = new List<MonSpawnPattern>();
                MonBossSpawnList.Add(MSPattern);
                EventPatternStorage.Add(PatternThemeNum, MonBossSpawnList);
            }
            else
            {
                EventPatternStorage[PatternThemeNum].Add(MSPattern);
            }
        }
    }

    public List<GameObject> GetActiveMonsters()
    {
        return ActiveMonsters;
    }

    public void SetBossSpawn(PlayerManager PMgr)//여기는 보스를 최초로 정할때만 들어옴
    {//1스테이지 라면 1200 ~ // 2스테이지 라면 2200~
        int ThemeNum = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor;
        int DetailOfEvents = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails;
        int RandBossPatternID = Random.Range(0, BossPatternStorage[ThemeNum].Count);

        CurrentSpawnPattern = BossPatternStorage[ThemeNum][RandBossPatternID];
        PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
        JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());
    }
    public void SetSpawnPattern(PlayerManager PMgr)
    {
        //DetailOfEvents는 1000 ~ 로 됨 1테마라면 1000~ 2테마 라면 2000~식이다
        int ThemeNum = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor;
        int DetailOfEvents = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails;
        int CurrentSearchPoint = PMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint;//0 -> 1티어 100% 40 -> 2티어 100%
        //패턴을 결정하기 전에 이미 정해진 패턴이 있다면 리턴 -> 이미 스폰 패턴이 결정된 상태(껏다 킨거임)
        //탐색도 ~40까지는 백퍼 1티어 ~80까지는 1티어 2티어 혼용 80~ 3티어까지 혼용
        if(DetailOfEvents % 1000 < 100 && DetailOfEvents % 1000 >= 0)
        {//0~99이라면 1티어 // 0 ~ 99임 0이면 패턴을 결정해야 되는거임
            for (int i = 0; i < Tier01PatternStorage[ThemeNum].Count; i++)
            {
                if (Tier01PatternStorage[ThemeNum][i].SpawnPatternID == DetailOfEvents)
                {
                    CurrentSpawnPattern = Tier01PatternStorage[ThemeNum][i];
                    return;
                }
            }
        }
        else if(DetailOfEvents % 1000 >= 100 && DetailOfEvents % 1000 < 149 )
        {//100 ~ 149까지 2티어
            for (int i = 0; i < Tier02PatternStorage[ThemeNum].Count; i++)
            {
                if (Tier02PatternStorage[ThemeNum][i].SpawnPatternID == DetailOfEvents)
                {
                    CurrentSpawnPattern = Tier02PatternStorage[ThemeNum][i];
                    return;
                }
            }
        }
        else if(DetailOfEvents % 1000 >= 150 && DetailOfEvents % 1000 < 200)
        {//150 ~ 199까지 3티어
            for (int i = 0; i < Tier03PatternStorage[ThemeNum].Count; i++)
            {
                if (Tier03PatternStorage[ThemeNum][i].SpawnPatternID == DetailOfEvents)
                {
                    CurrentSpawnPattern = Tier03PatternStorage[ThemeNum][i];
                    return;
                }
            }
        }
        else if (DetailOfEvents % 1000 >= 200 && DetailOfEvents % 1000 < 300)
        {//보스전일때 껏다 키면 여기로 들어옴//200 ~ 299라면 보스
            for (int i = 0; i < BossPatternStorage[ThemeNum].Count; i++)
            {
                if (BossPatternStorage[ThemeNum][i].SpawnPatternID == DetailOfEvents)
                {
                    CurrentSpawnPattern = BossPatternStorage[ThemeNum][i];
                    return;
                }
            }
        }
        else if (DetailOfEvents % 1000 >= 300 && DetailOfEvents % 1000 < 400)
        {//300 ~ 399라면 이벤트 몬스터 스폰
            for (int i = 0; i < EventPatternStorage[ThemeNum].Count; i++)
            {
                if (EventPatternStorage[ThemeNum][i].SpawnPatternID == DetailOfEvents)
                {
                    CurrentSpawnPattern = EventPatternStorage[ThemeNum][i];
                    return;
                }
            }
        }


        //위에서 걸러지지 않았다면 새로 결정
        int RandNum;
        if (ThemeNum == 4)
        {
            RandNum = Random.Range(0, 3);
            switch(RandNum)
            {
                case 0:
                    CurrentSpawnPattern = Tier01PatternStorage[ThemeNum][0];
                    break;
                case 1:
                    CurrentSpawnPattern = Tier02PatternStorage[ThemeNum][0];
                    break;
                case 2:
                    CurrentSpawnPattern = Tier03PatternStorage[ThemeNum][0];
                    break;
            }
            PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
            JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());//결정 된거 저장
        }
        else
        {
            if (CurrentSearchPoint < 40)//0~39 //1티어 패턴만
                RandNum = 1;
            else if (CurrentSearchPoint >= 40 && CurrentSearchPoint <= 79)//40~79 //1~2티어 패턴 중 하나
                RandNum = Random.Range(1, 3);//1~2
            else//80~ // 1~3티어 패턴중 하나
                RandNum = Random.Range(1, 4);//1~3

            switch (RandNum)
            {
                case 1:
                    if (Tier01PatternStorage.ContainsKey(ThemeNum))
                    {
                        RandNum = Random.Range(0, Tier01PatternStorage[ThemeNum].Count);
                        CurrentSpawnPattern = Tier01PatternStorage[ThemeNum][RandNum];
                        PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
                        JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());//결정 된거 저장
                        return;
                    }
                    break;
                case 2:
                    if (Tier02PatternStorage.ContainsKey(ThemeNum))
                    {
                        RandNum = Random.Range(0, Tier02PatternStorage[ThemeNum].Count);
                        CurrentSpawnPattern = Tier02PatternStorage[ThemeNum][RandNum];
                        PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
                        JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());//결정 된거 저장
                        return;
                    }
                    break;
                case 3:
                    if (Tier03PatternStorage.ContainsKey(ThemeNum))
                    {
                        RandNum = Random.Range(0, Tier03PatternStorage[ThemeNum].Count);
                        CurrentSpawnPattern = Tier03PatternStorage[ThemeNum][RandNum];
                        PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
                        JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());//결정 된거 저장
                        return;
                    }
                    break;
            }
        }

        //여기 까지 온거면 ThemeNum가 없는거임
        Debug.Log("NoMonsterPattern");
    }

    public void SpawnCurrentSpawnPatternMonster()
    {
        for(int i = 0; i < ActiveMonsters.Count; i++)
        {
            ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
        }
        ActiveMonsters.Clear();
        for (int i = 0; i < CurrentSpawnPattern.SpawnMonsterName.Length; i++)//0~2까지 밖에 안나옴
        {
            if (CurrentSpawnPattern.SpawnMonsterName[i] == "")//비어있다면
                continue;//건너 뛰기
            else
            {
                if (!MonsterStorage.ContainsKey(CurrentSpawnPattern.SpawnMonsterName[i]))//Dictionary에 없다면
                {
                    continue;//건너뛰기
                }
                else//있다면
                {
                    for (int j = 0; j < MonsterStorage[CurrentSpawnPattern.SpawnMonsterName[i]].Count; j++)
                    {
                        //비활성화 되어있는 몬스터라면 활성화, ActiveMonsters에 등록
                        if (MonsterStorage[CurrentSpawnPattern.SpawnMonsterName[i]][j].activeSelf == false)
                        {
                            MonsterStorage[CurrentSpawnPattern.SpawnMonsterName[i]][j].GetComponent<Monster>().SpawnMonster(MonsterSpawnPoint[i].transform.position);
                            MonsterStorage[CurrentSpawnPattern.SpawnMonsterName[i]][j].GetComponent<Monster>().MonsterClicked += SetCurrentTargetMonster;

                            ActiveMonsters.Add(MonsterStorage[CurrentSpawnPattern.SpawnMonsterName[i]][j]);
                            break;
                        }
                    }

                }
            }
        }
    }
    public void GiveBuffActiveMonsterByPlayer(PlayerInfo _PlayerInfo)
    {
        if (JsonReadWriteManager.Instance.E_Info.EarlySpeedLevel >= 7)
        {
            foreach(GameObject Mon in ActiveMonsters)
            {
                Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Defenseless] += 99;
            }
        }
        int RESEquip = 12;

        int BootsTier = (_PlayerInfo.EquipShoesCode / 1000) % 10;
        int IsEventBoots = _PlayerInfo.EquipShoesCode / 10000;
        int BootsStateType = (_PlayerInfo.EquipShoesCode / 100) % 10;
        int BootsType = (IsEventBoots * 10) + BootsStateType;

        int AccTier = (_PlayerInfo.EquipAccessoriesCode / 1000) % 10;
        int IsEventAcc = _PlayerInfo.EquipAccessoriesCode / 10000;
        int AccStateType = (_PlayerInfo.EquipAccessoriesCode / 100) % 10;
        int AccType = (IsEventBoots * 10) + BootsStateType;

        if(BootsType == RESEquip)
        {
            foreach (GameObject Mon in ActiveMonsters)
            {
                Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Slow] += (BootsTier * 2);
            }
        }
        if(AccType == RESEquip)
        {
            foreach (GameObject Mon in ActiveMonsters)
            {
                Mon.GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.CurseOfDeath] += (15 - (AccTier * 2));
            }
        }

    }

    public void SpawnMonsterBySummonMonster(List<string> MonsterID, GameObject SummonerMonster)
    {
        //살아 있는 몬스터와 몬스터 스폰가능 위치의 좌표를 비교해서 스폰가능한 지역을 찾음
        //살아있는 몬스터가 3마리 이상이면 그냥 return -> 어짜피 비어 있는곳이 없음
        //만약에 스폰 가능한 위치가 없다면 그대로 return(스폰하지 않음)
        //스폰 가능한 위치가 있다면 앞에서 부터 스폰하고 스폰 가능한 위치가 없어지면 스폰하지 않음
        //MonsterID.Count와 CanSpwanPont.Count중 큰것을 기준으로 몬스터를 스폰함
        if (MonsterID.Count >= InAdvanceAmount)
            return;

        List<Vector2> CanSpawnPoint = new List<Vector2>();
        for(int i = 0; i < MonsterSpawnPoint.Length; i++)
        {
            bool IsEmptySpawnPoint = true;
            Debug.Log(ActiveMonsters.Count);
            for (int j = 0; j < ActiveMonsters.Count; j++)
            {
                //ActiveMonster의 모두가 MonsterSpawnPoint[i]와 같은 위치가 아니여야지만 비어있는 것임
                if (Vector2.Distance(MonsterSpawnPoint[i].transform.position, ActiveMonsters[j].transform.position) <= 0.1f)
                {
                    IsEmptySpawnPoint = false;
                    break;
                    //여기 들어왔다는 것 = 비어있지 않은 것임
                }
            }
            if(IsEmptySpawnPoint == true)
            {
                //이게 true이다 : 이 MonsterSpawnPoint[i]의 좌표는 비어있는 것임
                CanSpawnPoint.Add(MonsterSpawnPoint[i].transform.position);
            }
        }

        if (CanSpawnPoint.Count <= 0)//비어있는 장소가 없다면
            return;

        int SpawnCount = 0;
        if (CanSpawnPoint.Count >= MonsterID.Count)//작은 쪽을 기준으로
            SpawnCount = MonsterID.Count;
        else
            SpawnCount = CanSpawnPoint.Count;

        for(int i = 0; i < SpawnCount; i ++)
        {
            if (MonsterStorage.ContainsKey(MonsterID[i]))//해당 몬스터가 있다면
            {
                for(int j = 0; j < MonsterStorage[MonsterID[i]].Count; j++)
                {
                    if (MonsterStorage[MonsterID[i]][j].activeSelf == false)//꺼져있을때만 소환
                    {
                        MonsterStorage[MonsterID[i]][j].GetComponent<Monster>().MasterMonster = SummonerMonster;
                        MonsterStorage[MonsterID[i]][j].GetComponent<Monster>().SpawnMonster(CanSpawnPoint[i]);
                        MonsterStorage[MonsterID[i]][j].GetComponent<Monster>().MonsterClicked += SetCurrentTargetMonster;

                        ActiveMonsters.Add(MonsterStorage[MonsterID[i]][j]);
                        break;
                    }
                }
            }
        }
    }

    public void GiveBuffToActiveServent(int i_BuffType, int i_BuffCount, GameObject g_MasterMosnter)
    {
        foreach(GameObject ActiveMonster in ActiveMonsters)
        {
            Monster Mon = ActiveMonster.GetComponent<Monster>();
            if (Mon.MonsterBuff.BuffList[(int)EBuffType.Servant] >= 1)
            {
                if(Mon.MasterMonster != null && Mon.MasterMonster == g_MasterMosnter)
                {
                    Mon.MonsterGetBuff(i_BuffType, i_BuffCount);
                }
            }
        }
    }

    public void SetActiveMonstersStatus()
    {
        foreach(GameObject Mon in ActiveMonsters)
        {
            Mon.GetComponent<Monster>().SetMonsterStatus();
        }
    }

    //ActiveMonster중 CurrentHp가 0이하가 된놈은 없애기
    public List<int> CheckActiveMonstersRSurvive(PlayerManager PlayerMgr)
    {
        List<int> DeadMonsterReward = new List<int>();
        List<GameObject> DeadMonsters = new List<GameObject>();
        for(int i = ActiveMonsters.Count - 1; i >= 0; i-- )
        {
            if (ActiveMonsters[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentHP < 1)
            {
                if (ActiveMonsters[i].GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.UnDead] >= 1)
                {
                    ActiveMonsters[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentHP = 1;
                    ActiveMonsters[i].GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.UnDead] = 0;
                    EffectManager.Instance.ActiveEffect("BattleEffect_Buff_UnDead", ActiveMonsters[i].gameObject.transform.position);
                    continue;
                }

                if (ActiveMonsters[i].GetComponent<Monster>().IsSummonTier == true)
                {
                    //여기에 들어오면 아무것도 안중가하게
                }
                else
                {
                    if (ActiveMonsters[i].GetComponent<Monster>().IsTierOne == true)//1티어
                    {
                        PlayerMgr.GetPlayerInfo().RecordKillCount(1);
                    }
                    else
                    {
                        PlayerMgr.GetPlayerInfo().RecordKillCount(0, 1);
                    }
                }

                ActiveMonsters[i].GetComponent<Monster>().InitAllBuff();//모든 버프들 없애기
                ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
                ActiveMonsters[i].GetComponent<Monster>().MasterMonster = null;
                ActiveMonsters[i].GetComponent<Monster>().DeSpawnFadeOut();
                int RewardEXP = (int)ActiveMonsters[i].GetComponent<Monster>().MonsterBaseEXP;
                int VarianceEXP = Random.Range(-(int)ActiveMonsters[i].GetComponent<Monster>().EXPVarianceAmount, (int)ActiveMonsters[i].GetComponent<Monster>().EXPVarianceAmount);
                int AdditionalEXP = (int)ActiveMonsters[i].GetComponent<Monster>().AdditionalEXP;
                DeadMonsterReward.Add(RewardEXP + VarianceEXP + AdditionalEXP);

                DeadMonsters.Add(ActiveMonsters[i]);
                ActiveMonsters.RemoveAt(i);
            }
        }
        //위쪽에서 죽은 애들을 다 거름//죽은 애들중에서 주인이 되는 몬스터가 있다면 survant도 죽임
        for (int i = ActiveMonsters.Count - 1; i >= 0; i--)
        {
            if (ActiveMonsters[i].GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Servant] >= 1)
            {//졸개가 있다면 죽은 애들중에 비교함
                for(int j = 0; j < DeadMonsters.Count; j++)
                {
                    if (ActiveMonsters[i].GetComponent<Monster>().MasterMonster == DeadMonsters[j])
                    {//죽은 애들중에 마스터가 있다면
                        ActiveMonsters[i].GetComponent<Monster>().InitAllBuff();//모든 버프들 없애기
                        ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
                        ActiveMonsters[i].GetComponent<Monster>().MasterMonster = null;
                        ActiveMonsters[i].GetComponent<Monster>().DeSpawnFadeOut();
                        ActiveMonsters.RemoveAt(i);

                        break;
                    }
                }
            }
        }
        //만약 DeadCount가 1이상인데, 살아있는 몬스터중에 호문클루스가 있다면 그놈에게 흡수 부여
        if(DeadMonsters.Count >= 1)
        {
            for(int i = 0; i < ActiveMonsters.Count; i++)
            {
                
                //지금은 임시로 짧다리새
                if (ActiveMonsters[i].GetComponent<Monster>().MonsterName == "Homunculus")
                {
                    //DeadMonsters.Count만큼 반복
                    for(int j = 0; j < DeadMonsters.Count; j++)
                    {
                        ActiveMonsters[i].GetComponent<Monster>().MonsterGetBuff((int)EBuffType.Consume);
                    }
                    
                }
                
            }
        }


        return DeadMonsterReward;
    }

    public Monster CheckActiveMonsterHaveProvocation()
    {
        for(int i = 0; i < ActiveMonsters.Count; i++)
        {
            if (ActiveMonsters[i].GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.Provocation] >= 1)//도발이 있다면
            {
                CurrentTarget = ActiveMonsters[i].GetComponent<Monster>();
                break;
            }
        }
        return CurrentTarget;
    }

    public void SetActiveMonsterChainAttack(bool IsMonsterTurn, bool IsAttack , Monster CurrentTurnMon = null)
    {
        //몬스터의 연속 타격이 초기화 되는 경우 -> 플레이어의 턴일때 혹은 자신이 공격 이외의 행동을 할때
        for(int i = 0; i < ActiveMonsters.Count; i++)
        {
            if(IsMonsterTurn == true && IsAttack == true)
            {//몬스터의 턴이고, 공격이며, 현재턴의 몬스터가 특정 몬스터일때
                if(CurrentTurnMon == ActiveMonsters[i].GetComponent<Monster>())
                {
                    switch(ActiveMonsters[i].GetComponent<Monster>().MonsterName)//임시로 짧다리새
                    {
                        /*
                        case "ShortLegBird":
                            ActiveMonsters[i].GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ChainAttack] += 1;
                            break;
                        */
                    }
                }
            }
            else if(IsMonsterTurn == true && IsAttack == false)
            {
                if(CurrentTurnMon == ActiveMonsters[i].GetComponent<Monster>())
                {
                    ActiveMonsters[i].GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ChainAttack] = 0;
                }
            }
            else//몬스터의 턴이 아닐때
            {
                ActiveMonsters[i].GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.ChainAttack] = 0;
            }
        }
    }

    public void SetAcitveMonsterMountainLord()
    {
        for (int i = 0; i < ActiveMonsters.Count; i++)
        {
            if (ActiveMonsters[i].GetComponent<Monster>().MonsterName == "MountainLord")
            {
                ActiveMonsters[i].GetComponent<Monster>().MonsterBuff.BuffList[(int)EBuffType.MountainLord] += 1;
            }
        }
    }

    public void InActiveAllActiveMonster()
    {
        for (int i = ActiveMonsters.Count - 1; i >= 0; i--)
        {
            ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
            ActiveMonsters[i].GetComponent<Monster>().DeSpawnFadeOut();
            ActiveMonsters.RemoveAt(i);
        }
    }
    public void SetCurrentTargetMonster(Monster ClickedMonster)//몬스터가 클릭이 되면 이 함수가 실행됨
    {
        //MainBattleUI가 켜저 있을때는 클릭이 안되야됨
        if (ClickedMonster == null)
        {
            CurrentTarget = null;
            return;
        }

        //Debug.Log("Monster!!!!!");
        CurrentTarget = ClickedMonster;
        CurrentTargetChange.Invoke(CurrentTarget.GetComponent<Monster>());
    }

    private void OnApplicationQuit()
    {
        for(int i = 0; i < ActiveMonsters.Count; i++)
        {
            ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
        }
    }
}
