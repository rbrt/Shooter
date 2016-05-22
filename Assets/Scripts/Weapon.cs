using UnityEngine;
using System.Collections;
using System.Linq;

public class Weapon : MonoBehaviour
{

	[SerializeField] protected Light muzzleFlash;
	[SerializeField] protected ParticleSystem gunshotParticle;
	[SerializeField] protected float damage,
						   			 reloadTime;

	[SerializeField] protected Animator animator;

	[SerializeField] protected bool hitscan;
	[SerializeField] protected GameObject hitscanTransform;

	[SerializeField] protected PlayerController player;

	protected bool shooting = false;
	protected BoxCollider hitscanCollider;

	protected void Awake()
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

	protected bool PointInOABB (Vector3 point, BoxCollider box )
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

	public virtual int GetWeaponDamage(float distance)
	{
		Debug.LogWarning("GetDamage not implemented");
		return 0;
	}

	public virtual IEnumerator Shoot()
	{
		Debug.LogWarning("Shoot not implemented");
		yield break;
	}

	public virtual IEnumerator FlashMuzzleFlare()
	{
		Debug.LogWarning("MuzzleFlash not implemented");
		yield break;
	}


}
