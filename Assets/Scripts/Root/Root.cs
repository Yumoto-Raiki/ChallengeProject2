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
    /// SectionDTOリストからNodeDTOリストへ変換
    /// ノードに変換する際移動できるノードか、などの情報も付与する
    /// </summary>
    /// <returns></returns>
    public void SetNodeDTOList(SectionDTO[,] stageObjs)
    {

        NodeDTO[,] nodeDTOs = new NodeDTO[stageObjs.GetLength(0), stageObjs.GetLength(1)];
        Vector2Int[,] chacks = new Vector2Int[stageObjs.GetLength(0), stageObjs.GetLength(1)];
        // 配列"X"軸の最小値の絶対値
        int arrayXMinAbs = Mathf.RoundToInt(stageObjs[0, 0].Pos.x);
        arrayXMinAbs = Mathf.Abs(arrayXMinAbs);
        // 配列"Z"軸の最小値の絶対値
        int arrayZMinAbs = Mathf.RoundToInt(stageObjs[0, 0].Pos.z);
        arrayZMinAbs = Mathf.Abs(arrayZMinAbs);
        for (int i = 0; i < nodeDTOs.GetLength(0); i++)
        {

            for (int k = 0; k < nodeDTOs.GetLength(1); k++)
            {

                if (stageObjs[i, k] == null)
                {

                    continue;

                }
                nodeDTOs[i, k] = new NodeDTO();
                int x = Mathf.RoundToInt(stageObjs[i, k].Pos.x);
                int z = Mathf.RoundToInt(stageObjs[i, k].Pos.z);
                // インデックスに対応させるため最小値の絶対値を足す
                x += arrayXMinAbs;
                z += arrayZMinAbs;
                nodeDTOs[i, k].IndexPosition = (Vector2Int.right * x) + (Vector2Int.up * z);
                nodeDTOs[i, k].Position = Vector2.right * stageObjs[i, k].Pos.x + Vector2.up * stageObjs[i, k].Pos.z;
                nodeDTOs[i, k].IntHeight = Mathf.RoundToInt(stageObjs[i, k].Pos.y + stageObjs[i, k].Height);
                nodeDTOs[i, k].Height = stageObjs[i, k].Pos.y + stageObjs[i, k].Height;
                nodeDTOs[i, k].LayerState = stageObjs[i, k].ObjectLayerState;
                chacks[i, k] = (Vector2Int.right * x) + (Vector2Int.up * z);

            }
            string text = "";
            int count = 0;
            for (int j = 0; j < nodeDTOs.GetLength(1); j++)
            {

                count++;
                text += chacks[i, j] + ",";

            }
            Debug.Log(text);

        }
        Debug.Log("完成" + nodeDTOs.Length);
        Debug.Log("-------------------------------------------------------------------------------------------------------");
        // キャッシュ
        _nodeDTOs = nodeDTOs;

    }

    /// <summary>
    /// ルートを制作し渡す
    /// </summary>
    /// <param name="climbHeight">登れる高さ</param>
    /// <returns></returns>
    public async Task<List<Vector3>> GetRoot(Vector3 startPos, Vector3 targetPos, float climbHeight)
    {

        Vector2 v2StartPos = Vector2.right * startPos.x + Vector2.up * startPos.z;
        Vector2 v2TargetPos = Vector2.right * targetPos.x + Vector2.up * targetPos.z;
        (Vector2Int startIndex, Vector2Int targetIndex) = await SerchIndex(v2StartPos, v2TargetPos);
        return Rooting(startIndex, targetIndex, climbHeight);

    }

    /// <summary>
    /// 開始位置とターゲットの居るインデックスを探す
    /// </summary>
    private async Task<(Vector2Int startindex, Vector2Int targetindex)> SerchIndex(Vector2 startPos, Vector2 targetPos)
    {

        /* 探索する配列確保まで待つ */
        if (_nodeDTOs == null)
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

                Debug.LogWarning("エラー：ノード配列確保失敗" + e.ToString());

            }

        }
        // 開始位置のインデックス
        Vector2Int startIndex = Vector2Int.right * int.MinValue + Vector2Int.up * int.MinValue;
        Vector2Int debugStartIndex = Vector2Int.right * int.MinValue + Vector2Int.up * int.MinValue;
        // ターゲットの入るインデックス
        Vector2Int targetIndex = Vector2Int.right * int.MinValue + Vector2Int.up * int.MinValue;
        Vector2Int debugTargetIndex = Vector2Int.right * int.MinValue + Vector2Int.up * int.MinValue;
        for (int i = 0; i < _nodeDTOs.GetLength(0); i++)
        {

            for (int k = 0; k < _nodeDTOs.GetLength(1); k++)
            {

                NodeDTO nordDTO = _nodeDTOs[i, k];
                if (nordDTO == null)
                {

                    continue;

                }
                if (Vector2.Distance(nordDTO.Position, startPos) < 1f)
                {

                    startIndex.x = i;
                    startIndex.y = k;
                    Debug.Log("開始のインデックス" + startIndex);
                    Debug.Log("開始のインデックス位置" + nordDTO.IndexPosition);

                }
                if (Vector2.Distance(nordDTO.Position, targetPos) < 1f)
                {

                    targetIndex.x = i;
                    targetIndex.y = k;
                    Debug.Log("ターゲットのインデックス" + targetIndex);
                    Debug.Log("ターゲットのインデックス位置" + nordDTO.IndexPosition);

                }

            }

        }
        if (startIndex == debugStartIndex)
        {

            Debug.LogWarning("開始のインデックスが見つかりませんでした");

        }
        else
        {

          

        }
        if (targetIndex == debugTargetIndex)
        {

            Debug.LogWarning("ターゲットのインデックスが見つかりませんでした。ターゲットの位置："+ targetPos);

        }
        else
        {

          

        }
        return (startIndex, targetIndex);

    }

    /// <summary>
    /// ルーティングする
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="targetIndex"></param>
    /// <returns></returns>
    public List<Vector3> Rooting(Vector2Int startIndex, Vector2Int targetIndex, float climbHeight)
    {

        // 開始時のノード
        NodeDTO startNode = _nodeDTOs[startIndex.x, startIndex.y];
        Debug.Log("開始インデックスの位置" + startNode.IndexPosition);
        // ターゲットのノード
        NodeDTO targetNode = _nodeDTOs[targetIndex.x, targetIndex.y]; 
        Debug.Log("ターゲットインデックスの位置" + targetNode.IndexPosition);
        // 調べていないノード
        List<NodeDTO> openList = new List<NodeDTO> { startNode };
        // 調べ終わったノード(HashSetで要素が重複しないようにする)
        HashSet<NodeDTO> closedSet = new HashSet<NodeDTO>();
        // 開始位置から原罪の位置までのコスト
        startNode.StartTOCurrentCost = 0;
        // ターゲットまでの予想コスト
        startNode.CurrentTOGoalCost = GetHeuristic(startIndex, targetIndex);
        Debug.Log("総コスト" + startNode.TotalCost);
        // 調べていないノードがなくなるまで周回
        while (openList.Count > 0)
        {

            // 現在調べるノードを取り出す（調べていないノードのリストの中からCOSTが一番低いもの）
            NodeDTO currentNode = openList.OrderBy(n => n.TotalCost).First();
            //Debug.Log("位置"+currentNode.Position);
            Debug.Log("インデックスの位置"+currentNode.IndexPosition);
            // ゴールした時
            if (currentNode == targetNode)
            {

                return ReconstructRoot(targetNode);

            }
            // 現在のノードを見終わったノードに移す
            openList.Remove(currentNode);
            closedSet.Add(currentNode);
            // 現在のノードの周囲のノードを調べていないノードに設定
            foreach (NodeDTO nextNode in GetNeighbors(_nodeDTOs, currentNode))
            {
                //Debug.Log("周囲のインデックスの位置" + nextNode.IndexPosition);
                // 移動できないまたはすでに調べたノードの時
                if (nextNode.LayerState == ObjectLayerState.WALL || closedSet.Contains(nextNode))
                {

                    continue;
                }
                // 隣接ノードのコスト
                float tentativeGCost = currentNode.StartTOCurrentCost + 1;
                // 探索するノードに入っていない
                // 登れない高さ
                if (tentativeGCost < nextNode.StartTOCurrentCost || !openList.Contains(nextNode) ||
                    Mathf.Abs(currentNode.Height - nextNode.Height) < climbHeight)
                {

                    // 移動コスト,ゴールまでの予想コスト,親ノード（ノードを呼び出したノード）を設定する
                    nextNode.StartTOCurrentCost = tentativeGCost;
                    nextNode.CurrentTOGoalCost = GetHeuristic(nextNode.IndexPosition, targetIndex);
                    nextNode.Parent = currentNode;
                    // 探索するノードに追加
                    openList.Add(nextNode);

                }
            }
        }
        // 経路が見つからなかった場合
        Debug.LogError("エラー経路が見つかりませんでした。");
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
    ///  現在のノードに隣接するノードを取得する
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    private List<NodeDTO> GetNeighbors(NodeDTO[,] nodeDTOs, NodeDTO currentNode)
    {

        // 探索するルート
        List<NodeDTO> chackRoot = new List<NodeDTO>();
        Vector2Int[] directions = {

            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)

        };
        foreach (Vector2Int dir in directions)
        {

            Vector2Int newPos = currentNode.IndexPosition + dir;
            Debug.Log("周囲のインデックスの位置"+newPos);
            if (newPos.x >= 0 && newPos.x < nodeDTOs.GetLength(0) &&
                newPos.y >= 0 && newPos.y < nodeDTOs.GetLength(1))
            {

                if (nodeDTOs[newPos.x, newPos.y] == null)
                {

                    continue;

                }
                Debug.Log("実際の周囲のインデックスの位置" + nodeDTOs[newPos.x, newPos.y].IndexPosition);
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

            Vector3 pos = new Vector3(currentNode.Position.x, currentNode.Height + 1, currentNode.Position.y);
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
