using System.Collections.Generic;
using Code.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Code.UI
{
    public class GridTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject overLayer;
        [SerializeField] private TextMeshProUGUI debugTExt;
        [SerializeField] private Image itemImage;
        [SerializeField] private Image background;
        [SerializeField] private RectTransform itemRect;

        [Inject] private InventoryController InventoryCtrl;
        [Inject] private ItemController itemCtrl;
        [Inject] private DescriptionController descriptionCtrl;

        private bool showDebugs;
        private GridTile parent;
        private List<GridTile> children;
        private List<GridTile> overlapChilds;
        private ItemData item;
        private bool canReplace;
        private GridTile replaceTarget;
        private ETileState tileState = ETileState.Free;
        private ETileState previousTileState = ETileState.Free;
        private EImageState imageState = EImageState.Normal;
        public int2 MyPos { get; private set; }

        public bool ShowDebugs
        {
            private get => showDebugs;

            set
            {
                showDebugs = value;
                debugTExt.gameObject.SetActive( value );
            }
        }

        public bool IsFree => tileState == ETileState.Free;
        public bool IsInSelection => tileState == ETileState.CurrentItemIsSelected;

        private void Update()
        {
            if( overlapChilds.Count > 0 && !itemCtrl.CurrentItem )
            {
                foreach( var child in overlapChilds )
                {
                    child.SetImageState( EImageState.Normal );
                }
            }

            if( ShowDebugs )
            {
                debugTExt.text = MyPos + tileState.ToString();
                if( InventoryCtrl.GridTIleIsInCurrentlySelected( this ) )
                    debugTExt.text += " Inventory Set";
            }
        }

        public void Init( int2 pos )
        {
            tileState = ETileState.Free;
            MyPos = pos;
            itemImage.gameObject.SetActive( false );
            overlapChilds = new List<GridTile>();
            children = new List<GridTile>();
        }

        public void SetStateFree()
        {
            itemImage.gameObject.SetActive( false );
            item = null;
            parent = null;
            previousTileState = tileState;
            tileState = ETileState.Free;

            foreach( var child in children )
            {
                if( child != this )
                    child.SetStateFree();
            }
        }

        public void SetStateOccupiedParent( ItemData newItem, List<GridTile> newChildren )
        {
            children = newChildren;
            itemImage.gameObject.SetActive( true );
            item = newItem;
            parent = null;

            itemImage.sprite = item.Icon;
            itemRect.sizeDelta = InventoryCtrl.GetImageSize( item.Size );
            itemRect.localPosition = InventoryCtrl.GetImagePos( item.Size );

            previousTileState = tileState;
            tileState = ETileState.OccupiedParent;
        }

        public void SetStateOccupiedChild( GridTile newParent )
        {
            parent = newParent;
            itemImage.gameObject.SetActive( false );

            previousTileState = tileState;
            tileState = ETileState.OccupiedChild;
        }

        public void SetItemSelectionMode( bool state )
        {
            var newColor = state ? Color.grey : Color.white;
            newColor.a = itemImage.color.a;
            itemImage.color = newColor;

            foreach( var child in children )
            {
                if( child != this )
                    child.SetItemSelectionMode( state );
            }

            var tileStateCopy = tileState;
            tileState = state ? ETileState.CurrentItemIsSelected : previousTileState;
            previousTileState = tileStateCopy;
        }

        public void SetImageState( EImageState newState )
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
                case EImageState.Replace:
                    newColor = Color.blue;
                    newColor.a = background.color.a;
                    background.color = newColor;
                    break;
            }
        }

        #region PointerActions

        public void OnPointerEnter( PointerEventData eventData )
        {
            switch( tileState )
            {
                case ETileState.Free:
                    if( itemCtrl.CurrentItem != null )
                        FitCheck();

                    break;
                case ETileState.OccupiedParent:
                    InventoryCtrl.SetCurrentlySelectedItem( GetImageTiles() );
                    if( itemCtrl.CurrentItem != null )
                    {
                        if( !FitCheck() )
                            ReplaceCheck( this );

                        if( descriptionCtrl.ShowDuringDrag )
                            descriptionCtrl.SetActive( item );
                    }
                    else
                    {
                        descriptionCtrl.SetActive( item );
                    }


                    break;
                case ETileState.OccupiedChild:
                    InventoryCtrl.SetCurrentlySelectedItem( parent.GetImageTiles() );
                    if( itemCtrl.CurrentItem != null )
                    {
                        if( !FitCheck() )
                            ReplaceCheck( parent );
                    }

                    descriptionCtrl.SetActive( parent.item );
                    break;
            }

            bool FitCheck()
            {
                bool willFIt = InventoryCtrl.WillFit( MyPos, itemCtrl.CurrentItem.Size, out var possiblePos, InventoryCtrl.GridTileIsFree, false );
                overlapChilds = InventoryCtrl.GetTilesFromPos( possiblePos );
                foreach( var child in overlapChilds )
                    child.SetImageState( willFIt ? EImageState.WillFIt : EImageState.WontFit );

                return willFIt;
            }

            void ReplaceCheck( GridTile checkTile )
            {
                if( !itemCtrl.ItemOriginTile )
                    return;

                bool otherFitsHere = InventoryCtrl.WillFit( MyPos, itemCtrl.CurrentItem.Size, out _, InventoryCtrl.GridTileDuringSelection );
                bool thisFitsThere = InventoryCtrl.WillFit( itemCtrl.ItemOriginTile.MyPos, checkTile.item.Size, out _, InventoryCtrl.GridTileDuringSelection );


                if( otherFitsHere && thisFitsThere )
                {
                    foreach( var child in overlapChilds )
                        child.SetImageState( EImageState.Replace );

                    canReplace = true;
                    replaceTarget = itemCtrl.ItemOriginTile;
                }
            }
        }

        public void OnPointerExit( PointerEventData eventData )
        {
            descriptionCtrl.Disable();
            canReplace = false;
            replaceTarget = null;

            foreach( var child in overlapChilds )
                child.SetImageState( EImageState.Normal );

            overlapChilds.Clear();
            InventoryCtrl.SetCurrentlySelectedItem( new List<GridTile>() );
        }

        public void OnPointerClick( PointerEventData eventData )
        {
            if( eventData.button != PointerEventData.InputButton.Left )
                return;

            switch( tileState )
            {
                case ETileState.Free:
                    if( itemCtrl.CurrentItem != null )
                    {
                        if( InventoryCtrl.PutItemOnGrid( this, itemCtrl.CurrentItem) )
                            itemCtrl.RemoveTemporaryItem();
                    }

                    break;
                case ETileState.OccupiedParent:
                    if( canReplace && itemCtrl.ItemOriginTile )
                    {
                        var thisItemCopy = item;
                        var replaceItemCopy = replaceTarget.item;

                        SetStateFree();
                        replaceTarget.SetStateFree();
                        InventoryCtrl.PutItemOnGrid( this, replaceItemCopy, InventoryCtrl.GridTileDuringSelection );
                        InventoryCtrl.PutItemOnGrid( replaceTarget, thisItemCopy, InventoryCtrl.GridTileDuringSelection );
                        replaceTarget.SetItemSelectionMode( true );

                        itemCtrl.RemoveTemporaryItem( false );
                    }
                    else if( itemCtrl.SetTemporaryItem( item, this ) )
                        SetItemSelectionMode( true );

                    break;
                case ETileState.OccupiedChild:
                    if( canReplace && itemCtrl.ItemOriginTile )
                    {
                        var parentCopy = parent;
                        var parentItemCopy = parent.item;
                        var replaceItemCopy = replaceTarget.item;

                        parent.SetStateFree();
                        replaceTarget.SetStateFree();
                        InventoryCtrl.PutItemOnGrid( parentCopy, replaceItemCopy, InventoryCtrl.GridTileDuringSelection );
                        InventoryCtrl.PutItemOnGrid( replaceTarget, parentItemCopy, InventoryCtrl.GridTileDuringSelection );
                        replaceTarget.SetItemSelectionMode( true );

                        itemCtrl.RemoveTemporaryItem( false );
                    }
                    else if( itemCtrl.SetTemporaryItem( parent.item, parent ) )
                        parent.SetItemSelectionMode( true );

                    break;
            }
        }

        #endregion

        private List<GridTile> GetImageTiles()
        {
            var selectedCopy = children;
            selectedCopy.Add( this );
            return selectedCopy;
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
            Replace
        }
    }
}
