using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private SpriteRenderer hitboxSprite;
    private void Start()
    {
        hitboxSprite = GetComponent<SpriteRenderer>();
        hitboxSprite.enabled = false;
    }

    private void Update()
    {
        switch (Player.Instance.moveState)
            {
                default:
            case Player.MoveState.Normal:
                GetComponent<Collider2D>().enabled = true;
                hitboxSprite.enabled = true;
                break;
            case Player.MoveState.Focused:
                GetComponent<Collider2D>().enabled = true;
                hitboxSprite.enabled = true;
                break;     
            case Player.MoveState.Death:
                hitboxSprite.enabled = false;
                GetComponent<Collider2D>().enabled = false;
                break;     
            }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.GetComponentInParent<Bullet>() != null && Player.Instance.moveState != Player.MoveState.Death)
        {
            other.GetComponentInParent<Bullet>().DespawnBullet();
            if (Player.Instance != null)
            {
                Player.Instance.moveState = Player.MoveState.Death;
                Player.Instance.Death();
                
            } 
        }
    }
}
