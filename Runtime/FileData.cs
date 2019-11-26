using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Created By: Jonathon Wigley - 11/05/2018
/// Last Edited By: Jonathon Wigley - 12/13/2018
/// </summary>

/// <summary>
/// Data container for files that can be opened
/// </summary>
public class FileData : ScriptableObject
{
    #region Properties
    /// <summary>
    /// Display name.
    /// </summary>
    public string DisplayName => displayName;

	/// <summary>
	/// File path.
	/// </summary>
	public string Path => path;

    /// <summary>
    /// Instance ID.
    /// </summary>
    public string GUID => guid;
    #endregion

    #region Serialized Private Variables
    [SerializeField] private string displayName = string.Empty;
    [SerializeField] private string path = string.Empty;
    [SerializeField] private string guid = string.Empty;
    #endregion

    #region Public Methods
    public void Init(string displayName, string path, string guid)
    {
        this.displayName = displayName;
        this.path = path;
        this.guid = guid;
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Refresh any data of the file
    /// </summary>
    public void RefreshFileReference()
	{
		// Get the file path relative to streaming assets
		string tempPath = AssetDatabase.GUIDToAssetPath(GUID);
        Debug.Log(tempPath);
		int indexOfStreamingAssets = tempPath.IndexOf ("Assets/StreamingAssets");
		if (indexOfStreamingAssets != -1)
		{
			tempPath = tempPath.Remove (0, indexOfStreamingAssets + 22);
			if (tempPath[0] != '/')
				tempPath.Insert(0, "/");
		}

		if (string.IsNullOrEmpty(tempPath))
		{
			Debug.LogWarning("Lost file path: " + path, this);
		}

		// If the filepath is new, update it and save the asset
		if (path != tempPath)
		{
			path = tempPath;
			AssetDatabase.SaveAssets();
		}
	}
    #endif
    #endregion
}
