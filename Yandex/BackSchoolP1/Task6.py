import argparse
import http.client
import json
import csv

parser = argparse.ArgumentParser()
parser.add_argument("host")
parser.add_argument("port")
parser.add_argument("--not_mult", dest="not_mult", required=False, default=100, type=int)
parser.add_argument("--smallest", dest="smallest", required=False, default=0, type=int, )
row_data: dict[str, list[int]]
not_mult: int
smallest: int


def calculate_data(name_numbers: str):
    max_val = max(filter(lambda x: x >= smallest & x % not_mult == 0, row_data[name_numbers]))
    min_val = min(filter(lambda x: x >= smallest & x % not_mult == 0, row_data[name_numbers]))
    sum_val = sum(filter(lambda x: x >= smallest & x % not_mult == 0, row_data[name_numbers]))
    aver_val = round(sum_val / len(list(filter(lambda x: x >= smallest & x % not_mult == 0, row_data[name_numbers]))), 2)
    return name_numbers, max_val, min_val, aver_val, sum_val


if __name__ == '__main__':
    args = parser.parse_args()
    smallest = args.smallest
    not_mult = args.not_mult
    connection = http.client.HTTPConnection(host=args.host, port=args.port)
    headers = {'Content-type': 'application/json'}
    connection.connect()
    connection.request("GET", "/", headers=headers)
    response = connection.getresponse()
    data = response.read().decode("utf8")
    list_of_data: list[dict[str, list[int]]] = json.decoder.JSONDecoder().decode(data)
    row_data = dict()
    for dictionary in list_of_data:
        for name in dictionary:
            for number in dictionary[name]:
                if name not in row_data:
                    row_data[name] = list()
                row_data[name].append(number)
    answer = list(map(calculate_data, row_data))
    answer.sort(key=lambda x: x[0])

    with open("truth.csv", "w") as file:
        writer = csv.writer(file, delimiter=";")
        for line in answer:
            writer.writerow(line)
