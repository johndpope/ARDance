using UnityEngine;

public class AudioSpectrum
{
    public float[] Spectrums { get; }
    public float[] MaxInEachUnit { get; }
    public float Max { get; private set; }
    
    private const int HIGH_FREQ = 9000; // 表示する周波数の上限
    private readonly float[] _spectrum = new float[1024];
    
    private AudioSource _audioSource;
    private int _unit;

    public AudioSpectrum(AudioSource audioSource, int num)
    {
        _audioSource = audioSource;
        _unit = HIGH_FREQ / num;
        Spectrums = new float[num];
        MaxInEachUnit = new float[num];
    }

    public void GetSpectrum()
    {
        var deltaFreq = AudioSettings.outputSampleRate / _spectrum.Length;
        _audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.Blackman);
        var upper = _unit;
        Max = 0f;
        var num = 0;
        Spectrums[num] = 0;
        MaxInEachUnit[num] = 0;
        for (int i = 0; i < _spectrum.Length; i++)
        {
            var freq = deltaFreq * i;
            if (Max < _spectrum[i]) Max = _spectrum[i];
            if (freq < upper)
            {
                Spectrums[num] += _spectrum[i];
                if (MaxInEachUnit[num] < _spectrum[i]) MaxInEachUnit[num] = _spectrum[i];
            }
            else
            {
                upper += _unit;
                if (upper > HIGH_FREQ) return;
                num++;
                Spectrums[num] = _spectrum[i];
                MaxInEachUnit[num] = _spectrum[i];
            }
        }
    }
}