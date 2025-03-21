using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class UserInputs : MonoBehaviour
{
    public static UserInputs inputREF;

    public InputActions actions;

    InputAction moveAction, lookAction, jumpAction, interactAction, attackAction;


    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool jumpInput { get; private set; }
    public bool interactInput { get; private set; }

    public bool attackInput { get; private set; }



    public bool nextTabInput;
    public bool prevTabInput;


    private void Awake()
    {
        if (InputManager.inputActions == null)
        {
            InputManager.inputActions = new InputActions(); // Ensure it's initialized
        }

        if (inputREF == null)
        {
            inputREF = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        actions = InputManager.inputActions;

        if (actions == null)
        {
            Debug.LogError("InputActions not initialized in InputManager");
            return;
        }

        SetUpInputActions();
    }

    private void Update()
    {
        UpdateInputs();
    }

    void SetUpInputActions()
    {
        actions.Player.Enable();
        actions.UI.Enable();

        moveAction = actions.Player.Move;
        lookAction = actions.Player.Look;
        jumpAction = actions.Player.Jump;
        interactAction = actions.Player.Interact;
        attackAction = actions.Player.Attack;
    }

    void UpdateInputs()
    {

        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
        interactInput = interactAction.WasPressedThisFrame();

        jumpInput = jumpAction.WasPressedThisFrame();
        attackInput = attackAction.WasPressedThisFrame();
    }
}
