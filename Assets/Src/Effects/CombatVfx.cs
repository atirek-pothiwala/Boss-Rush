using UnityEngine;

public static class CombatVfx
{
    private static Material particleMaterial;

    public static void PlayPlayerAttack(Vector3 position, PlayerState attackState, bool facingRight)
    {
        switch (attackState)
        {
            case PlayerState.QuickAttack:
                SpawnSlashBurst(position, facingRight, new Color(1f, 0.95f, 0.6f), 10, 0.12f, 2.2f, 0.35f);
                break;
            case PlayerState.HeavyAttack:
                SpawnSlashBurst(position, facingRight, new Color(1f, 0.55f, 0.2f), 18, 0.18f, 3f, 0.45f);
                SpawnRingBurst(position, new Color(1f, 0.35f, 0.1f, 0.5f), 12, 0.25f, 2.5f);
                break;
            case PlayerState.SpecialAttack:
                SpawnSlashBurst(position, facingRight, new Color(0.55f, 0.75f, 1f), 24, 0.22f, 3.6f, 0.55f);
                SpawnRingBurst(position, new Color(0.45f, 0.55f, 1f, 0.6f), 16, 0.3f, 3.2f);
                break;
        }
    }

    public static void PlayBossAttack(Vector3 position, BossState attackState, bool facingRight)
    {
        switch (attackState)
        {
            case BossState.QuickAttack:
            case BossState.RunAttack:
                SpawnSlashBurst(position, facingRight, new Color(1f, 0.35f, 0.35f), 12, 0.14f, 2.4f, 0.35f);
                break;
            case BossState.HeavyAttack:
            case BossState.JumpAttack:
                SpawnSlashBurst(position, facingRight, new Color(0.9f, 0.15f, 0.15f), 20, 0.2f, 3.2f, 0.5f);
                SpawnRingBurst(position, new Color(1f, 0.2f, 0.1f, 0.55f), 14, 0.28f, 2.8f);
                break;
            case BossState.SpecialAttack:
            case BossState.Scream:
                SpawnSlashBurst(position, facingRight, new Color(0.75f, 0.2f, 0.9f), 28, 0.24f, 3.8f, 0.6f);
                SpawnRingBurst(position, new Color(0.65f, 0.2f, 0.85f, 0.65f), 20, 0.35f, 3.5f);
                break;
            default:
                SpawnSlashBurst(position, facingRight, new Color(1f, 0.5f, 0.3f), 10, 0.12f, 2f, 0.3f);
                break;
        }
    }

    public static void PlayBloodHit(Vector3 position, Vector2 direction)
    {
        SpawnDirectionalBurst(position, direction, new Color(0.85f, 0.05f, 0.05f), 14, 0.1f, 2.5f, 0.4f);
        SpawnRingBurst(position, new Color(0.7f, 0f, 0f, 0.45f), 8, 0.16f, 1.8f);
    }

    public static void PlayMissSwipe(Vector3 position, bool facingRight)
    {
        SpawnSlashBurst(position, facingRight, new Color(0.8f, 0.8f, 0.85f, 0.35f), 6, 0.08f, 1.5f, 0.2f);
    }

    private static void SpawnSlashBurst(
        Vector3 position,
        bool facingRight,
        Color color,
        int count,
        float size,
        float speed,
        float lifetime)
    {
        var direction = facingRight ? Vector3.right : Vector3.left;
        SpawnDirectionalBurst(position, direction, color, count, size, speed, lifetime);
    }

    private static void SpawnDirectionalBurst(
        Vector3 position,
        Vector2 direction,
        Color color,
        int count,
        float size,
        float speed,
        float lifetime)
    {
        var effectObject = CreateParticleObject(position);
        var particleSystem = effectObject.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.startColor = color;
        main.startSize = size;
        main.startSpeed = speed;
        main.startLifetime = lifetime;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = count;
        main.loop = false;
        main.gravityModifier = 0.4f;

        var emission = particleSystem.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        var shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 28f;
        shape.radius = 0.05f;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        effectObject.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        var velocity = particleSystem.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x = direction.x * 0.5f;
        velocity.y = direction.y * 0.5f;

        particleSystem.Play();
        Object.Destroy(effectObject, lifetime + 0.6f);
    }

    private static void SpawnRingBurst(Vector3 position, Color color, int count, float size, float speed)
    {
        var effectObject = CreateParticleObject(position);
        var particleSystem = effectObject.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.startColor = color;
        main.startSize = size;
        main.startSpeed = speed;
        main.startLifetime = 0.35f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = count;
        main.loop = false;

        var emission = particleSystem.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)count) });

        var shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.08f;

        particleSystem.Play();
        Object.Destroy(effectObject, 0.9f);
    }

    private static GameObject CreateParticleObject(Vector3 position)
    {
        var effectObject = new GameObject("CombatVfx");
        effectObject.transform.position = position;
        var particleSystem = effectObject.AddComponent<ParticleSystem>();
        effectObject.AddComponent<ParticleSystemRenderer>();

        var renderer = effectObject.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetParticleMaterial();
        renderer.sortingOrder = 25;

        var main = particleSystem.main;
        main.playOnAwake = false;

        return effectObject;
    }

    private static Material GetParticleMaterial()
    {
        if (particleMaterial != null) return particleMaterial;

        var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Universal Render Pipeline/Particles/Unlit");
        particleMaterial = shader != null ? new Material(shader) : new Material(Shader.Find("UI/Default"));
        return particleMaterial;
    }
}
