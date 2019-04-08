using UnityEngine;

public class ParticleScaler : MonoBehaviour
{
    public float scaleRatioX;
    void Start ()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.startSize = particleSystem.startSize * scaleRatioX;
        particleSystem.startSpeed = particleSystem.startSpeed * scaleRatioX;
    }
}
