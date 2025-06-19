package com.perfect2645.messaging.websocket.handler;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.util.ConcurrentReferenceHashMap;
import org.springframework.web.socket.WebSocketSession;

import java.lang.annotation.Native;
import java.time.Duration;
import java.time.LocalDateTime;
import java.util.Collection;
import java.util.concurrent.ConcurrentMap;

public class NativeWebSocketConnectionManager implements WebSocketConnectionManager{

    private static final Logger logger = LoggerFactory.getLogger(NativeWebSocketConnectionManager.class);

    private final ConcurrentMap<String, NativeWebSocketConnection> connections = new ConcurrentReferenceHashMap<>();

    @Override
    public void addConnection(WebSocketSession session, String connectionId, String topic) {
        NativeWebSocketConnection connection = new NativeWebSocketConnection(
                session, LocalDateTime.now(), connectionId, topic);
        connections.put(connectionId, connection);

        logger.info("Connection added: {}, connectionId:{}, topic:{}",
                session.getId(), connectionId, topic);
        logConnection(session.getId());
    }

    @Override
    public void removeConnection(WebSocketSession session) {
        String connectionId = session.getAttributes().get("connectionId").toString();

        if (connections.containsKey(connectionId)) {
            connections.remove(connectionId);
            logger.info("Connection removed: {}, connectionId:{}", session.getId(), connectionId);
        } else {
            logger.warn("Connection not found: {}, connectionId:{}", session.getId(), connectionId);
        }
    }

    @Override
    @SuppressWarnings("resource")
    public void logConnection(String connectionId) {
        NativeWebSocketConnection connection = connections.get(connectionId);
        if (connection != null) {
            Duration duration = Duration.between(connection.createdDateTime(), LocalDateTime.now());
            logger.info("Connection: {}:[{}], has been active for {} seconds",
                    connection.session().getId(), connectionId, duration.getSeconds());
        } else {
            logger.warn("Connection not found, connectionId:[{}]", connectionId);
        }
    }

    @Override
    public NativeWebSocketConnection getConnection(String connectionId) {
        return connections.get(connectionId);
    }

    @Override
    public NativeWebSocketConnection getConnection(WebSocketSession session) {
        String connectionId = session.getAttributes().get("connectionId").toString();
        return connections.get(connectionId);
    }

    @Override
    public Collection<NativeWebSocketConnection> getConnections() {
        return connections.values();
    }
}
