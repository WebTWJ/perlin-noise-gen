using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    // Start is called before the first frame update

    public int width = 256;
    public int height = 256;

    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetxIncreaser = 0f;
    public float offsetyIncreaser = 0f;

    public float scale = 20f;
    public float scaleIncreaser = 0f;
    

    void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
        offsetX+=offsetxIncreaser;
        offsetY+=offsetyIncreaser;
        scale+=scaleIncreaser;
    }

    Texture2D GenerateTexture() {
        Texture2D texture = new Texture2D(width, height);

        //GENERATE A PERLIN NOISE MAP

        for(int x = 0; x < width; x ++) {
            for(int y = 0; y < height; y++) {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return(texture);
    }

    Color CalculateColor(int x, int y) {
        float xCord = (float)x / width * scale + offsetX;
        float yCord = (float)y / height * scale + offsetY;

        float sample = Mathf.PerlinNoise(xCord, yCord);
        return new Color(sample, sample, sample);
    }
}
