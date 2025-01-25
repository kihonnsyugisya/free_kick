using UnityEngine;
using UniRx;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera kickerCam;
    [SerializeField] private CinemachineCamera ballCam;

    public void SwitchToKickerCamera()
    {
        Observable.Timer(System.TimeSpan.FromSeconds(0.5)) // 0.5秒後に通知を発行
            .Subscribe(_ => SetKickerCamera())           // 通知を受け取ったらカメラ切り替え
            .AddTo(this);                               // 自動的に購読解除
    }

    public void SwitchToBallCamera()
    {
        Observable.Timer(System.TimeSpan.FromSeconds(0))
            .Subscribe(_ => SetBallCamera())
            .AddTo(this);
    }

    private void SetKickerCamera()
    {
        kickerCam.Priority = 10;
        ballCam.Priority = 0;
    }

    private void SetBallCamera()
    {
        kickerCam.Priority = 0;
        ballCam.Priority = 10;
    }
}
