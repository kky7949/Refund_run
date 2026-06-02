using Newtonsoft.Json.Linq;
using UnityCliConnector;
using UnityEditor;

namespace AIInspectionTools
{
    [UnityCliTool(
        Name = "inspect_selection",
        Description = "Read-only summary of the active Unity selection. Scene GameObject wins over Project asset/folder selection.",
        Group = "inspection")]
    public static class InspectSelection
    {
        public class Parameters
        {
            [ToolParameter("Child output limit. Default 80.")]
            public int ChildLimit { get; set; }

            [ToolParameter("Serialized field output limit per component. Default 32.")]
            public int FieldLimit { get; set; }
        }

        public static object HandleCommand(JObject parameters)
        {
            var p = new ToolParams(parameters);
            int childLimit = p.GetInt("child_limit", 80) ?? 80;
            int fieldLimit = p.GetInt("field_limit", 32) ?? 32;

            UnityEngine.Object selected = Selection.activeObject;
            string source = null;

            bool selectionIsSceneObject = selected != null
                && InspectionUtility.GetSelectedGameObject(selected) != null
                && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(selected));

            if (selectionIsSceneObject)
            {
                source = "selection_scene";
            }
            else
            {
                string folderPath = InspectionUtility.TryGetActiveProjectFolderPath();
                if (!string.IsNullOrEmpty(folderPath))
                {
                    UnityEngine.Object folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(folderPath);
                    if (folder != null)
                    {
                        selected = folder;
                        source = "project_browser_folder";
                    }
                }

                if (source == null && selected != null)
                {
                    source = "selection_asset";
                }
            }

            if (selected == null)
            {
                return new ErrorResponse("No active object or asset selection.");
            }

            var gameObject = InspectionUtility.GetSelectedGameObject(selected);
            object data = gameObject != null
                ? InspectionUtility.DescribeGameObject(gameObject, childLimit, fieldLimit)
                : InspectionUtility.DescribeAsset(selected);

            return new SuccessResponse("Selection inspected", new
            {
                active = InspectionUtility.DescribeObject(selected),
                source,
                selection_count = Selection.objects.Length,
                data
            });
        }
    }
}

