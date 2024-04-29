using System.Collections;
using System.Collections.Generic;
using Assets.Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Scriptables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Runtime.Battle
{
    public class BattleUnitModel : MonoBehaviour
    {
        public const float MAXTURNPROGRESS = 1000f;
        public readonly int MaxBarrierPercent = 10;

        [ShowInInspector] public BattleUnit BattleUnit { get; set; }

        [ShowInInspector] public Unit.Unit Unit { get; set; }

        // Public Battle Variables
        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public float TurnProgress { get; set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public bool IsTakingTurn { get; set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public bool IsControlledByAI { get; set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public bool IsDead => CurrentHealth <= 0;

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public int CurrentHealth { get; set; }

        [ShowIf("@this.CurrentBattleStats != null")]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public int MaxHealth => (int)CurrentBattleStats[StatType.Health];

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public int CurrentBarrier { get; set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public int MaxBarrier { get; set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public Dictionary<StatType, int> CurrentBattleStats { get; set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public Dictionary<StatType, int> BattleBonusStats { get; set; }

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public List<StatusEffect> StatusEffects { get; set; } = new();

        [ShowInInspector]
        [ReadOnly]
        [FoldoutGroup("Battle Vars")]
        public Dictionary<AbilitySO, int> AbilityCooldowns { get; set; } = new();

        public void InitializeModel(Unit.Unit unit, BattleUnit battleUnit, bool isAIUnit)
        {
            IsControlledByAI = isAIUnit;
            Unit = unit;
            BattleUnit = battleUnit;

            CurrentBattleStats = new Dictionary<StatType, int>(unit.currentUnitStats);
            //MaxHealth = (int)CurrentBattleStats[GeneralStat.Health];
            MaxBarrier = MaxHealth * MaxBarrierPercent;
            CurrentHealth = MaxHealth;
            BattleBonusStats = new Dictionary<StatType, int>();
        }
    }
}