using CommandSystem;
using EmreBeratKR.ServiceLocator;
using UnitSystem;

namespace Particles
{
    public class ParticleImpulser : ServiceBehaviour
    {
        private void Awake()
        {
            UnitCommander.OnFailedToExecuteCommand += UnitCommander_OnFailedToExecuteCommand;
        }

        private void OnDestroy()
        {
            UnitCommander.OnFailedToExecuteCommand -= UnitCommander_OnFailedToExecuteCommand;
        }

        
        private void UnitCommander_OnFailedToExecuteCommand(UnitCommander.FailedToExecuteCommandArgs args)
        {
            ParticleManager.EmitTextAtPosition(args.status.AsString(), args.worldPosition);
        }
    }
}