using UnityEngine;

namespace General
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] private Explosion explosion;
        [SerializeField] private Sweeper sweeper;
        [SerializeField] private Transform rootBone;


        public void Setup(Transform targetRootBone, Vector3 impactOffset)
        {
            MatchAllBones(targetRootBone, rootBone);

            impactOffset = impactOffset == Vector3.zero
                ? Randomizer.GetInsideUnitCircleXZNormalized()
                : impactOffset;
            
            var explosionPosition = transform.position + impactOffset;
            explosion.Explode(explosionPosition);
            sweeper.Sweep(() =>
            {
                Destroy(gameObject);
            });
        }


        private static void MatchAllBones(Transform targetBone, Transform bone)
        {
            foreach (Transform targetChildBone in targetBone)
            {
                var childBone = bone.Find(targetChildBone.name);
            
                if (!childBone) continue;

                childBone.position = targetChildBone.position;
                childBone.rotation = targetChildBone.rotation;
            
                MatchAllBones(targetChildBone, childBone);
            }
        }
    }
}