using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Root : ISetMapInfo, IGetRoot
{

    /// <summary>
    /// SectionDTOをNodeに変換した結果を入れる
    /// </summary>
    private static NodeDTO[,] _nodeDTOs = default;

    /// <summary>
    /// ルートを制作し渡す
    /// </summary>
    /// <param name="climbHeight">登れる高さ</param>
    /// <returns></returns>
    public async Task<List<Vector3>> GetRoot(Vector3 startPos, Vector3 targetPos , float climbHeight)
    {

        (Vector2Int startindex, Vector2Int targetindex) = await SerchIndex(new Vector2Int((int)startPos.x, (int)startPos.z), new Vector2Int((int)targetPos.x, (int)targetPos.z));
        return Rooting(startindex , targetindex, climbHeight);

    }

    /// <summary>
    /// 開始位置とターゲットの居るインデックスを探す
    /// </summary>
    private async Task<(Vector2Int startindex, Vector2Int targetindex)> SerchIndex(Vector2Int startPos, Vector2Int targetPos)
    {

        if(_nodeDTOs == null)
        {

            try
            {

                Debug.Log("ノードリスト生成待ち");
                while (true)
                {

                    await Task.Delay(3000);
                    if (_nodeDTOs != null)
                    {

                        break;

                    }

                }
                Debug.Log("ノードリスト生成終わり");

            }
            catch (UnityException e)
            {


            }

        }
        Debug.Log("開始位置"+startPos);
        // 開始位置のインデックス
        Vector2Int startindex = default;
        // ターゲットの入るインデックス
        Vector2Int targetindex = default;
        for (int i = 0; i < _nodeDTOs.GetLength(0); i++)
        {

            for (int k = 0; k < _nodeDTOs.GetLength(1); k++)
            {

                NodeDTO dto = _nodeDTOs[i,k];
                if (dto == null)
                {

                    continue;

                }
                Debug.Log(dto.Position +" == "+ startPos);
                if (Vector2.Distance(dto.Position, startPos) < 2f)
                {

                    startindex.x = i;
                    startindex.y = k;

                }
                if (Vector2.Distance(dto.Position, targetPos) < 2f)
                {

                    targetindex.x = i;
                    targetindex.y = k;

                }

            }

        }
        return (startindex, targetindex);

    }

    /// <summary>
    /// SectionDTOリストからNodeDTOリストへ変換
    /// ノードに変換する際移動できるノードか、などの情報も付与する
    /// </summary>
    /// <returns></returns>
    public void SetNodeDTOList(SectionDTO[,] stageObjs)
    {

        NodeDTO[,] nodeDTOs = new NodeDTO[stageObjs.GetLength(0),stageObjs.GetLength(1)];
        for (int i = 0; i < nodeDTOs.GetLength(0); i++)
        {

            for (int k = 0; k < nodeDTOs.GetLength(1); k++)
            {

                if(stageObjs[i, k] == null)
                {

                    continue;

                }
                nodeDTOs[i, k] = new NodeDTO();
                // 水があるとき最優先
                if (stageObjs[i,k].StageWater != null)
                {

                    nodeDTOs[i, k].IsWalkable = true;
                    nodeDTOs[i, k].IsWaterble = true;
                    nodeDTOs[i, k].Position = new Vector2Int(stageObjs[i, k].StageWaterPos.x > 0 ? stageObjs[i, k].StageWaterPos.x : stageObjs[i, k].StageWaterPos.x + stageObjs.GetLength(0),
                                                             stageObjs[i, k].StageWaterPos.z > 0 ? stageObjs[i, k].StageWaterPos.z : stageObjs[i, k].StageWaterPos.z + stageObjs.GetLength(1));
                    nodeDTOs[i, k].Height = stageObjs[i, k].StageWaterPos.y;
                    continue;

                }
                // その他
                if(stageObjs[i, k].StageWall != null)
                {

                    nodeDTOs[i, k].IsWalkable = false;

                }
                else
                {

                    nodeDTOs[i, k].IsWalkable = true;

                }
                nodeDTOs[i, k].IsWaterble = false;
                nodeDTOs[i, k].Position = new Vector2Int(stageObjs[i, k].Pos.x > 0 ? stageObjs[i, k].Pos.x : stageObjs[i, k].Pos.x + stageObjs.GetLength(0),
                                                         stageObjs[i, k].Pos.z > 0 ? stageObjs[i, k].Pos.z : stageObjs[i, k].Pos.z + stageObjs.GetLength(1));
                nodeDTOs[i, k].Height = stageObjs[i, k].Pos.y;

            }

        }
        Debug.Log("完成"+nodeDTOs.Length);
        _nodeDTOs = nodeDTOs;

    }

    /// <summary>
    /// ルーティングする
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public List<Vector3> Rooting(Vector2Int startPos, Vector2Int targetPos, float climbHeight)
    {

        Debug.Log("開始位置"+startPos.x+","+startPos.y);
        NodeDTO startNode = _nodeDTOs[startPos.x,startPos.y];
        NodeDTO goalNode = _nodeDTOs[targetPos.x,targetPos.y];
        // 調べていないノード
        List<NodeDTO> openList = new List<NodeDTO> { startNode };
        // 調べ終わったノード(一意のためHashSetで要素が重複しないようにする)
        HashSet<NodeDTO> closedSet = new HashSet<NodeDTO>();
        // 移動コスト
        startNode.GCost = 0;
        // ゴールまでの予想コスト
        startNode.HCost = GetHeuristic(startPos, targetPos);
        // 調べていないノードがなくなるまで周回
        while (openList.Count > 0)
        {

            // 現在調べるノードを取り出す（調べていないノードのリストの中からCOSTが一番低いもの）
            NodeDTO currentNode = openList.OrderBy(n => n.FCost).First();
            // ゴールした時
            if (currentNode == goalNode)
            {

                return ReconstructRoot(goalNode);

            }
            // 現在のノードを見終わったノードに移す
            openList.Remove(currentNode);
            closedSet.Add(currentNode);
            // 現在のノードの周囲のノードを調べていないノードに設定
            foreach (NodeDTO nextNode in GetNeighbors(_nodeDTOs, currentNode))
            {

                // 移動できないまたはすでに調べたノードの時
                if (!nextNode.IsWalkable || closedSet.Contains(nextNode))
                {

                    continue;

                }
                // 隣接ノードのコスト
                float tentativeGCost = currentNode.GCost + 1; 
                // 探索するノードに入っていない
                // 登れない高さ
                if (tentativeGCost < nextNode.GCost || !openList.Contains(nextNode) ||
                    Mathf.Abs(currentNode.Height - nextNode.Height) < climbHeight)
                {

                    // 移動コスト,ゴールまでの予想コスト,親ノード（ノードを呼び出したノード）を設定する
                    nextNode.GCost = tentativeGCost;
                    nextNode.HCost = GetHeuristic(nextNode.Position, targetPos);
                    nextNode.Parent = currentNode;
                    // 探索するノードに追加
                    openList.Add(nextNode);
                      
                }
            }
        }
        // 経路が見つからなかった場合
        Debug.LogError("経路が見つかりませんでした。");
        return null;

    }

    /// <summary>
    /// ２点間の距離計算
    /// ゴールまでの予想コスト
    /// </summary>
    /// <param name="currentNode">呼び出したノード</param>
    /// <param name="targetNode">ターゲットノード</param>
    /// <returns></returns>
    private float GetHeuristic(Vector2Int currentNode, Vector2Int targetNode)
    {
        // マンハッタン距離を使用
        return Mathf.Abs(currentNode.x - targetNode.x) + Mathf.Abs(currentNode.y - targetNode.y);
    }

    /// <summary>
    /// 次に探索するルートをノードリストにする
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    private List<NodeDTO> GetNeighbors(NodeDTO[,] nodeDTOs, NodeDTO node)
    {

        // 探索するルート
        List<NodeDTO> chackRoot = new List<NodeDTO>();
        Vector2Int[] directions = {

            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)

        };
        foreach (var dir in directions)
        {

            Vector2Int newPos = node.Position + dir;
            if (newPos.x >= 0 && newPos.x < nodeDTOs.GetLength(0) &&
                newPos.y >= 0 && newPos.y < nodeDTOs.GetLength(1))
            {

                chackRoot.Add(nodeDTOs[newPos.x, newPos.y]);

            }

        }
        return chackRoot;

    }

    /// <summary>
    /// ゴールまでたどり着いたときにエンドノードから順に親ノードを調べルートを生成する
    /// </summary>
    /// <param name="endNode">終了時のノード</param>
    /// <returns></returns>
    private List<Vector3> ReconstructRoot(NodeDTO endNode)
    {

        // ルート
        List<Vector3> root = new List<Vector3>();
        // 現在の調べているノード
        NodeDTO currentNode = endNode;
        // 
        while (currentNode != null)
        {

            Vector3 pos = new Vector3(currentNode.Position.x, currentNode.Height, currentNode.Position.y); 
            // ノードをルートに追加
            root.Add(pos);
            // 次にたどるノードを設定
            currentNode = currentNode.Parent;

        }
        // ルートを反対にしスタート地点から始める
        root.Reverse();
        return root;

    }

}
