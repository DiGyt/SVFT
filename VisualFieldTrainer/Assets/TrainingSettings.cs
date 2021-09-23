using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSettings {

	// object size & brightness
    public float StimSize;
    public float StimBright;
    public float FixSize;
    public float FixBright;
    public float BackBright;

	// training time settings
	public float TrainTime;
	public float StimTime;
	public float PauseTime;
	public float PauseRandRange;

	// Fixation color change settings
	public float FixChangePeriod;
	public float FixChangeRandRange;

	// Move the points a certain length towards the reference or away
	public float MoveToRef;
	public float MoveFromRef;
	public float MaxOutline;

	// save Progress of the training Points
	public bool SaveProgress;

	// save the Protocol + eventually the progress after a fixed number of seconds
	public float SaveAfterPeriod;

	// this variable is used to count the sessions and should not be changed
	public int NSession;
}
