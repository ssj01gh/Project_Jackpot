using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;
[Serializable]
public class BackGroundSprite
{
    public Sprite[] StageBackGroundSprites;
}

public class BackGroundUI : MonoBehaviour
{
    public float MovingTime;
    //public GameObject[] BackGrounds;
    //public List<GameObject> RestBackGround = new List<GameObject>();
    [Header("BackGroundSprite")]
    public BackGroundSprite[] BackGroundSprites;
    [Header("BackGroundObject")]
    public GameObject RestBackGround;
    public GameObject[] Layer01_BackGround;
    public GameObject[] Layer02_BackGround;
    public GameObject[] Layer03_BackGround;
    public GameObject[] Layer04_BackGround;
    public GameObject Layer05_BackGround;
    //public List<GameObject[]> Rayer01_BackGround = new List<GameObject[]>();
    //public List<GameObject[]> Rayer02_BackGround = new List<GameObject[]>();
    //public List<GameObject[]> Rayer03_BackGround = new List<GameObject[]>();
    //public List<GameObject[]> Rayer04_BackGround = new List<GameObject[]>();
    //public List<GameObject> Rayer05_BackGround = new List<GameObject>();

    protected List<float> TargetPosX = new List<float>();
    protected const float LeftBGSortPos = 1920f;
    protected const float TargetMoveX = 1440f;

    protected enum ELayer
    {
        Layer01,
        Layer02,
        Layer03,
        Layer04,
        Layer05,
        RestLayer
    }

    public bool IsMoveEnd { protected set; get; } = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBackGroundSprite(int ThemeNum)
    {
        /*
        if (ThemeNum - 1 >= BackGroundObjects.Length)
            return;
        */
        if(ThemeNum != 1 && ThemeNum != 2 && ThemeNum != 3 && ThemeNum != 4)
            ThemeNum = 1;

        //각 저장소에 아무것도 없는 곳에는 null(투명) 이미지을 등록
        //ThemeNum == 3일때는 좀 특별하게 해야 할듯?
        //-> 첫번째 레이어랑 RestBackGround만 사용하기 때문에

        if(ThemeNum == 3)
        {
            //ELayer.Layer01 = 일반 복도, ELayer.Lyaer02 = 문있는 복도, ELayer.Layer03 = 창문있는 복도, ELayer.RestLayer = 밤 창문 복도
            //ELyaer.Layer04, ELayer.Layer05 = Nuyll
            RestBackGround.GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.RestLayer];
            Layer05_BackGround.GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer05];
            for (int i = 0; i < 3; i++)
            {
                //60% 확률로 1번 벡그 0 ~ 5, 30% 확률로 2번 벡그 6 ~ 8, 10% 확률로 3번 벡그 9
                int RandNum = UnityEngine.Random.Range(0, 10);
                if(RandNum >= 0 && RandNum <= 5)
                    Layer01_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer01];
                else if(RandNum >= 6 && RandNum <= 8)
                    Layer01_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer02];
                else
                    Layer01_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer03];



                Layer02_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer04];
                Layer03_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer04];
                Layer04_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer04];
            }
        }
        else
        {
            RestBackGround.GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.RestLayer];
            Layer05_BackGround.GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer05];
            for (int i = 0; i < 3; i++)
            {
                Layer01_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer01];
                Layer02_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer02];
                Layer03_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer03];
                Layer04_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer04];
            }
        }

        if(ThemeNum == 1)
        {
            foreach(GameObject LayerObject in Layer04_BackGround)
            {
                LoopBackGround(0.15f, LayerObject);
            }
        }
        else
        {
            foreach(GameObject LayerObject in Layer04_BackGround)
            {
                LayerObject.GetComponent<RectTransform>().DOKill();
            }
        }
    }

    protected void LoopBackGround(float SpeedRatio, GameObject TargetObject)
    {
        float MoveAmount = TargetMoveX * SpeedRatio;
        float TargetX = TargetObject.GetComponent<RectTransform>().anchoredPosition.x - MoveAmount;

        TargetObject.GetComponent<RectTransform>().DOKill();
        //TargetMoveX = 1440;

        TargetObject.GetComponent<RectTransform>().
            DOAnchorPosX(TargetX, MovingTime).SetEase(Ease.Linear).
            OnComplete(() => 
            {
                if (TargetObject.GetComponent<RectTransform>().anchoredPosition.x <= -2800)
                {
                    Vector3 ReturnPos = TargetObject.GetComponent<RectTransform>().anchoredPosition;
                    ReturnPos.x += 5760f;
                    TargetObject.GetComponent<RectTransform>().anchoredPosition = ReturnPos;
                }
                LoopBackGround(SpeedRatio, TargetObject); 
            });
        //이 함수들로 구름을 이동 시킬때 구름이 역주행 하는 버그가 있음.... 왜 그럴까?
        //역주행 하는 이유 -> 목표 좌표가 현재 좌표 +5760으로 됬다. -> 이게 제일 확률이 높다?
        //왜 5760이 되지?
    }

    public void MoveBackGround(int ThemeNum)
    {
        IsMoveEnd = false;

        foreach(GameObject LayerObject in Layer01_BackGround)
        {
            if (DOTween.IsTweening(LayerObject))
                continue;

            LayerObject.GetComponent<RectTransform>().
                DOAnchorPosX(LayerObject.GetComponent<RectTransform>().anchoredPosition.x - TargetMoveX, MovingTime).SetEase(Ease.Linear).
                OnComplete(() => { CheckBackGroundLeftPos(LayerObject, ThemeNum ,true); });
        }
        foreach (GameObject LayerObject in Layer02_BackGround)
        {
            if (DOTween.IsTweening(LayerObject))
                continue;

            LayerObject.GetComponent<RectTransform>().
                DOAnchorPosX(LayerObject.GetComponent<RectTransform>().anchoredPosition.x - (TargetMoveX * 0.75f), MovingTime).SetEase(Ease.Linear).
                OnComplete(() => { CheckBackGroundLeftPos(LayerObject, ThemeNum, true); });
        }
        foreach (GameObject LayerObject in Layer03_BackGround)
        {
            if (DOTween.IsTweening(LayerObject))
                continue;

            LayerObject.GetComponent<RectTransform>().
                DOAnchorPosX(LayerObject.GetComponent<RectTransform>().anchoredPosition.x - (TargetMoveX*0.5f), MovingTime).SetEase(Ease.Linear).
                OnComplete(() => { CheckBackGroundLeftPos(LayerObject, ThemeNum, true); });
        }
        foreach (GameObject LayerObject in Layer04_BackGround)
        {
            if (ThemeNum == 1)
                break;

            if (DOTween.IsTweening(LayerObject))
                continue;

            LayerObject.GetComponent<RectTransform>().
                DOAnchorPosX(LayerObject.GetComponent<RectTransform>().anchoredPosition.x - (TargetMoveX * 0.25f), MovingTime).SetEase(Ease.Linear).
                OnComplete(() => { CheckBackGroundLeftPos(LayerObject, ThemeNum, true); });
        }
    }

    protected void CheckBackGroundLeftPos(GameObject CheckBackGround, int ThemeNum, bool CheckIsMoveEnd = false)//특정 좌표 이하에 간놈이 있다면 1920으로 좌표를 되돌린다.
    {
        //여기에 들어왔다는것 자체가 이미 다 이동한거임
        if(IsMoveEnd == false && CheckIsMoveEnd == true)
        {
            IsMoveEnd = true;
        }

        if (CheckBackGround.GetComponent<RectTransform>().anchoredPosition.x < -2800f)
        {
            if(ThemeNum == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    //60% 확률로 1번 벡그 0 ~ 5, 30% 확률로 2번 벡그 6 ~ 8, 10% 확률로 3번 벡그 9
                    int RandNum = UnityEngine.Random.Range(0, 10);
                    if (RandNum >= 0 && RandNum <= 5)
                        CheckBackGround.GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer01];
                    else if (RandNum >= 6 && RandNum <= 8)
                        CheckBackGround.GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer02];
                    else
                        CheckBackGround.GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer03];

                    Layer02_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer04];
                    Layer03_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer04];
                    Layer04_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer04];
                }
            }
            Vector3 ReturnPos = CheckBackGround.GetComponent<RectTransform>().anchoredPosition;
            ReturnPos.x += 5760f;
            CheckBackGround.GetComponent<RectTransform>().anchoredPosition = ReturnPos;
        }
    }

    public void SetRestBackGround(bool BackGroundState)
    {
        RestBackGround.SetActive(BackGroundState);
    }
}
