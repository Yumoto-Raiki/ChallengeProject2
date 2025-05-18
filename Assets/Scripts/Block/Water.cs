using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WaterMove : MonoBehaviour
{

    [SerializeField,Range(0,1),Header("沈む割合")]
    private float _subsidenceRate = 0.3f;
    [SerializeField, Header("実行秒数")]
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
    /// コンポーネント獲得
    /// </summary>
    private void GetCompoment()
    {

        _material = this.GetComponent<Renderer>().sharedMaterial;

    }

    private void AdjustMaterialAlpha()
    {

        Color color = _material.color;
        // 高いほど薄くするため反転
        float alpha = 1 - (_myScale.y / _maxHeight);
        color.a = alpha;
        _material.color = color;

    }

}
