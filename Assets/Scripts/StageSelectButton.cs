using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // DOTweenを使用

public class StageSelectButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonLabel;
    public TextMeshProUGUI statusText;

    [SerializeField] private float blinkDuration = 1.5f; // 🔥 点滅速度（Inspector で変更可能）

    private Tween statusTween;

    /// <summary>
    /// ステータスの点滅を開始する
    /// </summary>
    public void StartBlinkingStatusText()
    {
        statusTween?.Kill();

        statusTween = statusText.DOFade(0f, blinkDuration) // 👈 設定した点滅速度を使用
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    private void OnDestroy()
    {
        statusTween?.Kill();
    }
}
