using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class BackGroundUI : MonoBehaviour
{
    public Sprite[] BackGroundTexture;
    public float MovingTime;
    public GameObject[] BackGrounds;
    public GameObject RestBackGround;

    protected List<float> TargetPosX = new List<float>();
    protected const float LeftBGSortPos = 1920f;

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
        if (ThemeNum - 1 >= BackGroundTexture.Length)
            return;

        foreach(GameObject BackObj in BackGrounds)
        {
            BackObj.GetComponent<Image>().sprite = BackGroundTexture[ThemeNum - 1];
        }
    }

    public void MoveBackGround()
    {
        if (BackGrounds.Length < 2)
            return;

        IsMoveEnd = false;

        foreach(GameObject BackGroundObject in BackGrounds)
        {
            BackGroundObject.GetComponent<RectTransform>().
                DOAnchorPosX(BackGroundObject.GetComponent<RectTransform>().anchoredPosition.x - 1920f, MovingTime).SetEase(Ease.Linear).
                OnComplete(() => { CheckBackGroundLeftPos(BackGroundObject); });
        }
    }

    protected void CheckBackGroundLeftPos(GameObject CheckBackGround)//특정 좌표 이하에 간놈이 있다면 1920으로 좌표를 되돌린다.
    {
        //여기에 들어왔다는것 자체가 이미 다 이동한거임
        if(IsMoveEnd == false)
        {
            IsMoveEnd = true;
        }
        if (CheckBackGround.GetComponent<RectTransform>().anchoredPosition.x < -3800f)
        {
            CheckBackGround.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920, 0);
        }
    }

    public void SetRestBackGround(bool BackGroundState)
    {
        RestBackGround.SetActive(BackGroundState);
    }
}
