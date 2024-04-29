using System.Collections.Generic;
using System.Linq;
using Game._Scripts.Runtime.Scriptables;
using Game._Scripts.Runtime.Utilities;
using UnityEngine;

namespace Game._Scripts.Runtime.Systems
{
    /// <summary>
    /// One repository for all scriptable objects. Create your query methods here to keep your business logic clean.
    /// I make this a MonoBehaviour as sometimes I add some debug/development references in the editor.
    /// If you don't feel free to make this a standard class
    /// </summary>
    public class ResourceSystem : StaticInstance<ResourceSystem>
    {
        public List<UnitDataSO> UnitDatas { get; private set; }
        private Dictionary<string, UnitDataSO> _UnitDatasDict;

        protected override void Awake()
        {
            base.Awake();
            AssembleResources();
        }

        private void AssembleResources()
        {
            UnitDatas = Resources.LoadAll<UnitDataSO>("UnitData").ToList();
            _UnitDatasDict = UnitDatas.ToDictionary(r => r.unitName, r => r);
        }

        public UnitDataSO GetUnitData(string t)
        {
            return _UnitDatasDict[t];
        }
    }
}