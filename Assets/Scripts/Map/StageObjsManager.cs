using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

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
        HashSet<LayerMask> masks = new HashSet<LayerMask>();
        foreach (GameObject obj in allObjects)
        {

            if (((1 << obj.layer) & _targetLayers) != 0)
            {

                masks.Add(obj.layer);
                stageObjs.Add(obj);

            }

        }
        Debug.Log(masks.Count);
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
        Debug.Log("X軸最大"+maxX);
        Debug.Log("X軸最小"+minX);
        Debug.Log("Z軸最大"+maxZ);
        Debug.Log("Z軸最小"+minZ);
        // X軸のブロックの個数
        int widthCount = Mathf.CeilToInt(maxX - minX);
        Debug.Log("横軸の大きさ"+widthCount);
        // Z軸のブロックの個数
        int depthCount = Mathf.CeilToInt(maxZ - minZ);
        Debug.Log("奥軸の大きさ" + depthCount);
        // 枠制作
        _stageObjs = new SectionDTO[widthCount, depthCount];
        // リストに追加
        for (int i = 0; i < stageObjs.Count; i++)
        {

            int x = (int)(maxX - stageObjs[i].transform.position.x);
            int z = (int)(maxZ - stageObjs[i].transform.position.z);
            _stageObjs[x,z] = new SectionDTO();
            if(x == 1 ||  z == 29)
            {

                Debug.LogError(stageObjs[i].name);

            }
            Vector3 pos = stageObjs[i].transform.position;
            float height = pos.y + stageObjs[i].transform.localScale.y;
            // レイヤー取得
            ObjectLayerState layerStatus = ObjectLayerState.NULL;
            switch (stageObjs[i].layer)
            {

                // 地面
                case 6:

                    layerStatus = ObjectLayerState.GROUND;
                    
                    break;
                // 壁
                case 7:

                    layerStatus = ObjectLayerState.WALL;

                    break;
                // 水
                case 8:

                    layerStatus = ObjectLayerState.WATER;

                    break;
                // 施設
                case 9:

                    layerStatus = ObjectLayerState.FACILITY;

                    break;
                default:

                    layerStatus = ObjectLayerState.NULL;

                    break;

            }
            // 情報セット
            _stageObjs[x, z].SetInfo(stageObjs[i], pos, height, layerStatus);

        }
        for (int i = 0; i < _stageObjs.GetLength(0); i++)
        {

            string text = "";
            for (int j = 0; j < _stageObjs.GetLength(1); j++)
            {

                text += ".";
                if (_stageObjs[i, j] != null)
                {

                    text += "1";

                }
                else
                {

                   // Debug.LogError(i+","+ j);
                    text += "0"; 

                }

            }
            Debug.Log(text);

        }
        Debug.Log("<Color=red>----------------------------------------------------------------------------------</color>");

    }
    
}
