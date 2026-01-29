using UnityEngine;

public class PoisonEffectController : MonoBehaviour
{
    [SerializeField] private ParticleSystem poisonParticle;

    private int poisonStackCount = 0;

    private void Awake()
    {
        if (poisonParticle == null)
            poisonParticle = GetComponentInChildren<ParticleSystem>();

        poisonParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void OnPoisonApplied(int stacks)
    {
        poisonStackCount += stacks;

        if (!poisonParticle.isPlaying)
            poisonParticle.Play();
    }

    public void OnPoisonReduced(int stacks)
    {
        poisonStackCount -= stacks;

        if (poisonStackCount <= 0)
        {
            poisonStackCount = 0;
            poisonParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public void ClearPoison()
    {
        poisonStackCount = 0;
        poisonParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
