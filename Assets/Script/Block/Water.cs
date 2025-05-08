using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WaterMove : MonoBehaviour
{

    private System.Random ramdom = new System.Random();
    private Animator _animator = default;
    private Material _material = default;

    private float _maxHeight = 10f;

    // Start is called before the first frame update
    private void Start()
    {

        GetCompoment();
        AdjustMaterialAlpha();
        StartAnima();

    }

    /// <summary>
    /// コンポーネント獲得
    /// </summary>
    private void GetCompoment()
    {

        _animator = this.GetComponent<Animator>();
        _material = this.GetComponent<Renderer>().material;

    }

    private void AdjustMaterialAlpha()
    {

        Color color = _material.color;
        // 高いほど薄くするため反転
        float alpha = 1 - (transform.localScale.y / _maxHeight);
        color.a = alpha;
        _material.color = color;

    }

    /// <summary>
    /// アニメーションを開始する
    /// </summary>
    private async void StartAnima()
    {

        int startTime = ramdom.Next(3000);
        await Task.Delay(startTime);
        _animator.enabled = true;

    }


}
