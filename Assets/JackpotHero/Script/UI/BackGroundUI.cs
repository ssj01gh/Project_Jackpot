using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundUI : MonoBehaviour
{
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
        /*
        TargetPosX.Clear();
        for(int i = 0; i < BackGrounds.Length; i++)
        {
            float TargetPos = BackGrounds[i].GetComponent<RectTransform>().anchoredPosition.x;
            TargetPosX.Add(TargetPos - 1920f);
        }
        StartCoroutine(MovingBackGround());
        */
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

    IEnumerator MovingBackGround()
    {
        while(true)
        {
            yield return null;
            for(int i = 0; i < BackGrounds.Length; i++)
            {
                Vector2 RectPos = BackGrounds[i].GetComponent<RectTransform>().anchoredPosition;
                RectPos.x -= 1920 * Time.deltaTime / MovingTime;
                BackGrounds[i].GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            if (BackGrounds[0].GetComponent<RectTransform>().anchoredPosition.x <= TargetPosX[0])//1개라도 목표치에 도달하면 전부다 도착한거임
            {
                break;//while문을 벗어남
            }
        }

        for(int i = 0; i < BackGrounds.Length; i++)
        {
            Vector2 TargetRectPos = BackGrounds[i].GetComponent<RectTransform>().anchoredPosition;
            if (TargetPosX[i] <= -3800f)//제일 왼쪽에 있는 놈이라면
            {
                TargetPosX[i] = 1920f;//목표 좌표를 오른쪽 끝으로 변경
            }
            TargetRectPos.x = TargetPosX[i];
            BackGrounds[i].GetComponent<RectTransform>().anchoredPosition = TargetRectPos;
        }
        IsMoveEnd = true;
        yield break;
        //yield return null;
    }

    public void SetRestBackGround(bool BackGroundState)
    {
        RestBackGround.SetActive(BackGroundState);
    }
}
