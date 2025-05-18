using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetStageObjsInfo 
{

    /// <summary>
    /// ステージ用オブジェクトのレイヤー
    /// </summary>
    public LayerMask TargetLayers { set; }
    /// <summary>
    /// ステージの役割をしているオブジェクトを取得する
    /// </summary>
    public SectionDTO[,] StageObjs { get; }

    /// <summary>
    /// ステージ上のオブジェクト取得
    /// </summary>
    public void GetStageObjs();

}
