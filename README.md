# Game of Life API Documentation

The Game of Life API provides a way to interact with Conway's Game of Life by defining board states, progressing through generations, and determining final outcomes. This API provides a set of endpoints that simulate a Game of Life.

## Assumptions of the API

### 1. Initial State Defined by Coordinates

The initial board state is defined by a set of coordinates, each representing a cell that is alive at the start. These coordinates are specified in the form of (x, y) pairs.
Any cell not explicitly included in the initial set is assumed to be dead.

### 2. The board is assumed to be an infinite two-dimensional plane
This means there are no boundaries, and cells outside the given initial coordinates can still become alive based on the rules of the Game of Life.

### 3. Persistance
Given this is a demo like application I didn't use a database that could scale greatly. Instead I used a Repository pattern to isolate the implementation of the persistance that could be done through different relational and Non relational databases.

### 4. Reaching Conclusion
A "final state" is reached when the board enters a stable pattern. Stability is defined as a state where the board's configuration remains unchanged between two consecutive iterations (State X-1 equals State X). This could happen due to the board becoming static.

## Endpoints
### 1. Upload a New Board State
Uploads a new board state and returns the ID of the board for further operations.

URL: POST /Process/Board

- **Request Body:**
  ```json
  {
      "coordinates": [
          { "x": 1, "y": 2 },
          { "x": 2, "y": 2 },
          { "x": 3, "y": 2 }
      ]
  }

coordinates: List of live cell coordinates for the initial board state.
Response:

Returns the id of the created board.

### 2. Get the Next State for a Board
Retrieves the next state of the board based on the Game of Life rules.

URL: GET /Process/State/Next/{boardId}

Path Parameters:

boardId (integer): The ID of the board.

- **Request Body:**
  ```json
    [
        { "x": 2, "y": 1 },
        { "x": 2, "y": 2 },
        { "x": 2, "y": 3 }
    ]

Returns a list of coordinates representing the next state of the board.

### 3. Get X States Away for a Board
Retrieves the board's state after a specified number of iterations.

URL: GET /Process/State/{boardId}/Steps/{steps}

Path Parameters:

boardId (integer): The ID of the board.
steps (integer): The number of iterations to calculate.

- **Request Body:**
  ```json
    [
        { "x": 3, "y": 2 },
        { "x": 4, "y": 2 },
        { "x": 5, "y": 2 }
    ]

Returns a list of coordinates representing the board's state after the specified number of steps.

### 4. Get the Final State for a Board
Retrieves the final state of the board. If the board doesn't reach a conclusion after a set number of steps, an error is returned.

URL: GET /Process/State/{boardId}/Final

Path Parameters:

boardId (integer): The ID of the board.

- **Request Body:**
  ```json
    [
        { "x": 0, "y": 0 },
        { "x": 1, "y": 1 }
    ]

Returns the coordinates of the final board state.
Error:
{
    "error": "Unable to determine final state after 100 steps."
}

Returns an error message if the board fails to reach a stable or empty state after the configured number of steps.

## Models

### CreateBoardRequest
Represents the structure of the request body for creating a new board.

- **Request Body:**
  ```json
    {
        "coordinates": [
            { "x": 1, "y": 2 },
            { "x": 2, "y": 2 },
            { "x": 3, "y": 2 }
        ]
    }
  
coordinates: An array of objects representing live cell positions on the initial board.

### Coordinate
Represents a cell's position on the board.

{
    "x": 1,
    "y": 2
}

x: The x-coordinate of the cell.
y: The y-coordinate of the cell.

## Configuration
The number of default steps for the Final State endpoint is determined by the DefaultStepsConfig in the application settings. If the board cannot reach a final state within this limit, an error will be returned.

