using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Evo.Tools
{
    public static class MenuTools
    {
        static readonly string filePath = "Assets/Evo/TemplateScripts/";

        [MenuItem("Evo/Create/DNA Type")]
        public static void CreateDNATypeScript()
        {
            string copyPath = filePath + "DNAObject.evotmp";
            string path = "Assets/DNAObject.cs";

            CopyFileToPath(copyPath, path);
        }

        [MenuItem("Evo/Create/DNA Value Generator")]
        public static void CreateDNAValueGeneratorScript()
        {
            string copyPath = filePath + "DNAValueGenerator.evotmp";
            string path = "Assets/DNAValueGenerator.cs";

            CopyFileToPath(copyPath, path);
        }

        [MenuItem("Evo/Create/Custom Agent Fitness")]
        public static void CreateFitnessScript()
        {
            string copyPath = filePath + "AgentFitness.evotmp";
            string path = "Assets/AgentFitness.cs";

            CopyFileToPath(copyPath, path);
        }

        private static void CopyFileToPath(string copyPath, string path)
        {
            StreamReader sr = new StreamReader(copyPath);
            string script = sr.ReadToEnd();
            sr.Close();

            FileStream fs = File.Create(path);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(script);
            sw.Close();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        }
    }
}
