using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class EnemyBase : MonoBehaviour
{

    [SerializeField]
    protected EnemyStatusDTO _enemyStatusDTO = new EnemyStatusDTO();

    private static List<GameObject> _targetObjs = default;

    //protected IGetRoot _iGetRoot = new Root();

    protected List<Vector3> _root = new List<Vector3>(); 

    // Start is called before the first frame update
    protected async virtual void Start()
    {

        Debug.Log("�J�n");
        GetTarget();
        // ��ԋ߂��I�u�W�F�N�g���擾
        GameObject targetObj = _targetObjs.OrderBy(n => Vector3.Distance(this.transform.position,n.transform.position)).First();  
        if(targetObj != null)
        {

            //_root = await _iGetRoot.GetRoot(this.transform.position, targetObj.transform.position, _enemyStatusDTO.ClimbHeight);
            Move();

        }

    }
    
    /// <summary>
    /// �^�[�Q�b�g���擾
    /// </summary>
    private void GetTarget()
    {

        if(_targetObjs != null)
        {

            return;

        }
        // �w�肵���^�O�������ׂĂ�GameObject���擾
        _targetObjs = GameObject.FindGameObjectsWithTag("Target").ToList();

    }

    /// <summary>
    /// ���[�g�ړ�
    /// </summary>
    protected async void Move()
    {

        while (_targetObjs.Count > 0)
        {

            await Task.Delay(1000);
            this.transform.position = _root[0];
            _root.RemoveAt(0);

        }

    }

}
