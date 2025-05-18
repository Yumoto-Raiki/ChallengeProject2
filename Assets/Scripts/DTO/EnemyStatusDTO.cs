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
    private int _hp = default;
    /// <summary>
    /// �h��
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("�h��")]
    private int _def = default;
    /// <summary>
    /// �U����
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("��")]
    private int _pow = default;
    /// <summary>
    /// �ړ����x
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("�ړ����x")]
    private int _moveSpeed = default;

    /// <summary>
    /// �o��鍂��
    /// </summary>
    [SerializeField, Range(1f, 10f), Header("�o��鍂��")]
    private int _climbHeight = default;

    /// <summary>
    /// �̗�
    /// </summary>
    public int Hp { get => _hp; set => _hp = value >= 0 ? value : 0; }
    /// <summary>
    /// �h���
    /// </summary>
    public int Def { get => _def; set => _def = value >= 0 ? value : 0; }
    /// <summary>
    /// �U����
    /// </summary>
    public int Pow { get => _pow; set => _pow = value >= 0 ? value : 0; }
    /// <summary>
    /// �ړ����x
    /// </summary>
    public int MoveSpeed { get => _moveSpeed; set => _moveSpeed = value >= 0 ? value : 0; }
    /// <summary>
    /// �o��鍂��
    /// </summary>
    public int ClimbHeight { get => _climbHeight; set => _climbHeight = value >= 0 ? value : 0; }
}
