using UnityEngine;

public class PlayerBulletSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private BulletPool bulletPool;

    private const string PLAYER_BULLET = "Player Bullet";

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private float shootSoundCooldown = 0.05f; // prevents rapid fire from creating too many overlapping sounds

    private const string SFXKey = "SFXVolume"; // must match the key used by the main menu sfx slider
    private float lastShootSoundTime;

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
        Bullet bullet = bulletPool.GetBullet();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;
        //We change it later once we get enemies at main scene.
       // bullet.gameObject.layer = 2;
        bullet.Init(bulletTypeSO, direction.normalized);

        PlayShootSound(); // plays shooting sound after successfully spawning a bullet

    }

    private void PlayShootSound()
    {
        if (sfxSource == null || shootClip == null) 
        {
            return; 
        }

        if (Time.time < lastShootSoundTime + shootSoundCooldown) // checks if the sound was played too recently
        {
            return; 
        }

        lastShootSoundTime = Time.time; // records current time as the latest sound time
        sfxSource.PlayOneShot(shootClip); // plays one shooting sound without interrupting other sounds
    }
}
