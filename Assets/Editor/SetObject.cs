using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
/// <summary>
/// オブジェクトを配置するためのツール
/// 湯元
/// 2025/4/20
/// </summary>
public class SetObject : EditorWindow
{

    [SerializeField] private VisualTreeAsset _rootVisualTreeAsset;
    [SerializeField] private StyleSheet _rootStyleSheet;

    /// <summary>
    /// クリックモードで生成したオブジェクト
    /// </summary>
    private List<GameObject> _clickInstanceObjs = new List<GameObject>();

    /// <summary>
    /// コピーするオブジェクト
    /// </summary>
    private GameObject _copyObj = default;

    /// <summary>
    /// イベント
    /// </summary>
    private static Event _e = default;

    /// <summary>
    /// クリック中かの判定
    /// </summary>
    private bool _hasClick = false;
    /// <summary>
    /// クリックモードのフラグ
    /// </summary>
    private bool _isClickMode = false;


    [MenuItem("SetObject/オブジェクト設置")]
    private static void ShowWindow()
    {
        var window = GetWindow<SetObject>("UIElements");
        window.titleContent = new GUIContent("SetObject");
        window.Show();

    }

    private void CreateGUI()
    {

        _rootVisualTreeAsset.CloneTree(rootVisualElement);
        rootVisualElement.styleSheets.Add(_rootStyleSheet);
        var objectField = new ObjectField("Select a GameObject")
        {
            objectType = typeof(GameObject),
            allowSceneObjects = false // シーン内のオブジェクトを不許可
        };
        InitialClickMode();
        SceneView.duringSceneGui += OnSceneGUI;
        InitialWallFix();
        InitialGroundChange();
        InitialChangeBlock();

    }

    /// <summary>
    /// クリックモードボタンを押したときの仕込み
    /// </summary>
    private void InitialClickMode()
    {

        // ボタンを取得し押されたとき用に関数を登録
        Button clickButton = (Button)rootVisualElement.Q<Button>("ClickModeButton");
        clickButton.clicked += () =>
        {

            GameObject oldCopyObj = _copyObj;
            _copyObj = (GameObject)rootVisualElement.Q<ObjectField>("CopyObject").value;
            if ((_copyObj != oldCopyObj || !_isClickMode) && _copyObj != null)
            {

                rootVisualElement.Q<Label>("ClickModeRunning").text = "<color=green>稼動中</color>";

                _isClickMode = true;
                return;
            }
            rootVisualElement.Q<Label>("ClickModeRunning").text = "<color=red>停止中</color>";
            _isClickMode = false;

        };

    }

    /// <summary>
    /// エディター拡張を開いているときに継続的に起動
    /// </summary>
    /// <param name="sceneView"></param>
    private void OnSceneGUI(SceneView sceneView)
    {

        ClickMode();

    }

    /// <summary>
    /// Clickした箇所に条件通りにブロックを配置
    /// </summary>
    /// <param name="isRunning"></param>
    private void ClickMode()
    {

        if (!_isClickMode)
        {

            return;

        }
        // イベント取得しそれらを条件に検査
        _e = Event.current;
        // 左のボタンを押した瞬間
        if (_e.type == EventType.MouseDown && _e.button == 0)
        {

            _hasClick = !_hasClick;

        }
        if (_e.type == EventType.MouseLeaveWindow)
        {

            _hasClick = false;

        }
        if (!_hasClick)
        {

            _clickInstanceObjs.Clear();
            return;

        }
        Debug.Log("-------------------------------------------------------------------------------");
        // 設置個数を取得
        int setCount = rootVisualElement.Q<IntegerField>("SetCount").value;
        // マウスカーソルの位置からRayを打つ
        Ray ray = HandleUtility.GUIPointToWorldRay(_e.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            // 1つ前に生成したオブジェクトの時処理中断
            foreach (GameObject obj in _clickInstanceObjs)
            {

                if (obj == hit.collider.gameObject)
                {

                    Debug.Log("１つ前の生成したオブジェクトにあたる");
                    return;

                }

            }
            // ヒットした位置と法線を取得
            Vector3 hitPoint = hit.point;
            Vector3 normal = hit.normal;
            // 生成して処理をし終わったオブジェクトを入れる
            Transform oldInstanceTrans = default;
            if (_copyObj == null)
            {

                return;

            }
            // 初期化
            _clickInstanceObjs.Clear();
            // オブジェクトをクリックした位置に条件分生成
            for (int i = 1; i <= setCount; i++)
            {

                GameObject instance = PrefabUtility.InstantiatePrefab(_copyObj) as GameObject;
                if (instance == null)
                {

                    return;

                }
                Vector3 pos = hit.collider.transform.position;
                Vector3 scale = hit.collider.transform.localScale;
                if (oldInstanceTrans != null)
                {

                    pos = oldInstanceTrans.position;
                    scale = oldInstanceTrans.localScale;

                }
                // ノーマライズ方向のサイズのみ残す
                scale.x *= normal.x;
                scale.y *= normal.y;
                scale.z *= normal.z;
                // スケールの２倍動けば隣に設置できるため
                pos += scale * 2;
                instance.transform.position = pos;
                Undo.RegisterCreatedObjectUndo(instance, "Place Prefab");
                _clickInstanceObjs.Add(instance);
                oldInstanceTrans = instance.transform;

            }

        }

    }

    /// <summary>
    /// 整えるボタンを押したときの仕込み
    /// </summary>
    private void InitialWallFix()
    {

        // ボタンを取得し押されたとき用に関数を登録
        Button clickButton = (Button)rootVisualElement.Q<Button>("FixButton");
        clickButton.clicked += () =>
        {

            WallFix();

        };

    }

    /// <summary>
    ///壁を均して隙間を埋める処理
    /// </summary>
    private void WallFix()
    {

        List<GameObject> objs = new List<GameObject>();
        foreach (var objct in Selection.objects)
        {
            if (!(objct is GameObject obj))
            {

                continue;

            }
            objs.Add(obj);
        }
        for (int i = 0; i < objs.Count; i++)
        {

            Undo.RecordObject(objs[i].transform, "FixObj");
            Vector3 pos = objs[i].transform.localPosition;
            // 半分
            float yPos = pos.y;
            // 高さを0にして代入
            pos.y = 0;
            objs[i].transform.localPosition = pos;

            Vector3 scale = objs[i].transform.localScale;
            // 高さを幅に変換
            scale.y = yPos / 2 + 1;
            Undo.RecordObject(objs[i].transform, "FixObj");
            objs[i].transform.localScale = scale;

        }

    }

    /// <summary>
    /// 地面変更ボタンを押したときの仕込み
    /// </summary>
    private void InitialGroundChange()
    {

        // ボタンを取得し押されたとき用に関数を登録
        Button clickButton = rootVisualElement.Q<Button>("GroundChangeButton");
        clickButton.clicked += () =>
        {

            GroundChange();
            WallFix();

        };


    }

    /// <summary>
    /// 地面の状態を変更
    /// </summary>
    private void GroundChange()
    {

        // 最大の高さ
        SliderInt maxHeight = rootVisualElement.Q<SliderInt>("MaxHeight");
        // 斜傾（ブロック〇個分）
        Slider slope = rootVisualElement.Q<Slider>("Slope");
        // マップ生成
        GameObject[,] map = CreatMap();
        if (map == null)
        {

            return;

        }
        //同じマップにならないようにシード生成
        float seedX = Random.value * 100f;
        float seedZ = Random.value * 100f;
        // 高さの調整、条件次第でオブジェクトの変更をかける
        // 行
        for (int i = 0; i < map.GetLength(0); i++)
        {

            //列
            for (int j = 0; j < map.GetLength(1); j++)
            {

                if (map[i, j] == null)
                {

                    continue;

                }
                // 補正した高さを戻す
                Vector3 pos = map[i, j].transform.localPosition;
                float noiseMaterialX = (pos.x + seedX) / slope.value;
                float noiseMaterialZ = (pos.z + seedZ) / slope.value;
                // ノイズを使用し高さを出す(0~1)
                float noise = Mathf.PerlinNoise(noiseMaterialX, noiseMaterialZ);
                // この値以下は平らにする
                float cutoff = 0.3f;
                noise = noise < cutoff ? 0 : (noise - cutoff) / (1 - cutoff) * maxHeight.value;
                // ノイズ(0~1)に最大の高さをかけることで、高さを取得
                int height = (int)(noise * maxHeight.value);
                height = height % 2 == 0 ? height + 1 : height;

                pos.y = height;
                Undo.RecordObject(map[i, j].transform, "ChengeObj");
                map[i, j].transform.localPosition = pos;

            }

        }

    }

    /// <summary>
    /// ブロック変更ボタンを押されたときの仕込み
    /// </summary>
    private void InitialChangeBlock()
    {

        // ボタンを取得し押されたとき用に関数を登録
        Button clickButton = (Button)rootVisualElement.Q<Button>("BlockChangeButton");
        clickButton.clicked += () =>
        {

            _copyObj = (GameObject)rootVisualElement.Q<ObjectField>("CopyObject").value;
            ChangeBlock();

        };

    }

    /// <summary>
    /// ブロックを入れ替える
    /// </summary>
    private void ChangeBlock()
    {

        // マップ生成
        GameObject[,] map = CreatMap();
        if (map == null)
        {

            return;

        }
        // オブジェクト生成
        // 行
        for (int i = 0; i < map.GetLength(0); i++)
        {

            //列
            for (int j = 0; j < map.GetLength(1); j++)
            {

                if(_copyObj == null)
                {

                    return;

                }
                if (map[i, j] == null)
                {

                    continue;

                }
             
                // 生成した後にトランスフォームの情報を移す
                GameObject instance = PrefabUtility.InstantiatePrefab(_copyObj) as GameObject;
                Undo.RegisterCreatedObjectUndo(instance, "ChengeBlock");
                Transform oldTrams = map[i, j].transform;
                instance.transform.position = oldTrams.position;
                instance.transform.rotation = oldTrams.rotation;
                instance.transform.localScale = oldTrams.localScale;


            }

        }
        // いらなくなったオブジェクトを削除
        // 行
        for (int i = 0;map.GetLength(0) > 0; i++)
        {

            //列
            while (map.GetLength(1) > 0)
            {

                GameObject obj = map[i, 0];
                map.RemoveAll(l => l.Count == 0);
                if (obj == null)
                {

                    continue;

                }
                Undo.RecordObject(map[i, 0].transform, "ChengeBlock");
                DestroyImmediate(map[i, 0]);

            }

        }
    }

    /// <summary>
    /// 地面（ステージ上のブロック）を取得しマップを生成する
    /// </summary>
    /// <returns></returns>
    private GameObject[,] CreatMap()
    {

        // 選択中のブロックを取得
        List<GameObject> objs = new List<GameObject>();
        foreach (var objct in Selection.objects)
        {

            // ゲームオブジェクトのみ抜き出す
            if (!(objct is GameObject obj))
            {

                continue;

            }
            objs.Add(obj);

        }
        if (objs.Count == 0)
        {

            return null;

        }
        // まず最大と最小サイズを見積もる（幅・奥行き）
        float maxX = float.MinValue;
        float maxZ = float.MinValue;
        float minX = float.MaxValue;
        float minZ = float.MaxValue;
        // 地面を平らにしながらサイズを見積もる
        for (int i = 0; i < objs.Count; i++)
        {

            if (objs[i].transform.position.x > maxX)
            {

                maxX = objs[i].transform.position.x;

            }
            if (objs[i].transform.position.z > maxZ)
            {

                maxZ = objs[i].transform.position.z;

            }
            if (objs[i].transform.position.x < minX)
            {

                minX = objs[i].transform.position.x;

            }
            if (objs[i].transform.position.z < minZ)
            {

                minZ = objs[i].transform.position.z;

            }
            objs[i].transform.localScale = Vector3.one;
            Vector3 pos = objs[i].transform.position;
            pos.y = 0;
            objs[i].transform.position = pos;

        }
        // X軸のブロックの個数
        int widthCount = Mathf.CeilToInt(maxX - minX) + 1;
        // Z軸のブロックの個数
        int depthCount = Mathf.CeilToInt(maxZ - minZ) + 1;
        // リスト登録
        GameObject[,] map = new GameObject[widthCount, depthCount];
        for (int i = 0; i < objs.Count; i++)
        {

            int x = Mathf.CeilToInt(objs[i].transform.position.x - minX);
            int z = Mathf.CeilToInt(objs[i].transform.position.z - minZ);
            map[x, z] = objs[i];

        }
        return map;

    }

}
