class Robot:
    __x: int
    __y: int

    def __init__(self, coordinates):
        self.__x = coordinates[0]
        self.__y = coordinates[1]
        self.__last_path = list()
        self.__last_path.append((self.__x, self.__y))

    def move_n(self):
        if self.__y < 100:
            self.__y += 1

    def move_s(self):
        if self.__y > 0:
            self.__y -= 1

    def move_e(self):
        if self.__x < 100:
            self.__x += 1

    def move_w(self):
        if self.__x > 0:
            self.__x -= 1

    command_to_func = {
        "N": move_n,
        "S": move_s,
        "E": move_e,
        "W": move_w
    }

    __last_path: list[(int, int)]

    def move(self, commands: str):
        self.__last_path = list()
        self.__last_path.append((self.__x, self.__y))
        for command in commands:
            self.command_to_func[command](self)
            self.__last_path.append((self.__x, self.__y))
        return self.__x, self.__y

    def path(self):
        return self.__last_path
