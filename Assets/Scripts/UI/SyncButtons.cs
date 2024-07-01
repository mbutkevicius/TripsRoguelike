using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SyncMouseKeyboardSelection : MonoBehaviour, IPointerEnterHandler
{
    // This method is called when the mouse pointer enters the UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Set this UI element as the currently selected game object
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
