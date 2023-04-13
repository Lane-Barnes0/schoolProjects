import random

class boardSpot:
    def __init__(self):
        self.food = 2
        self.open1 = None
        self.open2 = None

class Animal:
    def __init__(self, type):
        self.food = 0
        self.type = type
        self.survived = 0

def createBoard():
    board = []
    for i in range(10):
        board.append([])
        for j in range(10):
            board[i].append(boardSpot())
    return board

def resetBoard(board):
    for i in range(len(board)):
        for j in range(len(board[i])):
            board[i][j].food = 2
            board[i][j].open1 = None
            board[i][j].open2 = None

def printBoard(board):
    for i in range(len(board)):
        for j in range(len(board[i])):
            if board[i][j].open1 != None:
                print(str(board[i][j].open1.food) + " ", end="")
            else:
                print(str(board[i][j].open1) + " ", end="")
        print()







def selectSpot(board, animal):
    row = random.randint(0, len(board) - 1)
    col = random.randint(0, len(board[0]) - 1)
    if board[row][col].open1 == None:
        board[row][col].open1 = animal
    elif board[row][col].open2 == None:
        board[row][col].open2 = animal
    else:
        selectSpot(board, animal)

#Randomly set a location on the board
def simulate(board, animals):
    #Animal Chooses A spot
    for animal in animals:
        selectSpot(board, animal)

    #After Every Animal has chosen a spot give out the food
    for i in range(len(board)):
        for j in range(len(board[i])):
            #Both Spots are Taken
            boardSpace = board[i][j]
            if boardSpace.open1 != None and boardSpace.open2 != None:
                if boardSpace.open1.type == "hawk" and board[i][j].open2.type == "hawk":
                   boardSpace.open1.food += 0
                   boardSpace.open2.food += 0
                elif boardSpace.open1.type == "hawk":
                    boardSpace.open1.food += 1.5
                    boardSpace.open2.food += 0.5
                elif boardSpace.open2.type == "hawk":
                    boardSpace.open1.food += 0.5
                    boardSpace.open2.food += 1.5
                else:
                    boardSpace.open1.food += 1
                    boardSpace.open2.food += 1
            elif boardSpace.open1 != None:
                boardSpace.open1.food += 2
            elif boardSpace.open2 != None:
                boardSpace.open2.food += 2

    #After each animal has food find out who lives, dies, and reproduces
    nextGeneration = []
    for animal in animals:
        if animal.food == 1:
            animal.food = 0
            nextGeneration.append(animal)
        elif animal.food == 2:
            animal.food = 0
            nextGeneration.append(animal)
            nextGeneration.append(Animal(animal.type))
        elif animal.food == 1.5:
            animal.food = 0
            chance = random.random()
            nextGeneration.append(animal)
            if chance <= 0.5:
                nextGeneration.append(Animal(animal.type))
        elif animal.food == 0.5:
            animal.food = 0
            chance = random.random()
            if chance <= 0.5:
                nextGeneration.append(animal)

    #Reset Animals to the next Generation and reset the board with new food
    
    resetBoard(board)
    return nextGeneration

board = createBoard()
animals = []
startingPopulation = 6
for i in range(startingPopulation):
    animals.append(Animal("prey"))

gen = 0
print("Dove Simulation All Prey")
while len(animals) < 200:
    print("Generation " + str(gen) + " Population " + str(len(animals)))
    animals = simulate(board, animals)
    gen += 1
    
print("Generation " + str(gen) + " Population " + str(len(animals)))

print()
board = createBoard()
animals = []
startingPopulation = 6
for i in range(startingPopulation):
    animals.append(Animal("hawk"))

gen = 0
print("Hawk Simulation All Hawks")
while gen < 100:
    print("Generation " + str(gen) + " Population " + str(len(animals)))
    animals = simulate(board, animals)
    gen += 1
    
print("Generation " + str(gen) + " Population " + str(len(animals)))

print()

board = createBoard()
animals = []
startingPopulation = 6
for i in range(startingPopulation):
    animals.append(Animal("prey"))

#Add one Hawk
animals.append(Animal("hawk"))

gen = 0
print("Dove Simulation One Hawk")
while gen < 100:
    hawks = 0
    for animal in animals:
        if animal.type == "hawk":
            hawks += 1

    print("Generation " + str(gen) + " Population " + str(len(animals)))
    print("\tHawks: " + str(hawks) + " Doves " + str(len(animals) - hawks) )
    animals = simulate(board, animals)
    gen += 1
    
print("Generation " + str(gen) + " Population " + str(len(animals)))
print("\tHawks: " + str(hawks) + " Doves " + str(len(animals) - hawks) )
