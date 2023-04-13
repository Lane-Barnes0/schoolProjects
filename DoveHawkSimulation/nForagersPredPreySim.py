import random

class boardSpot:
    def __init__(self):
        self.food = 2
        self.animals = []


class Animal:
    def __init__(self, type):
        self.food = 0
        self.type = type
        self.survived = 1

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
            board[i][j].animals = []




def selectSpot(board, animal):
    row = random.randint(0, len(board) - 1)
    col = random.randint(0, len(board[0]) - 1)
    board[row][col].animals.append(animal)


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
            hawks = []
            doves = []
            for animal in boardSpace.animals:
                if animal.type == "hawk":
                    hawks.append(animal)
                else:
                    doves.append(animal)

            if len(doves) == 0 and len(hawks) != 0:
                bestHawk = hawks[0]
                for hawk in hawks:
                    if hawk.survived > bestHawk.survived:
                        bestHawk = hawk
                bestHawk.food += 2  
            elif len(hawks) == 0:
                for dove in doves:
                    dove.food += 2/len(doves)
            else:
                bestHawk = hawks[0]
                for hawk in hawks:
                    if hawk.survived > bestHawk.survived:
                        bestHawk = hawk
                bestHawk.food += 1
                for dove in doves:
                    dove.food += 1/len(doves)

    #After each animal has food find out who lives, dies, and reproduces
    nextGeneration = []
    for animal in animals:
        if animal.food == 1:
            animal.food = 0
            animal.survived += 1
            nextGeneration.append(animal)
        elif animal.food == 2:
            animal.food = 0
            animal.survived += 1
            nextGeneration.append(animal)
            nextGeneration.append(Animal(animal.type))
        else:
            chance = random.random()
            if chance <= animal.food:
                animal.food = 0
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
while gen < 300:
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


for animal in animals:
    if animal.type == "hawk":
        print("\tHawk Survived: " + str(animal.survived) + " Generations")
