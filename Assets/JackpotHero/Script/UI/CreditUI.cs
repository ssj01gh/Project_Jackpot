using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class CreditUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject CreditBox;
    public float StartPos;
    public float EndPos;
    public GameObject SkipText;
    public TextMeshProUGUI StoryText;

    protected bool StoryCanSkip = false;
    protected bool CreditCanSkip = false;

    private Coroutine EndingTextCo;
    void Start()
    {
        //gameObject.SetActive(false);
        //StartAnimateCredit();
    }

    // Update is called once per frame
    void Update()
    {
        if (StoryCanSkip == true)
        {
            if (Input.anyKeyDown)
            {
                StoryCanSkip = false;
                SkipText.SetActive(false);
                if (DOTween.IsTweening(gameObject.GetComponent<CanvasGroup>()))
                {
                    gameObject.GetComponent<CanvasGroup>().DOKill();
                    gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                }
                TutorialTextSkip();
            }
        }

        if (CreditCanSkip == true)
        {
            if (Input.anyKeyDown)
            {
                CreditCanSkip = false;
                SkipText.SetActive(false);
                if (DOTween.IsTweening(gameObject.GetComponent<CanvasGroup>()))
                {
                    gameObject.GetComponent<CanvasGroup>().DOKill();
                    gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                }
                if(DOTween.IsTweening(CreditBox.GetComponent<RectTransform>()))
                {
                    Vector2 EndVec = new Vector2(0f, EndPos);
                    CreditBox.GetComponent<RectTransform>().DOKill();
                    CreditBox.GetComponent<RectTransform>().anchoredPosition = EndVec;
                }
                if(DOTween.IsTweening(CreditBox.GetComponent<CanvasGroup>()))
                {
                    CreditBox.GetComponent<CanvasGroup>().DOKill();
                    CreditBox.GetComponent<CanvasGroup>().alpha = 1f;
                }

                DOVirtual.DelayedCall(2f, () =>
                {
                    //2초뒤 자동으로 타이틀 로딩
                    LoadingScene.Instance.LoadAnotherScene("TitleScene");
                });
            }
        }

        
    }
    //조건에 따라 맞는 이미지를 출력한다?-> 있으면 좋긴 할텐데,......
    public void StartEnding()
    {
        StoryCanSkip = false;

        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);

        if (SkipText.activeSelf == true)
            SkipText.SetActive(false);

        if (StoryText.gameObject.activeSelf == false)
            StoryText.gameObject.SetActive(true);

        if (CreditBox.activeSelf == true)
            CreditBox.SetActive(false);

        StoryText.text = "";
        string EndingKey = GetEndingKey();

        gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        gameObject.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).OnComplete(() =>
        {
            StoryCanSkip = true;
            SkipText.SetActive(true);
            StartCoroutine(LoadStory(EndingKey));
        });
    }

    private string GetEndingKey()
    {
        //도플갱어랑 거인은 어떤선택을 했는지는 기록을 안했는디..... -> 해놨음
        //if(JsonReadWriteManager.Instance.LkEv_Info)
        if (JsonReadWriteManager.Instance.LkEv_Info.TalkingMonster == true && JsonReadWriteManager.Instance.LkEv_Info.TalkingDirtGolem == true &&
            JsonReadWriteManager.Instance.LkEv_Info.LetKnowGiant == true && JsonReadWriteManager.Instance.LkEv_Info.LetKnowDopple == true)
        {
            return "EST_Ending02";
        }

        if (JsonReadWriteManager.Instance.LkEv_Info.TotoBlessedSword == true)
        {//엔딩 3
            return "EST_Ending03";
        }

        return "EST_Ending01";
    }

    private IEnumerator LoadStory(string EndingKey)
    {
        yield return LocalizationSettings.InitializationOperation;

        var EndingTable = LocalizationSettings.StringDatabase.GetTable("EndingStoryTable");
        StoryText.text = EndingTable.GetEntry(EndingKey).GetLocalizedString();

        PlayEndingText();//글자 토도독, 버튼 누르면 스킵되게
    }

    private void PlayEndingText()
    {
        if (EndingTextCo != null)
            StopCoroutine(EndingTextCo);

        //TutorialText.text = CurrentTutorialInfo.TutorialText[CurrentTutorialIndex];
        StoryText.maxVisibleCharacters = 0;

        EndingTextCo = StartCoroutine(EndingTextCoroutine());
    }

    private IEnumerator EndingTextCoroutine()
    {
        StoryText.ForceMeshUpdate();
        int TotalTextCount = StoryText.textInfo.characterCount;//<-아마 여기서 접근하면서 터지는데.,....

        for (int i = 0; i <= TotalTextCount; i++)
        {
            StoryText.maxVisibleCharacters = i;
            if (i > 0)
            {
                TMP_CharacterInfo CharInfo = StoryText.textInfo.characterInfo[i - 1];
                if (CharInfo.character != ' ')
                {
                    SoundManager.Instance.PlayUISFX("TextTyping");
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
        EndingTextCo = null;
        //여기까지 오면 다온거
        StartCredit();
    }

    private void TutorialTextSkip()
    {
        if (EndingTextCo != null)
            StopCoroutine(EndingTextCo);

        EndingTextCo = null;
        StoryText.maxVisibleCharacters = int.MaxValue;
        StartCredit();
    }

    private void StartCredit()
    {
        if (DOTween.IsTweening(CreditBox.GetComponent<RectTransform>()))
            return;

        if (DOTween.IsTweening(CreditBox.GetComponent<CanvasGroup>()))
            return;

        DOVirtual.DelayedCall(2f, () =>
        {
            //2초뒤 크래딧 올라옴
            //LoadingScene.Instance.LoadAnotherScene("TitleScene");
            StoryText.text = "";

            CreditCanSkip = false;
            SkipText.SetActive(false);

            if (CreditBox.activeSelf == false)
                CreditBox.SetActive(true);

            Vector2 StartVec = new Vector2(0f, StartPos);
            Vector2 EndVec = new Vector2(0f, EndPos);
            CreditBox.GetComponent<RectTransform>().anchoredPosition = StartVec;
            CreditBox.GetComponent<CanvasGroup>().alpha = 0f;
            CreditBox.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).OnComplete(() =>
            {
                CreditCanSkip = true;
                SkipText.SetActive(true);
                CreditBox.GetComponent<RectTransform>().DOAnchorPos(EndVec, 20f).SetEase(Ease.Linear).SetDelay(2f).OnComplete(() =>
                {
                    CreditCanSkip = false;
                    SkipText.SetActive(false);
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        //2초뒤 자동으로 타이틀 로딩
                        LoadingScene.Instance.LoadAnotherScene("TitleScene");
                    });
                });
            });
        });
    }
}
