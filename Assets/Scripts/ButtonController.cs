using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using GoogleMobileAds.Sample;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    private const int ButtonThrottleTimeMs = 2000; // ボタンの連続クリック防止時間 (ms)
    [SerializeField] private Button reviewButton; // レビュー機能ボタン
    [SerializeField] private Button interstitialButton; // インステボタン
    [SerializeField] private InterstitialAdController interstitialAdController; // 広告管理コントローラ
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ConfigureReviewButton();
        ConfigureInterstitialButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// インステボタンのクリックイベントを設定。
    /// </summary>
    private void ConfigureInterstitialButton()
    {
        interstitialButton.OnClickAsObservable()
            //.ThrottleFirst(TimeSpan.FromMilliseconds(ButtonThrottleTimeMs))
            //.TakeUntilDestroy(this)
            .Subscribe(_ => ShowAd());
    }

    /// <summary>
    /// インステ表示
    /// </summary>
    private void ShowAd()
    {
        if (!interstitialAdController.isSkipAd)
        {
            interstitialAdController.ShowAd();
        }
        else
        {
            // 画面遷移とか
            //SceneManager.LoadScene("TitleScene");
            Debug.Log("koko");
        }
    }

    /// <summary>
    /// レビュー機能ボタンのクリックイベントを設定。
    /// </summary>
    private void ConfigureReviewButton()
    {
        reviewButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(ButtonThrottleTimeMs))
            .TakeUntilDestroy(this)
            .Subscribe(_ => StartCoroutine(InAppReviewManager.RequestReview()));
    }
}

//admobいれたから、ATTとかバナー広告とか出せるでもシーンつくるところから。それができたら実機にビルドしてちゃんとテスト広告出せてるか見る。
//    できたらGDRPについてどうたいおうするのかもしらべてナレッジに対応策まとめたい
    