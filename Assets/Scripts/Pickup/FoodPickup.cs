using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPickup : MonoBehaviour, IPickup {
	public float healAmount;
	public int segmentCount;

	public void ApplyPickup(TankController tank) {
		tank.Heal(this.healAmount);
		GameObject.Destroy(this.gameObject);
	}

	public void ApplyPickup(BulletController bullet) {
		bullet.Extend(this.segmentCount);
		GameObject.Destroy(this.gameObject);
	}
}
