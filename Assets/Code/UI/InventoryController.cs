using System;
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.UI
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private GameObject inventory;
        [SerializeField] private GameObject inventoryRoot;
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] private GameObject gridPrefab;
        [SerializeField] private int2 inventorySIze;
        [SerializeField] private float spawnSpacing;
        [SerializeField] private Vector2 gridTileSize;
        [SerializeField] private Vector2 gridImageSize;

        private Dictionary<int2, GridTile> gridTiles;

        [Inject]
        private void Inject( DiContainer container )
        {
            _container = container;
        }

        private DiContainer _container;

        private void Start()
        {
            CreateInventory();
        }

        public void CreateInventory()
        {
            gridTiles = new Dictionary<int2, GridTile>();
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = inventorySIze.x;
            gridLayout.spacing = new Vector2( spawnSpacing, spawnSpacing );
            gridLayout.cellSize = gridTileSize;

            for( int h = 0; h < inventorySIze.y; h++ )
            {
                for( int w = 0; w < inventorySIze.x; w++ )
                {
                    int2 pos = new int2( w, h );
                    var gobNew = Instantiate( gridPrefab, inventoryRoot.transform );
                    gobNew.hideFlags = HideFlags.DontSave;

                    gobNew.TryGetComponent( out GridTile gridTile );
                    gridTile.Init( pos );
                    _container.Inject( gridTile );
                    //gobNew.transform.localPosition = new Vector3(spawnSpacing.x * w, spawnSpacing.y * h, 0f);
                    gridTiles.Add( pos, gridTile );
                }
            }
        }

        public Vector2 GetImagePos( int2 size )
        {
            Vector2 imagePos = Vector2.zero;

            imagePos.x = GetAxisPos( gridTileSize.x, size.x );
            imagePos.y = -GetAxisPos( gridTileSize.y, size.y );

            float GetAxisPos( float gridTileS, int imgSize )
            {
                if( imgSize == 1 )
                    return 0f;

                var spacing = ( gridTileS + spawnSpacing ) / 2f;

                return spacing * ( imgSize - 1 );
            }

            return imagePos;
        }

        public Vector2 GetImageSize( int2 size )
        {
            Vector2 imageSize = Vector2.zero;

            imageSize.x = GetDimension( gridImageSize.x, gridTileSize.x, size.x );
            imageSize.y = GetDimension( gridImageSize.y, gridTileSize.y, size.y );

            float GetDimension( float gridImageS, float gridTileS, int imgSize )
            {
                if( imgSize == 1 )
                    return gridImageS;

                var spacingDif = ( gridTileS - gridImageS ) / 2f + spawnSpacing;
                return gridImageS * imgSize + spacingDif * ( imgSize - 1 );
            }

            return imageSize;
        }

        public void PutItemOnGrid( GridTile tile, ItemData itemData )
        {
            if( !WillFit( tile.MyPos, itemData.Size, out var possiblePositions ) )
                return;

            List<GridTile> childTiles = new List<GridTile>();

            foreach( var position in possiblePositions )
            {
                var childTile = gridTiles[position];
                childTile.SetStateOccupiedChild( tile );
                childTiles.Add( childTile );
            }

            tile.SetStateOccupiedParent( itemData, childTiles );
        }

        public bool FindFirstValidPosition( int2 size, out GridTile validTile )
        {
            if( size.x == 1 && size.y == 1 )
            {
                validTile = gridTiles.First( g => g.Value.IsFree ).Value;
                return validTile != null;
            }

            foreach( var tile in gridTiles.Where( g => g.Value.IsFree ) )
            {
                if( WillFit( tile.Key, size, out _ ) )
                {
                    validTile = tile.Value;
                    return true;
                }
            }

            validTile = null;
            return false;

            bool IsFreeBelow( int2 pos )
            {
                var newPos = pos + new int2( 0, 1 );
                return IsValidTile( newPos ) && gridTiles[pos].IsFree;
            }

            bool IsFreeOnRight( int2 pos )
            {
                var newPos = pos + new int2( 1, 0 );
                return IsValidTile( newPos ) && gridTiles[pos].IsFree;
            }
        }

        public List<GridTile> GetTilesFromPos( List<int2> positions )
        {
            var tileList = new List<GridTile>();

            foreach( int2 pos in positions.Where( IsValidTile ) )
            {
                tileList.Add( gridTiles[pos] );
            }

            return tileList;
        }

        public bool WillFit( int2 pos, int2 size, out List<int2> possiblePositions, bool doChecks = true )
        {
            var wontFit = false;
            var wontFitNoChecks = false;
            var posToCheck = pos;
            possiblePositions = new List<int2>();
            int x = 0, y = 0;
            do
            {
                do
                {
                    posToCheck = pos + new int2( x, y );
                    if( IsValidTile( posToCheck ) && gridTiles[posToCheck].IsFree )
                    {
                        x++;
                        possiblePositions.Add( posToCheck );
                        continue;
                    }

                    if( !doChecks )
                    {
                        x++;
                        possiblePositions.Add( posToCheck );
                        wontFitNoChecks = true;
                        continue;
                    }
                    wontFit = true;
                    break;
                } while( x < size.x && !wontFit );

                y++;
                x = 0;
            } while( y < size.y && !wontFit );
            
            return doChecks ? !wontFit : !wontFitNoChecks;
        }

        private bool IsValidTile( int2 pos )
        {
            return pos.x <= inventorySIze.x - 1 && pos.y <= inventorySIze.y - 1;
        }

        public void ToggleInventory()
        {
            inventory.SetActive( !inventory.activeInHierarchy );
        }

        public void ShowInventory( bool show )
        {
            inventory.SetActive( show );
        }
    }
}
