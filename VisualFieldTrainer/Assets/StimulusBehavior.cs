using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class StimulusBehavior : MonoBehaviour
{

	public Light fixCross;
	public Light myLight;
	public TextMesh goodbyeText;
	public TrainingSettings trainingSettings;
	public TrainingCoordinates trainingCoordinates;
	public TrainingProtocol trainingProtocol;

	private string sessionTime;
	private int sessionClicks;

	private List<float> basketX;
	private List<float> basketY;
	private List<float> refX;
	private List<float> refY;
	private List<float>  initDistLenArray;
	private List<int> indexArray;

	private float moveToRef;
	private float moveFromRef;
	private float maxOutline;

	private float trainTime;
	private float stimTime;
	private float pauseTime;
	private float pauseRandRange;

	public float time;
	private float savingTime;
	private float fixChangeTime;
	private float fixChangePeriod;
	private float fixChangeRandRange;

	private float saveAfterPeriod;
	private bool feedback;
	private bool saveProgress;



	// Use this for initialization
	void Start()
	{

		// Set the current time and clicks
		sessionTime = System.DateTime.Now.ToString("yyyy-MM-dd | HH:mm:ss");
		sessionClicks = 0;

		// load stuff
		trainingSettings = JsonUtility.FromJson<TrainingSettings>(File.ReadAllText(Application.persistentDataPath + "/train_settings.json"));
		trainingCoordinates = JsonUtility.FromJson<TrainingCoordinates>(File.ReadAllText(Application.persistentDataPath + "/train_coords.json"));

		// update the number of this session and save it
		trainingSettings.NSession += 1;
		string jsonSettingsData = JsonUtility.ToJson(trainingSettings, true);
		File.WriteAllText(Application.persistentDataPath + "/train_settings.json", jsonSettingsData);

		// set the coordinates & references
		basketX = trainingCoordinates.CurrentRotationsX;
		basketY = trainingCoordinates.CurrentRotationsY;
		refX = trainingCoordinates.RefX;
		refY = trainingCoordinates.RefY;

		// initialize the first index array
		indexArray = Enumerable.Range (0, basketX.Count).ToList ();

		// calculate an array for the distances
		initDistLenArray = DistanceArray(trainingCoordinates.StartRotationsX, trainingCoordinates.StartRotationsY, refX, refY);

		// set the Adaption vector length
		moveToRef = trainingSettings.MoveToRef;
		moveFromRef = trainingSettings.MoveFromRef;
		maxOutline = trainingSettings.MaxOutline;

		// set the timing
		trainTime = trainingSettings.TrainTime;
		stimTime = trainingSettings.StimTime;
		pauseTime = trainingSettings.PauseTime;
		pauseRandRange = trainingSettings.PauseRandRange;

		// load whether progress should be saved
		saveProgress = trainingSettings.SaveProgress;

		// load the period in which data should be saved.
		saveAfterPeriod = trainingSettings.SaveAfterPeriod;

		// load the period and random factor in which fix cross should change color.
		fixChangePeriod = trainingSettings.FixChangePeriod;
		fixChangeRandRange = trainingSettings.FixChangeRandRange;

		// update the stimulus light's attributes
		myLight.spotAngle = trainingSettings.StimSize * 2;
		myLight.intensity = Mathf.Pow(1.7f, trainingSettings.StimBright) / 6;

		// update the Fixation cross light attributes
		fixCross.spotAngle = trainingSettings.FixSize * 2;
		fixCross.intensity = Mathf.Pow(1.7f, trainingSettings.FixBright) / 6;


		//initialize the training loop
		time = 0.0f;
		savingTime = 0.0f;
		fixChangeTime = 0.0f;
		feedback = false;
		StartCoroutine(LoopStimulus(trainTime, stimTime, pauseTime, pauseRandRange, moveToRef, moveFromRef, maxOutline));

	}


	// Update is called once per frame
	void Update()
	{
		time += Time.deltaTime;

		if (indexArray.Count == 0)
		{
			// fill index array up with new values
			indexArray = Enumerable.Range (0, basketX.Count).ToList ();



		}

		// check feedback
		if (Input.GetKeyDown(KeyCode.Escape))
		{

			feedback = true;
			sessionClicks += 1;
		}

		// save the data each saveAfterPeriod
		if (time > savingTime) {
			savingTime += saveAfterPeriod;
			SaveData ();
		}

		// change fixCross color in periods
		if (time > fixChangeTime) {
			fixChangeTime += fixChangePeriod + Random.Range(0f, fixChangeRandRange);
			fixCross.color = new Color(
				Random.Range(0f, 1f), 
				Random.Range(0f, 1f), 
				Random.Range(0f, 1f));


		}
	}


	// Time the stimulus with the given parameters
	IEnumerator LoopStimulus(float trainTime, float stimTime, float pauseTime, float pauseRandRange, float moveToRef, float moveFromRef, float maxOutline)
	{
		while (time < trainTime)
		{

			// get the next Index from a random permutation
			int randInd = NextRandInd();

			// present the stimulus for presentation time
			ChangeAngle(basketX[randInd], basketY[randInd]);
			yield return new WaitForSeconds(stimTime);

			// "remove" the stim during pause time
			ChangeAngle(180, 0);
			float pauseRandTime = Random.Range(0f, pauseRandRange); // 0f could be replaced with -pauseRandRange
			yield return new WaitForSeconds(pauseTime + pauseRandTime);

			// prepare the distances to update the stimulus
			float distX = refX[randInd] - basketX[randInd];
			float distY = refY[randInd] - basketY[randInd];
			float distLen = Mathf.Sqrt (Mathf.Pow(distX, 2) + Mathf.Pow(distY, 2));



			if (feedback == true) {

				// Move the stimuli <moveToRef> degrees towards the Reference point
				basketX[randInd] = basketX[randInd] + (distX/distLen)* moveToRef;
				basketY[randInd] = basketY[randInd] + (distY/distLen)* moveToRef;
				Debug.Log("feedback was true");
				feedback = false;
			} else {

				float initDistLen = initDistLenArray [randInd];
				if (distLen < initDistLen + maxOutline) {

					// Move the stimuli <moveFromRef> degrees away from the Reference point
					basketX[randInd] = basketX[randInd] - (distX/distLen)* moveFromRef;
					basketY[randInd] = basketY[randInd] - (distY/distLen)* moveFromRef;

				}
				Debug.Log("feedback was false");

			}


			//time += stimTime + pauseTime + pauseRandTime;

			//debugging notes
			Debug.Log("random Range: " + pauseRandTime);
		}
			

		// show goodbye text, save and close the app
		goodbyeText.gameObject.SetActive (true);
		SaveData();
		yield return new WaitForSeconds(5.0f);
		Debug.Log("QUIT!");
		Application.Quit();
			
	}


	// Change the angle of the ligh (and the spot angle)
	void ChangeAngle(float angleX, float angleY)
	{

		// changes the Light projection Angle
		myLight.transform.rotation = Quaternion.Euler(angleY, angleX, 0);


		Debug.Log("X Angle: " + angleX);
		Debug.Log("Y Angle: " + angleY);
	}

	// draw a random index from of length n_basket to create random permutation
	private int NextRandInd()
	{
		int randN = new System.Random().Next(0, indexArray.Count);
		Debug.Log("random Index: " + randN);

		int randInd = indexArray[randN];
		indexArray.RemoveAt (randN);

		return randInd;
	}

	private List<float> DistanceArray(List<float> xListA, List<float> yListA, List<float> xListB, List<float> yListB){

		List<float> listC = new List<float>();

		for (int i = 0; i < xListA.Count; i++){
		// calculate the initial distance to the reference
		float initDistX = xListA[i] - xListB[i];
		float initDistY = yListA[i] - yListB[i];
		listC.Add( Mathf.Sqrt (Mathf.Pow(initDistX, 2) + Mathf.Pow(initDistY, 2)));
		}

		return listC;
	}


	// save the data
	void SaveData(){

		// save information on protocol
		trainingProtocol = new TrainingProtocol ();

		trainingProtocol.SessionTime = sessionTime;
		trainingProtocol.SessionRuntime = time;
		trainingProtocol.SessionClicks = sessionClicks;
		trainingProtocol.SessionRotationsX = basketX;
		trainingProtocol.SessionRotationsY = basketY;
		string nSessionStr = trainingSettings.NSession.ToString();
		string jsonProtocolData = JsonUtility.ToJson(trainingProtocol, true);
		File.WriteAllText(Application.persistentDataPath + "/Protocols/train_protocol_" + nSessionStr + ".json", jsonProtocolData);

		// if progress needs to be saved, safe it too.
		if (saveProgress){

			// save the CurrentRotations for next time
			string jsonCoordData = JsonUtility.ToJson(trainingCoordinates, true);
			File.WriteAllText(Application.persistentDataPath + "/train_coords.json", jsonCoordData);
			// Where the fuck are the training settings actually assigned to the adapted basketX and Y?

		}
	}

}
