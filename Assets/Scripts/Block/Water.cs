using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WaterMove : MonoBehaviour
{

    [SerializeField,Range(0,1),Header("���ފ���")]
    private float _subsidenceRate = 0.3f;
    [SerializeField, Header("���s�b��")]
    private float _executionSeconds = 2f;

    private System.Random ramdom = new System.Random();
    private Material _material = default;

    Vector3 _myScale = Vector3.one;

    private float _maxHeight = 30f;

    // Start is called before the first frame update
    private void Start()
    {

        _myScale = this.transform.localScale;
        GetCompoment();
        AdjustMaterialAlpha();

    }

    /// <summary>
    /// �R���|�[�l���g�l��
    /// </summary>
    private void GetCompoment()
    {

        _material = this.GetComponent<Renderer>().sharedMaterial;

    }

    private void AdjustMaterialAlpha()
    {

        Color color = _material.color;
        // �����قǔ������邽�ߔ��]
        float alpha = 1 - (_myScale.y / _maxHeight);
        color.a = alpha;
        _material.color = color;

    }

}
