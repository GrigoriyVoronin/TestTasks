from enum import Enum


class TransportType(Enum):
    Car = 1,
    Motorcycle = 2,
    Carts = 3


class Transport:
    number: int
    transportType: TransportType
    speed: int

    def __init__(self, number, transportType, speed):
        self.number = number
        self.transportType = transportType
        self.speed = speed


class MotorTransport(Transport):
    fuel: int

    def __init__(self, number, transportType, speed, fuel):
        super().__init__(number, transportType, speed)
        self.fuel = fuel


distance: int
time: int


def parse_transport(transport_Str: str):
    info = list(map(int, transport_Str.split()))
    if info[1] == 1:
        return MotorTransport(info[0], TransportType.Car, info[2], info[3])
    if info[1] == 2:
        return MotorTransport(info[0], TransportType.Motorcycle, info[2], info[3])
    if info[1] == 3:
        return Transport(info[0], TransportType.Carts, info[2])


def get_distance_from_end(current_distance):
    remainder = current_distance % distance
    if distance - remainder > remainder:
        return remainder
    return distance - remainder


def calculate_car_distance(car: MotorTransport):
    if car.fuel == 98:
        return get_distance_from_end(car.speed * time)
    if car.fuel == 95:
        return get_distance_from_end(car.speed * time * 0.9)
    if car.fuel == 92:
        return get_distance_from_end(car.speed * time * 0.8)


def calculate_motorcycle_distance(motorcycle: MotorTransport):
    if motorcycle.fuel == 98:
        return get_distance_from_end(motorcycle.speed * time)
    if motorcycle.fuel == 95:
        return get_distance_from_end(motorcycle.speed * time * 0.8)
    if motorcycle.fuel == 92:
        return get_distance_from_end(motorcycle.speed * time * 0.6)


def calculate_carts_distance(cart: Transport):
    return get_distance_from_end(cart.speed * time)


def calculate_distance(transport: Transport):
    if transport.transportType == TransportType.Car:
        return transport.number, calculate_car_distance(transport)
    if transport.transportType == TransportType.Motorcycle:
        return transport.number, calculate_motorcycle_distance(transport)
    if transport.transportType == TransportType.Carts:
        return transport.number, calculate_carts_distance(transport)


def find_min_distance(transports_distances):
    min_distance: int = -1
    for t_distance in transports_distances:
        if min_distance == -1:
            min_distance = t_distance
        if min_distance > t_distance[1]:
            min_distance = t_distance[1]
    return min_distance


if __name__ == '__main__':
    info_input = list(map(int, input().split()))
    distance = info_input[1]
    time = info_input[2]
    transports_strings = list()
    for i in range(info_input[0]):
        transports_strings.append(input())

    transports = list(map(parse_transport, transports_strings))
    transports_distance = list(map(calculate_distance, transports))
    min_distance_from_end = min(transports_distance, key=lambda x: x[1])[1]
    transports_with_min_distance = filter(lambda x: x[1] == min_distance_from_end, transports_distance)
    print(min(transports_with_min_distance, key=lambda x: x[0])[0])
