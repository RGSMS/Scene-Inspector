using UnityEngine;

namespace RGSMS.Scene
{
    /// <summary>
    /// Scene Information Reference
    /// </summary>
    [System.Serializable]
    public sealed class SceneInspector
    {
        /// <summary>
        /// Scene Name.
        /// </summary>
        [SerializeField]
        private string _name = string.Empty;
        public string Name => _name;

        /// <summary>
        /// Scene Folder Path.
        /// </summary>
        [SerializeField]
        private string _path = string.Empty;
        public string Path => _path;
        
        /// <summary>
        /// Scene Build Index.
        /// </summary>
        [SerializeField]
        private int _buildIndex = -1;
        public int BuildIndex => _buildIndex;
    }
}
