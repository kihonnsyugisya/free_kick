using UnityEngine;

public class LifeManager : MonoBehaviour
{
    private const int MAX_LIFE = 3; // 初期ライフ
    public static int life = MAX_LIFE;

    private GameObject[] hearts; // 子オブジェクトのハート（GameObject）

    private void Awake()
    {
        // 子オブジェクトをすべて取得
        hearts = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            hearts[i] = transform.GetChild(i).gameObject;
        }

        UpdateHeartDisplay();
    }

    /// <summary>
    /// ライフを全回復する
    /// </summary>
    public void HealFullLife()
    {
        life = MAX_LIFE;
        UpdateHeartDisplay();
    }

    /// <summary>
    /// ライフを1つ減らす
    /// </summary>
    /// <returns>減らした後のライフ</returns>
    public int ReduceLife()
    {
        if (life > 0)
        {
            life--;
            UpdateHeartDisplay();
        }
        return life;
    }

    /// <summary>
    /// ハートの表示をライフに合わせて更新
    /// </summary>
    private void UpdateHeartDisplay()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < life);
        }
    }
}
