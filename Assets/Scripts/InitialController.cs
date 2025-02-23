using System.Threading.Tasks;
using DG.Tweening;
using GoogleMobileAds.Sample;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialController : MonoBehaviour
{
    [Tooltip("Prefesのデータを消してスタートするか（デバッグ用）")]
    [SerializeField] private bool isDataResetStart;

    [Tooltip("ロード画面全体の CanvasGroup（フェードイン/アウトに使用）")]
    [SerializeField] private CanvasGroup loadingCanvasGroup;

    [Tooltip("読み込み中のテキスト（例：\"読み込み中\"）")]
    [SerializeField] private TextMeshProUGUI loadingText;

    [Tooltip("テキストの点滅にかかる時間（1回のフェードアウトまたはフェードインの時間）")]
    [SerializeField] private float blinkDuration = 1f;

    [Tooltip("AdMob のインタースティシャル広告を管理するクラス")]
    [SerializeField] private InterstitialAdController interstitialAdController; // ※AdManagerは別途実装してください

    private void Start()
    {
        if (isDataResetStart) PlayerPrefs.DeleteAll();

        interstitialAdController.isAdClosed.Subscribe(value => {
            if (value)
            {
                OnAdClosed();
            }
        }).AddTo(this);

        ShowLoadingAndContinue();
    }

    /// <summary>
    /// ロード画面を表示し、「読み込み中」テキストを点滅させる。
    /// AdMobの広告が閉じられたら、PlayerPrefsから最後のステージ名を取得し、
    /// 次のステージにシーン遷移する。
    /// </summary>
    public async void ShowLoadingAndContinue()
    {
        // ロード画面をフェードインで表示（0.5秒）
        loadingCanvasGroup.DOFade(1, 0.5f);

        // 「読み込み中」テキストを点滅させる（アルファ値を0～1の間で繰り返し変化させる）
        loadingText.DOFade(0, blinkDuration).SetLoops(-1, LoopType.Yoyo);

        await Task.Delay(2000);

        // AdMobのインタースティシャル広告を表示
        if (interstitialAdController.CheckShowAd())
        {
            interstitialAdController.ShowAd();
            return;
        }

        OnAdClosed();
    }

    /// <summary>
    /// 広告が閉じられたときに呼ばれるコールバックメソッド。
    /// 最後に到達したステージを確認し、次に遷移するステージを決定してシーン遷移を行います。
    /// </summary>
    private void OnAdClosed()
    {
        // 点滅アニメーションを停止
        loadingText.DOKill();

        // プレイヤーが初めてプレイした場合は "TutorialScene"、それ以外は "StageSelect" に遷移
        string nextStage = SaveLoadManager.LoadTutorialCompleted() ? "StageSelect" : "TutorialScene";

        // ロード画面をフェードアウトさせ、完了後にシーン遷移する（0.5秒の遅延）
        loadingCanvasGroup.DOFade(0, 0.5f).OnComplete(() => {
            SceneManager.LoadScene(nextStage);
        });
    }

}
