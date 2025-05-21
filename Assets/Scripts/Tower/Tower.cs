using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class Tower : MonoBehaviour
{

    [SerializeField]
    private GameObject _camera = default;

    [SerializeField]
    private GameObject _attackArea = default;

    [SerializeField]
    private float _towerRotateSpeed = 10;

    [SerializeField]
    private float _cameraRotateSpeed = 10;

    [SerializeField]
    private float _edgeSize = 20;

    [SerializeField]
    private int _pow = 1;

    private GameObject _oldObj = default;

    private SectionDTO[,] _sectionDTOs = default;

    private List<GameObject> _pool = new List<GameObject>();

    private List<GameObject> _useObjs = new List<GameObject>();

    public SectionDTO[,] SectionDTOs { set => _sectionDTOs = value; }

    /// <summary>
    /// 攻撃エリアに使用
    /// </summary>
    Vector2Int[] directions = {

            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
        };


    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKey(KeyCode.RightArrow))
        {

            this.transform.eulerAngles += (Vector3.up * _towerRotateSpeed) * Time.deltaTime;

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {

            this.transform.eulerAngles += (Vector3.up * -_towerRotateSpeed) * Time.deltaTime;

        }

        // カメラ回転
        Vector3 mousePos = Input.mousePosition;

        // カメラ回転--------------------------------------------------
        float screenWidth = Screen.width;

        float rotationY = 0f;

        // 左端
        if (mousePos.x <= _edgeSize)
        {
            rotationY = -1f;
        }
        // 右端
        else if (mousePos.x >= screenWidth - _edgeSize)
        {
            rotationY = 1f;
        }

        // 実際に回転させる
        if (rotationY != 0f)
        {
            _camera.transform.Rotate(Vector3.up, rotationY * _cameraRotateSpeed * Time.deltaTime);
        }

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            if (hit.collider != _oldObj)
            {

                _oldObj = hit.collider.gameObject;

            }

            // Tower回転--------------------------------------------------------
            Vector3 lookPos = hit.point - this.transform.position;
            lookPos.y = 0; // 水平面に制限

            if (lookPos != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookPos);
                this.transform.rotation = Quaternion.Slerp(
                    this.transform.rotation,
                    targetRotation,
                    _towerRotateSpeed * Time.deltaTime
                );
            }

          
            // 攻撃アリアブロックの生成プール---------------------------------------------------------------

            SectionDTO sectionDTO = null;
            for (int x = 0; x < _sectionDTOs.GetLength(0); x++)
            {
                for (int y = 0; y < _sectionDTOs.GetLength(1); y++)
                {
                    var dto = _sectionDTOs[x, y];
                    if (dto != null && dto.Obj == hit.collider.gameObject)
                    {
                        sectionDTO = dto;
                        break;
                    }
                }
                if (sectionDTO != null) break;
            }

            // null チェックを必ず行う
            if (sectionDTO == null || sectionDTO.Obj == null)
            {
                return;
            }

            List<GameObject> attackAreaObjs = GetArrackAreaObj(sectionDTO.Obj);
            foreach (GameObject obj in attackAreaObjs)
            {

                Vector3 instancePos = obj.transform.position + Vector3.up * (obj.transform.localScale.y+1) ;
                GameObject instanceObj = default;
                if(_pool.Count > 0)
                {

                    instanceObj = _pool[0];
                    instanceObj.SetActive(true);
                    _pool.Remove(instanceObj);
                    
                }
                else
                {

                    instanceObj = Instantiate(_attackArea);

                }
                instanceObj.transform.position = instancePos;
                _useObjs.Add(instanceObj);

            }
            

        }

        if (Input.GetMouseButtonDown(0)) // 左クリック
        {

            foreach (GameObject obj in _useObjs)
            {

                Debug.Log("攻撃インターフェース");
                IAttack iAttack = obj.GetComponent<IAttack>();
                if(iAttack != null)
                {

                    iAttack.Attack(_pow);

                }

            }

        }

        // 回収
        for (int i = 0; i < _useObjs.Count(); i++)
        {

            GameObject useObj = _useObjs[0];
            useObj.SetActive(false);
            _useObjs.Remove(useObj);
            _pool.Add(useObj);

        }
       // Debug.Break();
    }

    /// <summary>
    /// 攻撃するエリアのオブジェクトを取得
    /// </summary>
    /// <returns></returns>

    private List<GameObject> GetArrackAreaObj(GameObject obj)
    {

        List<GameObject> attackAreaObjs = new List<GameObject>();
        attackAreaObjs.Add(obj);

        int arrayXMinAbs = Mathf.RoundToInt(_sectionDTOs[0, 0].Pos.x);
        arrayXMinAbs = Mathf.Abs(arrayXMinAbs);
        // 配列"Z"軸の最小値の絶対値
        int arrayZMinAbs = Mathf.RoundToInt(_sectionDTOs[0, 0].Pos.z);
        arrayZMinAbs = Mathf.Abs(arrayZMinAbs);
        int x = Mathf.RoundToInt(obj.transform.position.x);
        int z = Mathf.RoundToInt(obj.transform.position.z);
        // インデックスに対応させるため最小値の絶対値を足す
        x += arrayXMinAbs;
        z += arrayZMinAbs;
        Vector2Int pos = new Vector2Int(x, z);
        foreach (Vector2Int dir in directions)
        {

            Vector2Int newPos = pos + dir;
            if (newPos.x >= 0 && newPos.x < _sectionDTOs.GetLength(0) &&
                newPos.y >= 0 && newPos.y < _sectionDTOs.GetLength(1))
            {

                if (_sectionDTOs[newPos.x, newPos.y] == null)
                {

                    continue;

                }
                attackAreaObjs.Add(_sectionDTOs[newPos.x, newPos.y].Obj);

            }

        }
        return attackAreaObjs;

    }
}
