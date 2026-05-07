using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private BaseBulletPattern bulletPattern;
  //  [SerializeField]
   // private BaseBulletPattern bulletPattern2;
   // [SerializeField]
   // private float fireRate2;
    

   
    private float timer;
    private float timer2;

    
    public float getFireRate(int fireRate)
    {
        return fireRate;
    }

    // Update is called once per frame
    private void Update()
    {
       

        if (bulletPattern == null)
        {
         
            return;
        }
      // if (bulletPattern2 == null)
      //  {
       //    return;
     //  }


        timer += Time.deltaTime;
      //  timer2 += Time.deltaTime;

        if (timer >= fireRate) {
            
            timer = 0f;
            
            bulletPattern.FirePattern();
        }

       
       // if (timer2 >= fireRate2) 
       // {
        //    timer2 = 0f;

         //  bulletPattern2.FirePattern();
        
    //   }
        
    }
}
