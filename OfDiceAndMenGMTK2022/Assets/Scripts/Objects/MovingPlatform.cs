using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IHittable
{
    #region Editor Fields
    [SerializeField] private Transform[] _movePoints;
    [SerializeField] private Transform[] _ledgeCornerPositions;
    [SerializeField] private float _waitTime;
    [SerializeField] private float _speed;
    [SerializeField] private bool _turnAround;
    [SerializeField] private bool _waitAtEachPoint;
    #endregion



    #region Fields
    private int _currentPoint = 0;
    private bool _forward = true;
    private bool _canMove = true;
    private bool _atEnd = false;
    private bool _enterOnce = true;
    private RigidbodyInterpolation2D _interpolation2D;
    private Vector2 _platformPositionLastUpdate = Vector2.zero;
    private Vector2 _platformVelocityVector = Vector2.zero;
    #endregion



    #region Properties
    public Transform[] LedgeCornerPositions { get => _ledgeCornerPositions; }
    public Vector2 PlatformVelocityVector { get => _platformVelocityVector; }
    #endregion



    #region Unity Callback Functions
    private void Update()
    {
        _platformVelocityVector = new Vector2(transform.position.x - _platformPositionLastUpdate.x, transform.position.y - _platformPositionLastUpdate.y);
        _platformPositionLastUpdate = transform.position;

        if (transform.position == _movePoints[_currentPoint].position)
        {
            if (!_canMove && _enterOnce)
            {
                _enterOnce = false;
                StartCoroutine(WaitAtEnd(_waitTime));
            }


            if (_canMove)
            {
                if (_atEnd)
                {
                    _atEnd = false;
                    _currentPoint = 0;
                    _canMove = false;
                }
                else
                {
                    if (_forward) { _currentPoint++; }
                    else { _currentPoint--; }

                    if (_currentPoint == _movePoints.Length - 1 || _currentPoint == 0)
                    {
                        if (_turnAround)
                        {
                            _forward = !_forward;
                        }
                        else
                        {
                            if (_currentPoint == _movePoints.Length - 1) { _atEnd = true; }
                        }
                        _canMove = false;
                    }

                    if (_waitAtEachPoint) { _canMove = false; }
                }

                StartCoroutine(MoveToNextPoint(_movePoints[_currentPoint].position));
                _enterOnce = true;
            }
        }
    }
    #endregion



    #region Private Methods
    IEnumerator MoveToNextPoint(Vector3 target)
    {
        Vector3 startPosition = transform.position;
        float time = 0f;

        while (transform.position != target)
        {
            transform.position = Vector3.Lerp(startPosition, target, (time / Vector3.Distance(startPosition, target)) * _speed);
            time += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator WaitAtEnd(float waitAtEndTime)
    {
        yield return new WaitForSeconds(waitAtEndTime);
        _canMove = true;
    }
    #endregion



    #region Collision Callbacks
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = this.transform;
            _interpolation2D = collision.gameObject.GetComponent<Rigidbody2D>().interpolation;
            collision.gameObject.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject;
            player.transform.parent = null;
            player.GetComponent<Rigidbody2D>().interpolation = _interpolation2D;
        }
    }
    #endregion
}
