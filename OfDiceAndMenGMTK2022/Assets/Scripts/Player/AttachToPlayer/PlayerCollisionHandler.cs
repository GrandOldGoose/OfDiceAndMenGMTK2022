using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    #region Fields
    private PlayerData _playerData;
    private CapsuleCollider2D _collisionCollider;

    private LayerMask _ground, _slimGround, _movingGround;
    private RaycastHit2D _slimGroundHit;
    private Collider2D _slimFloorCollider = null;

    private bool _gravLiftContact = false;
    private Vector2 _gravLiftForceApplied = Vector2.zero;
    #endregion



    #region Properties
    public bool GravLiftContact { get => _gravLiftContact; set => _gravLiftContact = value; }
    public Vector2 GravLiftForceApplied { get => _gravLiftForceApplied; }
    public CapsuleCollider2D CollisionCollider { get => _collisionCollider; }
    #endregion



    #region Unity Callback Methods
    private void Awake()
    {
        _playerData = GetComponent<Player>().PlayerData;
        _collisionCollider = GetComponent<CapsuleCollider2D>();


        _ground = LayerMask.GetMask("Ground");
        _slimGround = LayerMask.GetMask("SlimGround");
        _movingGround = LayerMask.GetMask("MovingGround");
    }
    #endregion



    #region Public Methods
    //detectors:
    public RaycastHit2D DetectGroundContact()
    {
        return RayCastForContactWithMask(new Vector2(0.0f, -4f), -transform.up, _playerData.CollisionHandler_GroundCheckDistance, _ground | _slimGround | _movingGround);
    }
    public RaycastHit2D DetectLateGroundContact()
    {
        return RayCastForContactWithMask(new Vector2((_playerData.CollisionHandler_LateGroundCheckDistance * -transform.right.x), 0f), -transform.up, _playerData.CollisionHandler_GroundCheckDistance, _ground | _movingGround);
    }
    public RaycastHit2D DetectFrontContact()
    {
        return RayCastForContactWithMask(new Vector2(0.0f, -2.0f), transform.right, _playerData.CollisionHandler_FrontCheckDistance, _ground | _movingGround);
    }
    public bool DetectTopContact()
    {
        return RayCastForContactWithMask(new Vector2(0.0f, -3.5f), transform.up, _playerData.CollisionHandler_TopCheckDistance, _ground | _movingGround);
    }
    public bool DetectSlimGroundContact()
    {
        _slimGroundHit = RayCastForContactWithMask(Vector2.zero, -transform.up, _playerData.CollisionHandler_GroundCheckDistance, _slimGround);
        return _slimGroundHit;
    }

    public void DisablePlayerCollisionWithSlimFloor()
    {
        if (_slimGroundHit)
        {
            if (_slimGroundHit.collider.usedByEffector)
            {
                _slimFloorCollider = _slimGroundHit.collider;
                Physics2D.IgnoreCollision(_collisionCollider, _slimFloorCollider, true);
            }
        }
    }

    public void EnablePlayerCollisionWithSlimFloor()
    {
        if (_slimFloorCollider != null)
        {
            Physics2D.IgnoreCollision(_collisionCollider, _slimFloorCollider, false);
            _slimFloorCollider = null;
        }
    }
    #endregion



    #region Private Methods
    private RaycastHit2D RayCastForContactWithMask(Vector2 originAdjustment, Vector2 direction, float distance, LayerMask layerMask)
    {
        RaycastHit2D[] hits = new RaycastHit2D[6];
        Physics2D.RaycastNonAlloc(new Vector2(transform.position.x + originAdjustment.x, transform.position.y + originAdjustment.y)
            , direction, hits, distance, layerMask);
        foreach (var hit in hits)
        {
            if (hit) return hit;
        }
        return hits[0];
    }
    #endregion
}
