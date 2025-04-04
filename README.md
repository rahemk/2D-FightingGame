# Knockout

Knockout is a local multiplayer 2D fighting game inspired by classic arcade fighters. Built in Godot using C#, it features dynamic combat, character selection, and support for both human and AI opponents.

## Features

- Character selection screen with animated previews.
- Local multiplayer (P1 vs P2) and single-player (P1 vs AI) support.
- Multiple characters with unique animations and inputs.
- Combo input recognition using motion input buffering.
- Attacks, blocking, jumping, crouching, and special moves.
- AI-controlled opponent with timed decision-making.
- Round-based flow with intros, KO logic, and scene transitions.

### Prerequisites

To view or work with this project, you'll need:

- Godot Engine (version 4.x or later)
- A C#-compatible IDE (e.g., Visual Studio, Rider, or VS Code)


### Usage

- Launch the game from the character selection screen.
- Choose your fighter(s) using directional inputs and confirm.
- In single-player mode, Player 2 will be automatically controlled by AI.
- Fight mechanics include directional movement, standard attacks (LP/HP), blocking, crouching, and jump variations.
- Special moves like Hadouken and Shoryuken can be performed using motion inputs.
- The round ends on KO or timeout, triggering a transition back to character select.

