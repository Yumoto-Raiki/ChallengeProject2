using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ステージ役のオブジェクトを管理する
/// </summary>
public class StageObjsManager : IGetStageObjsInfo
{

    /// <summary>
    /// ステージ用オブジェクトのレイヤー
    /// </summary>
    private LayerMask _targetLayers = default;

    /// <summary>
    /// ステージの役割をしているオブジェクトを取得する
    /// </summary>
    private SectionDTO[,] _stageObjs = default;

    /// <summary>
    /// ステージ用オブジェクトのレイヤー
    /// </summary>
    public LayerMask TargetLayers { set => _targetLayers = value; }

    /// <summary>
    /// ステージの役割をしているオブジェクトを取得する
    /// </summary>
    public SectionDTO[,] StageObjs { get => _stageObjs; }

    /// <summary>
    /// ステージ上のオブジェクト取得
    /// </summary>
    public void GetStageObjs()
    {

        /*　ステージ用オブジェクトを取得　*/

        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        List<GameObject> stageObjs = new List<GameObject>();

        foreach (GameObject obj in allObjects)
        {

            if (((1 << obj.layer) & _targetLayers) != 0)
            {

                stageObjs.Add(obj);

            }

        }
        if(stageObjs.Count == 0)
        {

            Debug.LogError("ステージ用のレイヤーまたはオブジェクトが存在しません");
            return;

        }
        /*　マップ生成　*/

        // 最大と最小サイズを見積もる（幅・奥行き）
        float maxX = float.MinValue;
        float minX = float.MaxValue;
        float maxZ = float.MinValue;
        float minZ = float.MaxValue;
        // 地面のサイズを見積もる
        for (int i = 0; i < stageObjs.Count; i++)
        {

            if (stageObjs[i].transform.position.x > maxX)
            {

                maxX = stageObjs[i].transform.position.x;

            }
            if (stageObjs[i].transform.position.x < minX)
            {

                minX = stageObjs[i].transform.position.x;

            }
            if (stageObjs[i].transform.position.z > maxZ)
            {

                maxZ = stageObjs[i].transform.position.z;

            }
            if (stageObjs[i].transform.position.z < minZ)
            {

                minZ = stageObjs[i].transform.position.z;

            }

        }
        // X軸のブロックの個数
        int widthCount = Mathf.CeilToInt(maxX - minX)+1;
        // Z軸のブロックの個数
        int depthCount = Mathf.CeilToInt(maxZ - minZ)+1;
        // 枠制作
        _stageObjs = new SectionDTO[widthCount, depthCount];
        // リストに追加
        for (int i = 0; i < stageObjs.Count; i++)
        {

            int x = Mathf.CeilToInt(maxX - stageObjs[i].transform.position.x);
            int z = Mathf.CeilToInt(maxZ - stageObjs[i].transform.position.z);
            _stageObjs[x,z] = new SectionDTO();
            _stageObjs[x,z].Pos = Vector3Int.FloorToInt(stageObjs[i].transform.position);
            switch (stageObjs[i].layer)
            {

                // 地面
                case 6:

                    _stageObjs[x,z].StageGround = stageObjs[i];
                    _stageObjs[x,z].StageGroundPos = Vector3Int.FloorToInt(stageObjs[i].transform.position);
                    _stageObjs[x,z].StageGroundScale = Vector3Int.FloorToInt(stageObjs[i].transform.localScale);
                    
                    break;
                // 壁
                case 7:

                    _stageObjs[x,z].StageWall = stageObjs[i];
                    _stageObjs[x,z].StageWallPos = Vector3Int.FloorToInt(stageObjs[i].transform.position);
                    _stageObjs[x,z].StageWallScale = Vector3Int.FloorToInt(stageObjs[i].transform.localScale);

                    break;
                // 水
                case 8:

                    _stageObjs[x,z].StageWater = stageObjs[i];
                    _stageObjs[x,z].StageWaterPos = Vector3Int.FloorToInt(stageObjs[i].transform.position);
                    _stageObjs[x,z].StageWaterScale = Vector3Int.FloorToInt(stageObjs[i].transform.localScale);

                    break;
                // 施設
                case 9:

                    _stageObjs[x,z].StageFacility = stageObjs[i];
                    _stageObjs[x,z].StageFacilityPos = Vector3Int.FloorToInt(stageObjs[i].transform.position);
                    _stageObjs[x,z].StageFacilityScale = Vector3Int.FloorToInt(stageObjs[i].transform.localScale);

                    break;
                default:

                    break;

            }

        }

    }
    
}
