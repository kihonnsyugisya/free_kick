using GoogleMobileAds.Ump.Api;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// Google User Messaging Platform (UMP) SDKを使用した同意を実装するヘルパークラス。
/// </summary>
namespace GoogleMobileAds.Samples
{
    public class GoogleMobileAdsConsentController : MonoBehaviour
    {
        /// <summary>
        /// trueの場合、MobileAds.Initialize()と広告のロードを安全に呼び出すことができます。
        /// </summary>
        public bool CanRequestAds => ConsentInformation.CanRequestAds();

        [SerializeField, Tooltip("ユーザーの同意とプライバシー設定を表示するボタン。")]
        private Button _privacyButton;

        [SerializeField, Tooltip("エラーポップアップを持つGameObject。")]
        private GameObject _errorPopup;

        [SerializeField, Tooltip("エラーポップアップ用のエラーメッセージ。")]
        private Text _errorText;

        public static BoolReactiveProperty isConsentFormVisible = new();

        private void Start()
        {
            // プライバシー設定ボタンを無効化します。
            if (_privacyButton != null)
            {
                _privacyButton.interactable = false;
            }
            // エラーポップアップを無効化します。
            if (_errorPopup != null)
            {
                _errorPopup.SetActive(false);
            }
        }

        /// <summary>
        /// Google User Messaging Platform (UMP) SDKの起動メソッド
        /// 必要なすべての更新をロードし、必要なフォームを表示します。
        /// </summary>
        public void GatherConsent(Action<string> onComplete)
        {   
            Debug.Log("同意を取得中。");
            // 未成年者の同意に関するタグを設定します。
            // falseはユーザーが未成年者でないことを意味します。

            var requestParameters = new ConsentRequestParameters
            {
                // Falseは、ユーザーが未成年でないことを意味します。
                TagForUnderAgeOfConsent = false,

                //ToDo ここはテスト用のコードっぽいのでリリース時に消したほうが良さそう
                //ConsentDebugSettings = new ConsentDebugSettings
                //{
                //    // 地理学による同意設定のデバッグ用。
                //    DebugGeography = DebugGeography.EEA,
                //    // https://developers.google.com/admob/unity/test-ads
                //    TestDeviceHashedIds = GoogleMobileAdsController.TestDeviceIds,
                //}
            };

            // コールバックをエラーポップアップハンドラーと組み合わせます。
            onComplete = (onComplete == null)
                ? UpdateErrorPopup
                : onComplete + UpdateErrorPopup;

            // Google Mobile Ads SDKはUser Messaging Platform (GoogleのIAB認定同意管理プラットフォーム)を提供しています。
            // GDPR対象国のユーザーの同意を取得するためのソリューションの1つとして使用します。
            ConsentInformation.Update(requestParameters, (FormError updateError) =>
            {
                // プライバシー設定ボタンを有効化します。
                UpdatePrivacyButton();

                if (updateError != null)
                {
                    onComplete(updateError.Message);
                    //Debug.LogWarning("111111111111");
                    return;
                }

                // ConsentStatusに基づいて同意関連のアクションを決定します。
                if (CanRequestAds)
                {
                    // 同意が既に取得されているか、必要ではない場合。
                    // ユーザーにコントロールを戻します。
                    onComplete(null);
                    //Debug.LogWarning("22222222222222");

                    return;
                }


                isConsentFormVisible.Value = true;

                // 同意が取得されておらず、必要な場合。
                // ユーザーの初回同意リクエストフォームをロードします。
                ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
                {
                    isConsentFormVisible.Value = false;
                    UpdatePrivacyButton();
                    if (showError != null)
                    {
                        //Debug.LogWarning("333333333");

                        // フォームの表示に失敗しました。
                        if (onComplete != null)
                        {
                            onComplete(showError.Message);
                            //Debug.LogWarning("444444444");

                        }
                    }
                    // フォームの表示が成功しました。
                    else if (onComplete != null)
                    {
                        onComplete(null);
                        //Debug.LogWarning("555555555555");

                    }
                });
            });
        }

        /// <summary>
        /// ユーザーにプライバシーオプションフォームを表示します。
        /// </summary>
        /// <remarks>
        /// アプリはユーザーにいつでも同意ステータスを変更することを許可する必要があります。
        /// 別のフォームをロードして保存し、ユーザーに同意ステータスを変更する機会を提供します。
        /// </remarks>
        public void ShowPrivacyOptionsForm(Action<string> onComplete)
        {
            Debug.Log("プライバシーオプションフォームを表示中。");

            // コールバックをエラーポップアップハンドラーと組み合わせます。
            onComplete = (onComplete == null)
                ? UpdateErrorPopup
                : onComplete + UpdateErrorPopup;

            ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
            {
                UpdatePrivacyButton();
                if (showError != null)
                {
                    // フォームの表示に失敗しました。
                    if (onComplete != null)
                    {
                        onComplete(showError.Message);
                    }
                }
                // フォームの表示が成功しました。
                else if (onComplete != null)
                {
                    onComplete(null);
                }
            });
        }

        /// <summary>
        /// ユーザーの同意情報をリセットします。
        /// </summary>
        public void ResetConsentInformation()
        {
            ConsentInformation.Reset();
            UpdatePrivacyButton();
        }

        void UpdatePrivacyButton()
        {
            if (_privacyButton != null)
            {
                //_privacyButton.interactable =
                //    ConsentInformation.PrivacyOptionsRequirementStatus ==
                //        PrivacyOptionsRequirementStatus.Required;
                _privacyButton.gameObject.SetActive(ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required);
            }
        }

        void UpdateErrorPopup(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (_errorText != null)
            {
                _errorText.text = message;
            }

            if (_errorPopup != null)
            {
                _errorPopup.SetActive(true);
            }
        }
    }
}
