using UnityEngine;

public class SectionDTO
{

    /// <summary>
    /// �Q�[���I�u�W�F�N�g
    /// </summary>
    private GameObject _obj = default;

    /// <summary>
    /// �ʒu
    /// </summary>
    private Vector3 _pos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

    /// <summary>
    /// ����
    /// </summary>
    private float _height = float.MinValue;

    /// <summary>
    /// ���g�̃I�u�W�F�N�g
    /// </summary>
    private ObjectLayerState _objectLayerState = ObjectLayerState.NULL;

    public GameObject Obj { get => _obj;}
    public Vector3 Pos { get => _pos;}
    public float Height { get => _height;}
    public ObjectLayerState ObjectLayerState { get => _objectLayerState;}

    public void SetInfo(GameObject obj,Vector3 pos, float heigth,ObjectLayerState objectLayerState)
    {

        // �i����̕ێ����Ă��鍂����菬�����@�܂��́@���̃��C���[�̃I�u�W�F�N�g�����łɕێ����Ă���j���@�����Ă����I�u�W�F�N�g�̃��C���[�����ȊO
        if((_height > heigth || ObjectLayerState == ObjectLayerState.WATER) && objectLayerState != ObjectLayerState.WATER )
        {

            return;

        }
        this._obj = obj;
        this._pos = pos;
        this._height = heigth;
        this._objectLayerState = objectLayerState;

    }

}

