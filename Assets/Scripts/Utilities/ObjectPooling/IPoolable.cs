public interface IPoolable {

    void SetupObject ();

    void ReuseObject ();

    void Destroy ();

    void TerminateObjectFunctions ();
}
