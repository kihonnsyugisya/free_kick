using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ハートの画像をドキドキさせるアニメーションを管理するクラス。
/// Image がアクティブな間はスケールを変化させ、非表示時にはアニメーションを停止する。
/// スケールの変化量や速度は Inspector から調整可能。
/// </summary>
public class HeartBeat : MonoBehaviour
{
    private Image heartImage;
    private Tween heartbeatTween;

    [Header("鼓動の設定")]
    [SerializeField] private float minScale = 1.0f;  // 最小スケール（デフォルトは1.0）
    [SerializeField] private float maxScale = 1.2f;  // 最大スケール（デフォルトは1.2）
    [SerializeField] private float duration = 1f;  // 1回の鼓動時間（デフォルトは0.5秒）

    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartHeartbeat();
    }

    private void OnDisable()
    {
        StopHeartbeat();
    }

    /// <summary>
    /// ハートのドキドキアニメーションを開始する。
    /// </summary>
    private void StartHeartbeat()
    {
        // すでにTweenがあるならリセット
        heartbeatTween?.Kill();

        // ドクドクさせるTween
        heartbeatTween = transform.DOScale(maxScale, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// ハートのドキドキアニメーションを停止し、元のサイズに戻す。
    /// </summary>
    private void StopHeartbeat()
    {
        heartbeatTween?.Kill();
        transform.localScale = Vector3.one; // 元のサイズに戻す
    }
}
