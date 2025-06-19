package com.perfect2645.messaging.websocket.config;

import com.perfect2645.messaging.websocket.handler.NativeWebSocketHandler;
import com.perfect2645.messaging.websocket.handler.WebSocketHandshakeInterceptor;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.socket.config.annotation.EnableWebSocket;
import org.springframework.web.socket.config.annotation.WebSocketConfigurer;
import org.springframework.web.socket.config.annotation.WebSocketHandlerRegistry;

@Configuration
@EnableWebSocket
public class NativeWebSocketConfig implements WebSocketConfigurer {

    private static final Logger logger = LoggerFactory.getLogger(NativeWebSocketConfig.class);

    private final NativeWebSocketHandler nativeWebSocketHandler;

    public NativeWebSocketConfig(NativeWebSocketHandler nativeWebSocketHandler) {
        this.nativeWebSocketHandler = nativeWebSocketHandler;
    }

    @Override
    public void registerWebSocketHandlers(WebSocketHandlerRegistry registry) {
        logger.debug("Registering WebSocket handlers");
        registry.addHandler(nativeWebSocketHandler, "/ws")
                .addInterceptors(new WebSocketHandshakeInterceptor())
                .setAllowedOrigins("*");
    }
}
