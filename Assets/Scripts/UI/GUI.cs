using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUI : MonoBehaviour
{

	static GUI instance;

	[SerializeField] protected Text p1HealthText;
	[SerializeField] protected Text p2HealthText;

	public static GUI Instance
	{
		get
		{
			return instance;
		}
	}

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void SetP1HealthText(int health)
	{
		if (health < 0)
		{
			health = 0;
		}
		p1HealthText.text = "" + health;
	}

	public void SetP2HealthText(int health)
	{
		if (health < 0)
		{
			health = 0;
		}
		p2HealthText.text = "" + health;
	}
}
