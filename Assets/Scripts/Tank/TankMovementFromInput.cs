using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovementFromInput : MonoBehaviour
{
    [SerializeField]
    private PlayerActionEvents myInput;

    [SerializeField]
    private TankMovement myMovement;

    private Vector3 movementInput;

    private void Awake() {
        myInput.OnMoveInput += ReceiveInput;
    }

    private void OnEnable() {
        
    }

    private void OnDestroy() {
        myInput.OnMoveInput -= ReceiveInput;
    }

    public void ReceiveInput(Vector2 directionalInput) {
        if (enabled) {
            movementInput.x = directionalInput.x;
            movementInput.z = directionalInput.y;

            myMovement.MoveInDirection(movementInput);
        }
    }
}
