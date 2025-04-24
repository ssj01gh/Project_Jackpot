using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    public OptionUI _OptionUI;
    public EarlyStrengthenUI _EarlyUI;
    public Button ContinueButton;
    // Start is called before the first frame update
    void Start()
    {
        SetContinueButton();
        _OptionUI.gameObject.SetActive(false);
        //_EarlyUI.gameObject.SetActive(false);
        SoundManager.Instance.PlayBGM("TitleBGM");
    }

    // Update is called once per frame
    void Update()
    {
        
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
