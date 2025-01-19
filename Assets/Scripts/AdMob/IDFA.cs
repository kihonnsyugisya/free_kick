#if UNITY_IOS

using System;
using System.Threading.Tasks;
using Unity.Advertisement.IosSupport;
using UnityEngine;
using UnityEngine.iOS;

/// <summary>
/// IDFA（広告ID）を取得するためのクラス。
/// トラッキング許可ダイアログを表示し、ユーザーの選択に基づいてIDFAを取得します。
/// 許可ダイアログが表示されたことがない場合は、ダイアログを表示し、選択が完了するまで同期的に待機します。
/// </summary>
public class IDFA
{
    /// <summary>
    /// トラッキング許可ダイアログを表示し、選択が完了するまで待機してからIDFAを取得します。
    /// 許可されていない場合は「00000000-0000-0000-0000-000000000000」を返します。
    /// </summary>
    /// <returns>IDFA（広告ID）。許可がされていない場合は「00000000-0000-0000-0000-000000000000」を返します。</returns>
    public async Task<string> RequestAsync()
    {
        // トラッキングの許可状態を確認し、まだ許可ダイアログが表示されていない場合
        var currentStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        Debug.Log("現在のトラッキング許可状態: " + currentStatus);

        if (currentStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            Debug.Log("トラッキング許可ダイアログを表示します。");
            // 許可ダイアログを表示し、ユーザーが選択するまで待機
            await RequestTrackingAuthorizationAsync();
        }
        else
        {
            Debug.Log("トラッキング許可ダイアログはすでに表示済みです。");
        }

        // 許可状態に基づいてIDFA（広告ID）を取得
        string idfa = Device.advertisingIdentifier;
        Debug.Log("IDFA: " + idfa);

        return idfa;
    }

    /// <summary>
    /// トラッキング許可ダイアログを非同期で表示し、ユーザーの選択が完了するまで待機します。
    /// </summary>
    /// <returns>許可ダイアログの結果が得られるとTaskが完了します。</returns>
    private Task RequestTrackingAuthorizationAsync()
    {
        var tcs = new TaskCompletionSource<bool>();

        // 許可ダイアログを表示
        Debug.Log("許可ダイアログを表示中...");
        ATTrackingStatusBinding.RequestAuthorizationTracking();

        // コールバックでタスクを完了させる
        Action checkAuthorizationStatus = null;
        checkAuthorizationStatus = () =>
        {
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            Debug.Log("トラッキング許可状態のチェック: " + status);

            if (status != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                tcs.SetResult(true);
                Debug.Log("トラッキング許可ダイアログが処理されました。");
                // コールバックからの解除
                Application.quitting -= checkAuthorizationStatus;
            }
        };

        // アプリケーションの状態変更イベントにコールバックを登録
        Application.quitting += checkAuthorizationStatus;

        return tcs.Task;
    }

    /// <summary>
    /// attを許可したか確認する
    /// </summary>
    /// <returns></returns>
    public bool isAuth()
    {
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED) return true;
        else return false;
    }
}

#endif
