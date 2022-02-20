using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.Gameplay
{
    [CreateAssetMenu(fileName = "New Inventory Item", menuName = "Scriptable/Inventory Item")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private int2 size;

        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private float value;
        [SerializeField] private float weight;

        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite icon;
    }
}

