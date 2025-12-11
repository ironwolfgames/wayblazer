# Sprint Issues Generator

This directory contains a script to automatically create GitHub issues from the sprint planning documents located in `reference/planning/`.

## Overview

The script `create_sprint_issues.py` will:
- Parse all 12 sprint planning documents (`SPRINT_1.md` through `SPRINT_12.md`)
- Create 12 parent GitHub issues (one for each sprint)
- Create sub-issues for each task described in the sprint documents (56 total tasks)
- Link sub-issues to their parent sprint issue using references and labels

## Prerequisites

Before running the script, ensure you have:

1. **GitHub CLI installed**: The script uses the `gh` command-line tool
   ```bash
   # Check if gh is installed
   gh --version
   ```

2. **GitHub Authentication**: You must be authenticated with GitHub
   ```bash
   # Authenticate if needed
   gh auth login
   ```

3. **Repository Permissions**: You must have permission to create issues in the `ironwolfgames/wayblazer` repository

4. **Python 3**: The script requires Python 3.6 or later
   ```bash
   python3 --version
   ```

## Usage

### Dry Run (Recommended First Step)

Before creating actual issues, run in dry-run mode to see what will be created:

```bash
python3 create_sprint_issues.py --dry-run
```

This will:
- Parse all sprint documents
- Show what issues would be created
- Not actually create any GitHub issues
- Help you verify the script is working correctly

### Create Issues

Once you're satisfied with the dry-run output, create the issues:

```bash
python3 create_sprint_issues.py
```

This will:
- Create all 12 parent sprint issues
- Create all 56 sub-issues (tasks)
- Apply appropriate labels to organize the issues
- Link sub-issues to their parent issues

## What Gets Created

### Parent Issues (12 total)

Each sprint gets a parent issue with:
- **Title**: The sprint title (e.g., "⚙️ Sprint 1: Project Setup & Data Core (16 Hours)")
- **Body**: Includes summary, goal, tech stack, and list of tasks
- **Labels**: `sprint-N` (where N is the sprint number) and `sprint`

Example:
```
Title: ⚙️ Sprint 1: Project Setup & Data Core (16 Hours)
Labels: sprint-1, sprint
```

### Sub-Issues (56 total)

Each task within a sprint gets a sub-issue with:
- **Title**: Sprint number and task title (e.g., "Sprint 1: Project Setup and Godot Configuration (2 Hours)")
- **Body**: Reference to parent issue and task details
- **Labels**: `sprint-N` and `task`

Example:
```
Title: Sprint 1: Project Setup and Godot Configuration (2 Hours)
Labels: sprint-1, task
Body: Parent issue: #123
      [task details from sprint document]
```

## Issue Organization

After running the script, you can:

1. **View all sprint issues**: Filter by label `sprint`
2. **View a specific sprint**: Filter by label `sprint-N` (e.g., `sprint-1`)
3. **View all tasks**: Filter by label `task`
4. **View tasks for a sprint**: Combine filters `sprint-N` and `task`

## Testing

To test the parsing without creating issues:

```bash
# Run the dry-run mode
python3 create_sprint_issues.py --dry-run

# Or use the test script
python3 test_sprint_parsing.py
```

## Troubleshooting

### "gh: command not found"

Install the GitHub CLI:
- **macOS**: `brew install gh`
- **Linux**: See https://github.com/cli/cli/blob/trunk/docs/install_linux.md
- **Windows**: See https://github.com/cli/cli/releases

### "authentication required"

Run `gh auth login` and follow the prompts to authenticate.

### "permission denied"

You need write access to the repository to create issues. Contact the repository owner if you need access.

## Sprint Summary

The script will create issues for these sprints:

1. Sprint 1: Project Setup & Data Core (8 tasks)
2. Sprint 2: Grid, Player Movement & Voxel Data (5 tasks)
3. Sprint 3: Resource Engine & ProcGen V1 (6 tasks)
4. Sprint 4: Interaction & Scanner UI (5 tasks)
5. Sprint 5: Field Lab & Analysis Logic (4 tasks)
6. Sprint 6: Planetary Analysis & Deduction Input (4 tasks)
7. Sprint 7: Basic Crafting & Inventory (4 tasks)
8. Sprint 8: Advanced Refining (Composition) (4 tasks)
9. Sprint 9: ProcGen Tech Tree V2 & Unlocks (4 tasks)
10. Sprint 10: Portal Construction & UI (4 tasks)
11. Sprint 11: Simulation Core & Win State (4 tasks)
12. Sprint 12: Aesthetic Polish & Juiciness (4 tasks)

**Total**: 12 parent issues + 56 task issues = 68 issues
