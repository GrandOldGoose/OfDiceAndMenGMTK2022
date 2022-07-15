using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerStateMachineNamespace;

public class Player : MonoBehaviour
{
    #region Editor Fields
    [SerializeField] private PlayerData _playerData;                           // Scriptable object that stores all data for the player.
    [SerializeField] private Camera _cam;                                      // in world main camera
    [SerializeField] private Transform _arms;                                  // Player arm transform.
    [SerializeField] private Transform _head;                                  // Player head transform.
    [SerializeField] private Transform _fX;                                    // FX transform.
    [SerializeField] private LineRenderer _grappleFlyLine;
    #endregion



    #region Fields
    private StateMachine _playerMovementStateMachine, _playerAttackStateMachine, _playerEquipmentStateMachine,
        _playerDamageStateMachine, _playerAffectStateMachine;

    private PlayerAim _playerAim;
    private PlayerAimedEnemyInfo _playerAimedEnemyInfo;

    private PlayerCollisionHandler _playerCollisionHandler;
    private PlayerInputHandler _playerInputHandler;
    private PlayerWeaponAndEquipmentHandler _playerWeaponAndEquipmentHandler;
    private PlayerDamageAndAffectHandler _playerDamageAndAffectHandler;
    private PlayerUIHandler _playerUIHandler;

    private PlayerProjectileSpawner _playerProjectileSpawner;

    private Animator _animator;

    private bool _startStart = false;
    private bool _setAllStatesInitial = false;

    private bool _facingRight = true;
    private int _doubleJumpCount = 0;
    private int _rollCount = 0;
    private float _previousInputDirection = 0f;

    private bool _canLedgeGrab = true;
    private bool _canMonkeyBar = true;
    private bool _canGravLift = true;
    private bool _canTeleportal = true;
    private bool _canGrindRail = true;
    private bool _onGrindRail = false;
    private bool _canGrapple = true;

    private bool _canAttack = false;
    private bool _canUseEquipent = false;
    #endregion



    #region Properties
    public PlayerData PlayerData { get => _playerData; }
    public PlayerAim PlayerAim { get => _playerAim; }
    public bool StartStart { get => _startStart; set => _startStart = value; }
    public bool SetAllStatesInitial { get => _setAllStatesInitial; set => _setAllStatesInitial = value; }
    public bool FacingRight { get => _facingRight; set => _facingRight = value; }
    public int DoubleJumpCount { get => _doubleJumpCount; set => _doubleJumpCount = value; }
    public int RollCount { get => _rollCount; set => _rollCount = value; }
    public float PreviousInputDirection { set => _previousInputDirection = value; }
    public bool CanAttack { get => _canAttack; set => _canAttack = value; }
    public bool CanLedgeGrab { get => _canLedgeGrab; set => _canLedgeGrab = value; }
    public bool CanUseEquipent { set => _canUseEquipent = value; }
    public bool CanMonkeyBar { set => _canMonkeyBar = value; }
    public bool CanGravLift { set => _canGravLift = value; }
    public bool CanTeleportal { set => _canTeleportal = value; }
    public bool CanGrindRail { set => _canGrindRail = value; }
    public bool OnGrindRail { get => _onGrindRail; set => _onGrindRail = value; }
    public bool CanGrapple { get => _canGrapple; set => _canGrapple = value; }
    #endregion



    #region Unity Awake Callback Method (contains statemachine setups)
    private void Awake()
    {
        //Assign callers to events:
        Action StartRollRechargeWait() => () => Invoke("RollRechargeWait", _playerData.OnGroundRoll_RechargeTime);
        OnGroundRollState.OnGroundRollEnter += StartRollRechargeWait();
        Action StartWaitBetweenNextLedgeGrab() => () => Invoke("WaitBetweenNextLedgeGrab", _playerData.OnLedgeGrab_WaitBetweenNextGrab);
        OnLedgeGrabState.OnLedgeGrabExit += StartWaitBetweenNextLedgeGrab();
        Action StartWaitBetweenNextMonkeyBar() => () => Invoke("WaitBetweenNextMonkeyBar", _playerData.OnMovementObjectMonekyBar_WaitBetweenNextMonkeyBar);
        OnMovementObjectMonkeyBarState.OnMovementObjectMonkeyBarExit += StartWaitBetweenNextMonkeyBar();
        Action StartWaitBetweenNextGravLift() => () => Invoke("WaitBetweenNextGravLift", _playerData.OnMovementObjectGravLift_WaitBetweenNextGravLift);
        OnMovementObjectGravLiftState.OnMovementObjectGravLiftExit += StartWaitBetweenNextGravLift();
        Action StartWaitBetweenNextTeleportal() => () => Invoke("WaitBetweenNextTeleportal", _playerData.OnMovementObjectTeleportal_WaitBetweenNextTelepotal);
        OnMovementObjectTeleportalState.OnMovementObjectTeleportalExit += StartWaitBetweenNextTeleportal();
        Action StartWaitBetweenNextGrindRail() => () => Invoke("WaitBetweenNextGrindRail", _playerData.OnMovementGrindRailAbove_WaitBetweenNextGrindRail);
        OnMovementObjectGrindRailAboveState.OnMovementObjectGrindRailAboveExit += StartWaitBetweenNextGrindRail();
        OnMovementObjectGrindRailBelowState.OnMovementObjectGrindRailBelowExit += StartWaitBetweenNextGrindRail();

        Action SpawnWaitTime() => () => Invoke("WaitToSpawn", _playerData.DamageAndAffectHandler_SpawnWait);
        OnSpawnWaitState.OnSpawnWait += SpawnWaitTime();

        //Create instances of state machines:
        _playerMovementStateMachine = new PlayerStateMachine();
        _playerAttackStateMachine = new PlayerStateMachine();
        _playerEquipmentStateMachine = new PlayerStateMachine();
        _playerDamageStateMachine = new PlayerStateMachine();
        _playerAffectStateMachine = new PlayerStateMachine();

        //Create instances of required components:
        var rigidbody2D = GetComponent<Rigidbody2D>();
        var transform = GetComponent<Transform>();

        _animator = GetComponent<Animator>();

        _playerCollisionHandler = GetComponent<PlayerCollisionHandler>();
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _playerWeaponAndEquipmentHandler = GetComponent<PlayerWeaponAndEquipmentHandler>();
        _playerDamageAndAffectHandler = GetComponent<PlayerDamageAndAffectHandler>();
        _playerUIHandler = GetComponent<PlayerUIHandler>();

        _playerProjectileSpawner = GetComponent<PlayerProjectileSpawner>();

        _playerAim = new PlayerAim(this, _playerInputHandler, transform, _head, _arms, _playerProjectileSpawner.ShotSpawn,
            _playerProjectileSpawner.ShotSpawnStandingOrigin1, _playerProjectileSpawner.ShotSpawnStandingOrigin2,
            _playerProjectileSpawner.ShotSpawnSlideOrigin1, _playerProjectileSpawner.ShotSpawnSlideOrigin1);
        _playerAimedEnemyInfo = new PlayerAimedEnemyInfo(this, _playerData, _playerUIHandler, _playerInputHandler, _playerProjectileSpawner.ShotSpawn, _playerAim);


        //Assign Fields:
        _doubleJumpCount = _playerData.InAirJump_DoubleJumpCount;
        _rollCount = _playerData.OnGroundRoll_NumberBeforeRecharge;





        #region Player Movement State Macine
        //set all states you are using in the player Movement Hierachial State Macine:
        //SuperStates.
        var onGroundSuperState = new OnGroundSuperState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator);
        var inAirSuperState = new InAirSuperState(this, _playerData, _playerInputHandler, rigidbody2D, _animator);
        var onWallSuperState = new OnWallSuperState(this, _playerInputHandler, _animator);
        var onLedgeSuperState = new OnLedgeSuperState(this, _playerInputHandler, _animator);
        var inGroundPoundSuperState = new InGroundPoundSuperState(this, _playerInputHandler, _animator);
        var onMovementObjectSuperState = new OnMovementObjectSuperState(this, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator);
        var onGrappleFlySuperState = new OnGrappleFlySuperState(this, _playerInputHandler, _animator);
        var onDamageSuperState = new OnDamageSuperState(this, _playerInputHandler, _playerDamageAndAffectHandler, _animator);
        var onDeathSuperState = new OnDeathSuperState(this, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator);
        var onSpawnSuperState = new OnSpawnSuperState(this, _playerInputHandler, _playerDamageAndAffectHandler, _animator);
        var onIceSuperState = new OnIceSuperState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator);
        var inGooSuperState = new InGooSuperState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator);
        //OnGroundSubStates.
        var onGroundMoveState = new OnGroundMoveState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onGroundIdleState = new OnGroundIdleState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onGroundJumpState = new OnGroundJumpState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onGroundLandState = new OnGroundLandState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onGroundSlideState = new OnGroundSlideState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onGroundRollState = new OnGroundRollState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerDamageAndAffectHandler, _playerAim);
        var onGroundLedgeClimbState = new OnGroundLedgeClimbState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim);
        var onGroundThroughFloorState = new OnGroundThroughFloorState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onGroundGroundPoundLandState = new OnGroundGroundPoundLandState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerDamageAndAffectHandler, _playerAim);
        //InAirSubStates.
        var inAirMoveState = new InAirMoveState(this, _playerData, _playerInputHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var inAirJumpState = new InAirJumpState(this, _playerData, _playerInputHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        //OnWallSubStates.
        var onWallSlideState = new OnWallSlideState(this, _playerInputHandler, _animator, _playerData, rigidbody2D, _playerAim, _playerAimedEnemyInfo);
        var onWallJumpState = new OnWallJumpState(_playerInputHandler, _animator, this, _playerData, rigidbody2D, _playerAim, _playerAimedEnemyInfo);
        //OnLedgeSubStates.
        var onLedgeGrabState = new OnLedgeGrabState(_playerInputHandler, _animator, rigidbody2D, this, _playerData, _playerCollisionHandler, _playerDamageAndAffectHandler, _playerAim, _playerAimedEnemyInfo);
        //InGroundPoundSubStates.
        var inGroundPoundState = new InGroundPoundDiveState(this, _playerData, _playerInputHandler, rigidbody2D, _animator, _playerAim);
        //OnMovementObjectSubStates.
        var onMovementObjectMonkeyBarState = new OnMovementObjectMonkeyBarState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim);
        var onMovementObjectGravLiftState = new OnMovementObjectGravLiftState(this, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onMovementObjectTeleportalState = new OnMovementObjectTeleportalState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim);
        var onMovementObjectGrindRailAboveState = new OnMovementObjectGrindRailAboveState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onMovementObjectGrindRailBelowState = new OnMovementObjectGrindRailBelowState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        //OnGrappleFlyStates.
        var onGrappleFlyInitialFailureState = new OnGrappleFlyInitialFailureState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onGrappleFlyInitialSuccessState = new OnGrappleFlyInitialSuccessState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo, _grappleFlyLine);
        var onGrappleFlyAttachedState = new OnGrappleFlyAttachedState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo, _grappleFlyLine);
        var onGrappleFlyReleaseState = new OnGrappleFlyReleaseState(this, _playerData, _playerInputHandler, _playerCollisionHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo, _grappleFlyLine);
        //OnDamageSubStates.
        var onDamageKnockedbackState = new OnDamageKnockedbackState(this, _playerInputHandler, _playerDamageAndAffectHandler, _animator, rigidbody2D, _playerAim);
        var onDamageFallState = new OnDamageFallState(this, _playerInputHandler, _playerDamageAndAffectHandler, _animator, rigidbody2D, _playerCollisionHandler, _playerAim);
        //OnDeathSubStates.
        var onDeathMeleeHitState = new OnDeathMeleeHitState(this, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator, _playerAim);
        var onDeathRangedHitState = new OnDeathRangedHitState(this, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator, _playerAim);
        var onDeathFallState = new OnDeathFallState(this, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator, _playerAim);
        var onDeathSpikeState = new OnDeathSpikeState(this, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator, _playerAim);
        //OnSpawnSubStates.
        var onSpawnInitialIdleState = new OnSpawnInitialIdleState(this, _playerInputHandler, _playerDamageAndAffectHandler, _animator, rigidbody2D, _playerAim);
        var onSpawnWaitState = new OnSpawnWaitState(this, _playerInputHandler, _playerDamageAndAffectHandler, _animator, rigidbody2D, _playerAim);
        var onSpawnInitialState = new OnSpawnInitialState(this, _playerInputHandler, _playerDamageAndAffectHandler, _animator, rigidbody2D, _playerCollisionHandler, _playerAim);
        var onSpawnFallState = new OnSpawnFallState(this, _playerInputHandler, _playerDamageAndAffectHandler, _animator, rigidbody2D, _playerCollisionHandler, _playerAim);
        //OnIceSubStates.
        var onIceMoveState = new OnIceMoveState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var onIceJumpState = new OnIceJumpState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        //InGooSubStates.
        var inGooIdleState = new InGooIdleState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);
        var inGooMoveState = new InGooMoveState(this, _playerData, _playerInputHandler, _playerCollisionHandler, _playerDamageAndAffectHandler, rigidbody2D, _animator, _playerAim, _playerAimedEnemyInfo);

        //Set all transitions that directly connect to another state:
        //SuperState Transitions.
        PMAT(onGroundSuperState, inAirMoveState, InAirAndAnimIsCompletedOrAnimIsLooping());
        PMAT(inAirSuperState, onGroundLandState, HitGround());
        PMAT(inAirSuperState, onGroundLedgeClimbState, HitLedgeAndLedgeGrabWaitTime());
        PMAT(inAirSuperState, onWallSlideState, FrontContactAndMoveInputTowardsWallAndAnimIsCompletedOrAnimIsLooping());
        PMAT(inAirSuperState, inGroundPoundState, SlideInputAndNoGroundFromHighUpContact());
        PMAT(onWallSuperState, onGroundLandState, HitGround());
        PMAT(onWallSuperState, inAirMoveState, NoFrontContactOrNoMoveInputTowardsWallAndAnimisCompletedOrAnimIsLooping());
        //PMAT(onLedgeSuperState, inAirMoveState, SlideInput());
        //PMAT(onLedgeSuperState, onGroundLedgeClimbState, JumpInput());
        PMAT(onMovementObjectSuperState, inAirMoveState, AnimCompletedAndNotLoopingOrGrindRailEndContact());
        PMAT(onDamageSuperState, inAirMoveState, AnimCompleted());
        PMAT(onDeathSuperState, onSpawnWaitState, AnimCompleted());
        PMAT(onSpawnSuperState, inAirMoveState, AnimCompletedAndNotInWaitState());
        PMAT(onIceSuperState, inAirMoveState, NotOnIceAndAnimCompletedOrAnimLooping());
        PMAT(inGooSuperState, inAirMoveState, NotInGoo());
        PMAT(inGroundPoundSuperState, onGroundGroundPoundLandState, HitGround());

        //OnGround SubState Transitions.
        PMAT(onGroundIdleState, onGroundMoveState, MoveInput());
        PMAT(onGroundIdleState, onGroundJumpState, JumpInput());
        PMAT(onGroundIdleState, onGroundRollState, RollInputAndCanRoll());
        PMAT(onGroundIdleState, onGroundThroughFloorState, ThroughFloorInputAndOnSlimGround());
        PMAT(onGroundMoveState, onGroundJumpState, JumpInput());
        PMAT(onGroundMoveState, onGroundSlideState, SlideInputAndNoFrontContact());
        PMAT(onGroundMoveState, onGroundRollState, RollInputAndCanRoll());
        PMAT(onGroundMoveState, onGroundThroughFloorState, ThroughFloorInputAndOnSlimGround());
        PMAT(onGroundMoveState, onGroundIdleState, NoMoveInput());
        PMAT(onGroundLandState, onGroundJumpState, JumpInput());
        PMAT(onGroundLandState, onGroundSlideState, SlideInputAndNoFrontContact());
        PMAT(onGroundLandState, onGroundRollState, RollInputAndCanRoll());
        PMAT(onGroundLandState, onGroundThroughFloorState, ThroughFloorInputAndOnSlimGround());
        PMAT(onGroundLandState, onGroundIdleState, AnimCompletedAndNoMoveInput());
        PMAT(onGroundLandState, onGroundMoveState, AnimCompletedAndMoveInput());
        PMAT(onGroundSlideState, onGroundIdleState, NoTopContacAndFrontContactOrNoMoveInputorDirectionInputChanged());
        PMAT(onGroundSlideState, onGroundJumpState, JumpInput());
        PMAT(onGroundSlideState, onGroundThroughFloorState, ThroughFloorInputAndOnSlimGround());
        PMAT(onGroundSlideState, onGroundRollState, RollInputAndCanRoll());
        /*PMAT(onGroundSlideState, onLedgeGrabState, HitEdge());*/
        PMAT(onGroundJumpState, inAirMoveState, AnimCompleted());
        PMAT(onGroundThroughFloorState, inAirMoveState, AnimCompleted());
        PMAT(onGroundLedgeClimbState, onGroundIdleState, AnimCompleted());
        PMAT(onGroundGroundPoundLandState, onGroundIdleState, AnimCompletedAndNoMoveInput());
        PMAT(onGroundGroundPoundLandState, onGroundMoveState, AnimCompletedAndMoveInput());
        PMAT(onGroundRollState, onGroundSlideState, AnimCompletedAndTopContact());
        PMAT(onGroundRollState, onGroundJumpState, AnimCompletedAndJumpInput());
        PMAT(onGroundRollState, onGroundSlideState, AnimCompletedAndSlideInput());
        PMAT(onGroundRollState, onGroundThroughFloorState, AnimCompletedAndFallThroughFloorInput());
        PMAT(onGroundRollState, onGroundIdleState, AnimCompletedAndNoMoveInput());
        PMAT(onGroundRollState, onGroundMoveState, AnimCompletedAndMoveInput());


        //InAir SubState Transitions.
        PMAT(inAirMoveState, inAirJumpState, JumpInputAndCanDoubleJump());
        PMAT(inAirJumpState, inAirMoveState, AnimCompleted());
        //OnWall SubState Transitions.
        PMAT(onWallSlideState, onWallJumpState, JumpInput());
        //OnLedge SubState Transitions.
        //InGroundPound SubState Transitions.
        //OnMovementObject SubState Transitions.
        PMAT(onMovementObjectGrindRailAboveState, onMovementObjectGrindRailBelowState, SlideInput());
        PMAT(onMovementObjectGrindRailBelowState, onMovementObjectGrindRailAboveState, JumpInput());
        PMAT(onMovementObjectGrindRailAboveState, onGroundJumpState, JumpInput());
        PMAT(onMovementObjectGrindRailBelowState, inAirMoveState, SlideInput());
        //OnGrappleFly SubState Transitions.
        PMAT(onGrappleFlyInitialSuccessState, onGrappleFlyReleaseState, GrappleFlyInputReleasedOrNotInGrappleFly());
        PMAT(onGrappleFlyInitialSuccessState, onGrappleFlyAttachedState, AnimCompleted());
        PMAT(onGrappleFlyAttachedState, onGrappleFlyReleaseState, GrappleFlyInputReleasedOrNotInGrappleFly());
        PMAT(onGrappleFlyReleaseState, inAirMoveState, AnimCompleted());
        //PMAT(onGrappleFlyInitialFailureState, inAirMoveState, AnimCompleted());
        //OnDamage Substate Transitions.
        PMAT(onDamageFallState, onSpawnWaitState, AnimCompleted());
        //OnDeath Substate Transitions.
        //OnSpawn Substate Transitions.
        PMAT(onSpawnInitialIdleState, onSpawnInitialState, OnStart());
        PMAT(onSpawnWaitState, onSpawnInitialState, WaitTimeAndIsDead());
        PMAT(onSpawnWaitState, onSpawnFallState, WaitTimeAndFallDamage());
        //OnIce SubState Transitions.
        PMAT(onIceMoveState, onIceJumpState, JumpInput());
        PMAT(onIceJumpState, inAirMoveState, AnimCompleted());
        //InGoo SubState Transitions.
        PMAT(inGooIdleState, inGooMoveState, MoveInput());
        PMAT(inGooMoveState, inGooIdleState, NoMoveInput());

        //Set transitions that come from any transition:
        PMAAT(onMovementObjectMonkeyBarState, OnMonkeyBarContactAndCanMonkeyBarAndNotDeadOrKnockback());
        PMAAT(onMovementObjectGravLiftState, OnGravLiftContactAndCanGravLiftAndNotDeadOrKnockback());
        PMAAT(onMovementObjectTeleportalState, OnTeleportalContactAndCanTeleportalAndNotDeadOrKnockback());
        PMAAT(onMovementObjectGrindRailAboveState, OnGrindRailBelowContactAndCanGrindRailAndNoEndContactAndNotDeadOrKnockback());
        PMAAT(onMovementObjectGrindRailBelowState, OnGrindRailAboveContactAndCanGrindRailAndNoEndContactAndNotDeadOrKnockback());
        PMAAT(onGrappleFlyInitialSuccessState, InGrappleFlyAndCanGrappleAndNotDeadOrKnockback());
        //PMAAT(onGrappleFlyInitialFailureState, GrappleFlyInputPressedAndCannotGrapple());
        PMAAT(onDamageKnockedbackState, OnKnockbackAndNotDeadOrKnockback());
        PMAAT(onDamageFallState, OnFallAndNotDeadOrKnockback());
        PMAAT(onIceMoveState, OnIceAndNotDeadOrKnockback());
        PMAAT(inGooIdleState, InGooAndNoMoveInputAndNotDeadOrKnockback());
        PMAAT(inGooMoveState, InGooAndMoveInputAndNotDeadOrKnockback());
        PMAAT(onDeathMeleeHitState, OnDeathAndMeleeDamageAndNotDeadOrKnockback());
        PMAAT(onDeathRangedHitState, OnDeathAndRangedDamageAndNotDeadOrKnockback());
        PMAAT(onDeathFallState, OnDeathAndFallDamageAndNotDeadOrKnockback());
        PMAAT(onDeathSpikeState, OnDeathAndSpikeDamageAndNotDeadOrKnockback());
        PMAAT(onSpawnInitialIdleState, Reset());


        //Set all of our conditions:
        //Super State Conditions.
        Func<bool> InAirAndAnimIsCompletedOrAnimIsLooping() => () => !_playerCollisionHandler.DetectGroundContact() && !_playerCollisionHandler.DetectLateGroundContact() && (!AnimatorIsPlaying(0) || AnimLooping(0));
        Func<bool> HitGround() => () => _playerCollisionHandler.DetectGroundContact();
        Func<bool> HitLedgeAndLedgeGrabWaitTime() => () => _playerCollisionHandler.DetectFrontContact() && !_playerCollisionHandler.DetectHighFrontContact() && _canLedgeGrab;
        Func<bool> FrontContactAndMoveInputTowardsWallAndAnimIsCompletedOrAnimIsLooping() => () => _playerCollisionHandler.DetectFrontContact() && MoveInputTowardsWall() && (!AnimatorIsPlaying(0) || AnimLooping(0));
        Func<bool> NoFrontContactOrNoMoveInputTowardsWallAndAnimisCompletedOrAnimIsLooping() => () => (!_playerCollisionHandler.DetectFrontContact() || !MoveInputTowardsWall()) && (!AnimatorIsPlaying(0) || AnimLooping(0));
        Func<bool> AnimCompletedAndNotInWaitState() => () => !AnimatorIsPlaying(0) && !_playerDamageAndAffectHandler.IsInSpawnWait;
        Func<bool> AnimCompletedAndNotLoopingOrGrindRailEndContact() => () => (!AnimatorIsPlaying(0) && !AnimLooping(0)) || _playerCollisionHandler.GrindEndContact;
        Func<bool> NotOnIceAndAnimCompletedOrAnimLooping() => () => _playerDamageAndAffectHandler.HasExitedIce && (!AnimatorIsPlaying(0) || AnimLooping(0));
        Func<bool> NotInGoo() => () => _playerDamageAndAffectHandler.HasExitedGoo;
        Func<bool> SlideInputAndNoGroundFromHighUpContact() => () => _playerInputHandler.SlideInput && !_playerCollisionHandler.DetectGroundFromHighUpContact();
        //OnGround SubState Conditions.
        Func<bool> MoveInput() => () => Mathf.Abs(_playerInputHandler.RawMoveInput) >= 0.2f;
        Func<bool> NoMoveInput() => () => Mathf.Abs(_playerInputHandler.RawMoveInput) < 0.2f;
        Func<bool> JumpInput() => () => _playerInputHandler.JumpInput;
        Func<bool> SlideInputAndNoFrontContact() => () => _playerInputHandler.SlideInput && !_playerCollisionHandler.DetectFrontContact();
        Func<bool> NoTopContacAndFrontContactOrNoMoveInputorDirectionInputChanged() => () => !_playerCollisionHandler.DetectTopContact() && (_playerCollisionHandler.DetectFrontContact() || Mathf.Abs(_playerInputHandler.RawMoveInput) < 0.2f || ChangedInputDirection());
        Func<bool> RollInputAndCanRoll() => () => _playerInputHandler.RollInput && _rollCount > 0;
        Func<bool> AnimCompletedAndTopContact() => () => !AnimatorIsPlaying(0) && _playerCollisionHandler.DetectTopContact();
        Func<bool> AnimCompletedAndNoMoveInput() => () => !AnimatorIsPlaying(0) && Mathf.Abs(_playerInputHandler.RawMoveInput) < 0.2f;
        Func<bool> AnimCompletedAndMoveInput() => () => !AnimatorIsPlaying(0) && Mathf.Abs(_playerInputHandler.RawMoveInput) >= 0.2f;
        Func<bool> AnimCompletedAndJumpInput() => () => !AnimatorIsPlaying(0) && _playerInputHandler.JumpInput;
        Func<bool> AnimCompletedAndSlideInput() => () => !AnimatorIsPlaying(0) && _playerInputHandler.SlideInput;
        Func<bool> AnimCompletedAndFallThroughFloorInput() => () => !AnimatorIsPlaying(0) && _playerInputHandler.FallThroughFloorInput;
        Func<bool> ThroughFloorInputAndOnSlimGround() => () => _playerInputHandler.FallThroughFloorInput && _playerCollisionHandler.DetectSlimGroundContact();
        //InAir SubState Conditions.
        Func<bool> JumpInputAndCanDoubleJump() => () => _playerInputHandler.JumpInput && _doubleJumpCount > 0;
        Func<bool> AnimCompleted() => () => !AnimatorIsPlaying(0);
        //OnWall SubState Conditions
        Func<bool> SlideInput() => () => _playerInputHandler.SlideInput;
        //OnLedge SubState Conditions.
        //InGroundPound SubState Conditions.
        //OnMovementObject SubState Conditions.
        //OnGrappleFly SubState Conditions.
        Func<bool> GrappleFlyInputReleasedOrNotInGrappleFly() => () => _playerInputHandler.GrappleFlyInputRelease || !_playerAimedEnemyInfo.InGrappleFly;
        //OnDamage Substate Conditions.
        //OnDeath Substate Conditions.
        //OnSpawn Substate Conditions.
        Func<bool> OnStart() => () => _startStart;
        Func<bool> WaitTimeAndIsDead() => () => !_playerDamageAndAffectHandler.IsInSpawnWait && _playerDamageAndAffectHandler.DamageType == DamageType.INITIAL;
        Func<bool> WaitTimeAndFallDamage() => () => !_playerDamageAndAffectHandler.IsInSpawnWait && _playerDamageAndAffectHandler.DamageType == DamageType.FALL;
        //OnIce SubState Conditions.
        //InGoo SubState Conditions.


        //AnyTransition Conditions.
        Func<bool> OnMonkeyBarContactAndCanMonkeyBarAndNotDeadOrKnockback() => () => _playerCollisionHandler.MonkeyBarContact && _canMonkeyBar && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnGravLiftContactAndCanGravLiftAndNotDeadOrKnockback() => () => _playerCollisionHandler.GravLiftContact && _canGravLift && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnTeleportalContactAndCanTeleportalAndNotDeadOrKnockback() => () => _playerCollisionHandler.TeleportalContact && _canTeleportal && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnGrindRailBelowContactAndCanGrindRailAndNoEndContactAndNotDeadOrKnockback() => () => _playerCollisionHandler.GrindRailBelowContact && _canGrindRail && !_playerCollisionHandler.GrindEndContact && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnGrindRailAboveContactAndCanGrindRailAndNoEndContactAndNotDeadOrKnockback() => () => _playerCollisionHandler.GrindRailAboveContact && _canGrindRail && !_playerCollisionHandler.GrindEndContact && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> InGrappleFlyAndCanGrappleAndNotDeadOrKnockback() => () => _playerAimedEnemyInfo.InGrappleFly && _canGrapple && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        //Func<bool> GrappleFlyInputPressedAndCannotGrapple() => () => _playerInputHandler.GrappleFlyInputPressed && !_playerAimedEnemyInfo.CanGrappleFly;
        Func<bool> OnKnockbackAndNotDeadOrKnockback() => () => _playerDamageAndAffectHandler.InitialKnockback && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnFallAndNotDeadOrKnockback() => () => _playerDamageAndAffectHandler.IsDamaged && _playerDamageAndAffectHandler.DamageType == DamageType.FALL && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnIceAndNotDeadOrKnockback() => () => _playerDamageAndAffectHandler.HasEnteredIce && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> InGooAndNoMoveInputAndNotDeadOrKnockback() => () => _playerDamageAndAffectHandler.HasEnteredGoo && Mathf.Abs(_playerInputHandler.RawMoveInput) < 0.2f && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> InGooAndMoveInputAndNotDeadOrKnockback() => () => _playerDamageAndAffectHandler.HasEnteredGoo && Mathf.Abs(_playerInputHandler.RawMoveInput) >= 0.2f && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnDeathAndMeleeDamageAndNotDeadOrKnockback() => () => _playerDamageAndAffectHandler.InitialDeath && _playerDamageAndAffectHandler.DamageType == DamageType.ENEMYMELEE && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnDeathAndRangedDamageAndNotDeadOrKnockback() => () => _playerDamageAndAffectHandler.InitialDeath && _playerDamageAndAffectHandler.DamageType == DamageType.ENEMYRANGED && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnDeathAndFallDamageAndNotDeadOrKnockback() => () => _playerDamageAndAffectHandler.InitialDeath && _playerDamageAndAffectHandler.DamageType == DamageType.FALL && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> OnDeathAndSpikeDamageAndNotDeadOrKnockback() => () => _playerDamageAndAffectHandler.InitialDeath && _playerDamageAndAffectHandler.DamageType == DamageType.SPIKE && !(_playerDamageAndAffectHandler.IsDead || _playerDamageAndAffectHandler.IsKnockedback);
        Func<bool> Reset() => () => _setAllStatesInitial;



        //AT is a shortcut for add transition. ATT is for Add Any Transition. Just renaming it basically. Bit eaiser to read.
        void PMAT(PlayerBaseState from, PlayerBaseState to, Func<bool> condition) => _playerMovementStateMachine.AddTransition(from, to, condition);
        void PMAAT(PlayerBaseState to, Func<bool> condition) => _playerMovementStateMachine.AddAnyTransition(to, condition);

        //set initial state.
        _playerMovementStateMachine.SetState(onSpawnInitialIdleState);
        #endregion



        #region Player Attack State Machine
        //set all states you are using in the player Attack Finite State Machine:
        var attackIdleState = new AttackIdleState(this, _playerInputHandler);

        var attackSwitchPrimaryWeaponState = new AttackSwitchPrimaryWeaponState(_playerData, _playerWeaponAndEquipmentHandler, _animator, _playerInputHandler);

        var attackPrePrimaryFireState = new AttackPrePrimaryFireState(_playerData, _playerWeaponAndEquipmentHandler, _animator);
        var attackMidPrimaryFireState = new AttackMidPrimaryFireState(_playerWeaponAndEquipmentHandler, _animator);
        var attackPostPrimaryFireState = new AttackPostPrimaryFireState(_playerData, _playerWeaponAndEquipmentHandler, _playerInputHandler, _animator);

        var attackPreSecondaryFireState = new AttackPreSecondaryFireState(_playerData, _playerWeaponAndEquipmentHandler, _animator);
        var attackMidSecondaryFireState = new AttackMidSecondaryFireState(_playerWeaponAndEquipmentHandler, _animator);
        var attackPostSecondaryFireState = new AttackPostSecondaryFireState(_playerData, _playerWeaponAndEquipmentHandler, _playerInputHandler, _animator);

        var attackPreMeleeState = new AttackPreMeleeState(_playerData, _playerWeaponAndEquipmentHandler, _animator);
        var attackMidMeleeState = new AttackMidMeleeState(_playerData, _playerWeaponAndEquipmentHandler, _animator);
        var attackPostMeleeState = new AttackPostMeleeState(_playerData, _playerWeaponAndEquipmentHandler, _playerInputHandler, _animator);

        //Set all transitions that directly connect to another state:
        PAAT(attackIdleState, attackPrePrimaryFireState, CanAttackAndCurrentWeaponIdlePreCon());
        PAAT(attackIdleState, attackPreSecondaryFireState, CanAttackAndSecondaryFireInput());
        PAAT(attackIdleState, attackPreMeleeState, CanAttackAndMeleeFireInput());
        PAAT(attackIdleState, attackSwitchPrimaryWeaponState, SwitchWeaponInputAndCanAttack());

        PAAT(attackSwitchPrimaryWeaponState, attackIdleState, AttackAnimCompleted());

        PAAT(attackPrePrimaryFireState, attackMidPrimaryFireState, AnimCompletedAndCurrentWeaponPreMidCon());
        PAAT(attackMidPrimaryFireState, attackPostPrimaryFireState, AnimCompletedAndCurrentWeaponMidPostCon());
        PAAT(attackPostPrimaryFireState, attackIdleState, AnimCompletedAndCurrentWeaponPostIdleCon());

        PAAT(attackPreSecondaryFireState, attackMidSecondaryFireState, AnimCompletedOrSecondaryFireInputChange());
        PAAT(attackMidSecondaryFireState, attackPostSecondaryFireState, AnimCompletedOrSecondaryFireInputChange());
        PAAT(attackPostSecondaryFireState, attackIdleState, AnimCompletedOrSecondaryFireInputChange());

        PAAT(attackPreMeleeState, attackMidMeleeState, AnimCompletedOrMeleeInputChange());
        PAAT(attackMidMeleeState, attackPostMeleeState, AnimCompletedOrMeleeInputChange());
        PAAT(attackPostMeleeState, attackIdleState, AnimCompletedOrMeleeInputChange());


        //Set transitions that come from any transition:
        PAAAT(attackIdleState, CannotAttack());
        PAAAT(attackIdleState, Reset());

        //Set all of our conditions:
        Func<bool> CanAttackAndCurrentWeaponIdlePreCon() => () => _canAttack && _playerWeaponAndEquipmentHandler.CurrentWeapon.IdleToPreCon;
        Func<bool> AnimCompletedAndCurrentWeaponPreMidCon() => () => _animator.GetCurrentAnimatorStateInfo(1).IsName("NoAnim") && _playerWeaponAndEquipmentHandler.CurrentWeapon.PreToMidCon;
        Func<bool> AnimCompletedAndCurrentWeaponMidPostCon() => () => _animator.GetCurrentAnimatorStateInfo(1).IsName("NoAnim") && _playerWeaponAndEquipmentHandler.CurrentWeapon.MidToPostCon;
        Func<bool> AnimCompletedAndCurrentWeaponPostIdleCon() => () => _animator.GetCurrentAnimatorStateInfo(1).IsName("NoAnim") && _playerWeaponAndEquipmentHandler.CurrentWeapon.PostToIdleCon;
        Func<bool> SwitchWeaponInputAndCanAttack() => () => _playerInputHandler.SwapWeaponInput && _canAttack;

        Func<bool> AttackAnimCompleted() => () => _animator.GetCurrentAnimatorStateInfo(1).IsName("NoAnim");

        Func<bool> CanAttackAndSecondaryFireInput() => () => _canAttack && _playerInputHandler.SecondaryFireInput;
        Func<bool> CanAttackAndMeleeFireInput() => () => _canAttack && _playerInputHandler.MeleeInput;

        Func<bool> AnimCompletedOrSecondaryFireInputChange() => () => _animator.GetCurrentAnimatorStateInfo(1).IsName("NoAnim") /* || sFireInputChange */;
        Func<bool> AnimCompletedOrMeleeInputChange() => () => _animator.GetCurrentAnimatorStateInfo(1).IsName("NoAnim") /* || meleeInputChange */;

        Func<bool> CannotAttack() => () => !_canAttack;


        //AT is a shortcut for add transition. ATT is for Add Any Transition. Just renaming it basically. Bit eaiser to read.
        void PAAT(PlayerBaseState from, PlayerBaseState to, Func<bool> condition) => _playerAttackStateMachine.AddTransition(from, to, condition);
        void PAAAT(PlayerBaseState to, Func<bool> condition) => _playerAttackStateMachine.AddAnyTransition(to, condition);

        //set initial state.
        _playerAttackStateMachine.SetState(attackIdleState);
        #endregion



        #region Player Equipent State Machine
        //set all states you are using in the player Equipment Finite State Machine:
        var equipmentIdleState = new EquipmentIdleState();

        var equipmentPreUseState = new EquipmentPreUseState(_playerData, _playerWeaponAndEquipmentHandler, _animator);
        var equipmentMidUseState = new EquipmentMidUseState(_playerData, _playerWeaponAndEquipmentHandler, rigidbody2D, _animator);
        var equipmentPostUseState = new EquipmentPostUseState(_playerData, _playerWeaponAndEquipmentHandler, _playerInputHandler, _animator);

        //Set all transitions that directly connect to another state:
        PEAT(equipmentIdleState, equipmentPreUseState, CanUseEquipmentAndEquipmentInput());

        PEAT(equipmentPreUseState, equipmentMidUseState, AnimCompletedOrEquipmentInputChange());
        PEAT(equipmentMidUseState, equipmentPostUseState, AnimCompletedOrEquipmentInputChange());
        PEAT(equipmentPostUseState, equipmentIdleState, AnimCompletedOrEquipmentInputChange());


        //Set transitions that come from any transition:
        PEAAT(equipmentIdleState, CannotUseEquipment());
        PEAAT(equipmentIdleState, Reset());

        //Set all of our conditions:
        Func<bool> CanUseEquipmentAndEquipmentInput() => () => _canUseEquipent && _playerInputHandler.EquipmentFireInput;
        Func<bool> AnimCompletedOrEquipmentInputChange() => () => _animator.GetCurrentAnimatorStateInfo(2).IsName("NoAnim") /* || equipmentInputChange */;

        Func<bool> CannotUseEquipment() => () => _canUseEquipent;

        //AT is a shortcut for add transition. ATT is for Add Any Transition. Just renaming it basically. Bit eaiser to read.
        void PEAT(PlayerBaseState from, PlayerBaseState to, Func<bool> condition) => _playerEquipmentStateMachine.AddTransition(from, to, condition);
        void PEAAT(PlayerBaseState to, Func<bool> condition) => _playerEquipmentStateMachine.AddAnyTransition(to, condition);

        //set initial state.
        _playerEquipmentStateMachine.SetState(equipmentIdleState);
        #endregion



        #region Player Damage State Machine
        //set all states you are using in the player Equipment Finite State Machine:
        var damageIdleState = new DamageIdleState(this);

        var damageDamagedState = new DamageDamagedState(this, _playerDamageAndAffectHandler, _animator);

        //Set all transitions that directly connect to another state:
        PDAT(damageIdleState, damageDamagedState, IsDamaged());
        PDAT(damageDamagedState, damageIdleState, DamageAnimCompleted());


        //Set transitions that come from any transition:
        PDAAT(damageIdleState, Reset());

        //Set all of our conditions:
        Func<bool> IsDamaged() => () => _playerDamageAndAffectHandler.IsDamaged;
        Func<bool> DamageAnimCompleted() => () => _animator.GetCurrentAnimatorStateInfo(3).IsName("NoAnim");

        //AT is a shortcut for add transition. ATT is for Add Any Transition. Just renaming it basically. Bit eaiser to read.
        void PDAT(PlayerBaseState from, PlayerBaseState to, Func<bool> condition) => _playerDamageStateMachine.AddTransition(from, to, condition);
        void PDAAT(PlayerBaseState to, Func<bool> condition) => _playerDamageStateMachine.AddAnyTransition(to, condition);

        //set initial state.
        _playerDamageStateMachine.SetState(damageIdleState);
        #endregion



        #region Player Affect State Machine
        //set all states you are using in the player Equipment Finite State Machine:
        var affectIdleState = new AffectIdleState();

        var affectInGooState = new AffectInGooState(_animator);

        //Set all transitions that directly connect to another state:
        PAfAT(affectIdleState, affectInGooState, InGoo());
        PAfAT(affectInGooState, affectIdleState, AffectAnimCompleted());



        //Set transitions that come from any transition:
        PAfAAT(affectIdleState, Reset());

        //Set all of our conditions:
        Func<bool> InGoo() => () => false;
        Func<bool> AffectAnimCompleted() => () => _animator.GetCurrentAnimatorStateInfo(4).IsName("NoAnim") /* || equipmentInputChange */;

        //AT is a shortcut for add transition. ATT is for Add Any Transition. Just renaming it basically. Bit eaiser to read.
        void PAfAT(PlayerBaseState from, PlayerBaseState to, Func<bool> condition) => _playerAffectStateMachine.AddTransition(from, to, condition);
        void PAfAAT(PlayerBaseState to, Func<bool> condition) => _playerAffectStateMachine.AddAnyTransition(to, condition);

        //set initial state.
        _playerAffectStateMachine.SetState(affectIdleState);
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
        _playerMovementStateMachine.LogicUpdate();
        _playerAttackStateMachine.LogicUpdate();
        _playerEquipmentStateMachine.LogicUpdate();
        _playerDamageStateMachine.LogicUpdate();
        _playerAffectStateMachine.LogicUpdate();
    }

    private void FixedUpdate()
    {
        //Run current state Physics from all StateMacines:
        _playerMovementStateMachine.PhysicsUpdate();
        _playerAttackStateMachine.PhysicsUpdate();
        _playerEquipmentStateMachine.PhysicsUpdate();
        _playerDamageStateMachine.PhysicsUpdate();
        _playerAffectStateMachine.PhysicsUpdate();
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

            // Flip player arms and head.
            Vector3 theScale = _arms.localScale;
            theScale.x *= -1;
            _arms.localScale = theScale;
            _head.localScale = theScale;
        }
    }
    public void Flip()
    {
        // change direction faced bool.
        _facingRight = !_facingRight;

        // Rotate the player
        transform.Rotate(0f, 180f, 0f);

        // Flip player arms and head.
        Vector3 theScale = _arms.localScale;
        theScale.x *= -1;
        _arms.localScale = theScale;
        _head.localScale = theScale;
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
    private void RollRechargeWait()
    {
        _playerInputHandler.RollInput = false;
        _rollCount = _playerData.OnGroundRoll_NumberBeforeRecharge;
    }

    private void WaitBetweenNextLedgeGrab()
    {
        _canLedgeGrab = true;
    }

    private void WaitBetweenNextMonkeyBar()
    {
        _playerCollisionHandler.MonkeyBarContact = false;
        _canMonkeyBar = true;
    }

    private void WaitBetweenNextGravLift()
    {
        _playerCollisionHandler.GravLiftContact = false;
        _canGravLift = true;
    }

    private void WaitBetweenNextTeleportal()
    {
        _playerCollisionHandler.TeleportalContact = false;
        _canTeleportal = true;
    }

    private void WaitBetweenNextGrindRail()
    {
        _playerCollisionHandler.GrindRailAboveContact = false;
        _playerCollisionHandler.GrindRailBelowContact = false;
        _playerCollisionHandler.GrindEndContact = false;
        _canGrindRail = true;
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

    private bool MoveInputTowardsWall()
    {
        if (_facingRight && _playerInputHandler.RawMoveInput > 0.2f)
        {
            return true;
        }
        else if (!_facingRight && _playerInputHandler.RawMoveInput < -0.2f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}
