using UnityEngine;

namespace RGSMS.Scene
{
    [System.Serializable]
    public sealed class SceneInspector
    {
        [SerializeField]
        private string _path = string.Empty;
        public string Path => _path;

        [SerializeField]
        private int _buildIndex = 0;
        public int BuildIndex => _buildIndex;

        public override string ToString() => _path;
    }
}
