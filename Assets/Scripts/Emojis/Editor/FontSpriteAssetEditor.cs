using System;
using System.Runtime.InteropServices;
using SharpFont;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace Scripts.Emojis.Editor
{
    [CustomEditor(typeof(EmojiFontAsset))]
    public class FontSpriteAssetEditor : TMP_FontAssetEditor
    {
        public override void OnInspectorGUI()
        {
            var fontPathProperty = serializedObject.FindProperty("fontPath");
            
            EditorGUILayout.PropertyField(fontPathProperty);
            
            var rect = EditorGUILayout.GetControlRect(false, 24);

            if (GUI.Button(rect, "Generate"))
            {
                GenerateAsset(fontPathProperty.stringValue);
            }
            
            serializedObject.ApplyModifiedProperties();
            
            base.OnInspectorGUI();

            if (EditorUtility.IsDirty(serializedObject.targetObject))
            {
                AssetDatabase.SaveAssets();
            }
        }

        private void GenerateAsset(string fontPath)
        {
            var library = new Library();
            var font = new Face(library, fontPath);
            
            var texture = new Texture2D(2048, 2048, TextureFormat.RGBA32,
                false);
            var pixels = new Color32[2048 * 2048];
            
            var startX = 100;
            var startY = 100;

            for (uint charCode = 0x1F603; charCode <= 0x10FFFF; charCode++)
            {
                var glyphIndex = font.GetCharIndex(charCode);
                
                // If the glyph index is not 0, the character exists in the font
                if (glyphIndex != 0)
                {
                    font.SetPixelSizes(160, 160);
                    font.LoadGlyph(glyphIndex, LoadFlags.Color, LoadTarget.Normal);
                    
                    var glyph = font.Glyph;
                    var bitmap = glyph.Bitmap;
                    var pixelData = new byte[bitmap.Width * bitmap.Rows * 4];
                    
                    if (bitmap.Width == 0)
                    {
                        continue;
                    }
                    
                    var smile = new Texture2D(160, 160, TextureFormat.RGBA32, false);

                    smile.LoadRawTextureData(glyph.Bitmap.Buffer, 160 * 160 * 4);
                    smile.Apply();
                    smile.name = "smile";
                    
                    //AssetDatabase.AddObjectToAsset(smile, serializedObject.targetObject);
                    
                    Marshal.Copy(bitmap.Buffer, pixelData, 0, pixelData.Length);
                    Array.Reverse(pixelData);

                    for (var y = 0; y < bitmap.Rows; y++)
                    {
                        for (var x = 0; x < bitmap.Width; x++)
                        {
                            var srcIndex = y * bitmap.Width + x;
                            var destIndex = (startX + y) * 2048 + (startY + x);

                            // Retrieve pixel data from glyph bitmap (RGBA format)
                            var r = pixelData[srcIndex * 4 + 1];
                            var g = pixelData[srcIndex * 4 + 2];
                            var b = pixelData[srcIndex * 4 + 3];
                            var a = pixelData[srcIndex * 4];

                            // Set pixel in the texture
                            pixels[destIndex] = new Color32(r, g, b, a);
                        }
                    }
                    
                    break;
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();
            texture.name = "Texture";
            
            foreach (var allAssetPath in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(serializedObject.targetObject)))
            {
                if (allAssetPath.name == "Texture" || allAssetPath.name == "smile")
                {
                    AssetDatabase.RemoveObjectFromAsset(allAssetPath);
                }
            }

            AssetDatabase.AddObjectToAsset(texture, serializedObject.targetObject);
            
            EditorUtility.SetDirty(serializedObject.targetObject);
            AssetDatabase.Refresh();
        }
    }
}