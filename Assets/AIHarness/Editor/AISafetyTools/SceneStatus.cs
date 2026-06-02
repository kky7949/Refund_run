using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityCliConnector;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AISafetyTools
{
    [UnityCliTool(
        Name = "scene_status",
        Description = "Return open scenes, dirty state, and editor play/compile/update state.",
        Group = "safety")]
    public static class SceneStatus
    {
        public static object HandleCommand(JObject parameters)
        {
            var scenes = new List<object>();
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                scenes.Add(new
                {
                    name = scene.name,
                    path = scene.path,
                    is_dirty = scene.isDirty,
                    is_loaded = scene.isLoaded,
                    root_count = scene.rootCount
                });
            }

            var active = EditorSceneManager.GetActiveScene();
            return new SuccessResponse("Scene status", new
            {
                scenes,
                active_scene = active.name,
                active_scene_path = active.path,
                any_dirty = AnyDirty(),
                is_playing = EditorApplication.isPlaying,
                is_paused = EditorApplication.isPaused,
                is_compiling = EditorApplication.isCompiling,
                is_updating = EditorApplication.isUpdating
            });
        }

        private static bool AnyDirty()
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                if (EditorSceneManager.GetSceneAt(i).isDirty)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

