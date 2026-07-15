using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Transform hero;
    private Transform boss;
    private Camera MainCamera => Camera.main;

    public float followSpeed = 5f;
    public float zoomSpeed = 5f;

    public float minZoom = 5f;
    public float maxZoom = 9f;

    public float leftLimit = -20;
    public float rightLimit = 20;
    public float bottomLimit = -2;
    public float topLimit = 8;

    public static CameraManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(Transform hero, Transform boss)
    {
        this.hero = hero;
        this.boss = boss;
    }

    void LateUpdate()
    {
        if (hero == null || boss == null) return;

        Vector3 center = (hero.position + boss.position) * 0.5f;

        float horizontal = Mathf.Abs(hero.position.x - boss.position.x);
        float vertical = Mathf.Abs(hero.position.y - boss.position.y);

        float widthSize = horizontal / MainCamera.aspect * 0.6f;
        float heightSize = vertical * 0.7f;

        float targetZoom = Mathf.Clamp(
            Mathf.Max(widthSize, heightSize),
            minZoom,
            maxZoom
        );

        MainCamera.orthographicSize = Mathf.Lerp(
            MainCamera.orthographicSize,
            targetZoom,
            zoomSpeed * Time.deltaTime
        );

        float halfHeight = MainCamera.orthographicSize;
        float halfWidth = halfHeight * MainCamera.aspect;

        Vector3 targetPosition = new(
            Mathf.Clamp(center.x, leftLimit + halfWidth, rightLimit - halfWidth),
            Mathf.Clamp(center.y, bottomLimit + halfHeight, topLimit - halfHeight),
            MainCamera.transform.position.z
        );

        MainCamera.transform.position = Vector3.Lerp(
            MainCamera.transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}
