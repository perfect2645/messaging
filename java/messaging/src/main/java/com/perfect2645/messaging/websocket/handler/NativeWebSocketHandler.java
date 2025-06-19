package com.perfect2645.messaging.websocket.handler;

import com.perfect2645.messaging.websocket.config.NativeWebSocketConfig;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;
import org.springframework.web.socket.CloseStatus;
import org.springframework.web.socket.TextMessage;
import org.springframework.web.socket.WebSocketSession;
import org.springframework.web.socket.handler.TextWebSocketHandler;

@Component
public class NativeWebSocketHandler extends TextWebSocketHandler {

    private static final Logger logger = LoggerFactory.getLogger(NativeWebSocketHandler.class);
    private final WebSocketConnectionManager connectionManager = new NativeWebSocketConnectionManager();

    @Override
    public void afterConnectionEstablished(WebSocketSession session) throws Exception {

        String connectionId = session.getAttributes().get("connectionId").toString();
        String topic = session.getAttributes().get("topic").toString();
        session.getHandshakeHeaders().forEach(
                (key, value) -> logger.debug("Header: {} -> {}", key, value)
        );

        if (connectionManager.getConnection(connectionId) != null) {
            session.close(new CloseStatus(CloseStatus.POLICY_VIOLATION.getCode(),
                    "connection already exists"));
            logger.warn("Connection [{}] already exists. Session [{}] closed",
                    connectionId, session.getId());

            return;
        }

        connectionManager.addConnection(session, connectionId, topic);
        logger.debug("Connection: {} established:[{}], topic:{}", session.getId(), connectionId, topic);
    }

    @Override
    public void handleTransportError(WebSocketSession session, Throwable exception) throws Exception {
        try {
            var connection = connectionManager.getConnection(session);
            if (connection == null) {
                logger.error("handleTransportError:Connection not found for session: {}",
                        session.getId());
                return;
            }
            logger.error("Transport Error {}: {}, connectionId=[{}]",
                    session.getId(), exception.getMessage(), connection.connectionId());

        } catch (Exception e) {
            logger.error("handleTransportError:Exception occurred. session={}", session.getId(), e);
        }

        super.handleTransportError(session, exception);
    }

    @Override
    public void afterConnectionClosed(WebSocketSession session, CloseStatus closeStatus) throws Exception {
        if (!validateCloseStatus(closeStatus)) {

        }

    }
    private boolean validateCloseStatus(CloseStatus closeStatus) {
        try {
            if (closeStatus == CloseStatus.POLICY_VIOLATION) {
                return false;
            }

            return true;
        } catch (Exception ex) {
            logger.error("validateCloseStatus:Exception occurred. closeStatus={}", closeStatus, ex);
            return false;
        }
    }

    @Override
    public boolean supportsPartialMessages() {
        return false;
    }

    @Override
    public void handleTextMessage(WebSocketSession session, TextMessage message) {
        logger.debug("Text message received:session=[{}], message=[{}]", session.getId(), message.getPayload());
    }

    public WebSocketConnectionManager getConnectionManager() {
        return connectionManager;
    }

    public void broadcastMessage(String message) {
        broadcastMessage(new TextMessage( message));
    }

    @SuppressWarnings("resource")
    public void broadcastMessage(TextMessage message) {
        synchronized (connectionManager) {
            connectionManager.getConnections().forEach(connection -> {
                WebSocketSession session = connection.session();
                if (session.isOpen()) {
                    try {
                        session.sendMessage(message);
                        logger.debug("broadcastMessage:session=[{}], message=[{}]",
                                session.getId(), message.getPayload());
                    } catch (Exception e) {
                        logger.error("broadcastMessage:Exception occurred. message=[{}]", e.getMessage(), e);
                    }
                } else {
                    logger.warn("Attempted to send message to a closed or null session.");
                }
            });
        }
    }
}
