using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStatusDTO
{

    /// <summary>
    /// 体力
    /// </summary>
    [SerializeField, Range(1f, 100f),Header("体力")]
    private int _hp = 1;
    /// <summary>
    /// 防御
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("防御")]
    private int _def = 1;
    /// <summary>
    /// 攻撃力
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("力")]
    private int _pow = 1;
    /// <summary>
    /// 移動速度
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("移動速度")]
    private int _moveSpeed = 1;

    /// <summary>
    /// 登れる高さ
    /// </summary>
    [SerializeField, Range(1f, 10f), Header("登れる高さ")]
    private int _climbHeight = 1;

    /// <summary>
    /// 体力
    /// </summary>
    public int Hp { get => _hp; set => _hp = value; }
    /// <summary>
    /// 防御力
    /// </summary>
    public int Def { get => _def; set => _def = value; }
    /// <summary>
    /// 攻撃力
    /// </summary>
    public int Pow { get => _pow; set => _pow = value; }
    /// <summary>
    /// 移動速度
    /// </summary>
    public int MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    /// <summary>
    /// 登れる高さ
    /// </summary>
    public int ClimbHeight { get => _climbHeight; set => _climbHeight = value; }
}
