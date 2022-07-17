using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachineNamespace
{
    public class InAirOnGravLiftState : InAirSuperState
    {
        #region Events
        public static event Action OnMovementObjectGravLiftExit;
        #endregion



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
        public InAirOnGravLiftState(Player player, PlayerData playerData, PlayerInputHandler playerInputHandler, PlayerCollisionHandler playerCollisionHandler, PlayerDamageAndAffectHandler playerDamageAndAffectHandler,
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
            _player.DoubleJumpCount = _playerData.InAirJump_DoubleJumpCount;
            _player.CanGravLift = false;
            AddForceToPlayer();
        }

        public override void OnExit()
        {
            base.OnExit();

            OnMovementObjectGravLiftExit.Invoke();
        }
        #endregion



        #region Private Methods
        private void AddForceToPlayer()
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.AddForce(new Vector2(_playerDamageAndAffectHandler.GravLiftForceApplied.x, _playerDamageAndAffectHandler.GravLiftForceApplied.y));
        }
        #endregion
    }
}
