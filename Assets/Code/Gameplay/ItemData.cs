using UnityEngine;

namespace Code.Gameplay
{
    [CreateAssetMenu( fileName = "New Inventory Item", menuName = "Scriptable/Inventory Item" )]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private int2 size;

        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private float value;
        [SerializeField] private float weight;

        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite icon;

        public int2 Size => size;
        public Sprite Icon => icon;
        public string Name => name;
        public string Description => description;
        public float Value => value;
    }
}
