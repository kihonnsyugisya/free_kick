using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// セーブ・ロード処理をまとめたユーティリティクラス\n\n※Staticメソッドを使用して、ゲームデータの保存や読み込みを行います。\nPlayerPrefs を利用して、最後に到達したステージ名を保存します。\n</summary>
public static class SaveLoadManager
{
    /// <summary>
    /// 最後に到達したpurefixを保存します。
    /// </summary>
    public static void SaveStagePrefix(StagePrefix currentStagePrefix, SaveStatus saveStatus)
    {
        // PlayerPrefs に保存
        PlayerPrefs.SetInt(currentStagePrefix.ToString(), (int)saveStatus);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 最後に到達したステージ名を読み込みます。
    /// キーが存在しない場合はデフォルトで false を返します。
    /// </summary>
    /// <returns>最後に保存されたステージ名が存在した場合はtrue、存在しない場合はfalse</returns>
    public static int LoadStagePrefix(StagePrefix loadPrefix)
    {
        // プレイヤープリファレンスから整数を取得（デフォルト値としてSaveStatus.LOCKの整数値を使用）
        return PlayerPrefs.GetInt(loadPrefix.ToString(), (int)SaveStatus.LOCK);
    }
}

public enum SaveStatus
{ 
    NEW = 0,
    CLEAR = 1,
    NON_CLEAR = 2,
    LOCK = 3
}