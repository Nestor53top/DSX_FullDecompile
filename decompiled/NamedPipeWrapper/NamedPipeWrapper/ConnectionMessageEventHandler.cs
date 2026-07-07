namespace NamedPipeWrapper;

public delegate void ConnectionMessageEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection, TRead message) where TRead : class where TWrite : class;
