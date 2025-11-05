using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Positional Shake Settings")]
    [Tooltip("How strong the shake movement is.")]
    public float positionIntensity = 0.5f;

    [Tooltip("How long the position shake lasts (seconds).")]
    public float positionDuration = 0.5f;

    [Tooltip("How fast the position noise moves (higher = rougher).")]
    public float positionFrequency = 25f;

    [Header("Rotational Shake Settings")]
    [Tooltip("How strong the rotational shake is (degrees).")]
    public float rotationIntensity = 2f;

    [Tooltip("How long the rotational shake lasts (seconds).")]
    public float rotationDuration = 0.4f;

    [Tooltip("How fast the rotation noise moves.")]
    public float rotationFrequency = 20f;

    [Header("Common Settings")]
    [Tooltip("How quickly the shake fades out.")]
    public float fadeSpeed = 2.0f;

    [Tooltip("Use local position instead of world position.")]
    public bool useLocalPosition = true;

    private Vector3 originalPos;
    private Quaternion originalRot;

    private float positionTimer;
    private float rotationTimer;

    private float posSeedX, posSeedY, posSeedZ;
    private float rotSeed;

    void Start()
    {
        // Save the camera's original transform
        originalPos = useLocalPosition ? transform.localPosition : transform.position;
        originalRot = transform.localRotation;

        // Random noise seeds
        posSeedX = Random.value * 100f;
        posSeedY = Random.value * 100f;
        posSeedZ = Random.value * 100f;
        rotSeed = Random.value * 100f;
    }

    void Update()
    {
        // Left click = positional shake
        if (Input.GetMouseButtonDown(0))
            StartPositionShake();

        // Right click = rotational shake
        if (Input.GetMouseButtonDown(1))
            StartRotationShake();

        UpdatePositionShake();
        UpdateRotationShake();
    }

    private void UpdatePositionShake()
    {
        if (positionTimer > 0)
        {
            positionTimer -= Time.deltaTime;
            float normalizedTime = 1f - (positionTimer / positionDuration);
            float fade = Mathf.Lerp(1f, 0f, normalizedTime * fadeSpeed);

            float noiseX = (Mathf.PerlinNoise(posSeedX, Time.time * positionFrequency) - 0.5f) * 2f;
            float noiseY = (Mathf.PerlinNoise(posSeedY, Time.time * positionFrequency) - 0.5f) * 2f;
            float noiseZ = (Mathf.PerlinNoise(posSeedZ, Time.time * positionFrequency) - 0.5f) * 2f;

            Vector3 offset = new Vector3(noiseX, noiseY, noiseZ) * positionIntensity * fade;

            if (useLocalPosition)
                transform.localPosition = originalPos + offset;
            else
                transform.position = originalPos + offset;
        }
        else
        {
            // reset when done
            if (useLocalPosition)
                transform.localPosition = originalPos;
            else
                transform.position = originalPos;
        }
    }

    private void UpdateRotationShake()
    {
        if (rotationTimer > 0)
        {
            rotationTimer -= Time.deltaTime;
            float normalizedTime = 1f - (rotationTimer / rotationDuration);
            float fade = Mathf.Lerp(1f, 0f, normalizedTime * fadeSpeed);

            // Smoothly rotate using Perlin noise
            float rotNoise = (Mathf.PerlinNoise(rotSeed, Time.time * rotationFrequency) - 0.5f) * 2f;
            float angleY = rotNoise * rotationIntensity * fade;

            transform.localRotation = originalRot * Quaternion.Euler(0f, angleY, 0f);
        }
        else
        {
            transform.localRotation = originalRot;
        }
    }

    // Public triggers
    public void StartPositionShake()
    {
        positionTimer = positionDuration;
    }

    public void StartRotationShake()
    {
        rotationTimer = rotationDuration;
    }
}

//Get camera to rotate properly
//Smooth animation curve with rotation shake
//Have multiple shakes not cancel each other out