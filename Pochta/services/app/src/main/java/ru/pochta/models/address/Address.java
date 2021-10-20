package ru.pochta.models.address;


import lombok.Getter;
import lombok.Setter;
import org.springframework.data.annotation.Id;
import org.springframework.data.relational.core.mapping.Table;

/**
 * @author voroningg
 */
@Getter
@Setter
@Table("Addresses")
public class Address extends AddressBase {
    @Id
    private String id;

    public Address() {
    }

    public Address(String id, AddressBase addressBase) {
        this(id,
             addressBase.getPlainAddress(),
             addressBase.getInstruction(),
             addressBase.getIndex(),
             addressBase.getRegion(),
             addressBase.getArea(),
             addressBase.getDistrict(),
             addressBase.getStreet(),
             addressBase.getHouse(),
             addressBase.getLetter(),
             addressBase.getSlash(),
             addressBase.getCorpus(),
             addressBase.getBuilding(),
             addressBase.getRoom());
    }


    public Address(
            String id,
            String plainAddress,
            String instruction,
            String index,
            String region,
            String area,
            String district,
            String street,
            String house,
            String letter,
            String slash,
            String corpus,
            String building,
            String room)
    {
        super(plainAddress,
              instruction,
              index,
              region,
              area,
              district,
              street,
              house,
              letter,
              slash,
              corpus,
              building,
              room);
        this.id = id;
    }
}
