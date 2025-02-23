using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// セーブ・ロード処理をまとめたユーティリティクラス\n\n※Staticメソッドを使用して、ゲームデータの保存や読み込みを行います。\nPlayerPrefs を利用して、最後に到達したステージ名を保存します。\n</summary>
public static class SaveLoadManager
{
    /// <summary>
    /// 最後に到達したpurefixを保存します。
    /// </summary>
    public static void SaveStagePrefix(StagePrefix currentStagePrefix)
    {
        // PlayerPrefs に保存
        PlayerPrefs.SetString(currentStagePrefix.ToString(), currentStagePrefix.ToString());
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 最後に到達したステージ名を読み込みます。
    /// キーが存在しない場合はデフォルトで false を返します。
    /// </summary>
    /// <returns>最後に保存されたステージ名が存在した場合はtrue、存在しない場合はfalse</returns>
    public static bool LoadStagefPrefix(StagePrefix loadPrefix)
    {
        string stage = PlayerPrefs.GetString(loadPrefix.ToString(), "");
        return !string.IsNullOrEmpty(stage);
    }

}
