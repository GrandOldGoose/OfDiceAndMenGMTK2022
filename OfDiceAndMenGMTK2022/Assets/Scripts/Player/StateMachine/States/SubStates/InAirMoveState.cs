using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachineNamespace
{
    public class InAirMoveState : InAirSuperState
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
        public InAirMoveState(Player player, PlayerData playerData, PlayerInputHandler playerInputHandler, PlayerCollisionHandler playerCollisionHandler, PlayerDamageAndAffectHandler playerDamageAndAffectHandler,
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
            _player.CheckForMoveInputDirectionChangeThenFlip();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            Move();
            if(_rigidbody2D.velocity.y < -_playerData.InAirMove_TerminalVelocity) { _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, -_playerData.InAirMove_TerminalVelocity + 1) ; }
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
        #endregion
    }
}
