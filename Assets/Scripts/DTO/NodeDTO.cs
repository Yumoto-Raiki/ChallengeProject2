using UnityEngine;

/// <summary>
/// ルート探索時のノードデータ
/// </summary>
public class NodeDTO
{

    // ノードの位置
    public Vector2Int Position { get; set; }
    // 高さ
    public int Height { get; set; }
    // 水かどうか
    public bool IsWaterble { get; set; }
    // 通行可能かどうか
    public bool IsWalkable { get; set; }
    // 開始点からこのノードまでのコスト
    public float GCost { get; set; } 
    // このノードから目標点までの推定コスト
    public float HCost { get; set; }
    // 総コスト
    public float FCost => GCost + HCost;
    // 経路再構築のための親ノード
    public NodeDTO Parent { get; set; }

}
