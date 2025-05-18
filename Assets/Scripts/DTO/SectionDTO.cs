using UnityEngine;

public class SectionDTO
{

    /// <summary>
    /// �ʒu
    /// </summary>
    private Vector3Int _pos;

    /// <summary>
    /// �ʒu
    /// ���ʒu���������̂�����
    /// </summary>
    public Vector3Int Pos
    {

        get => _pos;
        set => _pos = ( _pos.y < value.y) ? value : _pos;

    }

    /* �I�u�W�F�N�g */

    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i�n�ʁj
    /// </summary>
    public GameObject StageGround { get; set; }
    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i�ǁj
    /// </summary>
    public GameObject StageWall { get; set; }
    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i���j
    /// </summary>
    public GameObject StageWater { get; set; }
    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i�{�݁j
    /// </summary>
    public GameObject StageFacility { get; set; }

    /*�@�ʒu�@*/

    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i�n�ʁj
    /// </summary>
    public Vector3Int StageGroundPos { get; set; }
    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i�ǁj
    /// </summary>
    public Vector3Int StageWallPos { get; set; }
    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i���j
    /// </summary>
    public Vector3Int StageWaterPos { get; set; }
    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i�{�݁j
    /// </summary>
    public Vector3Int StageFacilityPos { get; set; }

    /*�@�T�C�Y�@*/

    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i�n�ʁj
    /// </summary>
    public Vector3Int StageGroundScale { get; set; }
    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i�ǁj
    /// </summary>
    public Vector3Int StageWallScale { get; set; }
    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i���j
    /// </summary>
    public Vector3Int StageWaterScale { get; set; }
    /// <summary>
    /// �X�e�[�W�p�̃I�u�W�F�N�g�i�{�݁j
    /// </summary>
    public Vector3Int StageFacilityScale { get; set; }

}

