using System.Collections.Generic;
using Game._Scripts.Runtime.Enums;
using Game._Scripts.Runtime.Managers;
using Game._Scripts.Runtime.Scriptables;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts.Runtime.Battle
{
    public class BattleSystem : MonoBehaviour
    {
 
        [FormerlySerializedAs("unitBasePrefab")] [SerializeField] private BattleUnit battleUnitBasePrefab;
        
        private BattleStateMachine _battleStateMachine;
        
        private List<BattleUnit> PlayerUnits { get; set; }
        
        private List<BattleUnit> EnemyUnits { get; set; }
        
        private List<BattleUnit> _allUnits;
        
        public BattleStateMachine BattleStateMachine => _battleStateMachine;
        
        public static BattleSystem Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        private void Start()
        {
            InitializeBattle();
        }
        
        private void Update()
        {
            if(_battleStateMachine == null) return;
            _battleStateMachine.Update();
        }

        /// <summary>
        /// Initializes the battle by performing the following steps:
        /// 1. Creates all units.
        /// 2. Sets the units to their spawn locations.
        /// 3. Sorts the units by their speed.
        /// 4. Creates the state machine for the battle.
        /// 5. Sets the initial state of the state machine to "Start".
        /// </summary>
        private void InitializeBattle()
        {
            CreateAllUnits();

            SetUnitToSpawnLocation();

            SortUnitsBySpeed();

            CreateStateMachine();

            _battleStateMachine.SetState(BattleState.Start);
        }
        
        private void CreateAllUnits()
        {
            PlayerUnits = new List<BattleUnit>();
            PlayerUnitManager.Instance.GetPlayerTeam().ForEach(x => PlayerUnits.Add(CreateBattleUnit(x, false)));
            EnemyUnits = GetEnemyUnitsForMission("Mission_A1");
        }

        /// <summary>
        /// Gets the enemy units for a given mission.
        /// </summary>
        /// <param name="missionName">The name of the mission.</param>
        /// <returns>A list of enemy BattleUnit objects.</returns>
        private List<BattleUnit> GetEnemyUnitsForMission(string missionName)
        {
            var missionDatabase = Resources.Load<MissionDatabaseSO>("MissionDatabase/MainMissionDatabase");
                var mission = missionDatabase.GetMissionByName(missionName);

                var enemies = new List<BattleUnit>();

            foreach (var unitData in mission.missionUnitDatas)
            {
                var unit = Unit.Unit.CreateUnit(unitData);
                unit.UnitData = unitData;

                var enemyUnit = CreateBattleUnit(unit, true);
                enemies.Add(enemyUnit);
            }

            return enemies;
        }

        /// <summary>
        /// Creates a new BattleUnit from a given Unit and a flag indicating if it is an AI unit.
        /// </summary>
        /// <param name="unit">The Unit object used to create the BattleUnit.</param>
        /// <param name="isAIUnit">A boolean flag indicating if the BattleUnit is controlled by AI.</param>
        /// <returns>A new BattleUnit object with the specified Unit and AI flag.</returns>
        private BattleUnit CreateBattleUnit(Unit.Unit unit, bool isAIUnit)
        {
            var battleUnit = Instantiate(battleUnitBasePrefab);
            if (isAIUnit) unit.InitializeCurrentStats();
            battleUnit.Initialize(unit, isAIUnit);
            battleUnit.name = unit.UnitData.unitName;
            return battleUnit;
        }

        private void SetUnitToSpawnLocation()
        {
            var playerSpawns = GameObject.Find("PlayerSpawns");
            var enemySpawns = GameObject.Find("EnemySpawns");

            for (var i = 0; i < PlayerUnits.Count; i++)
            {
                PlayerUnits[i].transform.SetParent(playerSpawns.transform.GetChild(i));
                PlayerUnits[i].transform.localPosition = Vector3.zero;
                PlayerUnits[i].transform.localRotation = Quaternion.identity;
            }

            for (var i = 0; i < EnemyUnits.Count; i++)
            {
                EnemyUnits[i].transform.SetParent(enemySpawns.transform.GetChild(i));
                EnemyUnits[i].transform.localPosition = Vector3.zero;
                EnemyUnits[i].transform.localRotation = Quaternion.identity;
            }
        }

        private void SortUnitsBySpeed()
        {
            _allUnits = new List<BattleUnit>(PlayerUnits);
            _allUnits.AddRange(EnemyUnits);

            _allUnits.Sort((a, b) =>
                b.Model.CurrentBattleStats[StatType.Speed]
                    .CompareTo(a.Model.CurrentBattleStats[StatType.Speed]));
        }
        
        private void CreateStateMachine()
        {
            _battleStateMachine = new BattleStateMachine(PlayerUnits, EnemyUnits, _allUnits);
        }
    }
}