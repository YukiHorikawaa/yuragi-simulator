using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 用

public class YuragiSimurator : MonoBehaviour
{
    [Header("波形パラメータ")]
    public float baseAmplitude = 1f;
    public float baseFrequency = 1f;
    public float noiseStrength = 0.3f;
    public float noiseLerpSpeed = 0.2f;

    [Header("スライダー")]
    public Slider amplitudeSlider;
    public Slider frequencySlider;
    public Slider noiseStrengthSlider;
    public Slider noiseLerpSpeedSlider;

    [Header("表示テキスト")]
    public TextMeshProUGUI amplitudeText;
    public TextMeshProUGUI frequencyText;
    public TextMeshProUGUI noiseStrengthText;
    public TextMeshProUGUI noiseLerpSpeedText;

    [Header("表示オブジェクト")]
    public Renderer sphereRenderer;

    private PinkNoiseGenerator noise;
    private float time;

    private float targetAmpNoise;
    private float smoothAmpNoise;
    private float targetFreqNoise;
    private float smoothFreqNoise;
    private float baseY;

    void Start()
    {
        noise = new PinkNoiseGenerator();

        // 初期値をスライダーに反映
        amplitudeSlider.value = baseAmplitude;
        frequencySlider.value = baseFrequency;
        noiseStrengthSlider.value = noiseStrength;
        noiseLerpSpeedSlider.value = noiseLerpSpeed;
        baseY = transform.position.y;
    }

    void Update()
    {
        // スライダーから値を取得
        baseAmplitude = amplitudeSlider.value;
        baseFrequency = frequencySlider.value;
        noiseStrength = noiseStrengthSlider.value;
        noiseLerpSpeed = noiseLerpSpeedSlider.value;

        // 値を表示
        amplitudeText.text = $"Amplitude: {baseAmplitude:F2}";
        frequencyText.text = $"Frequency: {baseFrequency:F2}";
        noiseStrengthText.text = $"Noise Strength: {noiseStrength:F2}";
        noiseLerpSpeedText.text = $"Noise Lerp: {noiseLerpSpeed:F2}";

        // --- 呼吸波形の処理 ---
        time += Time.deltaTime;

        if (Random.value < 0.02f)
        {
            targetAmpNoise = (noise.NextValue() * 2f - 1f) * noiseStrength;
            targetFreqNoise = (noise.NextValue() * 2f - 1f) * noiseStrength * 0.5f;
        }

        smoothAmpNoise = Mathf.Lerp(smoothAmpNoise, targetAmpNoise, noiseLerpSpeed * Time.deltaTime);
        smoothFreqNoise = Mathf.Lerp(smoothFreqNoise, targetFreqNoise, noiseLerpSpeed * Time.deltaTime);


        // 周期（呼吸っぽさ）
        float frequency = baseFrequency + smoothFreqNoise;
        float amplitude = baseAmplitude + smoothAmpNoise;
        Debug.Log($"Frequency: {frequency}, Amplitude: {amplitude}");

        float sine = Mathf.Sin(time * frequency);
        float y = baseY + sine * amplitude;

        // 球を上下移動
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;

        // 色に変換（sin波の位相を使う）
        float t = Mathf.InverseLerp(-1f, 1f, sine);
        Color newColor = Color.Lerp(Color.blue, Color.red, t);
        sphereRenderer.material.color = newColor;
    }

    int ToPWMValue(float val, float minInput, float maxInput, int resolution)
    {
        float t = Mathf.InverseLerp(minInput, maxInput, val);
        return Mathf.Clamp(Mathf.RoundToInt(t * resolution), 0, resolution);
    }
}