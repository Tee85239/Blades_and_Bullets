using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    


    private BulletPool bulletPool;

    [SerializeField] private AudioSource sfxSource; 
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private float shootSoundCooldown = 0.05f; // prevents bullet hell patterns from playing too many sounds at once

    private const string SFXKey = "SFXVolume"; 
    private float lastShootSoundTime; 


    public float PatternAngle {  get; set; }

    private void Start()
    {
        sfxSource.volume = PlayerPrefs.GetFloat(SFXKey, 1f); // applies saved sfx volume from the main menu options slider
    }
    public void Init(BulletPool pool)
    {
        bulletPool = pool;

    }




    public void Fire(BulletTypeSO bulletTypeSO, Vector2 direction) 
    
    {
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle - 90f);

        Fire(bulletTypeSO, direction, rot);

    }

    public void Fire(BulletTypeSO bulletTypeSO, Vector2 direction, Quaternion rotation)
    {
        Bullet bullet = bulletPool.GetBullet();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = rotation;
        bullet.Init(bulletTypeSO, direction.normalized);

        PlayShootSound(); // plays shooting sound after successfully spawning a bullet
    }


    private void PlayShootSound() // plays enemy shooting sound with cooldown protection
    {
        if (sfxSource == null || shootClip == null) // checks if audio references are missing
        {
            return; // exits so missing audio does not cause errors
        }

        if (Time.time < lastShootSoundTime + shootSoundCooldown) // checks if the sound was played too recently
        {
            return; // skips sound to prevent audio spam
        }

        lastShootSoundTime = Time.time; // records current time as the latest sound time
        sfxSource.PlayOneShot(shootClip); // plays one shooting sound without interrupting other sounds
    }

}
