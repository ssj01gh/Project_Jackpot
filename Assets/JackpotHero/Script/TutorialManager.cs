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
    private Image TutorialImage;
    [SerializeField]
    private TextMeshProUGUI TutorialText;

    private TutorialSetSO CurrentTutorialInfo;
    private AsyncOperationHandle<TutorialSetSO> _Handle;
    private int CurrentTutorialIndex = 0;

    private Coroutine TutorialTextCo;
    private float TextDelay = 0.06f;
    private string NextKey = "";
    public async void StartTutorial(string TutorialKey)//이걸로 튜토리얼이 시작됨
    {
        //이전 꺼 정리
        HideTutorialPage();

        //여기에서 battle족은 특별 관리?
        TutorialKey = CkeckTutorialForBattle(TutorialKey);

        _Handle = Addressables.LoadAssetAsync<TutorialSetSO>(TutorialKey);
        await _Handle.Task;//불러 와질 때까지 비동기 대기

        if(_Handle.Status !=AsyncOperationStatus.Succeeded)
        {//데이터를 불러오는것의 결과(상태)가 성공적과 다르다면
            Debug.LogError($"Tutorial Load Fail : {TutorialKey}");
            return;
        }

        CurrentTutorialInfo = _Handle.Result;
        CurrentTutorialIndex = 0;
        TutorialImage.gameObject.SetActive(true);
        ApplyTutorialPage();//이게 실질적인 text랑 이미지 적용
    }
    public void NextTutorial()//이건 글자 토토독이 끝났을때 클릭되면 작동되게
    {
        if (CurrentTutorialInfo == null)
            return;

        CurrentTutorialIndex++;
        if(CurrentTutorialIndex >= CurrentTutorialInfo.TutorialPages.Count)
        {
            //여기서 튜토리얼을 끝낼지 아니면 다른 튜토리얼로 연결할지
            if (NextKey == "")//이거면 연결된게 딱히 없음
            {
                HideTutorialPage();
            }
            else
            {//배틀에서 배틀 튜토리얼 안봤을때 튜토리얼 보고난 후의 플레이어턴, 몬스터턴, 습격 
                StartTutorial(NextKey);
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
        TutorialImage.sprite = null;
        TutorialText.text = null;
        TutorialImage.gameObject.SetActive(false);

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

    public void PlayTutorialText()
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

    private string CkeckTutorialForBattle(string TutorialKey)
    {
        string ReturnString = TutorialKey;
        if (TutorialKey == "Tutorial/BattlePlayerTurn")
        {
            if (JsonReadWriteManager.Instance.T_Info.Battle == false)
            {
                JsonReadWriteManager.Instance.T_Info.Battle = true;
                NextKey = TutorialKey;
                ReturnString = "Tutorial/Battle";
            }
            else
            {
                NextKey = "";
            }
        }
        else if (TutorialKey == "Tutorial/MonsterTurn")
        {
            if (JsonReadWriteManager.Instance.T_Info.Battle == false)
            {
                JsonReadWriteManager.Instance.T_Info.Battle = true;
                NextKey = TutorialKey;
                ReturnString = "Tutorial/Battle";
            }
            else
            {
                NextKey = "";
            }
        }
        else if (TutorialKey == "Tutorial/BattleSuddenAttack")
        {
            if (JsonReadWriteManager.Instance.T_Info.Battle == false)
            {
                JsonReadWriteManager.Instance.T_Info.Battle = true;
                NextKey = TutorialKey;
                ReturnString = "Tutorial/Battle";
            }
            else
            {
                if(JsonReadWriteManager.Instance.T_Info.BattleMonsterTurn == false)
                {
                    JsonReadWriteManager.Instance.T_Info.BattleMonsterTurn = true;
                    NextKey = "Tutorial/MonsterTurn";
                }
                else
                    NextKey = "";
            }
        }
        else
        {
            NextKey = "";
        }
        return ReturnString;
    }
}
