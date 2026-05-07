
using UnityEngine;


[System.Serializable]
public class WaveEvent : MonoBehaviour
{
    
    public float triggerTime;
    public WaveSO waveToSpawn;
    public Transform spawnRoot;
    public waveEventsType waveEvents;
    public BezierPath entryPath;
    public BezierPath exitPath;
    public Vector3 spawnPOS;

    

    public enum waveEventsType
    {
        SpawnWave,
        StartBoss,
        StartDialog,
        ChangeMusic
    }

   
}
