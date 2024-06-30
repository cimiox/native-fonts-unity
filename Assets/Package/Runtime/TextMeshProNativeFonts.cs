using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextMeshPro.Utils.Data;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace TextMeshPro.Utils
{
    public static class TextMeshProNativeFonts
    {
        public static IEnumerable<TMP_FontAsset> GetFallbacks(params NativeFontData[] data)
        {
            var nativeFonts = GetNativeFonts(data);
            var tmpFonts = ConvertNativeFontsToTmp(nativeFonts);

            return tmpFonts;
        }

        private static IEnumerable<(NativeFontData data, string path)> GetNativeFonts(IEnumerable<NativeFontData> data)
        {
            var nativeFonts = Font.GetPathsToOSFonts();

            foreach (var fontData in data)
            {
                var currentFontName = fontData.Name;
                var nativeFont = nativeFonts.FirstOrDefault(nf => IsTargetFont(currentFontName, nf));

                if (nativeFont is not null)
                {
                    yield return (fontData, nativeFont);
                }
                else
                {
                    Debug.Log($"Can't find any font with name: {fontData.Name}");
                }
            }

            yield break;

            bool IsTargetFont(string target, string current)
            {
                return target == Path.GetFileNameWithoutExtension(current);
            }
        }

        private static IEnumerable<TMP_FontAsset> ConvertNativeFontsToTmp(IEnumerable<(NativeFontData data, string fontPath)> fontPaths)
        {
            foreach (var (data, fontPath) in fontPaths)
            {
                var font = new Font(fontPath);
                var tmpFontAsset = TMP_FontAsset.CreateFontAsset(font, data.PointSize, data.Padding,
                    GlyphRenderMode.SDFAA_HINTED,
                    data.AtlasSize.x, data.AtlasSize.y);

                yield return tmpFontAsset;
            }
        }
    }
}