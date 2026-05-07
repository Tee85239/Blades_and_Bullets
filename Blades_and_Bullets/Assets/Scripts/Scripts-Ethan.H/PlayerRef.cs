using UnityEngine;

public class PlayerRef : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Transform Instance;

    private void Awake()
    {
        if (transform!= null) Instance = transform;
        

    }
}
