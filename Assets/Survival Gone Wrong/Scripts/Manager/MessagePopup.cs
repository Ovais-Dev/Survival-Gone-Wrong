using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Queue Settings")]
    [SerializeField] private bool queueMessages = true;
    [SerializeField] private float queueDelay = 0.5f;

    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;
    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowing = false;
    private Vector3 originalScale;

    void Start()
    {
        // Get or add CanvasGroup for fade effects
        if (popupPanel != null)
        {
            canvasGroup = popupPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null && useFadeAnimation)
            {
                canvasGroup = popupPanel.AddComponent<CanvasGroup>();
            }

            originalScale = popupPanel.transform.localScale;

            // Initially hide the popup
            if (canvasGroup != null && useFadeAnimation)
            {
                canvasGroup.alpha = 0;
            }
            popupPanel.SetActive(false);
        }

        // Get audio source if not assigned
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
        if (queueMessages && isShowing)
        {
            messageQueue.Enqueue(message);
            return;
        }

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

        // Set the message text
        if (messageText != null)
        {
            messageText.text = message;
        }

        // Play sound
        if (popupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(popupSound);
        }

        // Show the popup panel
        popupPanel.SetActive(true);

        // Start animation
        if (useFadeAnimation && canvasGroup != null)
        {
            // Fade in
            float elapsedTime = 0;
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
                canvasGroup.alpha = alpha;
                yield return null;
            }
            canvasGroup.alpha = 1;
        }

        if (useScaleAnimation)
        {
            // Scale animation
            float elapsedTime = 0;
            popupPanel.transform.localScale = Vector3.zero;
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = scaleCurve.Evaluate(elapsedTime / fadeInDuration);
                popupPanel.transform.localScale = originalScale * scale;
                yield return null;
            }
            popupPanel.transform.localScale = originalScale;
        }

        // Wait for display duration
        if (autoHide)
        {
            yield return new WaitForSeconds(displayDuration);

            // Hide with animation
            if (useFadeAnimation && canvasGroup != null)
            {
                // Fade out
                float elapsedTime = 0;
                while (elapsedTime < fadeOutDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
                    canvasGroup.alpha = alpha;
                    yield return null;
                }
                canvasGroup.alpha = 0;
            }

            if (useScaleAnimation)
            {
                // Scale out animation
                float elapsedTime = 0;
                Vector3 startScale = popupPanel.transform.localScale;
                while (elapsedTime < fadeOutDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float scale = 1 - scaleCurve.Evaluate(elapsedTime / fadeOutDuration);
                    popupPanel.transform.localScale = startScale * scale;
                    yield return null;
                }
                popupPanel.transform.localScale = Vector3.zero;
            }

            popupPanel.SetActive(false);

            // Reset scale for next time
            if (useScaleAnimation)
            {
                popupPanel.transform.localScale = originalScale;
            }
        }

        isShowing = false;
        currentCoroutine = null;

        // Show next message in queue
        if (queueMessages && messageQueue.Count > 0)
        {
            yield return new WaitForSeconds(queueDelay);
            string nextMessage = messageQueue.Dequeue();
            ShowPopup(nextMessage);
        }
    }

    /// <summary>
    /// Immediately hide the current popup
    /// </summary>
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

        // Reset scale if needed
        if (useScaleAnimation && popupPanel != null)
        {
            popupPanel.transform.localScale = originalScale;
        }

        // Clear message queue if desired
        messageQueue.Clear();
    }

    /// <summary>
    /// Clear all queued messages
    /// </summary>
    public void ClearQueue()
    {
        messageQueue.Clear();
    }

    /// <summary>
    /// Check if a popup is currently showing
    /// </summary>
    public bool IsShowing()
    {
        return isShowing;
    }

    /// <summary>
    /// Get number of queued messages
    /// </summary>
    public int GetQueueCount()
    {
        return messageQueue.Count;
    }

    /// <summary>
    /// Set message text color
    /// </summary>
    public void SetMessageColor(Color color)
    {
        if (messageText != null)
        {
            messageText.color = color;
        }
    }

    /// <summary>
    /// Set message font size
    /// </summary>
    public void SetFontSize(float fontSize)
    {
        if (messageText != null)
        {
            messageText.fontSize = fontSize;
        }
    }
}

// Optional: Static wrapper for easy access from anywhere
public static class PopupManager
{
    private static MessagePopup mainPopup;

    public static void Initialize(MessagePopup popup)
    {
        mainPopup = popup;
    }

    public static void Show(string message)
    {
        if (mainPopup != null)
        {
            mainPopup.ShowPopup(message);
        }
        else
        {
            Debug.LogWarning("PopupManager: No MessagePopup assigned!");
        }
    }

    public static void Show(string message, float duration)
    {
        if (mainPopup != null)
        {
            mainPopup.ShowPopup(message, duration);
        }
        else
        {
            Debug.LogWarning("PopupManager: No MessagePopup assigned!");
        }
    }
}