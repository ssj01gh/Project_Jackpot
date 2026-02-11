using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoSingleton<EffectManager>
{
    public EffectSO[] Effects;
    public GameObject EffectObjectPrefab;
    public int EffectAmount;
    // Start is called before the first frame update
    protected List<GameObject> EffectObjStorage = new List<GameObject>();
    protected Dictionary<string, Sprite> EffectStorage = new Dictionary<string, Sprite>();
    void Start()
    {
        InitEffectObj();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitEffectObj()
    {
        for(int i = 0; i < EffectAmount; i++)
        {
            GameObject obj = GameObject.Instantiate(EffectObjectPrefab);
            obj.transform.SetParent(gameObject.transform);
            obj.SetActive(false);
            EffectObjStorage.Add(obj);
        }

        foreach(EffectSO EI in Effects)
        {
            if(!EffectStorage.ContainsKey(EI.EffectID))//안담겨 있을때
            {
                EffectStorage.Add(EI.EffectID, EI.EffectSprite);
            }
        }
    }

    public void ActiveEffect(string Effect_ID, Vector2 EffectPos)
    {
        if(EffectStorage.ContainsKey(Effect_ID))
        {
            for(int i = 0; i < EffectObjStorage.Count; i++)
            {
                if (EffectObjStorage[i].activeSelf == false)//거짓인 얘들한테 전달
                {
                    EffectObjStorage[i].GetComponent<EffectSpriteScript>()
                        .ActiveSpriteObject(EffectStorage[Effect_ID], EffectPos);
                    break;
                }
            }
        }
    }

    public void ActiveNumberEffect()
    {

    }
}
