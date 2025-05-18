using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームの進行状況管理
/// </summary>
public class GameManager : MonoBehaviour
{

    /// <summary>
    /// ステージ用オブジェクトのレイヤー
    /// </summary>
    [SerializeField]
    private LayerMask _targetLayers = default;

    IGetStageObjsInfo _getStageObjsInfo = new StageObjsManager();
    ISetMapInfo _setMapInfo = new Root();

    GameState _gameState = GameState.StagePreparation;

    private void Start()
    { 

        Execution();
    
    }

    private void Execution()
    {

        switch (_gameState)
        {

            case GameState.StagePreparation:

                // 使用するレイヤーを渡す
                _getStageObjsInfo.TargetLayers = _targetLayers;
                // 取得処理
                _getStageObjsInfo.GetStageObjs();
                // RootクラスにRoot制作に必要な情報を渡す
                _setMapInfo.SetNodeDTOList(_getStageObjsInfo.StageObjs);

                break;

        }



    }



    public void ChangeState(GameState newState)
    {

        _gameState = newState;
        Execution();

    }

}
