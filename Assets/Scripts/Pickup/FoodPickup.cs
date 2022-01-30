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
			tank.NetHeal(this.healAmount);
			NetPickupCollected();
		}
	}

	public void ApplyPickup(BulletController bullet) {
		if (enabled) {
			bullet.NetExtend(this.segmentCount);
			NetPickupCollected();
		}
	}

	public void NetAppear() {
		if (PhotonNetwork.InLobby) {
			photonView.RPC("Appear", RpcTarget.All);
		} else {
			Appear();
		}
	}

	public void NetPickupCollected() {
		if (PhotonNetwork.InLobby) {
			photonView.RPC("PickupCollected", RpcTarget.All);
		} else {
			PickupCollected();
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

	private void Hide() {
		gameObject.SetActive(false);
	}
}
