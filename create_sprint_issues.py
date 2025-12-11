#!/usr/bin/env python3
"""
Script to create GitHub issues from sprint planning documents.

This script parses SPRINT_*.md files and creates:
- 12 parent GitHub issues (one for each sprint)
- Sub-issues for each task within a sprint

The script uses the GitHub CLI (gh) to create issues.

Usage:
    python3 create_sprint_issues.py [--dry-run]
    
Options:
    --dry-run    Show what would be created without actually creating issues

Prerequisites:
    - GitHub CLI (gh) must be installed
    - You must be authenticated with gh (run: gh auth login)
    - You must have permissions to create issues in the repository
"""

import argparse
import re
import subprocess
import sys
from pathlib import Path
from typing import List, Dict, Tuple


class SprintTask:
    """Represents a single task within a sprint."""
    
    def __init__(self, title: str, content: str, sprint_number: int):
        self.title = title
        self.content = content
        self.sprint_number = sprint_number
    
    def to_issue_body(self) -> str:
        """Convert task to GitHub issue body format."""
        return f"**Sprint {self.sprint_number} Task**\n\n{self.content}"


class Sprint:
    """Represents a complete sprint with its tasks."""
    
    def __init__(self, number: int, title: str, summary: str, goal: str, 
                 tech_stack: str, tasks: List[SprintTask]):
        self.number = number
        self.title = title
        self.summary = summary
        self.goal = goal
        self.tech_stack = tech_stack
        self.tasks = tasks
    
    def to_issue_title(self) -> str:
        """Generate the GitHub issue title for the sprint."""
        return self.title
    
    def to_issue_body(self) -> str:
        """Generate the GitHub issue body for the sprint."""
        body = f"## Summary\n\n{self.summary}\n\n"
        body += f"## 🎯 Goal\n\n{self.goal}\n\n"
        body += f"## 💻 Tech Stack Focus\n\n{self.tech_stack}\n\n"
        body += f"## Tasks\n\n"
        
        for i, task in enumerate(self.tasks, 1):
            body += f"{i}. {task.title}\n"
        
        return body


def parse_sprint_file(filepath: Path) -> Sprint:
    """Parse a sprint markdown file and extract sprint and task information."""
    
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    lines = content.split('\n')
    
    # Extract sprint number from filename
    sprint_number = int(re.search(r'SPRINT_(\d+)', filepath.name).group(1))
    
    # Extract title (first line, remove markdown heading)
    title_match = re.match(r'^#\s+(.+)$', lines[0])
    title = title_match.group(1) if title_match else f"Sprint {sprint_number}"
    
    # Extract summary
    summary = ""
    in_summary = False
    for i, line in enumerate(lines):
        if line.strip() == "## Summary":
            in_summary = True
            continue
        if in_summary:
            if line.startswith("## "):
                break
            if line.strip():
                summary += line.strip() + " "
    summary = summary.strip()
    
    # Extract goal
    goal = ""
    in_goal = False
    for i, line in enumerate(lines):
        if line.strip() == "## 🎯 Goal" or line.strip() == "## Goal":
            in_goal = True
            continue
        if in_goal:
            if line.startswith("## ") and not line.strip().startswith("##"):
                break
            if line.strip() and not line.startswith("##"):
                goal += line.strip() + "\n"
    goal = goal.strip()
    
    # Extract tech stack
    tech_stack = ""
    in_tech = False
    for i, line in enumerate(lines):
        if "Tech Stack Focus" in line and line.startswith("## "):
            in_tech = True
            continue
        if in_tech:
            if line.startswith("## ") or line.strip() == "-----":
                break
            if line.strip():
                tech_stack += line.strip() + "\n"
    tech_stack = tech_stack.strip()
    
    # Extract tasks
    tasks = []
    task_pattern = re.compile(r'^###\s+Task\s+\d+:\s+(.+)$')
    
    current_task_title = None
    current_task_content = []
    in_task = False
    
    for line in lines:
        # Check if this is a task header
        task_match = task_pattern.match(line)
        if task_match:
            # Save previous task if exists
            if current_task_title:
                task_content = '\n'.join(current_task_content).strip()
                tasks.append(SprintTask(current_task_title, task_content, sprint_number))
            
            # Start new task
            current_task_title = task_match.group(1)
            current_task_content = []
            in_task = True
            continue
        
        # If we're in a task, collect content until next task or end
        if in_task:
            # Stop at next task or major section
            if line.startswith("### Task") or (line.startswith("## ") and "Task Breakdown" not in line):
                continue
            current_task_content.append(line)
    
    # Don't forget the last task
    if current_task_title:
        task_content = '\n'.join(current_task_content).strip()
        tasks.append(SprintTask(current_task_title, task_content, sprint_number))
    
    return Sprint(sprint_number, title, summary, goal, tech_stack, tasks)


def create_github_issue(title: str, body: str, labels: List[str] = None, dry_run: bool = False) -> str:
    """
    Create a GitHub issue using the GitHub CLI.
    
    Args:
        title: Issue title
        body: Issue body (markdown formatted)
        labels: List of label names to apply
        dry_run: If True, don't actually create the issue
    
    Returns the issue number as a string (or "DRY-RUN-X" if dry_run is True).
    """
    cmd = ['gh', 'issue', 'create', '--title', title, '--body', body]
    
    if labels:
        cmd.extend(['--label', ','.join(labels)])
    
    if dry_run:
        # Return a placeholder issue number
        return "DRY-RUN"
    
    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        # Extract issue number from output (typically a URL)
        output = result.stdout.strip()
        # URL format: https://github.com/owner/repo/issues/123
        issue_number = output.split('/')[-1]
        return issue_number
    except subprocess.CalledProcessError as e:
        print(f"Error creating issue: {e.stderr}", file=sys.stderr)
        raise


def create_sub_issue(parent_issue_number: str, task: SprintTask, dry_run: bool = False) -> str:
    """
    Create a sub-issue (task issue) linked to a parent sprint issue.
    
    GitHub doesn't have native sub-issues, but we can link them by:
    1. Mentioning the parent issue in the body
    2. Using labels to indicate the relationship
    
    Args:
        parent_issue_number: The issue number of the parent sprint
        task: The task to create an issue for
        dry_run: If True, don't actually create the issue
    
    Returns the issue number as a string.
    """
    title = f"Sprint {task.sprint_number}: {task.title}"
    body = f"Parent issue: #{parent_issue_number}\n\n{task.to_issue_body()}"
    labels = [f"sprint-{task.sprint_number}", "task"]
    
    return create_github_issue(title, body, labels, dry_run)


def main():
    """Main execution function."""
    
    # Parse command line arguments
    parser = argparse.ArgumentParser(
        description='Create GitHub issues from sprint planning documents',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
    # Dry run to see what would be created
    python3 create_sprint_issues.py --dry-run
    
    # Actually create the issues
    python3 create_sprint_issues.py
        """
    )
    parser.add_argument(
        '--dry-run',
        action='store_true',
        help='Show what would be created without actually creating issues'
    )
    
    args = parser.parse_args()
    
    # Find all sprint planning documents
    planning_dir = Path(__file__).parent / "reference" / "planning"
    
    if not planning_dir.exists():
        print(f"Error: Planning directory not found at {planning_dir}", file=sys.stderr)
        sys.exit(1)
    
    sprint_files = sorted(planning_dir.glob("SPRINT_*.md"), 
                          key=lambda x: int(re.search(r'SPRINT_(\d+)', x.name).group(1)))
    
    if not sprint_files:
        print(f"Error: No SPRINT_*.md files found in {planning_dir}", file=sys.stderr)
        sys.exit(1)
    
    print(f"Found {len(sprint_files)} sprint planning documents")
    
    if args.dry_run:
        print("\n*** DRY RUN MODE - No issues will be created ***\n")
    
    print("=" * 80)
    
    # Process each sprint
    for sprint_file in sprint_files:
        print(f"\nProcessing {sprint_file.name}...")
        
        # Parse the sprint file
        sprint = parse_sprint_file(sprint_file)
        
        print(f"  Title: {sprint.title}")
        print(f"  Tasks: {len(sprint.tasks)}")
        
        # Create the parent sprint issue
        print(f"  Creating parent issue for Sprint {sprint.number}...")
        parent_issue_body = sprint.to_issue_body()
        
        if args.dry_run:
            print(f"    Would create issue:")
            print(f"      Title: {sprint.to_issue_title()}")
            print(f"      Labels: sprint-{sprint.number}, sprint")
            print(f"      Body: {len(parent_issue_body)} characters")
            parent_issue_number = f"DRY-{sprint.number}"
        else:
            parent_issue_number = create_github_issue(
                sprint.to_issue_title(),
                parent_issue_body,
                [f"sprint-{sprint.number}", "sprint"],
                dry_run=args.dry_run
            )
        
        print(f"  ✓ Created parent issue #{parent_issue_number}")
        
        # Create sub-issues for each task
        for i, task in enumerate(sprint.tasks, 1):
            print(f"    Creating task {i}/{len(sprint.tasks)}: {task.title[:50]}...")
            
            if args.dry_run:
                print(f"      Would create sub-issue:")
                print(f"        Title: Sprint {task.sprint_number}: {task.title}")
                print(f"        Labels: sprint-{task.sprint_number}, task")
                print(f"        Parent: #{parent_issue_number}")
                task_issue_number = f"DRY-{sprint.number}-{i}"
            else:
                task_issue_number = create_sub_issue(parent_issue_number, task, dry_run=args.dry_run)
            
            print(f"    ✓ Created task issue #{task_issue_number}")
        
        print(f"  ✓ Sprint {sprint.number} complete!")
    
    print("\n" + "=" * 80)
    
    if args.dry_run:
        print("✓ Dry run complete! Run without --dry-run to actually create issues.")
    else:
        print("✓ All sprint issues created successfully!")
        print("\nYou can view all issues at: https://github.com/ironwolfgames/wayblazer/issues")



if __name__ == "__main__":
    main()
