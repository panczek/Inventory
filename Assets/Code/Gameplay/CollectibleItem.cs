using UnityEngine;

namespace Code.Gameplay
{
    public class CollectibleItem : MonoBehaviour
    {
        [SerializeField] private ItemData item;

        public ItemData Item => item;

        public void OnItemCollected()
        {
            Destroy( this.gameObject );
        }
    }
}
