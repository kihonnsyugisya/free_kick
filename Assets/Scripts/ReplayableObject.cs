using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UniRx;
using System.Threading.Tasks;

public abstract class ReplayableObject : MonoBehaviour
{
    [Header("リプレイ機能")]
    [Tooltip("キックボタン (Inspector から設定)")]
    public Button kickButton;

    [Tooltip("衝突を受け取るクラス (外部からセット)")]
    public List<CollisionReciver> goalObjs;

    protected List<Vector3> recordedPositions = new(); // 記録した位置
    protected bool isRecording = false; // 記録中かどうか

    /// <summary>
    /// Start でボタンのクリックを監視
    /// </summary>
    protected virtual void Start()
    {
        if (kickButton != null)
        {
            kickButton.onClick.AddListener(OnKick);
        }
    }

    /// <summary>
    /// キックされた瞬間に記録を開始する
    /// </summary>
    protected virtual void OnKick()
    {
        recordedPositions.Clear();
        if (transform.childCount == 0)
        {
            Debug.LogWarning("子供オブジェクトがいないので機能しません：　俺の名は" + gameObject.name);
            return;
        }
        foreach (Transform child in transform)
        {
            Vector3 pos = child.position;
            recordedPositions.Add(pos);
        }
    }

    /// <summary>
    /// 記録した動きを再現する（継承先で実装）
    /// </summary>
    public abstract void Replay();
}
