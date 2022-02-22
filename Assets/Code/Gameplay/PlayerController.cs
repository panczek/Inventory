using Code.UI;
using UnityEngine;
using Zenject;

namespace Code.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float sprintSpeedMultiplier;

        [Inject] private InventoryController inventoryCtrl;

        private void Update()
        {
            if( Input.GetKeyDown( KeyCode.I ) )
                inventoryCtrl.ToggleInventory();

            var tempSpeed = speed;
            if( Input.GetKey( KeyCode.LeftShift ) )
                tempSpeed *= sprintSpeedMultiplier;
            
            var moveVec = new Vector3( Input.GetAxis( "Horizontal" ), 0, Input.GetAxis( "Vertical" ) );

            if( moveVec.magnitude > 0f )
                transform.position += moveVec * tempSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter( Collider other )
        {
            if( other.gameObject.TryGetComponent( out CollectibleItem item ) )
            {
                if( inventoryCtrl.FindFirstValidPosition( item.Item.Size, out var gridTile ) )
                {
                    inventoryCtrl.PutItemOnGrid( gridTile, item.Item );
                    item.OnItemCollected();
                }
            }
        }
    }
}
