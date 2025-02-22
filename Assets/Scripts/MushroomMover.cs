using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MushroomMover : ReplayableObject
{
    [Header("移動設定")]
    [Tooltip("キノコが移動する最低の高さ")]
    [SerializeField] private float minHeight = -1.0f;

    [Tooltip("キノコが移動する最高の高さ")]
    [SerializeField] private float maxHeight = 1.0f;

    [Tooltip("キノコの移動周期（秒）")]
    [SerializeField] private float moveCycle = 2.0f;

    private Dictionary<Transform, float> initialYPositions = new();
    private Sequence sequence;


    protected override void Start()
    {
        base.Start(); // 親クラスの Start() を呼ぶ（必要なら）

        foreach (Transform mushroom in transform)
        {
            initialYPositions[mushroom] = mushroom.position.y;
            StartRhythmicMovement(mushroom);
        }
    }

    private void OnDisable()
    {
        // Tween破棄
        if (DOTween.instance != null)
        {
            sequence?.Kill();
        }
    }

    /// <summary>
    /// キックボタンが押されたら記録
    /// </summary>
    protected override void OnKick()
    {
        base.OnKick(); // 位置記録
    }

    /// <summary>
    /// 指定されたキノコを一定のリズムで上下運動
    /// </summary>
    private void StartRhythmicMovement(Transform mushroom)
    {
        if (!initialYPositions.ContainsKey(mushroom)) return;

        sequence = DOTween.Sequence();
        sequence.Append(mushroom.DOMoveY(maxHeight, moveCycle / 2f).SetEase(Ease.InOutSine));
        sequence.Append(mushroom.DOMoveY(minHeight, moveCycle / 2f).SetEase(Ease.InOutSine));
        sequence.SetLoops(-1);
    }

    /// <summary>
    /// 記録した動きを再現
    /// </summary>
    public override void Replay()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform mushroom = transform.GetChild(i);

            // 破棄されていたらスキップ
            if (mushroom == null || !mushroom.gameObject.activeInHierarchy || i >= recordedPositions.Count)
            {
                continue;
            }

            // 以前の Tween を削除
            mushroom.DOKill();

            // DOTween で移動
            mushroom.DOMoveY(recordedPositions[i].y, 0.5f).SetEase(Ease.OutQuad);
        }

    }
}
