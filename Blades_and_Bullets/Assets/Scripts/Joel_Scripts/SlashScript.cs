using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlashScript : MonoBehaviour
{
    public static EventHandler<OnSlashingSomethingArgs> OnSlashingSomething;

    private float damage;

    [SerializeField] private AudioClip normalSlashClip;
    [SerializeField] private AudioClip specialSlashClip;
    [SerializeField] private AudioClip bombClip;

    [SerializeField] private float specialVolumeMultiplier = 1.8f; // makes special sound louder than normal sfx


    public enum BulletType
    {
        Normal,
        Special,
        Bomb
    }

    [SerializeField] public BulletType bulletType;

    public class OnSlashingSomethingArgs : EventArgs
    {
        public GameObject TargetHit;
    }

    private float speed = 50f;

    private void Start() 
    {
        if (bulletType == BulletType.Normal) // checks for normal projectile
        {
            Destroy(gameObject, .5f); // destroys normal slash after half a second
        }
    }

    private void OnEnable() // runs every time this object becomes active
    {
        PlaySpawnSound(); // plays sound every activation, including reused special attacks
    }

    private void Update()
    {
        if (bulletType == BulletType.Normal)
        {
            transform.position += transform.up * speed * Time.deltaTime;
        }
        
 
    }

    private void PlaySpawnSound() // plays the correct spawn sound based on attack type
    {
        if (GameplayAudioManager.Instance == null) // checks if the gameplay audio manager exists in the scene
        {
            return; // exits safely if no audio manager exists
        }

        switch (bulletType) // chooses audio behavior based on attack type
        {
            case BulletType.Normal: // handles normal slash sound
                GameplayAudioManager.Instance.PlaySFX(normalSlashClip); // plays normal slash as a one-shot sound
                break; // exits normal case

            case BulletType.Special: // handles special ability sound
                GameplayAudioManager.Instance.PlaySFX(specialSlashClip, specialVolumeMultiplier); // plays special louder
                break; // exits special case

            case BulletType.Bomb: // handles bomb sound
                GameplayAudioManager.Instance.PlaySFX(bombClip); // plays bomb as a one-shot sound
                break; // exits bomb case
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.GetComponentInParent<Bullet>();
        WaveEnemy enemy = other.GetComponent<WaveEnemy>();

        switch (bulletType) 
        {
            case BulletType.Normal: 
                if (enemy != null) 
                {
                    enemy.TakeDamage(damage); 
                    OnSlashingSomething?.Invoke(this, new OnSlashingSomethingArgs { TargetHit = enemy.gameObject }); 
                    Destroy(gameObject); 
                }

                break; 

            case BulletType.Special: 
                if (enemy != null) 
                {
                    enemy.TakeDamage(damage* 2); 
                    OnSlashingSomething?.Invoke(this, new OnSlashingSomethingArgs { TargetHit = enemy.gameObject });
                }

                if (bullet != null) 
                {
                    OnSlashingSomething?.Invoke(this, new OnSlashingSomethingArgs { TargetHit = bullet.gameObject }); 
                    bullet.DespawnBullet(); 
                }

                break; 

            case BulletType.Bomb: 
                if (enemy != null) 
                {
                    enemy.TakeDamage(10); 
                }
                else if (bullet != null) 
                {
                    OnSlashingSomething?.Invoke(this, new OnSlashingSomethingArgs { TargetHit = bullet.gameObject });
                    bullet.DespawnBullet(); 
                }

                break; 
        }
    }

    public BulletType GetBulletType() 
    {
        return bulletType; 
    }

    public void SetDamage(float num) 
    {
        damage = num;
    }

}