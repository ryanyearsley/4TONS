using UnityEngine;
public interface IPoolable {

    //Called at pool creation
    void SetupObject ();

    //called on every object re-use (even the first)
    void ReuseObject ();


    void Destroy ();

    void TerminateObjectFunctions ();
}
