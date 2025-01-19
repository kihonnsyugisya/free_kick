using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Samples;

/// <summary>
/// AdMobユニットの基本クラス。広告ユニットを管理するための抽象クラスです。
/// </summary>
namespace GoogleMobileAds
{
    public abstract class AdmobUnitBase : MonoBehaviour
    {
        public AdUnitIdSetting adUnitIdSetting;
        protected string _adUnitId;
        private void Awake()
        {
            AppStateEventNotifier.AppStateChanged += OnAppStateChangedBase;
        }

        private void OnDestroy()
        {
            AppStateEventNotifier.AppStateChanged -= OnAppStateChangedBase;
        }

        /// <summary>
        /// アプリケーションの状態が変更されたときに呼び出される基本メソッドです。
        /// </summary>
        private void OnAppStateChangedBase(AppState state)
        {
            Debug.Log("App State changed to : " + state);
            OnAppStateChanged(state);
        }

        private IEnumerator Start()
        {
            while (GoogleMobileAdsController._isInitialized == false)
            {
                yield return 0;
            }
            Initialize();
        }

        /// <summary>
        /// 広告管理が初期化された後に呼び出される仮想メソッドです。
        /// </summary>
        protected virtual void Initialize()
        {
            // AdsManagerの初期化が終わったあとに呼ばれる
        }

        /// <summary>
        /// アプリケーションの状態が変更されたときに呼び出される仮想メソッドです。
        /// </summary>
        protected virtual void OnAppStateChanged(AppState state)
        {
        }

        protected string GetAdUnitIDForAndroid(AdType adType)
        {
            if (adUnitIdSetting.isTest)
            {
                Debug.Log("Androidテスト広告の　" + adType + " の広告IDを取得");

                return adType switch
                {
                    AdType.APPOPEN => adUnitIdSetting.testAppOpenAndroid,
                    AdType.BANNER => adUnitIdSetting.testBannerAndroid,
                    AdType.INTERSTITIAL => adUnitIdSetting.testInterstitialAndroid,
                    AdType.REWARD => adUnitIdSetting.testRewardAndroid,
                    _ => "error",
                };
            }
            else {
                Debug.Log("Android本番広告の　" + adType + " の広告IDを取得");

                return adType switch
                {
                    AdType.APPOPEN => adUnitIdSetting.appOpenAndroid,
                    AdType.BANNER => adUnitIdSetting.bannerAndroid,
                    AdType.INTERSTITIAL => adUnitIdSetting.interstitialAndroid,
                    AdType.REWARD => adUnitIdSetting.rewardAndroid,
                    _ => "error",
                };
            }
        }

        protected string GetAdUnitIDForIos(AdType adType)
        {

            if (adUnitIdSetting.isTest)
            {
                Debug.Log("IOSテスト広告の　" + adType + " の広告IDを取得");

                return adType switch
                {
                    AdType.APPOPEN => adUnitIdSetting.testAppOpenIOS,
                    AdType.BANNER => adUnitIdSetting.testBannerIOS,
                    AdType.INTERSTITIAL => adUnitIdSetting.testInterstitialIOS,
                    AdType.REWARD => adUnitIdSetting.testRewardIOS,
                    _ => "error",
                };
            }
            else
            {
                Debug.Log("IOS本番広告の　" + adType + " の広告IDを取得");

                return adType switch
                {
                    AdType.APPOPEN => adUnitIdSetting.appOpenIOS,
                    AdType.BANNER => adUnitIdSetting.bannerIOS,
                    AdType.INTERSTITIAL => adUnitIdSetting.interstitialIOS,
                    AdType.REWARD => adUnitIdSetting.rewardIOS,
                    _ => "error",
                };
            }
        }

        protected enum AdType {
            BANNER,
            INTERSTITIAL,
            REWARD,
            APPOPEN
        }
    }
}
