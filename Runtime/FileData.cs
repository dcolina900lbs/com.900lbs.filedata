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
    public string GUID { get; private set; } = string.Empty;
    #endregion

    #region Serialized Private Variables
    [SerializeField] private string displayName = string.Empty;
    [SerializeField] private string path = string.Empty;
    #endregion

    #region Public Methods
    public void Init(string displayName, string path, string GUID)
    {
        this.displayName = displayName;
        this.path = path;
        this.GUID = GUID;
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Refresh any data of the file
    /// </summary>
    public void RefreshFileReference()
	{
		// Get the file path relative to streaming assets
		string tempPath = UnityEditor.AssetDatabase.GUIDToAssetPath(GUID);
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
