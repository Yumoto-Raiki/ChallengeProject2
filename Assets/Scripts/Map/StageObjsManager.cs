using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

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
        Debug.Log("X���ő�"+maxX);
        Debug.Log("X���ŏ�"+minX);
        Debug.Log("Z���ő�"+maxZ);
        Debug.Log("Z���ŏ�"+minZ);
        // X���̃u���b�N�̌�
        int widthCount = Mathf.CeilToInt(maxX - minX);
        Debug.Log("�����̑傫��"+widthCount);
        // Z���̃u���b�N�̌�
        int depthCount = Mathf.CeilToInt(maxZ - minZ);
        Debug.Log("�����̑傫��" + depthCount);
        // �g����
        _stageObjs = new SectionDTO[widthCount, depthCount];
        // ���X�g�ɒǉ�
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
            // ���C���[�擾
            ObjectLayerState layerStatus = ObjectLayerState.NULL;
            switch (stageObjs[i].layer)
            {

                // �n��
                case 6:

                    layerStatus = ObjectLayerState.GROUND;
                    
                    break;
                // ��
                case 7:

                    layerStatus = ObjectLayerState.WALL;

                    break;
                // ��
                case 8:

                    layerStatus = ObjectLayerState.WATER;

                    break;
                // �{��
                case 9:

                    layerStatus = ObjectLayerState.FACILITY;

                    break;
                default:

                    layerStatus = ObjectLayerState.NULL;

                    break;

            }
            // ���Z�b�g
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
