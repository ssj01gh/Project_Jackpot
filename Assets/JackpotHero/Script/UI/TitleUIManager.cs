using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class TitleUIManager : MonoBehaviour
{
    public OptionUI _OptionUI;
    public EarlyStrengthenUI _EarlyUI;
    public Button ContinueButton;
    public GameObject LogoCard;
    public GameObject[] TitleCloud;
    //public GameObject[] LogoStar;
    // Start is called before the first frame update

    private Vector2 CloudInitPos01 = new Vector2(-1920f, 0f);
    private Vector2 CloudInitPos02 = Vector2.zero;
    private Vector2 CloudInitPos03 = new Vector2(1920f, 0f);
    void Start()
    {
        SetContinueButton();
        _OptionUI.gameObject.SetActive(false);
        StartLogoAnimation();
        StartTitleCloudAnimation();
        //_EarlyUI.gameObject.SetActive(false);
        SoundManager.Instance.PlayBGM("TitleBGM");
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void StartLogoAnimation()
    {
        Vector3 OriginRotation = LogoCard.GetComponent<RectTransform>().transform.eulerAngles;
        LogoCard.GetComponent<RectTransform>().transform.DORotate(new Vector3(0, 360f, OriginRotation.z), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart); // 무한 반복

        /*
        foreach(GameObject Star in LogoStar)
        {
            float RandomTime = Random.Range(0.4f, 0.8f);
            Star.GetComponent<Image>().DOFade(0.1f, RandomTime).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        */
        //LogoStar.GetComponent<RectTransform>().transform.DORotate(new Vector3(0, 360f, 0), 3f, RotateMode.FastBeyond360)
        // .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        //LogoStar.GetComponent<Image>().DOFade(0.2f, 0.5f)
        //.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    protected void StartTitleCloudAnimation()
    {
        foreach(GameObject TargetObject in TitleCloud)
        {
            LoopBackGround(TargetObject);
        }
    }

    protected void LoopBackGround(GameObject TargetObject)
    {
        float TargetMoveX = 7f;
        float MovingTime = 1f;
        float MoveAmount = TargetMoveX;
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
                LoopBackGround(TargetObject);
            });
        //이 함수들로 구름을 이동 시킬때 구름이 역주행 하는 버그가 있음.... 왜 그럴까?
        //역주행 하는 이유 -> 목표 좌표가 현재 좌표 +5760으로 됬다. -> 이게 제일 확률이 높다?
        //왜 5760이 되지?
    }

    protected void SetContinueButton()
    {
        if (JsonReadWriteManager.Instance.P_Info.CurrentFloor <= 0)
        {
            ContinueButton.transform.localScale = Vector3.zero;
            ContinueButton.interactable = false;
        }
        else
        {
            ContinueButton.transform.localScale = Vector3.one;
            ContinueButton.interactable = true;
        }
    }

    public void ContinueButtonClick()
    {
        //Debug.Log("Aaaaaaa");
        SoundManager.Instance.PlayUISFX("UI_Button");
        LoadingScene.Instance.LoadAnotherScene("PlayScene");
    }
    public void StartButtonClick()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        _EarlyUI.EarlyStrengthenActive();
    }
    public void OptionButtonClick()
    {
        _OptionUI.OptionUIActive();
    }
    public void ExitButtonClick()
    {
#if UNITY_EDITOR
        SoundManager.Instance.PlayUISFX("UI_Button");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        SoundManager.Instance.PlayUISFX("UI_Button");
        Application.Quit();
#endif
    }
}
