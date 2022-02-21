using System;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.UI
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private Image temporaryItem;
        [SerializeField] private RectTransform rectTransform;

        [Inject] private InventoryController inventoryCtrl;

        public GridTile ItemOriginTile { get; private set; }
        public ItemData CurrentItem { get; private set; }

        private void Update()
        {
            if( CurrentItem )
            {
                temporaryItem.transform.position = Input.mousePosition;
                if( Input.GetMouseButtonDown( 1 ) )
                    RemoveTemporaryItem( false );
            }
        }

        public bool SetTemporaryItem( ItemData newItem, GridTile tile )
        {
            if( CurrentItem )
                return false;

            SetTemporaryItem( newItem );
            ItemOriginTile = tile;

            return true;
        }

        public void SetTemporaryItem( ItemData item )
        {
            if( CurrentItem )
                return;

            CurrentItem = item;
            temporaryItem.sprite = item.Icon;
            rectTransform.sizeDelta = inventoryCtrl.GetImageSize( item.Size );
            temporaryItem.gameObject.SetActive( true );
        }

        public void RemoveTemporaryItem( bool destroyItem = true )
        {
            CurrentItem = null;
            temporaryItem.gameObject.SetActive( false );

            if( !ItemOriginTile )
                return;

            ItemOriginTile.SetImageState( GridTile.EImageState.Normal );
            ItemOriginTile.SetItemSelectionMode( false );
            if( destroyItem )
                ItemOriginTile.SetStateFree();

            ItemOriginTile = null;
        }
    }
}
