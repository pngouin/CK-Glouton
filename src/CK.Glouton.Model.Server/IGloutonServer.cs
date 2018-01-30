namespace CK.Glouton.Model.Server
{
    public interface IGloutonServer
    {
        void Open( IHandlersManagerConfiguration handlersManagerConfiguration );
        void ApplyConfiguration( IHandlersManagerConfiguration handlersManagerConfiguration );
        void Close();
    }
}
