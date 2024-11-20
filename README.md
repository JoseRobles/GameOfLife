# Game of Life API

Welcome to the Game of Life API! This API allows you to interact with Conway's Game of Life by uploading board states, retrieving future states, and exploring the board's progression.

---

## Endpoints

### 1. Upload a New Board State
Uploads a new board state and returns the ID of the board for further operations.

- **URL:** `POST /Process/Board`
- **Request Body:**
  ```json
  {
      "coordinates": [
          { "x": 1, "y": 2 },
          { "x": 2, "y": 2 },
          { "x": 3, "y": 2 }
      ]
  }
