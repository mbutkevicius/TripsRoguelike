using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SceneViewFocus
{
    private const string MenuName = "Tools/Focus Scene View On Play";
    private static bool isEnabled;

    static SceneViewFocus()
    {
        // Load the saved state
        isEnabled = EditorPrefs.GetBool(MenuName, false);

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    [MenuItem(MenuName)]
    private static void ToggleAction()
    {
        isEnabled = !isEnabled;
        // Save the new state
        EditorPrefs.SetBool(MenuName, isEnabled);
    }

    [MenuItem(MenuName, true)]
    private static bool ToggleActionValidate()
    {
        // This adds a checkmark next to the menu item if it is enabled
        Menu.SetChecked(MenuName, isEnabled);
        return true;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (isEnabled && state == PlayModeStateChange.EnteredPlayMode)
        {
            FocusSceneView();
        }
    }

    private static void FocusSceneView()
    {
        // Get the current active scene view
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            // Focus the scene view
            sceneView.Focus();
        }
    }
}