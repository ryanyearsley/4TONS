using UnityEngine;
public interface IPoolable {

    void SetupObject (Transform parentTransform);

    void ReuseObject ();

    void Destroy ();

    void TerminateObjectFunctions ();
}
