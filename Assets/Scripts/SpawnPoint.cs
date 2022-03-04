using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemy;

    [Header("Spawn variables")]
    [SerializeField] private int startRound = 1;
    [SerializeField] private int startCount = 5;
    [SerializeField] private int countPerRound = 2;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnOffset = 0f;
    [SerializeField] private Vector2[] resetPoints;

    public bool AllRoundEnemiesSpawned;

    private int currentCount;

    public int StartRoundSpawn(int roundNumber)
    {
        if (roundNumber < this.startRound)
        {
            this.AllRoundEnemiesSpawned = true;
            return 0;
        }
        this.AllRoundEnemiesSpawned = false;

        if (roundNumber == this.startRound)
            this.currentCount = this.startCount;
        else if (this.resetPoints.Any(reset => reset.x == roundNumber))
        {
            var resetPoint = this.resetPoints.First(reset => reset.x == roundNumber);
            this.currentCount = Mathf.FloorToInt(resetPoint.y);
        }
        else
            this.currentCount += this.countPerRound;

        StartCoroutine(this.Spawn());

        return this.currentCount;
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(this.spawnOffset);

        for (int i = 0; i < this.currentCount; i++)
        {
            Instantiate(this.enemy, this.transform.position, this.enemy.transform.rotation);

            yield return new WaitForSeconds(this.spawnInterval);
        }

        this.AllRoundEnemiesSpawned = true;
    }
}
