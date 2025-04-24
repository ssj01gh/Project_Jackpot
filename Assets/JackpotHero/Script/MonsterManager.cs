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

    public void SetBossSpawn(PlayerManager PMgr)
    {//1�������� ��� 1101 ~ // 2�������� ��� 1201~
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
                    SetCurrentSpawnPatternReward(PMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack);
                    PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
                    JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());
                    return;//�Լ� ����
                }
            }
            //���� ��ġ�°� ���ٸ�
            int RandomPattern = Random.Range(0, PatternStorage[ThemeOfEvent].Count);
            CurrentSpawnPattern = PatternStorage[ThemeOfEvent][RandomPattern];
            SetCurrentSpawnPatternReward(PMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack);
            PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
            JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());
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
        SetCurrentSpawnPatternReward(PMgr.GetPlayerInfo().GetPlayerStateInfo().SaveRestQualityBySuddenAttack);
        PMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails = CurrentSpawnPattern.SpawnPatternID;
        JsonReadWriteManager.Instance.SavePlayerInfo(PMgr.GetPlayerInfo().GetPlayerStateInfo());
    }

    protected void SetCurrentSpawnPatternReward(int IsSuddenAttack)
    {
        if(IsSuddenAttack == -1)//������ �ƴҶ���
        {
            CurrentSpawnPatternReward = CurrentSpawnPattern.RewardEXPPoint;
            float RandVariation = Random.Range(-(int)CurrentSpawnPattern.VariationEXPPoint, (int)CurrentSpawnPattern.VariationEXPPoint + 1);
            CurrentSpawnPatternReward += RandVariation;
        }
        else//-1�� �ƴ϶�� �����ΰ���
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
