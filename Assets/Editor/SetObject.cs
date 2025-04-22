using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    private float _delayTime = 0.5f;
    private double _oldTime = 0f;

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
        WallFix();

    }

    private void OnSceneGUI(SceneView sceneView)
    {

        OnSetObject();

    }

    /// <summary>
    /// �N���b�N���[�h�N���E��~����
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
    /// �N���b�N���[�h
    /// </summary>
    /// <param name="isRunning"></param>
    private void OnSetObject()
    {

        if (!_isClickMode)
        {

            return;

        }
        Debug.Log(_isClickMode);
        // �C�x���g�擾�������������Ɍ���
        _e = Event.current;
        // ���̃{�^�����������u��
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
        // ���{�^�����������u�Ԃł͂Ȃ��܂���Event��Null�܂���ALT�������Ȃ���ł͂Ȃ��܂��̓^�C�}�[���^�C����菬����
        if (EditorApplication.timeSinceStartup - _oldTime <= _delayTime)
        {


            return;

        }
        _oldTime = EditorApplication.timeSinceStartup;
        // �ݒu�����擾
        int setCount = rootVisualElement.Q<IntegerField>("SetCount").value;
        // �}�E�X�J�[�\���̈ʒu����Ray��ł�
        Ray ray = HandleUtility.GUIPointToWorldRay(_e.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // �q�b�g�����ʒu�Ɩ@�����擾
            Vector3 hitPoint = hit.point;
            Vector3 normal = hit.normal;
            // �������ď��������I������I�u�W�F�N�g������
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
                        // �m�[�}���C�Y�����̃T�C�Y�̂ݎc��
                        scale.x *= normal.x;
                        scale.y *= normal.y;
                        scale.z *= normal.z;
                        // �X�P�[���̂Q�{�����Ηׂɐݒu�ł��邽��
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
    /// �t�B�����[�h
    /// ���Ƀu���b�N�𖄂߂鏈��
    /// </summary>
    private void OnFillObject()
    {



    }

    /// <summary>
    ///�ǂ��ς��Č��Ԃ𖄂߂鏈��
    /// </summary>
    private void WallFix()
    {

        // �{�^�����擾�������ꂽ�Ƃ��p�Ɋ֐���o�^
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
                // ����
                float yPos = pos.y;
                // ������0�ɂ��đ��
                pos.y = 0;
                objs[i].transform.position = pos;

                Vector3 scale = objs[i].transform.localScale;
                // �����𕝂ɕϊ�
                scale.y = yPos / 2 + 1;
                objs[i].transform.localScale = scale;

            }

        };

    }

    /// <summary>
    /// �n�ʂ̏�Ԃ�ύX
    /// </summary>
    private void GroundChange()
    {

        // �{�^�����擾�������ꂽ�Ƃ��p�Ɋ֐���o�^
        Button clickButton = rootVisualElement.Q<Button>("GroundChangeButton");
        clickButton.clicked += () =>
        {

            // �n�ʂ̊�{�̍���
            Slider Height = rootVisualElement.Q<Slider>("GroundHeight");
            // �N��
            Slider terrainRoughness = Height = rootVisualElement.Q<Slider>("TerrainRoughness");
            // �ΌX�i�u���b�N�Z���j
            Slider slope = rootVisualElement.Q<Slider>("Slope");
            // �u���b�N�ύX
            Toggle isChangeBlock = rootVisualElement.Q<Toggle>("ChangeBlock");
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
            //�ύX���������ꂩ��u���b�N�ύX



        };


    }

}
