using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
	[Header("Physics")]
	[Space]
	public PhysicsMaterial2D Physics_NoFriction;                                 // no friction physics material.
	public PhysicsMaterial2D Physics_FullFriction;                               // full friction physics material.


	[Header("CollisionHandler")]
	[Space]
	public float CollisionHandler_GroundCheckDistance = 3f;
	public float CollisionHandler_LateGroundCheckDistance = 3f;
	public float CollisionHandler_TopCheckDistance = 3f;
	public float CollisionHandler_FrontCheckDistance = 3f;


	[Header("DamageAndAffectHandler")]
	[Space]
	public int DamageAndAffectHandler_MaxHealth = 100;
	public int DamageAndAffectHandler_FallDamage = 10;
	public float DamageAndAffectHandler_SpawnWait = 1f;

	[Header("EnemyDetectionHandler")]
	[Space]
	public float EnemyDetectionHandler_Radius = 100f;
	public float EnemyDetectionHandler_CheckRate = 0.5f;


	[Header("OnGroundMoveState")]
	[Space]
	public float OnGroundMove_MoveSpeed = 80f;
	[Range(0, .3f)] public float OnGroundMove_MovementSmoothing = .05f;

	[Header("OnGroundJumpState")]
	[Space]
	public float OnGroundJump_JumpForce = 1000f;
	public float OnGroundJump_JumpWaitTime = 1f;
	public float OnGroundJump_MovingPlatformScaleCoeffient = 1f;
	public GameObject OnGroundJump_JumpFXAnim;

	[Header("OnGroundAttackState")]
	[Space]
	public Vector2 OnGroundAttack_KnockbackForce = new Vector2(500f, 500f);

	[Header("OnGroundThroughFloorState")]
	[Space]
	public float OnGroundThoughFloor_ForceDownThroughFloor = 100f;

	[Header("InAirMoveState")]
	[Space]
	public float InAirMove_TerminalVelocity = 100f;
	public float InAirMove_MoveSpeed = 80f;
	[Range(0, .3f)] public float InAirMove_MovementSmoothing = .05f;

	[Header("InAirJumpState")]
	[Space]
	public float InAirJump_DoubleJumpForce = 1000f;
	public float InAirJump_DoubleJumpWaitTime = 1f;
	public int InAirJump_DoubleJumpCount = 2; 
	public GameObject InAirJump_DoubleJumpFXAnim;

	[Header("InAirOnGravLiftState")]
	[Space]
	public float InAirOnGravLiftState_WaitBetweenNextGravLift = 1f;

	[Header("OnRollRollState")]
	[Space]
	public Vector2 OnRollRollState_RNGForceApplied = new Vector2(1000f, 1000f);
	public Vector2 OnRollRollState_RNGRollTime = new Vector2(0.5f, 5f);

	[Header("PlayerProjectile")]
	[Space]
	//public PlayerProjectile PlayerProjectile_ProjectileGameObject;
	public Vector2 PlayerProjectile_ShotSpawn;
	public float PlayerProjectile_ProjectileSpeed = 60f;
	public int PlayerProjectile_ProjectileDamage = 5;
	public float PlayerProjectile_ProjectileLifetime = 3f;
	public float PlayerProjectile_FireRate = 3f;
}
