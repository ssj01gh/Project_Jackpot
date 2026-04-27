using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Unity.VisualScripting;

public class SteamAchievementManager : MonoSingletonDontDestroy<SteamAchievementManager>
{
    // Start is called before the first frame update
    //private bool FirstTime = false;
    //private float AchieveTime = 0f;
    private bool IsSteamAPIConected = false;
    void Start()
    {
        if (!SteamAPI.Init())
        {
            //Debug.LogError("Steam 초기화 실패");
            IsSteamAPIConected = false;
        }
        else
        {
            //Debug.Log("Steam 연결 성공");
            IsSteamAPIConected = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsSteamAPIConected == true)
            SteamAPI.RunCallbacks();
    }

    public void SetSteamAchievement(string APICode, bool IsStoreStats = true)
    {
        if (IsSteamAPIConected == false)
            return;

        bool achieved;
        SteamUserStats.GetAchievement(APICode, out achieved);

        if (achieved == false)
        {
            SteamUserStats.SetAchievement(APICode);
            if(IsStoreStats == true)
                SetSteamACHStoreStats();
        }
    }

    public void SetSteamACHStoreStats()
    {
        if (IsSteamAPIConected == false)
            return;

        SteamUserStats.StoreStats();
    }

    void OnApplicationQuit()
    {
        if(IsSteamAPIConected == true)
            SteamAPI.Shutdown();
    }
}
