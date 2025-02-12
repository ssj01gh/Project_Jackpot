using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
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
                    SetCurrentSpawnPatternReward();
                    PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
                    return;//함수 종료
                }
            }
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
        SetCurrentSpawnPatternReward();
        PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
    }

    protected void SetCurrentSpawnPatternReward()
    {
        CurrentSpawnPatternReward = CurrentSpawnPattern.RewardEXPPoint;
        float RandVariation = Random.Range(-(int)CurrentSpawnPattern.VariationEXPPoint, (int)CurrentSpawnPattern.VariationEXPPoint + 1);
        CurrentSpawnPatternReward += RandVariation;
    }

    public void SpawnCurrentSpawnPatternMonster()
    {
        for(int i = 0; i < ActiveMonsters.Count; i++)
        {
            ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
        }
        ActiveMonsters.Clear();
        for (int i = 0; i < CurrentSpawnPattern.SpawnMonsterName.Length; i++)
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

    //ActiveMonster중 CurrentHp가 0이하가 된놈은 없애기
    public void CheckActiveMonstersRSurvive()
    {
        for(int i = ActiveMonsters.Count - 1; i >= 0; i-- )
        {
            if (ActiveMonsters[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentHP <= 0)
            {
                ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
                ActiveMonsters[i].SetActive(false);
                ActiveMonsters.RemoveAt(i);
            }
        }
    }

    public void InActiveAllActiveMonster()
    {
        for (int i = ActiveMonsters.Count - 1; i >= 0; i--)
        {
            ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
            ActiveMonsters[i].SetActive(false);
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
