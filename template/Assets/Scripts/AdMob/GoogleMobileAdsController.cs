using System;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

#if UNITY_IPHONE
using UnityEngine.iOS;
#endif

/// <summary>
/// Google Mobile Ads Unityプラグインの使用方法を示すクラス。
/// </summary>
namespace GoogleMobileAds.Samples
{
    [AddComponentMenu("GoogleMobileAds/Samples/GoogleMobileAdsController")]
    public class GoogleMobileAdsController : MonoBehaviour
    {
        // テスト広告のみを使用するテストデバイスIDのリスト。
        // https://developers.google.com/admob/unity/test-ads
        internal static List<string> TestDeviceIds = new List<string>()
        {
            AdRequest.TestDeviceSimulator,
#if UNITY_IPHONE
            //"ここは自分の実機のデバイスIDをログに出してそれを入れないといけない。",
            "3415EAA4-8DE1-4218-AF28-328C4A7AAFEE", //iphone 12mini
            "6709FEBF-DAF5-4FEA-80D3-583F57D3F32D", //iphoneSE
            "55F2676C-FF4A-4EC2-903B-1220188E3D5F",
#elif UNITY_ANDROID
            //"A4B4C5C3-CFA1-5623-BD1C-6758CF40B49D" //mac
            "C6ED4A79D46C41CE28E0D6893A968161" //pixel8a
#endif
        };

        // Google Mobile Ads Unityプラグインは1回だけ実行する必要がある。
        public static bool? _isInitialized;

        // Google User Messaging Platform (UMP) Unityプラグインを使用して同意を実装するためのヘルパークラス。
        [SerializeField, Tooltip("Google User Messaging Platform (UMP) Unityプラグインのコントローラー。")]
        private GoogleMobileAdsConsentController _consentController;

#if UNITY_IPHONE
        private IDFA _idfa;
#endif

        /// <summary>
        /// Google Mobile Ads Unityプラグインを構成する方法を示すメソッド。
        /// </summary>
        private async void Start()
        {

            //デバッグ用に端末のID取得
            //================================================================
            // 端末固有のIDを取得
            string deviceID = SystemInfo.deviceUniqueIdentifier;

            // 取得したIDを全て大文字に変換(大文字にしないとデバッグができません)
            string deviceIDUpperCase = deviceID.ToUpper();

            // 端末IDの表示
            Debug.LogWarning("TestDeviceHashedId = " + deviceIDUpperCase);
            //================================================================



            // Androidでは、インタースティシャル広告またはリワード動画を表示するとUnityが一時停止する。
            // この設定により、iOSもAndroidと同様に動作する。
            MobileAds.SetiOSAppPauseOnBackground(true);

            // trueにすると、GoogleMobileAdsが発生するすべてのイベントがUnityのメインスレッドで発生する。
            // デフォルト値はfalse。
            // https://developers.google.com/admob/unity/quick-start#raise_ad_events_on_the_unity_main_thread
            MobileAds.RaiseAdEventsOnUnityMainThread = true;

            //// Child Directed TreatmentとテストデバイスIDでリクエスト構成を構成する。
            //MobileAds.SetRequestConfiguration(new RequestConfiguration
            //{
            //    TestDeviceIds = TestDeviceIds
            //});

#if UNITY_IOS
            // ATTトラッキング許可ダイアログを表示する（iOSのみ）
            _idfa = new IDFA();
            await _idfa.RequestAsync();
#endif

            // 広告をリクエストできる場合は、Google Mobile Ads Unityプラグインを初期化する。
            if (_consentController.CanRequestAds)
            {
                InitializeGoogleMobileAds();
            }

#if UNITY_IOS
            // ATTでユーザが許可を選択していた場合のみ（iOSの場合）
            if (_idfa.isAuth())
            {
                // プライバシーと同意情報が最新であることを保証する。(UMPでダイアログ出すやつ)
                InitializeGoogleMobileAdsConsent();
            }
#else
            // Androidでは常にUMPを表示する
            InitializeGoogleMobileAdsConsent();
#endif


        }

        /// <summary>
        /// プライバシーと同意情報が最新であることを保証するメソッド。
        /// </summary>
        private void InitializeGoogleMobileAdsConsent()
        {
            Debug.Log("Google Mobile Adsの同意情報を収集中。");

            _consentController.GatherConsent((string error) =>
            {
                if (error != null)
                {
                    Debug.LogError("同意情報の収集に失敗しました。エラー: " + error);

                }
                else
                {
                    Debug.Log("Google Mobile Adsの同意情報が更新されました: " + ConsentInformation.ConsentStatus);
                }

                if (_consentController.CanRequestAds)
                {
                    InitializeGoogleMobileAds();
                }
            });
        }

        /// <summary>
        /// Google Mobile Ads Unityプラグインを初期化するメソッド。
        /// </summary>
        private void InitializeGoogleMobileAds()
        {
            // Google Mobile Ads Unityプラグインは1回だけ実行し、広告を読み込む前に実行する必要がある。
            if (_isInitialized.HasValue)
            {
                return;
            }

            _isInitialized = false;

            // Google Mobile Ads Unityプラグインを初期化する。
            Debug.Log("Google Mobile Adsを初期化中。");
            MobileAds.Initialize((InitializationStatus initstatus) =>
            {
                if (initstatus == null)
                {
                    Debug.LogError("Google Mobile Adsの初期化に失敗しました。");

                    _isInitialized = null;
                    return;
                }

                // メディエーションを使用する場合、各アダプターの状態を確認できる。
                var adapterStatusMap = initstatus.getAdapterStatusMap();
                if (adapterStatusMap != null)
                {
                    foreach (var item in adapterStatusMap)
                    {
                        Debug.Log(string.Format("Adapter {0} is {1}",
                            item.Key,
                            item.Value.InitializationState));
                    }
                }

                Debug.Log("Google Mobile Adsの初期化が完了しました。");
                _isInitialized = true;
            });
        }

        /// <summary>
        /// AdInspectorを開くメソッド。
        /// </summary>
        public void OpenAdInspector()
        {
            Debug.Log("Ad Inspectorを開いています。");
            MobileAds.OpenAdInspector((AdInspectorError error) =>
            {
                // 操作に失敗した場合、エラーが返される。
                if (error != null)
                {
                    Debug.Log("Ad Inspectorのオープンに失敗しました。エラー: " + error);
                    return;
                }

                Debug.Log("Ad Inspectorを正常に開きました。");
            });
        }

        /// <summary>
        /// ユーザーのプライバシーオプションフォームを開くメソッド。
        /// </summary>
        /// <remarks>
        /// アプリはユーザーがいつでも同意ステータスを変更できるようにする必要がある。
        /// </remarks>
        public void OpenPrivacyOptions()
        {
            _consentController.ShowPrivacyOptionsForm((string error) =>
            {
                if (error != null)
                {
                    Debug.LogError("同意プライバシーフォームの表示に失敗しました。エラー: " + error);
                }
                else
                {
                    Debug.Log("プライバシーフォームが正常に開かれました。");
                }
            });
        }
    }
}
