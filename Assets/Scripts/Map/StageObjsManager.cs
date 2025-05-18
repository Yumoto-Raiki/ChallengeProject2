using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �X�e�[�W���̃I�u�W�F�N�g���Ǘ�����
/// </summary>
public class StageObjsManager : IGetStageObjsInfo
{

    /// <summary>
    /// �X�e�[�W�p�I�u�W�F�N�g�̃��C���[
    /// </summary>
    private LayerMask _targetLayers = default;

    /// <summary>
    /// �X�e�[�W�̖��������Ă���I�u�W�F�N�g���擾����
    /// </summary>
    private SectionDTO[,] _stageObjs = default;

    /// <summary>
    /// �X�e�[�W�p�I�u�W�F�N�g�̃��C���[
    /// </summary>
    public LayerMask TargetLayers { set => _targetLayers = value; }

    /// <summary>
    /// �X�e�[�W�̖��������Ă���I�u�W�F�N�g���擾����
    /// </summary>
    public SectionDTO[,] StageObjs { get => _stageObjs; }

    /// <summary>
    /// �X�e�[�W��̃I�u�W�F�N�g�擾
    /// </summary>
    public void GetStageObjs()
    {

        /*�@�X�e�[�W�p�I�u�W�F�N�g���擾�@*/

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

            Debug.LogError("�X�e�[�W�p�̃��C���[�܂��̓I�u�W�F�N�g�����݂��܂���");
            return;

        }
        /*�@�}�b�v�����@*/

        // �ő�ƍŏ��T�C�Y�����ς���i���E���s���j
        float maxX = float.MinValue;
        float minX = float.MaxValue;
        float maxZ = float.MinValue;
        float minZ = float.MaxValue;
        // �n�ʂ̃T�C�Y�����ς���
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
        // X���̃u���b�N�̌�
        int widthCount = Mathf.CeilToInt(maxX - minX)+1;
        // Z���̃u���b�N�̌�
        int depthCount = Mathf.CeilToInt(maxZ - minZ)+1;
        // �g����
        _stageObjs = new SectionDTO[widthCount, depthCount];
        // ���X�g�ɒǉ�
        for (int i = 0; i < stageObjs.Count; i++)
        {

            int x = Mathf.CeilToInt(maxX - stageObjs[i].transform.position.x);
            int z = Mathf.CeilToInt(maxZ - stageObjs[i].transform.position.z);
            _stageObjs[x,z] = new SectionDTO();
            _stageObjs[x,z].Pos = Vector3Int.FloorToInt(stageObjs[i].transform.position);
            switch (stageObjs[i].layer)
            {

                // �n��
                case 6:

                    _stageObjs[x,z].StageGround = stageObjs[i];
                    _stageObjs[x,z].StageGroundPos = Vector3Int.FloorToInt(stageObjs[i].transform.position);
                    _stageObjs[x,z].StageGroundScale = Vector3Int.FloorToInt(stageObjs[i].transform.localScale);
                    
                    break;
                // ��
                case 7:

                    _stageObjs[x,z].StageWall = stageObjs[i];
                    _stageObjs[x,z].StageWallPos = Vector3Int.FloorToInt(stageObjs[i].transform.position);
                    _stageObjs[x,z].StageWallScale = Vector3Int.FloorToInt(stageObjs[i].transform.localScale);

                    break;
                // ��
                case 8:

                    _stageObjs[x,z].StageWater = stageObjs[i];
                    _stageObjs[x,z].StageWaterPos = Vector3Int.FloorToInt(stageObjs[i].transform.position);
                    _stageObjs[x,z].StageWaterScale = Vector3Int.FloorToInt(stageObjs[i].transform.localScale);

                    break;
                // �{��
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
