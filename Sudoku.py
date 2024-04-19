import time


class Board:
    def __init__(self, board):
        self.board = board
        self.original = [[], [], [], [], [], [], [], [], []]
        for i in range(9):
            for j in range(9):
                self.original[i].append(self.board[i][j])

    def sett(self, row, col, value):
        self.board[row][col] = value

    def get(self, row, col):
        return self.board[row][col]

    def inc(self, row, col):
        self.sett(row, col, self.get(row, col) + 1)

    def getRow(self, row, exclude=None):
        if exclude is None:
            exclude = -1
        return [self.board[row][i] for i in range(len(self.board[row])) if i != exclude]

    def getCol(self, col, exclude=None):
        if exclude is None:
            exclude = -1
        return [self.board[i][col] for i in range(len(self.board)) if i != exclude]

    def hasChanged(self, row, col):
        return self.board[row][col] != self.original[row][col]

    def readAble(self):
        string = ""
        for row in self.board:
            for col in row:
                string += str(col) + " "
            string += "\n"
        return string

    def getSquare(self, row, col):
        board = self.board
        square = []
        if row < 3:
            if col < 3:
                for i in range(3):
                    for j in range(3):
                        if not (i == row and j == col):
                            square.append(board[i][j])
                return square
            if col < 6:
                for i in range(3):
                    for j in range(3):
                        if not (i == row and j + 3 == col):
                            square.append(board[i][j + 3])
                return square

            for i in range(3):
                for j in range(3):
                    if not (i == row and j + 6 == col):
                        square.append(board[i][j + 6])
            return square
        if row < 6:
            if col < 3:
                for i in range(3):
                    for j in range(3):
                        if not (i + 3 == row and j == col):
                            square.append(board[i + 3][j])
                return square
            if col < 6:
                for i in range(3):
                    for j in range(3):
                        if not (i + 3 == row and j + 3 == col):
                            square.append(board[i + 3][j + 3])
                return square
            for i in range(3):
                for j in range(3):
                    if not (i + 3 == row and j + 6 == col):
                        square.append(board[i + 3][j + 6])
            return square
        if col < 3:
            for i in range(3):
                for j in range(3):
                    if not (i + 6 == row and j == col):
                        square.append(board[i + 6][j])
            return square
        if col < 6:
            for i in range(3):
                for j in range(3):
                    if not (i + 6 == row and j + 3 == col):
                        square.append(board[i + 6][j + 3])
            return square
        for i in range(3):
            for j in range(3):
                if not (i + 6 == row and j + 6 == col):
                    square.append(board[i + 6][j + 6])
        return square


def solve(board):
    startTime, gameBoard = time.time(), Board(board)
    row, col, backTrack = 0, 0, False
    inc, sett, get = gameBoard.inc, gameBoard.sett, gameBoard.get
    while 1:
        if backTrack:
            check = lastModified(row, col, gameBoard)
            backTrack = False
        else:
            check = findFirstBlank(gameBoard)
            if check == -1:
                break
        row = check[0]
        col = check[1]
        inc(row, col)
        if isValid(row, col, gameBoard):
            continue
        else:
            while not isValid(row, col, gameBoard):
                inc(row, col)
                if get(row, col) > 9:
                    sett(row, col, 0)
                    backTrack = True
                    break
    solveTime = time.time() - startTime
    print(gameBoard.readAble())
    print("In: ", solveTime)
    return gameBoard.board


def isValid(row, col, gameBoard):
    if row > 8 or col > 8 or (gameBoard.get(row, col) not in range(1, 10)):
        return False
    inCol = (gameBoard.get(row, col) in gameBoard.getCol(col, exclude=row))
    inRow = (gameBoard.get(row, col) in gameBoard.getRow(row, exclude=col))
    inSquare = (gameBoard.get(row, col) in gameBoard.getSquare(row, col))
    return not (inCol or inRow or inSquare)


def findFirstBlank(gameBoard):
    b = gameBoard.board
    for row in b:
        if 0 in row:
            col = row.index(0)
            return [b.index(row), col]
    return -1


def lastModified(startRow, startCol, gameBoard):
    row = startRow
    col = startCol
    hasChanged = gameBoard.hasChanged
    if col == 0:
        if row == 0:
            raise Exception("No Solution Available")
        row -= 1
        col = 8
    else:
        col -= 1
    while not hasChanged(row, col):
        col -= 1
        if col < 0:
            col = 8
            row -= 1
            if row < 0:
                raise Exception("No Solution Available")
    return [row, col]


solve([
    [0, 8, 3, 7, 0, 1, 6, 0, 0],
    [0, 4, 0, 0, 0, 0, 0, 0, 5],
    [0, 0, 0, 0, 8, 0, 0, 0, 0],

    [0, 0, 0, 0, 9, 0, 2, 0, 0],
    [0, 3, 0, 2, 0, 8, 0, 4, 0],
    [0, 0, 8, 0, 6, 0, 0, 0, 0],

    [0, 0, 0, 9, 0, 0, 0, 0, 0],
    [0, 0, 1, 3, 0, 2, 7, 0, 0],
    [7, 0, 0, 0, 0, 0, 0, 6, 0]
])
