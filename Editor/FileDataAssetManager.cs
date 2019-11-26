using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Created By: Jonathon Wigley - 11/19/2018
/// Last Edited By: Jonathon Wigley - 12/13/2018
/// </summary>

/// <summary>
/// Used to tell any file data objects to update when a file is moved
/// </summary>
[InitializeOnLoad]
public class FileDataAssetManager : AssetPostprocessor
{
    #region Public Methods
    /// <summary>
    // Called any time an asset is imported, moved, deleted, etc.
    /// </summary>
    /// <param name="importedAssets"></param>
    /// <param name="deletedAssets"></param>
    /// <param name="movedAssets"></param>
    /// <param name="movedFromAssetPaths"></param>
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		// Get all assets of type FileData
		List<string> fileDataGUIDs = new List<string>(AssetDatabase.FindAssets("t:FileData", null));

		// Refresh the file data for all FileData objects, ensuring the proper location is always referenced
		string assetPath = "";
		for (int i = 0; i < fileDataGUIDs.Count; i++)
		{
			assetPath = AssetDatabase.GUIDToAssetPath(fileDataGUIDs[i]);
			AssetDatabase.LoadAssetAtPath<FileData>(assetPath).RefreshFileReference();
		}
	}

	/// <summary>
	/// Creates a <see cref="FileData"/> object for any selected objects.
	/// </summary>
	/// <param name="fileName">Name of the file to create.</param>
	/// <param name="filePath">Path to the file the <see cref="FileData"/> will reference.</param>
	/// <param name="saveFolderPath">Path of the folder to save the file in.</param>
	public static void CreateFile(string fileName, string filePath, string saveFolderPath)
	{
		// Get the file path relative to streaming assets
		string relativeFilePath = filePath.Remove(0, filePath.IndexOf("Assets/StreamingAssets") + 22);
		if (relativeFilePath[0] != '/')
			relativeFilePath.Insert(0, "/");

        // Create the file data instance and set the data
        FileData fileData = ScriptableObject.CreateInstance<FileData>();
        fileData.Init(fileName, relativeFilePath, AssetDatabase.AssetPathToGUID(filePath));
		saveFolderPath = GetSaveFolderPath(filePath, saveFolderPath);

        // Get the path for the new asset
        char lastCharacter = saveFolderPath[saveFolderPath.Length - 1];
        string savedAssetPath = string.Format
        (
            "{0}{1}{2}{3}",
            saveFolderPath,
            (lastCharacter == '/' ? "" : "/"),
            fileData.DisplayName,
            " File Data.asset"
        );
        // Create the new asset
        AssetDatabase.CreateAsset(fileData, savedAssetPath);
        AssetDatabase.SaveAssets();

        // Focus the newly created object
        EditorUtility.FocusProjectWindow();
		Selection.activeObject = fileData;
	}
    #endregion

    #region Private Methods
    /// <summary>
    /// Context menu button for the editor to create a <see cref="FileData"/> from an asset.
    /// </summary>
    [MenuItem ("Assets/Create File Data")]
	private static void BTN_CreateFile()
	{
		string fileName = "";
		string filePath = "";

		// Get the GUIDs from any currently selected objects
		string[] currentlySelectedObjects = Selection.assetGUIDs;
		for (int i = 0; i < currentlySelectedObjects.Length; i++)
		{
			// Get the selected object
			string assetPath = AssetDatabase.GUIDToAssetPath(currentlySelectedObjects[i]);
			Object currentlySelectedObject = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

			fileName = currentlySelectedObject.name;
			filePath = assetPath;
			CreateFile(fileName, filePath, null);
		}
	}

    /// <summary>
    /// Validation method for <see cref="BTN_CreateFile"/>.
    /// </summary>
    /// <returns></returns>
    [MenuItem("Assets/Create File Data", true)]
    private static bool ValidateBTN_CreateFile()
    {
        string[] currentlySelectedObjects = Selection.assetGUIDs;
        for (int i = 0; i < currentlySelectedObjects.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(currentlySelectedObjects[i]);
            Object currentlySelectedObject = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (!assetPath.Contains("Assets/StreamingAssets"))
                return false;
        }
        return true;
    }

	/// <summary>
	/// Get a valid save path for the <see cref="FileData"/>.
	/// </summary>
	/// <param name="filePath">Path to the file associated with the <see cref="FileData"/>.</param>
	/// <param name="potentialSaveFolderPath">Potential save folder path.</param>
	/// <returns></returns>
	private static string GetSaveFolderPath(string filePath, string potentialSaveFolderPath)
	{
		string saveFolderPath = potentialSaveFolderPath;

		// Make sure the save path is relative to Assets
		if (string.IsNullOrEmpty(saveFolderPath) == false)
		{
			// Remove the path up to the Assets folder
			saveFolderPath = saveFolderPath.Remove(0, saveFolderPath.IndexOf("Assets"));
			if (saveFolderPath[0] == '/')
				saveFolderPath.Remove (0, 1);
		}

		// If the save path is empty, set the save folder to the same folder as the file being referenced
		else
		{
			int indexOfDirectory = filePath.LastIndexOf ('/');
			saveFolderPath = filePath.Remove (indexOfDirectory, filePath.Length - indexOfDirectory);
		}

		return saveFolderPath;
	}
    #endregion
}