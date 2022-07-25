using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    // Variavel Chão
    [Header("Sensores")]
    [SerializeField] bool _isGrounded;
    [SerializeField] bool _isTouchingWall;
    [SerializeField] bool _isTouchingRightWall;
    [SerializeField] bool _isTouchingLeftWall;
    [Min(0f)] [SerializeField] float groundRadius = .3f;
    [Min(0f)] [SerializeField] float wallRadius = .1f;


    [Header("Camadas")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;


    [Header("Deslocamento dos sensores")]
    [SerializeField] Vector2 bottomOffset = Vector2.down * .5f;
    [SerializeField] Vector2 leftOffset = Vector2.left * .5f;
    [SerializeField] Vector2 rightOffset = Vector2.right * .5f;

    public bool isGrounded => _isGrounded;
    public bool isTouchingWall => _isTouchingWall;

    public bool isTouchingRightWall => _isTouchingRightWall;
    public bool isTouchingLeftWall => _isTouchingLeftWall;

    // Update is called once per frame
    void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, groundRadius, whatIsGround);

        _isTouchingRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, wallRadius, whatIsWall);
        _isTouchingLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, wallRadius, whatIsWall);
        _isTouchingWall = _isTouchingLeftWall || _isTouchingRightWall;
    }

#if (UNITY_EDITOR)
    // Desenha os gizmos desse script
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, groundRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, wallRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, wallRadius);
    }
#endif

}
