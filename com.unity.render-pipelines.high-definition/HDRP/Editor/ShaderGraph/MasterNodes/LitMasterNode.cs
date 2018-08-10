using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.ShaderGraph
{
    [Serializable]
    [Title("Master", "Lit")]
    public class LitMasterNode : MasterNode<ILitSubShader>, IMayRequirePosition, IMayRequireNormal
    {
        public const string AlbedoSlotName = "Albedo";
        public const string NormalSlotName = "Normal";
        public const string BentNormalSlotName = "BentNormal";
        public const string TangentSlotName = "Tangent";
        public const string SubsurfaceMaskSlotName = "SubsurfaceMask";
        public const string ThicknessSlotName = "Thickness";
        public const string DiffusionProfileSlotName = "DiffusionProfile";
        public const string IridescenceMaskSlotName = "IridescenceMask";
        public const string IridescenceThicknessSlotName = "IridescenceThickness";
        public const string SpecularSlotName = "Specular";
        public const string CoatMaskSlotName = "CoatMask";
        public const string EmissionSlotName = "Emission";
        public const string MetallicSlotName = "Metallic";
        public const string SmoothnessSlotName = "Smoothness";
        public const string OcclusionSlotName = "Occlusion";
        public const string AlphaSlotName = "Alpha";
        public const string AlphaClipThresholdSlotName = "AlphaClipThreshold";
        public const string AlphaClipThresholdDepthPrepassSlotName = "AlphaClipThresholdDepthPrepass";
        public const string AlphaClipThresholdDepthPostpassSlotName = "AlphaClipThresholdDepthPrepass";
        public const string AnisotropySlotName = "Anisotropy";
        public const string PositionName = "Position";

        public const int PositionSlotId = 0;
        public const int AlbedoSlotId = 1;
        public const int NormalSlotId = 2;
        public const int BentNormalSlotId = 3;
        public const int TangentSlotId = 4;
        public const int SubsurfaceMaskSlotId = 5;
        public const int ThicknessSlotId = 6;
        public const int DiffusionProfileSlotId = 7;
        public const int IridescenceMaskSlotId = 8;
        public const int IridescenceThicknessSlotId = 9;
        public const int SpecularSlotId = 10;
        public const int CoatMaskSlotId = 11;
        public const int MetallicSlotId = 12;
        public const int EmissionSlotId = 13;
        public const int SmoothnessSlotId = 14;
        public const int OcclusionSlotId = 15;
        public const int AlphaSlotId = 16;
        public const int AlphaThresholdSlotId = 17;
        public const int AlphaThresholdDepthPrepassSlotId = 18;
        public const int AlphaThresholdDepthPostpassSlotId = 19;
        public const int AnisotropySlotId = 20;

        public enum MaterialType
        {
            Standard,
            SubsurfaceScattering,
            Anisoptropy,
            Iridescence,
            SpecularColor,
            Translucent
        }

        // Just for convenience of doing simple masks. We could run out of bits of course.
        [Flags]
        enum SlotMask
        {
            None = 0,
            Position = 1 << PositionSlotId,
            Albedo = 1 << AlbedoSlotId,
            Normal = 1 << NormalSlotId,
            BentNormal = 1 << BentNormalSlotId,
            Tangent = 1 << TangentSlotId,
            SubsurfaceMask = 1 << SubsurfaceMaskSlotId,
            Thickness = 1 << ThicknessSlotId,
            DiffusionProfile = 1 << DiffusionProfileSlotId,
            IridescenceMask = 1 << IridescenceMaskSlotId,
            IridescenceLayerThickness = 1 << IridescenceThicknessSlotId,
            Specular = 1 << SpecularSlotId,
            CoatMask = 1 << CoatMaskSlotId,
            Metallic = 1 << MetallicSlotId,
            Emission = 1 << EmissionSlotId,
            Smoothness = 1 << SmoothnessSlotId,
            Occlusion = 1 << OcclusionSlotId,
            Alpha = 1 << AlphaSlotId,
            AlphaThreshold = 1 << AlphaThresholdSlotId,
            AlphaThresholdDepthPrepass = 1 << AlphaThresholdDepthPrepassSlotId,
            AlphaThresholdDepthPostpass = 1 << AlphaThresholdDepthPostpassSlotId,
            Anisotropy = 1 << AnisotropySlotId,
        }

        const SlotMask StandardSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Metallic | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass;
        const SlotMask SubsurfaceScatteringSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.SubsurfaceMask | SlotMask.Thickness | SlotMask.DiffusionProfile | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass;
        const SlotMask AnisotropySlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.Tangent | SlotMask.Anisotropy | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Metallic | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass;
        const SlotMask IridescenceSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.IridescenceMask | SlotMask.IridescenceLayerThickness | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Metallic | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass;
        const SlotMask SpecularColorSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.Specular | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass;
        const SlotMask TranslucentSlotMask = SlotMask.Position | SlotMask.Albedo | SlotMask.Normal | SlotMask.BentNormal | SlotMask.Thickness | SlotMask.DiffusionProfile | SlotMask.CoatMask | SlotMask.Emission | SlotMask.Smoothness | SlotMask.Occlusion | SlotMask.Alpha | SlotMask.AlphaThreshold | SlotMask.AlphaThresholdDepthPrepass | SlotMask.AlphaThresholdDepthPostpass;

        // This could also be a simple array. For now, catch any mismatched data.
        SlotMask GetActiveSlotMask()
        {
            switch (materialType)
            {
                case MaterialType.Standard:
                    return StandardSlotMask;

                case MaterialType.SubsurfaceScattering:
                    return SubsurfaceScatteringSlotMask;

                case MaterialType.Anisoptropy:
                    return AnisotropySlotMask;

                case MaterialType.Iridescence:
                    return IridescenceSlotMask;

                case MaterialType.SpecularColor:
                    return SpecularColorSlotMask;

                case MaterialType.Translucent:
                    return TranslucentSlotMask;

                default:
                    return SlotMask.None;
            }
        }

        bool MaterialTypeUsesSlotMask(SlotMask mask)
        {
            SlotMask activeMask = GetActiveSlotMask();
            return (activeMask & mask) != 0;
        }

        [SerializeField]
        SurfaceType m_SurfaceType;

        public SurfaceType surfaceType
        {
            get { return m_SurfaceType; }
            set
            {
                if (m_SurfaceType == value)
                    return;

                m_SurfaceType = value;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        AlphaMode m_AlphaMode;

        public AlphaMode alphaMode
        {
            get { return m_AlphaMode; }
            set
            {
                if (m_AlphaMode == value)
                    return;

                m_AlphaMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_BlendPreserveSpecular;

        public ToggleData blendPreserveSpecular
        {
            get { return new ToggleData(m_BlendPreserveSpecular); }
            set
            {
                if (m_BlendPreserveSpecular == value.isOn)
                    return;
                m_BlendPreserveSpecular = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_TransparencyFog;

        public ToggleData transparencyFog
        {
            get { return new ToggleData(m_TransparencyFog); }
            set
            {
                if (m_TransparencyFog == value.isOn)
                    return;
                m_TransparencyFog = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_DrawBeforeRefraction;

        public ToggleData drawBeforeRefraction
        {
            get { return new ToggleData(m_DrawBeforeRefraction); }
            set
            {
                if (m_DrawBeforeRefraction == value.isOn)
                    return;
                m_DrawBeforeRefraction = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_AlphaTest;

        public ToggleData alphaTest
        {
            get { return new ToggleData(m_AlphaTest); }
            set
            {
                if (m_AlphaTest == value.isOn)
                    return;
                m_AlphaTest = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_AlphaTestDepthPrepass;

        public ToggleData alphaTestDepthPrepass
        {
            get { return new ToggleData(m_AlphaTestDepthPrepass); }
            set
            {
                if (m_AlphaTestDepthPrepass == value.isOn)
                    return;
                m_AlphaTestDepthPrepass = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_AlphaTestDepthPostpass;

        public ToggleData alphaTestDepthPostpass
        {
            get { return new ToggleData(m_AlphaTestDepthPostpass); }
            set
            {
                if (m_AlphaTestDepthPostpass == value.isOn)
                    return;
                m_AlphaTestDepthPostpass = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_BackThenFrontRendering;

        public ToggleData backThenFrontRendering
        {
            get { return new ToggleData(m_BackThenFrontRendering); }
            set
            {
                if (m_BackThenFrontRendering == value.isOn)
                    return;
                m_BackThenFrontRendering = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        int m_SortPriority;

        public int sortPriority
        {
            get { return m_SortPriority; }
            set
            {
                if (m_SortPriority == value)
                    return;
                m_SortPriority = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        DoubleSidedMode m_DoubleSidedMode;

        public DoubleSidedMode doubleSidedMode
        {
            get { return m_DoubleSidedMode; }
            set
            {
                if (m_DoubleSidedMode == value)
                    return;

                m_DoubleSidedMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        MaterialType m_MaterialType;

        public MaterialType materialType
        {
            get { return m_MaterialType; }
            set
            {
                if (m_MaterialType == value)
                    return;

                m_MaterialType = value;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_SSSTransmission;

        public ToggleData sssTransmission
        {
            get { return new ToggleData(m_SSSTransmission); }
            set
            {
                m_SSSTransmission = value.isOn;
                UpdateNodeAfterDeserialization();
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        bool m_ReceiveDecals;

        public ToggleData receiveDecals
        {
            get { return new ToggleData(m_ReceiveDecals); }
            set
            {
                if (m_ReceiveDecals == value.isOn)
                    return;
                m_ReceiveDecals = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_EnergyConservingSpecular;

        public ToggleData energyConservingSpecular
        {
            get { return new ToggleData(m_EnergyConservingSpecular); }
            set
            {
                if (m_EnergyConservingSpecular == value.isOn)
                    return;
                m_EnergyConservingSpecular = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_SpecularAA;

        public ToggleData specularAA
        {
            get { return new ToggleData(m_SpecularAA); }
            set
            {
                if (m_SpecularAA == value.isOn)
                    return;
                m_SpecularAA = value.isOn;
                Dirty(ModificationScope.Topological);
            }
        }

        [SerializeField]
        float m_SpecularAAScreenSpaceVariance;
    
        public float specularAAScreenSpaceVariance
        {
            get { return m_SpecularAAScreenSpaceVariance; }
            set
            {
                if (m_SpecularAAScreenSpaceVariance == value)
                    return;
                m_SpecularAAScreenSpaceVariance = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        float m_SpecularAAThreshold;

        public float specularAAThreshold
        {
            get { return m_SpecularAAThreshold; }
            set
            {
                if (m_SpecularAAThreshold == value)
                    return;
                m_SpecularAAThreshold = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_MotionVectors;

        public ToggleData motionVectors
        {
            get { return new ToggleData(m_MotionVectors); }
            set
            {
                if (m_MotionVectors == value.isOn)
                    return;
                m_MotionVectors = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_AlbedoAffectsEmissive;

        public ToggleData albedoAffectsEmissive
        {
            get { return new ToggleData(m_AlbedoAffectsEmissive); }
            set
            {
                if (m_AlbedoAffectsEmissive == value.isOn)
                    return;
                m_AlbedoAffectsEmissive = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        bool m_BentNormalSpecularOcclusion;

        public ToggleData bentNormalSpecularOcclusion
        {
            get { return new ToggleData(m_BentNormalSpecularOcclusion); }
            set
            {
                if (m_BentNormalSpecularOcclusion == value.isOn)
                    return;
                m_BentNormalSpecularOcclusion = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        int m_DiffusionProfile;

        public int diffusionProfile
        {
            get { return m_DiffusionProfile; }
            set
            {
                if (m_DiffusionProfile == value)
                    return;

                m_DiffusionProfile = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public LitMasterNode()
        {
            UpdateNodeAfterDeserialization();
        }

        public override string documentationURL
        {
            // Any entry needs to be created for Lit Master Node.
            // Do we want to ultimately just replace the other PBR Master Node?
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/PBR-Master-Node"; }
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
            name = "Lit Master";

            List<int> validSlots = new List<int>();
            if (MaterialTypeUsesSlotMask(SlotMask.Position))
            {
                AddSlot(new PositionMaterialSlot(PositionSlotId, PositionName, PositionName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
                validSlots.Add(PositionSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Albedo))
            {
                AddSlot(new ColorRGBMaterialSlot(AlbedoSlotId, AlbedoSlotName, AlbedoSlotName, SlotType.Input, Color.white, ColorMode.Default, ShaderStageCapability.Fragment));
                validSlots.Add(AlbedoSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Normal))
            {
                AddSlot(new NormalMaterialSlot(NormalSlotId, NormalSlotName, NormalSlotName, CoordinateSpace.Tangent, ShaderStageCapability.Fragment));
                validSlots.Add(NormalSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.BentNormal))
            {
                AddSlot(new NormalMaterialSlot(BentNormalSlotId, BentNormalSlotName, BentNormalSlotName, CoordinateSpace.Tangent, ShaderStageCapability.Fragment));
                validSlots.Add(BentNormalSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Tangent))
            {
                AddSlot(new NormalMaterialSlot(TangentSlotId, TangentSlotName, TangentSlotName, CoordinateSpace.Tangent, ShaderStageCapability.Fragment));
                validSlots.Add(TangentSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Anisotropy))
            {
                AddSlot(new Vector1MaterialSlot(AnisotropySlotId, AnisotropySlotName, AnisotropySlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(AnisotropySlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.SubsurfaceMask))
            {
                AddSlot(new Vector1MaterialSlot(SubsurfaceMaskSlotId, SubsurfaceMaskSlotName, SubsurfaceMaskSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(SubsurfaceMaskSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Thickness) && (sssTransmission.isOn || materialType == MaterialType.Translucent))
            {
                AddSlot(new Vector1MaterialSlot(ThicknessSlotId, ThicknessSlotName, ThicknessSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(ThicknessSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.DiffusionProfile))
            {
                AddSlot(new DiffusionProfileInputMaterialSlot(DiffusionProfileSlotId, DiffusionProfileSlotName, DiffusionProfileSlotName, ShaderStageCapability.Fragment));
                validSlots.Add(DiffusionProfileSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.IridescenceMask))
            {
                AddSlot(new Vector1MaterialSlot(IridescenceMaskSlotId, IridescenceMaskSlotName, IridescenceMaskSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));
                validSlots.Add(IridescenceMaskSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.IridescenceLayerThickness))
            {
                AddSlot(new Vector1MaterialSlot(IridescenceThicknessSlotId, IridescenceThicknessSlotName, IridescenceThicknessSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));
                validSlots.Add(IridescenceThicknessSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Specular))
            {
                AddSlot(new ColorRGBMaterialSlot(SpecularSlotId, SpecularSlotName, SpecularSlotName, SlotType.Input, Color.white, ColorMode.Default, ShaderStageCapability.Fragment));
                validSlots.Add(SpecularSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.CoatMask))
            {
                AddSlot(new Vector1MaterialSlot(CoatMaskSlotId, CoatMaskSlotName, CoatMaskSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));
                validSlots.Add(CoatMaskSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Metallic))
            {
                AddSlot(new Vector1MaterialSlot(MetallicSlotId, MetallicSlotName, MetallicSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));
                validSlots.Add(MetallicSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Smoothness))
            {
                AddSlot(new Vector1MaterialSlot(SmoothnessSlotId, SmoothnessSlotName, SmoothnessSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(SmoothnessSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Occlusion))
            {
                AddSlot(new Vector1MaterialSlot(OcclusionSlotId, OcclusionSlotName, OcclusionSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(OcclusionSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Emission))
            {
                AddSlot(new ColorRGBMaterialSlot(EmissionSlotId, EmissionSlotName, EmissionSlotName, SlotType.Input, Color.black, ColorMode.HDR, ShaderStageCapability.Fragment));
                validSlots.Add(EmissionSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.Alpha))
            {
                AddSlot(new Vector1MaterialSlot(AlphaSlotId, AlphaSlotName, AlphaSlotName, SlotType.Input, 1.0f, ShaderStageCapability.Fragment));
                validSlots.Add(AlphaSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.AlphaThreshold) && alphaTest.isOn)
            {
                AddSlot(new Vector1MaterialSlot(AlphaThresholdSlotId, AlphaClipThresholdSlotName, AlphaClipThresholdSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));
                validSlots.Add(AlphaThresholdSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.AlphaThresholdDepthPrepass) && surfaceType == SurfaceType.Transparent && alphaTest.isOn && alphaTestDepthPrepass.isOn)
            {
                AddSlot(new Vector1MaterialSlot(AlphaThresholdDepthPrepassSlotId, AlphaClipThresholdDepthPrepassSlotName, AlphaClipThresholdDepthPrepassSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));
                validSlots.Add(AlphaThresholdDepthPrepassSlotId);
            }
            if (MaterialTypeUsesSlotMask(SlotMask.AlphaThresholdDepthPostpass) && surfaceType == SurfaceType.Transparent && alphaTest.isOn && alphaTestDepthPostpass.isOn)
            {
                AddSlot(new Vector1MaterialSlot(AlphaThresholdDepthPostpassSlotId, AlphaClipThresholdDepthPostpassSlotName, AlphaClipThresholdDepthPostpassSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));
                validSlots.Add(AlphaThresholdDepthPostpassSlotId);
            }

            RemoveSlotsNameNotMatching(validSlots, true);
        }

        protected override VisualElement CreateCommonSettingsElement()
        {
            return new LitSettingsView(this);
        }

        public NeededCoordinateSpace RequiresNormal(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireNormal>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresNormal(stageCapability));
        }

        public NeededCoordinateSpace RequiresPosition(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequirePosition>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresPosition(stageCapability));
        }

        public bool RequiresSplitLighting()
        {
            return materialType == LitMasterNode.MaterialType.SubsurfaceScattering;
        }
    }
}
