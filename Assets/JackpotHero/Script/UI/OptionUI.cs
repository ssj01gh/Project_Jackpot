using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public bool IsInTitle;
    public Button ToTitleButton;
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
        if(IsInTitle == true)
        {
            ToTitleButton.gameObject.SetActive(false);
        }
        else
        {
            ToTitleButton.gameObject.SetActive(true);
        }

        gameObject.SetActive(true);
        gameObject.GetComponent<Animator>().SetInteger("OptionState", 1);
    }
    public void OptionInActive()
    {
        gameObject.GetComponent<Animator>().SetInteger("OptionState", 2);
    }

    public void OptionUISetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
