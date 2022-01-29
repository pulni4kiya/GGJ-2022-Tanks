using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPickup : MonoBehaviour, IPickup {
	public float healAmount;
	public int segmentCount;

	public System.Action OnPickupCollected;

	public void ApplyPickup(TankController tank) {
		if (enabled) {
			tank.Heal(this.healAmount);
			Hide();
			//GameObject.Destroy(this.gameObject);
			OnPickupCollected?.Invoke();
		}
	}

	public void ApplyPickup(BulletController bullet) {
		if (enabled) {
			bullet.Extend(this.segmentCount);
			Hide();
			//GameObject.Destroy(this.gameObject);
			OnPickupCollected?.Invoke();
		}
	}

	public void Appear() {
		gameObject.SetActive(true);
	}

	public void Hide() {
		gameObject.SetActive(false);
	}
}
