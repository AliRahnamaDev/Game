using UnityEngine;

public class CoinsPatern : MonoBehaviour
{
    public enum PatternType
    {
        Rectangle,
        Triangle,
        Circle,
        Star,
        Spiral,
        Zigzag
    }

    [Header("General")]
    public GameObject fruitPrefab;
    public PatternType patternType;
    public bool spawnOnStart = true;
    public bool autoUpdate = false;

    [Header("Rectangle Settings")]
    public int rows = 3;
    public int cols = 3;
    public float spacingX = 1f;
    public float spacingY = 1f;

    [Header("Triangle Settings")]
    public int triangleSize = 3;
    public float triangleSpacing = 1f;

    [Header("Circle Settings")]
    public float radius = 2f;
    public int circlePoints = 12;
    public bool circleFilled = false; // ✅ جدید

    [Header("Star Settings")]
    public int starPoints = 5;
    public float starRadiusOuter = 2f;
    public float starRadiusInner = 1f;
    public float starRotation = 0f; // ✅ جدید
    public bool starFilled = false; // ✅ جدید

    [Header("Spiral Settings")]
    public int spiralPoints = 30;
    public int spiralTurns = 3;
    public float spiralSpacing = 0.5f;

    [Header("Zigzag Settings")]
    public int zigzagLength = 10;
    public float zigzagHeight = 1f;
    public float zigzagSpacing = 1f;

    private PatternType lastPatternType;
    private string lastParamsHash = "";

    void Start()
    {
        if (spawnOnStart)
            SpawnPattern();
    }

    void Update()
    {
        if (autoUpdate && Application.isPlaying)
        {
            string currentHash = GetParamsHash();
            if (currentHash != lastParamsHash || patternType != lastPatternType)
            {
                lastPatternType = patternType;
                lastParamsHash = currentHash;
                ClearOldFruits();
                SpawnPattern();
            }
        }
    }

    void ClearOldFruits()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    string GetParamsHash()
    {
        return $"{rows}_{cols}_{spacingX}_{spacingY}_{triangleSize}_{triangleSpacing}_{radius}_{circlePoints}_{circleFilled}_{starPoints}_{starRadiusOuter}_{starRadiusInner}_{starRotation}_{starFilled}_{spiralPoints}_{spiralTurns}_{spiralSpacing}_{zigzagLength}_{zigzagHeight}_{zigzagSpacing}";
    }

    public void SpawnPattern()
    {
        switch (patternType)
        {
            case PatternType.Rectangle:
                SpawnRectangle();
                break;
            case PatternType.Triangle:
                SpawnTriangle();
                break;
            case PatternType.Circle:
                SpawnCircle();
                break;
            case PatternType.Star:
                SpawnStar();
                break;
            case PatternType.Spiral:
                SpawnSpiral();
                break;
            case PatternType.Zigzag:
                SpawnZigzag();
                break;
        }
    }

    void SpawnRectangle()
    {
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                Instantiate(fruitPrefab, transform.position + new Vector3(j * spacingX, i * spacingY, 0), Quaternion.identity, transform);
    }

    void SpawnTriangle()
    {
        for (int i = 0; i < triangleSize; i++)
            for (int j = 0; j <= i; j++)
            {
                float x = (j - i * 0.5f) * triangleSpacing;
                float y = -i * triangleSpacing;
                Instantiate(fruitPrefab, transform.position + new Vector3(x, y, 0), Quaternion.identity, transform);
            }
    }

    void SpawnCircle()
    {
        if (circleFilled)
        {
            for (int r = 1; r <= circlePoints / 2; r++)
            {
                float rad = radius * r / (circlePoints / 2f);
                for (int i = 0; i < circlePoints; i++)
                {
                    float angle = i * Mathf.PI * 2 / circlePoints;
                    Vector3 pos = new Vector3(Mathf.Cos(angle) * rad, Mathf.Sin(angle) * rad, 0);
                    Instantiate(fruitPrefab, transform.position + pos, Quaternion.identity, transform);
                }
            }
        }
        else
        {
            for (int i = 0; i < circlePoints; i++)
            {
                float angle = i * Mathf.PI * 2 / circlePoints;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                Instantiate(fruitPrefab, transform.position + pos, Quaternion.identity, transform);
            }
        }
    }

    void SpawnStar()
    {
        float rotationOffset = Mathf.Deg2Rad * starRotation;
        int totalPoints = starPoints * 2;

        if (starFilled)
        {
            // رسم ستاره توپر با حلقه داخلی (شعاع از داخلی به بیرونی)
            for (int r = 0; r <= 5; r++)
            {
                float t = r / 5f;
                float rInner = Mathf.Lerp(starRadiusInner, starRadiusOuter, t);
                for (int i = 0; i < totalPoints; i++)
                {
                    float angle = i * Mathf.PI * 2 / totalPoints + rotationOffset;
                    float radius = (i % 2 == 0) ? rInner : rInner * 0.5f;
                    Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                    Instantiate(fruitPrefab, transform.position + pos, Quaternion.identity, transform);
                }
            }
        }
        else
        {
            for (int i = 0; i < totalPoints; i++)
            {
                float angle = i * Mathf.PI * 2 / totalPoints + rotationOffset;
                float r = (i % 2 == 0) ? starRadiusOuter : starRadiusInner;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0);
                Instantiate(fruitPrefab, transform.position + pos, Quaternion.identity, transform);
            }
        }
    }

    void SpawnSpiral()
    {
        float angleStep = (spiralTurns * 2 * Mathf.PI) / spiralPoints;
        for (int i = 0; i < spiralPoints; i++)
        {
            float angle = i * angleStep;
            float radius = spiralSpacing * angle / (2 * Mathf.PI); // فاصله تمیز و طبیعی‌تر
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Instantiate(fruitPrefab, transform.position + pos, Quaternion.identity, transform);
        }
    }

    void SpawnZigzag()
    {
        for (int i = 0; i < zigzagLength; i++)
        {
            float x = i * zigzagSpacing;
            float y = (i % 2 == 0) ? 0 : zigzagHeight;
            Vector3 pos = new Vector3(x, y, 0);
            Instantiate(fruitPrefab, transform.position + pos, Quaternion.identity, transform);
        }
    }
}

