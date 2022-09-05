using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �L�����ʒu�R���g���[���[�@�L�����N�^�[�ɃA�^�b�`����
/// </summary>
public class CharacterPositionController : MonoBehaviour
{
    [SerializeField, Range(0f, 2.0f)]
    private float _velocity = 1f;

    [SerializeField, Range(0f, 1000.0f)]
    private float _jumpPower = 1f;

    [SerializeField]
    private Rigidbody2D _rigidbody;

    private static readonly float JUMP_DECAY = 0.7f;

    private static readonly Vector3 LEFT = Vector3.left;
    private static readonly Vector3 RIGHT = Vector3.right;
    private static readonly Vector2 UP = Vector2.up;

    private static string LEFT_WALL = "LeftWall";
    private static string RIGHT_WALL = "RightWall";
    private static string GROUND = "Ground";


    enum TouchWall
    {
        None,
        Left,
        Right,
    }

    private TouchWall _touchWall = TouchWall.None;
    private bool _isGround = true;

    private float _height;
    private float _width;

    private int _maxJumpNum = 2;
    private int _currentJumpNum = 0;

    private void Start()
    {
        var rectTransform = transform as RectTransform;
        _height = transform.localScale.y * rectTransform.sizeDelta.y;
        _width = transform.localScale.x * rectTransform.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
        // ���ړ�
        if ((_touchWall != TouchWall.Left) && Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += LEFT * _velocity;
        }

        // �E�ړ�
        if ((_touchWall != TouchWall.Right) && Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += RIGHT * _velocity;
        }

        // �W�����v
        if ((_currentJumpNum < _maxJumpNum) &&  // n�i��т����e�͈͓�
            (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space)))
        {
            _rigidbody.AddForce(UP * _jumpPower * Mathf.Pow(JUMP_DECAY, _currentJumpNum), ForceMode2D.Impulse);
            _currentJumpNum++;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == LEFT_WALL)
        {
            // �E���ɂ�����ǂɐG�ꂽ����
            if ((this.transform.position.x - _width / 4f) > collision.transform.position.x)
            {
                _touchWall = TouchWall.Left;

                // �߂肱�ݏC��
                var pos = this.transform.position;
                pos.x = collision.transform.position.x + _width / 2f;
                this.transform.position = pos;
            }
        }
        else if (tag == RIGHT_WALL)
        {
            // �����ɂ�����ǂɐG�ꂽ����
            if ((this.transform.position.x + _width / 4f) < collision.transform.position.x)
            {
                _touchWall = TouchWall.Right;

                // �߂肱�ݏC��
                var pos = this.transform.position;
                pos.x = collision.transform.position.x - _width / 2f;
                this.transform.position = pos;
            }
        }
        else if (tag == GROUND)
        {
            // �̂̉�����1/4�̏ꏊ���n�ʂ���ɂ����ԂŒn�ʂƐڂ����璅�n�Ƃ݂Ȃ�
            if ((this.transform.position.y + _height / 4f) > collision.transform.position.y)
            {
                _isGround = true;

                // �߂肱�ݏC��
                var pos = this.transform.position;
                pos.y = collision.transform.position.y;
                this.transform.position = pos;

                // �W�����v�̃X�e�[�^�X�����Z�b�g
                _currentJumpNum = 0;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == LEFT_WALL || tag == RIGHT_WALL)
        {
            _touchWall = TouchWall.None;
        }
        else if (tag == GROUND)
        {
            _isGround = false;

        }
    }
}
