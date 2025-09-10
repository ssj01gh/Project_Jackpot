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
    //public GameObject[] LogoStar;
    // Start is called before the first frame update
    void Start()
    {
        SetContinueButton();
        _OptionUI.gameObject.SetActive(false);
        StartLogoAnimation();
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
