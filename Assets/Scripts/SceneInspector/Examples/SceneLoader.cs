using UnityEngine.SceneManagement;
using UnityEngine;
using RGSMS.Scene;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private SceneInspector _sceneToLoad = null;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadSceneAsync(_sceneToLoad.BuildIndex);
            return;
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadSceneAsync(_sceneToLoad.Path);
            return;
        }
    }
}
