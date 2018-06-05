#if UNITY_EDITOR && USE_HOT
using ILRuntime.Runtime.Enviorment;

namespace xys
{
    public static class ILEditor 
    {
        static AppDomain appdomain_;

        public static AppDomain appdomain
        {
            get
            {
                if (App.my != null)
                {
                    return App.my.appdomain;
                }
                else if (appdomain_ == null)
                {
                    LoadDLL();
                }

                return appdomain_;
            }
        }

        static ILEditor()
        {
            LoadDLL();
        }

        [UnityEditor.MenuItem("Assets/LoadDLL")]
        static void LoadDLL()
        {
#if USE_HOT
            appdomain_ = new AppDomain();
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain_);
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream("Data/DyncDll.dll", System.IO.FileMode.Open))
                {
#if USE_PDB
                    using (System.IO.FileStream p = new System.IO.FileStream("Data/DyncDll.pdb", System.IO.FileMode.Open))
                    {
                        appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
                    }
#else
                    appdomain_.LoadAssembly(fs);
#endif
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
#endif
        }
    }

}
#endif