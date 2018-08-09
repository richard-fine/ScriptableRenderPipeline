using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    public class HDLitSubShader : ILitSubShader
    {
        Pass m_PassGBuffer = new Pass()
        {
            Name = "GBuffer",
            LightMode = "GBuffer",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_GBUFFER",

            ExtraDefines = new List<string>()
            {
                "#pragma multi_compile _ DEBUG_DISPLAY",
                "#pragma multi_compile _ LIGHTMAP_ON",
                "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassGBuffer.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
                "FragInputs.worldToTangent",
                "FragInputs.positionRWS",
                "FragInputs.texCoord1",
                "FragInputs.texCoord2"
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlbedoSlotId,
                LitMasterNode.NormalSlotId,
                LitMasterNode.BentNormalSlotId,
                LitMasterNode.TangentSlotId,
                LitMasterNode.SubsurfaceMaskSlotId,
                LitMasterNode.ThicknessSlotId,
                LitMasterNode.DiffusionProfileSlotId,
                LitMasterNode.IridescenceMaskSlotId,
                LitMasterNode.IridescenceThicknessSlotId,
                LitMasterNode.SpecularSlotId,
                LitMasterNode.CoatMaskSlotId,
                LitMasterNode.MetallicSlotId,
                LitMasterNode.EmissionSlotId,
                LitMasterNode.SmoothnessSlotId,
                LitMasterNode.OcclusionSlotId,
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId,
                LitMasterNode.AnisotropySlotId,
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            }
        };

        Pass m_PassGBufferWithPrepass = new Pass()
        {
            Name = "GBufferWithPrepass",
            LightMode = "GBufferWithPrepass",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_GBUFFER",
            ExtraDefines = new List<string>()
            {
                "#pragma multi_compile _ DEBUG_DISPLAY",
                "#pragma multi_compile _ LIGHTMAP_ON",
                "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                "#define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST",
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassGBuffer.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
//                "FragInputs.worldToTangent",
//                "FragInputs.positionRWS",
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlbedoSlotId,
                LitMasterNode.NormalSlotId,
                LitMasterNode.BentNormalSlotId,
                LitMasterNode.TangentSlotId,
                LitMasterNode.SubsurfaceMaskSlotId,
                LitMasterNode.ThicknessSlotId,
                LitMasterNode.DiffusionProfileSlotId,
                LitMasterNode.IridescenceMaskSlotId,
                LitMasterNode.IridescenceThicknessSlotId,
                LitMasterNode.SpecularSlotId,
                LitMasterNode.CoatMaskSlotId,
                LitMasterNode.MetallicSlotId,
                LitMasterNode.EmissionSlotId,
                LitMasterNode.SmoothnessSlotId,
                LitMasterNode.OcclusionSlotId,
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId,
                LitMasterNode.AnisotropySlotId,
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            }
        };

        Pass m_PassMETA = new Pass()
        {
            Name = "META",
            LightMode = "Meta",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_LIGHT_TRANSPORT",
            ExtraDefines = new List<string>()
            {
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            CullOverride = "Cull Off",
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassLightTransport.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
                "AttributesMesh.normalOS",
                "AttributesMesh.tangentOS",     // Always present as we require it also in case of Variants lighting
                "AttributesMesh.uv0",
                "AttributesMesh.uv1",
                "AttributesMesh.color",
                "AttributesMesh.uv2",           // SHADERPASS_LIGHT_TRANSPORT always uses uv2

                // Need these also?
                "FragInputs.worldToTangent",
                "FragInputs.positionRWS",
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlbedoSlotId,
                LitMasterNode.NormalSlotId,
                LitMasterNode.BentNormalSlotId,
                LitMasterNode.TangentSlotId,
                LitMasterNode.SubsurfaceMaskSlotId,
                LitMasterNode.ThicknessSlotId,
                LitMasterNode.DiffusionProfileSlotId,
                LitMasterNode.IridescenceMaskSlotId,
                LitMasterNode.IridescenceThicknessSlotId,
                LitMasterNode.SpecularSlotId,
                LitMasterNode.CoatMaskSlotId,
                LitMasterNode.MetallicSlotId,
                LitMasterNode.EmissionSlotId,
                LitMasterNode.SmoothnessSlotId,
                LitMasterNode.OcclusionSlotId,
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId,
                LitMasterNode.AnisotropySlotId,
            },
            VertexShaderSlots = new List<int>()
            {
                //LitMasterNode.PositionSlotId
            }
        };

        Pass m_PassShadowCaster = new Pass()
        {
            Name = "ShadowCaster",
            LightMode = "ShadowCaster",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_SHADOWS",
            ColorMaskOverride = "ColorMask 0",
            ExtraDefines = new List<string>()
            {
                "#define USE_LEGACY_UNITY_MATRIX_VARIABLES",
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassDepthOnly.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
//                "FragInputs.worldToTangent",
//                "FragInputs.positionRWS",
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            }
        };

        Pass m_PassDepthOnly = new Pass()
        {
            Name = "DepthOnly",
            LightMode = "DepthOnly",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_DEPTH_ONLY",
            ExtraDefines = new List<string>()
            {
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            ColorMaskOverride = "ColorMask 0",
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassDepthOnly.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
//                "FragInputs.worldToTangent",
//                "FragInputs.positionRWS",
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            }
        };

        Pass m_PassMotionVectors = new Pass()
        {
            Name = "Motion Vectors",
            LightMode = "MotionVectors",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_VELOCITY",
            ExtraDefines = new List<string>()
            {
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassVelocity.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
                "FragInputs.positionRWS",
            },
            StencilOverride = new List<string>()
            {
                "// If velocity pass (motion vectors) is enabled we tag the stencil so it don't perform CameraMotionVelocity",
                "Stencil",
                "{",
                "   WriteMask 128",         // [_StencilWriteMaskMV]        (int) HDRenderPipeline.StencilBitMask.ObjectVelocity   // this requires us to pull in the HD Pipeline assembly...
                "   Ref 128",               // [_StencilRefMV]
                "   Comp Always",
                "   Pass Replace",
                "}"
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            },
        };

        Pass m_PassDistortion = new Pass()
        {
            Name = "Distortion",
            LightMode = "DistortionVectors",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_DISTORTION",
            ExtraDefines = new List<string>()
            {
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            BlendOverride = "Blend One One, One One",   // [_DistortionSrcBlend] [_DistortionDstBlend], [_DistortionBlurSrcBlend] [_DistortionBlurDstBlend]
            BlendOpOverride = "BlendOp Add, Add",       // Add, [_DistortionBlurBlendOp]
            ZTestOverride = "ZTest LEqual",             // [_ZTestModeDistortion]
            ZWriteOverride = "ZWrite Off",
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassDistortion.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
//                "FragInputs.worldToTangent",
//                "FragInputs.positionRWS",
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            },
        };

        Pass m_PassTransparentDepthPrepass = new Pass()
        {
            Name = "TransparentDepthPrepass",
            LightMode = "TransparentDepthPrepass",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_DEPTH_ONLY",
            ColorMaskOverride = "ColorMask 0",
            ExtraDefines = new List<string>()
            {
                "#define CUTOFF_TRANSPARENT_DEPTH_PREPASS",
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassDepthOnly.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
//                "FragInputs.worldToTangent",
//                "FragInputs.positionRWS",
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            },
        };

        Pass m_PassTransparentBackface = new Pass()
        {
            Name = "TransparentBackface",
            LightMode = "TransparentBackface",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_FORWARD",
            CullOverride = "Cull Front",
            ExtraDefines = new List<string>()
            {
                "#pragma multi_compile _ DEBUG_DISPLAY",
                "#pragma multi_compile _ LIGHTMAP_ON",
                "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                "#define LIGHTLOOP_TILE_PASS",
                //"#pragma multi_compile LIGHTLOOP_SINGLE_PASS LIGHTLOOP_TILE_PASS",
                "#pragma multi_compile USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST",
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassForward.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
                "FragInputs.worldToTangent",
                "FragInputs.positionRWS",
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlbedoSlotId,
                LitMasterNode.NormalSlotId,
                LitMasterNode.BentNormalSlotId,
                LitMasterNode.TangentSlotId,
                LitMasterNode.SubsurfaceMaskSlotId,
                LitMasterNode.ThicknessSlotId,
                LitMasterNode.DiffusionProfileSlotId,
                LitMasterNode.IridescenceMaskSlotId,
                LitMasterNode.IridescenceThicknessSlotId,
                LitMasterNode.SpecularSlotId,
                LitMasterNode.CoatMaskSlotId,
                LitMasterNode.MetallicSlotId,
                LitMasterNode.EmissionSlotId,
                LitMasterNode.SmoothnessSlotId,
                LitMasterNode.OcclusionSlotId,
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId,
                LitMasterNode.AnisotropySlotId,
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            },
        };

        Pass m_PassForward = new Pass()
        {
            Name = "Forward",
            LightMode = "Forward",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_FORWARD",
            ExtraDefines = new List<string>()
            {
                "#pragma multi_compile _ DEBUG_DISPLAY",
                "#pragma multi_compile _ LIGHTMAP_ON",
                "#pragma multi_compile _ DIRLIGHTMAP_COMBINED",
                "#pragma multi_compile _ DYNAMICLIGHTMAP_ON",
                "#pragma multi_compile _ SHADOWS_SHADOWMASK",
                "#define LIGHTLOOP_TILE_PASS",
                //"#pragma multi_compile LIGHTLOOP_SINGLE_PASS LIGHTLOOP_TILE_PASS",
                "#pragma multi_compile USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST",
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassForward.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
                "FragInputs.worldToTangent",
                "FragInputs.positionRWS",
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlbedoSlotId,
                LitMasterNode.NormalSlotId,
                LitMasterNode.BentNormalSlotId,
                LitMasterNode.TangentSlotId,
                LitMasterNode.SubsurfaceMaskSlotId,
                LitMasterNode.ThicknessSlotId,
                LitMasterNode.DiffusionProfileSlotId,
                LitMasterNode.IridescenceMaskSlotId,
                LitMasterNode.IridescenceThicknessSlotId,
                LitMasterNode.SpecularSlotId,
                LitMasterNode.CoatMaskSlotId,
                LitMasterNode.MetallicSlotId,
                LitMasterNode.EmissionSlotId,
                LitMasterNode.SmoothnessSlotId,
                LitMasterNode.OcclusionSlotId,
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId,
                LitMasterNode.AnisotropySlotId,
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            },
        };

        Pass m_PassTransparentDepthPostpass = new Pass()
        {
            Name = "TransparentDepthPostpass",
            LightMode = "TransparentDepthPostpass",
            TemplateName = "HDPBRPass.template",
            ShaderPassName = "SHADERPASS_DEPTH_ONLY",
            ColorMaskOverride = "ColorMask 0",
            ExtraDefines = new List<string>()
            {
                "#define CUTOFF_TRANSPARENT_DEPTH_POSTPASS",
                "#pragma multi_compile_instancing",
                "#pragma instancing_options renderinglayer",
            },
            Includes = new List<string>()
            {
                "#include \"HDRP/ShaderPass/ShaderPassDepthOnly.hlsl\"",
            },
            RequiredFields = new List<string>()
            {
//                "FragInputs.worldToTangent",
//                "FragInputs.positionRWS",
            },
            PixelShaderSlots = new List<int>()
            {
                LitMasterNode.AlphaSlotId,
                LitMasterNode.AlphaThresholdSlotId
            },
            VertexShaderSlots = new List<int>()
            {
                LitMasterNode.PositionSlotId
            },
        };

        private static HashSet<string> GetActiveFieldsFromMasterNode(INode iMasterNode, Pass pass)
        {
            HashSet<string> activeFields = new HashSet<string>();

            LitMasterNode masterNode = iMasterNode as LitMasterNode;
            if (masterNode == null)
            {
                return activeFields;
            }

            if (masterNode.doubleSidedMode != DoubleSidedMode.Disabled)
            {
                activeFields.Add("DoubleSided");
                if (pass.ShaderPassName != "SHADERPASS_VELOCITY")   // HACK to get around lack of a good interpolator dependency system
                {                                                   // we need to be able to build interpolators using multiple input structs
                                                                    // also: should only require isFrontFace if Normals are required...
                    if (masterNode.doubleSidedMode == DoubleSidedMode.FlippedNormals)
                    {
                        activeFields.Add("DoubleSided.Flip");
                    }
                    else if (masterNode.doubleSidedMode == DoubleSidedMode.MirroredNormals)
                    {
                        activeFields.Add("DoubleSided.Mirror");
                    }
                        
                    activeFields.Add("FragInputs.isFrontFace");     // will need this for determining normal flip mode
                }
            }

            switch (masterNode.materialType)
            {
                case LitMasterNode.MaterialType.Anisoptropy:
                    activeFields.Add("Material.Anisotropy");
                    break;
                case LitMasterNode.MaterialType.Iridescence:
                    activeFields.Add("Material.Iridescence");
                    break;
                case LitMasterNode.MaterialType.SpecularColor:
                    activeFields.Add("Material.SpecularColor");
                    break;
                case LitMasterNode.MaterialType.Standard:
                    activeFields.Add("Material.Standard");
                    break;
                case LitMasterNode.MaterialType.SubsurfaceScattering:
                    {
                        activeFields.Add("Material.SubsurfaceScattering");
                        if (masterNode.sssTransmission.isOn)
                        {
                            activeFields.Add("Material.Transmission");
                        }
                    }
                    break;
                case LitMasterNode.MaterialType.Translucent:
                    {
                        activeFields.Add("Material.Translucent");
                        activeFields.Add("Material.Transmission");
                    }
                    break;
                default:
                    // TODO: error!
                    break;
            }

            if (pass.PixelShaderUsesSlot(LitMasterNode.AlphaThresholdSlotId))
            {
                float constantAlpha = 0.0f;

                if (masterNode.IsSlotConnected(LitMasterNode.AlphaThresholdSlotId) ||
                    (float.TryParse(masterNode.GetSlotValue(LitMasterNode.AlphaThresholdSlotId, GenerationMode.ForReals), out constantAlpha) && (constantAlpha > 0.0f)))
                {
                    activeFields.Add("AlphaTest");
                }
            }

            // Keywords for transparent
            // #pragma shader_feature _SURFACE_TYPE_TRANSPARENT
            if (masterNode.surfaceType != SurfaceType.Opaque)
            {
                // transparent-only defines
                activeFields.Add("SurfaceType.Transparent");

                if (masterNode.alphaMode == AlphaMode.Alpha)
                {
                    activeFields.Add("BlendMode.Alpha");
                }
                else if (masterNode.alphaMode == AlphaMode.Premultiply)
                {
                    activeFields.Add("BlendMode.Premultiply");
                }
                else if (masterNode.alphaMode == AlphaMode.Additive)
                {
                    activeFields.Add("BlendMode.Add");
                }

                if (masterNode.blendPreserveSpecular.isOn)
                {
                    activeFields.Add("BlendMode.PreserveSpecular");
                }

                if (masterNode.transparencyFog.isOn)
                {
                    activeFields.Add("AlphaFog");
                }
            }

            // enable dithering LOD crossfade
            // #pragma multi_compile _ LOD_FADE_CROSSFADE
            // TODO: We should have this keyword only if VelocityInGBuffer is enable, how to do that ?
            //#pragma multi_compile VELOCITYOUTPUT_OFF VELOCITYOUTPUT_ON

            if (masterNode.receiveDecals.isOn)
            {
                activeFields.Add("Decals");
            }

            if (masterNode.specularAA.isOn)
            {
                activeFields.Add("Specular.AA");
            }

            if (masterNode.energyConservingSpecular.isOn)
            {
                activeFields.Add("Specular.EnergyConserving");
            }

            if (masterNode.albedoAffectsEmissive.isOn)
            {
                activeFields.Add("AlbedoAffectsEmissive");
            }

            if (masterNode.IsSlotConnected(LitMasterNode.BentNormalSlotId) && pass.PixelShaderUsesSlot(LitMasterNode.BentNormalSlotId))
            {
                activeFields.Add("BentNormal");
                if (masterNode.bentNormalSpecularOcclusion.isOn)
                {
                    activeFields.Add("BentNormalSpecularOcclusion");
                }
            }

            if (pass.PixelShaderUsesSlot(LitMasterNode.OcclusionSlotId))
            {
                // This check could be more elegant.
                float ambientOcclusion = 0.0f;
                float defaultAmbientOcclusion = 0.0f;
                float.TryParse(masterNode.GetSlotValue(LitMasterNode.OcclusionSlotId, GenerationMode.ForReals), out ambientOcclusion);
                float.TryParse(masterNode.GetSlotValue(LitMasterNode.OcclusionSlotId, GenerationMode.ForReals), out defaultAmbientOcclusion);
                if (ambientOcclusion != defaultAmbientOcclusion)
                {
                    activeFields.Add("Occlusion");
                }
            }

            return activeFields;
        }

        private static List<string> GetActiveUniformsFromMasterNode(INode iMasterNode, Pass pass)
        {
            var masterNode = iMasterNode as LitMasterNode;

            List<string> uniforms = new List<string>();

            if (masterNode.specularAA.isOn)
            {
                // This is a short term measure for settings, probably need to be able to quickly update these setting in preview (via MaterialPropertyBlock?).
                // Adjusting sliders currently forces re-parsing/compilation. Eek.
                uniforms.Add("#define _SpecularAAScreenSpaceVariance (" + masterNode.specularAAScreenSpaceVariance.ToString() + ")");
                uniforms.Add("#define _SpecularAAThreshold (" + masterNode.specularAAThreshold.ToString() + ")");
            }

            return uniforms;
        }

        private static bool GenerateShaderPassLit(AbstractMaterialNode masterNode, Pass pass, GenerationMode mode, SurfaceMaterialOptions materialOptions, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
        {
            // apply master node options to active fields
            HashSet<string> activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);
            List<string> activeUniforms = GetActiveUniformsFromMasterNode(masterNode, pass);

            // use standard shader pass generation
            return HDSubShaderUtilities.GenerateShaderPass(masterNode, pass, mode, materialOptions, activeFields, activeUniforms, result, sourceAssetDependencyPaths);
        }

        void UpdateCullOverrides(LitMasterNode masterNode)
        {
            if (masterNode.backThenFrontRendering.isOn)
            {
                m_PassForward.CullOverride = "Cull Back";
            }
        }

        // Move all this down into the template?
        void UpdateStencilOverrides(LitMasterNode masterNode)
        {
            string stencilRef = masterNode.RequiresSplitLighting() ? "   Ref  1" : "   Ref  2";

            m_PassGBuffer.StencilOverride = new List<string>()
            {
                "// Stencil setup for gbuffer",
                "Stencil",
                "{",
                "   WriteMask 7",
                    stencilRef,
                "   Comp Always",
                "   Pass Replace",
                "}"
            };

            // Is this wired up anywhere?
            m_PassGBufferWithPrepass.StencilOverride = new List<string>()
            {
                "// Stencil setup for GBufferWithPrepass",
                "Stencil",
                "{",
                "   WriteMask 7",
                    stencilRef,
                "   Comp Always",
                "   Pass Replace",
                "}"
            };

            m_PassForward.StencilOverride = new List<string>()
            {
                "// Stencil setup for forward",
                "Stencil",
                "{",
                "   WriteMask 7",
                    stencilRef,
                "   Comp Always",
                "   Pass Replace",
                "}"
            };
        }

        public string GetSubshader(IMasterNode iMasterNode, GenerationMode mode, List<string> sourceAssetDependencyPaths = null)
        {
            if (sourceAssetDependencyPaths != null)
            {
                // HDLitSubShader.cs
                sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("4f0bba2c9470edc4c8873e124ec9ecb6"));
                // HDSubShaderUtilities.cs
                sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("713ced4e6eef4a44799a4dd59041484b"));
            }

            var masterNode = iMasterNode as LitMasterNode;

            UpdateCullOverrides(masterNode);
            UpdateStencilOverrides(masterNode);

            var subShader = new ShaderGenerator();
            subShader.AddShaderChunk("SubShader", true);
            subShader.AddShaderChunk("{", true);
            subShader.Indent();
            {
                SurfaceMaterialOptions materialOptions = HDSubShaderUtilities.BuildMaterialOptions(masterNode.surfaceType, masterNode.alphaMode, masterNode.doubleSidedMode != DoubleSidedMode.Disabled);

                // Add tags at the SubShader level
                {
                    var tagsVisitor = new ShaderStringBuilder();
                    materialOptions.GetTags(tagsVisitor);
                    subShader.AddShaderChunk(tagsVisitor.ToString(), false);
                }

                // generate the necessary shader passes
                bool opaque = (masterNode.surfaceType == SurfaceType.Opaque);
                bool transparent = (masterNode.surfaceType != SurfaceType.Opaque);
                bool velocity = masterNode.motionVectors.isOn;

                bool distortionActive = false;
                bool transparentDepthPrepassActive = transparent && false;
                bool transparentBackfaceActive = transparent && masterNode.backThenFrontRendering.isOn;
                bool transparentDepthPostpassActive = transparent && false;

                if (opaque)
                {
                    GenerateShaderPassLit(masterNode, m_PassGBuffer, mode, materialOptions, subShader, sourceAssetDependencyPaths);
                    GenerateShaderPassLit(masterNode, m_PassGBufferWithPrepass, mode, materialOptions, subShader, sourceAssetDependencyPaths);
                }

                GenerateShaderPassLit(masterNode, m_PassMETA, mode, materialOptions, subShader, sourceAssetDependencyPaths);
                GenerateShaderPassLit(masterNode, m_PassShadowCaster, mode, materialOptions, subShader, sourceAssetDependencyPaths);

                if (opaque)
                {
                    GenerateShaderPassLit(masterNode, m_PassDepthOnly, mode, materialOptions, subShader, sourceAssetDependencyPaths);
                }

                if (velocity)
                {
                    GenerateShaderPassLit(masterNode, m_PassMotionVectors, mode, materialOptions, subShader, sourceAssetDependencyPaths);
                }

                if (distortionActive)
                {
                    GenerateShaderPassLit(masterNode, m_PassDistortion, mode, materialOptions, subShader, sourceAssetDependencyPaths);
                }

                if (transparentDepthPrepassActive)
                {
                    GenerateShaderPassLit(masterNode, m_PassTransparentDepthPrepass, mode, materialOptions, subShader, sourceAssetDependencyPaths);
                }

                if (transparentBackfaceActive)
                {
                    GenerateShaderPassLit(masterNode, m_PassTransparentBackface, mode, materialOptions, subShader, sourceAssetDependencyPaths);
                }

                GenerateShaderPassLit(masterNode, m_PassForward, mode, materialOptions, subShader, sourceAssetDependencyPaths);

                if (transparentDepthPostpassActive)
                {
                    GenerateShaderPassLit(masterNode, m_PassTransparentDepthPostpass, mode, materialOptions, subShader, sourceAssetDependencyPaths);
                }
            }
            subShader.Deindent();
            subShader.AddShaderChunk("}", true);

            return subShader.GetShaderString(0);
        }

        public bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset)
        {
            return renderPipelineAsset is HDRenderPipelineAsset;
        }
    }
}
