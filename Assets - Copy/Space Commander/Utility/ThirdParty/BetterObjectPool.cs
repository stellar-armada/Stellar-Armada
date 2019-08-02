using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// By Anish Dhesikan
public abstract class BetterObjectPool : MonoBehaviour {

	public GameObject objectPrefab;

	// initialMax: How many objects should the pool initially contain?
	public int initialMax = 100;
	// minInPool: What is the smallest number of items this pool should contain at any point?
	public int minInPool = 20;

	/*
	 * -- How does it work? --
	 * You can set 3 thresholds: lowerBound, middleBound, and upperBound. 
	 * At any time, if the percent of active objects from the pool hits 
	 * the lowerBound or upperBound thresholds, the object pool will begin to dynamically 
	 * create or destroy objects to get the number back to the middle threshold. 
	 * Try out the demo in ExampleScenes for more info.
	 */
	public float upperBound = 0.75f; // between 0 and 1
	public float middleBound = 0.5f; // between 0 and 1
	public float lowerBound = 0.25f; // between 0 and 1


	// maxInstantiatesPerFrame: How many times can Instantiate() be called in a single frame?
	// For mobile and for best performance in general, keep this number low.
	// Currently, numbers less than 1 are not supported.
	public int maxInstantiatesPerFrame = 5;

	// maxDestroysPerFrame: How many times can Destroy() be called in a single frame?
	// For mobile and for best performance in general, keep this number low.
	// Currently, numbers less than 1 are not supported.
	public int maxDestroysPerFrame = 1;

	protected HashSet<GameObject> activeObjectPool = new HashSet<GameObject>();
	protected Transform objectPoolParent;
	protected List<GameObject> inactiveObjectPool = new List<GameObject>();

	private int totalCount = 0;

	private float percentActive = 1;
	
	public void Init()
	{
		objectPoolParent = new GameObject ("ObjectPool " + objectPrefab.name).transform;

		InstantiateMultiple (Mathf.FloorToInt(initialMax * middleBound));
		
		objectPoolParent.SetParent(transform);
	}


    // Update is called once per frame
    public virtual void Update () {
		UpdatePercentActive ();
		if (percentActive > upperBound) {
			StopAllCoroutines ();
			StartCoroutine (ScaleUp());
		} else if (percentActive < lowerBound) {
			StopAllCoroutines ();
			StartCoroutine (ScaleDown());
		}
	}
    
	IEnumerator ScaleUp () {
		while (percentActive > middleBound) {
			//Mathf.FloorToInt (Mathf.Pow (totalCount, 0.5f))
			int numToInstantiate = Mathf.Min (maxInstantiatesPerFrame, Mathf.CeilToInt(totalCount / 200f));
			InstantiateMultiple (numToInstantiate);
			UpdatePercentActive ();
			yield return 0;
		}
	}

	IEnumerator ScaleDown () {
		if (percentActive > 0 && totalCount > minInPool) {
			while (percentActive < middleBound) {
				DestroyMultiple (maxDestroysPerFrame);
				UpdatePercentActive ();
				yield return 0;
			}
		}
	}

	public virtual bool ObjectIsActive (GameObject instance) {
		return activeObjectPool.Contains (instance);
	}

	public virtual void PutInstanceBackInPool (GameObject instance) {
		if (gameObject == null)
		{
			Destroy(instance);
			return;
		}

		if(gameObject != null) StopAllCoroutines ();
		activeObjectPool.Remove (instance);
		inactiveObjectPool.Add (instance);
		instance.SetActive(false);
		instance.transform.SetParent(objectPoolParent);
	}

	public virtual GameObject GetInstanceFromPool () {
		StopAllCoroutines ();
		GameObject curObject = null;
		if (inactiveObjectPool == null || activeObjectPool == null) return curObject;


		if (inactiveObjectPool.Count <= 0) {
			InstantiateOne ();
		}

		curObject = inactiveObjectPool [0] as GameObject;
		curObject.SetActive (true);
		inactiveObjectPool.Remove (curObject);
		activeObjectPool.Add (curObject);
		curObject.transform.SetParent(null);

		return curObject;
	}

	public virtual GameObject GetInstanceFromPool (Vector3 position, Quaternion rotation) {
		GameObject curObject = GetInstanceFromPool ();
			curObject.transform.SetParent(null);
			curObject.transform.position = position;
			curObject.transform.rotation = rotation;
			curObject.SetActive(true);
		return curObject;
	}

	// Instantiates, deactivates, and adds one to the pool
	private void InstantiateOne () {
		GameObject newObject = Instantiate (objectPrefab) as GameObject;
		newObject.SetActive (false);
		inactiveObjectPool.Add (newObject);
		newObject.transform.SetParent(objectPoolParent);
		totalCount++;
	}

	private void InstantiateMultiple (int count)
	{
		//Debug.Log("Instantiating more prefabs");
		for (int i = 0; i < count; i++) {
			InstantiateOne ();
		}
	}

	private void DestroyOne () {
		if (inactiveObjectPool.Count > 0) {
			GameObject curObject = inactiveObjectPool [0] as GameObject;
			inactiveObjectPool.Remove (curObject);
			DestroyImmediate (curObject);
			totalCount--;
		}
	}

	private void DestroyMultiple (int count) {
		//Debug.Log("Destroying more prefabs");

		for (int i = 0; i < count; i++) {
			DestroyOne ();
		}
	}

	public string GetInfoText () {
		return "Percent Active:   " + percentActive * 100 + "%" + "\n" +
			"Number Active:   " + activeObjectPool.Count + "\n" +
			"Number Inactive: " + inactiveObjectPool.Count;
	}

	private void UpdatePercentActive () {
		percentActive = (float) activeObjectPool.Count / totalCount;


//		if (percentActive < lowerBound) {
//			Debug.Log ("Percent Active: " + percentActive);
//			Debug.Log ("TotalCount: " + totalCount);
//		}
	}
}
