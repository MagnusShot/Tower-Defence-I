using System;
using UnityEngine;

//---------------------------------------------VORONOI NOISE---------------------------------------------
public static class GenerationAlgorithim
{
    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings noiseSettings, Vector2 sampleCenter)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(noiseSettings.seed);
        Vector2[] octaveOffsets = new Vector2[noiseSettings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < noiseSettings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + noiseSettings.offset.x + sampleCenter.x;
            float offsetY = prng.Next(-100000, 100000) - noiseSettings.offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= noiseSettings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < noiseSettings.octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / noiseSettings.noiseScale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / noiseSettings.noiseScale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= noiseSettings.persistance;
                    frequency *= noiseSettings.lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;

                if(noiseSettings.normalizeMode == GenerationAlgorithim.NormalizeMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (noiseSettings.normalizeMode == NormalizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }
}

[Serializable]
public struct NoiseSettings
{
    public GenerationAlgorithim.NormalizeMode normalizeMode;

    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;


    public void ValidateValues()
    {
        noiseScale = Mathf.Max(noiseScale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}


//---------------------------------------------FALLOFF GENERATOR---------------------------------------------
public static class FalloffGenerator 
{
	public static float[,] GenerateFalloffMap(int size) {
		float[,] map = new float[size,size];

		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				float x = i / (float)size * 2 - 1;
				float y = j / (float)size * 2 - 1;

				float value = Mathf.Max (Mathf.Abs (x), Mathf.Abs (y));
				map [i, j] = Evaluate(value);
			}
		}

		return map;
	}

	static float Evaluate(float value) {
		float a = 3;
		float b = 2.2f;

		return Mathf.Pow (value, a) / (Mathf.Pow (value, a) + Mathf.Pow (b - b * value, a));
	}
}


//---------------------------------------------HEIGHT MAP GENERATION---------------------------------------------
public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings_Scripta settings, Vector2 sampleCentre)
    {
        float[,] values = GenerationAlgorithim.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

        AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }
                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }

}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}

[Serializable]
public struct HeightMapSettings_Scripta
{
    public NoiseSettings noiseSettings;

    public bool useFalloff;

    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }
}