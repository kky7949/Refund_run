using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityCliConnector;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AISafetyTools
{
    [UnityCliTool(
        Name = "save_dirty_scenes",
        Description = "Save only currently open dirty scenes. New unnamed scenes are skipped.",
        Group = "safety")]
    public static class SaveDirtyScenes
    {
        public static object HandleCommand(JObject parameters)
        {
            if (EditorApplication.isPlaying)
            {
                return new ErrorResponse("Play Mode is active. Stop Play Mode before saving scenes.");
            }

            var saved = new List<string>();
            var skipped = new List<string>();

            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (!scene.isDirty)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(scene.path))
                {
                    skipped.Add($"{scene.name} has no saved path");
                    continue;
                }

                if (EditorSceneManager.SaveScene(scene))
                {
                    saved.Add(scene.path);
                }
                else
                {
                    skipped.Add($"{scene.path} failed to save");
                }
            }

            return new SuccessResponse(
                $"Saved {saved.Count}; skipped {skipped.Count}",
                new { saved, skipped });
        }
    }
}

