using UnityEngine;
using TMPro;
using System.Collections;

public class MessagePopup : MonoBehaviour
{
    private static MessagePopup _instance;
    public static MessagePopup Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<MessagePopup>();
            }
            return _instance;
        }
    }

    [Header("Popup Settings")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private bool autoHide = true;

    [Header("Animation")]
    [SerializeField] private bool useFadeAnimation = true;
    [SerializeField] private bool useScaleAnimation = false;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio")]
    [SerializeField] private AudioClip popupSound;
    [SerializeField] private AudioSource audioSource;

    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;
    private bool isShowing = false;
    private Vector3 originalScale;

    void Start()
    {
        if (popupPanel != null)
        {
            canvasGroup = popupPanel.GetComponent<CanvasGroup>();

            if (canvasGroup == null && useFadeAnimation)
            {
                canvasGroup = popupPanel.AddComponent<CanvasGroup>();
            }

            originalScale = popupPanel.transform.localScale;

            if (canvasGroup != null && useFadeAnimation)
            {
                canvasGroup.alpha = 0;
            }

            popupPanel.SetActive(false);
        }

        if (popupSound != null && audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void ShowPopup(string message)
    {
        // ✅ ALWAYS override current popup
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(DisplayPopup(message));
    }

    public void ShowPopup(string message, float duration)
    {
        displayDuration = duration;
        ShowPopup(message);
    }

    public void ShowPopup(string message, float duration, bool autoHide)
    {
        displayDuration = duration;
        this.autoHide = autoHide;
        ShowPopup(message);
    }

    private IEnumerator DisplayPopup(string message)
    {
        isShowing = true;

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (popupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(popupSound);
        }

        popupPanel.SetActive(true);

        // Fade In
        if (useFadeAnimation && canvasGroup != null)
        {
            float t = 0;
            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInDuration);
                yield return null;
            }
            canvasGroup.alpha = 1;
        }

        // Scale In
        if (useScaleAnimation)
        {
            float t = 0;
            popupPanel.transform.localScale = Vector3.zero;

            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                float scale = scaleCurve.Evaluate(t / fadeInDuration);
                popupPanel.transform.localScale = originalScale * scale;
                yield return null;
            }

            popupPanel.transform.localScale = originalScale;
        }

        // Wait
        if (autoHide)
        {
            yield return new WaitForSeconds(displayDuration);

            // Fade Out
            if (useFadeAnimation && canvasGroup != null)
            {
                float t = 0;
                while (t < fadeOutDuration)
                {
                    t += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutDuration);
                    yield return null;
                }
                canvasGroup.alpha = 0;
            }

            // Scale Out
            if (useScaleAnimation)
            {
                float t = 0;
                Vector3 startScale = popupPanel.transform.localScale;

                while (t < fadeOutDuration)
                {
                    t += Time.deltaTime;
                    float scale = 1 - scaleCurve.Evaluate(t / fadeOutDuration);
                    popupPanel.transform.localScale = startScale * scale;
                    yield return null;
                }

                popupPanel.transform.localScale = Vector3.zero;
            }

            popupPanel.SetActive(false);

            if (useScaleAnimation)
            {
                popupPanel.transform.localScale = originalScale;
            }
        }

        isShowing = false;
        currentCoroutine = null;
    }

    public void HidePopup()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        isShowing = false;

        if (useScaleAnimation && popupPanel != null)
        {
            popupPanel.transform.localScale = originalScale;
        }
    }

    public bool IsShowing()
    {
        return isShowing;
    }

    public void SetMessageColor(Color color)
    {
        if (messageText != null)
        {
            messageText.color = color;
        }
    }

    public void SetFontSize(float fontSize)
    {
        if (messageText != null)
        {
            messageText.fontSize = fontSize;
        }
    }
}