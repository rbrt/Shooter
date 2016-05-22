using UnityEngine;
using System.Collections;
using System.Linq;

public class Weapon : MonoBehaviour {

	[SerializeField] protected Light muzzleFlash;
	[SerializeField] protected ParticleSystem gunshotParticle;
	[SerializeField] protected float damage,
						   			 reloadTime;

	[SerializeField] protected Animator animator;

	[SerializeField] bool hitscan;
	[SerializeField] GameObject hitscanTransform;

	[SerializeField] protected PlayerController player;

	bool shooting = false;

	public void Fire()
	{
		if (!shooting)
		{
			shooting = true;
			this.StartCoroutine(Shoot());
		}
	}

	IEnumerator Shoot()
	{
		hitscanTransform.transform.localScale = new Vector3(200,40,.1f);

		animator.Play("Fire");
		gunshotParticle.Play();
		this.StartCoroutine(FlashMuzzleFlare());

		var allPlayers = PlayerController.GetAllPlayers().Where(x => x != player).ToList();
		for (int i = 0; i < allPlayers.Count; i++)
		{
			if (hitscanTransform.GetComponent<BoxCollider>().bounds.Contains(allPlayers[i].transform.position))
			{
				Debug.Log("HIT " + allPlayers[i]);
			}
		}

		hitscanTransform.transform.localScale = new Vector3(.1f,40,.1f);

		yield return new WaitForSeconds(1.3f);

		shooting = false;
	}

	IEnumerator FlashMuzzleFlare()
	{
		muzzleFlash.enabled = true;
		yield return new WaitForSeconds(.1f);
		muzzleFlash.enabled = false;
	}


}
