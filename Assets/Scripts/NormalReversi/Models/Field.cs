using UnityEngine;

namespace NormalReversi.Models
{
    public class Field
    {
        public Field()
        {
            var mesh = new Mesh
            {
                vertices = new[]
                {
                    new Vector3(0, 1f),
                    new Vector3(1f, -1f),
                    new Vector3(-1f, -1f)
                },
                triangles = new[]
                {
                    0, 1, 2
                }
            };

            mesh.RecalculateNormals();
        }
    }
}