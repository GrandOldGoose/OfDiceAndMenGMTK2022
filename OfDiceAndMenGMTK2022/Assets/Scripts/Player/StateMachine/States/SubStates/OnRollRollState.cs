using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachineNamespace
{
    public class OnRollRollState : OnRollSuperState
    {

        #region Events
        public static event Action OnRollEnter;
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

        private bool _initialGroundContact = true;
        #endregion



        #region Constructors
        public OnRollRollState(Player player, PlayerData playerData, PlayerInputHandler playerInputHandler, PlayerCollisionHandler playerCollisionHandler, PlayerDamageAndAffectHandler playerDamageAndAffectHandler,
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

            if (_playerCollisionHandler.DetectGroundContact())
            {
                _initialGroundContact = false;
                if (!_player.CanRollLand)
                {

                    _rigidbody2D.AddForce(new Vector2(UnityEngine.Random.Range(-_playerData.OnRollRollState_RNGForceApplied.x, _playerData.OnRollRollState_RNGForceApplied.x), _playerData.OnRollRollState_RNGForceApplied.y));
                }
            }
            else
            {
                _initialGroundContact = true;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _rigidbody2D.velocity = Vector2.zero;
            _playerDamageAndAffectHandler.IsKnockedback = true;
            _playerDamageAndAffectHandler.InitialKnockback = false;
            _rigidbody2D.AddForce(_playerDamageAndAffectHandler.KnockbackForce);

            _player.CanRollLand = false;
            _player.DoubleJumpCount = 0;
            OnRollEnter?.Invoke();
        }

        public override void OnExit()
        {
            base.OnExit();

            _playerDamageAndAffectHandler.IsKnockedback = false;
        }
        #endregion
    }
}
