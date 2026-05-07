using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveController : MonoBehaviour
{
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private WaveFormationControler formation;
    [SerializeField] private WaveSO waveSO;

    private readonly List<WaveEnemy> enemies = new();

    private Vector3 formationBasePosition;
    private BezierPath entryPath;
    private WaveEnemy.EndBehavior endBehavior;
    private bool spawnInFormation;

   
   
    public void Initialize(
        WaveSO data,
        BulletPool pool,
        WaveFormationControler waveFormationControler,
        BezierPath entry)
    {
        waveSO = data;
        bulletPool = pool;
        formation = waveFormationControler;
        entryPath = entry;
        endBehavior = waveSO.endBehavior;
        spawnInFormation = waveSO.spawnInFormation;

        if (formation != null && formation.center != null)
        {
            formationBasePosition = formation.center.position;
        }

        StartCoroutine(RunWave());
    }

    public void SpawnWave()
    {
        enemies.Clear();

        for (int i = 0; i < waveSO.enemyCount; i++)
        {
            GameObject obj = Instantiate(waveSO.enemyPrefab);

            var bulletSpawner = obj.GetComponentInChildren<BulletSpawner>();
            if (bulletSpawner != null)
                bulletSpawner.Init(bulletPool);

            var enemy = obj.GetComponent<WaveEnemy>();
            if (enemy == null)
            {
                Destroy(obj);
                continue;
            }

            Vector3 slotOffset = Vector3.zero;

            if (formation != null)
            {
                slotOffset = formation.GetSlotOffset(
               i,
               waveSO.enemyCount,
               waveSO.formationType,
               waveSO.slotSpacing,
               waveSO.verticalDepth
           );
            }

            if (spawnInFormation)
            {
                enemy.Init(
                    entryPath,
                    waveSO.speed,
                    0f,
                    formation != null ? formation.center : null,
                    slotOffset,
                    endBehavior
                );


            }
            else
            {

                enemy.Init(
                    entryPath,
                    waveSO.speed,
                    -i * waveSO.entryGap,
                    formation != null ? formation.center : null,
                    slotOffset,
                    endBehavior
                );

            }

            enemies.Add(enemy);
        }
    }

    private IEnumerator WaitForFormation(float timeout)
    {
        float timer = 0f;

        while (timer < timeout)
        {
            if (AllActiveEnemiesInFormation())
                yield break;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private bool AllActiveEnemiesInFormation()
    {
        bool found = false;

        foreach (var enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
                continue;

            found = true;

            if (!enemy.IsInFormation())
                return false;
        }

        return found;
    }

    private IEnumerator MoveFormationTo(Vector3 target)
    {
        if (formation == null || formation.center == null)
            yield break;

        while (Vector3.Distance(formation.center.position, target) > 0.02f)
        {
            formation.center.position = Vector3.MoveTowards(
                formation.center.position,
                target,
                waveSO.formationTravelSpeed * Time.deltaTime
            );

            yield return null;
        }

        formation.center.position = target;
    }

    private IEnumerator RunWave()
    {
        SpawnWave();

        if (endBehavior == WaveEnemy.EndBehavior.Formation && formation != null)
        {
            yield return WaitForFormation(
                waveSO.speed + waveSO.enemyCount * waveSO.entryGap + 1f
            );

            yield return MoveFormationTo(formationBasePosition + waveSO.leftOffset);
            yield return new WaitForSeconds(waveSO.waitAtLeft);

            yield return MoveFormationTo(formationBasePosition + waveSO.rightOffset);
            yield return new WaitForSeconds(waveSO.waitAtRight);

            yield return MoveFormationTo(formationBasePosition + waveSO.middleOffset);
            yield return new WaitForSeconds(waveSO.waitAtMiddle);
        }

        // Optional cleanup delay if you want the wave controller to go away later
        yield return new WaitForSeconds(2f);

        if (formation != null)
            Destroy(formation.gameObject);

        Destroy(gameObject);
    }
}
