using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpotlightManager : MonoBehaviour
{

	List<Light> spotlights;
	Light flickerA,
		  flickerB;

	SafeCoroutine lightFlickerA,
				  lightFlickerB;

	void Awake()
	{
		spotlights = FindObjectsOfType<Light>().Where(light => light.type == LightType.Spot).ToList();
		this.StartSafeCoroutine(FlickerLights());
	}

	Light GetRandomLight(Light targetLight)
	{
		var lightPool = spotlights.Where(light => light != targetLight).ToList();
		targetLight = lightPool[Random.Range(0,lightPool.Count)];

		return targetLight;
	}

	IEnumerator FlickerLights()
	{
		while (true)
		{
			if (lightFlickerA == null || !lightFlickerA.IsRunning)
			{
				flickerA = GetRandomLight(flickerA);
				lightFlickerA = this.StartSafeCoroutine(FlickerLight(flickerA, Random.Range(2,5)));
			}
			if (lightFlickerB == null || !lightFlickerB.IsRunning)
			{
				flickerB = GetRandomLight(flickerB);
				lightFlickerB = this.StartSafeCoroutine(FlickerLight(flickerB, Random.Range(2,5)));
			}
			yield return null;
		}
	}

	IEnumerator FlickerLight(Light light, float duration)
	{
		float intensity = light.intensity;
		float flickerSpeed = .1f;
		int flashCount = Random.Range(3,10);

		for (int i = 0; i < flashCount; i++)
		{
			for (float j = 0; j < 1; j += Time.time / flickerSpeed)
			{
				light.intensity = (1 - i) * intensity;
				yield return null;
			}
			yield return new WaitForSeconds(Random.Range(.05f, .3f));

			for (float j = 0; j < 1; j += Time.time / flickerSpeed)
			{
				light.intensity = i * intensity;
				yield return null;
			}
			light.intensity = intensity;
			yield return new WaitForSeconds(Random.Range(.1f, .3f));
		}

		yield return new WaitForSeconds(Random.Range(2,8));
	}
}
