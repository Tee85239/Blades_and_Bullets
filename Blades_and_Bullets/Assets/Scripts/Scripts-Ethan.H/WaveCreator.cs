
using UnityEngine;

public class WaveCreator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private GameObject waveControllerPrefab;
    [SerializeField] private GameObject formationPrefab;

    public void RunWave(WaveSO waveSO, Vector3 position, BezierPath entryPath)
    {
        GameObject formationObj = Instantiate(formationPrefab, position, Quaternion.identity);
        var formation = formationObj.GetComponent<WaveFormationControler>();

        GameObject waveObj = Instantiate(waveControllerPrefab, position, Quaternion.identity);
        var controller = waveObj.GetComponent<WaveController>();

        controller.Initialize(waveSO, bulletPool, formation, entryPath);
    }
}
