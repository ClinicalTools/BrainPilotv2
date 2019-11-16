
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSeekOptimized : MonoBehaviour
{
    public Transform target;
    public float force = 10.0f;

    new ParticleSystem particleSystem;
    ParticleSystem.Particle[] particles;

    ParticleSystem.MainModule particleSystemMainModule;

	Vector3 targetPos;
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemMainModule = particleSystem.main;
		if (col != null) {
			targetPos = col.ClosestPoint(transform.position);
		} else {
			targetPos = target.position;
		}
	}
	public MeshCollider col;

    void LateUpdate()
    {
		int maxParticles = particleSystemMainModule.maxParticles;

        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        particleSystem.GetParticles(particles);
        float forceDeltaTime = force * Time.deltaTime;

        Vector3 targetTransformedPosition;
		
        switch (particleSystemMainModule.simulationSpace)
        {
			case ParticleSystemSimulationSpace.Local:
                {
                    targetTransformedPosition = transform.InverseTransformPoint(targetPos);
                    break;
                }
            case ParticleSystemSimulationSpace.Custom:
                {
                    targetTransformedPosition = particleSystemMainModule.customSimulationSpace.InverseTransformPoint(targetPos);
                    break;
                }
            case ParticleSystemSimulationSpace.World:
                {
					targetTransformedPosition = targetPos;// target.position;
                    break;
                }
            default:
                {
                    throw new System.NotSupportedException(

                        string.Format("Unsupported simulation space '{0}'.",
                        System.Enum.GetName(typeof(ParticleSystemSimulationSpace), particleSystemMainModule.simulationSpace)));
                }
        }

        int particleCount = particleSystem.particleCount;

        for (int i = 0; i < particleCount; i++)
        {
			if (particles[i].startLifetime > 1) {
				Vector3 directionToTarget = Vector3.Normalize(targetTransformedPosition - particles[i].position);
				Vector3 seekForce = directionToTarget * forceDeltaTime;

				particles[i].velocity += seekForce;
			} else {
				

				switch (testthing) {
					case testingEnum.test1:
						particles[i].velocity = Vector3.zero;
						break;
					case testingEnum.test2:
						//particleSystem.Clear();
						ParticleSystem.MainModule main = particleSystem.main;
						main.startSpeed = 0;
						break;
					case testingEnum.test3:
						particles[i].velocity = Vector3.zero;
						
						ParticleSystem.NoiseModule noise = particleSystem.noise;
						noise.enabled = false;
						Invoke("EnableNoiseAgain", .8f);
						break;
				}
			}
        }

        particleSystem.SetParticles(particles, particleCount);
    }
	private void EnableNoiseAgain()
	{
		ParticleSystem.NoiseModule noise = particleSystem.noise;
		noise.enabled = true;
	}

	public enum testingEnum
	{
		none,
		test1,
		test2,
		test3
	};
	public testingEnum testthing;
	

	[ContextMenu("Start")]
	private void st1()
	{
		particleSystem.Play();
	}
	[ContextMenu("Stop")]
	private void st2()
	{
		particleSystem.Stop();
	}
	[ContextMenu("Restart")]
	private void st3()
	{
		particleSystem.Clear();
		particleSystem.Play();
	}

}
