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
	BoxCollider hitscanCollider;

	void Awake()
	{
		hitscanCollider = hitscanTransform.GetComponent<BoxCollider>();
	}

	public void Fire()
	{
		if (!shooting)
		{
			shooting = true;
			this.StartCoroutine(Shoot());
		}
	}

	bool PointInOABB (Vector3 point, BoxCollider box )
 	{
         point = box.transform.InverseTransformPoint( point ) - box.center;

         float halfX = (box.size.x * 0.5f);
         float halfY = (box.size.y * 0.5f);
         float halfZ = (box.size.z * 0.5f);
         if(point.x < halfX && point.x > -halfX &&
            point.y < halfY && point.y > -halfY &&
            point.z < halfZ && point.z > -halfZ)
		 {
			 return true;
		 }
         else
		 {
			 return false;
		 }

 	 }

	IEnumerator Shoot()
	{
		hitscanTransform.transform.localScale = new Vector3(200,40,7.5f);

		animator.Play("Fire");
		gunshotParticle.Play();
		this.StartCoroutine(FlashMuzzleFlare());

		var allPlayers = PlayerController.GetAllPlayers().Where(x => x != player).ToList();
		for (int i = 0; i < allPlayers.Count; i++)
		{
			if (PointInOABB(allPlayers[i].transform.position, hitscanCollider))
			{
				Debug.Log("HIT " + allPlayers[i], allPlayers[i].gameObject);
			}
		}

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
