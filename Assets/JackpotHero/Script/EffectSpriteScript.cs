using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpriteScript : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer EffectObjectSprite;

    protected Vector2 MaxSize = new Vector2(5f, 5f);
    protected Color InitColor = new Color(1, 1, 1, 0);
    protected Color MaxColor = new Color(1, 1, 1, 0.5f);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveSpriteObject(Sprite _Sprite, Vector2 ActivePos)
    {
        gameObject.transform.position = ActivePos;
        gameObject.transform.localScale = Vector2.zero;
        EffectObjectSprite.color = InitColor;
        EffectObjectSprite.sprite = _Sprite;
        gameObject.SetActive(true);
        gameObject.transform.DOScale(MaxSize, 0.3f);
        EffectObjectSprite.DOColor(MaxColor, 0.3f).OnComplete(() => { InActiveSpriteObject(); });
    }

    protected void InActiveSpriteObject()
    {
        EffectObjectSprite.DOColor(InitColor, 0.3f).OnComplete(() => { gameObject.SetActive(false); });
    }
}
