using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingProtocol {

	// the date and time when the session started
	public string SessionTime;

	// The total runtime of this session in seconds
	public float SessionRuntime;

	// Time protocol of clicks
	public int SessionClicks;

	// Time protocol of current rotations
	public List<float> SessionRotationsX;
	public List<float> SessionRotationsY;
		
}
