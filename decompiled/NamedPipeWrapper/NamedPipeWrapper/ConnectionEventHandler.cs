namespace NamedPipeWrapper;

public delegate void ConnectionEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection) where TRead : class where TWrite : class;
