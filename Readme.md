# BoardGame Framework (C# .NET 10)

Welcome to the **BoardGame Framework**, a modular, console-based two-player board game engine built using modern **C# (.NET 10)**.
This system integrates multiple board games into a single unified architecture with support for gameplay, AI, undo/redo, and save/load functionality.

---

##  Overview

The framework provides a single runtime environment for multiple board games with:

- Human vs Human mode  
- Human vs Computer mode  
- Undo / Redo system  
- Save / Load system  
- AI opponent  

---

## Core Features

-  5 different board games in one system  
- Unified game loop for all games  
- Undo / Redo functionality (`u`, `r`)  
- Save / Load support (.txt and .json)  
- Simple AI opponent  

---

## Supported Games

| Game                       | Board Size | Rules                       |
|----------------------------|------------|-----------------------------|
| Tic-Tac-Toe                | 3×3        | 3 in a row wins             |
| Numerical Tic-Tac-Toe      | 3×3        | Line sum must equal 15      |
| Notakto                    | 3×3        | Completing a line loses     |
| Gomoku                     | 15×15      | 5 in a row wins             |
| Connect Four               | 6×7        | 4 in a row (gravity-based)  |

---

##  How to Run

### Prerequisites
- .NET SDK 10+
- Terminal / IDE supporting .NET console apps

### Build & Run

```bash
cd BoardGame
dotnet build
dotnet run
---
```

## Main Menu

When the application starts, you will see:

| Option| Action           |
|-------|------------------|
| 1     | Start a New Game |
| 2     | Load a Saved Game|
| 3     | Exit             |

---

## Start a New Game Flow

1. Select `1`
2. Choose a game (1–5)
3. Choose game mode:
   - `1` = Human vs Human
   - `2` = Human vs Computer
4. Follow prompts to play

---

## Load Saved Game Flow

1. Select `2` from the main menu  
2. Enter filename (`.txt` or `.json`)  
3. Game state is restored automatically  

---

## In-Game Commands

| Command | Action            |
|---------|-------------------|
| u       | Undo last move    |
| r       | Redo move         |
| s       | Save game         |
| h       | Help menu         |
| e       | Exit to main menu |

---

## Move Input Formats

| Game | Input Format |
|------------------------ |------------------|
| Tic-Tac-Toe             | `row col`        |
| Notakto                 | `row col`        |
| Gomoku                  | `row col`        |
| Numerical Tic-Tac-Toe   | `number row col` |
| Connect Four            | `column`         |

---

## Saving & Loading System

### Save Game
During gameplay, press: s


Then:

- Enter filename  
- Choose format:  
  - `t` → Text file  
  - `j` → JSON file  

### Load Game
From the main menu:


Then enter filename (extension optional).

---

## Full Game Flow

1. Start program  
2. Main Menu appears  
3. Select game or load save  
4. Choose mode (H vs H / H vs AI)  
5. Play using move inputs  
6. Use commands (`u`, `r`, `s`, `h`, `e`)  
7. Win / draw detection ends game  

---

## Project Structure

BoardGame/
├── Program.cs
├── Core/
│   ├── Game.cs
│   ├── Board.cs
│   ├── Move.cs
│   ├── GameState.cs
│   └── WinChecker.cs
├── Games/
│   ├── TicTacToeGame.cs
│   ├── NumericalTicTacToeGame.cs
│   ├── NotaktoGame.cs
│   ├── GomokuGame.cs
│   └── ConnectFourGame.cs
├── Moves/
│   └── MoveHistory.cs
├── Players/
│   └── Players.cs
├── SaveLoad/
│   ├── ISaveStrategy.cs
│   ├── TextSaveStrategy.cs
│   ├── JsonSaveStrategy.cs
│   └── SaveStrategyFactory.cs
└── UI/
└── HelpMenu.cs


---

## Architecture & Design Patterns

- Template Method Pattern → Core game loop  
- Strategy Pattern → Save system (Text / JSON)  
- Factory Pattern → Game creation  
- Memento Pattern → Undo / Redo system  
- Prototype Pattern → Board cloning for AI  

---

## AI Behaviour

The computer player:

1. Checks for immediate winning move  
2. If none found → selects a random valid move  

---

## Notes

- Fully console-based application  
- Modular and scalable design  
- Easy to add new games  
- Clean separation of concerns  

