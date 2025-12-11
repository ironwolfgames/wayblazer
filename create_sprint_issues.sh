#!/bin/bash
# Wrapper script to create GitHub issues from sprint planning documents

set -e  # Exit on error

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "================================"
echo "Sprint Issues Generator"
echo "================================"
echo ""

# Check if gh is installed
if ! command -v gh &> /dev/null; then
    echo "Error: GitHub CLI (gh) is not installed."
    echo "Please install it from: https://cli.github.com/"
    exit 1
fi

# Check if gh is authenticated
if ! gh auth status &> /dev/null; then
    echo "Error: You are not authenticated with GitHub."
    echo "Please run: gh auth login"
    exit 1
fi

# Check Python version
if ! command -v python3 &> /dev/null; then
    echo "Error: Python 3 is not installed."
    exit 1
fi

echo "✓ GitHub CLI is installed and authenticated"
echo "✓ Python 3 is available"
echo ""

# Parse command line arguments
DRY_RUN=""
if [[ "$1" == "--dry-run" ]]; then
    DRY_RUN="--dry-run"
    echo "Running in DRY RUN mode - no issues will be created"
    echo ""
fi

# Run the Python script
python3 create_sprint_issues.py $DRY_RUN

echo ""
echo "================================"
if [[ -n "$DRY_RUN" ]]; then
    echo "Dry run complete!"
    echo "To actually create issues, run:"
    echo "  ./create_sprint_issues.sh"
else
    echo "Issues created successfully!"
    echo "View them at: https://github.com/ironwolfgames/wayblazer/issues"
fi
echo "================================"
