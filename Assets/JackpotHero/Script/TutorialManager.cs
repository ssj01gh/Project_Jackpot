using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private PlayerManager PlayerMgr;
    [SerializeField]
    private Image TutorialImage;
    [SerializeField]
    private TextMeshProUGUI TutorialText;
    [SerializeField]
    private Button TutorialSkipButton;

    private TutorialSetSO CurrentTutorialInfo;
    private AsyncOperationHandle<TutorialSetSO> _Handle;
    private int CurrentTutorialIndex = 0;

    private Coroutine TutorialTextCo;
    private float TextDelay = 0.06f;
    private List<string> ForLinkedTutorial = new List<string>();
    private bool IsAfterTutorial = false;
    public void SetLinkedTutorialNStartTutorial(string TutorialKey)//이걸 타이밍 맞게 부르면.....
    {
        SaveLinkedTutorialList(TutorialKey);
        StartTutorial(TutorialKey);
        IsAfterTutorial = false;
    }
    public async void StartTutorial(string TutorialKey)//이걸로 튜토리얼이 시작됨 외부 class에서 부르는거 위에 함수로 바꾸기
    {
        //이전 꺼 정리
        HideTutorialPage();

        if(ForLinkedTutorial.Count >= 1)
        {//뭐하나 라도 들어가 있다면
            TutorialKey = ForLinkedTutorial[0];//주고 
            ForLinkedTutorial.RemoveAt(0);//없애기(1번에 있는 놈을 0번으로 땡김 Or Count를 0으로 만듬)
        }
        if (ForLinkedTutorial.Count == 0)
            ForLinkedTutorial.Clear();

        _Handle = Addressables.LoadAssetAsync<TutorialSetSO>(TutorialKey);
        await _Handle.Task;//불러 와질 때까지 비동기 대기

        if(_Handle.Status !=AsyncOperationStatus.Succeeded)
        {//데이터를 불러오는것의 결과(상태)가 성공적과 다르다면
            //Debug.LogError($"Tutorial Load Fail : {TutorialKey}");
            return;
        }

        CurrentTutorialInfo = _Handle.Result;
        CurrentTutorialIndex = 0;
        TutorialImage.gameObject.SetActive(true);
        TutorialSkipButton.gameObject.SetActive(IsAfterTutorial);
        ApplyTutorialPage();//이게 실질적인 text랑 이미지 적용
    }
    private void NextTutorial()//이건 글자 토토독이 끝났을때 클릭되면 작동되게
    {
        if (CurrentTutorialInfo == null)
            return;

        CurrentTutorialIndex++;
        if(CurrentTutorialIndex >= CurrentTutorialInfo.TutorialPages.Count)
        {
            if(ForLinkedTutorial.Count == 0)
            {//뒤이어 나올 Tutorial이 아무것도 없다면
                HideTutorialPage();
            }
            else
            {//뒤이어 나올 Tutorial이 있다면
                StartTutorial(ForLinkedTutorial[0]);
            }
                return;
        }
        ApplyTutorialPage();
    }

    private void ApplyTutorialPage()
    {
        TutorialImage.sprite = CurrentTutorialInfo.TutorialPages[CurrentTutorialIndex];
        TutorialText.text = CurrentTutorialInfo.TutorialText[CurrentTutorialIndex];
        TutorialText.rectTransform.anchoredPosition = CurrentTutorialInfo.TutorialTextPos[CurrentTutorialIndex];
        PlayTutorialText();//글자 토도독, 버튼 누르면 스킵되게
    }

    private void HideTutorialPage()
    {
        TutorialTextSkip();

        if(ForLinkedTutorial.Count == 0)//뒤에 이어질게 없을때만 끄기
        {
            TutorialImage.sprite = null;
            TutorialText.text = null;
            TutorialImage.gameObject.SetActive(false);
            IsAfterTutorial = false;
        }


        CurrentTutorialInfo = null;
        if(_Handle.IsValid())//살아있는 핸들이라면 해방
        {
            Addressables.Release(_Handle);
        }
    }

    public void ClickTutorialButton()
    {
        //글자가 출력중이라면 글자를 쭉 출력
        //글자의 출력이 끝났다면 다음 튜토리얼 페이지로
        if(TutorialTextCo != null)
        {//이거면 토도독 실행중->skip해야함
            TutorialTextSkip();
        }
        else
        {//토도독 실행 끝->다음꺼로
            NextTutorial();
        }
    }
    public void ClickAllTutorialButton()
    {//전체적 설명을 클릭했을때
        //구분해야함 -> 타이틀 씬인지, 탐색, 전투, 이벤트, 휴식 인지
        IsAfterTutorial = true;
        if (PlayerMgr == null)
        {//여기 들어오면 title씬
            SaveLinkedTutorialList("Title");//여기서 플레이할 튜토리얼이 정해짐
            //SaveLinkedTutorialList("Camping");
        }
        else
        {//여기 들어오면 플레이씬
            switch(PlayerMgr.GetPlayerInfo().GetPlayerStateInfo().CurrentPlayerAction)
            {
                case (int)EPlayerCurrentState.SelectAction:
                    SaveLinkedTutorialList("Search");
                    break;
                case (int)EPlayerCurrentState.Battle:
                    SaveLinkedTutorialList("Battle");
                    break;
                case (int)EPlayerCurrentState.OtherEvent:
                    SaveLinkedTutorialList("Event");
                    break;
                case (int)EPlayerCurrentState.Rest:
                    SaveLinkedTutorialList("Camping");
                    break;
                default:
                    return;
            }
        }
        StartTutorial(ForLinkedTutorial[0]);
    }
    public void CancelTutorial()
    {
        ForLinkedTutorial.Clear();
        HideTutorialPage();
    }

    private void PlayTutorialText()
    {
        if (TutorialTextCo != null)
            StopCoroutine(TutorialTextCo);

        TutorialText.text = CurrentTutorialInfo.TutorialText[CurrentTutorialIndex];
        TutorialText.maxVisibleCharacters = 0;

        TutorialTextCo = StartCoroutine(TextCoroutine());
    }

    private IEnumerator TextCoroutine()
    {
        TutorialText.ForceMeshUpdate();
        int TotalTextCount = TutorialText.textInfo.characterCount;//<-아마 여기서 접근하면서 터지는데.,....

        for(int i = 0; i <= TotalTextCount; i++)
        {
            TutorialText.maxVisibleCharacters = i;
            if(i > 0)
            {
                TMP_CharacterInfo CharInfo = TutorialText.textInfo.characterInfo[i - 1];
                if(CharInfo.character != ' ')
                {
                    SoundManager.Instance.PlayUISFX("TextTyping");
                }
            }
            yield return new WaitForSeconds(TextDelay);
        }
        TutorialTextCo = null;
    }

    private void TutorialTextSkip()
    {
        if (TutorialTextCo != null)
            StopCoroutine(TutorialTextCo);

        TutorialTextCo = null;
        TutorialText.maxVisibleCharacters = int.MaxValue;
    }

    private void SaveLinkedTutorialList(string CurrentSituation)
    {
        //버튼 클릭으로 들어오는 거면 여기에 들어와도 괜찮은데.....
        //버튼 클릭용 호출을 냅둔것 처럼 함수 호출용으로 하나더 만든다?
        ForLinkedTutorial.Clear();
        switch (CurrentSituation)
        {
            case "Title":
                ForLinkedTutorial.Add("Tutorial/Title");
                break;
            case "Search":
                ForLinkedTutorial.Add("Tutorial/Searching");
                ForLinkedTutorial.Add("Tutorial/SearchingBag");
                ForLinkedTutorial.Add("Tutorial/SearchingRest");
                break;
            case "Battle":
                ForLinkedTutorial.Add("Tutorial/Battle");
                ForLinkedTutorial.Add("Tutorial/BattlePlayerTurn");
                ForLinkedTutorial.Add("Tutorial/PlayerMagCard");
                ForLinkedTutorial.Add("Tutorial/MonsterTurn");
                ForLinkedTutorial.Add("Tutorial/BattleSuddenAttack");
                break;
            case "Event":
                ForLinkedTutorial.Add("Tutorial/Event");
                break;
            case "Camping":
                ForLinkedTutorial.Add("Tutorial/Camping");
                ForLinkedTutorial.Add("Tutorial/CampingRest");
                ForLinkedTutorial.Add("Tutorial/CampingLevelUp");
                ForLinkedTutorial.Add("Tutorial/CampingEquip");
                break;
            case "Tutorial/BattlePlayerTurn":
                if(JsonReadWriteManager.Instance.T_Info.Battle == false)
                {
                    JsonReadWriteManager.Instance.T_Info.Battle = true;
                    ForLinkedTutorial.Add("Tutorial/Battle");
                    ForLinkedTutorial.Add("Tutorial/BattlePlayerTurn");
                }//없으면 ForLinkTutorial.count = 0;
                break;
            case "Tutorial/MonsterTurn":
                if (JsonReadWriteManager.Instance.T_Info.Battle == false)
                {
                    JsonReadWriteManager.Instance.T_Info.Battle = true;
                    ForLinkedTutorial.Add("Tutorial/Battle");
                    ForLinkedTutorial.Add("Tutorial/MonsterTurn");
                }//없으면 ForLinkTutorial.count = 0;
                break;
            case "Tutorial/BattleSuddenAttack":
                if(JsonReadWriteManager.Instance.T_Info.Battle == false)
                {
                    JsonReadWriteManager.Instance.T_Info.Battle = true;
                    ForLinkedTutorial.Add("Tutorial/Battle");
                }//없으면 ForLinkTutorial.count = 0;
                if (JsonReadWriteManager.Instance.T_Info.BattleMonsterTurn == false)
                {
                    JsonReadWriteManager.Instance.T_Info.BattleMonsterTurn = true;
                    ForLinkedTutorial.Add("Tutorial/MonsterTurn");
                }//없으면 ForLinkTutorial.count = 0;
                if(ForLinkedTutorial.Count >= 1)
                {//위에서 뭐하나라도 들어간거임
                    ForLinkedTutorial.Add("Tutorial/BattleSuddenAttack");
                }
                break;
        }
    }
}
