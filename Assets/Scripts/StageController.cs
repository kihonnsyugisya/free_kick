using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    [SerializeField] private UIController uiController;
    [SerializeField] private List<CollisionReciver> goalObjs;

    [Tooltip("ボールをけってから、自動的にリトライするまでの時間")]
    [SerializeField] private float retryTime = 5.5f;

    [Tooltip("ゲーム内のサッカーボールオブジェクト")]
    [SerializeField] private SoccerBall soccerBall;

    private CompositeDisposable retrySubscription = new CompositeDisposable(); // リトライ処理の購読管理

    void Start()
    {
        foreach (var go in goalObjs)
        {
            go.OnBallHit.Subscribe(_ =>
            {
                StageClear();
            }).AddTo(this);
        }

        uiController.kickButton.onClick.AsObservable()
            .Subscribe(_ =>
            {
                StartRetryTimer(); // キックボタンを押した際にリトライタイマーを開始
            }).AddTo(this);
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
    /// ステージのリトライ処理
    /// </summary>
    private async void RetryStage()
    {
        await uiController.FadeToBlackForOneSecond();
        uiController.Retry();
    }

    /// <summary>
    /// ステージクリア処理
    /// </summary>
    private async void StageClear()
    {
        // クリア時にリトライタイマーをキャンセル
        retrySubscription.Clear();

        uiController.ShowClearText();
        //await uiController.FadeToBlackForOneSecond();
        //uiController.Retry();
        string sceneName = SceneManager.GetActiveScene().name;
        SaveLoadManager.SaveLastStage(sceneName);
        await Task.Delay(1200);

        SceneListUtility.LoadNextStage(sceneName);
    }
}
