using UnityEngine;

public class SectionDTO
{

    /// <summary>
    /// 位置
    /// </summary>
    private Vector3Int _pos = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

    private int _height = int.MinValue;

    /// <summary>
    /// 位置
    /// より位置が高いものが入る
    /// </summary>
    public Vector3Int Pos
    {

        get => _pos;

    }


    public int Height {
        get => _height;
    }

    public void SetInfo(Vector3Int pos,int heigth,bool isWater)
    {

        if((_height > heigth || StageWater != null) && !isWater)
        {

            return;

        }
        this._pos = pos;
        this._height = heigth;

    }

    /* オブジェクト */

    /// <summary>
    /// ステージ用のオブジェクト（地面）
    /// </summary>
    public GameObject StageGround { get; set; }
    /// <summary>
    /// ステージ用のオブジェクト（壁）
    /// </summary>
    public GameObject StageWall { get; set; }
    /// <summary>
    /// ステージ用のオブジェクト（水）
    /// </summary>
    public GameObject StageWater { get; set; }
    /// <summary>
    /// ステージ用のオブジェクト（施設）
    /// </summary>
    public GameObject StageFacility { get; set; }

    /*　位置　*/

    /// <summary>
    /// ステージ用のオブジェクト（地面）
    /// </summary>
    public Vector3Int StageGroundPos { get; set; }
    /// <summary>
    /// ステージ用のオブジェクト（壁）
    /// </summary>
    public Vector3Int StageWallPos { get; set; }
    /// <summary>
    /// ステージ用のオブジェクト（水）
    /// </summary>
    public Vector3Int StageWaterPos { get; set; }
    /// <summary>
    /// ステージ用のオブジェクト（施設）
    /// </summary>
    public Vector3Int StageFacilityPos { get; set; }

    /*　サイズ　*/

    /// <summary>
    /// ステージ用のオブジェクト（地面）
    /// </summary>
    public Vector3Int StageGroundScale { get; set; }
    /// <summary>
    /// ステージ用のオブジェクト（壁）
    /// </summary>
    public Vector3Int StageWallScale { get; set; }
    /// <summary>
    /// ステージ用のオブジェクト（水）
    /// </summary>
    public Vector3Int StageWaterScale { get; set; }
    /// <summary>
    /// ステージ用のオブジェクト（施設）
    /// </summary>
    public Vector3Int StageFacilityScale { get; set; }

}

