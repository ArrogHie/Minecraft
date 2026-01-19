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
            if (lastBreakProgress < Time.time - .1f)
            {
                var emission = breakParticleInstance.emission;
                emission.enabled = false;
            }
        }
    }

    public bool TryBreak(float breakSecond)
    {
        lastBreakProgress = Time.time;
        if (breakParticlePrefeb && !breakParticleInstance)
        {
            Vector3 position = transform.position;
            position += new Vector3(0.5f, 0.5f, 0.5f);
            breakParticleInstance = Instantiate(breakParticlePrefeb, position, Quaternion.identity);
            breakParticleInstance.transform.parent = transform;
        }
        var emission = breakParticleInstance.emission;
        emission.enabled = true;
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
