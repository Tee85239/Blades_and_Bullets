using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    private IMovementType movementType;
    private BulletTypeSO bulletTypeSO;
    private BulletPool pool;
    private bool isBulletActive;
    private float age;
    private bool hasSplit;
    private Vector2 direction;
    private Transform target;
    
    CircleCollider2D circleCollider;
  
    CapsuleCollider2D capsuleCollider;
   
    

    public Vector2 Direction => direction;
    public float Age => age;
    public Transform Target => target;

    private int generation;
    private int maxSplit = 1;

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    public void Init(BulletTypeSO bulletType, Vector2 AimDirection, int amountSplit = 0, Transform homingTarget = null)
    {
        bulletTypeSO = bulletType;
        direction = AimDirection.normalized;
        age = 0f;
        hasSplit = false;
        target = homingTarget;
        generation = amountSplit;

        movementType = null;

        if (bulletTypeSO.bulletMovement != null && bulletTypeSO != null) 
        {
            movementType = bulletTypeSO.bulletMovement.CreateMovement();
            movementType?.Init(this);
        }

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        circleCollider = GetComponentInChildren<CircleCollider2D>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider2D>();

        if(circleCollider == null || capsuleCollider == null)
        {
            Debug.Log("Colliders null");
        }


       SetTriggers();


  
        if (spriteRenderer != null)
        {
            
            spriteRenderer.sprite = bulletTypeSO != null ? bulletTypeSO.sprite : null;
        }
      

        if (bulletTypeSO != null)
        {
            transform.localScale = Vector3.one * bulletTypeSO.scale;
        }

        gameObject.SetActive(true);
    }

    private void Start()
    {

        WallScript.OnWallHit += OnWallHit;
        // Player.PlayerGetsHit += OnPlayerGetsHit;

    }
    

    // private void OnPlayerGetsHit(object sender, Player.PlayerGetsHitArgs e)
    // {
    //     if(e.TargetHit.Equals(gameObject)) Destroy(gameObject);
    // }


    private void OnWallHit(object sender, WallScript.OnWallHitArgs e)
    {
        if (e.wallHitGameObjectType.Equals(gameObject)) pool.ReturnBulletToPool(e.wallHitGameObjectType.GetComponent<Bullet>());
    }


    private void OnDestroy()
    {
        WallScript.OnWallHit -= OnWallHit;
        // Player.PlayerGetsHit -= OnPlayerGetsHit;
    }

    private void SetTriggers()
    {
       capsuleCollider.enabled = false;
        circleCollider.enabled = false;

        switch (bulletTypeSO.hurtBoxType)
        {
            case BulletTypeSO.HurtBoxType.Circle:
                circleCollider.enabled = true;
                circleCollider.radius = bulletTypeSO.radius;
                circleCollider.offset = bulletTypeSO.offset;
                break;

            case BulletTypeSO.HurtBoxType.Capsule:
               capsuleCollider.enabled = true;
                capsuleCollider.size = bulletTypeSO.sizeCapsule;
                capsuleCollider.offset = bulletTypeSO.offset;
                break;

        



       }



    }

    public void SetPool(BulletPool bulletPool)
    {
        pool = bulletPool;
    }

    public void BulletSplit(Vector2 childDirection)
    {
        if (pool == null || bulletTypeSO == null || bulletTypeSO.splitedBullet == null)
            return;

        Bullet child = pool.GetBullet();
        if (child == null)
        {
            return;
        }
        child.transform.position = transform.position;
        child.transform.rotation = Quaternion.identity;
        child.Init(bulletTypeSO.splitedBullet, childDirection, generation + 1, target);
    }
    private void Update()
    {
       
  

        if (bulletTypeSO == null)
        {
            return;
        }

        age += Time.deltaTime;

        if (bulletTypeSO.canSplit && !hasSplit && age >= bulletTypeSO.splitTime)
        {
            if(generation < bulletTypeSO.splitMaxCount && bulletTypeSO.splitBulletPattern != null)
            {
                hasSplit = true;
                bulletTypeSO.splitBulletPattern.Split(this);
                DespawnBullet();
                return;
            }
        }


        if (age >= bulletTypeSO.lifetime) 
        { 
            
            DespawnBullet();
            return;
        }
        movementType?.Tick(this);

       if (gameObject.transform.position.x < -8.75f  || gameObject.transform.position.x > 2.75f)
        {
            DespawnBullet();
        }
        if (gameObject.transform.position.y < -4.8f  || gameObject.transform.position.y > 4.8f)
        {
            DespawnBullet();
        }
    }

    public void DespawnBullet()
    {

        gameObject.SetActive(false);

        if (pool != null)
        {
          
            pool.ReturnBulletToPool(this);
        }


    }

    


}
