using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UniRx;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Google Mobile Adsのインタースティシャル広告を使用する方法を示します。
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/InterstitialAdController")]
    public class InterstitialAdController : AdmobUnitBase
    {
        /// <summary>
        /// 広告が表示可能な状態になるときにアクティブ化されるUI要素です。
        /// </summary>
        //public GameObject AdLoadedStatus;

        private InterstitialAd _interstitialAd;

        public BoolReactiveProperty isReady = new();
        public BoolReactiveProperty isAdClosed = new();
        public static bool isClickAd = false;

        public bool isSkipAd = false;

        protected override void Initialize()
        {
#if UNITY_ANDROID
            _adUnitId = GetAdUnitIDForAndroid(AdType.INTERSTITIAL);
#elif UNITY_IPHONE
            _adUnitId = GetAdUnitIDForIos(AdType.INTERSTITIAL);
#else
            _adUnitId = GetAdUnitIDForIos(AdType.INTERSTITIAL);
#endif
            LoadAd();
        }

        /// <summary>
        /// 広告を読み込みます。
        /// </summary>
        public void LoadAd()
        {
            // 古い広告を読み込む前にクリーンアップします。
            if (_interstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Inste Wo Yomikondeimasu !!!!!!!!!!!!");

            // 広告を読み込むためのリクエストを作成します。
            var adRequest = new AdRequest();

            // 広告を読み込むリクエストを送信します。
            InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                // エラーがある場合は処理を中断します。
                if (error != null)
                {
                    Debug.LogError("インタースティシャル広告の読み込みに失敗しました : " + error);
                    Debug.LogError("InterStitial Ad Field Error");

                    //とりあえず
                    //falseからfalseで通知しても反応しないので
                    //isReady.SetValueAndForceNotify(false);

                    //この2行はしゃーなし。ネットおふにしてるのと、ストック切れの場合わけの仕方がわからんからとりあえずどっちも許すからこうした。本当は上の感じにしたかった
                    isReady.Value = true;
                    isSkipAd = true;
                    return;
                }
                // 広告がnullの場合はエラーを出力します。
                if (ad == null)
                {
                    Debug.LogError("予期しないエラー：nullの広告とnullのエラーが発生しました。");
                    Debug.LogError("Yokisenu Error null no kouykoku to null error");

                    isReady.Value = true;
                    isSkipAd = true;
                    return;
                }

                // 広告の読み込みが成功した場合の処理です。
                Debug.Log("インタースティシャル広告が読み込まれました : " + ad.GetResponseInfo());
                Debug.Log("Insute Yomikomi Seikou!!!! : " + ad.GetResponseInfo());

                _interstitialAd = ad;

                // 広告のイベントハンドラーを登録します。
                RegisterEventHandlers(ad);

                isReady.Value = true;
                // UIに広告が読み込まれたことを通知します。
                //AdLoadedStatus?.SetActive(true);
            });
        }

        /// <summary>
        /// 広告を表示します。
        /// </summary>
        public void ShowAd()
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log("インタースティシャル広告を表示しています。");
                _interstitialAd.Show();
            }
            else
            {
                Debug.LogError("インタースティシャル広告はまだ準備ができていません。");
                Debug.LogError("InterStitial Ad Ha Mada Junbigadekiteimasei");

            }

            // UIに広告が準備できていないことを通知します。
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// 広告を破棄します。
        /// </summary>
        public void DestroyAd()
        {
            if (_interstitialAd != null)
            {
                Debug.Log("インタースティシャル広告を破棄しています。");
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }

            // UIに広告が準備できていないことを通知します。
            //AdLoadedStatus?.SetActive(false);
        }

        /// <summary>
        /// ResponseInfoをログに記録します。
        /// </summary>
        public void LogResponseInfo()
        {
            if (_interstitialAd != null)
            {
                var responseInfo = _interstitialAd.GetResponseInfo();
                UnityEngine.Debug.Log(responseInfo);
            }
        }

        private void RegisterEventHandlers(InterstitialAd ad)
        {
            // 広告が収益を得ると推定されたときに発生します。
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("インタースティシャル広告が{0} {1}を支払いました。",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // 広告のインプレッションが記録されたときに発生します。
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("インタースティシャル広告がインプレッションを記録しました。");
            };
            // 広告がクリックされたときに発生します。
            ad.OnAdClicked += () =>
            {
                Debug.Log("インタースティシャル広告がクリックされました。");
                isClickAd = true;
                isAdClosed.Value = true;
            };
            // 広告がフルスクリーンコンテンツを開いたときに発生します。
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("インタースティシャル広告がフルスクリーンコンテンツを開きました。");
            };
            // 広告がフルスクリーンコンテンツを閉じたときに発生します。
            ad.OnAdFullScreenContentClosed += () =>
            {
                isAdClosed.Value = true;
                Debug.Log("インタースティシャル広告がフルスクリーンコンテンツを閉じました。");
            };
            // 広告がフルスクリーンコンテンツを開けなかったときに発生します。
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                isAdClosed.Value = true;
                Debug.LogError("インタースティシャル広告がフルスクリーンコンテンツを開けませんでした : "
                    + error);
            };
        }
    }
}
