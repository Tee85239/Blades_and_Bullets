using System;
using System.Collections.Generic;

using UnityEngine;



public class WaveTimeline : MonoBehaviour
{
    [SerializeField]
    private List<WaveEvent> waveEvents = new();
    [SerializeField]
    private WaveCreator waveCreator;

    public static EventHandler onWaveRepeat;

    private float stageTimer;
    private bool isPlaying;
    private int nextEvent;

    private void Awake()
    {
        if (waveCreator == null)
        {
            Debug.LogError("WaveTimeline missing WaveCreator reference.", this);
            enabled = false;
            return;
        }

        waveEvents.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));
        stageTimer = 0f;
        nextEvent = 0;
        isPlaying = true;
    }


    private void Start()
    {
        GameControllerScript.OnChangeWaveTimer += OnChangeWaveTimer;
    }

    private void OnDestroy()
    {
        GameControllerScript.OnChangeWaveTimer -= OnChangeWaveTimer;
    }

    private void OnChangeWaveTimer(object sender, GameControllerScript.OnChangeWaveTimerArgs e)
    {
        stageTimer = e.newTimer;
        isPlaying = e.isPlayingValue;
    }

    // Update is called once per frame
   private void Update()
    {
        if (!isPlaying) return;

        stageTimer += Time.deltaTime;

        while (nextEvent < waveEvents.Count &&
               stageTimer >= waveEvents[nextEvent].triggerTime)
        {
            FireEvent(waveEvents[nextEvent]);
            nextEvent++;
        }

        if (stageTimer > 55f)
        {
            onWaveRepeat?.Invoke(this, EventArgs.Empty);
            waveEvents.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));
            stageTimer = 0f;
            nextEvent = 0;
        }
    }


   private void FireEvent(WaveEvent waveEvent)
    {
        switch (waveEvent.waveEvents)
        {
            case WaveEvent.waveEventsType.SpawnWave:
                if (waveEvent.waveToSpawn != null)
                {
                    waveCreator.RunWave(
                   waveEvent.waveToSpawn,
                   waveEvent.spawnPOS,
                   waveEvent.entryPath
               );
                }
                break;


            case WaveEvent.waveEventsType.ChangeMusic:
                Debug.Log("Change music to: ");
                break;

            case WaveEvent.waveEventsType.StartBoss:
                Debug.Log("Start boss!");
                break;

        }

    }

    public void Play()
    {
        isPlaying = true;
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void RestartTimeline()
    {
        stageTimer = 0f;
        nextEvent = 0;
        isPlaying = true;
    }

}
