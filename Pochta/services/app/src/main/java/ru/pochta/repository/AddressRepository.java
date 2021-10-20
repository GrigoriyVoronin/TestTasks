package ru.pochta.repository;

import java.util.List;

import org.springframework.data.jdbc.repository.query.Modifying;
import org.springframework.data.jdbc.repository.query.Query;
import org.springframework.data.repository.CrudRepository;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;
import ru.pochta.models.address.Address;

/**
 * @author voroningg
 */
@Repository
public interface AddressRepository extends CrudRepository<Address, String> {

    @Query("SELECT * FROM public.\"Addresses\" ORDER BY id OFFSET :skip ROWS FETCH NEXT :take ROWS ONLY")
    List<Address> getAddressesPage(@Param("skip") int skip, @Param("take") int take);

    @Modifying
    @Query("create table \"Addresses\"\n" +
            "(\n" +
            "\t\"id\" text not null,\n" +
            "\tplain_address text not null,\n" +
            "\tinstruction text,\n" +
            "\tindex text,\n" +
            "\tregion text,\n" +
            "\tarea text,\n" +
            "\tplace text,\n" +
            "\tdistrict text,\n" +
            "\tstreet text,\n" +
            "\thouse text,\n" +
            "\tletter text,\n" +
            "\tslash text,\n" +
            "\tcorpus text,\n" +
            "\tbuilding text,\n" +
            "\troom text\n" +
            ");\n" +
            "\n" +
            "create unique index addresses_id_uindex\n" +
            "\ton \"Addresses\" (\"id\");\n" +
            "\n" +
            "alter table \"Addresses\"\n" +
            "\tadd constraint addresses_pk\n" +
            "\t\tprimary key (\"id\");\n" +
            "\n")
    void createTable();
}
