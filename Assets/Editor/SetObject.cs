using System;
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
    /// クリックし終わったオブジェクト
    /// </summary>
    private GameObject _oldClickObj = default;
    /// <summary>
    /// クリックモードで生成し終わったオブジェクト
    /// </summary>
    private List<GameObject> _oldInstanceObjs = new List<GameObject>();

    /// <summary>
    /// 使用するオブジェクト
    /// </summary>
    private GameObject _useObj = default;
    /// <summary>
    /// 使用するオブジェクトの親オブジェクト
    /// </summary>
    private GameObject _useParentObj = default;

    /// <summary>
    /// イベント
    /// </summary>
    private static Event _event = default;

    /// <summary>
    /// クリックしたかのフラグ
    /// </summary>
    private bool _hasClick = false;
    /// <summary>
    /// クリックモードになっているかのフラグ
    /// </summary>
    private bool _isClickMode = false;

    // セーブの間隔
   private int _saveCount = 5;


    [MenuItem("CreateBoxMap/オブジェクト設置")]
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
        InitialSetTop();
        InitialSnap();
        InitialFixTop();
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

            GameObject oldCopyObj = _useObj;
            _useObj = (GameObject)rootVisualElement.Q<ObjectField>("UseObject").value;
            if ((_useObj != oldCopyObj || !_isClickMode))
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
        _event = Event.current;
        // マウスの左のボタンを押した瞬間
        if (_event.type == EventType.MouseDown && _event.button == 0)
        {

            _hasClick = !_hasClick;

        }
        // Scene画面外にカーソルが出たとき
        if (_event.type == EventType.MouseLeaveWindow)
        {

            _hasClick = false;

        }
        if (!_hasClick)
        {

            _oldClickObj = null;
            _oldInstanceObjs.Clear();
            return;

        }


        int setCount = rootVisualElement.Q<SliderInt>("SetCount1").value;
        bool isUpBuildinglimitation = rootVisualElement.Q<Toggle>("IsUpBuildinglimitation").value;
        bool isUseParentObject = rootVisualElement.Q<Toggle>("IsUseParentObject1").value;
        if (!IsCheckSerializeInObject(isUseParentObject))
        {

            _hasClick = false;
            return;

        }
        // マウスカーソルの位置からRayを打つ
        Ray ray = HandleUtility.GUIPointToWorldRay(_event.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            if (_oldClickObj == hit.collider.gameObject)
            {

                return;

            }
            // 1つ前に生成したオブジェクトの場合に処理を中断
            foreach (GameObject obj in _oldInstanceObjs)
            {

                if (obj == hit.collider.gameObject)
                {

                    return;

                }

            }
            // 法線を取得
            Vector3 normal = hit.normal;
            // ブロックの上に配置しない場合
            if (isUpBuildinglimitation)
            {

                if (normal == Vector3.up)
                {

                    return;

                }

            }
            Debug.Log("更新");
            _oldInstanceObjs.Clear();
            _oldClickObj = hit.collider.gameObject;
            // 生成処理が終わったオブジェクトを入れる
            Transform oldInstanceTrans = default;
            // グループID取得
            int group = Undo.GetCurrentGroup();
            // オブジェクトをクリックした位置に条件分生成
            for (int i = 1; i <= setCount; i++)
            {

               
                GameObject instance = PrefabUtility.InstantiatePrefab(_useObj) as GameObject;
                if (instance == null)
                {

                    return;

                }
                _saveCount--;
                Undo.RegisterCreatedObjectUndo(instance, "Place Prefab");
                // 5回ごとに Undo グループを開始
                if (_saveCount == 0 )
                {

                    Debug.Log("セーブ");
                    _saveCount = 5;
                    // 名前付けをここで行う
                    Undo.SetCurrentGroupName($"Place Prefab Group {(i - 1) / 5 + 1}");
                    // まとめる
                    Undo.CollapseUndoOperations(group);
                    // 次のグループへ
                    Undo.IncrementCurrentGroup();
                    // 新しいグループIDを取得
                    group = Undo.GetCurrentGroup();

                }
                Vector3 pos = hit.collider.transform.localPosition;
                Vector3 scale = hit.collider.transform.localScale;
                // 今回のループ中に生成したことがあるとき
                if (oldInstanceTrans != null)
                {

                    pos = oldInstanceTrans.localPosition;
                    scale = oldInstanceTrans.localScale;

                }
                // 法線方向のサイズのみ残す
                scale.x *= normal.x;
                scale.y *= normal.y;
                scale.z *= normal.z;
                // スケールの２倍動けば隣に設置できるため
                pos += scale;
                instance.name += "Clone";
                instance.transform.localPosition = pos;
                instance.transform.parent = _useParentObj == null ? null : _useParentObj.transform;
                _oldInstanceObjs.Add(instance);
                oldInstanceTrans = instance.transform;

            }

        }

    }



    /// <summary>
    /// 上部に実行ボタンををした時の仕込み
    /// </summary>
    private void InitialSetTop()
    {

        Button clickButton = (Button)rootVisualElement.Q<Button>("SetTopButton");
        clickButton.clicked += () =>
        {

            SetTop();

        };

    }

    private void SetTop()
    {

        int setCount = rootVisualElement.Q<SliderInt>("SetCount2").value;
        bool isUseParentObject = rootVisualElement.Q<Toggle>("IsUseParentObject2").value;
        bool isRemoval = rootVisualElement.Q<Toggle>("IsRemoval").value;

        // 設置
        if (!isRemoval)
        {

            if (!IsCheckSerializeInObject(isUseParentObject))
            {

                return;

            }
            List<GameObject> objs = new List<GameObject>();
            foreach (var objct in Selection.objects)
            {
                if (!(objct is GameObject obj))
                {

                    continue;

                }
                objs.Add(obj);
            }
            foreach (GameObject obj in objs)
            {

                // 生成処理が終わったオブジェクトを入れる
                Transform oldInstanceTrans = default;
                for (int i = 0; i < setCount; i++)
                {

                    GameObject instance = PrefabUtility.InstantiatePrefab(_useObj) as GameObject;
                    if (instance == null)
                    {

                        return;

                    }
                    Undo.RegisterCreatedObjectUndo(instance, "Place Prefab");
                    // オブジェクトの設置位置を取得
                    Vector3 pos = obj.transform.localPosition;
                    Vector3 scale = obj.transform.localScale;
                    if (oldInstanceTrans != null)
                    {

                        pos = oldInstanceTrans.localPosition;
                        scale = oldInstanceTrans.localScale;

                    }
                    pos.y += scale.y;
                    instance.name += "Clone";
                    instance.transform.localPosition = pos;
                    instance.transform.parent = _useParentObj == null ? null : _useParentObj.transform;
                    oldInstanceTrans = instance.transform;
                }

            }


        }
        // 撤去
        else
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
            foreach (GameObject obj in objs)
            {

                if (obj == null)
                {

                    continue;

                }
                for (int i = 0; i < setCount; i++)
                {

                    Vector3 pos = obj.transform.localPosition;
                    Vector3 scale = obj.transform.localScale;

                    Ray ray = new Ray(pos + Vector3.up * scale.y, Vector3.up);
                    RaycastHit hit = default;
                    if (!Physics.Raycast(ray, out hit))
                    {

                        if (obj == hit.collider)
                        {

                            Debug.LogError("自ブロックに触れた");
                            continue;

                        }
                        break;

                    }
                    Debug.Log("撤去中");
                    Undo.DestroyObjectImmediate(hit.collider.gameObject);

                }

            }

        }


    }



    /// <summary>
    /// 地面変更ボタンを押したときの仕込み
    /// </summary>
    private void InitialGroundChange()
    {

        // ボタンを取得し押されたとき用に関数を登録
        Button adjustmentButton = rootVisualElement.Q<Button>("GroundAdjustmentButton");
        adjustmentButton.clicked += () =>
        {

            GroundChange();
            SnapFix();

        };


    }

    /// <summary>
    /// 地面の状態を変更
    /// </summary>
    private void GroundChange()
    {

        // 最大の高さ
        int maxHeight = rootVisualElement.Q<SliderInt>("MaxHeight").value;
        // 最小の高さ
        int minHeight = rootVisualElement.Q<SliderInt>("MinHeight").value;
        // 滑らかさ
        float slope = rootVisualElement.Q<Slider>("Slope").value;
        // マップ生成
        List<List<GameObject>> map = CreatMap();
        if (map == null)
        {

            return;

        }
        //同じマップにならないようにシード生成
        float seedX = UnityEngine.Random.value * 1000f;
        float seedZ = UnityEngine.Random.value * 1000f;
        // 高さの調整、条件次第でオブジェクトの変更をかける
        // 行
        for (int i = 0; i < map.Count; i++)
        {

            for (int j = 0; j < map[i].Count; j++)
            {

                if (map[i][j] == null)
                {

                    continue;

                }
                Vector3 pos = map[i][j].transform.localPosition;
                float x = (pos.x + seedX) / slope;
                float z = (pos.z + seedZ) / slope;
                // 現在の位置のノイズ値
                float noise = Mathf.PerlinNoise(x, z);
                noise *= 1.1f;
                noise = Math.Clamp(noise, 0, 1);
                // 最終的な高さを決定
                int height = Mathf.RoundToInt(noise * maxHeight);
                if (height < minHeight)
                {

                    height /= 2;
                    height += minHeight;

                }
                // サイズの高さの分１をひいておく
                height -= 1;
                pos.y = height;
                Undo.RecordObject(map[i][j].transform, "ChangeObj");
                map[i][j].transform.localPosition = pos;

            }

        }

    }



    /// <summary>
    /// 地面に底面を付けるボタンを押したときの仕込み
    /// </summary>
    private void InitialSnap()
    {

        // ボタンを取得し押されたとき用に関数を登録
        Button clickButton = (Button)rootVisualElement.Q<Button>("SnapButton");
        clickButton.clicked += () =>
        {

            SnapFix();

        };

    }

    /// <summary>
    ///壁を均して隙間を埋める処理
    /// </summary>
    private void SnapFix()
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
            Vector3 scale = objs[i].transform.localScale;
            // 高さを記憶
            float yPos = pos.y;
            // 高さを0にする
            pos.y = 0;
            objs[i].transform.localPosition = pos;
            // 縦の長さを１にする
            scale.y = 1;
            scale.y += yPos;
            objs[i].transform.localScale = scale;

        }

    }



    /// <summary>
    /// 上にそろえるボタンを押したときの仕込み
    /// </summary>
    private void InitialFixTop()
    {

        // ボタンを取得し押されたとき用に関数を登録
        Button clickButton = (Button)rootVisualElement.Q<Button>("FixTopButton");
        clickButton.clicked += () =>
        {

            Debug.Log("押した");
            FixTop();

        };

    }

    /// <summary>
    /// 一番高いブロックにそろえる
    /// </summary>
    private void FixTop()
    {

        // マップ生成
        List<List<GameObject>> map = CreatMap();
        if (map == null)
        {

            return;

        }
        float maxHeight = float.MinValue;
        // 最大の高さを見つける
        // 行
        for (int i = 0; i < map.Count; i++)
        {

            //列
            for (int j = 0; j < map[i].Count; j++)
            {

                if (map[i][j] == null)
                {

                    continue;

                }
                float height = map[i][j].transform.localPosition.y;
                // ブロックの通常時のサイズが１，１，１で、
                // 原点がブロックの下にあるため、高さを図るときに
                // ブロック1サイズ分が余剰にカウントされているため-１をする。
                height += (map[i][j].transform.localScale.y - 1);
                if (maxHeight < height)
                {

                    maxHeight = height;

                }

            }

        }

        // オブジェクト生成
        // 行
        for (int i = 0; i < map.Count; i++)
        {

            //列
            for (int j = 0; j < map[i].Count; j++)
            {

                if (map[i][j] == null)
                {

                    continue;

                }
                Undo.RecordObject(map[i][j], "TopFixBlock");

                float height = map[i][j].transform.position.y;
                // ブロックの通常時のサイズが１，１，１で、
                // 原点がブロックの下にあるため、高さを図るときに
                // ブロック1サイズ分が余剰にカウントされているため-１をする。
                height += (map[i][j].transform.localScale.y - 1);
                float addHeight = maxHeight - height;

                Vector3 scale = map[i][j].transform.localScale;
                scale.y += addHeight;
                map[i][j].transform.localScale = scale;

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

            _useObj = (GameObject)rootVisualElement.Q<ObjectField>("UseObject").value;
            ChangeBlock();

        };

    }

    /// <summary>
    /// ブロックを入れ替える
    /// </summary>
    private void ChangeBlock()
    {

        // マップ生成
        List<List<GameObject>> map = CreatMap();
        // 最大の高さ
        SliderInt minBorder = rootVisualElement.Q<SliderInt>("MinBorder");
        SliderInt maxBorder = rootVisualElement.Q<SliderInt>("MaxBorder");
        bool isUseParentObject = rootVisualElement.Q<Toggle>("IsUseParentObject3").value;
        // 変換するブロックの中で一番高い位置に合わせるかのフラグ
        bool isFixMaxHeight = rootVisualElement.Q<Toggle>("IsFixMaxHeight").value;
        if (map == null || !IsCheckSerializeInObject(isUseParentObject))
        {

            return;

        }
        float maxHeight = float.MinValue;
        // 最大の高さを見つける
        if (isFixMaxHeight)
        {

            // 行
            for (int i = 0; i < map.Count; i++)
            {

                //列
                for (int j = 0; j < map[i].Count; j++)
                {

                    if (_useObj == null)
                    {

                        return;

                    }
                    if (map[i][j] == null)
                    {

                        continue;

                    }
                    float height = map[i][j].transform.localPosition.y;
                    // ブロックの通常時のサイズが１，１，１で、
                    // 原点がブロックの下にあるため、高さを図るときに
                    // ブロック1サイズ分が余剰にカウントされているため-１をする。
                    height += (map[i][j].transform.localScale.y - 1);
                    if (maxHeight < height)
                    {

                        maxHeight = height;

                    }

                }

            }

        }
        // オブジェクト生成
        // 行
        for (int i = 0; i < map.Count; i++)
        {

            //列
            for (int j = 0; j < map[i].Count; j++)
            {


                if (_useObj == null)
                {

                    return;

                }
                if (map[i][j] == null)
                {

                    continue;

                }
                Transform oldTrams = map[i][j].transform;
                if (oldTrams.localScale.y + oldTrams.position.y < minBorder.value || oldTrams.localScale.y + oldTrams.position.y > maxBorder.value)
                {

                    continue;

                }
                // 生成した後にトランスフォームの情報を移す
                GameObject instance = PrefabUtility.InstantiatePrefab(_useObj) as GameObject;
                Undo.RegisterCreatedObjectUndo(instance, "ChengeBlock");


                instance.transform.position = oldTrams.position;
                instance.transform.rotation = oldTrams.rotation;
                instance.transform.localScale = oldTrams.localScale;
                // 一番高い位置にそろえる
                if (isFixMaxHeight)
                {

                    float height = instance.transform.position.y;
                    // ブロックの通常時のサイズが１，１，１で、
                    // 原点がブロックの下にあるため、高さを図るときに
                    // ブロック1サイズ分が余剰にカウントされているため-１をする。
                    height += (instance.transform.localScale.y - 1);
                    float addHeight = maxHeight - height;

                    Vector3 scale = instance.transform.localScale;
                    scale.y += addHeight;
                    instance.transform.localScale = scale;

                }
                instance.name += "Clone";
                instance.transform.parent = _useParentObj == null ? null : _useParentObj.transform;

            }

        }
        // いらなくなったオブジェクトを削除
        // 行
        while (map.Count >= 1)
        {

            //列
            while (map[0].Count >= 1)
            {

                GameObject obj = map[0][0];
                map[0].RemoveAt(0);
                if (obj == null)
                {

                    continue;

                }
                if (obj.transform.localScale.y < minBorder.value || obj.transform.localScale.y > maxBorder.value)
                {

                    continue;

                }
                Undo.DestroyObjectImmediate(obj.transform);
                DestroyImmediate(obj);

            }
            map.RemoveAt(0);

        }
    }



    /// <summary>
    /// オブジェクトがセットされているかの確認
    /// </summary>
    /// <param name="isUseParent"></param>
    /// <returns></returns>
    private bool IsCheckSerializeInObject(bool isUseParent)
    {

        _useObj = (GameObject)rootVisualElement.Q<ObjectField>("UseObject").value;
        _useParentObj = (GameObject)rootVisualElement.Q<ObjectField>("UseParentObject").value;
        if (_useObj == null)
        {

            // 警告メッセージを表示
            EditorUtility.DisplayDialog("お知らせ", "使用するオブジェクトが設定されていません。", "OK");
            return false;

        }
        if (_useParentObj == null && isUseParent)
        {

            // 警告メッセージを表示
            EditorUtility.DisplayDialog("お知らせ", "使用する \"親\" オブジェクトが設定されていません。", "OK");
            return false;

        }
        // 使用する場合_useParentObjを切り替える
        if (isUseParent)
        {

            _useParentObj = (GameObject)rootVisualElement.Q<ObjectField>("UseParentObject").value;
            // 親オブジェクトがプレハブ等だった場合
            if (PrefabUtility.GetPrefabAssetType(_useParentObj) != PrefabAssetType.NotAPrefab)
            {

                // 警告メッセージを表示
                EditorUtility.DisplayDialog("お知らせ", "使用する \"親\" オブジェクトが \"ヒエラルキー上から\" 設定されていません。\nプレハブ等からの設定はできません。ご確認してください", "OK");

                return false;

            }

        }
        else
        {

            _useParentObj = null;

        }

        return true;

    }

    /// <summary>
    /// 地面（ステージ上のブロック）を取得しマップを生成する
    /// </summary>
    /// <returns></returns>
    private List<List<GameObject>> CreatMap()
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
        // 地面のサイズを見積もる
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

        }
        // X軸のブロックの個数
        int widthCount = Mathf.CeilToInt(maxX - minX);
        // Z軸のブロックの個数
        int depthCount = Mathf.CeilToInt(maxZ - minZ);
        // リスト登録
        List<List<GameObject>> map = new List<List<GameObject>>();
        for (int i = 0; i <= widthCount; i++)
        {

            map.Add(new List<GameObject>());
            for (int j = 0; j <= depthCount; j++)
            {

                map[i].Add(null);

            }

        }
        for (int i = 0; i < objs.Count; i++)
        {

            int x = Mathf.CeilToInt(objs[i].transform.position.x - minX);
            int z = Mathf.CeilToInt(objs[i].transform.position.z - minZ);
            map[x][z] = objs[i];

        }
        return map;

    }

}
