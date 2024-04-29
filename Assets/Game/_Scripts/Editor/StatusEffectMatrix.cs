using System;
using System.Linq;
using Game._Scripts.Runtime.Attributes;
using Game._Scripts.Runtime.Scriptables;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Game._Scripts.Editor
{
    public class StatusEffectMatrix : OdinMenuEditorWindow
    {
        [SerializeField]
        private MatrixData matrixData = new(); 

        [MenuItem("Custom Tools/Status Effect Matrix")]
        private static void OpenEditor()
        {
            GetWindow<StatusEffectMatrix>();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true)
            {
                {"Matrix", matrixData }
            };
            return tree;
        }
    }

    [HideLabel]
    [Serializable]
    [ShowOdinSerializedPropertiesInInspector]
    public class MatrixData
    {
        [TableMatrix(HorizontalTitle = "Status Effect Matrix", Labels = "GetLabels")]
        public bool[,] StatusEffectMatrix = new bool[5,5];


        private (string, LabelDirection) GetLabels(bool[,] array, TableAxis axis, int index)
        {
            string[] XAxisLabels = new string[]
                {
                    "test",
                    "test1",
                    "test2"
                };

            string[] YAxisLabels = new string[]
                {
                    "Acc Up",
                    "Advantage",
                    "Defense Up"
                };


            switch (axis)
            {
                case TableAxis.Y: return (YAxisLabels.Length > index ? YAxisLabels[index].ToString() : string.Empty, LabelDirection.LeftToRight);
                case TableAxis.X: return (XAxisLabels.Length>index ? XAxisLabels[index].ToString() : string.Empty, LabelDirection.LeftToRight); ;
                default: return ("ERROR", LabelDirection.LeftToRight);
            }
        }
    }
}
#endif