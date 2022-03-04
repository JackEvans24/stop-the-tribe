using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutroSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup spriteCanvas;
    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup text;
    [SerializeField] private AudioSource partyBlower;

    [Header("Timings")]
    [SerializeField] private float shortInterval = 0.6f;
    [SerializeField] private float longInterval = 2f;
    [SerializeField] private float spriteFade = 1f;

    [Header("Images")]
    [SerializeField] private Sprite[] dinoSprites;
    [SerializeField] private Sprite finalSprite;

    void Start()
    {
        StartCoroutine(this.Scene());
    }

    private IEnumerator Scene()
    {
        yield return this.spriteCanvas.DOFade(1f, this.spriteFade).WaitForCompletion();

        foreach (var sprite in this.dinoSprites)
        {
            yield return new WaitForSeconds(this.shortInterval);

            this.image.sprite = sprite;
        }

        yield return new WaitForSeconds(this.longInterval);

        yield return this.spriteCanvas.DOFade(0f, this.spriteFade).WaitForCompletion();
        yield return new WaitForSeconds(this.longInterval);
        this.image.sprite = this.finalSprite;

        this.partyBlower.Play();
        yield return this.spriteCanvas.DOFade(1f, this.spriteFade).WaitForCompletion();
        yield return new WaitForSeconds(this.longInterval);

        this.text.DOFade(1f, this.spriteFade).WaitForCompletion();
        yield return new WaitForSeconds(this.longInterval * 2);

        SceneManager.LoadScene((int)Scenes.MainMenu);
    }
}
