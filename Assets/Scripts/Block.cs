using UnityEngine;

public class Block : MonoBehaviour
{
    public float durabilitySecond = 2.0f;
    public ParticleSystem breakParticlePrefeb;
    ParticleSystem breakParticleInstance;
    float lastBreakProgress = 0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (breakParticleInstance)
        {
            if (lastBreakProgress < Time.time - .1f) Destroy(breakParticleInstance);
        }
    }

    public bool TryBreak(float breakSecond)
    {
        lastBreakProgress = Time.time;
        if (breakParticlePrefeb && !breakParticleInstance)
        {
            breakParticleInstance = Instantiate(breakParticlePrefeb);
            breakParticleInstance.transform.position = transform.position;
        }
        if (breakSecond > durabilitySecond)
        {
            Break();
            return true;
        }
        return false;
    }

    private void Break()
    {
        if (breakParticleInstance) Destroy(breakParticleInstance);
        Destroy(gameObject);
    }
}
