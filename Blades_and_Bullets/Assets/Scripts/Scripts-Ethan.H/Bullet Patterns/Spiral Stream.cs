using UnityEngine;

public class SpiralStream : BaseBulletPattern


{

    [SerializeField]
    private BulletTypeSO bulletType;
    [SerializeField]
    private int arms = 3;
    [SerializeField]
    private float angle = 10f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void FirePattern()
    {
        bulletSpawner.PatternAngle += angle;

        float armSpacing = 360f/arms;

        for (int i = 0; i < arms; i++)
        {
            float angle = bulletSpawner.PatternAngle + armSpacing * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            bulletSpawner.Fire(bulletType, dir);
        }


    }



}
