using Code.Gameplay;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class DescriptionController : MonoBehaviour
    {
        [SerializeField] private GameObject descriptionBox;
        [SerializeField] private Vector3 boxOffset;
        [SerializeField] private bool showDuringDrag;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI valueText;

        private bool isActive;

        public bool ShowDuringDrag => showDuringDrag;

        private void Update()
        {
            if( isActive )
            {
                descriptionBox.transform.position = Input.mousePosition + boxOffset;
            }
        }

        public void SetActive( ItemData item )
        {
            isActive = true;
            descriptionBox.SetActive( true );
            nameText.text = item.name;
            descriptionText.text = item.Description;
            valueText.text = item.Value.ToString();
        }

        public void Disable()
        {
            isActive = false;
            descriptionBox.SetActive( false );
        }
    }
}
