using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightHandler : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        FindObjectOfType<AudioManager>().Play("UiHighlighted");
    }
}
