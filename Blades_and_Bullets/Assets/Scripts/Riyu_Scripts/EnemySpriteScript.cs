using UnityEngine;

public class EnemySpriteScript : MonoBehaviour
{
    private SpriteRenderer sprite;
    private GameObject parent;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        parent = transform.parent.gameObject;

    }
    // Update is called once per frame
    void Update()
    {
        if (parent.transform.position.x < -9.25f  || parent.transform.position.x > 3.25f)
        {
            sprite.enabled = false;
        } else
        {
            sprite.enabled = true;
        }

    }
}
