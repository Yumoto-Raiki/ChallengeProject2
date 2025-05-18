using UnityEngine;

/// <summary>
/// ���[�g�T�����̃m�[�h�f�[�^
/// </summary>
public class NodeDTO
{

    // �m�[�h�̈ʒu
    public Vector2Int Position { get; set; }
    // ����
    public int Height { get; set; }
    // �����ǂ���
    public bool IsWaterble { get; set; }
    // �ʍs�\���ǂ���
    public bool IsWalkable { get; set; }
    // �J�n�_���炱�̃m�[�h�܂ł̃R�X�g
    public float GCost { get; set; } 
    // ���̃m�[�h����ڕW�_�܂ł̐���R�X�g
    public float HCost { get; set; }
    // ���R�X�g
    public float FCost => GCost + HCost;
    // �o�H�č\�z�̂��߂̐e�m�[�h
    public NodeDTO Parent { get; set; }

}
