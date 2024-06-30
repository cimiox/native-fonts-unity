using System.Collections.Generic;
using System.Linq;
using TextMeshPro.Utils;
using TextMeshPro.Utils.Data;
using TMPro;
using UnityEngine;

namespace Scripts
{
    public class TestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TMP_FontAsset font;
        
        private void Start()
        {
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
