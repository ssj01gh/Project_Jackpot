using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TutorialManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Image TutorialImage;

    private TutorialSetSO CurrentTutorialImageInfo;
    private AsyncOperationHandle<TutorialSetSO> _Handle;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
