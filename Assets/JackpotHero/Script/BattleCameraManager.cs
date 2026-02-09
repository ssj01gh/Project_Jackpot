using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    CinemachineVirtualCamera ActionCamera;

    [Header("Zoom")]
    public GameObject FollowGameObject;
    public float ZoomSize;
    public float InTime;//줌인 되는 시간
    public float HoldTime;//줌인 되서 유지 되는 시간
    public float OutTime;//줌 아웃 되는 시간
    
    protected float DefaultSize = 5f;
    protected Vector3 DefaultPos = Vector3.zero;
    protected Coroutine Routine;
    public bool IsCoroutineRunning { get; protected set; }
    void Start()
    {
        
    }

    public void PlayBattleCamera(Vector3 ZoomPos)
    {
        if (Routine != null)
            StopCoroutine(Routine);

        Routine = StartCoroutine(ZoomRoutine(ZoomPos));
    }

    IEnumerator ZoomRoutine(Vector3 ZoomPos)
    {
        IsCoroutineRunning = true;

        //ActionCamera.Follow = FollowGameObject.transform;
        //ActionCamera.LookAt = FollowGameObject.transform;

        ActionCamera.Priority = 20;
        //Damping은 Target에 대해 얼마나 빨리 움직이냐임 작을수로 빨리 움직임
        //정상적인 화면 밖이 안보이려면 줌인 할땐->줌인 속도가 떠 빠르게, 이동 속도가 느리게
        ActionCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0.3f;
        ActionCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0.3f;
        ActionCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = 0.3f;
        FollowGameObject.transform.position = ZoomPos;
        yield return LerpOrtho(DefaultSize, ZoomSize, InTime);

        yield return new WaitForSeconds(HoldTime);
        //줌 아웃 할땐-> 줌 아웃 속도가 더 느리게, 이동속도가 더 빠르게
        ActionCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0.5f;
        ActionCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0.5f;
        ActionCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ZDamping = 0.5f;

        FollowGameObject.transform.position = DefaultPos;
        yield return LerpOrtho(ZoomSize, DefaultSize, OutTime);

        ActionCamera.Priority = 5;
        IsCoroutineRunning = false;
    }

    IEnumerator LerpOrtho(float FromSize, float ToSize, float f_Time)
    {
        float T = 0f;
        while(T < f_Time)
        {
            T += Time.deltaTime;
            ActionCamera.m_Lens.OrthographicSize =
                Mathf.Lerp(FromSize, ToSize, T / f_Time);
            yield return null;
        }
        ActionCamera.m_Lens.OrthographicSize = ToSize;
    }
}
