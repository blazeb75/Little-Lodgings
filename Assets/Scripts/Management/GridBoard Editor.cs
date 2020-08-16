using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridBoard))]
[CanEditMultipleObjects]
public class GridBoardEditor : Editor
{

    private void OnDrawGizmos()
    {
        GridBoard target = base.target as GridBoard;
        foreach(Node node in target.nodes)
        {
            Gizmos.DrawWireSphere(node.transform.position, 0.25f);
        }
    }
}
