using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FoodPickup : MonoBehaviour, IPickup {
	public float healAmount;
	public int segmentCount;

	public void ApplyPickup(TankController tank) {
		if (PhotonNetwork.InLobby) {
			tank.photonView.RPC("Heal", RpcTarget.All, new object[] { healAmount });
		} else {
			tank.Heal(this.healAmount);
		}

		GameManager.Instance.DestroyObject(this.gameObject);
	}

	public void ApplyPickup(BulletController bullet) {
		if (PhotonNetwork.InLobby) {
			bullet.photonView.RPC("Extend", RpcTarget.All, new object[] { segmentCount });
		} else {
			bullet.Extend(this.segmentCount);
		}

		GameManager.Instance.DestroyObject(this.gameObject);
	}
}
