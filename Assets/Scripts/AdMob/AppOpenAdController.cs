using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

/// <summary>
/// Google Mobile Adsアプリオープン広告の使用方法を示すクラス。
/// </summary>
namespace GoogleMobileAds.Sample
{
    [AddComponentMenu("GoogleMobileAds/Samples/AppOpenAdController")]
    public class AppOpenAdController : AdmobUnitBase
    {
        /// <summary>
        /// 広告が表示可能な状態になるとアクティブ化されるUI要素。
        /// </summary>
        //public GameObject AdLoadedSt  atus;

        // アプリオープン広告は最大で4時間先までプリロードできます。
        private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
        private DateTime _expireTime;
        private AppOpenAd _appOpenAd;

        private void Awake()
        {
            // AppStateEventNotifierを使用してアプリケーションのオープン/クローズイベントを監視します。
            // これは、アプリを開いたときにロードされた広告を起動するために使用されます。
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
        }

        private void OnDestroy()
        {
            // イベントが完了したら常にイベントリスナーを解除します。
            AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
        }

        protected override void Initialize()
        {
#if UNITY_ANDROID
            _adUnitId = GetAdUnitIDForAndroid(AdType.APPOPEN);
#elif UNITY_IPHONE
            _adUnitId = GetAdUnitIDForIos(AdType.APPOPEN);
#else
            _adUnitId = GetAdUnitIDForIos(AdType.APPOPEN);
#endif
            LoadAd();
        }

        /// <summary>
        /// 広告を読み込みます。
        /// </summary>
        public void LoadAd()
        {
            // 新しい広告をロードする前に古い広告をクリーンアップします。
            if (_appOpenAd != null)
            {
                DestroyAd();
            }

            Debug.Log("アプリオープン広告を読み込み中。");

            // 広告をロードするために使用するリクエストを作成します。
            var adRequest = new AdRequest();

            // リクエストを送信して広告をロードします。
            AppOpenAd.Load(_adUnitId, adRequest, (AppOpenAd ad, LoadAdError error) =>
            {
                // 操作が理由付きで失敗した場合。
                if (error != null)
                {
                    Debug.LogError("アプリオープン広告の読み込みに失敗しました: " + error);
                    return;
                }

                // 不明な理由で操作が失敗した場合。
                // これは予期しないエラーです。発生した場合はこのバグを報告してください。
                if (ad == null)
                {
                    Debug.LogError("予期しないエラー: アプリオープン広告の読み込みイベントが null ad および null error で発生しました。");
                    return;
                }

                // 操作が正常に完了した場合。
                Debug.Log("アプリオープン広告が正常に読み込まれました。: " + ad.GetResponseInfo());
                _appOpenAd = ad;

                // アプリオープン広告は最大で4時間先までプリロードできます。
                _expireTime = DateTime.Now + TIMEOUT;

                // 広告イベントハンドラーを登録して機能を拡張します。
                RegisterEventHandlers(ad);

                // UIに広告が準備できたことを通知します。
                //AdLoadedStatus?.SetActive(true);
            });
        }

        /// <summary>
        /// 広告を表示します。
        /// </summary>
        public void ShowAd()
        {
            // アプリオープン広告は最大で4時間先までプリロードできます。
            if (_appOpenAd != null && _appOpenAd.CanShowAd() && DateTime.Now < _expireTime)
            {
                Debug.Log("アプリオープン広告を表示中。");
                if (InterstitialAdController.isClickAd)
                {
                    InterstitialAdController.isClickAd = false;
                    Debug.Log("インタースティシャルからの戻りなので、今回だけはアプリ起動広告を見逃してやる。。");
                    return;
                }
                _appOpenAd.Show();
            }
            else
            {
                Debug.LogError("アプリオープン広告はまだ準備ができていません。");
            }

            // UIに広告が準備できていないことを通知します。
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// 広告を破棄します。
        /// </summary>
        public void DestroyAd()
        {
            if (_appOpenAd != null)
            {
                Debug.Log("アプリオープン広告を破棄中。");
                _appOpenAd.Destroy();
                _appOpenAd = null;
            }

            // UIに広告が準備できていないことを通知します。
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// ResponseInfoをログに記録します。
        /// </summary>
        public void LogResponseInfo()
        {
            if (_appOpenAd != null)
            {
                var responseInfo = _appOpenAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        protected override void OnAppStateChanged(AppState state)
        {
            Debug.Log("アプリの状態が変更されました: " + state);

            // アプリがフォアグラウンドになり、広告が利用可能な場合は、それを表示します。
            if (state == AppState.Foreground)
            {
                if (RewardedAdController.isShowedAd) return;
                ShowAd();
                RewardedAdController.isShowedAd = false;
            }
        }

        private void RegisterEventHandlers(AppOpenAd ad)
        {
            // 広告がお金を稼いだと見積もられたときに発生します。
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("アプリオープン広告が{0} {1}を支払いました。",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // 広告のインプレッションが記録されたときに発生します。
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("アプリオープン広告がインプレッションを記録しました。");
            };
            // 広告がクリックされたときに発生します。
            ad.OnAdClicked += () =>
            {
                Debug.Log("アプリオープン広告がクリックされました。");
            };
            // 広告がフルスクリーンコンテンツを開いたときに発生します。
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("アプリオープン広告がフルスクリーンコンテンツを開きました。");

                // UIに広告が使用されており、準備ができていないことを通知します。
                //AdLoadedStatus?.SetActive(false);
            };
            // 広告がフルスクリーンコンテンツを閉じたときに発生します。
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("アプリオープン広告がフルスクリーンコンテンツを閉じました。");

                // 現在の広告が完了したときに新しい広告をロードすると便利です。
                LoadAd();
            };
            // 広告がフルスクリーンコンテンツのオープンに失敗したときに発生します。
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("アプリオープン広告がフルスクリーンコンテンツを開けませんでした: " + error);
            };
        }
    }
}
