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
    private float TextDelay = 0.04f;
    public async void StartTutorial(string TutorialKey)
    {
        //이전 꺼 정리
        HideTutorialPage();

        _Handle = Addressables.LoadAssetAsync<TutorialSetSO>(TutorialKey);
        await _Handle.Task;//불어 와질 때까지 비동기 대기

        if(_Handle.Status !=AsyncOperationStatus.Succeeded)
        {//데이터를 불러오는것의 결과(상태)가 성공적과 다르다면
            Debug.LogError($"Tutorial Load Fail : {TutorialKey}");
            return;
        }

        CurrentTutorialInfo = _Handle.Result;
        CurrentTutorialIndex = 0;
        ApplyTutorialPage();
        TutorialImage.gameObject.SetActive(true);
    }



    public void NextTutorial()
    {
        if (CurrentTutorialInfo == null)
            return;

        CurrentTutorialIndex++;
        if(CurrentTutorialIndex >= CurrentTutorialInfo.TutorialPages.Count)
        {
            HideTutorialPage();
            return;
        }
        ApplyTutorialPage();
    }

    private void ApplyTutorialPage()
    {
        TutorialImage.sprite = CurrentTutorialInfo.TutorialPages[CurrentTutorialIndex];
        TutorialText.text = CurrentTutorialInfo.TutorialText[CurrentTutorialIndex];
        TutorialText.rectTransform.anchoredPosition = CurrentTutorialInfo.TutorialTextPos[CurrentTutorialIndex];
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

    public void PlayTutorialText()
    {
        if (TutorialTextCo != null)
            StopCoroutine(TutorialTextCo);

        TutorialText.text = CurrentTutorialInfo.TutorialText[CurrentTutorialIndex];
        TutorialText.maxVisibleCharacters = 0;
    }

    private IEnumerator TextCoroutine()
    {
        TutorialText.ForceMeshUpdate();
        int TotalTextCount = TutorialText.textInfo.characterCount;

        for(int i = 0; i <= TotalTextCount; i++)
        {
            TutorialText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(TextDelay);
        }
    }

    private void TutorialTextSkip()
    {
        if (TutorialTextCo != null)
            StopCoroutine(TutorialTextCo);

        TutorialText.maxVisibleCharacters = int.MaxValue;
    }
}
