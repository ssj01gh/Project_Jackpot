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

    public BuffInfoSO GetBuffInfo(int BuffType)
    {
        return BuffInfos[BuffType];
    }
}
