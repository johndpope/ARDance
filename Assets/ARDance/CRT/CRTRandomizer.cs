﻿using UnityEngine;
using Random = UnityEngine.Random;

public class CRTRandomizer
{
    float totalTime;
    float time;
    float noisePower;
    float baseNoisePower;
    float noisyTime;
    Vector2 offset = Vector2.zero;
    Vector2 baseOffset = Vector2.zero;

    private readonly int PropertyID_NoiseX = Shader.PropertyToID("_NoiseX");
    private readonly int PropertyID_RGBNoise = Shader.PropertyToID("_RGBNoise");
    private readonly int PropertyID_SinNoiseScale = Shader.PropertyToID("_SinNoiseScale");
    private readonly int PropertyID_SinNoiseWidth = Shader.PropertyToID("_SinNoiseWidth");
    private readonly int PropertyID_SinNoiseOffset = Shader.PropertyToID("_SinNoiseOffset");
    private readonly int PropertyID_ScanLineSpeed = Shader.PropertyToID("_ScanLineSpeed");
    private readonly int PropertyID_ScanLineTail = Shader.PropertyToID("_ScanLineTail");
    private readonly int PropertyID_Offset = Shader.PropertyToID("_Offset");

    public void ApplyPrameter(Material mat)
    {
        float t = time / totalTime;
        float nt = Mathf.Clamp01(t / noisyTime);
        float np = baseNoisePower + noisePower * (1 - nt);
        mat.SetFloat(PropertyID_NoiseX, np * 0.5f);
        mat.SetFloat(PropertyID_RGBNoise, np * 0.7f);
        mat.SetFloat(PropertyID_SinNoiseScale, np * 1.0f);
        mat.SetFloat(PropertyID_SinNoiseOffset, Time.deltaTime * 2);
        mat.SetVector(PropertyID_Offset, baseOffset + offset * (np + baseNoisePower * t * 4));
        time += Time.deltaTime;
    }

    public void SetParameter(Material mat, float t)
    {
        totalTime = t;
        time = 0;
        noisePower = Random.Range(0.0f, 1.0f);
        baseNoisePower = Mathf.Clamp01(Random.Range(-0.01f, 0.01f));
        noisyTime = Random.Range(0.0f, 0.5f);
        mat.SetFloat(PropertyID_SinNoiseWidth, Random.Range(0.0f, 30.0f));
        offset = Vector2.right * Random.Range(-5.0f, 5) + Vector2.up * Random.Range(-5.0f, 5);
        baseOffset = (Vector2.right * Random.Range(-1.0f, 1) + Vector2.up * Random.Range(-1.0f, 1)) * 0.05f;
    }
}