using UnityEditor;
using System.IO;
using UnityEngine;

/// <summary>
/// Created By: Jonathon Wigley - 11/19/2018
/// Last Edited By: Jonathon Wigley - 12/13/2018
/// </summary>

/// <summary>
/// Editor window used for creating assets
/// </summary>
public class FileObjectCreatorWindow : EditorWindow
{
	/// <summary>
	/// File path to the folder that objects will be saved in, relative to Application.dataPath
	/// </summary>
	private static string saveFolderPath;

	/// <summary>
	/// Path to the file associated with new File Data
	/// </summary>
	private static string filePath;

	/// <summary>
	/// Name of the file to be associated with new File Data
	/// </summary>
	private static string fileName;

	/// <summary>
	/// Object that is currently selected in editor
	/// </summary>
	private static Object currentSelectedObject;

	private void OnEnable ()
	{
		Selection.selectionChanged += Selection_SelectedObjectChanged;
	}

	private void OnDisable ()
	{
		Selection.selectionChanged -= Selection_SelectedObjectChanged;
	}

	[MenuItem ("900lbs/File Object Creator")]
	private static void ShowWindow ()
	{
		GetWindow<FileObjectCreatorWindow> ().Show ();
		//Show existing window instance. If one doesn't exist, make one with the specified rect size.
		Rect windowRect = new Rect (0, 0, 300, 300);
		EditorWindow editorWindow = EditorWindow.GetWindowWithRect (typeof (FileObjectCreatorWindow), windowRect, false, "File Object Creator");

		// Show the editor window on screen
		editorWindow.Show ();
	}

	private void OnGUI ()
	{
		// Adjust the label width
		float originalLabelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 150f;

		// Create browser for save file path
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.DelayedTextField("Save Folder Path", saveFolderPath);
		if(GUILayout.Button("Browse", GUILayout.MaxWidth(75)))
		{
			string tempPath = Application.dataPath.Replace('/', '\\');
			saveFolderPath = EditorUtility.SaveFolderPanel("Save Folder Path", saveFolderPath, tempPath);
		}
		EditorGUILayout.EndHorizontal();

		// Show the selected object's file name and path
		fileName = EditorGUILayout.TextField ("File Name", fileName);
		EditorGUILayout.TextField ("File Path", filePath);

		// Draw a button used to create the file
		if(GUILayout.Button ("Create File"))
			BTN_CreateFile();

		EditorGUIUtility.labelWidth = originalLabelWidth;
	}

	/// <summary>
	/// Called whenever the selection is changed in editor
	/// </summary>
	private void Selection_SelectedObjectChanged()
	{
		// Get the currently selected object in the editor
		currentSelectedObject = Selection.activeObject;

		fileName = "";
		filePath = "";
		string tempPath = "";

		// Check to see if a valid object is selected
		if(currentSelectedObject != null)
		{
			tempPath = AssetDatabase.GetAssetPath(currentSelectedObject.GetInstanceID());
			if(File.Exists(tempPath))
			{
				fileName = currentSelectedObject.name;
				filePath = tempPath;

				// Make sure the save path is relative to Assets
				if(string.IsNullOrEmpty(saveFolderPath) == false)
					saveFolderPath = saveFolderPath.Remove(0, saveFolderPath.IndexOf("Assets"));

				else
				{
					int indexOfDirectory = filePath.LastIndexOf('/');
					saveFolderPath = filePath.Remove(indexOfDirectory, filePath.Length - indexOfDirectory);
				}
			}
		}

		// Update the editor window
		Repaint();
	}

	/// <summary>
	/// Called by the button used to create the file
	/// </summary>
	private static void BTN_CreateFile()
	{
		FileDataAssetManager.CreateFile(fileName, filePath, saveFolderPath);
	}
}