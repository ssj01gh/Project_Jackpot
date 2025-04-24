using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterManager : MonoBehaviour
{
    public GameObject[] MonsterSpawnPoint;
    [Header("MonsterInfo")]
    public GameObject[] MonsterPrefabs;
    public MonSpawnPattern[] SpawnPatternSO;

    protected Dictionary<string, List<GameObject>> MonsterStorage = new Dictionary<string, List<GameObject>>();
    protected Dictionary<int, List<MonSpawnPattern>> PatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected List<GameObject> ActiveMonsters = new List<GameObject>();

    protected MonSpawnPattern CurrentSpawnPattern;
    public float CurrentSpawnPatternReward { protected set; get; }
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
        foreach(MonSpawnPattern MSPattern in SpawnPatternSO)
        {
            int PatternThemeNum = MSPattern.SpawnPatternID / 100;
            //저장된 테마가 없을경우
            if(!PatternStorage.ContainsKey(PatternThemeNum))
            {
                List<MonSpawnPattern> MonSpawnList = new List<MonSpawnPattern>();
                MonSpawnList.Add(MSPattern);
                PatternStorage.Add(PatternThemeNum, MonSpawnList);
            }
            else//저장된 테마가 있을경우
            {
                PatternStorage[PatternThemeNum].Add(MSPattern);
            }
        }
    }

    public List<GameObject> GetActiveMonsters()
    {
        return ActiveMonsters;
    }

    public void SetBossSpawn(PlayerManager PMgr)
    {//1스테이지 라면 1101 ~ // 2스테이지 라면 1201~
        int ThemeNum = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor + 10;
        int RandBossPatternID = Random.Range(0, PatternStorage[ThemeNum].Count);

        CurrentSpawnPattern = PatternStorage[ThemeNum][RandBossPatternID];
        SetCurrentSpawnPatternReward(PMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack);
        PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
        JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());
    }
    public void SetSpawnPattern(PlayerManager PMgr)
    {
        int ThemeNum = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor;
        int DetailOfEvents = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails;
        //DetailOfEvents는 101 ~ 로 됨 1테마라면 101~ 2테마 라면 201~식이다
        if(DetailOfEvents != 0)
        {
            int ThemeOfEvent = DetailOfEvents / 100;
            //PatternStorage[DetailOfEvents / 100];이게 이제 저장된 적들의 스폰 패턴중 하나// 여기서 찾아야함
            for (int i = 0; i < PatternStorage[ThemeOfEvent].Count; i++)
            {
                if (PatternStorage[ThemeOfEvent][i].SpawnPatternID == DetailOfEvents)//겹치는게 있다면
                {
                    CurrentSpawnPattern = PatternStorage[ThemeOfEvent][i];
                    SetCurrentSpawnPatternReward(PMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack);
                    PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
                    JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());
                    return;//함수 종료
                }
            }
            //만약 겹치는게 없다면
            int RandomPattern = Random.Range(0, PatternStorage[ThemeOfEvent].Count);
            CurrentSpawnPattern = PatternStorage[ThemeOfEvent][RandomPattern];
            SetCurrentSpawnPatternReward(PMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack);
            PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
            JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());
        }

        //모든 테마를 다 포함한 랜덤값
        if (ThemeNum == -1)
        {
            List<MonSpawnPattern> MSPatterns = new List<MonSpawnPattern>();
            foreach (int Key in PatternStorage.Keys)
            {
                foreach (MonSpawnPattern MSPattern in PatternStorage[Key])
                {
                    MSPatterns.Add(MSPattern);
                }
            }
            int Rand = Random.Range(0, MSPatterns.Count);
            CurrentSpawnPattern = MSPatterns[Rand];
        }
        else//-1이 아닐때 들어온 테마 값에 맞게 결정
        {
            int Rand = Random.Range(0, PatternStorage[ThemeNum].Count);
            CurrentSpawnPattern = PatternStorage[ThemeNum][Rand];
        }
        SetCurrentSpawnPatternReward(PMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack);
        PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
        JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    protected void SetCurrentSpawnPatternReward(int IsSuddenAttack)
    {
        if(IsSuddenAttack == -1)//습격이 아닐때는
        {
            CurrentSpawnPatternReward = CurrentSpawnPattern.RewardEXPPoint;
            float RandVariation = Random.Range(-(int)CurrentSpawnPattern.VariationEXPPoint, (int)CurrentSpawnPattern.VariationEXPPoint + 1);
            CurrentSpawnPatternReward += RandVariation;
        }
        else//-1이 아니라면 습격인거임
        {
            CurrentSpawnPatternReward = 0;
        }
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

    public void SpawnMonsterBySummonMonster(List<string> MonsterID)
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
                        MonsterStorage[MonsterID[i]][j].GetComponent<Monster>().SpawnMonster(CanSpawnPoint[i]);
                        MonsterStorage[MonsterID[i]][j].GetComponent<Monster>().MonsterClicked += SetCurrentTargetMonster;

                        ActiveMonsters.Add(MonsterStorage[MonsterID[i]][j]);
                        break;
                    }
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
    public void CheckActiveMonstersRSurvive()
    {
        for(int i = ActiveMonsters.Count - 1; i >= 0; i-- )
        {
            if (ActiveMonsters[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentHP <= 0)
            {
                ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
                ActiveMonsters[i].GetComponent<Monster>().DeSpawnFadeOut();
                ActiveMonsters.RemoveAt(i);
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

        Debug.Log("Monster!!!!!");
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
