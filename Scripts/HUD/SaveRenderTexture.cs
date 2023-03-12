using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveRenderTexture : MonoBehaviour
{
    [SerializeField]
    private string _saveDirectory;
    [SerializeField]
    private RenderTexture _target;

    private int _captureIndex;


    public void OnPress(InputAction.CallbackContext content)
    {
        if (!content.performed)
        {
            return;
        }

        Save();
    }

    public void SaveAs(string path)
    {
        var texture = new Texture2D(_target.width, _target.height, TextureFormat.RGB24, false);
        RenderTexture.active = _target;
        texture.ReadPixels(new Rect(0, 0, _target.width, _target.height), 0, 0);
        texture.Apply();

        var bytes = texture.EncodeToPNG();
        Destroy(texture);

        File.WriteAllBytes(path, bytes);
    }
        
    public void Save()
    {
        var path = Path.Combine(_saveDirectory, $"capture_{_captureIndex:000}.png");
        ++_captureIndex;
        SaveAs(path);
    }

    public int CaptureCount => _captureIndex;
}
