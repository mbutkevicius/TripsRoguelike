using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android.LowLevel;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;
    [HideInInspector] public Controls controls;
    // pressing left returns -1 and pressing right returns 1
    [HideInInspector] public Vector2 moveInput;

    public void Awake() {
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        controls = new Controls();

        // send whatever button pressed into moveInput
        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnEnable(){
        controls.Enable();
    }

    private void OnDisable(){
        controls.Disable();
    }
}
