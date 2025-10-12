using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoSingleton<ScreenManager>
{
    
    protected Resolution[] Resolution16_9 = new Resolution[]
    {
        new Resolution { width = 1280, height = 720},
        new Resolution { width = 1600, height = 900},
        new Resolution {width = 1920, height = 1080},
        new Resolution {width = 2560, height = 1440}
    };
    
    // Start is called before the first frame update
    void Start()
    {
        InitScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitScreen()
    {
        float SavedScreenValue = JsonReadWriteManager.Instance.O_Info.ScreenResolutionWidth;
        bool IsFullScreen = JsonReadWriteManager.Instance.O_Info.IsFullScreen;
        for(int i = 0; i < Resolution16_9.Length; i++)
        {
            if (Resolution16_9[i].width == SavedScreenValue)
            {
                Screen.SetResolution(Resolution16_9[i].width, Resolution16_9[i].height, IsFullScreen);
                break;
            }
        }
    }

    public int GetCurrentScreenResolutionIndex()
    {
        for(int i = 0; i < Resolution16_9.Length; i++)
        {
            if(JsonReadWriteManager.Instance.O_Info.ScreenResolutionWidth == Resolution16_9[i].width)
                return i;
        }
        return 0;
    }

    public void SetScreenResolution(int Value)
    {
        if(Value < Resolution16_9.Length)
        {
            JsonReadWriteManager.Instance.O_Info.ScreenResolutionWidth = Resolution16_9[Value].width;
            Screen.SetResolution(Resolution16_9[Value].width, Resolution16_9[Value].height, JsonReadWriteManager.Instance.O_Info.IsFullScreen);
        }
    }

    public void SetScreenMod(bool IsFullScreen)
    {
        JsonReadWriteManager.Instance.O_Info.IsFullScreen = IsFullScreen;
        if (IsFullScreen == true)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
