package ru.pochta.controllers;

import java.util.List;
import java.util.Optional;

import com.google.gson.Gson;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;
import ru.pochta.models.address.Address;
import ru.pochta.models.address.UpdateAddressRestCommand;
import ru.pochta.services.AddressService;
import ru.pochta.utils.CheckParam;
import ru.pochta.utils.Response;

/**
 * @author voroningg
 */
@RestController
@RequestMapping("/v1.0/addresses")
public class AddressesController {
    private final Logger logger = LoggerFactory.getLogger(AddressesController.class);

    private final AddressService addressService;

    public AddressesController(AddressService addressService) {
        this.addressService = addressService;
        addressService.createTableIfNotExist();
    }

    @GetMapping("/{id}")
    public ResponseEntity<String> findAddressById(@PathVariable String id) {
        try {
            Optional<Address> addressO = addressService.findAddressById(id);
            if (addressO.isEmpty()) {
                return ResponseEntity.notFound().build();
            }
            return Response.ok(addressO.get());
        } catch (IllegalArgumentException ex) {
            logger.warn(ex.getMessage());
            return Response.badRequest();
        } catch (Exception ex) {
            logger.error(ex.getMessage());
            return Response.internalServerError();
        }
    }

    @PutMapping("/{id}")
    public ResponseEntity<String> update(@PathVariable String id, @RequestBody String body) {
        try {
            Optional<Address> addressO = addressService.findAddressById(id);
            if (addressO.isEmpty()) {
                return Response.notFound();
            }
            UpdateAddressRestCommand updateAddress = new Gson().fromJson(body, UpdateAddressRestCommand.class);
            addressService.updateAddress(new Address(id, updateAddress));
            return Response.noContent();
        } catch (IllegalArgumentException ex) {
            logger.warn(ex.getMessage());
            return Response.badRequest();
        } catch (Exception ex) {
            logger.error(ex.getMessage());
            return Response.internalServerError();
        }
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<String> delete(@PathVariable String id) {
        try {
            Optional<Address> addressO = addressService.findAddressById(id);
            if (addressO.isEmpty()) {
                return Response.notFound();
            }
            addressService.deleteAddress(id);
            return Response.noContent();
        } catch (IllegalArgumentException ex) {
            logger.warn(ex.getMessage());
            return Response.badRequest();
        } catch (Exception ex) {
            logger.error(ex.getMessage());
            return Response.internalServerError();
        }
    }

    @GetMapping
    public ResponseEntity<String> getAddressesPage(
            @RequestParam(defaultValue = "0") int page,
            @RequestParam(defaultValue = "20") int size)
    {
        try {
            CheckParam.greaterThan(page, 0);
            CheckParam.inRange(size, 1, 1000);
            List<Address> addresses = addressService.getPage(page, size);
            return Response.ok(addresses);
        } catch (IllegalArgumentException ex) {
            logger.warn(ex.getMessage());
            return Response.badRequest();
        } catch (Exception ex) {
            logger.error(ex.getMessage());
            return Response.internalServerError();
        }
    }

    @PostMapping
    public ResponseEntity<String> createAddress(@RequestBody String body) {
        try {
            Address address = new Gson().fromJson(body, Address.class);
            Optional<Address> addressO = addressService.findAddressById(address.getId());
            if (addressO.isPresent()) {
                return Response.conflict();
            }
            addressService.insertAddress(address);
            return Response.noContent();
        } catch (IllegalArgumentException ex) {
            logger.warn(ex.getMessage());
            return Response.badRequest();
        } catch (Exception ex) {
            logger.error(ex.getMessage());
            return Response.internalServerError();
        }
    }
}
