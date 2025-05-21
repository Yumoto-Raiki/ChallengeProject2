using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStatusDTO
{

    /// <summary>
    /// �̗�
    /// </summary>
    [SerializeField, Range(1f, 100f),Header("�̗�")]
    private int _hp = 1;
    /// <summary>
    /// �h��
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("�h��")]
    private int _def = 1;
    /// <summary>
    /// �U����
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("��")]
    private int _pow = 1;
    /// <summary>
    /// �ړ����x
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("�ړ����x")]
    private int _moveSpeed = 1;

    /// <summary>
    /// �o��鍂��
    /// </summary>
    [SerializeField, Range(1f, 10f), Header("�o��鍂��")]
    private int _climbHeight = 1;

    /// <summary>
    /// �̗�
    /// </summary>
    public int Hp { get => _hp; set => _hp = value; }
    /// <summary>
    /// �h���
    /// </summary>
    public int Def { get => _def; set => _def = value; }
    /// <summary>
    /// �U����
    /// </summary>
    public int Pow { get => _pow; set => _pow = value; }
    /// <summary>
    /// �ړ����x
    /// </summary>
    public int MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    /// <summary>
    /// �o��鍂��
    /// </summary>
    public int ClimbHeight { get => _climbHeight; set => _climbHeight = value; }
}
