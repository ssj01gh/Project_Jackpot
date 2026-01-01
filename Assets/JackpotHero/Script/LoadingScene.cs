using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoSingletonDontDestroy<LoadingScene>
{
    // Start is called before the first frame update
    public Canvas LoadingCanvas;
    public GameObject LoadingStar;
    public GameObject FillObject;
    public Image FillImage;
    public GameObject BlackBackGround;
    public GameObject LoadingCard;

    //private Vector3 LoadingStarInitScale = Vector3.zero;
    //private Vector3 LoadingStarTargetScale = new Vector3(3f, 3f, 3f);

    //private Vector3 LoadingStarInitRotate = Vector3.zero;
    //private Vector2 LoadingStarInitSize = Vector2.zero;
    //private float LoadingStarRatio = 1.235f;
    //private Color LoadingStarInitColor = new Color(1f, 1f, 1f, 0f);

    private Color LoadingBackGroundColor = new Color(0f, 0f, 0f, 0f);
    void Start()
    {
        LoadingCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void LoadAnotherScene(string SceneName)
    {
        /*
        if (SceneName == "TitleScene")
            SoundManager.Instance.StopBGM();
        */
        //1. fade
        //2. y축 회전
        //3. 둘다
        /*
        FillObject.SetActive(false);
        FillImage.fillAmount = 0f;
        //LoadingStar.GetComponent<RectTransform>().transform.localScale = LoadingStarInitScale;
        float LoadingStarTargetWidth = Screen.width;
        float LoadingStarTargetHeight = LoadingStarTargetWidth * LoadingStarRatio;
        //꽉 채우려면 *3정도?
        LoadingStarTargetWidth *= 3;
        LoadingStarTargetHeight *= 3;
        LoadingStar.GetComponent<RectTransform>().transform.eulerAngles = LoadingStarInitRotate;
        LoadingStar.GetComponent<RectTransform>().sizeDelta = LoadingStarInitSize;
        LoadingStar.GetComponent<Image>().color = LoadingStarInitColor;

        LoadingCanvas.gameObject.SetActive(true);

        LoadingStar.GetComponent<Image>().DOFade(1f, 0.5f).SetEase(Ease.Linear);
        LoadingStar.GetComponent<RectTransform>().DOSizeDelta(new Vector2(LoadingStarTargetWidth, LoadingStarTargetHeight), 0.5f).SetEase(Ease.Linear);
        LoadingStar.GetComponent<RectTransform>().transform.DORotate(new Vector3(0, 180f, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear)
        .OnComplete(() => 
        {//다되면(화면을 다 가리면)로딩 시작
            FillObject.SetActive(true);
            StartCoroutine(DisplayProgressBar(SceneName));
        });
        */
        /*
        FillObject.SetActive(false);
        FillImage.fillAmount = 0f;
        BlackBackGround.GetComponent<Image>().color = LoadingBackGroundColor;
        BlackBackGround.SetActive(true);

        LoadingCanvas.gameObject.SetActive(true);

        BlackBackGround.GetComponent<Image>().DOFade(1f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            FillObject.SetActive(true);
            StartCoroutine(DisplayProgressBar(SceneName));
        });
        */
        LoadingCard.SetActive(false);
        LoadingCard.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        BlackBackGround.GetComponent<Image>().color = LoadingBackGroundColor;
        BlackBackGround.SetActive(true);

        LoadingCanvas.gameObject.SetActive(true);

        BlackBackGround.GetComponent<Image>().DOFade(1f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            LoadingCard.SetActive(true);
            LoadingCard.GetComponent<RectTransform>().DOKill();
            Vector3 OriginRotation = LoadingCard.GetComponent<RectTransform>().transform.eulerAngles;
            LoadingCard.GetComponent<RectTransform>().transform.DORotate(new Vector3(0, 360f, OriginRotation.z), 2f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart); // 무한 반복
            StartCoroutine(DisplayProgressBar(SceneName));
        });
        //LoadingBackGround.SetActive(true);
        //LoadingAnimator.SetInteger("LoadingCanvasState", 0);
        //StartCoroutine(DisplayProgressBar(SceneName));
    }

    IEnumerator DisplayProgressBar(string SceneName)
    {
        AsyncOperation Operation = SceneManager.LoadSceneAsync(SceneName);
        Operation.allowSceneActivation = false;

        while (Operation.progress < 0.9f)
        {
            yield return null;
            //FillImage.fillAmount = Operation.progress;
        }

        Operation.allowSceneActivation = true;

        while (true)
        {
            yield return null;
            if (SceneManager.GetActiveScene().name == SceneName)
                break;
        }

        yield return new WaitForSeconds(0.5f);

        while(!(Mathf.Abs(LoadingCard.GetComponent<RectTransform>().localEulerAngles.z) < 0.1f))
        {
            yield return null;
        }
        LoadingCard.GetComponent<RectTransform>().DOKill();
        LoadingCard.SetActive(false);
        BlackBackGround.GetComponent<Image>().DOFade(0f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            LoadingCanvas.gameObject.SetActive(false);
        });
        /*
        while (FillImage.fillAmount < 1)
        {
            yield return null;
            float _FillAmount = FillImage.fillAmount;
            _FillAmount += Time.deltaTime / 5;
            FillImage.fillAmount = _FillAmount;
        }
        //여기에 오면 다 FillImage가 다 찬거임
        FillObject.SetActive(false);
        BlackBackGround.GetComponent<Image>().DOFade(0f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            LoadingCanvas.gameObject.SetActive(false);
        });
        */
        /*
        LoadingStar.GetComponent<RectTransform>().transform.eulerAngles = LoadingStarInitRotate;

        LoadingStar.GetComponent<Image>().DOFade(0f, 0.5f).SetEase(Ease.Linear);
        LoadingStar.GetComponent<RectTransform>().DOSizeDelta(LoadingStarInitSize, 0.5f).SetEase(Ease.Linear);
        LoadingStar.GetComponent<RectTransform>().transform.DORotate(new Vector3(0, -180f, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear)
        .OnComplete(() =>
        {//다되면(화면을 다 가리면)로딩 시작
            LoadingCanvas.gameObject.SetActive(false);
        });
        */
        yield break;
    }
    
}
