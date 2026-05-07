using System;
using UnityEngine;

public class PlayerBulletPattern : BaseBulletPattern
{
    [SerializeField]
    private BulletTypeSO bulletTypeSO;
    
    private Transform player;

    private void Awake()
    {
        player = PlayerRef.Instance;
    }

    public override void FirePattern()
    {
        try
        {
            Vector2 dir = (player.transform.position - bulletSpawner.transform.position).normalized;
            bulletSpawner.Fire(bulletTypeSO, dir);
        }
        catch(Exception e)
        {
            return;
        }
        
    }




}
