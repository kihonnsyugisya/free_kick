using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class ReplayCamera : MonoBehaviour
{
    public Camera cameraToRecord; // 録画するカメラ
    public string fileName = "replay.mp4"; // 録画ファイル名
    
    private VideoPlayer videoPlayer; // VideoPlayerコンポーネント
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    void Start()
    {
        // RenderTextureの初期設定
        renderTexture = new RenderTexture(1920, 1080, 24); // 解像度設定
        cameraToRecord.targetTexture = renderTexture;
        screenShot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        // VideoPlayerコンポーネントがなければ追加
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }
    }

    // 録画を開始するメソッド
    public void RecordShot()
    {
        // 録画ファイルが既に存在すれば削除
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Existing file deleted: " + filePath);
        }

        // 画面をRenderTextureにレンダリング
        cameraToRecord.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        RenderTexture.active = null;

        // PNG形式で保存
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Recording saved to: " + filePath);
    }

    // 録画ファイルのパスを返す
    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    // 録画したデータを再生するメソッド
    public void PlayReplay()
    {
        // VideoPlayerが設定されていれば再生
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            videoPlayer.url = filePath;
            videoPlayer.Play();
            Debug.Log("Playing replay from: " + filePath);
        }
        else
        {
            Debug.LogWarning("Replay file not found at: " + filePath);
        }
    }
}
