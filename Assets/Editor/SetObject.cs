using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    /// コピーするオブジェクト
    /// </summary>
    private GameObject _copyObj = default;

    /// <summary>
    /// イベント
    /// </summary>
    private static Event _e = default;

    /// <summary>
    /// クリック下かの判定
    /// </summary>
    private bool _hasClick = false;
    /// <summary>
    /// クリックモードのフラグ
    /// </summary>
    private bool _isClickMode = false;

    private float _delayTime = 0.5f;
    private double _oldTime = 0f;

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
        WallFix();

    }

    private void OnSceneGUI(SceneView sceneView)
    {

        OnSetObject();

    }

    /// <summary>
    /// クリックモード起動・停止処理
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
    /// クリックモード
    /// </summary>
    /// <param name="isRunning"></param>
    private void OnSetObject()
    {

        if (!_isClickMode)
        {

            return;

        }
        Debug.Log(_isClickMode);
        // イベント取得しそれらを条件に検査
        _e = Event.current;
        // 左のボタンを押した瞬間
        if (_e.type == EventType.MouseDown && _e.button == 0)
        {

            _hasClick = !_hasClick;

        }
        if(_e.type == EventType.MouseLeaveWindow)
        {

            _hasClick = false;

        }
        if (!_hasClick)
        {

            return;

        }
        // 左ボタンを押した瞬間ではないまたはEventがNullまたはALTを押しながらではないまたはタイマーがタイムより小さい
        if (EditorApplication.timeSinceStartup - _oldTime <= _delayTime)
        {


            return;

        }
        _oldTime = EditorApplication.timeSinceStartup;
        // 設置個数を取得
        int setCount = rootVisualElement.Q<IntegerField>("SetCount").value;
        // マウスカーソルの位置からRayを打つ
        Ray ray = HandleUtility.GUIPointToWorldRay(_e.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // ヒットした位置と法線を取得
            Vector3 hitPoint = hit.point;
            Vector3 normal = hit.normal;
            // 生成して処理をし終わったオブジェクトを入れる
            Transform oldInstanTrans = default;
            if (_copyObj != null)
            {

                for (int i = 1; i <= setCount; i++)
                {
                    GameObject instance = PrefabUtility.InstantiatePrefab(_copyObj) as GameObject;
                    if (instance != null)
                    {

                        Vector3 pos = hit.collider.transform.position;
                        Vector3 scale = hit.collider.transform.localScale;
                        if (oldInstanTrans != null)
                        {

                            pos = oldInstanTrans.position;
                            scale = oldInstanTrans.localScale;

                        }
                        // ノーマライズ方向のサイズのみ残す
                        scale.x *= normal.x;
                        scale.y *= normal.y;
                        scale.z *= normal.z;
                        // スケールの２倍動けば隣に設置できるため
                        pos += scale * 2;
                        instance.transform.position = pos;
                        Undo.RegisterCreatedObjectUndo(instance, "Place Prefab");
                        oldInstanTrans = instance.transform;

                    }

                }
            }
        }

    }

    /// <summary>
    /// フィルモード
    /// 穴にブロックを埋める処理
    /// </summary>
    private void OnFillObject()
    {



    }

    /// <summary>
    ///壁を均して隙間を埋める処理
    /// </summary>
    private void WallFix()
    {

        // ボタンを取得し押されたとき用に関数を登録
        Button clickButton = (Button)rootVisualElement.Q<Button>("FixButton");
        clickButton.clicked += () =>
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

                Vector3 pos = objs[i].transform.position;
                // 半分
                float yPos = pos.y;
                // 高さを0にして代入
                pos.y = 0;
                objs[i].transform.position = pos;

                Vector3 scale = objs[i].transform.localScale;
                // 高さを幅に変換
                scale.y = yPos / 2 + 1;
                objs[i].transform.localScale = scale;

            }

        };

    }

    /// <summary>
    /// 地面の状態を変更
    /// </summary>
    private void GroundChange()
    {

        // ボタンを取得し押されたとき用に関数を登録
        Button clickButton = rootVisualElement.Q<Button>("GroundChangeButton");
        clickButton.clicked += () =>
        {

            // 地面の基本の高さ
            Slider Height = rootVisualElement.Q<Slider>("GroundHeight");
            // 起伏
            Slider terrainRoughness = Height = rootVisualElement.Q<Slider>("TerrainRoughness");
            // 斜傾（ブロック〇個分）
            Slider slope = rootVisualElement.Q<Slider>("Slope");
            // ブロック変更
            Toggle isChangeBlock = rootVisualElement.Q<Toggle>("ChangeBlock");
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
            //変更をかけそれからブロック変更



        };


    }

}
