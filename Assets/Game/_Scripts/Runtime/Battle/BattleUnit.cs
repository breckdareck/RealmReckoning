using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game._Scripts.Runtime.Battle;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Managers;
using Game._Scripts.Runtime.Scriptables;
using Game._Scripts.Runtime.UI.Unit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Runtime.Battle
{
    [Serializable]
    public class BattleUnit : MonoBehaviour
    {
        private const string BuffImmunity = "Buff Immunity";

        [field: SerializeField] public UI_BattleUnit UIBattleUnit { get; private set; }
        [field: SerializeField] public BattleUnitModel Model { get; private set; }


        private void Awake()
        {
            if (UIBattleUnit == null)
                UIBattleUnit = GetComponent<UI_BattleUnit>();
            if(Model == null)
                Model = GetComponent<BattleUnitModel>();
        }

        public void Initialize(Unit.Unit unit, bool isAIUnit)
        {
            Model.InitializeModel(unit, this, isAIUnit);
            UIBattleUnit.InitializeUI(this);
        }

        public void UpdateTurnProgress(float deltaTime)
        {
            if (Model.IsTakingTurn) return;

            Model.TurnProgress += Model.CurrentBattleStats[StatType.Speed] * deltaTime;

            UIBattleUnit.UpdateTurnSliderValue(Model.TurnProgress);

            if (!(Model.TurnProgress >= BattleUnitModel.MAXTURNPROGRESS)) return;
            //Debug.Log($"{name}'s turn!");
            StartTurn();
        }

        public void StartTurn()
        {
            Model.IsTakingTurn = true;
        }

        public void EndTurn()
        {
            Model.IsTakingTurn = false;
            Model.TurnProgress %= BattleUnitModel.MAXTURNPROGRESS;
            TickDownStatusEffects();
            TickCooldowns();
        }

        private void SetUnitDead()
        {
            Debug.Log($"{name} - Is Dead");
            Model.TurnProgress = 0;
            Model.StatusEffects.Clear();
            gameObject.SetActive(false);
        }

        public void ApplyDamage(int damageAmount, bool isAttackDodged)
        {
            if (isAttackDodged)
            {
                UIBattleUnit.CreateDamageText("Dodged");
                return;
            }

            if (damageAmount == 0)
            {
                UIBattleUnit.CreateDamageText("Damage Negated");
                return;
            }

            // TODO : If Unit has Status Effect - Unassailable, all damage is null. Only calculate Debuffs
            // TODO : Change Color of damage text depending on Damage Type
            // TODO : Add Barrier and Pierce Barrier to formula
            // TODO : Calculate Barrier reduction formula to incoming damage

            if (Model.CurrentBarrier > 0)
            {
                var damageAfterBarrier = Math.Max(0, damageAmount - Model.CurrentBarrier);
                Model.CurrentBarrier = Math.Max(0, Model.CurrentBarrier - damageAmount);
                damageAmount = damageAfterBarrier;
            }

            Model.CurrentHealth = Math.Max(0, Model.CurrentHealth - damageAmount);

            UIBattleUnit.CreateDamageText(damageAmount.ToString());
            UIBattleUnit.UpdateHealthUI();
            UIBattleUnit.UpdateBarrierUI();
            Debug.Log($"{name} - Damage Taken: {damageAmount}");
            if (!Model.IsDead) return;
            SetUnitDead();
        }
        
        public void ApplyHeal(int healAmount, int barrierAmount)
        {
            Model.CurrentHealth = Mathf.Clamp(Model.CurrentHealth + healAmount, 0, Model.MaxHealth);
            Model.CurrentBarrier = Mathf.Clamp(Model.CurrentBarrier + barrierAmount, 0, Model.MaxBarrier);
            UIBattleUnit.UpdateHealthUI();
            UIBattleUnit.UpdateBarrierUI();
            UIBattleUnit.CreateHealText(healAmount);
            Debug.Log($"{name} - Heal Received: {healAmount}");
        }

        [Button]
        public void ApplyStatusEffect(StatusEffectSO statusEffectSO, int turnsEffected)
        {
            // Check For Status Effect (Buff Immunity) - Don't allow buffs to be added
            if (Model.StatusEffects.Exists(x => x.StatusEffectSO.StatusEffectName == BuffImmunity) &&
                statusEffectSO.StatusEffectType == StatusEffectType.Buff) return;

            var effectCheck = Model.StatusEffects.FirstOrDefault(effect => effect.StatusEffectSO.StatusEffectName == statusEffectSO.StatusEffectName);
            if (effectCheck != default)
            {
                // Check For Stacking, Max Stacks
                if (statusEffectSO.CanStack)
                {
                    effectCheck.IncreaseStackCount();
                    effectCheck.ResetTurnsEffected();
                }
                // If can't Stack, but exists and Duplicatable
                else if (!statusEffectSO.CanStack &&
                         statusEffectSO.Duplicatable)
                {
                    var newStatusEffect = new StatusEffect(statusEffectSO, turnsEffected);
                    Model.StatusEffects.Add(newStatusEffect);
                    UIBattleUnit.CreateStatusEffectIcon(newStatusEffect);
                }
                // If can't Stack but exists ResetTurns
                else
                {
                    effectCheck.ResetTurnsEffected();
                    effectCheck.SetAppliedThisTurn(true);
                }
            }
            else
            {
                var newStatusEffect = new StatusEffect(statusEffectSO, turnsEffected);
                Model.StatusEffects.Add(newStatusEffect);
                UIBattleUnit.CreateStatusEffectIcon(newStatusEffect);
            }

            RemoveCurrentBonusStats();
            CalculateBattleBonusStats();
            RecalculateCurrentStats();
        }

        public void DispellAllStatusEffects()
        {
            DispellStatusEffects(e => e.Dispellable);
        }

        public void DispellSpecificStatusEffects(List<StatusEffectSO> effects)
        {
            DispellStatusEffects(e => e.Dispellable && effects.Contains(e));
        }

        private void DispellStatusEffects(Func<StatusEffectSO, bool> predicate)
        {
            int effectsRemoved = 0;
            var targetStatusEffects = new List<StatusEffect>(Model.StatusEffects);
            foreach (var statusEffect in targetStatusEffects)
            {
                if (predicate(statusEffect.StatusEffectSO))
                {
                    Model.StatusEffects.Remove(statusEffect);
                    effectsRemoved += 1;
                }
            }

            EventManager.Instance.InvokeOnUnitStatusEffectsDispelledEvent(effectsRemoved);
            RemoveCurrentBonusStats();
            CalculateBattleBonusStats();
            RecalculateCurrentStats();
        }

        private void RemoveCurrentBonusStats()
        {
            foreach (var stat in Model.BattleBonusStats)
            {
                Model.CurrentBattleStats[stat.Key] -= stat.Value;
                if (stat.Key == StatType.Health) Model.CurrentHealth -= (int)stat.Value;
            }

            Model.BattleBonusStats.Clear();
        }

        private void CalculateBattleBonusStats()
        {
            foreach (var (effect, data) in from effect in Model.StatusEffects
                                           from data in effect.StatusEffectSO.StatusEffectDatas
                                           select (effect, data))
            {   
                if (!Model.BattleBonusStats.ContainsKey(data.StatEffected))
                    Model.BattleBonusStats[data.StatEffected] = 0;

                if (effect.StatusEffectSO.StatusEffectCalculationType == StatusEffectCalculationType.Additive)
                    Model.BattleBonusStats[data.StatEffected] = (data.EffectAmountPercent * effect.StackCount) +
                            Model.BattleBonusStats[data.StatEffected];

                else
                    Model.BattleBonusStats[data.StatEffected] =
                    (Model.Unit.currentUnitStats[data.StatEffected] *
                    (data.EffectAmountPercent * effect.StackCount)) +
                    Model.BattleBonusStats[data.StatEffected];
                    
            }

        }

        private void RecalculateCurrentStats()
        {
            foreach (var stat in Model.BattleBonusStats)
            {
                Model.CurrentBattleStats[stat.Key] += stat.Value;
                if (stat.Key == StatType.Health)
                    Model.CurrentHealth = Mathf.Clamp(Model.CurrentHealth + (int)stat.Value, 0, Model.MaxHealth);
            }

            UIBattleUnit.UpdateHealthUI();
            UIBattleUnit.UpdateBarrierUI();
        }

        private void ApplyBonusStats()
        {
            RemoveCurrentBonusStats();
            CalculateBattleBonusStats();
            RecalculateCurrentStats();
        }

        public void TickDownStatusEffects()
        {
            for (int i = Model.StatusEffects.Count - 1; i >= 0; i--)
            {
                var statusEffect = Model.StatusEffects[i];
                if (!statusEffect.AppliedThisTurn)
                {
                    statusEffect.TickDownStatusEffect();
                    if (statusEffect.RemainingTurnsEffected <= 0)
                    {
                        Model.StatusEffects.RemoveAt(i);
                        statusEffect.OnDestroy();
                    }
                }
                else
                {
                    statusEffect.SetAppliedThisTurn(false);
                }
            }

            if (Model.StatusEffects.Count == 0)
            {
                ApplyBonusStats();
            }
        }

        public bool IsAbilityOnCooldown(AbilitySO ability)
        {
            return Model.AbilityCooldowns.TryGetValue(ability, out int cooldown) && cooldown > 0;
        }

        public void StartCooldown(AbilitySO ability)
        {
            int cooldown = ability.CooldownTurns + 1;
            if(Model.AbilityCooldowns.ContainsKey(ability))
                Model.AbilityCooldowns[ability] = cooldown;
            else
                Model.AbilityCooldowns.Add(ability, cooldown);
        }

        public void TickCooldowns()
        {
            var keys = new List<AbilitySO>(Model.AbilityCooldowns.Keys);
            foreach (var ability in keys)
            {
                if (--Model.AbilityCooldowns[ability] == 0)
                    Model.AbilityCooldowns.Remove(ability);
            }
        }
    }
}