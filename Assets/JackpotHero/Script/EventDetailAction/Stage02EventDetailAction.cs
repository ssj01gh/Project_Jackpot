using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage02EventDetailAction
{
    //-----------------------------------------Event2000
    public int Event2000(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 피로도 +300 +-150 -> 이벤트 2005로
        //1. 피로도 -300 +-150 -> 이벤트 2001로
        int RandomSTA = Random.Range(-150, 151);
        switch(ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 2005;
            case 1:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                return 2001;
        }    

        return 2000;
    }
    public int Event2001(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 피로도 +300 +-150 -> 이벤트 2005로
        //1. 피로도 -300 +-150 -> 50% 이벤트 2006로 50% 2002로
        int RandomSTA = Random.Range(-150, 151);
        int RandomPath = Random.Range(0, 2);//0~1
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 2005;
            case 1:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                if (RandomPath == 0)//0걸리면 2002
                {
                    return 2002;
                }
                else//아니면 2006
                {
                    return 2006;
                }
        }
        return 2001;
    }
    public int Event2002(int ButtonType, PlayerManager PlayerMgr)
    {
        //0. 피로도 +300 +-150 -> 이벤트 2005로
        //1. 피로도 -300 +-150 -> 75% 이벤트 2006로 25% 2003로
        int RandomSTA = Random.Range(-150, 151);
        int RandomPath = Random.Range(0, 4);//0~3
        switch (ButtonType)
        {
            case 0:
                PlayerMgr.GetPlayerInfo().PlayerRegenSTA(300 + RandomSTA);
                SoundManager.Instance.PlaySFX("Buff_Healing");
                return 2005;
            case 1:
                PlayerMgr.GetPlayerInfo().PlayerSpendSTA(300 + RandomSTA);
                if (RandomPath == 0)
                {
                    
                }
                else
                {

                }
                break;
        }
        return 2002;
    }
    //-----------------------------------------Event2010
    //-----------------------------------------Event2020
    //-----------------------------------------Event2030
    //-----------------------------------------Event2040
    //-----------------------------------------Event2050
}
