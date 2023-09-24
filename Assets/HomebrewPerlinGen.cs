using UnityEngine;

public class HomebrewPerlinGen : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetXIncreaser = 0f;
    public float offsetYIncreaser = 0f;
    public float scaleIncreaser = 0f;

    private Material material;
    private Texture2D noiseTexture;

    private bool needsRecalculation = true;
    private void Start()
    {
        material = GetComponent<Renderer>().material;
        GenerateNoiseTexture();
    }
    private void Update()
    {
        offsetX+= offsetXIncreaser;
        offsetY+= offsetYIncreaser;
        scale+= scaleIncreaser;

        if (needsRecalculation)
        {
            InitializeGradients();
            needsRecalculation = false;
        }

        GenerateNoiseTexture();
    }

    private void InitializeGradients()
    {
        int gridSize = width * height;
        gradients = new Vector2[gridSize];
        permutations = new int[gridSize * 2]; // Update the permutation table size
        for (int i = 0; i < gridSize; i++)
        {
            float randomAngle = Random.Range(0f, Mathf.PI * 2f);
            gradients[i] = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

            // Populate the permutation table
            permutations[i] = i;
            permutations[i + gridSize] = i;
        }

        // Shuffle the permutation table
        for (int i = gridSize - 1; i >= 0; i--)
        {
            int j = Random.Range(0, gridSize);
            int temp = permutations[i];
            permutations[i] = permutations[j];
            permutations[j] = temp;
        }
    }

    private void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;
                float sample = GeneratePerlinNoise(xCoord, yCoord);
                pixels[y * width + x] = new Color(sample, sample, sample);
            }
        }

        noiseTexture.SetPixels(pixels);
        noiseTexture.Apply();
        material.mainTexture = noiseTexture;
    }

    private float GeneratePerlinNoise(float x, float y)
    {
        int X = Mathf.FloorToInt(x) & 255;
        int Y = Mathf.FloorToInt(y) & 255;

        x -= Mathf.Floor(x);
        y -= Mathf.Floor(y);

        float u = Fade(x);
        float v = Fade(y);

        int A = permutations[X] + Y;
        int AA = permutations[A];
        int AB = permutations[A + 1];
        int B = permutations[X + 1] + Y;
        int BA = permutations[B];
        int BB = permutations[B + 1];

        float gradAA = gradients[AA % 8].x * x + gradients[AA % 8].y * y;
        float gradBA = gradients[BA % 8].x * (x - 1) + gradients[BA % 8].y * y;
        float gradAB = gradients[AB % 8].x * x + gradients[AB % 8].y * (y - 1);
        float gradBB = gradients[BB % 8].x * (x - 1) + gradients[BB % 8].y * (y - 1);

        float lerpedX1 = Lerp(gradAA, gradBA, u);
        float lerpedX2 = Lerp(gradAB, gradBB, u);

        return (Lerp(lerpedX1, lerpedX2, v) + 1) / 2;
    }

    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private float Lerp(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, t);
    }

    private Vector2[] gradients = {
        new Vector2(1, 1), new Vector2(-1, 1), new Vector2(1, -1), new Vector2(-1, -1),
        new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)
    };

    private int[] permutations = {
        151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
        140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
        247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
        57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
        74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
        60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
        65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
        200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3,
        64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82,
        85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223,
        183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101,
        155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113,
        224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193,
        238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14,
        239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176,
        115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114,
        67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
    };
}