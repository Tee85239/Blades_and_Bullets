using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BulletPool pool;
    public GameObject enemy;
    public Player player;



    private void Start()
    {
       enemy.GetComponentInChildren<BulletSpawner>().Init(pool);
        //player.GetComponentInChildren<PlayerBulletSpawner>().Init(pool);
        
        
    }
    void Update()
    {
        
    }
}
