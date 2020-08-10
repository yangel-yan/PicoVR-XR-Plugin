using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using UnityEngine.XR;
#if UNITY_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
//using Unity.XR.PicoVR.Input;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Unity.XR.PicoVR
{
#if UNITY_INPUT_SYSTEM
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    static class InputLayoutLoader
    {
        static InputLayoutLoader()
        {
            Debug.Log("InputLayoutLoader");
            RegisterInputLayouts();
        }

        public static void RegisterInputLayouts()
        {
            //InputSystem.RegisterLayout<PicoVRHMD>(
            //    matches: new InputDeviceMatcher()
            //        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
            //        .WithProduct("^(PicoVR Rift)|^(PicoVR Quest)|^(PicoVR Go)"));
            //InputSystem.RegisterLayout<PicoVRTouchController>(
            //    matches: new InputDeviceMatcher()
            //        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
            //        .WithProduct(@"(^(PicoVR Touch Controller))|(^(PicoVR Quest Controller))"));
            //InputSystem.RegisterLayout<PicoVRRemote>(
            //    matches: new InputDeviceMatcher()
            //        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
            //        .WithProduct(@"PicoVR Remote"));
            //InputSystem.RegisterLayout<PicoVRTrackingReference>(
            //    matches: new InputDeviceMatcher()
            //        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
            //        .WithProduct(@"((Tracking Reference)|(^(PicoVR Rift [a-zA-Z0-9]* \(Camera)))"));

            //InputSystem.RegisterLayout<PicoVRHMDExtended>(
            //    name: "GearVR",
            //    matches: new InputDeviceMatcher()
            //        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
            //        .WithProduct("PicoVR HMD"));
            //InputSystem.RegisterLayout<GearVRTrackedController>(
            //    matches: new InputDeviceMatcher()
            //        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
            //        .WithProduct("^(PicoVR Tracked Remote)"));
        }
    }
#endif

    public class PicoVRLoader : XRLoaderHelper
#if UNITY_EDITOR
    , IXRLoaderPreInit
#endif
    {
        private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
        private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();

        public XRDisplaySubsystem displaySubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRDisplaySubsystem>();
            }
        }

        public XRInputSubsystem inputSubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRInputSubsystem>();
            }
        }

        public override bool Initialize()
        {
#if UNITY_INPUT_SYSTEM
            InputLayoutLoader.RegisterInputLayouts();
#endif

            PicoVRSettings settings = GetSettings();
            if (settings != null)
            {
                UserDefinedSettings userDefinedSettings = new UserDefinedSettings();
                //userDefinedSettings.sharedDepthBuffer = (ushort)(settings.SharedDepthBuffer ? 1 : 0);
                //userDefinedSettings.dashSupport = (ushort)(settings.DashSupport ? 1 : 0);
                userDefinedSettings.stereoRenderingMode = (ushort) settings.GetStereoRenderingMode();
                userDefinedSettings.colorSpace = (ushort) ((QualitySettings.activeColorSpace == ColorSpace.Linear) ? 1 : 0);
                userDefinedSettings.useDefaultRenderTexture = settings.UseDefaultRenderTexture;
                userDefinedSettings.eyeRenderTextureResolution = settings.eyeRenderTextureResolution;
                userDefinedSettings.antiAliasing = (ushort)settings.AntiAliasing;
                userDefinedSettings.renderTextureDepth = (ushort)settings.renderTextureDepth;

                // Application.targetFrameRate = 72;

                try
                {
                   Debug.Log("Pvr_GetHmdHardwareVersion" + Pvr_GetHmdHardwareVersion());
                }
                catch(DllNotFoundException d)
                {
                   Debug.LogError(d.Message);
                }
                Debug.Log("Initialize++++ " + userDefinedSettings.stereoRenderingMode
                    + " "+ userDefinedSettings.colorSpace
                    + " " + userDefinedSettings.antiAliasing
                    + " " + userDefinedSettings.useDefaultRenderTexture);
                SetUserDefinedSettings(userDefinedSettings);
            }

            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, "PicoVR display");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "PicoVR input");
        
            if (displaySubsystem == null || inputSubsystem == null)
            {
                Debug.LogError("Unable to start PicoVR XR Plugin.");
            }

            if (displaySubsystem == null)
            {
                Debug.LogError("Failed to load display subsystem.");
            }

            if (inputSubsystem == null)
            {
                Debug.LogError("Failed to load input subsystem.");
            }
 

            return displaySubsystem != null;// && inputSubsystem != null;
        }

        public override bool Start()
        {
            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();

            return true;
        }

        public override bool Stop()
        {
            StopSubsystem<XRDisplaySubsystem>();
            StopSubsystem<XRInputSubsystem>();

            return true;
        }

        public override bool Deinitialize()
        {
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();

            return true;
        }

#if UNITY_EDITOR && XR_MGMT_GTE_320
        private void RemoveVulkanFromAndroidGraphicsAPIs()
        {
            // don't need to do anything if auto apis is selected
            if (PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android))
                return;

            GraphicsDeviceType[] oldApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
            List<GraphicsDeviceType> newApisList = new List<GraphicsDeviceType>();
            bool vulkanRemoved = false;

            // copy all entries except vulkan
            foreach (GraphicsDeviceType dev in oldApis)
            {
                if (dev == GraphicsDeviceType.Vulkan)
                {
                    vulkanRemoved = true;
                    continue;
                }
                
                newApisList.Add(dev);
            }

            // if we didn't remove Vulkan from the list, no need to do any further processing
            if (vulkanRemoved == false)
                return;

            if (newApisList.Count <= 0)
            {
                newApisList.Add(GraphicsDeviceType.OpenGLES3);
                Debug.LogWarning(
                    "Vulkan is currently experimental on PicoVR Quest. It has been removed from your list of Android graphics APIs and replaced with OpenGLES3.\n" +
                    "If you would like to use experimental Quest Vulkan support, you can add it back into the list of graphics APIs in the Player settings.");
            }
            else
            {
                Debug.LogWarning(
                    "Vulkan is currently experimental on PicoVR Quest. It has been removed from your list of Android graphics APIs.\n" +
                    "If you would like to use experimental Quest Vulkan support, you can add it back into the list of graphics APIs in the Player settings.");
            }

            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, newApisList.ToArray());
        }

        public override void WasAssignedToBuildTarget(BuildTargetGroup buildTargetGroup)
        {
            if (buildTargetGroup == BuildTargetGroup.Android)
            {
                RemoveVulkanFromAndroidGraphicsAPIs();
            }
        }
#endif

        [StructLayout(LayoutKind.Sequential)]
        struct UserDefinedSettings
        {
            //public ushort sharedDepthBuffer;
            //public ushort dashSupport;
            public ushort stereoRenderingMode;
            public ushort colorSpace;
            public int antiAliasing;
            public bool useDefaultRenderTexture;
            public Vector2 eyeRenderTextureResolution;
            public int renderTextureDepth;
        }

        [DllImport("UnityPicoVR")]// "Unity.XR.Management"、、, CharSet=CharSet.Auto
        static extern void SetUserDefinedSettings(UserDefinedSettings settings);

        [DllImport("Pvr_UnitySDK", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Pvr_GetHmdHardwareVersion();

        public PicoVRSettings GetSettings()
        {
            PicoVRSettings settings = null;
#if UNITY_EDITOR
            UnityEditor.EditorBuildSettings.TryGetConfigObject<PicoVRSettings>("Unity.XR.PicoVR.Settings", out settings);
#else
            settings = PicoVRSettings.s_Settings;
#endif
            return settings;
        }

#if UNITY_EDITOR
        public string GetPreInitLibraryName(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup)
        {
            Debug.Log("Ever in here ? GetPreInitLibraryName ...");
            return "UnityPicoVR";
        }
#endif
    }
}
