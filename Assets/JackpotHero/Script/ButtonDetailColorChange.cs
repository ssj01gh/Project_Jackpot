using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDetailColorChange : MonoBehaviour
{
    // Start is called before the first frame update
    public Image[] DetailUIs;

    public float[] RValue;
    public float[] GValue;
    public float[] BValue;

    Color[] Colors = new Color[2];
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeColor()
    {
        for (int i = 0; i< Colors.Length; i++)
        {
            Colors[i].r = RValue[i] / 255f;
            Colors[i].g = GValue[i] / 255f;
            Colors[i].b = BValue[i] / 255f;
            Colors[i].a = 1f;
        }

        for(int i = 0; i < DetailUIs.Length; i++)
        {
            if (DetailUIs[i].color == Colors[1])
            {
                DetailUIs[i].color = Colors[0];
            }
            else
            {
                DetailUIs[i].color = Colors[1];
            }
        }
    }
}
