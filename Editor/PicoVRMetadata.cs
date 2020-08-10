#if XR_MGMT_GTE_320

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;

namespace Unity.XR.PicoVR.Editor
{
    internal class PicoVRMetadata : IXRPackage
    {
        private class PicoVRPackageMetadata : IXRPackageMetadata
        {
            public string packageName => "PicoVR XR Plugin";
            public string packageId => "com.unity.xr.picovr";
            public string settingsType => "Unity.XR.PicoVR.PicoVRSettings";
            public List<IXRLoaderMetadata> loaderMetadata => s_LoaderMetadata;

            private readonly static List<IXRLoaderMetadata> s_LoaderMetadata = new List<IXRLoaderMetadata>() { new PicoVRLoaderMetadata() };
        }

        private class PicoVRLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName => "PicoVR";
            public string loaderType => "Unity.XR.PicoVR.PicoVRLoader";
            public List<BuildTargetGroup> supportedBuildTargets => s_SupportedBuildTargets;

            private readonly static List<BuildTargetGroup> s_SupportedBuildTargets = new List<BuildTargetGroup>()
            {
                BuildTargetGroup.Android
            };
        }

        private static IXRPackageMetadata s_Metadata = new PicoVRPackageMetadata();
        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            var settings = obj as PicoVRSettings;
            if (settings != null)
            {
                //settings.m_StereoRenderingModeDesktop = PicoVRSettings.StereoRenderingModeDesktop.MultiPass;
                settings.m_StereoRenderingModeAndroid = PicoVRSettings.StereoRenderingModeAndroid.MultiPass;
                settings.UseDefaultRenderTexture = true;
                settings.eyeRenderTextureResolution = new Vector2(2048, 2048);
                settings.renderTextureDepth = PicoVRSettings.RenderTextureDepthType.BD_24;

                return true;
            }

            return false;
        }
    }
}

#endif // XR_MGMT_GTE_320
