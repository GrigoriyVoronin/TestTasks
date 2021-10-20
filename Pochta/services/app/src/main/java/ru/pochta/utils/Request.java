package ru.pochta.utils;

import java.io.ByteArrayInputStream;
import java.io.InputStreamReader;
import java.util.zip.GZIPInputStream;

import com.google.gson.Gson;
import org.springframework.web.client.RestTemplate;


/**
 * @author voroningg
 */
public class Request {
    public static <T> T getZippedWithRetry(String url, Class<T> classz) throws Exception {
        RestTemplate restTemplate = new RestTemplate();
        var body = RetryPolicies.get(
                () -> restTemplate.getForEntity(url, byte[].class));
        ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(body.getBody());
        GZIPInputStream gzipInputStream = new GZIPInputStream(byteArrayInputStream);
        InputStreamReader reader = new InputStreamReader(gzipInputStream);
        return new Gson().fromJson(reader, classz);
    }

    public static <T> T getWithRetry(String url, Class<T> classz) throws Exception {
        RestTemplate restTemplate = new RestTemplate();
        var body = RetryPolicies.get(
                () -> restTemplate.getForEntity(url, byte[].class));
        ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(body.getBody());
        InputStreamReader reader = new InputStreamReader(byteArrayInputStream);
        return new Gson().fromJson(reader, classz);
    }
}
