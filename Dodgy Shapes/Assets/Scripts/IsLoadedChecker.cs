using UnityEngine;
using System.Collections;

public class IsLoadedChecker : MonoBehaviour
{
    public GameObject loadingCanvas;
    public GameObject mainAudio;

    public void Awake()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("Loading");
        GameObject mainAudio = GameObject.FindGameObjectWithTag("Audio");
        if (canvas == null && mainAudio == null)
        {
            Instantiate(this.loadingCanvas);
            Instantiate(this.mainAudio);
        }
    }

    public void LoadScene(int scene)
    {
        LoadingManager.Load(scene);
    }
}
