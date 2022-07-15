using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachineNamespace
{
    public class OnGroundJumpState : OnGroundSuperState
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
        public OnGroundJumpState(Player player, PlayerData playerData, PlayerInputHandler playerInputHandler, PlayerCollisionHandler playerCollisionHandler, PlayerDamageAndAffectHandler playerDamageAndAffectHandler,
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

            RaycastHit2D rayHit = _playerCollisionHandler.DetectGroundContact();
            RaycastHit2D rayHitLate = _playerCollisionHandler.DetectLateGroundContact();

            if (rayHit)
            {
                //check for moving ground
                if (rayHit.transform.gameObject.layer == 15)
                {
                    GameObject jumpFx = _player.InstantiateObjectWithRef(_playerData.OnGroundJump_JumpFXAnim, _transform.position - new Vector3(0f, 4f, 0f), Quaternion.Euler(0f, 0f, 90f));
                    jumpFx.transform.parent = rayHit.transform;

                    _player.transform.parent = null;
                    _player.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;

                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0.0f);

                    var movingPlatformVelocity = rayHit.transform.GetComponent<MovingPlatform>().PlatformVelocityVector;

                    var jumpForceVector = new Vector2(movingPlatformVelocity.x * _playerData.OnGroundJump_MovingPlatformScaleCoeffient, _playerData.OnGroundJump_JumpForce + (movingPlatformVelocity.y * _playerData.OnGroundJump_MovingPlatformScaleCoeffient));
                    var jumpForceVectorClamped = new Vector2(Mathf.Clamp(jumpForceVector.x, -3000f, 3000f), Mathf.Clamp(jumpForceVector.y, 1000f, 4500f));

                    _rigidbody2D.AddForce(jumpForceVectorClamped);
                }
                else
                {
                    _player.InstantiateObject(_playerData.OnGroundJump_JumpFXAnim, _transform.position - new Vector3(0f, 4f, 0f), Quaternion.Euler(0f, 0f, 90f));

                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0.0f);
                    Jump();
                }
            }
            else if (rayHitLate)
            {
                //check for moving ground
                if (rayHitLate.transform.gameObject.layer == 15)
                {
                    GameObject jumpFx = _player.InstantiateObjectWithRef(_playerData.OnGroundJump_JumpFXAnim, _transform.position - new Vector3(0f, 4f, 0f), Quaternion.Euler(0f, 0f, 90f));
                    jumpFx.transform.parent = rayHitLate.transform;

                    _player.transform.parent = null;
                    _player.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;

                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0.0f);

                    var movingPlatformVelocity = rayHitLate.transform.GetComponent<MovingPlatform>().PlatformVelocityVector;

                    var jumpForceVector = new Vector2(movingPlatformVelocity.x * _playerData.OnGroundJump_MovingPlatformScaleCoeffient, _playerData.OnGroundJump_JumpForce + (movingPlatformVelocity.y * _playerData.OnGroundJump_MovingPlatformScaleCoeffient));
                    var jumpForceVectorClamped = new Vector2(Mathf.Clamp(jumpForceVector.x, -3000f, 3000f), Mathf.Clamp(jumpForceVector.y, 1000f, 4500f));

                    _rigidbody2D.AddForce(jumpForceVectorClamped);
                }
                else
                {
                    _player.InstantiateObject(_playerData.OnGroundJump_JumpFXAnim, _transform.position - new Vector3(0f, 4f, 0f), Quaternion.Euler(0f, 0f, 90f));

                    _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0.0f);
                    Jump();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
        #endregion



        #region Private Methods
        private void Jump() // Add a one time vertical force to the player.
        {
            _rigidbody2D.AddForce(new Vector2(0f, _playerData.OnGroundJump_JumpForce));
        }
        #endregion
    }
}

