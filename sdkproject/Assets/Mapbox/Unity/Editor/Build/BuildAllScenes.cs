using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class BuildAllScenes : MonoBehaviour
{

	// https://docs.unity3d.com/2017.1/Documentation/Manual/CommandLineArguments.html
	// C:\program files\Unity\Editor\Unity.exe -quit -batchmode -executeMethod BuildAllScenes.Android
	// "C:\Program Files\Unity 5.6.0b11\Editor\Unity.exe" -buildTarget android  -quit -batchmode -logFile C:\mb\_TEMP\unity-auto-builds\log.txt

	private static string exportPathBase = @"C:\mb\_TEMP\unity-auto-builds";
	private static string exportPathAndroid = Path.Combine(exportPathBase, "android");
	private static string exportPathWindowsNative64bit = Path.Combine(exportPathBase, "win32-x64");
	private static string exportPathWindowsUWP = Path.Combine(exportPathBase, "UWP");



	[MenuItem("Build/List scenes in Console")]
	public static void ListScenes()
	{
		Debug.Log(string.Join(Environment.NewLine, getAllScenes()));
	}



	[MenuItem("Build/Build all Scenes/Android")]
	public static void Android()
	{
		string[] scenes = getAllScenes();
		Debug.Log("about to build:" + Environment.NewLine + string.Join(Environment.NewLine, scenes));

		foreach (var scene in scenes)
		{
			Debug.Log("BUILDING NOW: " + scene);
			string playerFile = getPlayerFilename(scene, exportPathAndroid, "apk");

			BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
			buildPlayerOptions.scenes = new[] { scene };
			buildPlayerOptions.locationPathName = playerFile;
			buildPlayerOptions.target = BuildTarget.Android;
			buildPlayerOptions.options = BuildOptions.Development;

			string error = BuildPipeline.BuildPlayer(buildPlayerOptions);
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError("==== ERROR BUILDING " + scene + " ======" + Environment.NewLine + error);
			}

			Debug.Log("finished building: " + scene);
		}
	}



	[MenuItem("Build/Build all Scenes/Windows Standalone x64")]
	public static void Win32x64()
	{
		string[] scenes = getAllScenes();
		Debug.Log("about to build:" + Environment.NewLine + string.Join(Environment.NewLine, scenes));

		foreach (var scene in scenes)
		{
			Debug.Log("BUILDING NOW: " + scene);
			string playerFile = getPlayerFilename(scene, exportPathWindowsNative64bit, "exe");

			BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
			buildPlayerOptions.scenes = new[] { scene };
			buildPlayerOptions.locationPathName = playerFile;
			buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
			buildPlayerOptions.options = BuildOptions.Development;

			string error = BuildPipeline.BuildPlayer(buildPlayerOptions);
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError("==== ERROR BUILDING " + scene + " ======" + Environment.NewLine + error);
			}

			Debug.Log("finished building: " + scene);
		}
	}



	[MenuItem("Build/Build all Scenes/UWP - Windows Store")]
	public static void UWP()
	{
		// https://docs.unity3d.com/ScriptReference/EditorUserBuildSettings.html
		// https://docs.unity3d.com/2017.1/Documentation/ScriptReference/EditorUserBuildSettings.SwitchActiveBuildTargetAsync.html
		// https://docs.unity3d.com/2017.1/Documentation/Manual/CommandLineArguments.html
		// https://docs.unity3d.com/ScriptReference/BuildOptions.html
		// https://docs.unity3d.com/ScriptReference/BuildPlayerOptions.html
		// https://docs.unity3d.com/ScriptReference/BuildPipeline.BuildPlayer.html
		// https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html
		// https://docs.unity3d.com/ScriptReference/GameObject.Find.html

		string[] scenes = getAllScenes();
		Debug.Log("about to build:" + Environment.NewLine + string.Join(Environment.NewLine, scenes));

		foreach (var scene in scenes)
		{
			Debug.Log("BUILDING NOW: " + scene);
			string playerFile = getPlayerFilename(scene, exportPathWindowsUWP, string.Empty);

			BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
			buildPlayerOptions.scenes = new[] { scene };
			buildPlayerOptions.locationPathName = playerFile;
			buildPlayerOptions.target = BuildTarget.WSAPlayer;
			buildPlayerOptions.options = BuildOptions.Development;

			string error = BuildPipeline.BuildPlayer(buildPlayerOptions);
			if (!string.IsNullOrEmpty(error))
			{
				Debug.LogError("==== ERROR BUILDING " + scene + " ======" + Environment.NewLine + error);
			}

			Debug.Log("finished building: " + scene);
		}

	}



	private static string getPlayerFilename(string scene, string exportPath, string extension)
	{
		string sceneName = Path.GetFileNameWithoutExtension(scene);
		string playerFile;
		if (!string.IsNullOrEmpty(extension))
		{
			playerFile = Path.Combine(exportPath, sceneName) + "." + extension;
		}
		else
		{
			//UWP needs a directory of its own as it creates a new Visual Studio solution
			exportPath = Path.Combine(exportPath, sceneName);
			playerFile = exportPath;
		}
		Debug.Log("saving to: " + playerFile);

		// create base output directory if it doesn't exist
		if (!Directory.Exists(exportPath)) { Directory.CreateDirectory(exportPath); }
		// delete player file it is exists
		if (File.Exists(playerFile)) { File.Delete(playerFile); }
		// if exporting to UWP we a directory to export to, remove if it exists
		// in that case 'playerFile' is the same as exportPath
		if (exportPath == playerFile && Directory.Exists(playerFile))
		{
			Directory.Delete(playerFile, true);
		}

		// delete <scene>_Data directory if it exists
		string dataDirectory = Path.Combine(exportPath, sceneName) + "_Data";
		if (Directory.Exists(dataDirectory)) { Directory.Delete(dataDirectory, true); }

		return playerFile;
	}


	private static string[] getAllScenesDirty()
	{
		string dataPath = Application.dataPath;
		Debug.Log(dataPath);
		DirectoryInfo di = new DirectoryInfo(dataPath);
		FileInfo[] scenes = di.GetFiles("*.unity", SearchOption.AllDirectories);

		return scenes.Select(fi => fi.FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets")).ToArray();
	}


	private static string[] getAllScenes()
	{
		//return AssetDatabase
		//	.FindAssets("t:Scene")
		//	.Select(s => AssetDatabase.GUIDToAssetPath(s))
		//	.ToArray();

		// safer: if there is a folder named 'foo.unity' it's considered a scene
		var guids = AssetDatabase.FindAssets("t:Scene");
		var paths = Array.ConvertAll<string, string>(guids, AssetDatabase.GUIDToAssetPath);
		paths = Array.FindAll(paths, File.Exists);
		return paths;
	}




}