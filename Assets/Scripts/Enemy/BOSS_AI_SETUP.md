# Boss AI System Setup Guide

## Overview

The Boss AI system consists of three main components:

- **BossController**: Main AI logic with state management and decision-making
- **BossHealthSystem**: Health management, damage, invincibility frames
- **BossAttackSystem**: Attack patterns and cooldown management

## Setup Steps

### 1. Add Components to Boss GameObject

1. Select your Boss GameObject in the hierarchy
2. Add these components (if not already present):
   - `BossController` (script)
   - `BossHealthSystem` (script)
   - `BossAttackSystem` (script)
   - `Animator` (Unity component)
   - `Rigidbody2D` (Unity component)

### 2. Configure BossController

In the Inspector, set:

- **Character**: Drag your character sprite/visual GameObject here
- **Walk Speed**: ~1.5 (slower movement)
- **Run Speed**: ~5 (faster approach speed)
- **Jump Height**: ~0.5 (for jump attacks)
- **Gravity**: -10 (standard gravity)
- **Transition Speed**: 0.2 (animation blend speed)

#### AI Decision Settings

- **Decision Update Interval**: 0.5 (how often AI makes decisions - lower = more responsive)
- **Combat Distance**: 2-3 (how close to player before engaging)
- **Retreat Distance**: 3-4 (distance to maintain after attacking)
- **Attack Range Multiplier**: 1.2 (extends reach slightly)
- **Post Attack Retreat Time**: 0.3 (seconds to back away after attack)
- **Phase Threshold Percent**: 50 (at 50% health, boss becomes more aggressive)

### 3. Configure BossHealthSystem

In the Inspector, set:

- **Max Health**: 500 (adjust based on game difficulty)
- **Invincibility Duration**: 0.3 (seconds of invincibility after taking damage)

### 4. Configure BossAttackSystem - CRITICAL

Set up your attack patterns. For each attack type:

**Attack 1: QuickAttack** (Close Range)

- Attack State: QuickAttack
- Cooldown: 1.5 seconds
- Range: 0.6 units
- Damage: 10 points
- Knockback Force: 3 units
- Preferred Distance: 0.7 (close melee range)

**Attack 2: PowerAttack** (Close Range)

- Attack State: PowerAttack
- Cooldown: 3 seconds
- Range: 0.7 units
- Damage: 25 points
- Knockback Force: 6 units
- Preferred Distance: 0.8

**Attack 3: JumpAttack** (Extended Range)

- Attack State: JumpAttack
- Cooldown: 4 seconds

- Range: 1.5 units
- Damage: 20 points
- Knockback Force: 7 units
- Preferred Distance: 1.5

**Attack 4: SpecialAttack** (Close Range)

- Attack State: SpecialAttack
- Cooldown: 5 seconds
- Range: 0.8 units
- Damage: 40 points
- Knockback Force: 10 units
- Preferred Distance: 0.9

### 5. Animation Setup

Make sure your Animator has these parameters:

- **State** (Integer): Controls which animation plays
  - 0 = Idle
  - 1 = Walk
  - 2 = Run
  - 3 = RunAttack
  - 4 = JumpAttack
  - 5 = QuickAttack
  - 6 = PowerAttack
  - 7 = SpecialAttack
  - 8 = Hurt
  - 9 = Dead

- **Move** (Float): Animation blend for walking/running (0-1)

### 6. Animation Events - IMPORTANT

For damage to work, add animation events to your attack animations:

1. Open each attack animation in the Animation window
2. At the point where the attack connects, add an event
3. Call the function: `OnAttackHit()` on the BossController

This triggers damage when the animation reaches that point.

### 7. Player Integration

The boss will deal damage to the player via:

- First, it looks for a `BossHealthSystem` component on the player
- If not found, it looks for an `IDamageable` interface implementation
- If neither exists, no damage is dealt

To receive damage, your player needs one of:

```csharp
// Option 1: Use BossHealthSystem (same component)
BossHealthSystem playerHealth = player.GetComponent<BossHealthSystem>();
playerHealth.TakeDamage(damage, knockback, knockbackForce);

// Option 2: Implement IDamageable interface
public class PlayerController : MonoBehaviour, IDamageable
{
    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        // Your damage logic here
    }
}
```

### 8. Tags

Make sure:

- Boss GameObject has tag "Boss" or the player's finding code works
- Player GameObject has tag "Player"

## Testing Tips

1. **Visual Debugging**: Select the boss in Scene view to see:
   - Yellow circle = combat distance
   - Red circle = retreat distance
   - Green line = direction to player

2. **Adjust Difficulty**:
   - Lower `Decision Update Interval` = faster decisions = harder
   - Increase attack damage = harder
   - Decrease attack cooldowns = harder

3. **Phase Transitions**: Boss becomes 20% faster and more aggressive at 50% health

## Common Adjustments

**Make boss easier:**

- Increase attack cooldowns
- Decrease damage values
- Increase decision update interval

**Make boss harder:**

- Decrease attack cooldowns
- Increase damage values
- Decrease decision update interval
- Lower phase threshold (activates phase 2 sooner)

**Fine-tune attack patterns:**

- Adjust `Preferred Distance` to change how close boss gets before attacking
- Modify `Range` to make attacks more/less reach-heavy
- Adjust `Knockback Force` for hit feedback
