using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackArea : MonoBehaviour,IAttack
{
    [SerializeField]
    // 当たりたいレイヤーだけに制限する
    public LayerMask hitLayers; 

    public void Attack(int damage)
    {

        Debug.Log("攻撃");
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null) return;

        Vector3 boxSize = Vector3.Scale(boxCollider.size, transform.lossyScale) / 2f;
        Vector3 boxCenter = transform.position + transform.rotation * Vector3.Scale(boxCollider.center, transform.lossyScale);

        RaycastHit hit;
        float castDistance = 1.0f;
        if (Physics.BoxCast(boxCenter, boxSize, transform.forward, out hit, transform.rotation, castDistance, hitLayers))
        {
            // インターフェースを取得
            ITakeDamage myInterface = hit.collider.GetComponent<ITakeDamage>();
            if (myInterface != null)
            {
                myInterface.TakeDamage(damage);
            }
            else
            {
                Debug.Log("インターフェースを実装していないオブジェクトにヒット。");
            }
        }
        else
        {
            Debug.Log("何もヒットしませんでした。");
        }


    }



}
