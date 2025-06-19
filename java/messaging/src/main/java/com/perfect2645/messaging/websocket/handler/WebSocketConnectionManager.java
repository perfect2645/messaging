package com.perfect2645.messaging.websocket.handler;

import org.springframework.web.socket.WebSocketSession;

import java.util.Collection;

public interface WebSocketConnectionManager {
    void addConnection(WebSocketSession session, String user, String topic);
    void removeConnection(WebSocketSession session);
    void logConnection(String connectionId);
    NativeWebSocketConnection getConnection(String connectionId);
    Collection<NativeWebSocketConnection> getConnections();
    NativeWebSocketConnection getConnection(WebSocketSession session);
}
