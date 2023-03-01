using EmreBeratKR.ServiceLocator;
using UnityEngine;

namespace Particles
{
    public class ParticleManager : ServiceBehaviour
    {
        private const float DefaultFontSize = 2f;


        [SerializeField] private TextParticle textParticlePrefab;


        public static void EmitTextAtPosition(
            object obj, 
            Vector3 position, 
            float fontSize = DefaultFontSize)
        {
            var instance = GetInstance();
            var prefab = instance.textParticlePrefab;
            var rotation = Quaternion.identity;
            var parent = instance.transform;
            var textParticle = Instantiate(prefab, position, rotation, parent);
            textParticle.SetText(obj, fontSize);
        }
        

        private static ParticleManager GetInstance()
        {
            return ServiceLocator.Get<ParticleManager>();
        }
    }
}