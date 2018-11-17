using UnityEngine;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;
using DG.Tweening;

public class MessageManager : MonoBehaviour
{
    public CanvasGroup[] groupsToHide;
    public GameObject[] objectsToHide;
    public ParticleSystem newShapeParticle;
    public GameObject newShapeMessage;
    public Text tokensText;
    
    public Text errorMessage;
    public Image errorBackground;
    public Animator anim;
    public GraphicRaycaster raycaster;


    public float timeForErrorMessage = 3;

    public bool isNewShapeMessageHidden;
    public bool isInNewShapesMessage;

    public AudioClip messageSound;
    public AudioClip newShapeUnlockedMessageSound;

    private BlurOptimized blur;

    private bool beginShowingError;
    private Tween showErrorTween;
    private float timeCounter;
    private bool wasBlurEnabled;
    public void Start()
    {
        blur = Camera.main.GetComponent<BlurOptimized>();
    }

    public void Update()
    {
        if (beginShowingError)
        {
            timeCounter += Time.unscaledDeltaTime;
            if (!showErrorTween.IsPlaying() && timeCounter > timeForErrorMessage)
            {
                errorMessage.DOFade(0, 1);
                errorBackground.DOFade(0, 1);
                ShowObjects();

                raycaster.enabled = true;
                beginShowingError = false;

                if (!wasBlurEnabled)
                {
                    blur.enabled = false;
                }
                else
                {
                    wasBlurEnabled = false;
                }
            }
        }
    }

    public void HideNewShapeMessage()
    {
        ShowObjects();
        anim.SetBool("ShowNewShapeMessage", false);
        newShapeMessage.SetActive(false);
        newShapeParticle.Stop();
        isNewShapeMessageHidden = true;
        isInNewShapesMessage = false;
        if (!wasBlurEnabled)
        {
            blur.enabled = false;
        }
        else
        {
            wasBlurEnabled = false;
        }
    }

    public void ShowErrorMessage(string message)
    {
        this.errorMessage.text = message;
        showErrorTween = errorMessage.DOFade(1, 2);
        errorBackground.DOFade(1, 2);
        beginShowingError = true;
        timeCounter = 0;
        raycaster.enabled = false;
        MainAuidoManager.mainAudio.PlaySound(messageSound, 0.5f);
        if (blur.enabled)
        {
            wasBlurEnabled = true;
        }
        else
        {
            blur.enabled = true;
            wasBlurEnabled = false;
        }
        HideObjects();
    }

    public void ShowNewUnlockedShapeMessage()
    {
        tokensText.text = PlayerInfo.info.mysteryTokens + "/" + PlayerInfo.info.maxMysteryTokens;
        anim.SetBool("ShowNewShapeMessage", true);
        isInNewShapesMessage = true;
        newShapeParticle.Play();
        MainAuidoManager.mainAudio.PlaySound(newShapeUnlockedMessageSound, 0.6f);
    }

    public void HideObjects()
    {
        int length = groupsToHide.Length;
        for (int i = 0; i < length; i++)
        {
            groupsToHide[i].alpha = 0;
            groupsToHide[i].blocksRaycasts = false;
        }

        length = objectsToHide.Length;
        for (int i = 0; i < length; i++)
        {
            objectsToHide[i].SetActive(false);
        }
       // Time.timeScale = 0;
    }

    public void ShowObjects()
    {
        int length = groupsToHide.Length;
        for (int i = 0; i < length; i++)
        {
            groupsToHide[i].alpha = 1;
            groupsToHide[i].blocksRaycasts = true;
        }

        length = objectsToHide.Length;
        for (int i = 0; i < length; i++)
        {
            objectsToHide[i].SetActive(true);
        }
       // Time.timeScale = 1;
    }

    public void GoToStore()
    {
        LoadingManager.Load(2);
    }
}
