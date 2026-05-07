using UnityEngine;

public class SineBulletMovement : IMovementType
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private readonly float speed;
    private readonly float amplitude;
    private readonly float frequency;

    private float age;
    private Vector2 startPos;
    private Vector2 direction;

    public void Init(Bullet bullet)
    {
        startPos = bullet.transform.position;
        direction = bullet.Direction.normalized;
        age = 0f;

    }

    public void Tick(Bullet bullet)
    {
        age += Time.deltaTime;

        
        //
        Vector2 wavePos = startPos + direction * speed * age + new Vector2(-direction.y, direction.x) * Mathf.Sin(age * frequency) * amplitude;

        //bullet.transform.position = startPos + forward + perpendicular * offset;
        bullet.transform.position = new Vector3(wavePos.x, wavePos.y, 0.3f);
    }

    public SineBulletMovement(float speed, float amplitude, float frequency)
    {
        this.speed = speed;
        this.amplitude = amplitude;
        this.frequency = frequency;


    }


}
