using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SoundInfo
{
    public string SoundName;
    public AudioClip _Sound;
}

public class SoundManager : MonoSingletonDontDestroy<SoundManager>
{
    [Header("AudioClip")]
    public SoundInfo[] BGM;
    public SoundInfo[] SFX;
    public SoundInfo[] UISFX;

    [Header("AudioSource")]
    public AudioSource BGMPlayer;// = new AudioSource();
    public AudioSource SFXPlayer;// = new AudioSource();
    public AudioSource UISFXPlayer;// = new AudioSource();

    public int SFXPlayerAmount;
    public int UISFXPlayerAmount;

    [Header("Mixer")]
    public AudioMixer Mixer;

    protected Dictionary<string, AudioClip> BGMStorage = new Dictionary<string, AudioClip>();
    protected Dictionary<string, AudioClip> SFXStorage = new Dictionary<string, AudioClip>();
    protected Dictionary<string, AudioClip> UISFXStorage = new Dictionary<string, AudioClip>();

    protected AudioSource BGMPlayerStorage = new AudioSource();
    protected List<AudioSource> SFXPlayerStorage = new List<AudioSource>();
    protected List<AudioSource> UISFXPlayerStorge = new List<AudioSource>();
    // Start is called before the first frame update
    void Start()
    {
        InitSoundPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void InitSoundPlayer()
    {
        AudioSource AudioObj = GameObject.Instantiate(BGMPlayer);
        AudioObj.gameObject.transform.SetParent(gameObject.transform);
        BGMPlayerStorage = AudioObj;

        for(int i = 0; i < SFXPlayerAmount; i++)
        {
            AudioObj = GameObject.Instantiate(SFXPlayer);
            AudioObj.gameObject.transform.SetParent(gameObject.transform);
            SFXPlayerStorage.Add(AudioObj);
        }

        for(int i = 0; i < UISFXPlayerAmount; i++)
        {
            AudioObj = GameObject.Instantiate(UISFXPlayer);
            AudioObj.gameObject.transform.SetParent(gameObject.transform);
            UISFXPlayerStorge.Add(AudioObj);
        }
        //--------------------------------------------

        foreach(SoundInfo SI in BGM)
        {
            if (BGMStorage.ContainsKey(SI.SoundName) == false)
                BGMStorage.Add(SI.SoundName, SI._Sound);
        }

        foreach(SoundInfo SI in SFX)
        {
            if (SFXStorage.ContainsKey(SI.SoundName) == false)
                SFXStorage.Add(SI.SoundName, SI._Sound);
        }

        foreach(SoundInfo SI in UISFX)
        {
            if (UISFXStorage.ContainsKey(SI.SoundName) == false)
                UISFXStorage.Add(SI.SoundName, SI._Sound);
        }
        InitSoundValue();
        //PlayBGM("TitleBGM");
        //타이틀 <-> 게임씬 구분
        //게임씬 <-> 보스전투, 일반전투, 일반 상태, 휴식 구분
    }

    protected void InitSoundValue()
    {
        OptionInfo SoundOptionInfo = JsonReadWriteManager.Instance.O_Info;
        SetSoundValue("MasterValue", SoundOptionInfo.MasterVolume);
        SetSoundValue("BGMValue", SoundOptionInfo.BGMVolume);
        SetSoundValue("SFXValue", SoundOptionInfo.SFXVolume);
        SetSoundValue("UISFXValue", SoundOptionInfo.UISFXVolume);
    }

    public void PlayBGM(string BGMName)
    {
        if (BGMStorage.ContainsKey(BGMName) == true &&
            BGMPlayerStorage.clip != BGMStorage[BGMName])
        {
            BGMPlayerStorage.clip = BGMStorage[BGMName];
            BGMPlayerStorage.Play();
        }
        else
        {
            //겹쳤을때도 뜸
            Debug.Log("There is No " + BGMName);
        }
    }
    public AudioSource PlaySFX(string SFXName, float Pitch = 1)
    {
        if(SFXStorage.ContainsKey(SFXName) == true)
        {
            foreach(AudioSource AS in SFXPlayerStorage)
            {
                if(AS.isPlaying == false)
                {
                    AS.clip = SFXStorage[SFXName];
                    AS.pitch = Pitch;
                    AS.Play();
                    return AS;
                }
            }
        }
        return SFXPlayerStorage[0];
    }

    public void StopSFX(AudioSource StopSource)
    {
        foreach(AudioSource AS in SFXPlayerStorage)
        {
            if(AS == StopSource)
            {
                AS.Stop();
                break;
            }
        }
    }

    public void PlayUISFX(string UISFXName)
    {
        if (UISFXStorage.ContainsKey(UISFXName) == true)
        {
            foreach (AudioSource AS in UISFXPlayerStorge)
            {
                if (AS.isPlaying == false)
                {
                    AS.clip = UISFXStorage[UISFXName];
                    AS.Play();
                    break;
                }
            }
        }
    }

    public void SetSoundValue(string GroupName, float Value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(Value, 0.001f, 1f)) * 20;
        Mixer.SetFloat(GroupName, dB);
        
        /*
        float dB = Mathf.Lerp(-80, 20f, Value);
        Mixer.SetFloat(GroupName, dB);
        */
        /*
        float SliderValue = Mathf.Clamp(Value, 0.001f, 1f); // 0 방지
        float Log = Mathf.Log10(SliderValue); // -3 ~ 0
        float InverseLog = Mathf.InverseLerp(-3f, 0f, Log); // 0~1 정규화
        float dB =  Mathf.Lerp(-80f, 20f, InverseLog); // 최종 dB
        Mixer.SetFloat(GroupName, dB);
        */
    }
}
