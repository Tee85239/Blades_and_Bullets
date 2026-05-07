using UnityEngine;

public class BezierPath : MonoBehaviour
{
    public Transform[] points;

    [SerializeField] private int sampleCount = 100;

    private Vector3[] sampledPoints;
    private float[] cumulativeLengths;
    private float totalLength;


    private void Awake()
    {
        BuildLengthTable();
    }

    private void OnValidate()
    {
        BuildLengthTable();
    }

    public Vector3 GetPointByDistance(float normalizedDistance)
    {
        if (sampledPoints == null || sampledPoints.Length == 0)
            BuildLengthTable();

        if (sampledPoints == null || sampledPoints.Length == 0)
            return transform.position;

        normalizedDistance = Mathf.Clamp01(normalizedDistance);

        float targetLength = normalizedDistance * totalLength;

        for (int i = 1; i < cumulativeLengths.Length; i++)
        {
            if (cumulativeLengths[i] >= targetLength)
            {
                float segmentLength = cumulativeLengths[i] - cumulativeLengths[i - 1];

                if (segmentLength <= 0.0001f)
                    return sampledPoints[i];

                float localT = (targetLength - cumulativeLengths[i - 1]) / segmentLength;
                return Vector3.Lerp(sampledPoints[i - 1], sampledPoints[i], localT);
            }
        }

        return sampledPoints[sampledPoints.Length - 1];
    }

    private void BuildLengthTable()
    {
        if (points == null || points.Length < 4)
        {
            sampledPoints = null;
            cumulativeLengths = null;
            totalLength = 0f;
            return;
        }

        sampledPoints = new Vector3[sampleCount + 1];
        cumulativeLengths = new float[sampleCount + 1];

        sampledPoints[0] = GetPoint(0f);
        cumulativeLengths[0] = 0f;

        float runningLength = 0f;

        for (int i = 1; i <= sampleCount; i++)
        {
            float t = i / (float)sampleCount;
            sampledPoints[i] = GetPoint(t);
            runningLength += Vector3.Distance(sampledPoints[i - 1], sampledPoints[i]);
            cumulativeLengths[i] = runningLength;
        }

        totalLength = runningLength;
    }

    public Vector3 GetPoint(float t)
    {
        int numSegments = (points.Length - 1) / 3;

        int segmentIndex = Mathf.Min(Mathf.FloorToInt(t * numSegments), numSegments - 1);

        float localT = (t * numSegments) - segmentIndex;

        int i = segmentIndex * 3;

        return CalculateBezierPoint(
            localT,
            points[i].position,
            points[i + 1].position,
            points[i + 2].position,
            points[i + 3].position
        );
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;

        return u * u * u * p0 +
               3 * u * u * t * p1 +
               3 * u * t * t * p2 +
               t * t * t * p3;
    }

    void OnDrawGizmos()
    {
        if (points == null || points.Length < 4) return;

        Vector3 prevPoint = points[0].position;

        for (float t = 0; t <= 1f; t += 0.02f)
        {
            Vector3 point = GetPoint(t);
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}