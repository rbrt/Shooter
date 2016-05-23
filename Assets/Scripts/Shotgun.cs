using UnityEngine;
using System.Collections;
using System.Linq;

public class Shotgun : Weapon
{

	float effectiveDistance = 20f;

	// Shotgun does more damage up close
	public override int GetWeaponDamage(float distance)
	{
		return (int)(damage * (1 - (distance / effectiveDistance)));
	}

   public override IEnumerator Shoot()
   {
        hitscanTransform.transform.localScale = new Vector3(200,40,7.5f);

        animator.Play("Fire");
        gunshotParticle.Play();
        this.StartCoroutine(FlashMuzzleFlare());
        AudioManager.Instance.PlayOneShotClip(weaponFireEffect);

        var allPlayers = PlayerController.GetAllPlayers().Where(x => x != player).ToList();
        for (int i = 0; i < allPlayers.Count; i++)
        {
           if (PointInOABB(allPlayers[i].transform.position, hitscanCollider))
           {
        	   float distance = (allPlayers[i].transform.position - transform.position).magnitude;
        	   allPlayers[i].GetComponent<PlayerStats>().TakeDamage(GetWeaponDamage(distance));
           }
        }

        yield return new WaitForSeconds(1.3f);

        shooting = false;
   }

   public override IEnumerator FlashMuzzleFlare()
   {
	   muzzleFlash.enabled = true;
	   yield return new WaitForSeconds(.25f);
	   muzzleFlash.enabled = false;
   }
}
