using System.IO;
using UnityEngine;

public class WaveEnemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum Phase
    {
        Entry,
        Formation,
        Idle,
        FlyOff
       

    }

    public enum EndBehavior
    {
        Die,
        Formation,
        FlyOffRandom

    }

    public Phase currentPhase {  get; private set; }

    private BezierPath entryPath;
    private float speed;
    private float entryStartOffset;
    private Transform formationCenter;
    private Vector3 slotOffset;


    [SerializeField]
    private int maxHP;
    private int currentHP;
    [SerializeField]
    private int pointDropCount;
    [SerializeField]
    private int powerDropCount;
    [SerializeField]
    private GameObject pointDropPrefab;
    [SerializeField]
    private GameObject powerDropPrefab;
    private float phaseTimer;

    private Vector2 flyDirection;
    private float flyTimer;
    private float flyLifeTime = 10f;
    private float flySpeed = 1f;
    private EndBehavior endBehavior;


    private void Awake()
    {
        currentHP = maxHP;
   
    }

    public void Init(
       BezierPath path,
       float duration,
       float startOffset,
       Transform center,
       Vector3 formationSlotOffset,
       EndBehavior behavior)
    {
        entryPath = path;
        speed = duration;
        entryStartOffset = startOffset;
        formationCenter = center;
        slotOffset = formationSlotOffset;
        endBehavior = behavior;

        phaseTimer = 0f;
        flyTimer = 0f;
  
        currentPhase = Phase.Entry;

        if (entryPath != null)
        {
            float t = Mathf.Clamp01(entryStartOffset / speed);

            if (t < 0f)
                t = 0f;

            transform.position = entryPath.GetPoint(t) + slotOffset + new Vector3(-10,0,0);
        }
    }



    public bool IsInFormation()
    {
        return currentPhase == Phase.Formation;
    }

   private void Die()
    {
        SpawnDrops();
  
        Destroy(gameObject);
    }

    private void SpawnDrops()
    {
        for (int i = 0; i < pointDropCount; i++)
        {
            if (pointDropPrefab != null)
            {
                Instantiate(pointDropPrefab, transform.position, Quaternion.identity);
            }
        }

        for (int i = 0; i < powerDropCount; i++)
        {
            if (powerDropPrefab != null)
            {
                Instantiate(powerDropPrefab, transform.position, Quaternion.identity);
            }
        }
    }

   private void Start()
    {
        SlashScript.OnSlashingSomething += OnSlashingSomething;
       
    }

    // Update is called once per frame
   private void Update()
    {
        switch (currentPhase)
        {
            case Phase.Entry:
                UpdateEntry();
                break;

            case Phase.Formation:
                UpdateFormation();
                break;
            case Phase.Idle:
                break;
            case Phase.FlyOff:
                UpdateFlyOff();
                break;

        }
    }



    private void UpdateEntry()
    {
        if (entryPath == null)
        {
            
            return;
        }

        phaseTimer += Time.deltaTime;

        float t = (phaseTimer + entryStartOffset) / speed;

        if (t < 0f)
            return;

        t = Mathf.Clamp01(t);
        transform.position = entryPath.GetPoint(t) + slotOffset;

        if (t >= 1f)
        {
            
            switch (endBehavior)
            {
                case EndBehavior.Die:
                    Die();
                    break;

                case EndBehavior.Formation:
                    if (formationCenter != null)
                        transform.position = formationCenter.position + slotOffset;

                    currentPhase = Phase.Formation;
                    break;

                case EndBehavior.FlyOffRandom:
                    RandomExit();
                    break;
            }
        }
    }

    private void UpdateFormation()
    {
        if (formationCenter == null)
            return;

        transform.position = formationCenter.position + slotOffset;
    }


    private void RandomExit()
    {
        Vector2 p1 = entryPath.GetPoint(0.95f);
        Vector2 p2 = entryPath.GetPoint(1f);
        Vector2 tangent = (p2 - p1).normalized;

        float angle = Random.Range(-50f, 50f);
        flyDirection = (Quaternion.Euler(0f, 0f, angle) * tangent).normalized;

        flyTimer = 0f;
        currentPhase = Phase.FlyOff;
    }

    private void UpdateFlyOff()
    {
        flyTimer += Time.deltaTime;
        transform.position += (Vector3)(flyDirection * flySpeed * Time.deltaTime);

        if (flyTimer >= flyLifeTime)
        {
            Die();
        }
    }

    private void OnSlashingSomething(object sender, SlashScript.OnSlashingSomethingArgs e)
    {

        if (e.TargetHit.Equals(gameObject)) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SlashScript.OnSlashingSomething -= OnSlashingSomething;
    }
}
