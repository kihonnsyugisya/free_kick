using UnityEngine;
using UniRx;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private CinemachineCamera kickerCam;
    [SerializeField] private CinemachineCamera ballCam;
    [SerializeField] private CinemachineCamera replayCam1;
    [SerializeField] private CinemachineCamera replayCam2;

    public static CameraSwitcher Instance { get; private set; }

    // カメラタイプをEnumで管理
    public enum CameraType
    {
        Kicker,
        Ball,
        Replay1,
        Replay2
    }

    private void Awake()
    {
        Instance = this;
    }

    // 指定されたカメラに切り替えるメソッド
    public void SwitchCamera(CameraType cameraType)
    {
        Observable.Timer(System.TimeSpan.FromSeconds(0)) // 0秒後に通知を発行
            .Subscribe(_ => SetCamera(cameraType))
            .AddTo(this);
    }

    // カメラの設定を行うメソッド
    private void SetCamera(CameraType cameraType)
    {
        // 全カメラの優先度を0に設定
        kickerCam.Priority = 0;
        ballCam.Priority = 0;
        replayCam1.Priority = 0;
        replayCam2.Priority = 0;

        // 指定されたカメラの優先度を10に設定
        switch (cameraType)
        {
            case CameraType.Kicker:
                kickerCam.Priority = 10;
                break;
            case CameraType.Ball:
                ballCam.Priority = 10;
                break;
            case CameraType.Replay1:
                replayCam1.Priority = 10;
                break;
            case CameraType.Replay2:
                replayCam2.Priority = 10;
                break;
        }
    }
}
