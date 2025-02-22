using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// セーブ・ロード処理をまとめたユーティリティクラス\n\n※Staticメソッドを使用して、ゲームデータの保存や読み込みを行います。\nPlayerPrefs を利用して、最後に到達したステージ名を保存します。\n</summary>
public static class SaveLoadManager
{
    // セーブ・ロードに使用するキーを定義
    private const string LastStageKey = "LastStage";

    /// <summary>
    /// 最後に到達したステージ名を保存します。
    /// ※例: "Stage3" を保存する
    /// </summary>
    public static void SaveLastStage(string currentStage)
    {
        // PlayerPrefs に保存
        PlayerPrefs.SetString(LastStageKey, currentStage);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 最後に到達したステージ名を読み込みます。
    /// キーが存在しない場合はデフォルトで \"TutorialScene\" を返します。
    /// </summary>
    /// <returns>最後に保存されたステージ名</returns>
    public static string LoadLastStage()
    {
        return PlayerPrefs.GetString(LastStageKey, "TutorialScene");
    }
}
