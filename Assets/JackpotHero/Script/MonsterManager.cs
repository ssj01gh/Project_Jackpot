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
    public MonSpawnPattern[] Tier01SpawnPatternSO;
    public MonSpawnPattern[] Tier02SpawnPatternSO;
    public MonSpawnPattern[] BossSpawnPatternSO;
    public MonSpawnPattern[] EventSpawnPatternSO;

    protected Dictionary<string, List<GameObject>> MonsterStorage = new Dictionary<string, List<GameObject>>();
    protected Dictionary<int, List<MonSpawnPattern>> Tier01PatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected Dictionary<int, List<MonSpawnPattern>> Tier02PatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected Dictionary<int, List<MonSpawnPattern>> BossPatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected Dictionary<int, List<MonSpawnPattern>> EventPatternStorage = new Dictionary<int, List<MonSpawnPattern>>();
    protected List<GameObject> ActiveMonsters = new List<GameObject>();

    protected MonSpawnPattern CurrentSpawnPattern;
    //public float CurrentSpawnPatternReward { protected set; get; }
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
        //SaveTier01SpawnPattern
        foreach(MonSpawnPattern MSPattern in Tier01SpawnPatternSO)
        {
            int PatternThemeNum = MSPattern.SpawnPatternID / 1000;
            //����� �׸��� �������
            if(!Tier01PatternStorage.ContainsKey(PatternThemeNum))
            {
                List<MonSpawnPattern> MonTier01SpawnList = new List<MonSpawnPattern>();
                MonTier01SpawnList.Add(MSPattern);
                Tier01PatternStorage.Add(PatternThemeNum, MonTier01SpawnList);
            }
            else//����� �׸��� �������
            {
                Tier01PatternStorage[PatternThemeNum].Add(MSPattern);
            }
        }
        //SaveTier02SpawnPattern
        foreach (MonSpawnPattern MSPattern in Tier02SpawnPatternSO)
        {
            int PatternThemeNum = MSPattern.SpawnPatternID / 1000;
            //����� �׸��� �������
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

    public void SetBossSpawn(PlayerManager PMgr)//����� ������ ���ʷ� ���Ҷ��� ����
    {//1�������� ��� 1200 ~ // 2�������� ��� 2200~
        int ThemeNum = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor;
        int DetailOfEvents = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails;
        int RandBossPatternID = Random.Range(0, BossPatternStorage[ThemeNum].Count);

        CurrentSpawnPattern = BossPatternStorage[ThemeNum][RandBossPatternID];
        PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
        JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());
    }
    public void SetSpawnPattern(PlayerManager PMgr)
    {
        //DetailOfEvents�� 1000 ~ �� �� 1�׸���� 1000~ 2�׸� ��� 2000~���̴�
        int ThemeNum = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentFloor;
        int DetailOfEvents = PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails;
        int CurrentSearchPoint = PMgr.GetPlayerInfo().GetPlayerStateInfo().DetectNextFloorPoint;//0 -> 1Ƽ�� 100% 40 -> 2Ƽ�� 100%
        //������ �����ϱ� ���� �̹� ������ ������ �ִٸ� ���� -> �̹� ���� ������ ������ ����(���� Ų����)
        if(DetailOfEvents % 1000 < 100 && DetailOfEvents % 1000 >= 0)
        {//0~99�̶�� 1Ƽ�� // 0 ~ 99�� 0�̸� ������ �����ؾ� �Ǵ°���
            for (int i = 0; i < Tier01PatternStorage[ThemeNum].Count; i++)
            {
                if (Tier01PatternStorage[ThemeNum][i].SpawnPatternID == DetailOfEvents)
                {
                    CurrentSpawnPattern = Tier01PatternStorage[ThemeNum][i];
                    return;
                }
            }
        }
        else if(DetailOfEvents % 1000 >= 100 && DetailOfEvents % 1000 < 200 )
        {//100 ~ 199��� 2Ƽ��
            for (int i = 0; i < Tier02PatternStorage[ThemeNum].Count; i++)
            {
                if (Tier02PatternStorage[ThemeNum][i].SpawnPatternID == DetailOfEvents)
                {
                    CurrentSpawnPattern = Tier02PatternStorage[ThemeNum][i];
                    return;
                }
            }
        }
        else if(DetailOfEvents % 1000 >= 200 && DetailOfEvents % 1000 < 300)
        {//�������϶� ���� Ű�� ����� ����//200 ~ 299��� ����
            for (int i = 0; i < BossPatternStorage[ThemeNum].Count; i++)
            {
                if (BossPatternStorage[ThemeNum][i].SpawnPatternID == DetailOfEvents)
                {
                    CurrentSpawnPattern = BossPatternStorage[ThemeNum][i];
                    return;
                }
            }
        }
        else if(DetailOfEvents % 1000 >= 300 && DetailOfEvents % 1000 < 400)
        {//300 ~ 399��� �̺�Ʈ ���� ����
            for(int i = 0; i < EventPatternStorage[ThemeNum].Count; i++)
            {
                if (EventPatternStorage[ThemeNum][i].SpawnPatternID == DetailOfEvents)
                {
                    CurrentSpawnPattern = EventPatternStorage[ThemeNum][i];
                    return;
                }
            }
        }

        //������ �ɷ����� �ʾҴٸ� ���� ����
        int RandNum = Random.Range(0, 41);
        if(RandNum >= CurrentSearchPoint)
        {//���Ⱑ 1Ƽ��
            if(Tier01PatternStorage.ContainsKey(ThemeNum))
            {//�ش� �׸��� ���ϰ� ������
                RandNum = Random.Range(0, Tier01PatternStorage[ThemeNum].Count);
                CurrentSpawnPattern = Tier01PatternStorage[ThemeNum][RandNum];
                PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
                JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());//���� �Ȱ� ����
                return;
            }
        }
        else
        {//���Ⱑ 2Ƽ��
            if (Tier02PatternStorage.ContainsKey(ThemeNum))
            {//�ش� �׸��� ���ϰ� ������
                RandNum = Random.Range(0, Tier02PatternStorage[ThemeNum].Count);
                CurrentSpawnPattern = Tier02PatternStorage[ThemeNum][RandNum];
                PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
                JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());//���� �Ȱ� ����
                return;
            }
        }

        //���� ���� �°Ÿ� ThemeNum�� ���°���
        Debug.Log("NoThemeNum");
    }

    public void SpawnCurrentSpawnPatternMonster()
    {
        for(int i = 0; i < ActiveMonsters.Count; i++)
        {
            ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
        }
        ActiveMonsters.Clear();
        for (int i = 0; i < CurrentSpawnPattern.SpawnMonsterName.Length; i++)//0~2���� �ۿ� �ȳ���
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

    public void SpawnMonsterBySummonMonster(List<string> MonsterID)
    {
        //��� �ִ� ���Ϳ� ���� �������� ��ġ�� ��ǥ�� ���ؼ� ���������� ������ ã��
        //����ִ� ���Ͱ� 3���� �̻��̸� �׳� return -> ��¥�� ��� �ִ°��� ����
        //���࿡ ���� ������ ��ġ�� ���ٸ� �״�� return(�������� ����)
        //���� ������ ��ġ�� �ִٸ� �տ��� ���� �����ϰ� ���� ������ ��ġ�� �������� �������� ����
        //MonsterID.Count�� CanSpwanPont.Count�� ū���� �������� ���͸� ������
        if (MonsterID.Count >= InAdvanceAmount)
            return;

        List<Vector2> CanSpawnPoint = new List<Vector2>();
        for(int i = 0; i < MonsterSpawnPoint.Length; i++)
        {
            bool IsEmptySpawnPoint = true;
            Debug.Log(ActiveMonsters.Count);
            for (int j = 0; j < ActiveMonsters.Count; j++)
            {
                //ActiveMonster�� ��ΰ� MonsterSpawnPoint[i]�� ���� ��ġ�� �ƴϿ������� ����ִ� ����
                if (Vector2.Distance(MonsterSpawnPoint[i].transform.position, ActiveMonsters[j].transform.position) <= 0.1f)
                {
                    IsEmptySpawnPoint = false;
                    break;
                    //���� ���Դٴ� �� = ������� ���� ����
                }
            }
            if(IsEmptySpawnPoint == true)
            {
                //�̰� true�̴� : �� MonsterSpawnPoint[i]�� ��ǥ�� ����ִ� ����
                CanSpawnPoint.Add(MonsterSpawnPoint[i].transform.position);
            }
        }

        if (CanSpawnPoint.Count <= 0)//����ִ� ��Ұ� ���ٸ�
            return;

        int SpawnCount = 0;
        if (CanSpawnPoint.Count >= MonsterID.Count)//���� ���� ��������
            SpawnCount = MonsterID.Count;
        else
            SpawnCount = CanSpawnPoint.Count;

        for(int i = 0; i < SpawnCount; i ++)
        {
            if (MonsterStorage.ContainsKey(MonsterID[i]))//�ش� ���Ͱ� �ִٸ�
            {
                for(int j = 0; j < MonsterStorage[MonsterID[i]].Count; j++)
                {
                    if (MonsterStorage[MonsterID[i]][j].activeSelf == false)//������������ ��ȯ
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

    //ActiveMonster�� CurrentHp�� 0���ϰ� �ȳ��� ���ֱ�
    public List<int> CheckActiveMonstersRSurvive(PlayerManager PlayerMgr)
    {
        List<int> DeadMonsterReward = new List<int>();
        for(int i = ActiveMonsters.Count - 1; i >= 0; i-- )
        {
            if (ActiveMonsters[i].GetComponent<Monster>().GetMonsterCurrentStatus().MonsterCurrentHP <= 0)
            {
                if (ActiveMonsters[i].GetComponent<Monster>().IsTierOne == true)//1Ƽ��
                {
                    PlayerMgr.GetPlayerInfo().RecordKillCount(1);
                }
                else
                {
                    PlayerMgr.GetPlayerInfo().RecordKillCount(0,1);
                }
                ActiveMonsters[i].GetComponent<Monster>().InitAllBuff();//��� ������ ���ֱ�
                ActiveMonsters[i].GetComponent<Monster>().MonsterClicked -= SetCurrentTargetMonster;
                ActiveMonsters[i].GetComponent<Monster>().DeSpawnFadeOut();
                int RewardEXP = (int)ActiveMonsters[i].GetComponent<Monster>().MonsterBaseEXP;
                int VarianceEXP = Random.Range(-(int)ActiveMonsters[i].GetComponent<Monster>().EXPVarianceAmount, (int)ActiveMonsters[i].GetComponent<Monster>().EXPVarianceAmount);
                DeadMonsterReward.Add(RewardEXP + VarianceEXP);
                ActiveMonsters.RemoveAt(i);
            }
        }
        return DeadMonsterReward;
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
    public void SetCurrentTargetMonster(Monster ClickedMonster)//���Ͱ� Ŭ���� �Ǹ� �� �Լ��� �����
    {
        //MainBattleUI�� ���� �������� Ŭ���� �ȵǾߵ�
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
