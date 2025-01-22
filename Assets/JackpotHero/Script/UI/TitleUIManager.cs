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
        _EarlyUI.gameObject.SetActive(false);
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
        Debug.Log("Aaaaaaa");
    }
    public void StartButtonClick()
    {
        _EarlyUI.EarlyStrengthenActive();
    }
    public void OptionButtonClick()
    {
        _OptionUI.OptionUIActive();
    }
    public void ExitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
