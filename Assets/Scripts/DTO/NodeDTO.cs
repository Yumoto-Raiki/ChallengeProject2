using UnityEngine;

/// <summary>
/// ルート探索時のノードデータ
/// </summary>
public class NodeDTO
{

    /// <summary>
    /// ノードの位置
    /// </summary>
    public Vector2Int IndexPosition { get; set; }
    /// <summary>
    /// オブジェクトの位置
    /// </summary>
    public Vector2 Position { get; set; }
    /// <summary>
    /// Intに正規化した高さ
    /// </summary>
    public int IntHeight { get; set; }
    /// <summary>
    /// 高さ
    /// </summary>
    public float Height { get; set; }
    /// <summary>
    /// オブジェクトのレイヤーのステート
    /// </summary>
    public ObjectLayerState LayerState { get; set; }

    // ここからノード----------------------------------------

    /// <summary>
    /// 開始点からこのノードまでのコスト
    /// </summary>
    public float StartTOCurrentCost { get; set; }
    /// <summary>
    /// このノードから目標点までの推定コスト
    /// </summary>
    public float CurrentTOGoalCost { get; set; }
    /// <summary>
    /// 壁に近いか判断するコスト
    /// </summary>
    public float WallPenalty { get; set; } 
    /// <summary>
    /// 総コスト
    /// </summary>
    public float TotalCost => StartTOCurrentCost + CurrentTOGoalCost + WallPenalty;
    /// <summary>
    /// 経路再構築のための親ノード
    /// </summary>
    public NodeDTO Parent { get; set; }

}
