using System;

namespace NamedPipeWrapper;

public delegate void ConnectionExceptionEventHandler<TRead, TWrite>(NamedPipeConnection<TRead, TWrite> connection, Exception exception) where TRead : class where TWrite : class;
