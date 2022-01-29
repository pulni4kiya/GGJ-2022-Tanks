using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FoodPickup : MonoBehaviourPun, IPickup {
	public float healAmount;
	public int segmentCount;

	public System.Action OnPickupCollected;

	public void ApplyPickup(TankController tank) {
		if (enabled) {
			if (PhotonNetwork.InLobby) {
				tank.photonView.RPC("Heal", RpcTarget.All, new object[] { healAmount });
				this.photonView.RPC("PickupCollected", RpcTarget.All);
			} else {
				tank.Heal(this.healAmount);
				PickupCollected();
			}
		}
	}

	public void ApplyPickup(BulletController bullet) {
		if (enabled) {
			if (PhotonNetwork.InLobby) {
				bullet.photonView.RPC("Extend", RpcTarget.All, new object[] { segmentCount });
				this.photonView.RPC("PickupCollected", RpcTarget.All);
			} else {
				bullet.Extend(this.segmentCount);
				PickupCollected();
			}

		}
	}

	[PunRPC]
	public void Appear() {
		gameObject.SetActive(true);
	}

	[PunRPC]
	public void PickupCollected() {
		Hide();
		OnPickupCollected?.Invoke();
	}

	public void Hide() {
		gameObject.SetActive(false);
	}
}
