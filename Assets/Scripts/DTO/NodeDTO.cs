using UnityEngine;

/// <summary>
/// ���[�g�T�����̃m�[�h�f�[�^
/// </summary>
public class NodeDTO
{

    /// <summary>
    /// �m�[�h�̈ʒu
    /// </summary>
    public Vector2Int IndexPosition { get; set; }
    /// <summary>
    /// �I�u�W�F�N�g�̈ʒu
    /// </summary>
    public Vector2 Position { get; set; }
    /// <summary>
    /// Int�ɐ��K����������
    /// </summary>
    public int IntHeight { get; set; }
    /// <summary>
    /// ����
    /// </summary>
    public float Height { get; set; }
    /// <summary>
    /// �I�u�W�F�N�g�̃��C���[�̃X�e�[�g
    /// </summary>
    public ObjectLayerState LayerState { get; set; }

    // ��������m�[�h----------------------------------------

    /// <summary>
    /// �J�n�_���炱�̃m�[�h�܂ł̃R�X�g
    /// </summary>
    public float StartTOCurrentCost { get; set; }
    /// <summary>
    /// ���̃m�[�h����ڕW�_�܂ł̐���R�X�g
    /// </summary>
    public float CurrentTOGoalCost { get; set; }
    /// <summary>
    /// ���R�X�g
    /// </summary>
    public float TotalCost => StartTOCurrentCost + CurrentTOGoalCost;
    /// <summary>
    /// �o�H�č\�z�̂��߂̐e�m�[�h
    /// </summary>
    public NodeDTO Parent { get; set; }

}
