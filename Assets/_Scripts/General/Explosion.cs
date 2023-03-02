using UnityEngine;

namespace General
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private float force = 15f;
        
        
        public void Explode(
            float explosionForce,
            Vector3 explosionPosition,
            float explosionRadius = 10f,
            float upwardsModifier = 0f, 
            ForceMode mode = ForceMode.Impulse)
        {
            ApplyExplosiveForceToAllBones(
                root,
                explosionForce,
                explosionPosition,
                explosionRadius,
                upwardsModifier,
                mode);
        }
        
        public void Explode(
            Vector3 explosionPosition,
            float explosionRadius = 10f,
            float upwardsModifier = 0f, 
            ForceMode mode = ForceMode.Impulse)
        {
            ApplyExplosiveForceToAllBones(
                root,
                force,
                explosionPosition,
                explosionRadius,
                upwardsModifier,
                mode);
        }
        
        
        private static void ApplyExplosiveForceToAllBones(
            Transform bone,
            float explosionForce,
            Vector3 explosionPosition,
            float explosionRadius,
            float upwardsModifier, 
            ForceMode mode)
        {
            var childCount = bone.childCount;

            for (var i = 0; i < childCount; i++)
            {
                var childBone = bone.GetChild(i);
            
                if (!childBone.TryGetComponent(out Rigidbody rigidbody)) continue;
            
                rigidbody.AddExplosionForce(
                    explosionForce,
                    explosionPosition,
                    explosionRadius,
                    upwardsModifier,
                    mode);
            }
        }
    }
}