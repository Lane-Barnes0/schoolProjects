originalBoard = [
    [7,8,0,4,0,0,1,2,0],
    [6,0,0,0,7,5,0,0,9],
    [0,0,0,6,0,1,0,7,8],
    [0,0,7,0,4,0,2,6,0],
    [0,0,1,0,5,0,9,3,0],
    [9,0,4,0,6,0,0,0,5],
    [0,7,0,3,0,0,0,1,2],
    [1,2,0,0,0,7,4,0,0],
    [0,4,9,2,0,6,0,0,7]
]

#Hardest Board according to https://abcnews.go.com/blogs/headlines/2012/06/can-you-solve-the-hardest-ever-sudoku
hardestSudoku = [
        [8, 0, 0, 0, 0, 0, 0, 0, 0],
        [0, 0, 3, 6, 0, 0, 0, 0, 0],
        [0, 7, 0, 0, 9, 0, 2, 0, 0],
        [0, 5, 0, 0, 0, 7, 0, 0, 0],
        [0, 0, 0, 0, 4, 5, 7, 0, 0],
        [0, 0, 0, 1, 0, 0, 0, 3, 0],
        [0, 0, 1, 0, 0, 0, 0, 6, 8],
        [0, 0, 8, 5, 0, 0, 0, 1, 0],
        [0, 9, 0, 0, 0, 0, 4, 0, 0]
    ]

def print_board(board):
    for i in range(len(board)):
        if i % 3 == 0 and i != 0:
            print("- - - - - - - - -")
        for j in range(len(board[0])):
            if j % 3 == 0 and j != 0:
                print(" | ", end="")
            if j == 8:
                print(board[i][j])
            else:
                print(board[i][j], end="")

def findEmpty(board):
    for i in range(len(board)):
        for j in range(len(board[i])):
            if board[i][j] == 0:
                return (i, j)
    return None

def validateBoard(board, number, pos):

    #Check row
    for item in board[pos[0]]:
        if item == number and pos[1] != item:
            return False
        
    #Check Column  
    for i in range(len(board)):
        if board[i][pos[1]] == number and pos[0] != i:
            return False
    
    #Check 3 X 3
    boxColumn = pos[1] // 3
    boxRow = pos[0] // 3

    for i in range(boxRow * 3, boxRow * 3 + 3):
        for j in range(boxColumn * 3, boxColumn * 3 + 3):
            if board[i][j] == number and (i,j) != pos:
                return False

    return True

def solve(board):
    emptySpot = findEmpty(board)

    if not emptySpot:
        return True
    else:
        row, col = emptySpot

    for i in range(1, 10):
        if validateBoard(board, i, (row, col)):
            board[row][col] = i

            if solve(board):
                return True
            
            board[row][col] = 0
            
    return False

print("-----Original Board-----")
print_board(originalBoard)
solve(originalBoard)
print()
print("-----Solved Board-----")
print_board(originalBoard)
print()

print("-----Hardest Board-----")
print_board(hardestSudoku)
solve(hardestSudoku)
print()
print("-----Solved Hardest Board-----")
print_board(hardestSudoku)
print()