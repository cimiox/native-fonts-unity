using TMPro;
using UnityEngine;

namespace Scripts.Emojis
{
    public class Utils : MonoBehaviour
    {
        private class Test1
        {
            public void Method()
            {
                Debug.Log("test1");
            }
        }
        private class Test2 : Test1
        {
            public new  void Method()
            {
                Debug.Log("test2");
            }
        }
        
        [SerializeField]
        private TMP_FontAsset font;

        [ContextMenu("Debug")]
        private void DebugCharacter()
        {
            Test1 test2 = new Test2();
            
            test2.Method();

            // var character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(0xF600, font, false,
            //     default, default, out var test);
            //
            // Debug.Log(character);
        }
    }
}