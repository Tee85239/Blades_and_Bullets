using UnityEngine;

public class Bomb : MonoBehaviour
{

    void Update()
    {
        if (transform.localScale.x < 25f)
        {
            float timer = 40f * Time.deltaTime;
            transform.localScale += new Vector3(timer, timer, 0f);
        } else
        {
            Destroy(gameObject);
        } 

    }
    
}
