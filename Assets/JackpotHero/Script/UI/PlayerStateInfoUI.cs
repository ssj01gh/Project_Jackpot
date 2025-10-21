using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

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

    public void SetPlayerStateUI(TotalPlayerState TPInfo, PlayerInfo PInfo, int[] BuffList, bool IsRestUpgrade = false)
    {
        //HP, STA는 강화를 해도 업데이트 되지않음 따로 확인할게 필요함
        //SetHpUI
        float[] BeforeHPs = Regex.Matches(PlayerHPText.text, @"\d+").Cast<Match>().Select(m => float.Parse(m.Value)).ToArray();
        if (BeforeHPs.Length >= 2 &&
            (BeforeHPs[0] != TPInfo.CurrentHP || BeforeHPs[1] != TPInfo.MaxHP || PlayerHPSlider.value != PInfo.CurrentHpRatio))
        {
            DOTween.To(() => new Vector2(BeforeHPs[0], BeforeHPs[1]), xy =>
            {
                BeforeHPs[0] = xy.x;
                BeforeHPs[1] = xy.y;
                PlayerHPText.text = BeforeHPs[0].ToString("F0") + " / " + BeforeHPs[1].ToString("F0");
            }, new Vector2(TPInfo.CurrentHP, TPInfo.MaxHP), 0.5f);
            PlayerHPSlider.DOValue(PInfo.CurrentHpRatio, 0.5f);
        }
        //SetSTAUI
        /*
        float BeforeSTA = TPInfo.MaxSTA * PlayerSTASlider.value;
        if(IsRestUpgrade == true || (int)BeforeSTA != (int)TPInfo.CurrentSTA)//다를때만 업데이트
        {
            DOTween.To(() => BeforeSTA, x =>
            {
                BeforeSTA = x;
                PlayerSTAText.text = BeforeSTA.ToString("F0") + " / " + TPInfo.MaxSTA.ToString();
            }, TPInfo.CurrentSTA, 0.5f);
            PlayerSTASlider.DOValue(PInfo.CurrentTirednessRatio, 0.5f);
        }
        */
        float[] BeforeSTAs = Regex.Matches(PlayerSTAText.text, @"\d+").Cast<Match>().Select(m => float.Parse(m.Value)).ToArray();
        if(BeforeSTAs.Length >= 2 &&
            (BeforeSTAs[0] != TPInfo.CurrentSTA || BeforeSTAs[1] != TPInfo.MaxSTA || PlayerSTASlider.value != PInfo.CurrentTirednessRatio))
        {
            DOTween.To(() => new Vector2(BeforeSTAs[0], BeforeSTAs[1]), xy =>
            {
                BeforeSTAs[0] = xy.x;
                BeforeSTAs[1] = xy.y;
                PlayerSTAText.text = BeforeSTAs[0].ToString("F0") + " / " + BeforeSTAs[1].ToString("F0");
            }, new Vector2(TPInfo.CurrentSTA, TPInfo.MaxSTA), 0.5f);
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
                if(TPInfo.TotalSTR > TPInfo.WithOutBuffSTR)
                    PlayerSTR.color = Color.blue;
                else if(TPInfo.TotalSTR < TPInfo.WithOutBuffSTR)
                    PlayerSTR.color = Color.red;
                else
                    PlayerSTR.color = Color.white;
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
                if (TPInfo.TotalDUR > TPInfo.WithOutBuffDUR)
                    PlayerDUR.color = Color.blue;
                else if (TPInfo.TotalDUR < TPInfo.WithOutBuffDUR)
                    PlayerDUR.color = Color.red;
                else
                    PlayerDUR.color = Color.white;
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
                if (TPInfo.TotalRES > TPInfo.WithOutBuffRES)
                    PlayerRES.color = Color.blue;
                else if (TPInfo.TotalRES < TPInfo.WithOutBuffRES)
                    PlayerRES.color = Color.red;
                else
                    PlayerRES.color = Color.white;
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
                if (TPInfo.TotalSPD > TPInfo.WithOutBuffSPD)
                    PlayerSPD.color = Color.blue;
                else if (TPInfo.TotalSPD < TPInfo.WithOutBuffSPD)
                    PlayerSPD.color = Color.red;
                else
                    PlayerSPD.color = Color.white;
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
                if (TPInfo.TotalLUK > TPInfo.WithOutBuffLUK)
                    PlayerLUK.color = Color.blue;
                else if (TPInfo.TotalLUK < TPInfo.WithOutBuffLUK)
                    PlayerLUK.color = Color.red;
                else
                    PlayerLUK.color = Color.white;
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
