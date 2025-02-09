using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;

public class StageController : MonoBehaviour
{
    [SerializeField] private UIController uiController;
    [SerializeField] private List<CollisionReciver> goalObjs;

    [Tooltip("ボールをけってから、自動的にリトライするまでの時間")]
    [SerializeField] private float retryTime = 5.5f;

    [Tooltip("リトライボタンを表示するまでの時間")]
    [SerializeField] private float retryButtonDelay = 5.0f;

    [Tooltip("ゲーム内のサッカーボールオブジェクト")]
    [SerializeField] private SoccerBall soccerBall;

    private CompositeDisposable retrySubscription = new CompositeDisposable(); // リトライ処理の購読管理
    private bool isReplay = false;

    void Start()
    {
        foreach (var go in goalObjs)
        {
            go.OnBallHit
                .ThrottleFirst(TimeSpan.FromSeconds(1.5))  // 1.5秒間隔で受け取る
                .Subscribe(_ =>
                {
                    StageClear();
                })
                .AddTo(this);
        }

        uiController.kickButton.onClick.AsObservable()
            .Subscribe(_ =>
            {
                StartRetryTimer(); // キックボタンを押した際にリトライタイマーを開始
                ShowRetryButtonWithDelay(); // 5秒後にリトライボタンを表示
                isReplay = false; // 念のため、もしフラグがクリアされてない状態で次のシーン来ても安全だから
            }).AddTo(this);

        uiController.retryButton.onClick.AddListener(() => {
            retrySubscription.Clear();
            uiController.ShowRetryButton(false); 
            RetryStage();
        });

        uiController.NextButtonAlpa.onClick.AddListener(() =>
        {
            retrySubscription.Clear();
            isReplay = false;
            string sceneName = SceneManager.GetActiveScene().name;
            SaveLoadManager.SaveLastStage(sceneName);
            SceneListUtility.LoadNextStage(sceneName);
        });

        }

    /// <summary>
    /// 一定時間後にリトライを実行する
    /// </summary>
    private void StartRetryTimer()
    {
        // 既存のリトライ購読を破棄（リセット）
        retrySubscription.Clear();

        // 指定時間後にリトライチェック
        Observable.Timer(System.TimeSpan.FromSeconds(retryTime))
            .Subscribe(_ =>
            {
                // ボールが停止している場合のみリトライ
                if (soccerBall.IsStopped.Value)
                {
                    RetryStage();
                }
                else
                {
                    // ボールが停止するまで監視し、停止したらリトライを実行
                    soccerBall.IsStopped
                        .Where(isStopped => isStopped) // 停止した瞬間のみ処理
                        .Take(1) // 1回だけ実行
                        .Subscribe(__ => RetryStage())
                        .AddTo(retrySubscription);
                }
            })
            .AddTo(retrySubscription);
    }

    /// <summary>
    /// 5秒後にリトライボタンを表示する
    /// </summary>
    private void ShowRetryButtonWithDelay()
    {
        Observable.Timer(System.TimeSpan.FromSeconds(retryButtonDelay))
            .Subscribe(_ =>
            {
                uiController.ShowRetryButton(true);
            })
            .AddTo(retrySubscription);
    }

    /// <summary>
    /// ステージのリトライ処理
    /// </summary>
    private async void RetryStage()
    {
        uiController.ShowRetryButton(false); // リトライ時に非表示
        await uiController.FadeToBlackForOneSecond();
        uiController.Retry();
    }

    /// <summary>
    /// ステージクリア処理
    /// </summary>
    private async void StageClear()
    {
        if (isReplay) 
        {
            isReplay = false;
            string sceneName = SceneManager.GetActiveScene().name;
            SaveLoadManager.SaveLastStage(sceneName);
            await Task.Delay(1200);

            SceneListUtility.LoadNextStage(sceneName);
        }
        isReplay = true;
        // クリア時にリトライタイマーをキャンセル
        retrySubscription.Clear();

        uiController.ShowRetryButton(false); // クリア時に非表示
        uiController.ShowClearText();

        uiController.cameraSwitcher.SwitchCamera(CameraSwitcher.CameraType.Replay1);
        await Task.Delay(1300);
        uiController.PlayReplay();

        uiController.freeKicker.Replay();
    }
}
