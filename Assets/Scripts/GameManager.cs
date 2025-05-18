using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���̐i�s�󋵊Ǘ�
/// </summary>
public class GameManager : MonoBehaviour
{

    /// <summary>
    /// �X�e�[�W�p�I�u�W�F�N�g�̃��C���[
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

                // �g�p���郌�C���[��n��
                _getStageObjsInfo.TargetLayers = _targetLayers;
                // �擾����
                _getStageObjsInfo.GetStageObjs();
                // Root�N���X��Root����ɕK�v�ȏ���n��
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
