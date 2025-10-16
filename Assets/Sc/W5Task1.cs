using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class W5Task1 : MonoBehaviour
{
    [Header("UI References")]
    public RawImage ImageDisplay;
    public RawImage HistogramDisplay;

    [Header("Image Settings")]
    public Texture2D sourceImage;
    [Range(-1f, 1f)] public float brightness = 0f;
    [Range(0f, 2f)] public float contrast = 1f;

    Texture2D modifiedImage;

    // Delegate definition
    delegate Color ColorOperation(Color color, float amount);

    void Start()
    {
        if (sourceImage == null)
        {
            Debug.LogError("No source image assigned!");
            return;
        }

        // Luo uusi tekstuuri ja kopioi pikselit turvallisesti
        modifiedImage = new Texture2D(sourceImage.width, sourceImage.height, TextureFormat.RGBA32, false);
        modifiedImage.SetPixels(sourceImage.GetPixels());
        modifiedImage.Apply();

        ImageDisplay.texture = modifiedImage;

        UpdateImageAndHistogram();
    }


    void Update()
    {
        // Update image & histogram every frame if settings change (for real-time preview)
        UpdateImageAndHistogram();
    }

    void UpdateImageAndHistogram()
    {
        // Apply brightness and contrast in sequence using delegates
        Texture2D contrastAdjusted = ImageOperation(sourceImage, contrast, AdjustContrast);
        Texture2D finalImage = ImageOperation(contrastAdjusted, brightness, AdjustBrightness);

        ImageDisplay.texture = finalImage;
        Debug.Log($"First pixel color: {finalImage.GetPixel(0, 0)}");
        HistogramDisplay.texture = CreateGrayscaleHistogram(finalImage);
    }

    // ========== IMAGE OPERATION MASTER FUNCTION ==========
    Texture2D ImageOperation(Texture2D original, float amount, ColorOperation operation)
    {
        Texture2D result = new Texture2D(original.width, original.height);

        for (int y = 0; y < original.height; y++)
        {
            for (int x = 0; x < original.width; x++)
            {
                Color color = original.GetPixel(x, y);
                Color newColor = operation(color, amount);
                result.SetPixel(x, y, newColor);
            }
        }

        result.Apply();
        return result;
    }

    // ========== OPERATION FUNCTIONS ==========
    Color AdjustBrightness(Color color, float amount)
    {
        // amount: -1 (darker) to +1 (brighter)
        color.r = Mathf.Clamp01(color.r + amount);
        color.g = Mathf.Clamp01(color.g + amount);
        color.b = Mathf.Clamp01(color.b + amount);
        return color;
    }

    Color AdjustContrast(Color color, float amount)
    {
        // amount: 1 = normal, <1 = lower contrast, >1 = higher contrast
        float mid = 0.5f;
        color.r = Mathf.Clamp01((color.r - mid) * amount + mid);
        color.g = Mathf.Clamp01((color.g - mid) * amount + mid);
        color.b = Mathf.Clamp01((color.b - mid) * amount + mid);
        return color;
    }

    // ========== HISTOGRAM CREATION ==========
    Texture2D CreateGrayscaleHistogram(Texture2D texture)
    {
        int width = 256;
        int height = 100;
        Texture2D histogram = new Texture2D(width, height);
        float[] counts = new float[width];

        // Count grayscale values
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = texture.GetPixel(x, y);
                int gray = Mathf.Clamp((int)(((color.r + color.g + color.b) / 3f) * 255f), 0, 255);
                counts[gray]++;
            }
        }

        // Find max for normalization
        float maxCount = Mathf.Max(counts);

        // Clear texture to black
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.black;

        // Draw histogram as white bars
        for (int x = 0; x < width; x++)
        {
            int barHeight = Mathf.RoundToInt((counts[x] / maxCount) * (height - 1));
            for (int y = 0; y < barHeight; y++)
            {
                pixels[x + y * width] = Color.white;
            }
        }

        histogram.SetPixels(pixels);
        histogram.Apply();
        return histogram;
    }
}
