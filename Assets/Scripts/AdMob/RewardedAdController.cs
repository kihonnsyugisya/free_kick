using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UniRx;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Google Mobile Adsのリワード広告を使用する方法を示します。
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/RewardedAdController")]
    public class RewardedAdController : AdmobUnitBase
    {
        /// <summary>
        /// 広告が表示可能な状態になるときにアクティブ化されるUI要素です。
        /// </summary>
        //public GameObject AdLoadedStatus;

        [HideInInspector] public BoolReactiveProperty isReady = new();
        [HideInInspector] public BoolReactiveProperty isAdClosed = new();
        [HideInInspector] public static bool isClickAd = false;

        /// <summary>
        /// リワード広告を見た後の状態かを判定するためのフラグ
        /// リワード広告視聴後にアプリ起動広告が表示されるのを防ぐため
        /// </summary>
        [HideInInspector] public static bool isShowedAd = false;

        protected override void Initialize()
        {
#if UNITY_ANDROID
            _adUnitId = GetAdUnitIDForAndroid(AdType.REWARD);
#elif UNITY_IPHONE
            _adUnitId = GetAdUnitIDForIos(AdType.REWARD);
#else
            _adUnitId = GetAdUnitIDForIos(AdType.REWARD);
#endif
            LoadAd();
        }

        private RewardedAd _rewardedAd;

        /// <summary>
        /// 広告を読み込みます。
        /// </summary>
        public void LoadAd()
        {
            // 古い広告を読み込む前にクリーンアップします。
            if (_rewardedAd != null)
            {
                DestroyAd();
            }

            Debug.Log("リワード広告を読み込んでいます。");

            // 広告を読み込むためのリクエストを作成します。
            var adRequest = new AdRequest();

            // 広告を読み込むリクエストを送信します。
            RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                // エラーがある場合は処理を中断します。
                if (error != null)
                {
                    Debug.LogError("リワード広告の読み込みに失敗しました : " + error);
                    isReady.Value = false;
                    return;
                }
                // 広告がnullの場合はエラーを出力します。
                if (ad == null)
                {
                    Debug.LogError("予期しないエラー：nullの広告とnullのエラーが発生しました。");
                    isReady.Value = false;
                    return;
                }

                // 広告の読み込みが成功した場合の処理です。
                Debug.Log("リワード広告が読み込まれました : " + ad.GetResponseInfo());
                _rewardedAd = ad;

                // 広告のイベントハンドラーを登録します。
                RegisterEventHandlers(ad);

                isReady.Value = true;
                isAdClosed.Value = false;

                // UIに広告が読み込まれたことを通知します。
                //AdLoadedStatus?.SetActive(true);
            });
        }

        /// <summary>
        /// 広告を表示します。
        /// </summary>
        public void ShowAd()
        {
            if (_rewardedAd != null && _rewardedAd.CanShowAd())
            {
                Debug.Log("リワード広告を表示しています。");
                isShowedAd = true;
                _rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log(String.Format("リワード広告が報酬を授与しました: {0} {1}",
                                            reward.Amount,
                                            reward.Type));
                });
            }
            else
            {
                Debug.LogError("リワード広告はまだ準備ができていません。");
            }

            // UIに広告が準備できていないことを通知します。
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// 広告を破棄します。
        /// </summary>
        public void DestroyAd()
        {
            if (_rewardedAd != null)
            {
                Debug.Log("リワード広告を破棄しています。");
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            // UIに広告が準備できていないことを通知します。
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// ResponseInfoをログに記録します。
        /// </summary>
        public void LogResponseInfo()
        {
            if (_rewardedAd != null)
            {
                var responseInfo = _rewardedAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        private void RegisterEventHandlers(RewardedAd ad)
        {
            // 広告が収益を得ると推定されたときに発生します。
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("リワード広告が支払われました : {0} {1}",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // 広告のインプレッションが記録されたときに発生します。
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("リワード広告がインプレッションを記録しました。");
            };
            // 広告がクリックされたときに発生します。
            ad.OnAdClicked += () =>
            {
                Debug.Log("リワード広告がクリックされました。");
            };
            // 広告が全画面コンテンツを開いたときに発生します。
            ad.OnAdFullScreenContentOpened += () =>
            {
                isReady.Value = false;
                Debug.Log("リワード広告が全画面コンテンツを開きました。");
            };
            // 広告が全画面コンテンツを閉じたときに発生します。
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("リワード広告が全画面コンテンツを閉じました。");
                isAdClosed.Value = true;
                LoadAd();
                
            };
            // 広告が全画面コンテンツのオープンに失敗したときに発生します。
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                isAdClosed.Value = true;
                Debug.LogError("リワード広告が全画面コンテンツを開くのに失敗しました : "
                    + error);
                LoadAd();
            };
        }
    }
}
