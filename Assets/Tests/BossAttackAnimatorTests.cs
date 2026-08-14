using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class BossAttackAnimatorTests
{
    static readonly string[] BossPrefabPaths =
    {
        "Assets/Prefabs/Characters/Minotaur.prefab",
        "Assets/Prefabs/Characters/Warewolf.prefab",
        "Assets/Prefabs/Characters/Gorgon.prefab",
    };

    [Test]
    public void BossAttacks_HaveAnimatorTransitionsForEveryConfiguredState()
    {
        foreach (var path in BossPrefabPaths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Assert.IsNotNull(prefab, $"Missing prefab at {path}");

            var boss = prefab.GetComponent<BossController>();
            Assert.IsNotNull(boss, $"{prefab.name} is missing BossController");

            var animator = prefab.GetComponent<Animator>();
            Assert.IsNotNull(animator, $"{prefab.name} is missing Animator");

            var controller = animator.runtimeAnimatorController as AnimatorController;
            Assert.IsNotNull(controller, $"{prefab.name} is missing AnimatorController");

            var attackStates = GetConfiguredAttackStates(boss).ToList();
            var coveredStates = GetAnimatorAttackStateIds(controller);

            foreach (var state in attackStates)
            {
                Assert.IsTrue(
                    coveredStates.Contains((int)state),
                    $"{prefab.name} attack {state} has no animator transition with OnAction"
                );
            }
        }
    }

    [Test]
    public void BossAttackAnimations_FirePostAttackRoutine()
    {
        foreach (var path in BossPrefabPaths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var boss = prefab.GetComponent<BossController>();
            var animator = prefab.GetComponent<Animator>();
            var controller = animator.runtimeAnimatorController as AnimatorController;
            var attackStates = GetConfiguredAttackStates(boss).ToHashSet();

            foreach (var clip in GetAttackClipsForStates(controller, attackStates))
            {
                Assert.IsTrue(
                    clip.events.Any(animationEvent => animationEvent.functionName == "PostAttackRoutine"),
                    $"{prefab.name} clip {clip.name} is missing PostAttackRoutine animation event"
                );
            }
        }
    }

    static IEnumerable<BossState> GetConfiguredAttackStates(BossController boss)
    {
        var serializedBoss = new SerializedObject(boss);
        var attacks = serializedBoss.FindProperty("attacks");

        for (int i = 0; i < attacks.arraySize; i++)
        {
            var stateValue = attacks.GetArrayElementAtIndex(i).FindPropertyRelative("state").intValue;
            yield return (BossState)stateValue;
        }
    }

    static HashSet<int> GetAnimatorAttackStateIds(AnimatorController controller)
    {
        var coveredStates = new HashSet<int>();

        foreach (var layer in controller.layers)
        {
            CollectAttackStates(layer.stateMachine.anyStateTransitions, coveredStates);
            CollectAttackStatesFromMachine(layer.stateMachine, coveredStates);
        }

        return coveredStates;
    }

    static void CollectAttackStatesFromMachine(AnimatorStateMachine stateMachine, HashSet<int> coveredStates)
    {
        foreach (var childState in stateMachine.states)
            CollectAttackStates(childState.state.transitions, coveredStates);

        foreach (var childMachine in stateMachine.stateMachines)
            CollectAttackStatesFromMachine(childMachine.stateMachine, coveredStates);
    }

    static void CollectAttackStates(AnimatorStateTransition[] transitions, HashSet<int> coveredStates)
    {
        foreach (var transition in transitions)
        {
            int? stateValue = null;
            var requiresAction = false;

            foreach (var condition in transition.conditions)
            {
                if (condition.mode == AnimatorConditionMode.Equals && condition.parameter == "State")
                    stateValue = (int)condition.threshold;

                if (condition.mode == AnimatorConditionMode.If && condition.parameter == "OnAction")
                    requiresAction = true;
            }

            if (stateValue.HasValue && requiresAction)
                coveredStates.Add(stateValue.Value);
        }
    }

    static IEnumerable<AnimationClip> GetAttackClipsForStates(
        AnimatorController controller,
        HashSet<BossState> attackStates)
    {
        var clips = new HashSet<AnimationClip>();
        var attackStateIds = attackStates.Select(state => (int)state).ToHashSet();

        foreach (var layer in controller.layers)
        {
            foreach (var transition in layer.stateMachine.anyStateTransitions)
            {
                int? stateValue = null;
                var hasOnAction = false;

                foreach (var condition in transition.conditions)
                {
                    if (condition.mode == AnimatorConditionMode.Equals && condition.parameter == "State")
                        stateValue = (int)condition.threshold;

                    if (condition.mode == AnimatorConditionMode.If && condition.parameter == "OnAction")
                        hasOnAction = true;
                }

                if (!stateValue.HasValue || !hasOnAction || !attackStateIds.Contains(stateValue.Value))
                    continue;

                if (transition.destinationState?.motion is AnimationClip clip)
                    clips.Add(clip);
            }
        }

        return clips;
    }
}
