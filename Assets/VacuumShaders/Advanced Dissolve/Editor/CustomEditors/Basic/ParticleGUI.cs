using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VacuumShaders.AdvancedDissolve
{
    public class ParticleGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            VacuumShaders.AdvancedDissolve.MaterialProperties.Init(materialEditor, properties);


            if (VacuumShaders.AdvancedDissolve.MaterialProperties.DrawSurfaceInputs(materialEditor))
            {
                using (new VacuumEditorGUIUtility.EditorGUIIndentLevel(1))
                {
                    base.OnGUI(materialEditor, properties);
                }
            }


            VacuumShaders.AdvancedDissolve.MaterialProperties.DrawDissolveOptions(materialEditor, false);
        }
    }
}