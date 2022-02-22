using UnityEngine;

namespace Code.Gameplay
{
    [CreateAssetMenu( fileName = "New Weapon", menuName = "Scriptable/Weapon Item" )]
    public class WeaponData : ItemData
    {
        [SerializeField] private float damage;
    }
}
