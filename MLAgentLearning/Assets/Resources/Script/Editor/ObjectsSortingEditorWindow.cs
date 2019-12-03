using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 서클, 메쉬, 리니어 형태로 정렬 가능한 에디터
/// 
/// UI를 일정 간격으로 배치할 때 사용하면 편합니다.
/// custom tool - sort Object  를 선택하여 실행 할 수 있습니다.
/// mesh : Mesh vertex 형태로 오브젝트를 배치합니다.
/// </summary>
#if UNITY_EDITOR
public class ObjectsSortingEditorWindow : EditorWindow
{
    string sortCircleString = "Circle";
    string sortMeshString = "Mesh";
    string sortLinearString = "Linear";
    string statusString = "Option";

    float sliderCircleRadiouScale = 0.0f;
    int sliderGabScale = 0;
    float sliderLinearScale = 0.0f;

    int sliderMaxRange = 100;

    bool isRealTimeUpdate = false;

    bool isOption = false;

    //properties
    public GameObject generateObject;
    public GameObject[] objects = null;
    public Mesh mesh;

    int selected = -1;


    [MenuItem("CircusTools/Object Sorting")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ObjectsSortingEditorWindow));
    }
    private void OnGUI()
    {

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty gameobjectsProperty = so.FindProperty("objects");
        SerializedProperty generateObjectProperty = so.FindProperty("generateObject");
        SerializedProperty meshProperty = so.FindProperty("mesh");

        //각 버튼에 대한 파라미터
        switch (selected)
        {
            case 0:
                GUILayout.Label("Sort By Circle", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(gameobjectsProperty, true); // True means show children

                //원 반지름
                sliderCircleRadiouScale = EditorGUILayout.Slider(sliderCircleRadiouScale, 0, sliderMaxRange);
                break;
            case 1:
                GUILayout.Label("Sort By Mesh", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(gameobjectsProperty, true); // True means show children
                EditorGUILayout.PropertyField(meshProperty, true); // True means show children


                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.TextField("Mesh Vertices Length", EditorStyles.label);
                    if (mesh != null)
                        GUILayout.TextField(mesh.vertices.Length.ToString(), EditorStyles.label);
                    else
                        GUILayout.TextField("null", EditorStyles.label);
                }

                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Objects Gab", EditorStyles.label);
                    sliderGabScale = (int)EditorGUILayout.Slider(sliderGabScale, 0, sliderMaxRange);
                }

                break;
            case 2:
                GUILayout.Label("Sort By Linear", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(gameobjectsProperty, true); // True means show children

                GUILayout.Label("Objects Gab", EditorStyles.label);
                sliderLinearScale = EditorGUILayout.Slider(sliderLinearScale, 0, sliderMaxRange);
                break;
        }

        if (selected != -1)
        {
            isRealTimeUpdate = EditorGUILayout.Toggle("realTimeUpdate", isRealTimeUpdate);
        }
        else
        {
            GUILayout.Label("Chose Mode", EditorStyles.boldLabel);
        }

        so.ApplyModifiedProperties(); // Remember to apply modified properties


        //리얼타임 true / false 에대한 버튼 셋
        if (isRealTimeUpdate)
        {
            selected = GUILayout.Toolbar(selected, new string[] { sortCircleString, sortMeshString, sortLinearString });
        }
        else
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(sortCircleString))
                {
                    selected = 0;
                    SortCircle(objects, sliderCircleRadiouScale);
                }
                if (GUILayout.Button(sortMeshString))
                {
                    selected = 1;
                    SortByMesh(objects, sliderGabScale);
                }
                if (GUILayout.Button(sortLinearString))
                {
                    selected = 2;
                    SortByLinear(objects, sliderLinearScale);
                }
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Clear"))
        {
            Clear();
        }

        isOption = EditorGUILayout.Foldout(isOption, statusString);
        if (isOption)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.TextField("Slider Max Range", EditorStyles.label);
                sliderMaxRange = int.Parse(GUILayout.TextField(sliderMaxRange + "", EditorStyles.textField));
            }
        }
        
    }


    void OnInspectorUpdate()
    {
        if (isRealTimeUpdate == false || selected == -1)
        {
            return;
        }

        switch (selected)
        {
            case 0:
                SortCircle(objects, sliderCircleRadiouScale);
                break;
            case 1:
                SortByMesh(objects, sliderGabScale);
                break;
            case 2:
                SortByLinear(objects, sliderLinearScale);
                break;
        }

        // Call Repaint on OnInspectorUpdate as it repaints the windows
        // less times as if it was OnGUI/Update
        Repaint();
    }


    public void SortCircle(GameObject[] objects, float radius = 0)
    {
        for (int i = 0; i < objects.Length; i++)
        {
        Found:

            float cornerAngle = 2f * Mathf.PI / (float)objects.Length * i;

            if (objects[i] == null)
            {
                i++;
                if (i < objects.Length)
                    goto Found;
                else
                    break;
            }

            objects[i].transform.localPosition = new Vector2(Mathf.Cos(cornerAngle) * radius, Mathf.Sin(cornerAngle) * radius);
        }
    }

    public void SortByMesh(GameObject[] objects, int gab = 0)
    {
        int j = 0;
        Vector3[] verts = mesh.vertices;
        int count = 0;

        if (verts.Length <= objects.Length * gab + 1)
        {
            count = 1 + gab;
        }
        else
        {
            count = verts.Length / objects.Length + gab;
        }
        for (int i = 0; i < verts.Length; i += count)
        {
            if (verts.Length < i)
            {
                break;
            }
            objects[j].transform.localPosition = verts[i];
            j++;
        }

        if(objects.Length > j)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.TextField("OutRange Ojbects", EditorStyles.label);
                GUILayout.TextField((objects.Length - j).ToString(), EditorStyles.label);
            }
        }
    }

    public void SortByLinear(GameObject[] objects, float gab = 0)
    {
        for(int i = 0; i < objects.Length; i++)
        {
            objects[i].transform.localPosition = Vector3.right * i * gab;
        }
    }

    void OnDestroy()
    {
        Clear();
    }

    public void Clear()
    {
        ShowNotification(new GUIContent("Clear"));
        objects = null;
        selected = -1;
        isRealTimeUpdate = false;
        mesh = null;
        isOption = false;
        sliderMaxRange = 100;
    }
}
#endif