using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageAndAffectHandler : MonoBehaviour, IRespawnable, IDamageable, IKnockbackable, IHittable, IGravLiftable, IHealthPackable
{
    #region Editor Fields
    [SerializeField] private CapsuleCollider2D _hitBox;
    #endregion



    #region Fields
    private PlayerData _playerData;
    private PlayerUIHandler _playerUIHandler;

    private int _currentHealth;
    private DamageType _damageType;

    private Transform _initialSpawnPoint;
    private Transform _respawnPoint;

    private bool _initialDeath = false;
    private bool _isDead = false;
    private bool _isRespawnning = false;

    private bool _isInSpawnWait = false;
    private bool _canBeDamaged = true;
    private bool _isDamaged = false;

    private bool _gravLiftContact = false;
    private Vector2 _gravLiftForceApplied = Vector2.zero;

    private bool _initialRoll = false;
    private bool _isInRoll = false;
    private bool _intialKnockback = false;
    private bool _isKnockedback = false;
    private Vector2 _knockbackForce = Vector2.zero;
    #endregion



    #region Properties
    public CapsuleCollider2D HitBox { get => _hitBox;}

    public bool InitialDeath { get => _initialDeath; set => _initialDeath = value; }
    public bool IsDead { get => _isDead; set => _isDead = value; }
    public DamageType DamageType { get => _damageType; }

    public bool IsRespawnning { get => _isRespawnning; set => _isRespawnning = value; }
    public Transform RespawnPoint { get => _respawnPoint; set => _respawnPoint = value; }
    public bool IsInSpawnWait { get => _isInSpawnWait; set => _isInSpawnWait = value; }

    public bool CanBeDamaged { set => _canBeDamaged = value; }
    public bool IsDamaged { get => _isDamaged; set => _isDamaged = value; }

    public bool GravLiftContact { get => _gravLiftContact; set => _gravLiftContact = value; }
    public Vector2 GravLiftForceApplied { get => _gravLiftForceApplied; }

    public bool InitialRoll { get => _initialRoll; set => _initialRoll = value; }
    public bool IsInRoll { get => _isInRoll; set => _isInRoll = value; }
    public bool InitialKnockback { get => _intialKnockback; set => _intialKnockback = value; }
    public bool IsKnockedback { get => _isKnockedback; set => _isKnockedback = value; }
    public Vector2 KnockbackForce { get => _knockbackForce; }
    #endregion



    #region Unity Callback Methods
    private void Awake()
    {
        Setup();
    }
    #endregion



    #region Public Interface Methods

    public void Setup()
    {
        _playerData = GetComponent<Player>().PlayerData;
        _playerUIHandler = GetComponent<PlayerUIHandler>();
        _damageType = DamageType.INITIAL;
        _currentHealth = _playerData.DamageAndAffectHandler_MaxHealth;
    }


    public void Respawn(Transform respawnPoint)
    {
        _isRespawnning = true;

        _respawnPoint = respawnPoint;
    }

    public void TakeDamage(int damageAmount, DamageType damageType)
    {
        if (_canBeDamaged)
        {
            //Set damage type.
            switch (damageType)
            {
                case DamageType.ENEMY:
                    _damageType = DamageType.ENEMY;
                    break;
                case DamageType.FALL:
                    _damageType = DamageType.FALL;
                    break;
                case DamageType.SPIKE:
                    _damageType = DamageType.SPIKE;
                    break;
                default:
                    break;
            }

            //triggers DamageDamagedState.
            _isDamaged = true;

            //Take Damage.
            /*
            _currentHealth -= damageAmount;
            _playerUIHandler.SetUIHealth(_currentHealth);
            */

            //Death Stuff
            /*
            if (_currentHealth <= 0)
            {
                //hes dead Jim.
                _initialDeath = true;

                this.gameObject.SetActive(false);
            }
            */
        }
    }

    public void ApplyKnockbackForce(Vector2 force)
    {
        _knockbackForce = force;
        _intialKnockback = true;
    }

    public void ApplyGravForce(Vector2 gravForce)
    {
        _gravLiftContact = true;
        _gravLiftForceApplied = gravForce;
    }

    public void AddHealth(int amount)
    {
        _currentHealth += amount;
        if (_currentHealth > _playerData.DamageAndAffectHandler_MaxHealth) { _currentHealth = _playerData.DamageAndAffectHandler_MaxHealth; }
        _playerUIHandler.SetUIHealth(_currentHealth);
    }


    #endregion
}

