using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CircularNavigation : MonoBehaviour
{
    private Selectable[] selectables;

    void Start()
    {
        // Find all the Selectable UI elements in the current Canvas
        selectables = GetComponentsInChildren<Selectable>();
        
        // Configure navigation for each Selectable
        for (int i = 0; i < selectables.Length; i++)
        {
            var nav = selectables[i].navigation;
            nav.mode = Navigation.Mode.Explicit;

            // Set navigation up and down
            nav.selectOnDown = selectables[(i == 0) ? selectables.Length - 1 : i - 1];
            nav.selectOnUp = selectables[(i == selectables.Length - 1) ? 0 : i + 1];

            // Apply the modified navigation settings
            selectables[i].navigation = nav;
        }
    }
}
