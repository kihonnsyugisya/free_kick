using DG.Tweening;
using UnityEngine;

public class IrisCanvas : MonoBehaviour
{
    [SerializeField] RectTransform unmask;
    readonly Vector2 IRIS_IN_SCALE = new Vector2(80, 50);
    readonly float SCALE_DURATION = 1;

    /// <summary>
    /// アイリスを開く（スケールを大きくする）
    /// </summary>
    private void IrisIn()
    {
        unmask.DOScale(IRIS_IN_SCALE, SCALE_DURATION)
              .SetEase(Ease.InCubic);
    }

    /// <summary>
    /// アイリスを閉じる（スケールをゼロにする）
    /// </summary>
    private void IrisOut()
    {
        unmask.DOScale(Vector2.zero, SCALE_DURATION)
              .SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// オブジェクトが有効化されたとき（SetActive(true)）にアイリスを開く
    /// </summary>
    private void OnEnable()
    {
        IrisIn();
    }

    /// <summary>
    /// オブジェクトが無効化されたとき（SetActive(false)）にアイリスを閉じる
    /// </summary>
    private void OnDisable()
    {
        IrisOut();
    }
}
