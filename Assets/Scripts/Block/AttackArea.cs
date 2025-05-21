using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackArea : MonoBehaviour,IAttack
{
    [SerializeField]
    // �����肽�����C���[�����ɐ�������
    public LayerMask hitLayers; 

    public void Attack(int damage)
    {

        Debug.Log("�U��");
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null) return;

        Vector3 boxSize = Vector3.Scale(boxCollider.size, transform.lossyScale) / 2f;
        Vector3 boxCenter = transform.position + transform.rotation * Vector3.Scale(boxCollider.center, transform.lossyScale);

        RaycastHit hit;
        float castDistance = 1.0f;
        if (Physics.BoxCast(boxCenter, boxSize, transform.forward, out hit, transform.rotation, castDistance, hitLayers))
        {
            // �C���^�[�t�F�[�X���擾
            ITakeDamage myInterface = hit.collider.GetComponent<ITakeDamage>();
            if (myInterface != null)
            {
                myInterface.TakeDamage(damage);
            }
            else
            {
                Debug.Log("�C���^�[�t�F�[�X���������Ă��Ȃ��I�u�W�F�N�g�Ƀq�b�g�B");
            }
        }
        else
        {
            Debug.Log("�����q�b�g���܂���ł����B");
        }


    }



}
