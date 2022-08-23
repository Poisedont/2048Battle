using UnityEngine;

namespace FSM
{
    public abstract class BaseState
    {
        public abstract int GetID();

        public virtual void OnEnter(MonoBehaviour _owner) { }
        public abstract void OnUpdate(MonoBehaviour _owner);
        public virtual void OnExit(MonoBehaviour _owner) { }
        public virtual void OnFixedUpdate(MonoBehaviour _owner) { }
    }
}
