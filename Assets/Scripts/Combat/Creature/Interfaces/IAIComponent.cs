using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIComponent
{

	void SubscribeToAIEvents (BabyBrainsObject aiStateController);

	void UnsubscribeFromAIEvents (BabyBrainsObject aiStateController);

}
