using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    public void OnEnable(){
        // ensure controls isn't null
        controls?.Enable();
    }

    public void OnDisable(){
        // ensure controls isn't null
        // was getting an error on restart because controls became null
        controls?.Disable(); 
    }

    // Method to clear user input
    public void ClearInput(){
        moveInput = Vector2.zero;
    }
}
