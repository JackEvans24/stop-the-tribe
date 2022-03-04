using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup background;
    [SerializeField] private CanvasGroup page;
    [SerializeField] private CanvasGroup textCanvas;
    [SerializeField] private TMP_Text label;

    [Header("Timings")]
    [SerializeField] private float fadeInterval = 0.6f;
    [SerializeField] private float fadeSpeed = 0.5f;
    [SerializeField] private float readTime = 3f;

    [Header("Text")]
    [SerializeField, TextArea(1, 5)] private string[] lines;

    void Start()
    {
        StartCoroutine(this.Scene());
    }

    private IEnumerator Scene()
    {
        yield return page.DOFade(1, this.fadeSpeed).WaitForCompletion();

        yield return new WaitForSeconds(this.fadeInterval);

        foreach (var line in lines)
        {
            label.text = line;

            yield return textCanvas.DOFade(1, this.fadeSpeed).WaitForCompletion();

            yield return new WaitForSeconds(this.readTime);

            yield return textCanvas.DOFade(0, this.fadeSpeed).WaitForCompletion();

            yield return new WaitForSeconds(this.fadeInterval);
        }

        yield return page.DOFade(0, this.fadeSpeed).WaitForCompletion();

        yield return new WaitForSeconds(this.fadeInterval);

        yield return background.DOFade(0, this.fadeSpeed).WaitForCompletion();

        SceneManager.LoadScene((int)Scenes.Game);
    }
}
