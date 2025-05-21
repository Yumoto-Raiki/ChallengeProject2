using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStatusDTO
{

    /// <summary>
    /// ‘Ì—Í
    /// </summary>
    [SerializeField, Range(1f, 100f),Header("‘Ì—Í")]
    private int _hp = 1;
    /// <summary>
    /// –hŒä
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("–hŒä")]
    private int _def = 1;
    /// <summary>
    /// UŒ‚—Í
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("—Í")]
    private int _pow = 1;
    /// <summary>
    /// ˆÚ“®‘¬“x
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("ˆÚ“®‘¬“x")]
    private int _moveSpeed = 1;

    /// <summary>
    /// “o‚ê‚é‚‚³
    /// </summary>
    [SerializeField, Range(1f, 10f), Header("“o‚ê‚é‚‚³")]
    private int _climbHeight = 1;

    /// <summary>
    /// ‘Ì—Í
    /// </summary>
    public int Hp { get => _hp; set => _hp = value; }
    /// <summary>
    /// –hŒä—Í
    /// </summary>
    public int Def { get => _def; set => _def = value; }
    /// <summary>
    /// UŒ‚—Í
    /// </summary>
    public int Pow { get => _pow; set => _pow = value; }
    /// <summary>
    /// ˆÚ“®‘¬“x
    /// </summary>
    public int MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    /// <summary>
    /// “o‚ê‚é‚‚³
    /// </summary>
    public int ClimbHeight { get => _climbHeight; set => _climbHeight = value; }
}
