using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Code.UI
{
    public class ItemEraser : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private ItemController itemCtrl;

        public void OnPointerClick( PointerEventData eventData )
        {
            if( itemCtrl.CurrentItem != null )
                itemCtrl.RemoveTemporaryItem();
        }
    }
}
