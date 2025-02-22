using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Samples;

namespace GoogleMobileAds.Sample
{
    [AddComponentMenu("GoogleMobileAds/Samples/BannerViewController")]
    public class BannerViewController : AdmobUnitBase
    {
        private BannerView _bannerView;

        [Tooltip("広告の表示位置をBottmかTopか設定する（falseでBottm）")]
        [SerializeField] private bool showAtTop = false; // デフォルトでBottomに設定

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

        public void CreateBannerView()
        {
            Debug.Log("バナー広告ビューを作成しています。");

            if (_bannerView != null)
            {
                DestroyAd();
            }

            AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            AdPosition adPosition = showAtTop ? AdPosition.Top : AdPosition.Bottom;

            _bannerView = new BannerView(_adUnitId, adSize, adPosition);
            ListenToAdEvents();
            Debug.LogWarning("バナー広告ビューが作成されました。");
        }

        public void LoadAd()
        {
            if (_bannerView == null)  // バナーが未作成の場合のみ作成
            {
                CreateBannerView();
            }

            if (_bannerView != null)  // ここを追加して、念のためチェック
            {
                var adRequest = new AdRequest();
                Debug.Log("バナー広告を読み込んでいます。");
                _bannerView.LoadAd(adRequest);
            }
        }


        public void ShowAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("バナー広告を表示しています。");
                _bannerView.Show();
            }
        }

        public void HideAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("バナー広告を非表示にしています。");
                _bannerView.Hide();
            }
        }

        public void DestroyAd()
        {
            if (_bannerView != null)
            {
                Debug.Log("バナー広告ビューを破棄しています。");
                _bannerView.Destroy();
                _bannerView = null;
            }
        }

        private void ListenToAdEvents()
        {
            _bannerView.OnBannerAdLoaded += () => Debug.Log("バナー広告がロードされました。");
            _bannerView.OnBannerAdLoadFailed += (LoadAdError error) => Debug.LogError("バナー広告のロードに失敗しました: " + error);
            _bannerView.OnAdPaid += (AdValue adValue) => Debug.Log($"バナー広告が{adValue.Value} {adValue.CurrencyCode}を支払いました。");
            _bannerView.OnAdImpressionRecorded += () => Debug.Log("バナー広告のインプレッションが記録されました。");
            _bannerView.OnAdClicked += () => Debug.Log("バナー広告がクリックされました。");
            _bannerView.OnAdFullScreenContentOpened += () => Debug.Log("バナー広告がフルスクリーンを開きました。");
            _bannerView.OnAdFullScreenContentClosed += () => Debug.Log("バナー広告がフルスクリーンを閉じました。");
        }
    }
}
