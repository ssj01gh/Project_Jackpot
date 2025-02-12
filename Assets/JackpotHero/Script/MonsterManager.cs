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
    public event System.Action<Monster> CurrentTargetChange;//��Ʋ UI���� ���� ���� ǥ���ҷ��� ���� event
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
            //�ش� �̸��� ���� ���Ͱ� �������� �ʴ´ٸ�
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
            //����� �׸��� �������
            if(!PatternStorage.ContainsKey(PatternThemeNum))
            {
                List<MonSpawnPattern> MonSpawnList = new List<MonSpawnPattern>();
                MonSpawnList.Add(MSPattern);
                PatternStorage.Add(PatternThemeNum, MonSpawnList);
            }
            else//����� �׸��� �������
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
        //DetailOfEvents�� 101 ~ �� �� 1�׸���� 101~ 2�׸� ��� 201~���̴�
        if(DetailOfEvents != 0)
        {
            int ThemeOfEvent = DetailOfEvents / 100;
            //PatternStorage[DetailOfEvents / 100];�̰� ���� ����� ������ ���� ������ �ϳ�// ���⼭ ã�ƾ���
            for (int i = 0; i < PatternStorage[ThemeOfEvent].Count; i++)
            {
                if (PatternStorage[ThemeOfEvent][i].SpawnPatternID == DetailOfEvents)//��ġ�°� �ִٸ�
                {
                    CurrentSpawnPattern = PatternStorage[ThemeOfEvent][i];
                    SetCurrentSpawnPatternReward();
                    PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
                    return;//�Լ� ����
                }
            }
        }

        //��� �׸��� �� ������ ������
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
        else//-1�� �ƴҶ� ���� �׸� ���� �°� ����
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
            if (CurrentSpawnPattern.SpawnMonsterName[i] == "")//����ִٸ�
                continue;//�ǳ� �ٱ�
            else
            {
                if (!MonsterStorage.ContainsKey(CurrentSpawnPattern.SpawnMonsterName[i]))//Dictionary�� ���ٸ�
                {
                    continue;//�ǳʶٱ�
                }
                else//�ִٸ�
                {
                    for (int j = 0; j < MonsterStorage[CurrentSpawnPattern.SpawnMonsterName[i]].Count; j++)
                    {
                        //��Ȱ��ȭ �Ǿ��ִ� ���Ͷ�� Ȱ��ȭ, ActiveMonsters�� ���
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

    //ActiveMonster�� CurrentHp�� 0���ϰ� �ȳ��� ���ֱ�
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
    public void SetCurrentTargetMonster(Monster ClickedMonster)//���Ͱ� Ŭ���� �Ǹ� �� �Լ��� �����
    {
        //MainBattleUI�� ���� �������� Ŭ���� �ȵǾߵ�
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
