using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public Image fadeImage;
    public Image logo;
    public float duration = 1;
    public static bool hasLoaded;
    public static bool canUseBackButton = true;

    private static LoadingManager loadManager;
    private AsyncOperation operation;
    private float secondsCount;

    public void Awake()
    {
        if (loadManager == null)
        {
            loadManager = this;
        }
        else if (loadManager != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        fadeImage.enabled = false;
        logo.enabled = false;
        hasLoaded = false;

        EnableRaycaster();
    }

    public static void Load(int sceneNumber)
    {
        loadManager.LoadScene(sceneNumber);
    }

    private void LoadScene(int sceneNumber)
    {
        hasLoaded = false;
        StopAllCoroutines();
        StartCoroutine(_Load(sceneNumber));
        MainAuidoManager.mainAudio.PlayClickSound();
    }

    private IEnumerator _Load(int sceneNumber)
    {
        fadeImage.enabled = true;
        logo.enabled = true;
        canUseBackButton = false;

        while (fadeImage.color.a < 1)
        {
            Color c = fadeImage.color;
            c.a += 0.1f;
            fadeImage.color = c;
            logo.color = c;
            yield return null;
        }
        operation = SceneManager.LoadSceneAsync(sceneNumber, LoadSceneMode.Single);
        while (!operation.isDone || secondsCount < duration)
        {
            secondsCount += Time.fixedDeltaTime;
            yield return null;
        }

        secondsCount = 0;
        while (fadeImage.color.a > 0)
        {
            Color c = fadeImage.color;
            c.a -= 0.1f;
            fadeImage.color = c;
            logo.color = c;
            yield return null;
        }
        //fadeImage.gameObject.SetActive(false);
        //logo.gameObject.SetActive(false);

        fadeImage.enabled = false;
        logo.enabled = false;
        hasLoaded = true;
        canUseBackButton = true;
        EnableRaycaster();
    }

    private void EnableRaycaster()
    {
        GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();
        raycaster.enabled = true;
    }
}
