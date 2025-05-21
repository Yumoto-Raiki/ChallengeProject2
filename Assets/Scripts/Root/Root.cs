using System.Collections.Generic;
using System.Data;
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
    /// SectionDTO���X�g����NodeDTO���X�g�֕ϊ�
    /// �m�[�h�ɕϊ�����ۈړ��ł���m�[�h���A�Ȃǂ̏����t�^����
    /// </summary>
    /// <returns></returns>
    public void SetNodeDTOList(SectionDTO[,] stageObjs)
    {

        NodeDTO[,] nodeDTOs = new NodeDTO[stageObjs.GetLength(0), stageObjs.GetLength(1)];
        //Vector2Int[,] chacks = new Vector2Int[stageObjs.GetLength(0), stageObjs.GetLength(1)];
        // �z��"X"���̍ŏ��l�̐�Βl
        int arrayXMinAbs = Mathf.RoundToInt(stageObjs[0, 0].Pos.x);
        arrayXMinAbs = Mathf.Abs(arrayXMinAbs);
        // �z��"Z"���̍ŏ��l�̐�Βl
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
                // �C���f�b�N�X�ɑΉ������邽�ߍŏ��l�̐�Βl�𑫂�
                x += arrayXMinAbs;
                z += arrayZMinAbs;
                nodeDTOs[i, k].IndexPosition = (Vector2Int.right * x) + (Vector2Int.up * z);
                nodeDTOs[i, k].Position = Vector2.right * stageObjs[i, k].Pos.x + Vector2.up * stageObjs[i, k].Pos.z;
                nodeDTOs[i, k].IntHeight = Mathf.RoundToInt(stageObjs[i, k].Pos.y + stageObjs[i, k].Height);
                nodeDTOs[i, k].Height = stageObjs[i, k].Height;
                nodeDTOs[i, k].LayerState = stageObjs[i, k].ObjectLayerState;

                // --- �ǃy�i���e�B�������Œǉ� ---
                int wallCount = 0;
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        if (dx == 0 && dz == 0) continue; // ���g������

                        int nx = i + dx;
                        int nz = k + dz;

                        if (nx >= 0 && nx < stageObjs.GetLength(0) && nz >= 0 && nz < stageObjs.GetLength(1))
                        {
                            if (stageObjs[nx, nz]?.ObjectLayerState == ObjectLayerState.WALL)
                            {
                                wallCount++;
                            }
                        }
                    }
                }

                nodeDTOs[i, k].WallPenalty = wallCount * 1000f; // �������Ńy�i���e�B���Z�i�� �~ �d�݁j


                //chacks[i, k] = (Vector2Int.right * x) + (Vector2Int.up * z);

            }
            //string text = "";
            //int count = 0;
            //for (int j = 0; j < nodeDTOs.GetLength(1); j++)
            //{

            //    count++;
            //    text += chacks[i, j] + ",";

            //}
            //Debug.Log(text);

        }
        //Debug.Log("����" + nodeDTOs.Length);
        //Debug.Log("-------------------------------------------------------------------------------------------------------");
        // �L���b�V��
        _nodeDTOs = nodeDTOs;

    }

    /// <summary>
    /// ���[�g�𐧍삵�n��
    /// </summary>
    /// <param name="climbHeight">�o��鍂��</param>
    /// <returns></returns>
    public async Task<List<Vector3>> GetRoot(Vector3 startPos, Vector3 targetPos, float climbHeight)
    {

        Vector2 v2StartPos = Vector2.right * startPos.x + Vector2.up * startPos.z;
        Vector2 v2TargetPos = Vector2.right * targetPos.x + Vector2.up * targetPos.z;
        (Vector2Int startIndex, Vector2Int targetIndex) = await SerchIndex(v2StartPos, v2TargetPos);
        return Rooting(startIndex, targetIndex, climbHeight);

    }

    /// <summary>
    /// �J�n�ʒu�ƃ^�[�Q�b�g�̋���C���f�b�N�X��T��
    /// </summary>
    private async Task<(Vector2Int startindex, Vector2Int targetindex)> SerchIndex(Vector2 startPos, Vector2 targetPos)
    {

        /* �T������z��m�ۂ܂ő҂� */
        if (_nodeDTOs == null)
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

                Debug.LogWarning("�G���[�F�m�[�h�z��m�ێ��s" + e.ToString());

            }

        }
        // �J�n�ʒu�̃C���f�b�N�X
        Vector2Int startIndex = Vector2Int.right * int.MinValue + Vector2Int.up * int.MinValue;
        Vector2Int debugStartIndex = Vector2Int.right * int.MinValue + Vector2Int.up * int.MinValue;
        // �^�[�Q�b�g�̓���C���f�b�N�X
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

                }
                if (Vector2.Distance(nordDTO.Position, targetPos) < 1f)
                {

                    targetIndex.x = i;
                    targetIndex.y = k;

                }

            }

        }
        if (startIndex == debugStartIndex)
        {

            Debug.LogWarning("�J�n�̃C���f�b�N�X��������܂���ł���");

        }
        else
        {


            Debug.Log("�J�n�̃C���f�b�N�X" + startIndex);

        }
        if (targetIndex == debugTargetIndex)
        {

            Debug.LogWarning("�^�[�Q�b�g�̃C���f�b�N�X��������܂���ł����B�^�[�Q�b�g�̈ʒu�F"+ targetPos);

        }
        else
        {


            Debug.Log("�^�[�Q�b�g�̃C���f�b�N�X" + targetIndex);

        }
        return (startIndex, targetIndex);

    }

    /// <summary>
    /// ���[�e�B���O����
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="targetIndex"></param>
    /// <returns></returns>
    public List<Vector3> Rooting(Vector2Int startIndex, Vector2Int targetIndex, float climbHeight)
    {

        // �J�n���̃m�[�h
        NodeDTO startNode = _nodeDTOs[startIndex.x, startIndex.y];
        Debug.Log("�J�n�C���f�b�N�X�̈ʒu" + startNode.IndexPosition);
        // �^�[�Q�b�g�̃m�[�h
        NodeDTO targetNode = _nodeDTOs[targetIndex.x, targetIndex.y]; 
        Debug.Log("�^�[�Q�b�g�C���f�b�N�X�̈ʒu" + targetNode.IndexPosition);
        // ���ׂĂ��Ȃ��m�[�h
        List<NodeDTO> openList = new List<NodeDTO> { startNode };
        // ���׏I������m�[�h(HashSet�ŗv�f���d�����Ȃ��悤�ɂ���)
        HashSet<NodeDTO> closedSet = new HashSet<NodeDTO>();
        // �J�n�ʒu���猴�߂̈ʒu�܂ł̃R�X�g
        startNode.StartTOCurrentCost = 0;
        // �^�[�Q�b�g�܂ł̗\�z�R�X�g
        startNode.CurrentTOGoalCost = GetHeuristic(startIndex, targetIndex);
        Debug.Log("���R�X�g" + startNode.TotalCost);
        // ���ׂĂ��Ȃ��m�[�h���Ȃ��Ȃ�܂Ŏ���
        while (openList.Count > 0)
        {

            // ���ݒ��ׂ�m�[�h�����o���i���ׂĂ��Ȃ��m�[�h�̃��X�g�̒�����COST����ԒႢ���́j
            NodeDTO currentNode = openList.OrderBy(n => n.TotalCost).First();
            //Debug.Log("�ʒu"+currentNode.Position);
           // Debug.Log("�C���f�b�N�X�̈ʒu"+currentNode.IndexPosition);
            // �S�[��������
            if (currentNode == targetNode)
            {

                return ReconstructRoot(targetNode);

            }
            // ���݂̃m�[�h�����I������m�[�h�Ɉڂ�
            openList.Remove(currentNode);
            closedSet.Add(currentNode);

            bool isNearWall = false;
            // ���݂̃m�[�h�̎��͂̃m�[�h�𒲂ׂĂ��Ȃ��m�[�h�ɐݒ�
            foreach (NodeDTO nextNode in GetNeighbors(_nodeDTOs, currentNode))
            {

                // �o��Ȃ�����
                if (Mathf.Abs(currentNode.Height - nextNode.Height) > climbHeight)
                {

                    continue;

                }

                // ���łɒ��ׂ��m�[�h�̓X�L�b�v
                if (closedSet.Contains(nextNode))
                {
                    continue;
                }
                // �אڃm�[�h�̃R�X�g
                float tentativeGCost = currentNode.StartTOCurrentCost + 1;
                // ���łɒT������m�[�h�ɓ����Ă���
                // ���łɒ��ׂ��m�[�h�̎�
                if (tentativeGCost < nextNode.StartTOCurrentCost || !openList.Contains(nextNode))
                {

                    // �ړ��R�X�g,�S�[���܂ł̗\�z�R�X�g,�e�m�[�h�i�m�[�h���Ăяo�����m�[�h�j��ݒ肷��
                    nextNode.StartTOCurrentCost = tentativeGCost;
                    nextNode.CurrentTOGoalCost = GetHeuristic(nextNode.IndexPosition, targetIndex);
                    nextNode.Parent = currentNode;
                    // �T������m�[�h�ɒǉ�
                    openList.Add(nextNode);

                }
            }
        }
        // �o�H��������Ȃ������ꍇ
        Debug.LogError("�G���[�o�H��������܂���ł����B");
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
    ///  ���݂̃m�[�h�ɗאڂ���m�[�h���擾����
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="currentNode"></param>
    /// <returns></returns>
    private List<NodeDTO> GetNeighbors(NodeDTO[,] nodeDTOs, NodeDTO currentNode)
    {

        // �T�����郋�[�g
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
            if (newPos.x >= 0 && newPos.x < nodeDTOs.GetLength(0) &&
                newPos.y >= 0 && newPos.y < nodeDTOs.GetLength(1))
            {

                if (nodeDTOs[newPos.x, newPos.y] == null)
                {

                    continue;

                }
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

            Vector3 pos = new Vector3(currentNode.Position.x, currentNode.Height + 1, currentNode.Position.y);
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
