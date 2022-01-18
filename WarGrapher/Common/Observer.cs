namespace WarGrapher.Common
{
    internal interface IObserver
    {
        void Update();
    }

    internal interface IObservable
    {
        void RegisterObserver(IObserver obs);
        void RemoveObserver(IObserver obs);
        void NotifyObservers();
    }
}
