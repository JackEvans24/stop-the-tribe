using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [Header("References")]
    [SerializeField] protected Dinosaur dinosaur;

    [Header("Game Over")]
    [SerializeField] private float gameOverTime;
    [SerializeField] private SpriteRenderer endGameScreen;

    private bool isGameOver;
    public static bool IsGameOver { get => instance.isGameOver; }

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        this.isGameOver = false;
    }

    public static void AddDinoHealthPoint() => instance.dinosaur.AddHealth();

    public static void GameOver() => instance.StartCoroutine(instance.GameOver_Local());

    private IEnumerator GameOver_Local()
    {
        if (this.isGameOver)
            yield break;

        this.isGameOver = true;

        yield return this.endGameScreen.DOFade(1f, this.gameOverTime).WaitForCompletion();

        SceneManager.LoadScene((int)Scenes.Outro);
    }
}
