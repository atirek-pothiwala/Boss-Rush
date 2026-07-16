using UnityEngine;

[System.Serializable]
public class PlayerAttackConfig
{
    public PlayerState state;
    [SerializeField] public AudioClip hitSound;
    [SerializeField] public AudioClip bloodSound;
    [Range(0.5f, 2f)] public float cooldown = 1f;
    [Range(0.6f, 1f)] public float range = 0.6f;
    [Range(1, 50)] public int damage;
    [Range(3f, 9f)] public float knockbackForce = 3f;
    [Range(1, 100)] public int stamina = 10;
}