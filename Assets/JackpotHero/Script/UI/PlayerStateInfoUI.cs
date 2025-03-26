using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateInfoUI : MonoBehaviour
{
    [Header("PlayerHP")]
    public TextMeshProUGUI PlayerHPText;
    public Slider PlayerHPSlider;
    [Header("PlayerSTA")]
    public TextMeshProUGUI PlayerSTAText;
    public Slider PlayerSTASlider;
    [Header("PlayerState")]
    public TextMeshProUGUI PlayerSTR;
    public TextMeshProUGUI PlayerDUR;
    public TextMeshProUGUI PlayerRES;
    public TextMeshProUGUI PlayerSPD;
    public TextMeshProUGUI PlayerLUK;
    public TextMeshProUGUI PlayerEXP;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerStateUI(TotalPlayerState TPInfo, PlayerInfo PInfo)
    {
        //SetHpUI
        float BeforeHP = TPInfo.MaxHP * PlayerHPSlider.value;
        if((int)BeforeHP != (int)TPInfo.CurrentHP)//다를때만 업데이트
        {
            DOTween.To(() => BeforeHP, x =>
            {
                BeforeHP = x;
                PlayerHPText.text = BeforeHP.ToString("F0") + " / " + TPInfo.MaxHP.ToString();
            }, TPInfo.CurrentHP, 0.5f);
            PlayerHPSlider.DOValue(PInfo.CurrentHpRatio, 0.5f);
        }
        //SetSTAUI
        float BeforeSTA = TPInfo.MaxSTA * PlayerSTASlider.value;
        if((int)BeforeSTA != (int)TPInfo.CurrentSTA)//다를때만 업데이트
        {
            DOTween.To(() => BeforeSTA, x =>
            {
                BeforeSTA = x;
                PlayerSTAText.text = BeforeSTA.ToString("F0") + " / " + TPInfo.MaxSTA.ToString();
            }, TPInfo.CurrentSTA, 0.5f);
            PlayerSTASlider.DOValue(PInfo.CurrentTirednessRatio, 0.5f);
        }
        //SetSTRUI
        float BeforeSTR = float.Parse(PlayerSTR.text);
        if(BeforeSTR != TPInfo.TotalSTR)//다를때만 업데이트
        {
            DOTween.To(() => BeforeSTR, x =>
            {
                BeforeSTR = x;
                PlayerSTR.text = BeforeSTR.ToString("F0");
            }, TPInfo.TotalSTR, 0.5f);
            //PlayerSTR.text = TPInfo.TotalSTR.ToString();
        }
        //SetDURUI
        float BeforeDUR = float.Parse(PlayerDUR.text);
        if(BeforeDUR != TPInfo.TotalDUR)
        {
            DOTween.To(() => BeforeDUR, x =>
            {
                BeforeDUR = x;
                PlayerDUR.text = BeforeDUR.ToString("F0");
            }, TPInfo.TotalDUR, 0.5f);
            //PlayerDUR.text = TPInfo.TotalDUR.ToString();
        }
        //SetRESUI
        float BeforeRES = float.Parse(PlayerRES.text);
        if(BeforeRES != TPInfo.TotalDUR)
        {
            DOTween.To(() => BeforeRES, x =>
            {
                BeforeRES = x;
                PlayerRES.text = BeforeRES.ToString("F0");
            }, TPInfo.TotalRES, 0.5f);
            //PlayerRES.text = TPInfo.TotalRES.ToString();
        }
        //SetSPDUI
        float BeforeSPD = float.Parse(PlayerSPD.text);
        if(BeforeSPD != TPInfo.TotalSPD)
        {
            DOTween.To(() => BeforeSPD, x =>
            {
                BeforeSPD = x;
                PlayerSPD.text = BeforeSPD.ToString("F0");
            }, TPInfo.TotalSPD, 0.5f);
            //PlayerSPD.text = TPInfo.TotalSPD.ToString();
        }
        //SetLUKUI
        float BeforeLUK = float.Parse(PlayerLUK.text);
        if(BeforeLUK != TPInfo.TotalLUK)
        {
            DOTween.To(() => BeforeLUK, x =>
            {
                BeforeLUK = x;
                PlayerLUK.text = BeforeLUK.ToString("F0");
            }, TPInfo.TotalLUK, 0.5f);
            //PlayerLUK.text = TPInfo.TotalLUK.ToString();
        }
        //SetEXPUI
        float BeforeEXP = float.Parse(PlayerEXP.text);
        DOTween.To(() => BeforeEXP, x =>
        {
            BeforeEXP = x;
            PlayerEXP.text = BeforeEXP.ToString("F0");
            //PlayerEXP.text = PInfo.Experience.ToString();
        }, PInfo.Experience, 0.5f);
    }
}
