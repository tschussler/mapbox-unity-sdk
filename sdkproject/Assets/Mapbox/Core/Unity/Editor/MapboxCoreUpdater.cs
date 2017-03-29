namespace Mapbox.Editor
{
    using System.IO;
    using System.Diagnostics;
    using Mapbox.Unity;
    using UnityEditor;
    using UnityEngine;

    public static class MapboxCoreUpdater
    {
        [MenuItem("Mapbox/Update Core")]
        public static void Sync()
        {
            var path = Application.dataPath;
            path = path.Replace("sdkproject" + Path.DirectorySeparatorChar + "Assets", "");
            var executableExenstion = Application.platform == RuntimePlatform.WindowsEditor ? ".bat" : ".sh";

            var exe = new Process();
            exe.StartInfo.WorkingDirectory = path;
            exe.StartInfo.FileName = path + Constants.Path.EXECUTABLE_NAME + executableExenstion;
            exe.StartInfo.RedirectStandardOutput = true;
            exe.StartInfo.UseShellExecute = false;

            exe.Start();
            var output = exe.StandardOutput.ReadToEnd();
            exe.WaitForExit();
            UnityEngine.Debug.Log("MapboxCoreUpdater: " + output);

            AssetDatabase.Refresh();
        }
    }
}