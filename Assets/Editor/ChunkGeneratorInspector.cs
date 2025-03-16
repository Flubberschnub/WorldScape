namespace Editor
{
    using Scripting.Terrain_Generation;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ChunkGenerator))]
// ^ This is the script we are making a custom editor for.
    public class ChunkGeneratorInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            //Called whenever the inspector is drawn for this object.
            DrawDefaultInspector();
            //This draws the default screen.  You don't need this if you want
            //to start from scratch, but I use this when I'm just adding a button or
            //some small addition and don't feel like recreating the whole inspector.

            // Add a button to run the function
            if (GUILayout.Button("Generate Chunks"))
            {
                // When the button is clicked, run the function
                ((ChunkGenerator)target).GenerateChunks();
            }

            if (GUILayout.Button("Clear Chunks"))
            {
                // When the button is clicked, run the function
                ((ChunkGenerator)target).ClearChunks();
            }
        }
    }
}