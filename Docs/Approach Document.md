# Sudoku Game Development – Approach Document

Name: Hanish Vigneshwar

Project Title: Development of a Polished Sudoku Game in Unity
# **1. Objective**
The objective of this project is to develop a fully functional, visually polished Sudoku game using the Unity game engine. The game will feature procedural puzzle generation, a clean and intuitive user interface, input validation, difficulty modes, and responsive feedback to enhance player engagement and usability.
# **2. Scope**
This project includes:\
\- Interactive 9×9 Sudoku grid with selectable and editable cells\
\- Puzzle generation and unique solution validation using backtracking\
\- Difficulty levels (Easy, Medium, Hard)\
\- User input validation and error highlighting\
\- Cell highlighting for selection, related rows, columns, and boxes\
\- Timer and game restart functionality\
\- Responsive UI layout suitable for desktop and WebGL
# **3. Tools and Technologies**

|Component|Tool / Technology|
| :- | :- |
|Game Engine|Unity 2022.3 (2D Project)|
|Programming Language|C#|
|UI Framework|Unity Canvas System|
|Puzzle Algorithm|Backtracking |
|Target Platforms|PC, WebGL |
# **4. Project Components**
## **4.1 Game Design**
\- Flat modern visual style with a light color palette\
\- Clear, readable UI with consistent font usage (e.g., Open Sans)\
\- Highlighting and feedback for ease of play and reduced user error
## **4.2 Core Functional Modules**
a. SudokuCell.cs

- - Manages input, highlighting, and editability of a cell\
-     - Validates digit entry (1–9 only)

b. GameManager.cs

- - Handles board loading, input response, win detection, and UI updates\
-     - Manages locked cells and game flow

c. SudokuGenerator.cs

- - Generates full Sudoku puzzles using recursive backtracking\
-     - Masks cells based on difficulty to create playable puzzles\
-     - Ensures uniqueness of solutions

d. BoardData.cs

- - Stores original puzzle and solution\
-     - Provides utility functions for puzzle validation
# **5. Development Phases**

|Phase|Tasks|Estimated Time|
| :- | :- | :- |
|1|Unity project setup, UI grid, and prefab creation|Day 1|
|2|Implement puzzle generator and solver|Day 2–3|
|3|Input handling, validation, and cell highlighting|Day 4|
|4|Build GameManager logic, timer, and restart functions|Day 5|
|5|UI polish (fonts, spacing, color feedback), menu screens|Day 6|
|6|Testing, refinement, and optional WebGL/PC export|Day 7–8|
# **6. Expected Outcome**
\- A complete Sudoku game with clean and modern UI\
\- Input validation and solution checking system\
\- Export-ready for PC or WebGL with responsive UI\
\- Efficient and reusable codebase structured for future expansion
# **7. Conclusion**
This project targets the delivery of a production-ready Sudoku game within a short development cycle of one to two weeks. It will demonstrate proficiency in game logic, user interface design, and the Unity game engine, making it a strong addition to both academic and personal portfolios.
