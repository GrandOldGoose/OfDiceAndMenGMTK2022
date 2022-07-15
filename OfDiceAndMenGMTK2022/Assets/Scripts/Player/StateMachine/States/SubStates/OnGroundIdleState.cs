using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachineNamespace
{
    public class OnGroundIdleState : OnGroundSuperState
    {
        #region Fields
        private readonly Player _player;
        private readonly PlayerData _playerData;
        private readonly PlayerInputHandler _playerInputHandler;
        private readonly PlayerCollisionHandler _playerCollisionHandler;
        private readonly PlayerDamageAndAffectHandler _playerDamageAndAffectHandler;
        private readonly PlayerEnemyDetectionHandler _playerEnemyDetectionHandler;
        private readonly PlayerProjectileSpawner _playerProjectileSpawner;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly Animator _animator;
        #endregion



        #region Constructors
        public OnGroundIdleState(Player player, PlayerData playerData, PlayerInputHandler playerInputHandler, PlayerCollisionHandler playerCollisionHandler, PlayerDamageAndAffectHandler playerDamageAndAffectHandler,
            PlayerEnemyDetectionHandler playerEnemyDetectionHandler, PlayerProjectileSpawner playerProjectileSpawner, Rigidbody2D rigidbody2D, Animator animator)
            : base(player, playerData, playerInputHandler, playerCollisionHandler, playerDamageAndAffectHandler, playerEnemyDetectionHandler, playerProjectileSpawner, rigidbody2D, animator)
        {
            _player = player;
            _playerData = playerData;
            _playerInputHandler = playerInputHandler;
            _playerCollisionHandler = playerCollisionHandler;
            _playerDamageAndAffectHandler = playerDamageAndAffectHandler;
            _playerEnemyDetectionHandler = playerEnemyDetectionHandler;
            _playerProjectileSpawner = playerProjectileSpawner;
            _rigidbody2D = rigidbody2D;
            _animator = animator;
        }
        #endregion



        #region Public Inherited Methods
        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            SetVelocityXZero();
            ChangePhysicsMaterialToFullFriction();
        }

        public override void OnExit()
        {
            base.OnExit();
            ChangePhysicsMaterialToFrictionless();
        }
        #endregion



        #region Private Methods
        private void SetVelocityXZero()
        {
            _rigidbody2D.velocity = new Vector2(0.0f, _rigidbody2D.velocity.y);
        }

        private void ChangePhysicsMaterialToFrictionless()
        {
            _rigidbody2D.sharedMaterial = _playerData.Physics_NoFriction;
        }

        private void ChangePhysicsMaterialToFullFriction()
        {
            _rigidbody2D.sharedMaterial = _playerData.Physics_FullFriction;
        }
        #endregion
    }
}