using UnityEngine;
using System.Collections;

public class NavTest : MonoBehaviour {

    NavMeshAgent nav;
    LineRenderer line;

    void Start()
    {
        nav = this.GetComponent<NavMeshAgent>();
        line = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update () {
        OnDrawGizmosSelected();
    }

    void OnDrawGizmosSelected()
    {

        if (nav == null)
            return;
        

        if (nav.path == null)
            return;
        
        
        var path = nav.path;

        line.SetVertexCount(path.corners.Length);

        for (int i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]);
        }
    }
}
