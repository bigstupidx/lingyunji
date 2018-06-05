namespace xys.Editor
{
    public class IndentLevel : System.IDisposable
    {
        public IndentLevel()
        {
            ++UnityEditor.EditorGUI.indentLevel;
        }

        public void Dispose()
        {
            --UnityEditor.EditorGUI.indentLevel;
        }
    }
}