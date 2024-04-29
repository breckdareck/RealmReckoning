using System;
using System.Collections.Generic;
using System.Linq;
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
            List<AbilitySO> keys = new List<AbilitySO>(Model.AbilityCooldowns.Keys);
            foreach(AbilitySO ability in keys)
            { 
                Model.AbilityCooldowns[ability]--;  
                if(Model.AbilityCooldowns[ability]==0)
                    Model.AbilityCooldowns.Remove(ability);
            }
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
            
            var resultDamage = ComputeDamage(damageAmount);
            
            UIBattleUnit.CreateDamageText(resultDamage.ToString());
            UIBattleUnit.UpdateHealthUI();
            UIBattleUnit.UpdateBarrierUI();
            Debug.Log($"{name} - Damage Taken: {resultDamage}");
            if (!Model.IsDead) return;
            SetUnitDead();
        }

        private void SetUnitDead()
        {
            Debug.Log($"{name} - Is Dead");
            Model.TurnProgress = 0;
            Model.StatusEffects.Clear();
            gameObject.SetActive(false);
        }

        private int ComputeDamage(int damageAmount)
        {
            var damageRemaining = damageAmount;
            var oldBarrierAmount = ApplyDamageToBarrier(ref damageRemaining);
            ApplyDamageToHealth(ref damageRemaining);
            return damageRemaining + oldBarrierAmount;
        }
        
        private int ApplyDamageToBarrier(ref int remainingDamage)
        {
            var oldBarrierAmount = 0;
            if (Model.CurrentBarrier > 0)
            {
                if (remainingDamage > Model.CurrentBarrier)
                {
                    remainingDamage -= Model.CurrentBarrier;
                    oldBarrierAmount = Model.CurrentBarrier;
                    Model.CurrentBarrier = 0;
                }
                else
                {
                    UIBattleUnit.CreateDamageText(remainingDamage.ToString());
                    Model.CurrentBarrier -= Mathf.Clamp(remainingDamage, 0, Model.MaxBarrier);
                    remainingDamage = 0;
                }
            }
            return oldBarrierAmount;
        }
        
        private void ApplyDamageToHealth(ref int remainingDamage)
        {
            if (remainingDamage > 0) 
            {
                Model.CurrentHealth = Mathf.Clamp(Model.CurrentHealth - remainingDamage, 0, Model.MaxHealth);
            }
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
        public void ApplyStatusEffect(StatusEffectSO statusEffectSo)
        {
            // Check For Status Effect (Buff Immunity) - Don't allow buffs to be added
            if (Model.StatusEffects.Exists(x => x.StatusEffectName == BuffImmunity) &&
                statusEffectSo.StatusEffectType == StatusEffectType.Buff) return;

            var effectCheck = Model.StatusEffects.FirstOrDefault(effect => effect.StatusEffectName == statusEffectSo.StatusEffectName);
            if (effectCheck != default)
            {
                // Check For Stacking, Max Stacks
                if (statusEffectSo.CanStack)
                {
                    effectCheck.IncreaseStackCount();
                    effectCheck.ResetTurnsEffected();
                }
                // If can't Stack, but exists and Duplicatable
                else if (!statusEffectSo.CanStack &&
                         statusEffectSo.Duplicatable)
                {
                    Model.StatusEffects.Add(statusEffectSo);
                    UIBattleUnit.CreateStatusEffectIcon(statusEffectSo);
                }
                // If can't Stack but exists ResetTurns
                else
                {
                    effectCheck.ResetTurnsEffected();
                }
            }
            else
            {
                statusEffectSo.ResetTurnsEffected();
                Model.StatusEffects.Add(statusEffectSo);
                UIBattleUnit.CreateStatusEffectIcon(statusEffectSo);
            }

            RemoveCurrentBonusStats();
            CalculateBattleBonusStats();
            RecalculateCurrentStats();
        }

        public void DispellAllStatusEffects()
        {
            int effectsRemoved = 0;
            
            var targetStatusEffects = new List<StatusEffectSO>(Model.StatusEffects);
            foreach (var statusEffect in targetStatusEffects)
            {
                if (statusEffect.Dispellable)
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

        public void DispellSpecificStatusEffects(List<StatusEffectSO> effects)
        {
            int effectsRemoved = 0;

            var targetStatusEffects = new List<StatusEffectSO>(Model.StatusEffects);
            foreach (var statusEffect in targetStatusEffects)
            {
                if (statusEffect.Dispellable && effects.Contains(statusEffect))
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
            foreach (var effect in Model.StatusEffects)
            foreach (var data in effect.StatusEffectDatas)
                if (effect.StatusEffectCalculationType == StatusEffectCalculationType.Additive)
                    Model.BattleBonusStats[data.StatEffected] =
                        (float)data.EffectAmountPercent / 100 * effect.StackCount;
                else
                        Model.BattleBonusStats[data.StatEffected] =
                        Model.Unit.currentUnitStats[data.StatEffected] * ((float)data.EffectAmountPercent /
                                                                           100 *
                                                                           effect.StackCount);
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

        public void TickDownStatusEffects()
        {
            if (Model.StatusEffects.Count > 0)
            {
                foreach (var statusEffect in Model.StatusEffects)
                {
                    if (statusEffect.AppliedThisTurn)
                    {
                        statusEffect.SetAppliedThisTurn(false);
                        continue;
                    }

                    statusEffect.TickDownStatusEffect();
                }
                var expiredStatusEffects = Model.StatusEffects.Where(statusEffect => statusEffect.RemainingTurnsEffected <= 0).ToList();
                Model.StatusEffects.RemoveAll(x => x.RemainingTurnsEffected <= 0);
                expiredStatusEffects.ForEach(x => x.OnDestroy());
                RemoveCurrentBonusStats();
                CalculateBattleBonusStats();
                RecalculateCurrentStats();
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
    }
}