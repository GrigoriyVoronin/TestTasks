package ru.pochta.models.address;

import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;

import com.google.gson.annotations.SerializedName;
import lombok.Getter;
import lombok.Setter;

/**
 * @author voroningg
 */
@Getter
@Setter
public abstract class AddressBase {
    @SerializedName("plain_address")
    @NotNull
    private String plainAddress;
    private String instruction;
    @Size(max = 9)
    private String index;
    @Size(max = 200)
    private String region;
    @Size(max = 200)
    private String area;
    @Size(max = 200)
    private String place;
    @Size(max = 200)
    private String district;
    @Size(max = 200)
    private String street;
    @Size(max = 60)
    private String house;
    @Size(max = 2)
    private String letter;
    @Size(max = 8)
    private String slash;
    @Size(max = 8)
    private String corpus;
    @Size(max = 8)
    private String building;
    @Size(max = 60)
    private String room;

    public AddressBase() {

    }

    public AddressBase(
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
        this.plainAddress = plainAddress;
        this.instruction = instruction;
        this.index = index;
        this.region = region;
        this.area = area;
        this.district = district;
        this.street = street;
        this.house = house;
        this.letter = letter;
        this.slash = slash;
        this.corpus = corpus;
        this.building = building;
        this.room = room;
    }
}
