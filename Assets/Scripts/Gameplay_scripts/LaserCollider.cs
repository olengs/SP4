using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using allgameColor;
public class LaserCollider : MonoBehaviour
{
    private Laser laser;
    private float laserdistconst = Laser.max_laserlen / 0.51f;
    public Collider selfcollider;

    public void Start()
    {
        laser = GetComponentInParent<Laser>();
        selfcollider = GetComponent<Collider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Collider")
        {
            Laser otherlaser = other.GetComponentInParent<Laser>();
            if (other == laser.parent[0] || other == laser.parent[1]
                || selfcollider == otherlaser.parent[0] || selfcollider == otherlaser.parent[1])
            {
                return;
            }
            float laserlen, otherlaserlen;
            Vector3 intersect;
            if (Checkforintersection(ref otherlaser, out laserlen, out otherlaserlen, out intersect))
            {
                otherlaser.laserlen = otherlaserlen * 0.51f;
                laser.laserlen = laserlen * 0.51f;
                if (laser.JoinedLaser == null)
                {
                    laser.JoinedLaser = LaserPool.SharedInstance.FetchLaser();
                    otherlaser.JoinedLaser = laser.JoinedLaser;
                    if (laser.JoinedLaser == null) return;
                    laser.JoinedLaser.GetComponent<Laser>().attachedtoplayer = false;
                }
                Laser joinedlaser = laser.JoinedLaser.GetComponent<Laser>();
                joinedlaser.parent[0] = this.GetComponentInChildren<Collider>();
                joinedlaser.parent[1] = other;
                joinedlaser.color = CGameColors.mixColor(laser.color, otherlaser.color);
                joinedlaser.origin = intersect;
                joinedlaser.dir = (laser.dir + otherlaser.dir).normalized;
                joinedlaser.laserlen = Laser.max_laserlen;
            }
            else
            {
                if (laser.JoinedLaser != null)
                    clearlaser(otherlaser, laser.JoinedLaser.GetComponent<Laser>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Collider")
        {
            Laser otherlaser = other.GetComponentInParent<Laser>();
            if (other == laser.parent[0] || other == laser.parent[1]
                || selfcollider == otherlaser.parent[0] || selfcollider == otherlaser.parent[1])
            {
                return;
            }
            if (laser.JoinedLaser != null)
                clearlaser(other.GetComponentInParent<Laser>(), laser.JoinedLaser.GetComponent<Laser>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Collider")
        {
            Laser otherlaser = other.GetComponentInParent<Laser>();
            if (other == laser.parent[0] || other == laser.parent[1]
                || selfcollider == otherlaser.parent[0] || selfcollider == otherlaser.parent[1])
            {
                return;
            }
            float laserlen, otherlaserlen;
            Vector3 intersect;
            if (Checkforintersection(ref otherlaser, out laserlen, out otherlaserlen, out intersect))
            {
                otherlaser.laserlen = otherlaserlen * 0.51f;
                laser.laserlen = laserlen * 0.51f;
                //Debug.Log(laserlen + "," + otherlaserlen);
                if (laser.JoinedLaser == null)
                {
                    laser.JoinedLaser = LaserPool.SharedInstance.FetchLaser();
                    otherlaser.JoinedLaser = laser.JoinedLaser;
                    if (laser.JoinedLaser == null) return;
                    laser.JoinedLaser.GetComponent<Laser>().attachedtoplayer = false;
                }
                Laser joinedlaser = laser.JoinedLaser.GetComponent<Laser>();
                joinedlaser.parent[0] = this.GetComponentInChildren<Collider>();
                joinedlaser.parent[1] = other;
                joinedlaser.color = CGameColors.mixColor(laser.color, otherlaser.color);
                joinedlaser.origin = intersect;
                joinedlaser.dir = (laser.dir + otherlaser.dir).normalized;
                joinedlaser.laserlen = Laser.max_laserlen;
            }
            else
            {
                if (laser.JoinedLaser != null)
                    clearlaser(otherlaser, laser.JoinedLaser.GetComponent<Laser>());
            }
        }
    }

    private bool Checkforintersection(ref Laser other, out float laserlen, out float otherlaserlen, out Vector3 intersect)
    {
        Laser.LineLineIntersection(out intersect, laser.origin, laser.dir, other.origin, other.dir);
        otherlaserlen = Vector3.Dot(intersect - other.origin, other.dir.normalized);
        laserlen = Vector3.Dot(intersect - laser.origin, laser.dir.normalized);
        if(laserlen > laserdistconst || otherlaserlen > laserdistconst)
        {
            return false;
        }
        return true;
    }

    private void clearlaser(Laser other, Laser joinedlaser)
    {
        laser.JoinedLaser.GetComponent<Laser>().parent[0] = null;
        laser.JoinedLaser.GetComponent<Laser>().parent[1] = null;
        laser.JoinedLaser.SetActive(false);
        laser.JoinedLaser = null;
        other.JoinedLaser = null;

        return;
    }
}
