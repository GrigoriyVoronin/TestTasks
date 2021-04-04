create table if not exists couriers(
    id integer primary key,
    type varchar,
    regions varchar,
    working_hours varchar
);

create table if not exists orders(
    id integer primary key,
    weight float,
    region integer,
    delivery_hours varchar,

    is_complete bit,
    courier_id integer,
    delivery_time integer,
    delivery_id integer,
    foreign key (delivery_id) references deliveries(id),
    foreign key (courier_id) references couriers(id)
);

create table if not exists deliveries(
    id integer primary key autoincrement,
    courier_type varchar,
    previous_order_complete_time varchar,
    courier_id integer,
    assign_time varchar,
    is_complete bit,
    foreign key (courier_id) references couriers(id)
);