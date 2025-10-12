using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public bool IsInTitle;
    public Button ToTitleButton;

    [Header("ScreenOptionUI")]
    public TMP_Dropdown ScreenResolutionUI;
    public Toggle WindowScreenToggle;
    public Toggle FullScreenToggle;
    [Header("SoundOptionUI")]
    public Slider MasterSlider;
    public Slider BGMSlider;
    public Slider SFXSlider;
    public Slider UISFXSlider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OptionUIActive()
    {
        if (IsInTitle == true)
        {
            ToTitleButton.gameObject.SetActive(false);
        }
        else
        {
            ToTitleButton.gameObject.SetActive(true);
        }

        SoundManager.Instance.PlayUISFX("UI_Button");
        SetScreenOptionUI();
        SetSoundOptionUI();

        gameObject.SetActive(true);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1080);
        gameObject.GetComponent<RectTransform>().DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack);
        //gameObject.GetComponent<Animator>().SetInteger("OptionState", 1);
    }

    protected void SetScreenOptionUI()
    {
        ScreenResolutionUI.value = ScreenManager.Instance.GetCurrentScreenResolutionIndex();

        if (Screen.fullScreen == true)
            JsonReadWriteManager.Instance.O_Info.IsFullScreen = true;
        else
            JsonReadWriteManager.Instance.O_Info.IsFullScreen = false;

        if (JsonReadWriteManager.Instance.O_Info.IsFullScreen == true)
            FullScreenToggle.isOn = true;
        else
            WindowScreenToggle.isOn = true;
    }

    protected void SetSoundOptionUI()
    {
        MasterSlider.value = JsonReadWriteManager.Instance.O_Info.MasterVolume;
        BGMSlider.value = JsonReadWriteManager.Instance.O_Info.BGMVolume;
        SFXSlider.value = JsonReadWriteManager.Instance.O_Info.SFXVolume;
        UISFXSlider.value = JsonReadWriteManager.Instance.O_Info.UISFXVolume;
    }

    public void ChangeResolutionEvent()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        ScreenManager.Instance.SetScreenResolution(ScreenResolutionUI.value);
    }

    public void ChangeScreenModeEvent()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        ScreenManager.Instance.SetScreenMod(FullScreenToggle.isOn);
    }

    public void ChangeSoundValueEvent(string SliderType)
    {
        switch(SliderType)
        {
            case "Master":
                JsonReadWriteManager.Instance.O_Info.MasterVolume = MasterSlider.value;
                SoundManager.Instance.SetSoundValue("MasterValue", MasterSlider.value);
                break;
            case "BGM":
                JsonReadWriteManager.Instance.O_Info.BGMVolume = BGMSlider.value;
                SoundManager.Instance.SetSoundValue("BGMValue", BGMSlider.value);
                break;
            case "SFX":
                JsonReadWriteManager.Instance.O_Info.SFXVolume = SFXSlider.value;
                SoundManager.Instance.SetSoundValue("SFXValue", SFXSlider.value);
                break;
            case "UISFX":
                JsonReadWriteManager.Instance.O_Info.UISFXVolume = UISFXSlider.value;
                SoundManager.Instance.SetSoundValue("UISFXValue", UISFXSlider.value);
                break;
        }
        /*
        SoundManager.Instance.SetSoundValue(SliderType)
        SetSoundValue
        */
    }
    public void OptionInActive()
    {
        if (gameObject.activeSelf == false)
            return;

        SoundManager.Instance.PlayUISFX("UI_Button");
        gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        gameObject.GetComponent<RectTransform>().DOAnchorPosY(1080, 0.3f).OnComplete(() => { gameObject.SetActive(false); });
        //gameObject.GetComponent<Animator>().SetInteger("OptionState", 2);
    }

    public void PressToTitleButton()
    {
        SoundManager.Instance.PlayUISFX("UI_Button");
        LoadingScene.Instance.LoadAnotherScene("TitleScene");
    }
}
