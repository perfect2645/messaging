package com.perfect2645.messaging.websocket.handler;

import com.perfect2645.messaging.websocket.config.NativeWebSocketConfig;
import jakarta.servlet.http.HttpServletResponse;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpHeaders;
import org.springframework.http.server.ServerHttpRequest;
import org.springframework.http.server.ServerHttpResponse;
import org.springframework.web.socket.WebSocketHandler;
import org.springframework.web.socket.server.HandshakeInterceptor;
import org.springframework.web.util.UriComponents;
import org.springframework.web.util.UriComponentsBuilder;

import java.lang.reflect.Field;
import java.util.List;
import java.util.Map;

public class WebSocketHandshakeInterceptor implements HandshakeInterceptor {

    private static final Logger logger = LoggerFactory.getLogger(NativeWebSocketConfig.class);

    @Override
    public boolean beforeHandshake(ServerHttpRequest request,
                                   ServerHttpResponse response,
                                   WebSocketHandler wsHandler,
                                   Map<String, Object> attributes) throws Exception {

        adjustHandShakeHeader(request, response);
        UriComponents uriComponents = UriComponentsBuilder.fromUri(request.getURI()).build();
        String connectionId = uriComponents.getQueryParams().getFirst("connectionId");

        if (connectionId == null || connectionId.isEmpty()) {
            logger.error("Ws Connection Id not found in query parameters");
            return false;
        }

        String topic = uriComponents.getQueryParams().getFirst("topic");
        if (topic == null || topic.isEmpty()) {
            logger.error("Ws Topic not found in query parameters");
            return false;
        }

        attributes.put("connectionId", connectionId);
        attributes.put("topic", topic);

        return true;
    }

    private void adjustHandShakeHeader(ServerHttpRequest request, ServerHttpResponse response) {
        if (request.getHeaders().containsKey("Connection")) {
            logger.debug("Connection header found: {}", request.getHeaders().get("Connection"));

            request.getHeaders().put("Connection", List.of("Upgrade"));

            logger.debug("Adjust connection header to {}", request.getHeaders().get("Connection"));
        }
    }

    @Override
    public void afterHandshake(ServerHttpRequest request,
                               ServerHttpResponse response,
                               WebSocketHandler wsHandler,
                               Exception ex) {
        if (!response.getHeaders().containsKey("Connection")) {
            logger.debug("afterHandshake:Connection header not found");
        }

        String connectionHeader = response.getHeaders().getConnection().getFirst();
        if (connectionHeader.equalsIgnoreCase("upgrade")) {
            return;
        }

        logger.debug("afterHandshake:Invalid connection header [{}] found in response", response.getHeaders().getConnection());

        Field servletResponseField = null;
        try {
            servletResponseField = response.getClass().getDeclaredField("servletResponse");
            servletResponseField.setAccessible(true);

            HttpServletResponse servletResponse = (HttpServletResponse) servletResponseField.get(response);
            servletResponse.setHeader(HttpHeaders.CONNECTION, "upgrade");

            logger.debug("afterHandshake:Adjust connection header to [{}]", response.getHeaders().getConnection());

        } catch (NoSuchFieldException | IllegalAccessException e) {
            logger.error("afterHandshake::adjust response header Exception occurred", e);
        }
    }
}
