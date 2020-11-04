using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerComponent {
    void SubscribeToEvents ();
    void UnsubscribeFromEvents ();
    bool InitializeComponent (WizardSaveData wizardSaveData, Player player);
}
