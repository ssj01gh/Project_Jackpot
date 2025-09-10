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
            if(!EffectStorage.ContainsKey(EI.EffectID))//�ȴ�� ������
            {
                EffectStorage.Add(EI.EffectID, EI.EffectSprite);
            }
        }
    }

    public void ActiveEffect(string Effect_ID, Vector2 EffectPos)
    {
        Debug.Log(Effect_ID);
        //���⼭ ����Ʈ ����� �����ؼ� �ϸ� �ɵ�? ����Ʈ + ���� �Ѽ�Ʈ�ΰŷ�
        switch(Effect_ID)
        {
            //Ÿ��
            case "BattleEffect_Hit_Sward":
            case "BattleEffect_Hit_Mon_Sward":
                SoundManager.Instance.PlaySFX("Hit_Sward");
                break;
            //�Ҹ���
            case "BattleEffect_Buff_Burn":
                SoundManager.Instance.PlaySFX("Buff_Burn");
                break;
            case "BattleEffect_Buff_Poison":
            case "BattleEffect_Buff_CurseOfDeath":
            case "BattleEffect_Buff_Weakness":
                SoundManager.Instance.PlaySFX("Buff_Consume");
                break;
            //ȸ����
            case "BattleEffect_Buff_RegenHP":
            case "BattleEffect_Buff_RegenSTA":
            case "BattleEffect_Buff_UnDead":
            case "BattleEffect_Buff_RegenArmor":
                SoundManager.Instance.PlaySFX("Buff_Healing");
                break;
            //������
            case "BattleEffect_Buff_Frost":
            case "BattleEffect_Buff_Fear":
                SoundManager.Instance.PlaySFX("Buff_Forcing");
                break;
        }

        if(EffectStorage.ContainsKey(Effect_ID))
        {
            for(int i = 0; i < EffectObjStorage.Count; i++)
            {
                if (EffectObjStorage[i].activeSelf == false)//������ ������� ����
                {
                    EffectObjStorage[i].GetComponent<EffectSpriteScript>()
                        .ActiveSpriteObject(EffectStorage[Effect_ID], EffectPos);
                    break;
                }
            }
        }
    }
}
