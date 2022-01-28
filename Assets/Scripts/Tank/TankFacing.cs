using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFacing : MonoBehaviour
{
    [SerializeField]
    private Rigidbody myRigidbody;

    [SerializeField]
    private bool cardinalDirectionsOnly = true;

    private Vector3 facingTarget;
    private Quaternion appliedRotation;

    private void FixedUpdate() {
        if (facingTarget != Vector3.zero) {
            RotateBasedOnInput();
        }
    }

    private void OnDisable() {
        facingTarget = Vector3.zero;
    }

    public void FaceDirection(Vector2 targetDirection) {
        if (enabled) {
            facingTarget.x = targetDirection.x;
            facingTarget.z = targetDirection.y;

            if (cardinalDirectionsOnly) {
                facingTarget = ConvertToCardinal(facingTarget);
            }
        }
    }

    public void FacePosition(Vector3 targetPosition) {
        if (enabled) {
            facingTarget.x = targetPosition.x - myRigidbody.position.x;
            facingTarget.z = targetPosition.z - myRigidbody.position.z;
        }
    }

    private void RotateBasedOnInput() {
        appliedRotation = Quaternion.LookRotation(facingTarget, Vector3.up);

        myRigidbody.MoveRotation(appliedRotation);
    }

    private Vector3 ConvertToCardinal(Vector3 directionalInput) {
        if (Mathf.Abs(directionalInput.x) >= Mathf.Abs(directionalInput.z)) {
            directionalInput.z = 0f;
        } else {
            directionalInput.x = 0f;
        }
        directionalInput.y = 0f;

        return directionalInput;
    }
}
