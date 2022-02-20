using System;
using Code.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.UI
{
    public class GridTile : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject overLayer;
        
        private ItemData item;
        private InventoryController owner;
        private ETileState tileState;
        public int2 MyPos { get; private set; }

        public bool IsFree => tileState == ETileState.Free;
        
        public void Init(InventoryController inventoryCtrl, int2 pos)
        {
            owner = inventoryCtrl;
            tileState = ETileState.Free;
            MyPos = pos;
        }
        
        private void OnMouseOver()
        {
            Debug.Log("mouse is over");
        }

        
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            //owner.OnMouseOverTile(this);
            Debug.Log("mouse is over");
        }
        
        public enum ETileState
        {
            Free,
            OccupiedParent,
            OccupiedChild
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("pointer click");
        }
    }
}

