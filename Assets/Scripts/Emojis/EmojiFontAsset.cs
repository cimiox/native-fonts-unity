using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

namespace Scripts.Emojis
{
    public class EmojiFontAsset : TMP_FontAsset
    {
        [SerializeField]
        private string fontPath;
        
        private void Reset()
        {
            characterTable.Add(new TMP_Character(0xF600, new Glyph(0xF600, new GlyphMetrics(160, 160, 0,0, 0), 
                new GlyphRect(0,0, 160, 160), 1, 0)));
            
            Debug.Log(characterTable[0].unicode);
        }   
    }

    public static class FontAsset
    {
        [MenuItem("Assets/Create/TextMeshPro/Emoji Font Asset", false, 100)]
        public static void CreateFontAsset()
        {
            Object[] targets = Selection.objects;

            if (targets == null)
            {
                Debug.LogWarning("A Font file must first be selected in order to create a Font Asset.");
                return;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                Object target = targets[i];

                // Make sure the selection is a font file
                if (target == null || target.GetType() != typeof(Font))
                {
                    Debug.LogWarning("Selected Object [" + target.name + "] is not a Font file. A Font file must be selected in order to create a Font Asset.", target);
                    continue;
                }

                CreateFontAssetFromSelectedObject(target);
            }
        }

        static void CreateFontAssetFromSelectedObject(Object target)
        {
            Font sourceFont = (Font)target;

            string sourceFontFilePath = AssetDatabase.GetAssetPath(target);

            string folderPath = Path.GetDirectoryName(sourceFontFilePath);
            string assetName = Path.GetFileNameWithoutExtension(sourceFontFilePath);

            string newAssetFilePathWithName = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + assetName + " SDF.asset");

            // Initialize FontEngine
            FontEngine.InitializeFontEngine();

            // Load Font Face
            if (FontEngine.LoadFontFace(sourceFont, 90) != FontEngineError.Success)
            {
                Debug.LogWarning("Unable to load font face for [" + sourceFont.name + "]. Make sure \"Include Font Data\" is enabled in the Font Import Settings.", sourceFont);
                return;
            }

            // Create new Font Asset
            TMP_FontAsset fontAsset = ScriptableObject.CreateInstance<EmojiFontAsset>();
            AssetDatabase.CreateAsset(fontAsset, newAssetFilePathWithName);

            fontAsset.SetProperty("version", "1.1.0");

            fontAsset.faceInfo = FontEngine.GetFaceInfo();

            // Set font reference and GUID
            fontAsset.SetField("m_SourceFontFileGUID", AssetDatabase.AssetPathToGUID(sourceFontFilePath));
            fontAsset.SetField("m_SourceFontFile_EditorRef", sourceFont);
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;

            fontAsset.SetProperty("atlasWidth", 1024);
            fontAsset.SetProperty("atlasHeight", 1024);
            fontAsset.SetProperty("atlasPadding", 9);
            fontAsset.SetProperty("atlasRenderMode", GlyphRenderMode.SDFAA);

            // Default atlas resolution is 1024 x 1024.
            int atlasWidth = fontAsset.atlasWidth;
            int atlasHeight = fontAsset.atlasHeight;
            int atlasPadding = fontAsset.atlasPadding;

            // Initialize array for the font atlas textures.
            fontAsset.atlasTextures = new Texture2D[1];

            // Create atlas texture of size zero.
            Texture2D texture = new Texture2D(0, 0, TextureFormat.Alpha8, false);

            texture.name = assetName + " Atlas";
            fontAsset.atlasTextures[0] = texture;
            AssetDatabase.AddObjectToAsset(texture, fontAsset);

            // Add free rectangle of the size of the texture.
            int packingModifier = 0;//((GlyphRasterModes)fontAsset.atlasRenderMode & GlyphRasterModes.RASTER_MODE_BITMAP) == GlyphRasterModes.RASTER_MODE_BITMAP ? 0 : 1;
            fontAsset.SetProperty("freeGlyphRects", new List<GlyphRect>() { new GlyphRect(0, 0, atlasWidth - packingModifier, 
                atlasHeight - packingModifier) });
            fontAsset.SetProperty("usedGlyphRects", new List<GlyphRect>());

            // Create new Material and Add it as Sub-Asset
            Shader default_Shader = Shader.Find("TextMeshPro/Distance Field");
            Material tmp_material = new Material(default_Shader);

            tmp_material.name = texture.name + " Material";
            tmp_material.SetTexture(ShaderUtilities.ID_MainTex, texture);
            tmp_material.SetFloat(ShaderUtilities.ID_TextureWidth, atlasWidth);
            tmp_material.SetFloat(ShaderUtilities.ID_TextureHeight, atlasHeight);

            tmp_material.SetFloat(ShaderUtilities.ID_GradientScale, atlasPadding + packingModifier);

            tmp_material.SetFloat(ShaderUtilities.ID_WeightNormal, fontAsset.normalStyle);
            tmp_material.SetFloat(ShaderUtilities.ID_WeightBold, fontAsset.boldStyle);

            fontAsset.material = tmp_material;

            AssetDatabase.AddObjectToAsset(tmp_material, fontAsset);

            // Add Font Asset Creation Settings
            fontAsset.creationSettings = new FontAssetCreationSettings()
            {
                sourceFontFileName = fontAsset.GetField<string>("m_SourceFontFileGUID"),
                pointSize = (int) fontAsset.faceInfo.pointSize,
                pointSizeSamplingMode = 0,
                padding = atlasPadding,
                packingMode = 0,
                atlasWidth = 1024,
                atlasHeight = 1024,
                characterSetSelectionMode = 7,
                characterSequence = string.Empty,
                renderMode = (int)GlyphRenderMode.SDFAA
            };

            // Not sure if this is still necessary in newer versions of Unity.
            EditorUtility.SetDirty(fontAsset);

            AssetDatabase.SaveAssets();
        }

        private static void SetProperty(this object obj, string property, object value)
        {
            obj.GetType().GetProperty(property, BindingFlags.Default | BindingFlags.Instance | BindingFlags
                .NonPublic)?.SetValue(obj, value);
        }
        
        private static void SetField(this object obj, string property, object value)
        {
            obj.GetType().GetField(property, BindingFlags.Default | BindingFlags.Instance | BindingFlags
                .NonPublic)?.SetValue(obj, value);
        } 
        
        private static T GetField<T>(this object obj, string property)
        {
            return (T)obj.GetType().GetField(property, BindingFlags.Default | BindingFlags.Instance | BindingFlags
                .NonPublic)?.GetValue(obj);
        }
    }
}