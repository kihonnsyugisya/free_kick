using UnityEngine;
using UniRx;
using System.Collections.Generic;
using System.Threading.Tasks;

public class StageController : MonoBehaviour
{
    [SerializeField] private UIController uiController;
    [SerializeField] private List<CollisionReciver> goalObjs;
    [Tooltip("ボールをけってから、自動的にリトライするまでの時間")]
    [SerializeField] private float retryCount = 5.5f;

    private CompositeDisposable retrySubscription = new CompositeDisposable(); // リトライ処理の購読を管理するためのオブジェクト

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

    private void StartRetryTimer()
    {
        // 既存のリトライ購読を破棄（必要に応じてリセット）
        retrySubscription.Clear();

        // タイマー購読の登録
        Observable.Timer(System.TimeSpan.FromSeconds(retryCount))
            .Subscribe(async _ =>
            {
                await uiController.FadeToBlackForOneSecond();
                uiController.Retry();
            })
            .AddTo(retrySubscription); // CompositeDisposableに登録
    }

    private async void StageClear()
    {
        // クリア時にリトライタイマーをキャンセル
        retrySubscription.Clear();

        // 1秒待機を挟む
        await Task.Delay(1000);

        uiController.ShowClearText();
        await uiController.FadeToBlackForOneSecond();
        uiController.Retry();
    }
}
