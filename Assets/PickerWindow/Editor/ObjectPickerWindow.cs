//-----------------------------------------------------------------------
// <filename>ObjectPickerWindow</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #小窗口选取对应类型的文件# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/14 星期五 18:31:49# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Taurus
{
     public enum ObjectType
    {
        Assembly = 0,
    }

    public class ObjectPickerWindow : EditorWindow
    {
        private static ObjectPickerWindow _instance;

        public static ObjectPickerWindow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GetWindow<ObjectPickerWindow>();
                return _instance;
            }
        }

        public ObjectType Type;
        public List<Tuple<string, string>> dataList;
        public List<Tuple<string, string>> showDataList;
        public Action<Tuple<string, string>> SelectedItemCallBack;

        private string searchString = string.Empty;
        private Color _defaultColor;
        private Color _label_bg_color;
        private Color _typeColor;
        private GUIStyle _fontStyle;
        private GUIStyle _textBgStyle;

        private Vector2 scrollPosition;

        private void OnEnable()
        {
            _defaultColor = GUI.backgroundColor;
            ColorUtility.TryParseHtmlString("#030303", out _label_bg_color);
            ColorUtility.TryParseHtmlString("#C2C2C2", out _typeColor);

            _fontStyle = new GUIStyle();
            _fontStyle.fontSize = 18;
            _fontStyle.normal.textColor = _typeColor;

            _textBgStyle = new GUIStyle();
            _textBgStyle.margin = new RectOffset(3, 3, 3, 3);

            dataList = new List<Tuple<string, string>>();
            showDataList = new List<Tuple<string, string>>();

            OnInitializeData(Type);

            showDataList = dataList;
        }

        private void OnInitializeData(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Assembly:
                    InitializeAssembly();
                    break;
            }
        }

        private void InitializeAssembly()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                //Debug.Log(assemblies[i].GetName().FullName);
                try
                {
                    Tuple<string, string> tuple = new Tuple<string, string>(assemblies[i].GetName().Name, assemblies[i].Location);
                    dataList.Add(tuple);
                }
                catch (NotSupportedException ex)
                {
                    //
                    Debug.Log(ex.Source);
                }

                
                
            }
        }

        /// <summary>
        /// 查找数据
        /// </summary>
        /// <param name="dataName"></param>
        /// <returns></returns>
        private Tuple<string, string> FindData(string dataName)
        {
            foreach (var data in showDataList)
            {
                if (data.Item1.Equals(dataName))
                    return data;
            }

            return null;
        }


        private void OnGUI()
        {
            GUILayout.BeginVertical();

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            GUILayout.FlexibleSpace();
            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"),GUILayout.Width(280),GUILayout.Height(10));
            if (GUI.changed)
            {
                showDataList = dataList.FindAll((tuple) => { return tuple.Item1.Contains(searchString); });
            }
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                // Remove focus if cleared
                searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            GUI.backgroundColor = _label_bg_color;
            GUILayout.BeginHorizontal("Box");
            GUI.backgroundColor = _defaultColor;

            GUILayout.FlexibleSpace();
            GUILayout.Label(Type.ToString(), _fontStyle , GUILayout.Width(100),GUILayout.Height(20));
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            foreach (var data in showDataList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);

                DrawRecordItem(data.Item1);
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }



        void DrawRecordItem(string item_name)
        {
            GUILayout.FlexibleSpace();
                
            if (GUILayout.Button(item_name, GUILayout.Width(270), GUILayout.Height(20)))
            {
                SelectedItemCallBack(FindData(item_name));
                this.Close();
            }
            GUILayout.FlexibleSpace();
        }

        public static void Show(ObjectType type, Action<Tuple<string, string>> selectedItemCallBack)
        {
            ObjectPickerWindow window = Instance;
            window.titleContent = new GUIContent(type.ToString());
            window.minSize = new Vector2(300, 500);
            window.maxSize = new Vector2(300, 500);
            window.Type = type;
            window.SelectedItemCallBack = selectedItemCallBack;
            window.Show();
        }

        /// <summary>
        /// 获取富文本颜色字体
        /// </summary>
        static string GetRichText(string text,Color color)
        {
            string textColor = ColorUtility.ToHtmlStringRGB(color);
            return "<color = #"+ textColor + ">" + text + "</color>";
        }


        [MenuItem("Test/ShowPicker")]
        static void Test()
        {
            Show(ObjectType.Assembly, (data) => { Debug.Log("selected: " + data.Item1 + "path: " + data.Item2); });
        }
    }
}
