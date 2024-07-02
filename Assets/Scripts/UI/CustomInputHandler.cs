using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomInputHandler : MonoBehaviour
{
    void Update()
    {
        // Check if the Spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Get the currently selected GameObject
            GameObject selected = EventSystem.current.currentSelectedGameObject;

            // Check if the selected GameObject is a button and trigger its click event
            if (selected != null)
            {
                Button button = selected.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                }
            }
        }
    }
}
