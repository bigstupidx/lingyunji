#if UNITY_EDITOR
using UnityEditor;

namespace PackTool
{
    public class SetSpritePackerMode : System.IDisposable
    {
        SpritePackerMode spritePackerMode;

        public SetSpritePackerMode(SpritePackerMode spm)
        {
            spritePackerMode = EditorSettings.spritePackerMode;
            EditorSettings.spritePackerMode = spm;

        }

        public void Dispose()
        {
            EditorSettings.spritePackerMode = spritePackerMode;
        }
    }

    public class SetSelectedPolicy : System.IDisposable
    {
        string SelectedPolicy;

        public SetSelectedPolicy(string sp)
        {
            SelectedPolicy = UnityEditor.Sprites.Packer.SelectedPolicy;
            UnityEditor.Sprites.Packer.SelectedPolicy = sp;
        }

        public void Dispose()
        {
            UnityEditor.Sprites.Packer.SelectedPolicy = SelectedPolicy;
        }
    }
}
#endif