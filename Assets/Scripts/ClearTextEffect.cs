using DG.Tweening;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class ClearTextEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clearText; // 表示するテキスト
    [SerializeField] private float dropHeight = 100f; // 初期位置（画面外）
    [SerializeField] private float dropDuration = 0.8f; // 落ちる時間
    [SerializeField] private float bounceStrength = 30f; // バウンドの強さ
    [SerializeField] private float bounceDuration = 0.3f; // バウンド時間

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = clearText.transform.position;
        clearText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 指定した文字を一文字ずつポップさせながら表示する
    /// </summary>
    /// <param name="message">表示する文字列</param>
    /// <returns>すべての文字が表示されるまで待機するTask</returns>
    public async Task ShowClearText(string message)
    {
        await AnimateText(message);
    }

    /// <summary>
    /// 一文字ずつポップさせながらテキストを表示する
    /// </summary>
    /// <param name="message">表示する文字列</param>
    private async Task AnimateText(string message)
    {
        clearText.text = ""; // テキストを空にする
        clearText.gameObject.SetActive(true);

        for (int i = 0; i < message.Length; i++)
        {
            clearText.text += message[i]; // 1文字ずつ追加
            clearText.transform.DOPunchScale(Vector3.one * 0.4f, 0.2f); // 文字のポップアニメーション
            await Task.Delay(200); // 次の文字までの待機時間
        }
    }

    /// <summary>
    /// 上から降ってきてバウンドするアニメーション付きで表示
    /// </summary>
    /// <param name="message">表示する文字列</param>
    public async Task ShowClearTextWithBounce(string message)
    {
        clearText.text = message;
        clearText.gameObject.SetActive(true);

        // 画面外の位置からスタート
        clearText.transform.position = originalPosition + Vector3.up * dropHeight;

        // 落下 & バウンド
        await clearText.transform
            .DOJump(originalPosition, bounceStrength, 1, dropDuration + bounceDuration)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();
    }
}
