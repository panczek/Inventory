using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.UI
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private GameObject inventory;
        [SerializeField] private GameObject gridPrefab;
        [SerializeField] private GameObject horizontalPrefab;
        [SerializeField] private int2 inventorySIze;
        [SerializeField] private Vector2 spawnSpacing;

        [SerializeField] private Dictionary<int2, GridTile> gridTiles;
        
        private void Start()
        {
            CreateInventory();
        }
        
        public void OnMouseOverTile(GridTile tile)
        {
            
        }

        [Button]
        public void TryLoop(int a, int b)
        {
            for (int i = a; i < b; i++)
            {
                Debug.Log(i);
            }
        }
        
        public void CreateInventory()
        {
            
            
            /*Vector2 spawnPos = Vector2.zero;
            
            for (int w = 0; w < inventorySIze.x; w++)
            {
                for (int h = 0; h < inventorySIze.y; h++)
                {
                    var gobNew = Instantiate( gridPrefab, gridSpawnRoot, false );
                    gobNew.hideFlags = HideFlags.DontSave;

                    gobNew.transform.localPosition = new Vector3(spawnSpacing.x * w, spawnSpacing.y * h, 0f);
                }
            }*/
        }

        private bool FindFirstValidPosition(int2 size, out GridTile validTile)
        {
            if (size.x == 1 && size.y == 1)
            {
                validTile = gridTiles.First(g => g.Value.IsFree).Value;
                return validTile != null;
            }
            foreach (var tile in gridTiles.Where( g => g.Value.IsFree))
            {
                if (WillFit(tile.Key, size))
                {
                    validTile = tile.Value;
                    return true;
                }
            }

            validTile = null;
            return false;

            bool IsFreeBelow(int2 pos)
            {
                var newPos = pos + new int2(0, 1);
                return IsValidTile(newPos) && gridTiles[pos].IsFree;
            }

            bool IsFreeOnRight(int2 pos)
            {
                var newPos = pos + new int2(1, 0);
                return IsValidTile(newPos) && gridTiles[pos].IsFree;
            }
        }

        private bool WillFit(int2 pos, int2 size)
        {
            var wontFit = false;
            var posToCheck = pos;
            int x = 0, y = 0;
            do
            {
                do
                {
                    posToCheck = pos + new int2(x, y);
                    if (IsValidTile(posToCheck) && gridTiles[posToCheck].IsFree)
                    {
                        x++;
                        continue;
                    }
                    wontFit = true;
                    break;
                } while (x < size.x - 1 && !wontFit );

                y++;
                x = 0;
            } while (y < size.y - 1 && !wontFit);

            return !wontFit;
        }
        
        private bool IsValidTile(int2 pos)
        {
            return pos.x <= inventorySIze.x - 1 && pos.y <= inventorySIze.y - 1;
        }
        
        public void ToggleInventory()
        {
            inventory.SetActive(!inventory.activeInHierarchy);
        }
        
        public void ShowInventory(bool show)
        {
            inventory.SetActive(show);
        }
    }
}

