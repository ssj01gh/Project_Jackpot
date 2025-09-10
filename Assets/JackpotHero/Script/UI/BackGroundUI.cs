using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
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
        RestBackGround.GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.RestLayer];
        Layer05_BackGround.GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer05];
        for (int i = 0; i < 3; i++)
        {
            Layer01_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer01];
            Layer02_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer02];
            Layer03_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer03];
            Layer04_BackGround[i].GetComponent<Image>().sprite = BackGroundSprites[ThemeNum - 1].StageBackGroundSprites[(int)ELayer.Layer04];
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
    }

    public void MoveBackGround()
    {
        IsMoveEnd = false;

        foreach(GameObject LayerObject in Layer01_BackGround)
        {
            if (DOTween.IsTweening(LayerObject))
                continue;

            LayerObject.GetComponent<RectTransform>().
                DOAnchorPosX(LayerObject.GetComponent<RectTransform>().anchoredPosition.x - TargetMoveX, MovingTime).SetEase(Ease.Linear).
                OnComplete(() => { CheckBackGroundLeftPos(LayerObject, true); });
        }
        foreach (GameObject LayerObject in Layer02_BackGround)
        {
            if (DOTween.IsTweening(LayerObject))
                continue;

            LayerObject.GetComponent<RectTransform>().
                DOAnchorPosX(LayerObject.GetComponent<RectTransform>().anchoredPosition.x - (TargetMoveX * 0.75f), MovingTime).SetEase(Ease.Linear).
                OnComplete(() => { CheckBackGroundLeftPos(LayerObject, true); });
        }
        foreach (GameObject LayerObject in Layer03_BackGround)
        {
            if (DOTween.IsTweening(LayerObject))
                continue;

            LayerObject.GetComponent<RectTransform>().
                DOAnchorPosX(LayerObject.GetComponent<RectTransform>().anchoredPosition.x - (TargetMoveX*0.5f), MovingTime).SetEase(Ease.Linear).
                OnComplete(() => { CheckBackGroundLeftPos(LayerObject, true); });
        }
        foreach (GameObject LayerObject in Layer04_BackGround)
        {
            if (DOTween.IsTweening(LayerObject))
                continue;

            LayerObject.GetComponent<RectTransform>().
                DOAnchorPosX(LayerObject.GetComponent<RectTransform>().anchoredPosition.x - (TargetMoveX * 0.25f), MovingTime).SetEase(Ease.Linear).
                OnComplete(() => { CheckBackGroundLeftPos(LayerObject, true); });
        }
    }

    protected void CheckBackGroundLeftPos(GameObject CheckBackGround, bool CheckIsMoveEnd = false)//특정 좌표 이하에 간놈이 있다면 1920으로 좌표를 되돌린다.
    {
        //여기에 들어왔다는것 자체가 이미 다 이동한거임
        if(IsMoveEnd == false && CheckIsMoveEnd == true)
        {
            IsMoveEnd = true;
        }

        if (CheckBackGround.GetComponent<RectTransform>().anchoredPosition.x < -2800f)
        {
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
