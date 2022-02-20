using System;
using System.Collections.Generic;
using Code.Gameplay;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Code.UI
{
    public class GridTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
    {
        [SerializeField] private GameObject overLayer;
        [SerializeField] private TextMeshProUGUI debugTExt;
        [SerializeField] private Image itemImage;
        [SerializeField] private Image background;
        [SerializeField] private RectTransform itemRect;

        [Inject] private InventoryController InventoryCtrl;
        [Inject] private ItemController itemCtrl;
        [Inject] private DescriptionController descriptionCtrl;

        private GridTile parent;
        private List<GridTile> childs;
        private List<GridTile> overlapChilds;
        private ItemData item;
        private ETileState tileState = ETileState.Free;
        private EImageState imageState = EImageState.Normal;
        public int2 MyPos { get; private set; }

        public bool IsFree => tileState == ETileState.Free;

        private void Update()
        {
            switch( tileState )
            {
                case ETileState.Free:
                    debugTExt.text = "free";
                    break;
                case ETileState.OccupiedParent:
                    debugTExt.text = "parent";
                    break;
                case ETileState.OccupiedChild:
                    debugTExt.text = "child";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //debugTExt.text = MyPos + tileState.ToString();
        }

        public void Init( int2 pos )
        {
            tileState = ETileState.Free;
            MyPos = pos;
            itemImage.gameObject.SetActive( false );
            overlapChilds = new List<GridTile>();
        }

        public void SetStateFree()
        {
            itemImage.gameObject.SetActive( false );
            item = null;
            parent = null;
            tileState = ETileState.Free;
        }

        public void SetStateOccupiedParent( ItemData newItem, List<GridTile> newChildren )
        {
            childs = newChildren;
            itemImage.gameObject.SetActive( true );
            item = newItem;
            parent = null;

            itemImage.sprite = item.Icon;
            itemRect.sizeDelta = InventoryCtrl.GetImageSize( item.Size );
            itemRect.localPosition = InventoryCtrl.GetImagePos( item.Size );

            tileState = ETileState.OccupiedParent;
        }

        public void SetStateOccupiedChild( GridTile newParent )
        {
            parent = newParent;
            itemImage.gameObject.SetActive( false );

            tileState = ETileState.OccupiedChild;
        }

        public void OnPointerEnter( PointerEventData eventData )
        {
            switch( tileState )
            {
                case ETileState.Free:
                    if( itemCtrl.CurrentItem != null )
                    {
                        bool willFIt = InventoryCtrl.WillFit( MyPos, itemCtrl.CurrentItem.Size, out var possiblePos, false );
                        overlapChilds = InventoryCtrl.GetTilesFromPos( possiblePos );
                        foreach( var child in overlapChilds )
                            child.SetImageState( willFIt ? EImageState.WillFIt : EImageState.WontFit );
                    }
                    break;
                case ETileState.OccupiedParent:
                    descriptionCtrl.SetActive( item );
                    break;
                case ETileState.OccupiedChild:
                    descriptionCtrl.SetActive( parent.item );
                    break;

            }
        }

        public void OnPointerExit( PointerEventData eventData )
        {
            descriptionCtrl.Disable();
            
            foreach( var child in overlapChilds )
                child.SetImageState( EImageState.Normal );
        }
        
        public enum ETileState
        {
            Free,
            OccupiedParent,
            OccupiedChild,
            CurrentItemIsSelected
        }
        
        public enum EImageState
        {
            Normal,
            WillFIt,
            WontFit,
            DuringItemSelection
        }

        public void SetImageState(EImageState newState)
        {
            Color newColor;
            switch( newState )
            {
                case EImageState.Normal:
                    newColor = Color.white;
                    newColor.a = background.color.a;
                    background.color = newColor;
                    break;
                case EImageState.WillFIt:
                    newColor = Color.green;
                    newColor.a = background.color.a;
                    background.color = newColor;
                    break;
                case EImageState.WontFit:
                    newColor = Color.red;
                    newColor.a = background.color.a;
                    background.color = newColor;
                    break;
                case EImageState.DuringItemSelection:
                    newColor = Color.grey;
                    newColor.a = background.color.a;
                    background.color = newColor;
                    break;
            }
        }
        
        public void OnPointerClick( PointerEventData eventData )
        {
            if( eventData.button != PointerEventData.InputButton.Left )
                return;
            
            switch( tileState )
            {
                case ETileState.Free:
                    
                    break;
                case ETileState.OccupiedParent:
                    if(itemCtrl.SetTemporaryItem( item, this )){}
                        SetImageState( EImageState.DuringItemSelection );
                    break;
                case ETileState.OccupiedChild:
                    if(itemCtrl.SetTemporaryItem( parent.item, parent ))
                        parent.SetImageState( EImageState.DuringItemSelection );
                    break;

            }
        }

        public void OnPointerUp( PointerEventData eventData )
        {
            Debug.Log( "pointer up" );
        }

        
    }
}
