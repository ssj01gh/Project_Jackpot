using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestManager : MonoBehaviour
{
    [SerializeField]
    private PlayerManager PlayerMgr;
    [SerializeField]
    private PlaySceneUIManager UIMgr;
    [SerializeField]
    private PlaySceneManager PlaySceneMgr;
    

    public Slider TimeCountSlider;
    // Start is called before the first frame update
    public List<bool> IsPeacefulRest { protected set; get; } = new List<bool>();
    protected int MaxRestTime = 0;
    protected int CurrentRestTime = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRestResult(int RestQuality)//����� �����Ѵ�. <- �޾ƾ� �ϴ� ���� �޽� Ƚ��, �޽� ����Ƽ
    {
        //VeryBad -> 0 ~ 49, Bad -> 0 ~ 24, Good -> 0 ~ 9, VeryGood -> 0 ~ 4, Perfect -> Ȯ�� ����
        MaxRestTime = (int)TimeCountSlider.value;
        CurrentRestTime = 0;
        IsPeacefulRest.Clear();
        int MonAttackRange = 0;

        switch (RestQuality)
        {
            case (int)EPlayerRestQuality.VeryBad:
                MonAttackRange = 50;
                break;
            case (int)EPlayerRestQuality.Bad:
                MonAttackRange = 25;
                break;
            case (int)EPlayerRestQuality.Good:
                MonAttackRange = 10;
                break;
            case (int)EPlayerRestQuality.VeryGood:
                MonAttackRange = 5;
                break;
            case (int)EPlayerRestQuality.Perfect:
                MonAttackRange = 0;
                break;
        }

        for (int i = 0; i < MaxRestTime; i++)
        {
            int RandNum = Random.Range(0, 100);
            if (RandNum < MonAttackRange)// 50�̻� ������ �佺 //25�̻� ������ �佺
            {//���� �ɸ��� ���� ������
                IsPeacefulRest.Add(false);
            }
            else
            {
                IsPeacefulRest.Add(true);
            }
        }
    }

    public void StartRestCheck(RestUIScript R_UI)
    {
        StartCoroutine(RestCheckCoroutine(R_UI));
    }

    IEnumerator RestCheckCoroutine(RestUIScript R_UI)
    {
        yield return null;
        while(CurrentRestTime < MaxRestTime)
        {
            if (IsPeacefulRest[CurrentRestTime] == true)//��ȭ�Ӱ� �������ٸ�
            {
                R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1)) / MaxRestTime);
                while(true)
                {
                    yield return null;
                    if(R_UI.FillAmountAnimEnd == true)
                    {
                        break;
                    }
                }
                PlayerMgr.GetPlayerInfo().RecoverHPNSTAByRest(0.1f);
                UIMgr.PSI_UI.SetPlayerStateUI(PlayerMgr.GetPlayerInfo().GetTotalPlayerStateInfo(), PlayerMgr.GetPlayerInfo().GetPlayerStateInfo());
                CurrentRestTime++;
            }
            else//������ �Ͼ�ٸ�
            {
                R_UI.InActiveLeftTimeObject();
                //�޽� UI�� ����
                PlaySceneMgr.SuddenAttackByMonsterInRest(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails);
                /*
                MonMgr.SetSpawnPattern(PlayerMgr);//������ ���� ���� ����
                MonMgr.SpawnCurrentSpawnPatternMonster();//���� ���Ͽ� �°� ���� ����//ActiveMonster����
                BattleMgr.InitMonsterNPlayerActiveGuage(MonMgr.GetActiveMonsters().Count, MonMgr);
                PlayerSceneMgr.ProgressBattle();
                */
                //�̰� �ӽ�
                R_UI.SetLeftTimeTextNSlider(MaxRestTime - (CurrentRestTime + 1), (float)(MaxRestTime - (CurrentRestTime + 1)) / MaxRestTime);
                while (true)
                {
                    yield return null;
                    if (R_UI.FillAmountAnimEnd == true)
                    {
                        break;
                    }
                }
                CurrentRestTime++;
                Debug.Log("AAAAAAA");
            }
            yield return null;
        }
        //ȸ���� ������ �ٽ� �޽� �ൿ ����â�� ���;���
        //Player�� ������ Json�� �����ϰ�
        Debug.Log("RestEnd");
    }
}
