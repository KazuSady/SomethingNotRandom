using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    
    public void StartParticles()
    {
        particleSystem.gameObject.SetActive(true);
        particleSystem.Play();
    }

    public void StopParticles()
    {
        particleSystem.Stop();
        particleSystem.gameObject.SetActive(false);
    }
}
