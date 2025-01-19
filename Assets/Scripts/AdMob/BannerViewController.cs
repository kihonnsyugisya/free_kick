using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Samples;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Google Mobile Adsのバナー広告ビューを使用する方法を示します。
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/BannerViewController")]
    public class BannerViewController : AdmobUnitBase
    {
        /// <summary>
        /// 広告が表示可能な状態になるときにアクティブ化されるUI要素です。
        /// </summary>
        //public GameObject AdLoadedStatus;

        // これらの広告ユニットは常にテスト広告を提供するように構成されています。

        private BannerView _bannerView;
        

        protected override void Initialize()
        {
#if UNITY_ANDROID
            _adUnitId = GetAdUnitIDForAndroid(AdType.BANNER);
#elif UNITY_IPHONE
            _adUnitId = GetAdUnitIDForIos(AdType.BANNER);
#else
            _adUnitId = GetAdUnitIDForIos(AdType.BANNER);
#endif
            LoadAd();
            ShowAd();
        }

        /// <summary>
        /// 画面の上部に320x50のバナーを作成します。
        /// </summary>
        public void CreateBannerView()
        {
            Debug.Log("バナー広告ビューを作成しています。");

            // すでにバナー広告がある場合は古いものを破棄します。
            if (_bannerView != null) 
            {
                DestroyAd();
            }
            AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            

#if UNITY_IPHONE
            // iPhoneの場合の処理
            _bannerView = new BannerView(_adUnitId, adSize, AdPosition.Bottom);

#elif UNITY_IOS && !UNITY_IPHONE
            // iPadの場合の処理
            _bannerView = new BannerView(_adUnitId, adSize, AdPosition.Bottom);
#else
            // それ以外のプラットフォーム（Androidなど）の場合の処理
             _bannerView = new BannerView(_adUnitId, adSize, AdPosition.Bottom);
#endif


            // バナーが発生する可能性のあるイベントをリッスンします。
            ListenToAdEvents();

            Debug.Log("バナー広告ビューが作成されました。");
        }

        /// <summary>
        /// バナー広告ビューを作成し、バナー広告を読み込みます。
        /// </summary>
        public void LoadAd()
        {
            // まずバナー広告ビューのインスタンスを作成します。
            if (_bannerView == null)
            {
                CreateBannerView();
            }

            // 広告を読み込むためのリクエストを作成します。
            var adRequest = new AdRequest();

            // 広告を読み込むリクエストを送信します。
            Debug.Log("バナー広告を読み込んでいます。");
            _bannerView.LoadAd(adRequest);
        }

        /// <summary>
        /// 広告を表示します。
        /// </summary>
        public void ShowAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("バナー広告を表示しています。");
                _bannerView.Show();
            }
        }

        /// <summary>
        /// 広告を非表示にします。
        /// </summary>
        public void HideAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("バナー広告を非表示にしています。");
                _bannerView.Hide();
            }
        }

        /// <summary>
        /// 広告を破棄します。
        /// BannerViewを使用し終わったら、その参照を削除する前に
        /// Destroy()メソッドを呼び出すことを必ず確認してください。
        /// </summary>
        public void DestroyAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("バナー広告ビューを破棄しています。");
                _bannerView.Destroy();
                _bannerView = null;
            }

            // 広告が準備できていないことをUIに通知します。
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// ResponseInfoをログに記録します。
        /// </summary>
        public void LogResponseInfo()
        {
            if (_bannerView != null)
            {
                var responseInfo = _bannerView.GetResponseInfo();
                if (responseInfo != null)
                {
                    Debug.Log(responseInfo);
                }
            }
        }

        /// <summary>
        /// バナーが発生する可能性のあるイベントをリッスンします。
        /// </summary>
        private void ListenToAdEvents()
        {
            // バナー広告がバナー広告ビューにロードされたときに発生します。
            _bannerView.OnBannerAdLoaded += () =>
            {
                Debug.Log("バナー広告ビューが次の応答で広告をロードしました : "
                    + _bannerView.GetResponseInfo());

                // 広告が準備できたことをUIに通知します。
                //AdLoadedStatus?.SetActive(true);
            };
            // バナー広告がバナー広告ビューにロードできなかったときに発生します。
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                Debug.LogError("バナー広告ビューがエラーで広告をロードできませんでした : " + error);
            };
            // 広告が収益を得ると推定されたときに発生します。
            _bannerView.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("バナー広告が{0} {1}を支払いました。",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // 広告のインプレッションが記録されたときに発生します。
            _bannerView.OnAdImpressionRecorded += () =>
            {
                Debug.Log("バナー広告ビューがインプレッションを記録しました。");
            };
            // 広告がクリックされたときに発生します。
            _bannerView.OnAdClicked += () =>
            {
                Debug.Log("バナー広告ビューがクリックされました。");
            };
            // 広告がフルスクリーンコンテンツを開いたときに発生します。
            _bannerView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("バナー広告ビューがフルスクリーンコンテンツを開きました。");
            };
            // 広告がフルスクリーンコンテンツを閉じたときに発生します。
            _bannerView.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("バナー広告ビューがフルスクリーンコンテンツを閉じました。");
            };
        }
    }
}

