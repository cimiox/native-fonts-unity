using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextMeshPro.Utils;
using TextMeshPro.Utils.Data;
using TMPro;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using Object = UnityEngine.Object;

namespace Scripts
{
    public class TestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TMP_FontAsset font;

        [SerializeField]
        private Font emojies;

        [SerializeField]
        private TextMeshProUGUI text;

        [SerializeField]
        private TMP_SpriteAsset spriteAsset;

        [SerializeField]
        private Object obj;

        [ContextMenu("AddAsset")]
        public void AddAsset()
        {
            AssetDatabase.AddObjectToAsset(obj, emojies);
        }

        private void OnDestroy()
        {
            Destroy(spriteAsset);
        }
        

        private void Start()
        {
            var path = Font.GetPathsToOSFonts().First(x =>
            {
                return x.Trim().Contains("Apple Color Emoji");
            });
            
            
            return;
            
            var initStatus = FontEngine.InitializeFontEngine();
            var loadStatus = FontEngine.LoadFontFace(emojies);

            var faceInfo = FontEngine.GetFaceInfo();

            for (uint i = 0x0000; i < 0xffff; i++)
            {
                var hasIndexInCycle = FontEngine.TryGetGlyphIndex(i, out var test);

                if (hasIndexInCycle)
                {
                    Debug.Log($"Found: {i} {test}");
                }
            }

            var hasGlyph = FontEngine.TryGetGlyphWithIndexValue(3, GlyphLoadFlags.LOAD_COLOR, out var glyph);
            
            var hasIndex = FontEngine.TryGetGlyphIndex(0xf600, out var index);
            FontEngine.UnloadFontFace();

            spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            var tmpGlyph = new TMP_SpriteGlyph(glyph.index, glyph.metrics, glyph.glyphRect, 1, 0);
            
            spriteAsset.spriteCharacterTable.Add(new TMP_SpriteCharacter(glyph.index, spriteAsset, tmpGlyph));
            
            Debug.Log(loadStatus);
            Debug.Log(initStatus);
            Debug.Log(faceInfo.scale);
            Debug.Log(hasGlyph);
            Debug.Log(hasIndex);
            Debug.Log(glyph.index);
            
            text.text = "\uF600";
            
            return;
            var fontData = Application.platform switch
            {
                RuntimePlatform.Android => new[]
                {
                    //Android
                    new NativeFontData("NotoNaskhArabic-Regular"), //Arabic
                    new NativeFontData("NotoSansDevanagari-Regular"), //Hindi
                    new NativeFontData("NotoSansCJK-Regular", 40, 3, new Vector2Int(2048, 2048)), //Chinese, Japanese, Korean 
                    new NativeFontData("Roboto-Black"), //Unicode
                    new NativeFontData("Arial"), new NativeFontData("LiberationSans")
                },
                RuntimePlatform.IPhonePlayer => new[]
                {
                    //iOS
                    new NativeFontData("HiraginoMincho"), //Japanese
                    new NativeFontData("NotoNastaliq"), //Arabic
                    new NativeFontData("DevanagariSangamMN"), //Hindi
                    new NativeFontData("PingFang", 40, 3, new Vector2Int(2048, 2048)), //Chinese
                    new NativeFontData("AppleSDGothicNeo"), //Korean
                    new NativeFontData("SFUI"), //Currency symbols
                    new NativeFontData("Arial"), new NativeFontData("LiberationSans")
                },
                RuntimePlatform.OSXEditor => new[]
                {
                    //iOS
                    new NativeFontData("HiraginoMincho"), //Japanese
                    new NativeFontData("NotoNastaliq"), //Arabic
                    new NativeFontData("DevanagariSangamMN"), //Hindi
                    new NativeFontData("PingFang", 40, 3,new Vector2Int(2048, 2048)), //Chinese
                    new NativeFontData("AppleSDGothicNeo"), //Korean
                    new NativeFontData("SFUI"), //Currency symbols
                    new NativeFontData("Arial"), new NativeFontData("LiberationSans")
                },
                _ => new[] {new NativeFontData("Arial"), new NativeFontData("LiberationSans")}
            };

            var fallbacks = TextMeshProNativeFonts.GetFallbacks(fontData);
            
            var currentFallbacks = font.fallbackFontAssetTable ?? new List<TMP_FontAsset>();
            var allFallbacks = fallbacks.Concat(currentFallbacks).ToList();

            font.fallbackFontAssetTable = allFallbacks;
        }
    }
}
