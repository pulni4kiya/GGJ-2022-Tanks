using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody myRigidbody;

    [SerializeField]
    private float minVelocity;
    [SerializeField]
    private float maxVelocity;

    [SerializeField]
    private bool cardinalDirectionsOnly = true;

    private Vector3 latestInput;
    private Vector3 intendedVelocity;
    private Vector3 appliedVelocity;

    private void FixedUpdate() {
        if (latestInput != Vector3.zero) {
            AddForceBasedOnInput();
        }
    }

    private void OnDisable() {
        latestInput = Vector3.zero;
    }

    public void MoveInDirection(Vector3 directionalInput) {
        if (enabled) {
            latestInput = directionalInput;

            if (latestInput != Vector3.zero) {

                if (cardinalDirectionsOnly) {
                    latestInput = ConvertToCardinal(latestInput);
                }
            
                intendedVelocity = latestInput.normalized;
                intendedVelocity *= Mathf.Lerp(minVelocity, maxVelocity, latestInput.magnitude);
                intendedVelocity /= 1f - (Time.fixedDeltaTime * myRigidbody.drag);
            }
        }
    }

    private void AddForceBasedOnInput() {
        appliedVelocity = intendedVelocity;
        appliedVelocity -= myRigidbody.velocity;
        appliedVelocity.y = 0f;

        myRigidbody.AddForce(appliedVelocity, ForceMode.VelocityChange);
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
