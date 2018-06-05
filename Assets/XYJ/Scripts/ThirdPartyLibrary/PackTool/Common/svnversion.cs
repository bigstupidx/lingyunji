#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.IO;

public class svnversion
{
    [UnityEditor.MenuItem("PackTool/SvnVersion")]
    public static void CheckSvnVersion()
    {
        GetVersion();
    }

    public static long GetVersion()
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = Application.dataPath + "/../svnversion.bat";
        p.StartInfo.Arguments = "1234";
        p.Start();
        p.WaitForExit();
        p.Close();

        string versionfile = Application.dataPath + "/../svnversion.txt";
        if (File.Exists(versionfile))
        {
            try
            {
                string[] ss = File.ReadAllLines(versionfile, System.Text.Encoding.Unicode);
                foreach (string s in ss)
                {
                    if (s.StartsWith("Last committed at revision "))
                    {
                        try
                        {
                            long svn = long.Parse(ss[1].Substring("Last committed at revision ".Length));
                            Debuger.Log(string.Format("svn version :{0}", svn));
                            return svn;
                        }
                        catch (System.Exception /*ex*/)
                        {

                        }
                    }
                }
            }
            catch (System.Exception /*ex*/)
            {

            }
        }

        Debuger.LogError("获取SVN版本失败!");
        return 0;
    }
}
#endif