using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using allgameColor;

using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class Laser : MonoBehaviour
{
    public static float max_laserlen = 5f;
    const float laserOffset = 0.5f;
    public CGameColors.GameColors color;
    public GameObject JoinedLaser = null;
    private Renderer renderer;
    public Vector3 origin;
    public Vector3 dir;
    public float laserlen;
    public bool attachedtoplayer;
    public Collider[] parent;
    public ParticleSystem ps;
    public GameObject line;

    public void Awake()
    {
        parent = new Collider[2];
        origin = transform.position;
        dir = new Vector3(0.0f, 0, 1.0f);
        laserlen = max_laserlen;
        this.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        ps.startLifetime = laserlen * 1.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (parent[0] != null && parent[1] != null)
        {
            if (parent[0].gameObject.activeInHierarchy == false || parent[1].gameObject.activeInHierarchy == false)
            {
                //this.laserlen = max_laserlen;
                this.parent[0] = null;
                this.parent[1] = null;
                this.transform.position = new Vector3(5000, 5000, 5000);
                this.origin = this.transform.position;
                this.gameObject.SetActive(false);
            }
        }
        if(dir.Equals(new Vector3(0, 0, 0)) || laserlen <= 0)
        {
            return;
        }
        renderer.material.color = CGameColors.getDefColor(color);
        line.GetComponent<MeshRenderer>().material.color = renderer.material.color;
        this.transform.localScale = new Vector3(this.transform.localScale.x, laserlen, this.transform.localScale.z);
        this.transform.rotation = Quaternion.Euler(0, -Mathf.Rad2Deg * Mathf.Atan2(dir.z, dir.x), 0) * Quaternion.Euler(0, 0, 90);
        this.transform.position = origin;
        if (attachedtoplayer)
        {
            this.transform.position += dir * laserlen;
            this.transform.position += dir * laserOffset;
        }
        else
        {
            this.transform.position += dir.normalized * laserlen;
        }
        ps.startLifetime = laserlen * 0.2f;
        ps.startSpeed = 25f;
        //ps.GetComponent<Renderer>().material.color = CGameColors.getDefColor(color);
    }
    
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
        Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        // lineP1 = (a,0,b)
        // lineP2 = (c,0,d)
        // DirP1 = (x,0,z)
        // DirP2 = (i,0,k)
        // lineP1_m = z/x;
        // lineP1_eqn = b = m(a) + c
        // lineP1_c = b - m(a)
        float lineP1_grad = lineVec1.z / lineVec1.x;
        float lineP2_grad = lineVec2.z / lineVec2.x;
        float a1, b1, c1, a2, b2, c2;
        intersection = new Vector3(1000, 1000, 1000);
        // H- horinzontal, V-vertical, D-Diagonal
/*        if (lineVec1.normalized == lineVec2.normalized || lineVec1.normalized == -lineVec2.normalized)
        { // lines with same gradient
            return false;
        }*/
        if (lineVec1.z == 0 && lineVec2.x == 0) //HV
        {
            intersection = new Vector3(linePoint2.x, linePoint1.y, linePoint1.z);
            Debug.Log("HV");
            return true;
        }
        if (lineVec1.x == 0 && lineVec2.z == 0)//VH
        {
            intersection = new Vector3(linePoint1.x, linePoint1.y, linePoint2.z);
            Debug.Log("VH");
            return true;
        }
        if (lineVec2.z == 0) //DH
        {
            c2 = linePoint2.z;
            float m = (c2 - linePoint1.z) / lineVec1.z;
            intersection = new Vector3(m * lineVec1.x + linePoint1.x, linePoint1.y, c2);
            return true;

        }
        if (lineVec2.x == 0) //DV
        {
            c2 = linePoint2.x;
            //float m = c2 / lineVec1.x - linePoint1.x;
            float m = (c2 - linePoint1.x)/lineVec1.x;
            intersection = new Vector3(m * lineVec1.x + linePoint1.x, linePoint1.y, m * lineVec1.z + linePoint1.z);
            return true;
        }
        if (lineVec1.z == 0) //HD
        {
            c2 = linePoint1.z;
            float m = (c2 - linePoint2.z) / lineVec2.z;
            intersection = new Vector3(m * lineVec2.x + linePoint2.x, linePoint1.y, c2);
            return true;
        }
        if (lineVec1.x == 0) //VD
        {
            c2 = linePoint1.x;
            float m = (c2  - linePoint2.x) / lineVec2.x;
            intersection = new Vector3(m * lineVec2.x + linePoint2.x, linePoint1.y, m * lineVec2.z + linePoint2.z);
            return true;
        }
        a1 = lineP1_grad;
        b1 = -1;
        c1 = linePoint1.z - lineP1_grad * linePoint1.x;
        a2 = lineP2_grad;
        b2 = -1;
        c2 = linePoint2.z - lineP2_grad * linePoint2.x;
        float denom = a1 * b2 - a2 * b1;
        float x = (b1 * c2 - b2 * c1) / denom;
        float y = (a2 * c1 - a1 * c2) / denom;
        intersection = new Vector3(x, 1f, y);

        return true;
    }
}
