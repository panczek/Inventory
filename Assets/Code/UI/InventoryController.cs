using System;
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay;
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
        private List<GridTile> currentlySelectedTiles;
        private DiContainer _container;
        private bool showDebugs;

        public bool GridTileIsFree( GridTile tile ) => tile.IsFree;
        public bool GridTileDuringSelection( GridTile tile ) => tile.IsInSelection || currentlySelectedTiles.Contains( tile ) || tile.IsFree;
        public bool GridTIleIsInCurrentlySelected( GridTile tile ) => currentlySelectedTiles.Contains( tile );
        
        [Inject]
        private void Inject( DiContainer container )
        {
            _container = container;
        }
        
        private void Start()
        {
            CreateInventory();
            foreach( var tile in gridTiles.Values )
                tile.ShowDebugs = false;
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

        public bool PutItemOnGrid( GridTile tile, ItemData itemData, Func<GridTile, bool> tileCheck  )
        {
            if( !WillFit( tile.MyPos, itemData.Size, out var possiblePositions, tileCheck ) )
                return false;

            List<GridTile> childTiles = new List<GridTile>();

            foreach( var position in possiblePositions.Where( p => p != tile.MyPos ) )
            {
                var childTile = gridTiles[position];
                childTile.SetStateOccupiedChild( tile );
                childTiles.Add( childTile );
            }
            
            tile.SetStateOccupiedParent( itemData, childTiles );
            return true;
        }

        public void SetCurrentlySelectedItem( List<GridTile> newSelectedTiles )
        {
            currentlySelectedTiles = newSelectedTiles;
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
                if( WillFit( tile.Key, size, out _, GridTileIsFree ) )
                {
                    validTile = tile.Value;
                    return true;
                }
            }

            validTile = null;
            return false;
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
        
        public bool WillFit( int2 pos, int2 size, out List<int2> possiblePositions, Func<GridTile, bool> tileCheck, bool doChecks = true )
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
                    if( IsValidTile( posToCheck ) && tileCheck(gridTiles[posToCheck]) )
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

        public void ToggleDebugs()
        {
            showDebugs = !showDebugs;

            foreach( var tile in gridTiles.Values )
                tile.ShowDebugs = showDebugs;
        }
        
        private void CreateInventory()
        {
            gridTiles = new Dictionary<int2, GridTile>();
            currentlySelectedTiles = new List<GridTile>();
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
                    gridTiles.Add( pos, gridTile );
                }
            }
        }
        
        private bool IsValidTile( int2 pos )
        {
            return pos.x <= inventorySIze.x - 1 && pos.y <= inventorySIze.y - 1;
        }
    }
}
