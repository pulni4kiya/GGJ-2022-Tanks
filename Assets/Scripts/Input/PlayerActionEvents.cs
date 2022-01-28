using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionEvents : MonoBehaviour
{
    [SerializeField]
    private PlayerInput inputEventSource;

    [SerializeField]
    private string moveActionName;
    private InputAction moveAction;

    [SerializeField]
    private string lookActionName;
    private InputAction lookAction;

    [SerializeField]
    private string fireActionName;
    private InputAction fireAction;

    private Vector2 newMovementInput;
    private Vector2 newLookInput;

    public System.Action<Vector2> OnMoveInput;
    public System.Action<Vector2> OnLookInput;
    public System.Action OnFireInput;

    private void Awake() {
        moveAction = inputEventSource.currentActionMap.FindAction(moveActionName, true);
        lookAction = inputEventSource.currentActionMap.FindAction(lookActionName, true);
        fireAction = inputEventSource.currentActionMap.FindAction(fireActionName, true);

        moveAction.started += OnMove;
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        lookAction.started += OnLook;
        lookAction.performed += OnLook;
        lookAction.canceled += OnLook;

        fireAction.performed += OnFire;
    }

    private void OnEnable() {
        OnMoveInput?.Invoke(newMovementInput);
        OnLookInput?.Invoke(newLookInput);
    }

    private void OnDisable() {
        OnMoveInput?.Invoke(Vector2.zero);
        OnLookInput?.Invoke(Vector2.zero);
    }

    public void OnMove(InputAction.CallbackContext context) {
        newMovementInput = context.ReadValue<Vector2>();
        if (enabled) {
            OnMoveInput?.Invoke(newMovementInput);
        }
    }

    public void OnLook(InputAction.CallbackContext context) {
        newLookInput = context.ReadValue<Vector2>();
        if (enabled) {
            OnLookInput?.Invoke(newLookInput);
        }
    }

    public void OnFire(InputAction.CallbackContext context) {
        OnFireInput?.Invoke();
    }
}
