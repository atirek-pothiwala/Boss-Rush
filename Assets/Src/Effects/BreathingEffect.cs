using UnityEngine;

public class BreathingEffect : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float scaleAmount = 0.1f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = originalScale * scale;
    }
}