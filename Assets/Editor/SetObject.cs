using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
/// <summary>
/// �I�u�W�F�N�g��z�u���邽�߂̃c�[��
/// ����
/// 2025/4/20
/// </summary>
public class SetObject : EditorWindow
{

    [SerializeField] private VisualTreeAsset _rootVisualTreeAsset;
    [SerializeField] private StyleSheet _rootStyleSheet;

    /// <summary>
    /// �N���b�N���[�h�Ő��������I�u�W�F�N�g
    /// </summary>
    private List<GameObject> _clickInstanceObjs = new List<GameObject>();

    /// <summary>
    /// �R�s�[����I�u�W�F�N�g
    /// </summary>
    private GameObject _copyObj = default;

    /// <summary>
    /// �C�x���g
    /// </summary>
    private static Event _e = default;

    /// <summary>
    /// �N���b�N�����̔���
    /// </summary>
    private bool _hasClick = false;
    /// <summary>
    /// �N���b�N���[�h�̃t���O
    /// </summary>
    private bool _isClickMode = false;


    [MenuItem("SetObject/�I�u�W�F�N�g�ݒu")]
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
            allowSceneObjects = false // �V�[�����̃I�u�W�F�N�g��s����
        };
        InitialClickMode();
        SceneView.duringSceneGui += OnSceneGUI;
        InitialWallFix();
        InitialGroundChange();
        InitialChangeBlock();

    }

    /// <summary>
    /// �N���b�N���[�h�{�^�����������Ƃ��̎d����
    /// </summary>
    private void InitialClickMode()
    {

        // �{�^�����擾�������ꂽ�Ƃ��p�Ɋ֐���o�^
        Button clickButton = (Button)rootVisualElement.Q<Button>("ClickModeButton");
        clickButton.clicked += () =>
        {

            GameObject oldCopyObj = _copyObj;
            _copyObj = (GameObject)rootVisualElement.Q<ObjectField>("CopyObject").value;
            if ((_copyObj != oldCopyObj || !_isClickMode) && _copyObj != null)
            {

                rootVisualElement.Q<Label>("ClickModeRunning").text = "<color=green>�ғ���</color>";

                _isClickMode = true;
                return;
            }
            rootVisualElement.Q<Label>("ClickModeRunning").text = "<color=red>��~��</color>";
            _isClickMode = false;

        };

    }

    /// <summary>
    /// �G�f�B�^�[�g�����J���Ă���Ƃ��Ɍp���I�ɋN��
    /// </summary>
    /// <param name="sceneView"></param>
    private void OnSceneGUI(SceneView sceneView)
    {

        ClickMode();

    }

    /// <summary>
    /// Click�����ӏ��ɏ����ʂ�Ƀu���b�N��z�u
    /// </summary>
    /// <param name="isRunning"></param>
    private void ClickMode()
    {

        if (!_isClickMode)
        {

            return;

        }
        // �C�x���g�擾�������������Ɍ���
        _e = Event.current;
        // ���̃{�^�����������u��
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
        // �ݒu�����擾
        int setCount = rootVisualElement.Q<IntegerField>("SetCount").value;
        // �}�E�X�J�[�\���̈ʒu����Ray��ł�
        Ray ray = HandleUtility.GUIPointToWorldRay(_e.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            // 1�O�ɐ��������I�u�W�F�N�g�̎��������f
            foreach (GameObject obj in _clickInstanceObjs)
            {

                if (obj == hit.collider.gameObject)
                {

                    Debug.Log("�P�O�̐��������I�u�W�F�N�g�ɂ�����");
                    return;

                }

            }
            // �q�b�g�����ʒu�Ɩ@�����擾
            Vector3 hitPoint = hit.point;
            Vector3 normal = hit.normal;
            // �������ď��������I������I�u�W�F�N�g������
            Transform oldInstanceTrans = default;
            if (_copyObj == null)
            {

                return;

            }
            // ������
            _clickInstanceObjs.Clear();
            // �I�u�W�F�N�g���N���b�N�����ʒu�ɏ���������
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
                // �m�[�}���C�Y�����̃T�C�Y�̂ݎc��
                scale.x *= normal.x;
                scale.y *= normal.y;
                scale.z *= normal.z;
                // �X�P�[���̂Q�{�����Ηׂɐݒu�ł��邽��
                pos += scale * 2;
                instance.transform.position = pos;
                Undo.RegisterCreatedObjectUndo(instance, "Place Prefab");
                _clickInstanceObjs.Add(instance);
                oldInstanceTrans = instance.transform;

            }

        }

    }

    /// <summary>
    /// ������{�^�����������Ƃ��̎d����
    /// </summary>
    private void InitialWallFix()
    {

        // �{�^�����擾�������ꂽ�Ƃ��p�Ɋ֐���o�^
        Button clickButton = (Button)rootVisualElement.Q<Button>("FixButton");
        clickButton.clicked += () =>
        {

            WallFix();

        };

    }

    /// <summary>
    ///�ǂ��ς��Č��Ԃ𖄂߂鏈��
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
            // ����
            float yPos = pos.y;
            // ������0�ɂ��đ��
            pos.y = 0;
            objs[i].transform.localPosition = pos;

            Vector3 scale = objs[i].transform.localScale;
            // �����𕝂ɕϊ�
            scale.y = yPos / 2 + 1;
            Undo.RecordObject(objs[i].transform, "FixObj");
            objs[i].transform.localScale = scale;

        }

    }

    /// <summary>
    /// �n�ʕύX�{�^�����������Ƃ��̎d����
    /// </summary>
    private void InitialGroundChange()
    {

        // �{�^�����擾�������ꂽ�Ƃ��p�Ɋ֐���o�^
        Button clickButton = rootVisualElement.Q<Button>("GroundChangeButton");
        clickButton.clicked += () =>
        {

            GroundChange();
            WallFix();

        };


    }

    /// <summary>
    /// �n�ʂ̏�Ԃ�ύX
    /// </summary>
    private void GroundChange()
    {

        // �ő�̍���
        SliderInt maxHeight = rootVisualElement.Q<SliderInt>("MaxHeight");
        // �ΌX�i�u���b�N�Z���j
        Slider slope = rootVisualElement.Q<Slider>("Slope");
        // �}�b�v����
        GameObject[,] map = CreatMap();
        if (map == null)
        {

            return;

        }
        //�����}�b�v�ɂȂ�Ȃ��悤�ɃV�[�h����
        float seedX = Random.value * 100f;
        float seedZ = Random.value * 100f;
        // �����̒����A��������ŃI�u�W�F�N�g�̕ύX��������
        // �s
        for (int i = 0; i < map.GetLength(0); i++)
        {

            //��
            for (int j = 0; j < map.GetLength(1); j++)
            {

                if (map[i, j] == null)
                {

                    continue;

                }
                // �␳����������߂�
                Vector3 pos = map[i, j].transform.localPosition;
                float noiseMaterialX = (pos.x + seedX) / slope.value;
                float noiseMaterialZ = (pos.z + seedZ) / slope.value;
                // �m�C�Y���g�p���������o��(0~1)
                float noise = Mathf.PerlinNoise(noiseMaterialX, noiseMaterialZ);
                // ���̒l�ȉ��͕���ɂ���
                float cutoff = 0.3f;
                noise = noise < cutoff ? 0 : (noise - cutoff) / (1 - cutoff) * maxHeight.value;
                // �m�C�Y(0~1)�ɍő�̍����������邱�ƂŁA�������擾
                int height = (int)(noise * maxHeight.value);
                height = height % 2 == 0 ? height + 1 : height;

                pos.y = height;
                Undo.RecordObject(map[i, j].transform, "ChengeObj");
                map[i, j].transform.localPosition = pos;

            }

        }

    }

    /// <summary>
    /// �u���b�N�ύX�{�^���������ꂽ�Ƃ��̎d����
    /// </summary>
    private void InitialChangeBlock()
    {

        // �{�^�����擾�������ꂽ�Ƃ��p�Ɋ֐���o�^
        Button clickButton = (Button)rootVisualElement.Q<Button>("BlockChangeButton");
        clickButton.clicked += () =>
        {

            _copyObj = (GameObject)rootVisualElement.Q<ObjectField>("CopyObject").value;
            ChangeBlock();

        };

    }

    /// <summary>
    /// �u���b�N�����ւ���
    /// </summary>
    private void ChangeBlock()
    {

        // �}�b�v����
        GameObject[,] map = CreatMap();
        if (map == null)
        {

            return;

        }
        // �I�u�W�F�N�g����
        // �s
        for (int i = 0; i < map.GetLength(0); i++)
        {

            //��
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
             
                // ����������Ƀg�����X�t�H�[���̏����ڂ�
                GameObject instance = PrefabUtility.InstantiatePrefab(_copyObj) as GameObject;
                Undo.RegisterCreatedObjectUndo(instance, "ChengeBlock");
                Transform oldTrams = map[i, j].transform;
                instance.transform.position = oldTrams.position;
                instance.transform.rotation = oldTrams.rotation;
                instance.transform.localScale = oldTrams.localScale;


            }

        }
        // ����Ȃ��Ȃ����I�u�W�F�N�g���폜
        // �s
        for (int i = 0;map.GetLength(0) > 0; i++)
        {

            //��
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
    /// �n�ʁi�X�e�[�W��̃u���b�N�j���擾���}�b�v�𐶐�����
    /// </summary>
    /// <returns></returns>
    private GameObject[,] CreatMap()
    {

        // �I�𒆂̃u���b�N���擾
        List<GameObject> objs = new List<GameObject>();
        foreach (var objct in Selection.objects)
        {

            // �Q�[���I�u�W�F�N�g�̂ݔ����o��
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
        // �܂��ő�ƍŏ��T�C�Y�����ς���i���E���s���j
        float maxX = float.MinValue;
        float maxZ = float.MinValue;
        float minX = float.MaxValue;
        float minZ = float.MaxValue;
        // �n�ʂ𕽂�ɂ��Ȃ���T�C�Y�����ς���
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
        // X���̃u���b�N�̌�
        int widthCount = Mathf.CeilToInt(maxX - minX) + 1;
        // Z���̃u���b�N�̌�
        int depthCount = Mathf.CeilToInt(maxZ - minZ) + 1;
        // ���X�g�o�^
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
