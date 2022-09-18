using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public float fadeInDuration;

    public float fadeOutDuration;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeOutIn()
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }

    public IEnumerator FadeOut(float time)
    {
        while(canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float time)
    {
        while(canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }

        Destroy(gameObject); //切换场景渐入以后就销毁
    }
}
