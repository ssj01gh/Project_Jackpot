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
        PlayerHPText.text = TPInfo.CurrentHP.ToString("F1") + " / " + TPInfo.MaxHP.ToString();
        PlayerHPSlider.value = PInfo.CurrentHpRatio;
        //SetSTAUI
        PlayerSTAText.text = TPInfo.CurrentSTA.ToString("F1") + " / " + TPInfo.MaxSTA.ToString();
        PlayerSTASlider.value = PInfo.CurrentTirednessRatio;
        //SetSTRUI
        PlayerSTR.text = TPInfo.TotalSTR.ToString();
        //SetDURUI
        PlayerDUR.text = TPInfo.TotalDUR.ToString();
        //SetRESUI
        PlayerRES.text = TPInfo.TotalRES.ToString();
        //SetSPDUI
        PlayerSPD.text = TPInfo.TotalSPD.ToString();
        //SetLUKUI
        PlayerLUK.text = TPInfo.TotalLUK.ToString();
        //SetEXPUI
        PlayerEXP.text = PInfo.Experience.ToString();
    }
}
