using System;

using UnityEngine;
using UnityEngine.XR.Management;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.XR.PicoVR
{
    [System.Serializable]
    [XRConfigurationData("PicoVR", "Unity.XR.PicoVR.Settings")]
    public class PicoVRSettings : ScriptableObject
    {
        public enum StereoRenderingModeAndroid
        {
            /// <summary>
            /// Unity makes two passes across the scene graph, each one entirely indepedent of the other. 
            /// Each pass has its own eye matrices and render target. Unity draws everything twice, which includes setting the graphics state for each pass. 
            /// This is a slow and simple rendering method which doesn't require any special modification to shaders.
            /// </summary>
            MultiPass = 0,
             /// <summary>
            /// Unity uses a single texture array with two elements. 
            /// Multiview is very similar to Single Pass Instanced; however, the graphics driver converts each call into an instanced draw call so it requires less work on Unity's side. 
            /// As with Single Pass Instanced, shaders need to be aware of the Multiview setting. Unity's shader macros handle the situation.
            /// </summary>
            Multiview = 1
        }

    
        public enum RenderTextureDepthType
        {
            BD_0 = 0,
            BD_16 = 16,
            BD_24 = 24
        }

        public enum RenderTextureAntiAliasing
        {
            X_1 = 1,
            X_2 = 2,
            X_4 = 4,
            X_8 = 8,
        }


        /// <summary>
        /// Enable or disable support for using a shared depth buffer. This allows Unity and Oculus to use a common depth buffer which enables Oculus to composite the Oculus Dash and other utilities over the Unity application.
        /// </summary>
        //[SerializeField, Tooltip("Enable a shared depth buffer")]
        //public bool SharedDepthBuffer = true;

        /// <summary>
        /// Enable or disable Dash support. This inintializes the Oculus Plugin with Dash support which enables the Oculus Dash to composite over the Unity application.
        /// </summary>
        //[SerializeField, Tooltip("Enable Oculus Dash Support")]
        //public bool DashSupport = true;
        
        /// <summary>
        /// The current stereo rendering mode selected for Android-based Oculus platforms
        /// </summary>
        [SerializeField, Tooltip("Set the Stereo Rendering Method")]
        public StereoRenderingModeAndroid m_StereoRenderingModeAndroid;

        [SerializeField, Tooltip("Set whether use the default render texture")]
        public bool UseDefaultRenderTexture = true;

        [SerializeField, Tooltip("Set render texture antiAliasing type")]
        public RenderTextureAntiAliasing AntiAliasing = RenderTextureAntiAliasing.X_1;

        [SerializeField, Tooltip("Set the Resolution of eyes")]
        public Vector2 eyeRenderTextureResolution = new Vector2(2048, 2048);

        [SerializeField, Tooltip("Set the depth type of render texture")]
        public RenderTextureDepthType renderTextureDepth = RenderTextureDepthType.BD_24;


        
//        public RenderTextureAntiAliasing AntiAliasing
//        {
//            get
//            {
//                return m_AntiAliasing;
//            }
//            set
//            {
//                m_AntiAliasing = value;
//#if UNITY_EDITOR
                
//                switch(m_AntiAliasing)
//                {
//                    case RenderTextureAntiAliasing.X_1:
//                        QualitySettings.antiAliasing = 0;
//                        break;
//                    case RenderTextureAntiAliasing.X_2:
//                        QualitySettings.antiAliasing = 2;
//                        break;
//                    case RenderTextureAntiAliasing.X_4:
//                        QualitySettings.antiAliasing = 4;
//                        break;
//                    case RenderTextureAntiAliasing.X_8:
//                        QualitySettings.antiAliasing = 8;
//                        break;
//                }
//#endif
//            }
//        }
        /// <summary>
        /// Enable this if you are building for Quest. This enables application signing with the Android Package (APK) Signature Scheme v2. Disable v2 signing if building for Oculus Go.
        /// </summary>
        // [SerializeField, Tooltip("Configure Manifest for Oculus Quest")]
        // public bool V2Signing = true;


        public ushort GetStereoRenderingMode()
        {
            return (ushort)m_StereoRenderingModeAndroid;
        }
#if !UNITY_EDITOR
		public static PicoVRSettings s_Settings;

		public void Awake()
		{
            Debug.Log("PicoVRSettings awake....");
			s_Settings = this;
            Debug.Log("PicoVRSettings awake...." + this.eyeRenderTextureResolution);
		}
#endif
    }
}
