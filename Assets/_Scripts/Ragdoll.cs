using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private static readonly int SweepID = Animator.StringToHash("Sweep");
    
    
    [SerializeField] private Transform rootBone;
    [SerializeField] private Animator animator;


    public void Setup(Transform targetRootBone)
    {
        MatchAllBones(targetRootBone, rootBone);
        
        const float force = 150f;
        ApplyExplosiveForce(force, transform.position);
        TriggerSweep();
    }


    private void OnAnimatorSweep()
    {
        Destroy(gameObject);
    }
    
    
    private void ApplyExplosiveForce(
        float explosionForce,
        Vector3 explosionPosition,
        float explosionRadius = 10f,
        float upwardsModifier = 0f, 
        ForceMode mode = ForceMode.Impulse)
    {
        ApplyExplosiveForceToAllBones(
            rootBone,
            explosionForce,
            explosionPosition,
            explosionRadius,
            upwardsModifier,
            mode);
    }

    private void TriggerSweep()
    {
        animator.SetTrigger(SweepID);
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