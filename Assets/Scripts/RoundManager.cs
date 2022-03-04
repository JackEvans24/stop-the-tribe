using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    private static RoundManager instance;

    [Header("Rounds")]
    [SerializeField] private HUDUpdater hud;
    [SerializeField] private SpawnPoint[] spawnPoints;

    [Header("Upgrades")]
    [SerializeField] protected UpgradePoint[] upgradePoints;
    [SerializeField] protected Upgrade[] upgrades;
    [SerializeField] private float endOfRoundInterval = 3f;
    [SerializeField] private float upgradeInterval = 1f;
    [SerializeField] private float postUpgradeInterval = 5f;

    private bool newRoundStarted;
    private int currentRound;
    private int currentRoundEnemies;
    private int currentDeadEnemies;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        this.currentRound = 0;
        StartCoroutine(this.NewRound());
    }

    public static void LogKill() => instance.LogKill_Local();

    private void LogKill_Local()
    {
        this.currentDeadEnemies++;

        if (this.currentRoundEnemies < 0 || this.currentDeadEnemies < this.currentRoundEnemies)
            return;
        if (this.spawnPoints.Any(spawnPoint => !spawnPoint.AllRoundEnemiesSpawned))
            return;
        if (GameManager.IsGameOver)
            return;

        StartCoroutine(this.SetUpgrades());
    }

    private IEnumerator SetUpgrades()
    {
        yield return new WaitForSeconds(this.endOfRoundInterval);

        var nextRoundUpgrades = new List<Upgrade>();
        for (var i = 0; i < this.upgradePoints.Length; i++)
        {
            var upgrade = this.upgrades
                .OrderBy(u => Random.Range(0f, 1f))
                .FirstOrDefault(u => nextRoundUpgrades?.Any(nru => nru.Type == u.Type) != true);
            nextRoundUpgrades.Add(upgrade);
        }

        for (var i = 0; i < this.upgradePoints.Length; i++)
        {
            this.upgradePoints[i].SetUp(nextRoundUpgrades.ElementAt(i));

            yield return new WaitForSeconds(this.upgradeInterval);
        }
    }

    public static void UpgradeChosen() => instance.StartCoroutine(instance.NewRound());

    private IEnumerator NewRound()
    {
        if (this.newRoundStarted)
            yield break;
        this.newRoundStarted = true;

        if (this.currentRound > 0)
        {
            foreach (var upgradePoint in this.upgradePoints)
                upgradePoint.Hide();

            yield return new WaitForSeconds(this.postUpgradeInterval);
        }

        this.currentRound++;
        this.currentRoundEnemies = -1;
        this.currentDeadEnemies = 0;

        this.hud.UpdateRoundText(this.currentRound);

        var enemies = 0;

        foreach (var spawnPoint in this.spawnPoints)
            enemies += spawnPoint.StartRoundSpawn(this.currentRound);

        this.currentRoundEnemies = enemies;

        this.newRoundStarted = false;
    }
}
