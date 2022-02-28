using UnityEngine;

namespace TrailsFX.Demos {

    public class RotateObject : MonoBehaviour
	{
		public bool random;
		public float speed = 100f;

		Vector3 eulerAngles;

		void Start ()
		{
			SetAngles ();
		}

		void Update ()
		{
			transform.Rotate (eulerAngles * (Time.deltaTime * speed));
			//if (Random.value > 0.995f) {
			//	SetAngles ();
			//}
		}

		void SetAngles ()
		{
			if(random)
				eulerAngles = new Vector3 (Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
			else
			{
				eulerAngles = Vector3.forward;
			}
		}
	}
}