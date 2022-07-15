using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachineNamespace
{
    public abstract class PlayerBaseState
    {
        #region Public Abstract Methods
        public abstract void LogicUpdate();

        public abstract void PhysicsUpdate();

        public abstract void OnEnter();

        public abstract void OnExit();
        #endregion
    }
}
