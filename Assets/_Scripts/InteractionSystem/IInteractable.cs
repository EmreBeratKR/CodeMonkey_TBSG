using System;
using UnitSystem;

namespace InteractionSystem
{
    public interface IInteractable
    {
        void Interact(Unit interactor, Action onComplete);
        bool IsValid();
    }
}