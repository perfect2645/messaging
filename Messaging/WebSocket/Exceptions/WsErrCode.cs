namespace Messaging.WebSocket.Exceptions
{
    public enum WsErrCode
    {
        SocketError = -1,
        Success = 0,
        Exception = 1,
        Config = 2,
        BuildClient = 3,
        Register = 4,
        GetClient = 5,

        Controller = 101,
        Service = 102,

        Connection = 301,
        ConnectionHeaderInvalid = 302,

        Authentication = 402,

        ReceiveMessage = 501,
        SendMessage = 502,
        JsonParse = 503,

        //
        // 摘要:
        //     The overlapped operation was aborted due to the closure of the System.Net.Sockets.Socket.
        OperationAborted = 995,
        //
        // 摘要:
        //     The application has initiated an overlapped operation that cannot be completed
        //     immediately.
        IOPending = 997,
        //
        // 摘要:
        //     A blocking System.Net.Sockets.Socket call was canceled.
        Interrupted = 10004,
        //
        // 摘要:
        //     An attempt was made to access a System.Net.Sockets.Socket in a way that is forbidden
        //     by its access permissions.
        AccessDenied = 10013,
        //
        // 摘要:
        //     An invalid pointer address was detected by the underlying socket provider.
        Fault = 10014,
        //
        // 摘要:
        //     An invalid argument was supplied to a System.Net.Sockets.Socket member.
        InvalidArgument = 10022,
        //
        // 摘要:
        //     There are too many open sockets in the underlying socket provider.
        TooManyOpenSockets = 10024,
        //
        // 摘要:
        //     An operation on a nonblocking socket cannot be completed immediately.
        WouldBlock = 10035,
        //
        // 摘要:
        //     A blocking operation is in progress.
        InProgress = 10036,
        //
        // 摘要:
        //     The nonblocking System.Net.Sockets.Socket already has an operation in progress.
        AlreadyInProgress = 10037,
        //
        // 摘要:
        //     A System.Net.Sockets.Socket operation was attempted on a non-socket.
        NotSocket = 10038,
        //
        // 摘要:
        //     A required address was omitted from an operation on a System.Net.Sockets.Socket.
        DestinationAddressRequired = 10039,
        //
        // 摘要:
        //     The datagram is too long.
        MessageSize = 10040,
        //
        // 摘要:
        //     The protocol type is incorrect for this System.Net.Sockets.Socket.
        ProtocolType = 10041,
        //
        // 摘要:
        //     An unknown, invalid, or unsupported option or level was used with a System.Net.Sockets.Socket.
        ProtocolOption = 10042,
        //
        // 摘要:
        //     The protocol is not implemented or has not been configured.
        ProtocolNotSupported = 10043,
        //
        // 摘要:
        //     The support for the specified socket type does not exist in this address family.
        SocketNotSupported = 10044,
        //
        // 摘要:
        //     The address family is not supported by the protocol family.
        OperationNotSupported = 10045,
        //
        // 摘要:
        //     The protocol family is not implemented or has not been configured.
        ProtocolFamilyNotSupported = 10046,
        //
        // 摘要:
        //     The address family specified is not supported. This error is returned if the
        //     IPv6 address family was specified and the IPv6 stack is not installed on the
        //     local machine. This error is returned if the IPv4 address family was specified
        //     and the IPv4 stack is not installed on the local machine.
        AddressFamilyNotSupported = 10047,
        //
        // 摘要:
        //     Only one use of an address is normally permitted.
        AddressAlreadyInUse = 10048,
        //
        // 摘要:
        //     The selected IP address is not valid in this context.
        AddressNotAvailable = 10049,
        //
        // 摘要:
        //     The network is not available.
        NetworkDown = 10050,
        //
        // 摘要:
        //     No route to the remote host exists.
        NetworkUnreachable = 10051,
        //
        // 摘要:
        //     The application tried to set System.Net.Sockets.SocketOptionName.KeepAlive on
        //     a connection that has already timed out.
        NetworkReset = 10052,
        //
        // 摘要:
        //     The connection was aborted by .NET or the underlying socket provider.
        ConnectionAborted = 10053,
        //
        // 摘要:
        //     The connection was reset by the remote peer.
        ConnectionReset = 10054,
        //
        // 摘要:
        //     No free buffer space is available for a System.Net.Sockets.Socket operation.
        NoBufferSpaceAvailable = 10055,
        //
        // 摘要:
        //     The System.Net.Sockets.Socket is already connected.
        IsConnected = 10056,
        //
        // 摘要:
        //     The application tried to send or receive data, and the System.Net.Sockets.Socket
        //     is not connected.
        NotConnected = 10057,
        //
        // 摘要:
        //     A request to send or receive data was disallowed because the System.Net.Sockets.Socket
        //     has already been closed.
        Shutdown = 10058,
        //
        // 摘要:
        //     The connection attempt timed out, or the connected host has failed to respond.
        TimedOut = 10060,
        //
        // 摘要:
        //     The remote host is actively refusing a connection.
        ConnectionRefused = 10061,
        //
        // 摘要:
        //     The operation failed because the remote host is down.
        HostDown = 10064,
        //
        // 摘要:
        //     There is no network route to the specified host.
        HostUnreachable = 10065,
        //
        // 摘要:
        //     Too many processes are using the underlying socket provider.
        ProcessLimit = 10067,
        //
        // 摘要:
        //     The network subsystem is unavailable.
        SystemNotReady = 10091,
        //
        // 摘要:
        //     The version of the underlying socket provider is out of range.
        VersionNotSupported = 10092,
        //
        // 摘要:
        //     The underlying socket provider has not been initialized.
        NotInitialized = 10093,
        //
        // 摘要:
        //     A graceful shutdown is in progress.
        Disconnecting = 10101,
        //
        // 摘要:
        //     The specified class was not found.
        TypeNotFound = 10109,
        //
        // 摘要:
        //     No such host is known. The name is not an official host name or alias.
        HostNotFound = 11001,
        //
        // 摘要:
        //     The name of the host could not be resolved. Try again later.
        TryAgain = 11002,
        //
        // 摘要:
        //     The error is unrecoverable or the requested database cannot be located.
        NoRecovery = 11003,
        //
        // 摘要:
        //     The requested name or IP address was not found on the name server.
        NoData = 11004

    }
}
