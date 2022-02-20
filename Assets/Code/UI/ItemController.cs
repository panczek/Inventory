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
        //private ItemData CurrentItem;

        [Inject] private InventoryController inventoryCtrl;

        private GridTile itemOriginTIle;
        
        public ItemData CurrentItem { get; private set; }

        private void Update()
        {
            if( CurrentItem )
            {
                temporaryItem.transform.position = Input.mousePosition;
                if( Input.GetMouseButtonDown( 1 ) )
                    RemoveTemporaryItem();
            }
        }

        public bool SetTemporaryItem( ItemData newItem, GridTile tile )
        {
            if( CurrentItem )
                return false;
            
            SetTemporaryItem( newItem );
            itemOriginTIle = tile;

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

        public void RemoveTemporaryItem()
        {
            CurrentItem = null;
            temporaryItem.gameObject.SetActive( false );
            if( itemOriginTIle )
            {
                itemOriginTIle.SetImageState( GridTile.EImageState.Normal );
                itemOriginTIle = null;
            }
        }
        
    }
}
