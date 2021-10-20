package ru.pochta.services;

import java.util.List;
import java.util.Optional;
import java.util.UUID;

import org.springframework.data.jdbc.core.JdbcAggregateOperations;
import org.springframework.stereotype.Service;
import ru.pochta.models.address.Address;
import ru.pochta.repository.AddressRepository;
import ru.pochta.utils.RetryPolicies;


/**
 * @author voroningg
 */
@Service
public class AddressService {

    private final JdbcAggregateOperations aggregateOperations;
    private final AddressRepository addressRepository;

    public AddressService(JdbcAggregateOperations entityOperations, AddressRepository addressRepository) {
        this.aggregateOperations = entityOperations;
        this.addressRepository = addressRepository;

    }

    public Optional<Address> findAddressById(String id) throws Exception {
        checkIdFormat(id);
        return RetryPolicies.get(() -> addressRepository.findById(id));
    }

    public Address updateAddress(Address updateEntity) throws Exception {
        return RetryPolicies.get(() -> addressRepository.save(updateEntity));
    }

    public void deleteAddress(String id) throws Exception {
        checkIdFormat(id);
        RetryPolicies.run(() -> addressRepository.deleteById(id));
    }

    public List<Address> getPage(int page, int size) throws Exception {
        return RetryPolicies.get(() -> addressRepository.getAddressesPage(page * size, size));
    }

    public Address insertAddress(Address newAddress) throws Exception {
        checkIdFormat(newAddress.getId());
        return RetryPolicies.get(() -> aggregateOperations.insert(newAddress));
    }

    public void checkIdFormat(String id) {
        UUID.fromString(id);
    }

    public void createTableIfNotExist() {
        try {
            findAddressById(UUID.randomUUID().toString());
        } catch (Exception ex) {
            addressRepository.createTable();
        }
    }
}
