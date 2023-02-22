using UnityEngine;
using UnityEditor;

namespace UnityVolumeRendering
{
    public class SlicingPlaneEditorWindow : EditorWindow
    {
        bool last_show_status = false;
        bool show_display = true;
        private void OnGUI()
        {
            VolumeRenderedObject[] spawnedObjects = GameObject.FindObjectsOfType<VolumeRenderedObject>();
            if (spawnedObjects.Length == 0)
            {
                EditorGUILayout.LabelField("Please load a dataset first.");
            }
            else
            {
                foreach (VolumeRenderedObject volobj in spawnedObjects)
                {
                    if (GUILayout.Button(volobj.gameObject.name))
                        volobj.CreateSlicingPlane();
                }
            }
            show_display = GUILayout.Toggle(show_display, "Show Display Rack");
            if (show_display != last_show_status)
            {
                if (show_display)
                    DisplayRackFactory.AttachToRack(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD, "Slicing Planes");
                else
                    DisplayRackFactory.DeAttachFromRack(DisplayRackFactory.DisplayRacks.ROOM_LEFT_BOARD);
                last_show_status = show_display;
            }
        }
    }
}
