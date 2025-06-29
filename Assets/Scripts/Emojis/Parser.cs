using System;
using System.Runtime.InteropServices;
using SharpFont;
using TMPro;
using UnityEngine;

namespace Scripts.Emojis
{
    public class Parser : MonoBehaviour
    {
        [SerializeField]
        private string path;
        
        [SerializeField]
        private Texture2D texture;

        [SerializeField]
        private TMP_FontAsset fontAsset;

        private void Awake()
        {   
            var library = new Library();
            var font = new Face(library, path);

            font.SetPixelSizes(160, 160);
            font.LoadChar(0x1F600, LoadFlags.Color, LoadTarget.VerticalLcd);

            var glyph = font.Glyph;

            texture = new Texture2D(160, 160, TextureFormat.RGBA32,
                false);

            var bitmap = glyph.Bitmap;
            var pixelData = new byte[bitmap.Width * bitmap.Rows * 4];
            
            Marshal.Copy(bitmap.Buffer, pixelData, 0, pixelData.Length);
            Array.Reverse(pixelData);

            var pixels = new Color32[bitmap.Width * bitmap.Rows];
            
            for (var i = 0; i < pixels.Length; i++)
            {
                var r = pixelData[i * 4 + 1];
                var g = pixelData[i * 4 + 2];
                var b = pixelData[i * 4 + 3];
                var a = pixelData[i * 4];
                
                pixels[i] = new Color32(r, g, b, a);
            }

            texture.SetPixels32(pixels);
            texture.Apply();
        }
    }
}