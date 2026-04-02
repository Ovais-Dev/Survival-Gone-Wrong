using System.Collections;
using UnityEngine;
using TMPro;

public class ReloadTextUI : MonoBehaviour
{
    public TextMeshProUGUI reloadText;
    public float fadeSpeed = 2f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        reloadText.alpha = 0f;
    }

    public void ShowReloading()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeEffect());
    }

    IEnumerator FadeEffect()
    {
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * fadeSpeed;
            reloadText.alpha = Mathf.PingPong(time, 1f);
            yield return null;
        }

        reloadText.alpha = 0f;
    }
}