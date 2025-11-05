using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("How strong the shake movement is.")]
    public float intensity = 0.5f;

    [Tooltip("How long the shake lasts (seconds).")]
    public float duration = 1f;

    [Tooltip("How quickly the shake fades out.")]
    public float fadeSpeed = 2.0f;

    [Tooltip("How fast the noise moves (higher = rougher).")]
    public float frequency = 25f;

    [Tooltip("Magnitude of the rotation in degrees.")]
    public float shakeMagnitude = 20f;

    [Tooltip("Use local position instead of world position.")]
    public bool useLocalPosition = true;

    private Vector3 originalPos;
    private float shakeTimer = 0f;
    private float seedX;
    private float seedY;
    private float seedZ;
    private bool isShaking = false;
    private Quaternion cameraRotation;

    void Start()
    {
        // Save original position
        originalPos = useLocalPosition ? transform.localPosition : transform.position;
        cameraRotation = Quaternion.identity;

        // Generate random seeds for noise variation
        seedX = Random.value * 100f;
        seedY = Random.value * 100f;
        seedZ = Random.value * 100f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartShake();
        }
        if (Input.GetMouseButtonDown(1))
        {
            StartShaking();
        }

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            float normalizedTime = 1f - (shakeTimer / duration);
            float fade = Mathf.Lerp(1f, 0f, normalizedTime * fadeSpeed);

            // Smooth, noise-based offset
            float noiseX = (Mathf.PerlinNoise(seedX, Time.time * frequency) - 0.5f) * 2f;
            float noiseY = (Mathf.PerlinNoise(seedY, Time.time * frequency) - 0.5f) * 2f;
            float noiseZ = (Mathf.PerlinNoise(seedZ, Time.time * frequency) - 0.5f) * 2f;

            Vector3 offset = new Vector3(noiseX, noiseY, noiseZ) * intensity * fade;

            if (useLocalPosition)
                transform.localPosition = originalPos + offset;
            else
                transform.position = originalPos + offset;

            if (shakeTimer <= 0)
            {
                ResetPosition();
            }
        }
    }

    public void StartShake()
    {
        shakeTimer = duration;
    }

    private void ResetPosition()
    {
        if (useLocalPosition)
            transform.localPosition = originalPos;
        else
            transform.position = originalPos;
    }

    public void StartShaking()
    {
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float yRotation = Mathf.PerlinNoise(-shakeMagnitude, shakeMagnitude);

            cameraRotation = Quaternion.Euler(new Vector3(0f, 10f, 0f));

            yield return null;
        }

        cameraRotation = Quaternion.identity;
        isShaking = false;
    }
}

//Get camera to rotate properly
//Smooth animation curve with rotation shake
//Have multiple shakes not cancel each other out