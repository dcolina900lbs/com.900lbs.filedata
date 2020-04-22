using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Data object that wraps files non-serialized by Unity, caching references to GUID
/// and path and updating at editor runtime.
/// </summary>
public class FileData : ScriptableObject
{
    #region Properties
    /// <summary>
    /// Display name.
    /// </summary>
    public string DisplayName => displayName;

	/// <summary>
	/// Path relative to the <see cref="Application.streamingAssetsPath"/>.
	/// </summary>
	public string RelativePath => relativePath;

    /// <summary>
    /// Absolute path to the file this data references.
    /// </summary>
    public string AbsolutePath => Path.Combine(Application.streamingAssetsPath, RelativePath);

    /// <summary>
    /// Instance ID.
    /// </summary>
    public string GUID => guid;
    #endregion

    #region Serialized Private Variables
    [SerializeField] private string displayName = string.Empty;
    [SerializeField] private string relativePath = string.Empty;
    [SerializeField] private string guid = string.Empty;
    #endregion

    #region Public Methods
    public void Init(string displayName, string path, string guid)
    {
        this.displayName = displayName;
        this.relativePath = path;
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
		int indexOfStreamingAssets = tempPath.IndexOf ("Assets/StreamingAssets/");
		if (indexOfStreamingAssets != -1)
		{
			tempPath = tempPath.Remove (0, indexOfStreamingAssets + 22);
		}

		if (string.IsNullOrEmpty(tempPath))
		{
			Debug.LogWarning("Lost file path: " + relativePath, this);
		}

		// If the filepath is new, update it and save the asset
		if (relativePath != tempPath)
		{
			relativePath = tempPath;
			AssetDatabase.SaveAssets();
		}
	}
    #endif
    #endregion
}
