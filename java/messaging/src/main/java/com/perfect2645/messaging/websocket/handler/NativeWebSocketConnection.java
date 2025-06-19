package com.perfect2645.messaging.websocket.handler;

import org.springframework.web.socket.WebSocketSession;

import java.time.LocalDateTime;

public record NativeWebSocketConnection(
        WebSocketSession session,
        LocalDateTime createdDateTime,
        String connectionId,
        String topic) {
}
