using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.XR.PicoVR;

namespace Unity.XR.PicoVR.Editor
{
    [CustomEditor(typeof(PicoVRSettings))]
    public class PicoVRSettingsEditor : UnityEditor.Editor
    {
        private const string kStereoRenderingModeAndroid = "m_StereoRenderingModeAndroid";
        private const string kUseDefaultRenderTexture = "UseDefaultRenderTexture";
        private const string kEyeRenderTextureResolution = "eyeRenderTextureResolution";
        private const string kAntiAliasing = "AntiAliasing";
        private const string kRenderTextureDepth = "renderTextureDepth";

        static GUIContent s_StereoRenderingMode = EditorGUIUtility.TrTextContent("Stereo Rendering Mode");
        static GUIContent s_UseDefaultRenderTexture = EditorGUIUtility.TrTextContent("Use Default Render Texture");
        static GUIContent s_EyeRenderTextureResolution = EditorGUIUtility.TrTextContent("Render Texture Resolution");
        static GUIContent s_AntiAliasing = EditorGUIUtility.TrTextContent("Render Texture Anti-Aliasing");
        static GUIContent s_RendertTextureDepth = EditorGUIUtility.TrTextContent("Render Textue Bit Depth");


        private SerializedProperty m_StereoRenderingModeAndroid;
        private SerializedProperty m_UseDefaultRenderTexture;
        private SerializedProperty m_EyeRenderTextureResolution;
        private SerializedProperty m_AntiAliasing;
        private SerializedProperty m_RenderTextureDetph;

        void OnEnable()
        {
            if (m_StereoRenderingModeAndroid == null) m_StereoRenderingModeAndroid = serializedObject.FindProperty(kStereoRenderingModeAndroid);
            if (m_UseDefaultRenderTexture == null) m_UseDefaultRenderTexture = serializedObject.FindProperty(kUseDefaultRenderTexture);
            if (m_EyeRenderTextureResolution == null) m_EyeRenderTextureResolution = serializedObject.FindProperty(kEyeRenderTextureResolution);
            if (m_AntiAliasing == null) m_AntiAliasing = serializedObject.FindProperty(kAntiAliasing);
            if (m_RenderTextureDetph == null) m_RenderTextureDetph = serializedObject.FindProperty(kRenderTextureDepth);

            switch (QualitySettings.antiAliasing)
            {
                case 0:
                    ((PicoVRSettings)target).AntiAliasing = PicoVRSettings.RenderTextureAntiAliasing.X_1;
                    break;
                case 2:
                    ((PicoVRSettings)target).AntiAliasing = PicoVRSettings.RenderTextureAntiAliasing.X_2;
                    break;
                case 4:
                    ((PicoVRSettings)target).AntiAliasing = PicoVRSettings.RenderTextureAntiAliasing.X_4;
                    break;
                case 8:
                    ((PicoVRSettings)target).AntiAliasing = PicoVRSettings.RenderTextureAntiAliasing.X_8;
                    break;
            }
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return;


            
            
            serializedObject.Update();

            BuildTargetGroup selectedBuildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("PicoVR settings cannnot be changed when the editor is in play mode.", MessageType.Info);
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            if (selectedBuildTargetGroup == BuildTargetGroup.Android)
            {

                

                EditorGUILayout.PropertyField(m_StereoRenderingModeAndroid, s_StereoRenderingMode);
                EditorGUILayout.PropertyField(m_UseDefaultRenderTexture, s_UseDefaultRenderTexture);
                if (!((PicoVRSettings)target).UseDefaultRenderTexture)
                {
                    EditorGUILayout.PropertyField(m_EyeRenderTextureResolution, s_EyeRenderTextureResolution);
                }
                EditorGUILayout.PropertyField(m_AntiAliasing, s_AntiAliasing);
                EditorGUILayout.PropertyField(m_RenderTextureDetph, s_RendertTextureDepth);

                switch (((PicoVRSettings)target).AntiAliasing)
                {
                    case PicoVRSettings.RenderTextureAntiAliasing.X_1:
                        QualitySettings.antiAliasing = 0;
                        break;
                    case PicoVRSettings.RenderTextureAntiAliasing.X_2:
                        QualitySettings.antiAliasing = 2;
                        break;
                    case PicoVRSettings.RenderTextureAntiAliasing.X_4:
                        QualitySettings.antiAliasing = 4;
                        break;
                    case PicoVRSettings.RenderTextureAntiAliasing.X_8:
                        QualitySettings.antiAliasing = 8;
                        break;
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndBuildTargetSelectionGrouping();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
