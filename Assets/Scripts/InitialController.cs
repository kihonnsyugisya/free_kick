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

        // AdMobのインタースティシャル広告を表示
        if (!interstitialAdController.isSkipAd)
        {
            await Task.Delay(1200);
            interstitialAdController.ShowAd();
            return;
        }

        OnAdClosed();
    }

    /// <summary>
    /// 広告が閉じられたときに呼ばれるコールバック。
    /// PlayerPrefsから最後のステージ名を取得し、次のステージ名を計算してシーン遷移する。
    /// </summary>
    private void OnAdClosed()
    {
        // 点滅アニメーションを停止
        loadingText.DOKill();

        // PlayerPrefsから最後のステージ名を取得（存在しなければ "Stage1" をデフォルトとする）
        string lastStage = SaveLoadManager.LoadLastStage();

        // 次のステージ名を計算
        string nextStage = GetNextStageName(lastStage);

        // ロード画面をフェードアウトさせ、完了後にシーン遷移する（0.5秒）
        loadingCanvasGroup.DOFade(0, 0.5f).OnComplete(() => {
            SceneManager.LoadScene(nextStage);
        });
    }

    /// <summary>
    /// 現在のステージ名から次のステージ名を計算する。
    /// 例: "Stage3" → "Stage4"
    /// </summary>
    /// <param name="currentStage">現在のステージ名</param>
    /// <returns>次のステージ名</returns>
    private string GetNextStageName(string currentStage)
    {
        int number = 1;
        if (currentStage.StartsWith("Stage"))
        {
            // "Stage" の文字数は5文字なので、5文字目以降を取得
            string numberPart = currentStage.Substring(5);
            if (int.TryParse(numberPart, out number))
            {
                number += 1;
            }
            return "Stage" + number.ToString();
        }
        return "TutorialScene";
    }
}
