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

    public void SetRestResult(int RestQuality)//결과를 결정한다. <- 받아야 하는 것은 휴식 횟수, 휴식 퀄리티
    {
        //VeryBad -> 0 ~ 49, Bad -> 0 ~ 24, Good -> 0 ~ 9, VeryGood -> 0 ~ 4, Perfect -> 확률 없음
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
            if (RandNum < MonAttackRange)// 50이상 나오면 페스 //25이상 나오면 페스
            {//여기 걸리면 몬스터 조우임
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
            if (IsPeacefulRest[CurrentRestTime] == true)//평화롭게 지나간다면
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
            else//습격이 일어난다면
            {
                R_UI.InActiveLeftTimeObject();
                //휴식 UI는 끄고
                PlaySceneMgr.SuddenAttackByMonsterInRest(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerActionDetails);
                /*
                MonMgr.SetSpawnPattern(PlayerMgr);//몬스터의 스폰 패턴 설정
                MonMgr.SpawnCurrentSpawnPatternMonster();//스폰 패턴에 맞게 몬스터 생성//ActiveMonster설정
                BattleMgr.InitMonsterNPlayerActiveGuage(MonMgr.GetActiveMonsters().Count, MonMgr);
                PlayerSceneMgr.ProgressBattle();
                */
                //이거 임시
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
        //회복이 끝나면 다시 휴식 행동 선택창이 나와야함
        //Player의 정보도 Json에 갱신하고
        Debug.Log("RestEnd");
    }
}
