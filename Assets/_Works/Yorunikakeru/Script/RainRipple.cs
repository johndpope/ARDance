using System.Collections.Generic;
using UnityEngine;

public class RainRipple : MonoBehaviour
{
    [SerializeField] private Yorunikakeru.Ripple _ripple;
    private ParticleSystem particle;
    private List<ParticleCollisionEvent> collisionEventList = new List<ParticleCollisionEvent>();
    
    private void Awake() {
        particle = GetComponent<ParticleSystem>();
    }
    
    private void OnParticleCollision(GameObject other)
    {
        particle.GetCollisionEvents(other, collisionEventList);
        foreach(var collisionEvent in collisionEventList) {
            Vector3 pos = collisionEvent.intersection;
            _ripple.Pulse(pos, Yorunikakeru.Ripple.PulseStrength.Weak);
        }
    }
}
