using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachineNamespace
{
    public class OnDeathSuperState : PlayerBaseState
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
        public OnDeathSuperState(Player player, PlayerData playerData, PlayerInputHandler playerInputHandler, PlayerCollisionHandler playerCollisionHandler, PlayerDamageAndAffectHandler playerDamageAndAffectHandler,
            PlayerEnemyDetectionHandler playerEnemyDetectionHandler, PlayerProjectileSpawner playerProjectileSpawner, Rigidbody2D rigidbody2D, Animator animator)
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
        }

        public override void PhysicsUpdate()
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Entered " + _animator.GetLayerName(0) + "." + this.ToString().Remove(0, 28));
            _animator.Play(_animator.GetLayerName(0) + "." + this.ToString().Remove(0, 28), 0);


            _playerInputHandler.SetInputsFalse();

            _playerDamageAndAffectHandler.InitialDeath = false;
            _playerDamageAndAffectHandler.IsDead = true;

            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.isKinematic = true;

            _playerCollisionHandler.CollisionCollider.enabled = false;
            _playerDamageAndAffectHandler.CanBeDamaged = false;
        }

        public override void OnExit()
        {

        }
        #endregion
    }
}

