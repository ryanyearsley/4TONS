using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVitalEntity
{
    void InitializeVitals();
    void RegisterVitals();
    void DeregisterVitals();
}
