using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorialSubscriber
{

	void SubscribeToTutorialEvents ();

	void OnStartPhase (TutorialPhaseInfo phaseInfo);

	void OnTaskComplete (TutorialTask tutorialTask);

	void OnPhaseComplete (TutorialPhaseInfo completedPhaseInfo);


}
