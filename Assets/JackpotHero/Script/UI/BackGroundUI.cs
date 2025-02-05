using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundUI : MonoBehaviour
{
    public float MovingTime;
    public GameObject[] BackGrounds;

    protected List<float> TargetPosX = new List<float>();
    protected const float LeftBGSortPos = 1920f;

    protected bool IsMoveEnd = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool GetIsMoveEnd()
    {
        return IsMoveEnd;
    }

    public void MoveBackGround()
    {
        if (BackGrounds.Length < 2)
            return;

        IsMoveEnd = false;
        TargetPosX.Clear();
        for(int i = 0; i < BackGrounds.Length; i++)
        {
            float TargetPos = BackGrounds[i].GetComponent<RectTransform>().anchoredPosition.x;
            TargetPosX.Add(TargetPos - 1920f);
        }
        StartCoroutine(MovingBackGround());
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
}
