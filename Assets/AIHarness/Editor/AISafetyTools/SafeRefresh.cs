using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityCliConnector;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AISafetyTools
{
    [UnityCliTool(
        Name = "safe_refresh",
        Description = "AssetDatabase.Refresh with guards for dirty scenes, Play Mode, compiling, and importing.",
        Group = "safety")]
    public static class SafeRefresh
    {
        public class Parameters
        {
            [ToolParameter("Force script compile/import update. Default false.")]
            public bool Compile { get; set; }
        }

        public static object HandleCommand(JObject parameters)
        {
            var p = new ToolParams(parameters);
            bool compile = p.GetBool("compile", false);

            if (EditorApplication.isPlaying)
            {
                return new ErrorResponse("Play Mode is active. Stop Play Mode before refreshing.");
            }

            if (EditorApplication.isCompiling)
            {
                return new ErrorResponse("Unity is already compiling. Wait and retry.");
            }

            if (EditorApplication.isUpdating)
            {
                return new ErrorResponse("Unity is importing/updating assets. Wait and retry.");
            }

            var dirty = GetDirtyScenes();
            if (dirty.Count > 0)
            {
                return new ErrorResponse(
                    $"Unsaved scene count: {dirty.Count}. Save intentionally before refresh.",
                    new { dirty_scenes = dirty });
            }

            if (compile)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            else
            {
                AssetDatabase.Refresh();
            }

            return new SuccessResponse(compile ? "Refresh with compile requested" : "Refresh requested");
        }

        private static List<string> GetDirtyScenes()
        {
            var dirty = new List<string>();
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (scene.isDirty)
                {
                    dirty.Add(string.IsNullOrEmpty(scene.path) ? scene.name : scene.path);
                }
            }

            return dirty;
        }
    }
}

