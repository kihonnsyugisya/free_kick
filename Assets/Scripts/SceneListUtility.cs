using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// ビルド設定に登録されているシーンの中から、
/// 「Stage + 数字」のシーン名を取得して、数字順に並べるユーティリティクラス
/// </summary>
public static class SceneListUtility
{
    /// <summary>
    /// ビルド設定に登録されたシーンのうち、
    /// 名前が "Stage" + 数字 の形式になっているものを昇順に並べてリストとして返す
    /// </summary>
    /// <returns>ステージシーンのリスト（例: Stage1, Stage2, Stage10）</returns>
    public static List<string> GetStageSceneNamesInBuildSettings()
    {
        List<string> stageScenes = new List<string>(); // ステージシーンを格納するリスト

        // ビルド設定に登録されているシーンの数を取得
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            // シーンのパスを取得（例: "Assets/Scenes/Stage1.unity"）
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

            // パスからファイル名（拡張子なしのシーン名）を取得（例: "Stage1"）
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            // "Stage" で始まっているかチェック
            if (sceneName.StartsWith("Stage"))
            {
                // "Stage" の後ろの部分（数字）を取得
                string suffix = sceneName.Substring("Stage".Length);

                // 数字部分だけかどうかを判定
                int dummy;
                if (int.TryParse(suffix, out dummy))
                {
                    stageScenes.Add(sceneName); // リストに追加
                }
            }
        }

        // 数字部分で昇順にソート
        stageScenes.Sort((a, b) =>
        {
            int numA = int.Parse(a.Substring("Stage".Length)); // "Stage1" → 1
            int numB = int.Parse(b.Substring("Stage".Length)); // "Stage10" → 10
            return numA.CompareTo(numB); // 昇順ソート
        });

        return stageScenes; // ソート済みリストを返す
    }



    /// <summary>
    /// 現在のシーン名が "Stage" + 数字 の形式になっていると仮定し、
    /// 数字部分を＋１したシーンをロードします。
    /// 例: "Stage3" なら "Stage4" をロードする。
    /// </summary>
    public static void LoadNextStage()
    {
        // 現在のシーン名を取得
        string currentSceneName = SceneManager.GetActiveScene().name;
        string prefix = "Stage";

        // シーン名が "Stage" で始まっているかチェック
        if (currentSceneName.StartsWith(prefix))
        {
            // "Stage" の後ろの数字部分を取得
            string numberPart = currentSceneName.Substring(prefix.Length);

            int currentStageNumber;
            if (int.TryParse(numberPart, out currentStageNumber))
            {
                // 次のステージ番号を計算
                int nextStageNumber = currentStageNumber + 1;
                string nextSceneName = prefix + nextStageNumber.ToString();

                // 次のシーンをロードする
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogError("現在のシーン名の数字部分をパースできませんでした: " + currentSceneName);
            }
        }
        else
        {
            Debug.LogError("現在のシーン名は 'Stage' で始まっていません: " + currentSceneName);
        }
    }

}
