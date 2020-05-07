using System.Collections;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

public class ScreenShot : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SaveToAlbum(string path);
    [SerializeField] RenderTexture _renderTexture;

    IEnumerator SaveToCameraroll(string path)
    {
        // ファイルが生成されるまで待つ
        while (true)
        {
            if (File.Exists(path)) break;
            yield return null;
        }
        
        SaveToAlbum(path);
    }

    public void TakeShot()
    {
#if UNITY_EDITOR
#elif PLATFORM_IOS
        SaveScreenCapture();
        SaveRenderTexture();
#endif
    }

    private void SaveScreenCapture()
    {
        string filename = "screen.png";
        string path = Application.persistentDataPath + "/" + filename;

        // 以前のスクリーンショットを削除する
        File.Delete(path);
        // スクリーンショットを撮影する
        ScreenCapture.CaptureScreenshot(filename);
        // カメラロールに保存する
        StartCoroutine(SaveToCameraroll(path));
    }

    private void SaveRenderTexture()
    {
        string filename = "renderTexture.png";
        string path = Application.persistentDataPath + "/" + filename;
        File.Delete(path);
        Texture2D tex = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = _renderTexture;
        tex.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
        // ガンマ補正
        var color = tex.GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            color[i].r = Mathf.Pow(color[i].r, 1 / 2.2f);
            color[i].g = Mathf.Pow(color[i].g, 1 / 2.2f);
            color[i].b = Mathf.Pow(color[i].b, 1 / 2.2f);
        }
        tex.SetPixels(color);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        StartCoroutine(SaveToCameraroll(path));
    }
}