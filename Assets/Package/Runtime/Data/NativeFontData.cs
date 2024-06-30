using UnityEngine;

namespace TextMeshPro.Utils.Data
{
    public class NativeFontData
    {
        public NativeFontData(string name)
        {
            Name = name;
            PointSize = 40;
            Padding = 3;
            AtlasSize = new Vector2Int(2048, 2048);
        }
        
        public NativeFontData(string name, int pointSize, int padding, Vector2Int atlasSize)
        {
            Name = name;
            PointSize = pointSize;
            Padding = padding;
            AtlasSize = atlasSize;
        }

        public string Name { get; }
        public int PointSize { get; }
        public int Padding { get; }
        public Vector2Int AtlasSize { get; }
    }
}