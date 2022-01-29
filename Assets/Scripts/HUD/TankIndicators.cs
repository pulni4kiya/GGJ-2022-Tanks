using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankIndicators : MonoBehaviour {
    public TankController tankController;
    public RectTransform healthIndicatorForeground;

    void Update() {
        transform.rotation = Quaternion.identity;

        healthIndicatorForeground.anchorMax = new Vector2(Mathf.Clamp(tankController.health, 0, 1), 1f);
    }
}
