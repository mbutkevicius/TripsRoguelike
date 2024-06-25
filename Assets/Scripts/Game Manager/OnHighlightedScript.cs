using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightHandler : MonoBehaviour, IPointerEnterHandler
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            FindObjectOfType<AudioManager>().Play("UiHighlighted");
        }
    }
}
