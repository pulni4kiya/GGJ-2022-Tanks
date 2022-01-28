using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFacingFromMovement : MonoBehaviour
{
    [SerializeField]
    private PlayerActionEvents myInput;

    [SerializeField]
    private TankFacing facing;

    private void Awake() {
        myInput.OnMoveInput += ReceiveInput;
    }

    private void OnEnable() {

    }

    private void OnDestroy() {
        myInput.OnMoveInput -= ReceiveInput;
    }

    private void ReceiveInput(Vector2 directionalInput) {
        facing.FaceDirection(directionalInput);
    }
}
