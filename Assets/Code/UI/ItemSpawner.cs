using Code.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Code.UI
{
    public class ItemSpawner : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private ItemData item;
        [SerializeField] private Image icon;
        [SerializeField] private float timeForDoubleClick = 0.5f;

        [Inject] private ItemController itemCtrl;
        [Inject] private InventoryController inventoryCtrl;

        private float doubleClickTimer;

        private void Start()
        {
            icon.sprite = item.Icon;
        }

        private void Update()
        {
            if( doubleClickTimer > 0f )
                doubleClickTimer -= Time.deltaTime;
        }

        public void OnPointerDown( PointerEventData eventData )
        {
            itemCtrl.SetTemporaryItem( item );

            if( doubleClickTimer > 0f )
            {
                if( inventoryCtrl.FindFirstValidPosition( item.Size, out var gridTile ) )
                {
                    itemCtrl.RemoveTemporaryItem();
                    inventoryCtrl.PutItemOnGrid( gridTile, item, inventoryCtrl.GridTileIsFree );
                }
            }

            doubleClickTimer = timeForDoubleClick;
        }
    }
}
