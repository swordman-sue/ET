using System.IO;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class AssemblyEditor
    {
        private static readonly string[] DllNames = { "ET.Hotfix", "ET.HotfixView", "ET.Model", "ET.ModelView" };
        
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            EditorApplication.playModeStateChanged += change =>
            {
                switch (change)
                {
                    case PlayModeStateChange.ExitingEditMode:
                    {
                        OnExitingEditMode();
                        break;
                    }
                }
            };
        }

        /// <summary>
        /// 退出编辑模式时处理(即将进入运行模式)
        /// EnableDll模式时, 屏蔽掉Library的dll(通过改文件后缀方式屏蔽), 仅使用Define.CodeDir下的dll
        /// </summary>
        static void OnExitingEditMode()
        {
            foreach (string dll in DllNames)
            {
                string dllFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.dll";
                if (File.Exists(dllFile))
                {
                    /* 由于OnGenerateCSProjectProcessor.GenerateCustomProject中插入的生成后事件，对VS2022无效
                     * 且进Editor Play Mode前，需执行ET/Loader/Compile _F6，否则无法在Define.CodeDir加载到最新的热更dll
                     * 所以在此处加入File.Copy(拷贝最新的热更dll到Define.CodeDir)
                     */
                    File.Copy(dllFile, $"{Define.CodeDir}/{dll}.dll.bytes", true);
                    File.Delete(dllFile);
                }

                string pdbFile = $"{Application.dataPath}/../Library/ScriptAssemblies/{dll}.pdb";
                if (File.Exists(pdbFile))
                {
                    File.Copy(pdbFile, $"{Define.CodeDir}/{dll}.pdb.bytes", true);
                    File.Delete(pdbFile);
                }
            }
        }
    }
}