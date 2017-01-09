using UnityEngine;
using System.Collections;

public class ExplodeScripts : MonoBehaviour
{


    public class Stuff
    {
        public Transform transform;
        public Vector3 startPos;
        public Vector3 endPos;
		public Quaternion startQuation;
    }

    [Range(1, 5)]
    public float radius = 1;
    public float smooth = 100;
    private Vector3 center;
    private bool explode;
    private bool recover;
    public float processTime = 1.3f;
    private float exStartTime = 0.0f;
    private float reStartTime = 0.0f;

    private Stuff[] stuffs;
    void Awake()
    {
        center = transform.position;
    }
    // Use this for initialization
    void Start()
    {
        stuffs = new Stuff[transform.childCount];
		var local = Vector3.zero;
		foreach (Transform trans in transform) 
		{
			local +=trans.GetComponent<MeshFilter> ().mesh.bounds.center;
		}
		center = local / transform.childCount;
        for(int i = 0; i < transform.childCount; i++)
        {
            Stuff stuff = new Stuff();
            stuffs[i] = stuff;

            stuff.transform = transform.GetChild(i);
            stuff.startPos = stuff.transform.position;
			var oc = stuff.transform.GetComponent<MeshFilter> ().mesh.bounds.center;
			var direction = oc - center;
            var offset = direction * radius * 2;
			stuff.endPos =  stuff.startPos + offset;
			stuff.startQuation = stuff.transform.localRotation;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (explode)
        {
            foreach (Stuff s in stuffs)
            {
                if (Translate2EndPos(s))
                {
                    exStartTime = 0;
                    explode = false;
                }
            }
        }

        if (recover)
        {
           
            foreach (Stuff s in stuffs)
            {

                if (Translate2StartPos(s))
                {
                    reStartTime = 0;
                    recover = false;
                }
            }
        }
    }


    private bool Translate2EndPos(Stuff stuff)
    {
		if (Time.time - exStartTime > processTime || Vector3.Magnitude (stuff.transform.position - stuff.endPos) <= float.Epsilon) 
		{
			stuff.transform.position = stuff.endPos;
			return true;
		}
		stuff.transform.position = Vector3.Lerp(stuff.transform.position, stuff.endPos,  Time.deltaTime * smooth);
        return false;

    }
    private bool Translate2StartPos(Stuff stuff)
    {
		if (Time.time - reStartTime > processTime || Vector3.Magnitude (stuff.transform.position - stuff.startPos) <= float.Epsilon) 
		{
			stuff.transform.position = stuff.startPos;
			stuff.transform.localRotation = stuff.startQuation;
			return true;
		}
		stuff.transform.position = Vector3.Lerp(stuff.transform.position, stuff.startPos,Time.deltaTime * smooth);
		stuff.transform.localRotation = Quaternion.Lerp (stuff.transform.localRotation, stuff.startQuation,Time.deltaTime * smooth);
        return false;

    }
    public void Explode()
    {
        exStartTime = Time.time;
        explode = true;
    }

    public void Recover()
    {
        reStartTime = Time.time;
        recover = true;
    }
}
