using UnityEngine;

public class SectionDTO
{

    /// <summary>
    /// ゲームオブジェクト
    /// </summary>
    private GameObject _obj = default;

    /// <summary>
    /// 位置
    /// </summary>
    private Vector3 _pos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

    /// <summary>
    /// 高さ
    /// </summary>
    private float _height = float.MinValue;

    /// <summary>
    /// 中身のオブジェクト
    /// </summary>
    private ObjectLayerState _objectLayerState = ObjectLayerState.NULL;

    public GameObject Obj { get => _obj;}
    public Vector3 Pos { get => _pos;}
    public float Height { get => _height;}
    public ObjectLayerState ObjectLayerState { get => _objectLayerState;}

    public void SetInfo(GameObject obj,Vector3 pos, float heigth,ObjectLayerState objectLayerState)
    {

        // （現状の保持している高さより小さい　または　水のレイヤーのオブジェクトをすでに保持している）かつ　入ってきたオブジェクトのレイヤーが水以外
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

