
using UnityEngine.Rendering;
using UnityEngine;

public class GraphicsManager : MonoBehaviour
{
	private void Awake () {
		Initialize ();
	}
	static void Initialize () {
		GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
		GraphicsSettings.transparencySortAxis = new Vector3 (0.0f, 1.0f, 1.0f);
	}
}
