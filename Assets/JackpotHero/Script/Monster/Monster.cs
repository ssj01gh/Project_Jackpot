using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//이건 계속해서 늘어나야 할 enum일듯?
public enum MonsterActionState
{
    Attack,
    Defense,
}
public class MonsterCurrentStatus
{
    public float MonsterMaxHP;
    public float MonsterCurrentHP;

    public float MonsterCurrentActionGauge;
    public float MonsterCurrentShieldPoint;

    public float MonsterCurrentATK;
    public float MonsterCurrentDUR;
    public float MonsterCurrentSPD;
}

public class Monster : MonoBehaviour
{
    public string MonsterName;
    public SpriteRenderer MonsterBody;
    [Header("HP_MonsterState")]
    public float MonsterBaseHP;
    public float HPVarianceAmount;
    [Header("ATK_MonsterState")]
    public float MonsterBaseATK;
    public float ATKVarianceAmount;
    [Header("DUR_MonsterState")]
    public float MonsterBaseDUR;
    public float DURVarianceAmount;
    [Header("SPD_MonsterState")]
    public float MonsterBaseSPD;
    public float SPDVarianceAmount;

    public int MonsterWeaponCode;
    public int MonsterArmorCode;
    public int[] MonsterAnotherEquipmentCode;
    [Header("MonsterUIPos")]
    public GameObject MonsterShieldPos;
    public GameObject HpSliderPos;
    public float HpsliderWidth;
    public float BuffPosUpperHp;

    //--------------------^GetFromInspector\

    public event System.Action<Monster> MonsterClicked;
    [HideInInspector]
    public int MonsterCurrentState;
    protected MonsterCurrentStatus MonTotalStatus = new MonsterCurrentStatus();
    protected Collider2D MonCollider;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        MonCollider = gameObject.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (MonCollider != null && MonCollider.OverlapPoint(mousePosition))  // 충돌 검사//지금 이거는 MainBattle이 진행중일때도 바뀌어 버림 나중에 바꿔야함
            {
                MonsterClicked.Invoke(gameObject.GetComponent<Monster>());
            }
        }
    }

    public void SpawnMonster(Vector2 SpawnPosition)
    {
        gameObject.SetActive(true);
        Color MonColor = MonsterBody.color;
        MonColor.a = 0;
        MonsterBody.color = MonColor;
        gameObject.transform.position = SpawnPosition;
        //SetHP
        int Rand = Random.Range(-(int)HPVarianceAmount, (int)HPVarianceAmount + 1);
        MonTotalStatus.MonsterMaxHP = MonsterBaseHP + Rand;
        MonTotalStatus.MonsterCurrentHP = MonTotalStatus.MonsterMaxHP;
        //SetATK
        Rand = Random.Range(-(int)ATKVarianceAmount, (int)ATKVarianceAmount + 1);
        MonTotalStatus.MonsterCurrentATK = MonsterBaseATK + Rand;
        //SetDUR
        Rand = Random.Range(-(int)DURVarianceAmount, (int)DURVarianceAmount + 1);
        MonTotalStatus.MonsterCurrentDUR = MonsterBaseDUR + Rand;
        //SetSPD
        Rand = Random.Range(-(int)SPDVarianceAmount, (int)SPDVarianceAmount + 1);
        MonTotalStatus.MonsterCurrentSPD = MonsterBaseSPD + Rand;

        StartCoroutine(SpawnFadeIn());
    }

    IEnumerator SpawnFadeIn()
    {
        Color MonColor = MonsterBody.color;
        MonColor.a = 0;
        while (true)
        {
            yield return null;
            MonColor.a += Time.deltaTime;//1초에 걸쳐서 1로;
            MonsterBody.color = MonColor;
            if(MonColor.a >= 1)
            {
                break;
            }
        }
        yield break;
    }

    public MonsterCurrentStatus GetMonsterCurrentStatus()
    {
        return MonTotalStatus;
    }
}
