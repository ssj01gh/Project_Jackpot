using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffInfoManager : MonoSingleton<BuffInfoManager>
{
    [SerializeField]
    protected BuffInfoSO[] BuffInfos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public BuffSOInfo GetBuffInfo(int BuffType)
    {
        BuffSOInfo AssembleBuffInfo = new BuffSOInfo();
        AssembleBuffInfo.BuffImage = BuffInfos[BuffType].BuffImage;
        AssembleBuffInfo.BuffName = "";
        AssembleBuffInfo.BuffDetail = "";
        if (JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.English)
        {
            AssembleBuffInfo.BuffName = BuffInfos[BuffType].BuffNameEN;
            AssembleBuffInfo.BuffDetail = BuffInfos[BuffType].BuffDetailEN;
        }
        else if(JsonReadWriteManager.Instance.O_Info.CurrentLanguage == (int)ELanguageNum.Japanese)
        {
            AssembleBuffInfo.BuffName = BuffInfos[BuffType].BuffNameJA;
            AssembleBuffInfo.BuffDetail = BuffInfos[BuffType].BuffDetailJA;
        }
        else
        {
            AssembleBuffInfo.BuffName = BuffInfos[BuffType].BuffName;
            AssembleBuffInfo.BuffDetail = BuffInfos[BuffType].BuffDetail;
        }

         return AssembleBuffInfo;
        //return BuffInfos[BuffType];
    }
}
