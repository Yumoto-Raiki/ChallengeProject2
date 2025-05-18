using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Root : ISetMapInfo, IGetRoot
{

    /// <summary>
    /// SectionDTO��Node�ɕϊ��������ʂ�����
    /// </summary>
    private static NodeDTO[,] _nodeDTOs = default;

    /// <summary>
    /// ���[�g�𐧍삵�n��
    /// </summary>
    /// <param name="climbHeight">�o��鍂��</param>
    /// <returns></returns>
    public async Task<List<Vector3>> GetRoot(Vector3 startPos, Vector3 targetPos , float climbHeight)
    {

        (Vector2Int startindex, Vector2Int targetindex) = await SerchIndex(new Vector2Int((int)startPos.x, (int)startPos.z), new Vector2Int((int)targetPos.x, (int)targetPos.z));
        return Rooting(startindex , targetindex, climbHeight);

    }

    /// <summary>
    /// �J�n�ʒu�ƃ^�[�Q�b�g�̋���C���f�b�N�X��T��
    /// </summary>
    private async Task<(Vector2Int startindex, Vector2Int targetindex)> SerchIndex(Vector2Int startPos, Vector2Int targetPos)
    {

        if(_nodeDTOs == null)
        {

            try
            {

                Debug.Log("�m�[�h���X�g�����҂�");
                while (true)
                {

                    await Task.Delay(3000);
                    if (_nodeDTOs != null)
                    {

                        break;

                    }

                }
                Debug.Log("�m�[�h���X�g�����I���");

            }
            catch (UnityException e)
            {


            }

        }
        Debug.Log("�J�n�ʒu"+startPos);
        // �J�n�ʒu�̃C���f�b�N�X
        Vector2Int startindex = default;
        // �^�[�Q�b�g�̓���C���f�b�N�X
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
    /// SectionDTO���X�g����NodeDTO���X�g�֕ϊ�
    /// �m�[�h�ɕϊ�����ۈړ��ł���m�[�h���A�Ȃǂ̏����t�^����
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
                // ��������Ƃ��ŗD��
                if (stageObjs[i,k].StageWater != null)
                {

                    nodeDTOs[i, k].IsWalkable = true;
                    nodeDTOs[i, k].IsWaterble = true;
                    nodeDTOs[i, k].Position = new Vector2Int(stageObjs[i, k].StageWaterPos.x > 0 ? stageObjs[i, k].StageWaterPos.x : stageObjs[i, k].StageWaterPos.x + stageObjs.GetLength(0),
                                                             stageObjs[i, k].StageWaterPos.z > 0 ? stageObjs[i, k].StageWaterPos.z : stageObjs[i, k].StageWaterPos.z + stageObjs.GetLength(1));
                    nodeDTOs[i, k].Height = stageObjs[i, k].StageWaterPos.y;
                    continue;

                }
                // ���̑�
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
        Debug.Log("����"+nodeDTOs.Length);
        _nodeDTOs = nodeDTOs;

    }

    /// <summary>
    /// ���[�e�B���O����
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public List<Vector3> Rooting(Vector2Int startPos, Vector2Int targetPos, float climbHeight)
    {

        Debug.Log("�J�n�ʒu"+startPos.x+","+startPos.y);
        NodeDTO startNode = _nodeDTOs[startPos.x,startPos.y];
        NodeDTO goalNode = _nodeDTOs[targetPos.x,targetPos.y];
        // ���ׂĂ��Ȃ��m�[�h
        List<NodeDTO> openList = new List<NodeDTO> { startNode };
        // ���׏I������m�[�h(��ӂ̂���HashSet�ŗv�f���d�����Ȃ��悤�ɂ���)
        HashSet<NodeDTO> closedSet = new HashSet<NodeDTO>();
        // �ړ��R�X�g
        startNode.GCost = 0;
        // �S�[���܂ł̗\�z�R�X�g
        startNode.HCost = GetHeuristic(startPos, targetPos);
        // ���ׂĂ��Ȃ��m�[�h���Ȃ��Ȃ�܂Ŏ���
        while (openList.Count > 0)
        {

            // ���ݒ��ׂ�m�[�h�����o���i���ׂĂ��Ȃ��m�[�h�̃��X�g�̒�����COST����ԒႢ���́j
            NodeDTO currentNode = openList.OrderBy(n => n.FCost).First();
            // �S�[��������
            if (currentNode == goalNode)
            {

                return ReconstructRoot(goalNode);

            }
            // ���݂̃m�[�h�����I������m�[�h�Ɉڂ�
            openList.Remove(currentNode);
            closedSet.Add(currentNode);
            // ���݂̃m�[�h�̎��͂̃m�[�h�𒲂ׂĂ��Ȃ��m�[�h�ɐݒ�
            foreach (NodeDTO nextNode in GetNeighbors(_nodeDTOs, currentNode))
            {

                // �ړ��ł��Ȃ��܂��͂��łɒ��ׂ��m�[�h�̎�
                if (!nextNode.IsWalkable || closedSet.Contains(nextNode))
                {

                    continue;

                }
                // �אڃm�[�h�̃R�X�g
                float tentativeGCost = currentNode.GCost + 1; 
                // �T������m�[�h�ɓ����Ă��Ȃ�
                // �o��Ȃ�����
                if (tentativeGCost < nextNode.GCost || !openList.Contains(nextNode) ||
                    Mathf.Abs(currentNode.Height - nextNode.Height) < climbHeight)
                {

                    // �ړ��R�X�g,�S�[���܂ł̗\�z�R�X�g,�e�m�[�h�i�m�[�h���Ăяo�����m�[�h�j��ݒ肷��
                    nextNode.GCost = tentativeGCost;
                    nextNode.HCost = GetHeuristic(nextNode.Position, targetPos);
                    nextNode.Parent = currentNode;
                    // �T������m�[�h�ɒǉ�
                    openList.Add(nextNode);
                      
                }
            }
        }
        // �o�H��������Ȃ������ꍇ
        Debug.LogError("�o�H��������܂���ł����B");
        return null;

    }

    /// <summary>
    /// �Q�_�Ԃ̋����v�Z
    /// �S�[���܂ł̗\�z�R�X�g
    /// </summary>
    /// <param name="currentNode">�Ăяo�����m�[�h</param>
    /// <param name="targetNode">�^�[�Q�b�g�m�[�h</param>
    /// <returns></returns>
    private float GetHeuristic(Vector2Int currentNode, Vector2Int targetNode)
    {
        // �}���n�b�^���������g�p
        return Mathf.Abs(currentNode.x - targetNode.x) + Mathf.Abs(currentNode.y - targetNode.y);
    }

    /// <summary>
    /// ���ɒT�����郋�[�g���m�[�h���X�g�ɂ���
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    private List<NodeDTO> GetNeighbors(NodeDTO[,] nodeDTOs, NodeDTO node)
    {

        // �T�����郋�[�g
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
    /// �S�[���܂ł��ǂ蒅�����Ƃ��ɃG���h�m�[�h���珇�ɐe�m�[�h�𒲂׃��[�g�𐶐�����
    /// </summary>
    /// <param name="endNode">�I�����̃m�[�h</param>
    /// <returns></returns>
    private List<Vector3> ReconstructRoot(NodeDTO endNode)
    {

        // ���[�g
        List<Vector3> root = new List<Vector3>();
        // ���݂̒��ׂĂ���m�[�h
        NodeDTO currentNode = endNode;
        // 
        while (currentNode != null)
        {

            Vector3 pos = new Vector3(currentNode.Position.x, currentNode.Height, currentNode.Position.y); 
            // �m�[�h�����[�g�ɒǉ�
            root.Add(pos);
            // ���ɂ��ǂ�m�[�h��ݒ�
            currentNode = currentNode.Parent;

        }
        // ���[�g�𔽑΂ɂ��X�^�[�g�n�_����n�߂�
        root.Reverse();
        return root;

    }

}
