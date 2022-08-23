using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorUtil
{
    ////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
    
    [MenuItem("GameTools/Generate scene define")]
    static void GenerateSceneDefine()
    {
        string scriptPath = "Assets/Scripts/Generated/SceneDef.cs";
        FileInfo scriptInfo = new FileInfo(scriptPath);

        if (!Directory.Exists(scriptInfo.Directory.FullName))
        {
            Directory.CreateDirectory(scriptInfo.Directory.FullName);
        }

        List<string> contents = new List<string>();
        contents.Add("// This is generated file. DO NOT MODIFY MANUALLY!");
        contents.Add("public static class SceneDef ");
        contents.Add("{");
        foreach (var file in EditorBuildSettings.scenes)
        {
            FileInfo fileInfo = new FileInfo(file.path);
            string scene = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf(fileInfo.Extension));
            string varStr = scene.Replace('.', '_').Replace(' ', '_');
            contents.Add(string.Format("\tpublic const string k_{0} = \"{1}\";", varStr, scene));
        }
        contents.Add("}");
        File.WriteAllLines(scriptPath, contents.ToArray());

        Debug.Log("Generate done: " + scriptPath);

    }

    [MenuItem("GameTools/Generate Sound define")]
    static void GenerateSoundDefine()
    {
        string scriptPath = "Assets/Scripts/Config/SoundDefine.cs";
        string soundPath = "Assets/Resources/Sounds";
        FileInfo scriptInfo = new FileInfo(scriptPath);

        if (!Directory.Exists(scriptInfo.Directory.FullName))
        {
            Directory.CreateDirectory(scriptInfo.Directory.FullName);
        }

        List<string> contents = new List<string>();
        contents.Add("// This is generated file. DO NOT MODIFY MANUALLY!");
        contents.Add("public static class SoundDefine ");
        contents.Add("{");
        
        DirectoryInfo d = new DirectoryInfo(soundPath);//Assuming Test is your Folder
        FileInfo[] Files = d.GetFiles("*.*", SearchOption.AllDirectories); //Getting files
        foreach (var file in Files)
        {
            if (file.Extension.Equals(".meta"))
            {

            }
            else
            {
                string nameNoExt = file.Name.Substring(0, file.Name.IndexOf(file.Extension));
                Debug.Log("file: " + nameNoExt + ", ext: " + file.Extension + ", dir: " + file.Directory.Name);
                contents.Add(string.Format("\tpublic const string k_{0}_{1} = \"{1}\";", 
                    file.Directory.Name,
                    nameNoExt
                ));
            }
        }

        contents.Add("}");
        File.WriteAllLines(scriptPath, contents.ToArray());

        Debug.Log("Generate done: " + scriptPath);

    }
#endif
}