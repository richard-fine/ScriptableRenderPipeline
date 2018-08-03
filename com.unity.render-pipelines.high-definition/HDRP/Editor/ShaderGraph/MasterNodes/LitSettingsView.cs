using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.ShaderGraph.Drawing
{
    public class LitSettingsView : VisualElement
    {
        LitMasterNode m_Node;

        public LitSettingsView(LitMasterNode node)
        {
            m_Node = node;
            PropertySheet ps = new PropertySheet();

            ps.Add(new PropertyRow(new Label("Surface Type")), (row) =>
            {
                row.Add(new EnumField(SurfaceType.Opaque), (field) =>
                {
                    field.value = m_Node.surfaceType;
                    field.OnValueChanged(ChangeSurfaceType);
                });
            });

            if (m_Node.surfaceType == SurfaceType.Transparent)
            {
                ps.Add(new PropertyRow(new Label("    Blend Mode")), (row) =>
                {
                    row.Add(new EnumField(AlphaMode.Additive), (field) =>
                    {
                        field.value = m_Node.alphaMode;
                        field.OnValueChanged(ChangeBlendMode);
                    });
                });

                ps.Add(new PropertyRow(new Label("    Blend Preserves Specular")), (row) =>
                {
                    row.Add(new Toggle(), (toggle) =>
                    {
                        toggle.value = m_Node.blendPreserveSpecular.isOn;
                        toggle.OnToggleChanged(ChangeBlendPreserveSpecular);
                    });
                });

                ps.Add(new PropertyRow(new Label("    Fog")), (row) =>
                {
                    row.Add(new Toggle(), (toggle) =>
                    {
                        toggle.value = m_Node.transparencyFog.isOn;
                        toggle.OnToggleChanged(ChangeTransparencyFog);
                    });
                });

                //ps.Add(new PropertyRow(new Label("    Draw Before Refraction")), (row) =>
                //{
                //    row.Add(new Toggle(), (toggle) =>
                //    {
                //        toggle.value = m_Node.drawBeforeRefraction.isOn;
                //        toggle.OnToggleChanged(ChangeDrawBeforeRefraction);
                //    });
                //});
            }

            ps.Add(new PropertyRow(new Label("Double Sided")), (row) =>
            {
                row.Add(new EnumField(DoubleSidedMode.Disabled), (field) =>
                {
                    field.value = m_Node.doubleSidedMode;
                    field.OnValueChanged(ChangeDoubleSidedMode);
                });
            });

            ps.Add(new PropertyRow(new Label("Material Type")), (row) =>
            {
                row.Add(new EnumField(LitMasterNode.MaterialType.Standard), (field) =>
                {
                    field.value = m_Node.materialType;
                    field.OnValueChanged(ChangeMaterialType);
                });
            });

            if (m_Node.materialType == LitMasterNode.MaterialType.SubsurfaceScattering)
            {
                ps.Add(new PropertyRow(new Label("    Transmission")), (row) =>
                {
                    row.Add(new Toggle(), (toggle) =>
                    {
                        toggle.value = m_Node.sssTransmission.isOn;
                        toggle.OnToggleChanged(ChangeSSSTransmission);
                    });
                });
            }

            ps.Add(new PropertyRow(new Label("Receive Decals")), (row) =>
            {
                row.Add(new Toggle(), (toggle) =>
                {
                    toggle.value = m_Node.receiveDecals.isOn;
                    toggle.OnToggleChanged(ChangeDecal);
                });
            });

            if (m_Node.materialType == LitMasterNode.MaterialType.SpecularColor)
            {
                ps.Add(new PropertyRow(new Label("Energy Conserving Specular")), (row) =>
                {
                    row.Add(new Toggle(), (toggle) =>
                    {
                        toggle.value = m_Node.energyConservingSpecular.isOn;
                        toggle.OnToggleChanged(ChangeEnergyConservingSpecular);
                    });
                });
            }

            ps.Add(new PropertyRow(new Label("Specular AA")), (row) =>
            {
                row.Add(new Toggle(), (toggle) =>
                {
                    toggle.value = m_Node.specularAA.isOn;
                    toggle.OnToggleChanged(ChangeSpecularAA);
                });
            });

            if (m_Node.specularAA.isOn)
            {
                ps.Add(new PropertyRow(new Label("    Screen Space Variance")), (row) =>
                {
                    Action<float> changed = (s) => { ChangeSpecularAAScreenSpaceVariance(s); };
                    row.Add(new Slider(0.0f, 1.0f, changed), (slider) =>
                    {
                        slider.value = m_Node.specularAAScreenSpaceVariance;
                    });
                });

                ps.Add(new PropertyRow(new Label("    Threshold")), (row) =>
                {
                    Action<float> changed = (s) => { ChangeSpecularAAThreshold(s); };
                    row.Add(new Slider(0.0f, 1.0f, changed), (slider) =>
                    {
                        slider.value = m_Node.specularAAThreshold;
                    });
                });
            }

            ps.Add(new PropertyRow(new Label("Motion Vectors For Vertex Animation")), (row) =>
            {
                row.Add(new Toggle(), (toggle) =>
                {
                    toggle.value = m_Node.motionVectors.isOn;
                    toggle.OnToggleChanged(ChangeMotionVectors);
                });
            });

            //ps.Add(new PropertyRow(new Label("UV Channel")), (row) =>
            //{
            //    row.Add(new EnumField(UVChannel.UV0), (field) =>
            //    {
            //        field.value = m_Node.uvChannel;
            //        field.OnValueChanged(ChangeUVChannel);
            //    });
            //});

            ps.Add(new PropertyRow(new Label("Albedo Affects Emissive")), (row) =>
            {
                row.Add(new Toggle(), (toggle) =>
                {
                    toggle.value = m_Node.albedoAffectsEmissive.isOn;
                    toggle.OnToggleChanged(ChangeAlbedoAffectsEmissive);
                });
            });

            //ps.Add(new PropertyRow(new Label("GPU Instancing")), (row) =>
            //{
            //    row.Add(new Toggle(), (toggle) =>
            //    {
            //        toggle.value = m_Node.gpuInstancing.isOn;
            //        toggle.OnToggleChanged(ChangeGPUInstancing);
            //    });
            //});

            ps.Add(new PropertyRow(new Label("Specular Occlusion From Bent Normal ")), (row) =>
            {
                row.Add(new Toggle(), (toggle) =>
                {
                    toggle.value = m_Node.bentNormalSpecularOcclusion.isOn;
                    toggle.OnToggleChanged(ChangeBentNormalSpecularOcclusion);
                });
            });

            Add(ps);
        }

        void ChangeSurfaceType(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.surfaceType, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("Surface Type Change");
            m_Node.surfaceType = (SurfaceType)evt.newValue;
        }

        void ChangeDoubleSidedMode(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.doubleSidedMode, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("Double Sided Mode Change");
            m_Node.doubleSidedMode = (DoubleSidedMode)evt.newValue;
        }

        void ChangeMaterialType(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.materialType, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("Material Type Change");
            m_Node.materialType = (LitMasterNode.MaterialType)evt.newValue;
        }

        void ChangeSSSTransmission(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("SSS Transmission Change");
            ToggleData td = m_Node.sssTransmission;
            td.isOn = evt.newValue;
            m_Node.sssTransmission = td;
        }

        void ChangeBlendMode(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.alphaMode, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("Alpha Mode Change");
            m_Node.alphaMode = (AlphaMode)evt.newValue;
        }

        void ChangeBlendPreserveSpecular(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Blend Preserve Specular Change");
            ToggleData td = m_Node.blendPreserveSpecular;
            td.isOn = evt.newValue;
            m_Node.blendPreserveSpecular = td;
        }

        void ChangeTransparencyFog(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Transparency Fog Change");
            ToggleData td = m_Node.transparencyFog;
            td.isOn = evt.newValue;
            m_Node.transparencyFog = td;
        }

        void ChangeDrawBeforeRefraction(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Draw Before Refraction Change");
            ToggleData td = m_Node.drawBeforeRefraction;
            td.isOn = evt.newValue;
            m_Node.drawBeforeRefraction = td;
        }

        void ChangeDecal(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Decal Change");
            ToggleData td = m_Node.receiveDecals;
            td.isOn = evt.newValue;
            m_Node.receiveDecals = td;
        }

        void ChangeSpecularAA(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Specular AA Change");
            ToggleData td = m_Node.specularAA;
            td.isOn = evt.newValue;
            m_Node.specularAA = td;
        }

        void ChangeSpecularAAScreenSpaceVariance(float value)
        {
            if (Equals(m_Node.specularAAScreenSpaceVariance, value))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("Specular AA Screen Space Variance Change");
            m_Node.specularAAScreenSpaceVariance = value;
        }

        void ChangeSpecularAAThreshold(float value)
        {
            if (Equals(m_Node.specularAAThreshold, value))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("Specular AA Threshold Change");
            m_Node.specularAAThreshold = value;
        }

        void ChangeEnergyConservingSpecular(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Energy Conserving Specular Change");
            ToggleData td = m_Node.energyConservingSpecular;
            td.isOn = evt.newValue;
            m_Node.energyConservingSpecular = td;
        }

        void ChangeMotionVectors(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Motion Vectors Change");
            ToggleData td = m_Node.motionVectors;
            td.isOn = evt.newValue;
            m_Node.motionVectors = td;
        }

        void ChangeUVChannel(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.uvChannel, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("UV Channel Change");
            m_Node.uvChannel = (UVChannel)evt.newValue;
        }

        void ChangeAlbedoAffectsEmissive(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Albedo Affects Emissive Change");
            ToggleData td = m_Node.albedoAffectsEmissive;
            td.isOn = evt.newValue;
            m_Node.albedoAffectsEmissive = td;
        }

        void ChangeGPUInstancing(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("GPU Instancing Change");
            ToggleData td = m_Node.gpuInstancing;
            td.isOn = evt.newValue;
            m_Node.gpuInstancing = td;
        }

        void ChangeBentNormalSpecularOcclusion(ChangeEvent<bool> evt)
        {
            m_Node.owner.owner.RegisterCompleteObjectUndo("Bent Normal Specular Occlusion Change");
            ToggleData td = m_Node.bentNormalSpecularOcclusion;
            td.isOn = evt.newValue;
            m_Node.bentNormalSpecularOcclusion = td;
        }
    }
}
