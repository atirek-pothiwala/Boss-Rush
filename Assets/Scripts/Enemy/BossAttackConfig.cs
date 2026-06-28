using UnityEngine;

[System.Serializable]
public class BossAttackConfig
{
    public BossState state;
    [Range(0.5f, 1f)] public float cooldown = 1f;
    [Range(0.6f, 1f)] public float range = 0.6f;
    [Range(1, 50)] public int damage;
    [Range(3f, 5f)] public float knockbackForce = 3f;
    [Range(1, 100)] public int stamina = 10;
    public bool isAerial = false;
    public bool isRun = false;
}