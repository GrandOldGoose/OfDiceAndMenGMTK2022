using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachineNamespace
{
    public class InAirSuperState : PlayerBaseState
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

        private float _velocityXRef = 0f;
        #endregion



        #region Constructors
        public InAirSuperState(Player player, PlayerData playerData, PlayerInputHandler playerInputHandler, PlayerCollisionHandler playerCollisionHandler, PlayerDamageAndAffectHandler playerDamageAndAffectHandler,
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
        }

        public override void OnExit()
        {
        }
        #endregion



        #region Protected Methods
        protected void Move()
        {
            float targetVelocity = _playerInputHandler.RawMoveInput * _playerData.OnGroundMove_MoveSpeed;
            _rigidbody2D.velocity = new Vector2(Mathf.SmoothDamp(_rigidbody2D.velocity.x, targetVelocity, ref _velocityXRef, _playerData.OnGroundMove_MovementSmoothing), _rigidbody2D.velocity.y);
        }
        #endregion
    }
}