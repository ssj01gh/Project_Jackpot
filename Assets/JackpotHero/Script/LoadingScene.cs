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
    public Image FillImage;
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

        LoadingCanvas.gameObject.SetActive(true);
        FillImage.fillAmount = 0f;
        LoadingCanvas.GetComponent<Animator>().SetInteger("LoadingCanvasState", 1);
        //LoadingBackGround.SetActive(true);
        //LoadingAnimator.SetInteger("LoadingCanvasState", 0);
        StartCoroutine(DisplayProgressBar(SceneName));
    }

    IEnumerator DisplayProgressBar(string SceneName)
    {
        Animator LoadingAnimator = LoadingCanvas.GetComponent<Animator>();
        while(true)
        {
            yield return null;
            if (LoadingAnimator.GetInteger("LoadingCanvasState") == 1 &&
                LoadingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                break;
            }
        }

        AsyncOperation Operation = SceneManager.LoadSceneAsync(SceneName);
        Operation.allowSceneActivation = false;

        while (Operation.progress < 0.9f)
        {
            yield return null;
            FillImage.fillAmount = Operation.progress;
        }

        Operation.allowSceneActivation = true;

        while (true)
        {
            yield return null;
            if (SceneManager.GetActiveScene().name == SceneName)
                break;
        }

        while (FillImage.fillAmount < 1)
        {
            yield return null;
            float _FillAmount = FillImage.fillAmount;
            _FillAmount += Time.deltaTime / 5;
            FillImage.fillAmount = _FillAmount;
        }

        LoadingAnimator.SetInteger("LoadingCanvasState", 2);
        while (true)
        {
            yield return null;
            if (LoadingAnimator.GetInteger("LoadingCanvasState") == 2 &&
                LoadingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                break;
            }
        }
        LoadingCanvas.gameObject.SetActive(false);
        yield break;
        /*
        while (true)
        {
            if (LoadingAnimator.GetInteger("LoadingCanvasState") == 0 &&
            LoadingAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                Operation.allowSceneActivation = true;
                if (Operation.isDone)
                {
                    LoadingAnimator.SetInteger("LoadingCanvasState", 1);
                    LoadingBackGround.SetActive(false);
                    SoundManager.Instance.PlayUISFX("LoadingDoorOpen", 1f);
                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (LoadingAnimator.gameObject.activeSelf == false)
            {
                break;
            }
            yield return null;
        }
        if (SceneName != "TitleScene")
            SoundManager.Instance.PlayBGM(SceneName);
        */

    }
    
}
