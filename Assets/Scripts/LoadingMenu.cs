using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    [SerializeField]
    private Image _ProgressBar;

    void Start()
    {
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        yield return new WaitForSeconds(.01f);
        AsyncOperation GameLevel = SceneManager.LoadSceneAsync("SampleScene");
        //var progress = GameLevel.progress / .9f;
        while (!GameLevel.isDone)
        {
            _ProgressBar.fillAmount = GameLevel.progress;
            yield return null;
        }
    }
}
