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
    private int _hp = default;
    /// <summary>
    /// –hŒä
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("–hŒä")]
    private int _def = default;
    /// <summary>
    /// UŒ‚—Í
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("—Í")]
    private int _pow = default;
    /// <summary>
    /// ˆÚ“®‘¬“x
    /// </summary>
    [SerializeField, Range(1f, 100f), Header("ˆÚ“®‘¬“x")]
    private int _moveSpeed = default;

    /// <summary>
    /// “o‚ê‚é‚‚³
    /// </summary>
    [SerializeField, Range(1f, 10f), Header("“o‚ê‚é‚‚³")]
    private int _climbHeight = default;

    /// <summary>
    /// ‘Ì—Í
    /// </summary>
    public int Hp { get => _hp; set => _hp = value >= 0 ? value : 0; }
    /// <summary>
    /// –hŒä—Í
    /// </summary>
    public int Def { get => _def; set => _def = value >= 0 ? value : 0; }
    /// <summary>
    /// UŒ‚—Í
    /// </summary>
    public int Pow { get => _pow; set => _pow = value >= 0 ? value : 0; }
    /// <summary>
    /// ˆÚ“®‘¬“x
    /// </summary>
    public int MoveSpeed { get => _moveSpeed; set => _moveSpeed = value >= 0 ? value : 0; }
    /// <summary>
    /// “o‚ê‚é‚‚³
    /// </summary>
    public int ClimbHeight { get => _climbHeight; set => _climbHeight = value >= 0 ? value : 0; }
}
