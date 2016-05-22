using UnityEngine;
using System.Collections;

public enum PlayerNumbers {
	Player1,
	Player2
}

public class PlayerStats : MonoBehaviour {

	int playerHealth = 100;
	[SerializeField] PlayerNumbers playerNumber;

	public void TakeDamage(int damage)
	{
		playerHealth -= damage;
		if (playerNumber.Equals(PlayerNumbers.Player1))
		{
			GUI.Instance.SetP1HealthText(playerHealth);
		}
		else if (playerNumber.Equals(PlayerNumbers.Player2))
		{
			GUI.Instance.SetP2HealthText(playerHealth);
		}

	}
}
