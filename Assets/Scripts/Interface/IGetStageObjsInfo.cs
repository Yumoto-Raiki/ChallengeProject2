using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGetStageObjsInfo 
{

    /// <summary>
    /// �X�e�[�W�p�I�u�W�F�N�g�̃��C���[
    /// </summary>
    public LayerMask TargetLayers { set; }
    /// <summary>
    /// �X�e�[�W�̖��������Ă���I�u�W�F�N�g���擾����
    /// </summary>
    public SectionDTO[,] StageObjs { get; }

    /// <summary>
    /// �X�e�[�W��̃I�u�W�F�N�g�擾
    /// </summary>
    public void GetStageObjs();

}
