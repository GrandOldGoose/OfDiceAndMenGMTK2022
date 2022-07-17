using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateMachineNamespace;

public class Player : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private Camera _cam;
    [SerializeField] private Transform _fX;
    #endregion



    #region Fields
    private StateMachine _playerStateMachine;

    private PlayerInputHandler _playerInputHandler;
    private PlayerCollisionHandler _playerCollisionHandler;
    private PlayerDamageAndAffectHandler _playerDamageAndAffectHandler;
    private PlayerUIHandler _playerUIHandler;
    private PlayerEnemyDetectionHandler _playerEnemyDetectionHandler;
    private PlayerProjectileSpawner _playerProjectileSpawner;
    private Animator _animator;

    private bool _startStart = false;
    private bool _setAllStatesInitial = false;

    private bool _facingRight = true;
    private int _doubleJumpCount = 0;
    private float _previousInputDirection = 0f;

    private bool _canGravLift = true;
    private bool _canFire = false;
    #endregion



    #region Properties
    public PlayerData PlayerData { get => _playerData; }

    public bool StartStart { get => _startStart; set => _startStart = value; }
    public bool SetAllStatesInitial { get => _setAllStatesInitial; set => _setAllStatesInitial = value; }

    public bool FacingRight { get => _facingRight; set => _facingRight = value; }
    public int DoubleJumpCount { get => _doubleJumpCount; set => _doubleJumpCount = value; }
    public float PreviousInputDirection { set => _previousInputDirection = value; }

    public bool CanFire { get => _canFire; set => _canFire = value; }
    public bool CanGravLift { set => _canGravLift = value; }
    #endregion



    #region Unity Awake Callback Method (contains statemachine setups)
    private void Awake()
    {
        //Assign callers to events:
        Action StartWaitBetweenNextGravLift() => () => Invoke("WaitBetweenNextGravLift", _playerData.InAirOnGravLiftState_WaitBetweenNextGravLift);
        InAirOnGravLiftState.OnMovementObjectGravLiftExit += StartWaitBetweenNextGravLift();

        Action SpawnWaitTime() => () => Invoke("WaitToSpawn", _playerData.DamageAndAffectHandler_SpawnWait);
        OnSpawnWaitState.OnSpawnWait += SpawnWaitTime();

        //Create instances of state machine.
        _playerStateMachine = new StateMachine();


        //Create instances of required components:
        var rigidbody2D = GetComponent<Rigidbody2D>();
        var transform = GetComponent<Transform>();



        _playerCollisionHandler = GetComponent<PlayerCollisionHandler>();
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _playerDamageAndAffectHandler = GetComponent<PlayerDamageAndAffectHandler>();
        _playerUIHandler = GetComponent<PlayerUIHandler>();
        _playerEnemyDetectionHandler = GetComponent<PlayerEnemyDetectionHandler>();
        _playerProjectileSpawner = GetComponent<PlayerProjectileSpawner>();
        _animator = GetComponent<Animator>();


        //Assign Fields:
        _doubleJumpCount = _playerData.InAirJump_DoubleJumpCount;





        #region Player State Macine
        //set all states you are using in the player Movement Hierachial State Macine:
        //SuperStates.
        var onGroundSuperState = new OnGroundSuperState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var inAirSuperState = new InAirSuperState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onRollSuperState = new OnRollSuperState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onDamageSuperState = new OnDamageSuperState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onDeathSuperState = new OnDeathSuperState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onSpawnSuperState = new OnSpawnSuperState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        //OnGroundSubStates.
        var onGroundMoveState = new OnGroundMoveState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onGroundIdleState = new OnGroundIdleState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onGroundJumpState = new OnGroundJumpState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onGroundLandState = new OnGroundLandState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onGroundThroughFloorState = new OnGroundThroughFloorState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onGroundAttackState = new OnGroundAttackState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        //InAirSubStates.
        var inAirMoveState = new InAirMoveState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var inAirJumpState = new InAirJumpState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var inAirOnGravLiftState = new InAirOnGravLiftState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        //OnRollSubStates.
        var onRollRollState = new OnRollRollState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        //OnDamageSubStates.
        var onDamageRollState = new OnDamageRollState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onDamageFallState = new OnDamageFallState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        //OnDeathSubStates.
        var onDeathRollState = new OnDeathRollState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onDeathFallState = new OnDeathFallState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onDeathSpikesState = new OnDeathSpikesState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        //OnSpawnSubStates.
        var onSpawnInitialIdleState = new OnSpawnInitialIdleState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onSpawnWaitState = new OnSpawnWaitState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onSpawnInitialState = new OnSpawnInitialState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);
        var onSpawnFallState = new OnSpawnFallState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerEnemyDetectionHandler, _playerProjectileSpawner, rigidbody2D, _animator);

        //Set all transitions that directly connect to another state:
        //SuperState Transitions.
        PMAT(onGroundSuperState, inAirMoveState, InAirAndAnimIsCompletedOrAnimIsLooping());
        PMAT(inAirSuperState, onGroundLandState, HitGround());
        PMAT(onRollRollState, onGroundIdleState, AnimCompleted());
        PMAT(onDeathSuperState, onSpawnInitialIdleState, AnimCompleted());
        //OnGround SubState Transitions.
        PMAT(onGroundIdleState, onGroundMoveState, MoveInput());
        PMAT(onGroundIdleState, onGroundJumpState, JumpInput());
        PMAT(onGroundIdleState, onGroundThroughFloorState, ThroughFloorInputAndOnSlimGround());
        PMAT(onGroundIdleState, onGroundAttackState, FireInput());
        PMAT(onGroundMoveState, onGroundJumpState, JumpInput());
        PMAT(onGroundMoveState, onGroundThroughFloorState, ThroughFloorInputAndOnSlimGround());
        PMAT(onGroundMoveState, onGroundIdleState, NoMoveInput());
        PMAT(onGroundMoveState, onGroundAttackState, FireInput());
        PMAT(onGroundLandState, onGroundJumpState, JumpInput());
        PMAT(onGroundLandState, onGroundThroughFloorState, ThroughFloorInputAndOnSlimGround());
        PMAT(onGroundLandState, onGroundIdleState, AnimCompletedAndNoMoveInput());
        PMAT(onGroundLandState, onGroundMoveState, AnimCompletedAndMoveInput());
        PMAT(onGroundLandState, onGroundAttackState, FireInput());
        PMAT(onGroundJumpState, inAirMoveState, AnimCompleted());
        PMAT(onGroundAttackState, inAirMoveState, AnimCompleted());
        PMAT(onGroundThroughFloorState, inAirMoveState, AnimCompleted());
        //InAir SubState Transitions.
        PMAT(inAirMoveState, inAirJumpState, JumpInputAndCanDoubleJump());
        PMAT(inAirJumpState, inAirMoveState, AnimCompleted());
        PMAT(inAirOnGravLiftState, inAirMoveState, AnimCompleted());
        //OnDamage Substate Transitions.
        PMAT(onDamageFallState, onSpawnWaitState, AnimCompleted());
        PMAT(onDamageRollState, onGroundIdleState, AnimCompleted());
        //OnDeath Substate Transitions.
        //OnSpawn Substate Transitions.
        PMAT(onSpawnInitialIdleState, onSpawnInitialState, OnStart());
        PMAT(onSpawnWaitState, onSpawnFallState, WaitTimeAndFallDamage());
        PMAT(onSpawnFallState, inAirMoveState, AnimCompleted());
        PMAT(onSpawnInitialState, inAirMoveState, AnimCompleted());
        //Set transitions that come from any transition:
        PMAAT(inAirOnGravLiftState, OnGravLiftContactAndCanGravLiftAndNotDeadOrKnockbackOrRoll());
        PMAAT(onRollRollState, RollTriggerAndNotDeadOrKnockbackOrRoll());
        PMAAT(onDamageRollState, OnKnockbackAndNotDeadOrKnockbackOrRoll());
        PMAAT(onDamageFallState, OnFallAndNotDead());
        PMAAT(onDeathRollState, OnDeathAndMeleeDamageAndNotDeadOrKnockbackOrRoll());
        PMAAT(onDeathFallState, OnDeathAndFallDamageAndNotDead());
        PMAAT(onDeathSpikesState, OnDeathAndSpikeDamageAndNotDeadOrKnockbackOrRoll());
        PMAAT(onSpawnInitialIdleState, Reset());


        //Set all of our conditions:
        //Super State Conditions.
        Func<bool> InAirAndAnimIsCompletedOrAnimIsLooping() => () => !_playerCollisionHandler.DetectGroundContact() && !_playerCollisionHandler.DetectLateGroundContact() && (!AnimatorIsPlaying(0) || AnimLooping(0));
        Func<bool> HitGround() => () => _playerCollisionHandler.DetectGroundContact();
        //OnGround SubState Conditions.
        Func<bool> MoveInput() => () => Mathf.Abs(_playerInputHandler.RawMoveInput) >= 0.2f;
        Func<bool> NoMoveInput() => () => Mathf.Abs(_playerInputHandler.RawMoveInput) < 0.2f;
        Func<bool> JumpInput() => () => _playerInputHandler.JumpInput;
        Func<bool> FireInput() => () => _playerInputHandler.FireInput;
        Func<bool> AnimCompletedAndNoMoveInput() => () => !AnimatorIsPlaying(0) && Mathf.Abs(_playerInputHandler.RawMoveInput) < 0.2f;
        Func<bool> AnimCompletedAndMoveInput() => () => !AnimatorIsPlaying(0) && Mathf.Abs(_playerInputHandler.RawMoveInput) >= 0.2f;
        Func<bool> ThroughFloorInputAndOnSlimGround() => () => _playerInputHandler.FallThroughFloorInput && _playerCollisionHandler.DetectSlimGroundContact();
        //InAir SubState Conditions.
        Func<bool> JumpInputAndCanDoubleJump() => () => _playerInputHandler.JumpInput && _doubleJumpCount > 0;
        Func<bool> AnimCompleted() => () => !AnimatorIsPlaying(0);
        //OnRoll Substate Conditions.
        //OnDamage Substate Conditions.
        //OnDeath Substate Conditions.
        //OnSpawn Substate Conditions.
        Func<bool> OnStart() => () => _startStart;
        Func<bool> WaitTimeAndFallDamage() => () => !_playerDamageAndAffectHandler.IsInSpawnWait && _playerDamageAndAffectHandler.DamageType == DamageType.FALL;


        //AnyTransition Conditions.
        Func<bool> OnGravLiftContactAndCanGravLiftAndNotDeadOrKnockbackOrRoll() => () => _playerDamageAndAffectHandler.GravLiftContact && _canGravLift && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback || _playerDamageAndAffectHandler.IsInRoll);
        Func<bool> RollTriggerAndNotDeadOrKnockbackOrRoll() => () => _playerDamageAndAffectHandler.InitialRoll && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback||_playerDamageAndAffectHandler.IsInRoll);
        Func<bool> OnKnockbackAndNotDeadOrKnockbackOrRoll() => () => _playerDamageAndAffectHandler.InitialKnockback && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback || _playerDamageAndAffectHandler.IsInRoll);
        Func<bool> OnFallAndNotDead() => () => _playerDamageAndAffectHandler.IsDamaged && _playerDamageAndAffectHandler.DamageType == DamageType.FALL && !(_playerDamageAndAffectHandler.IsDead);
        Func<bool> OnDeathAndMeleeDamageAndNotDeadOrKnockbackOrRoll() => () => _playerDamageAndAffectHandler.InitialDeath && _playerDamageAndAffectHandler.DamageType == DamageType.ENEMY && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback || _playerDamageAndAffectHandler.IsInRoll);
        Func<bool> OnDeathAndFallDamageAndNotDead() => () => _playerDamageAndAffectHandler.InitialDeath && _playerDamageAndAffectHandler.DamageType == DamageType.FALL && !(_playerDamageAndAffectHandler.IsDead);
        Func<bool> OnDeathAndSpikeDamageAndNotDeadOrKnockbackOrRoll() => () => _playerDamageAndAffectHandler.InitialDeath && _playerDamageAndAffectHandler.DamageType == DamageType.SPIKE && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback|| _playerDamageAndAffectHandler.IsInRoll);
        Func<bool> Reset() => () => _setAllStatesInitial;



        //AT is a shortcut for add transition. ATT is for Add Any Transition. Just renaming it basically. Bit eaiser to read.
        void PMAT(PlayerBaseState from, PlayerBaseState to, Func<bool> condition) => _playerStateMachine.AddTransition(from, to, condition);
        void PMAAT(PlayerBaseState to, Func<bool> condition) => _playerStateMachine.AddAnyTransition(to, condition);

        //set initial state.
        _playerStateMachine.SetState(onSpawnInitialIdleState);
        #endregion
    }
    #endregion



    #region Unity Update & FixedUpdate Callback Methods 
    private void Start()
    {
        _startStart = true;
    }


    private void Update()
    {
        //Run current state Logic from all StateMachines:
        _playerStateMachine.LogicUpdate();
    }

    private void FixedUpdate()
    {
        //Run current state Physics from all StateMacines:
        _playerStateMachine.PhysicsUpdate();
    }
    #endregion



    #region Public Functions
    public void CheckForMoveInputDirectionChangeThenFlip()
    {
        bool previousFacingight = _facingRight;
        if (_playerInputHandler.RawMoveInput > 0.05f)
        {
            _facingRight = true;
        }
        if (_playerInputHandler.RawMoveInput < -0.05f)
        {
            _facingRight = false;
        }
        if (previousFacingight != _facingRight)
        {
            // Rotate the player
            transform.Rotate(0f, 180f, 0f);
        }
    }
    public void Flip()
    {
        // change direction faced bool.
        _facingRight = !_facingRight;

        // Rotate the player
        transform.Rotate(0f, 180f, 0f);
    }

    public void InstantiateObject(GameObject gameObject, Vector3 position, Quaternion angle)
    {
        Instantiate(gameObject, position, angle);
    }

    public GameObject InstantiateObjectWithRef(GameObject gameObject, Vector3 position, Quaternion angle)
    {
        return Instantiate(gameObject, position, angle);
    }
    #endregion



    #region Private Functions
    private void WaitBetweenNextGravLift()
    {
        _playerDamageAndAffectHandler.GravLiftContact = false;
        _canGravLift = true;
    }

    private void WaitToSpawn()
    {
        _playerDamageAndAffectHandler.IsInSpawnWait = false;
    }

    private bool AnimatorIsPlaying(int animLayer)
    {
        return _animator.GetCurrentAnimatorStateInfo(animLayer).length >
               _animator.GetCurrentAnimatorStateInfo(animLayer).normalizedTime;
    }

    private bool AnimLooping(int animLayer)
    {
        return _animator.GetCurrentAnimatorStateInfo(animLayer).loop;
    }

    private bool ChangedInputDirection()
    {
        if (_previousInputDirection == 0f)
        {
            _previousInputDirection = Mathf.Sign(_playerInputHandler.RawMoveInput);
        }
        else if (_previousInputDirection != Mathf.Sign(_playerInputHandler.RawMoveInput))
        {
            return true;
        }
        else
        {
            _previousInputDirection = Mathf.Sign(_playerInputHandler.RawMoveInput);
        }
        return false;
    }
    #endregion
}
