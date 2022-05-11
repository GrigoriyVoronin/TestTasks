import csv
from models.InputLine import InputLine

FILE_NAME = "recommends.csv"


def parse_csv_file():
    with open(FILE_NAME, "r") as file:
        reader = csv.reader(file)
        for row in reader:
            yield InputLine(row)
