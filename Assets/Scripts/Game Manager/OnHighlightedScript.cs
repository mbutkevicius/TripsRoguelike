using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightHandler : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    private Button button;
    private AudioManager AudioManager;
    void Start()
    {
        //Get Audio Manager
        AudioManager = GameObject.FindObjectOfType<AudioManager>();
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            //FindObjectOfType<AudioManager>().Play("UiHighlighted");
            AudioManager.playSoundName("ui_highlighted", gameObject);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (button.interactable)
        {
            FindObjectOfType<AudioManager>().Play("UiHighlighted");
        }
    }
}
