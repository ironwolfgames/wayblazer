#!/usr/bin/env python3
"""
Test script to validate sprint parsing without creating GitHub issues.
"""

import re
from pathlib import Path
from create_sprint_issues import parse_sprint_file


def main():
    """Test the parsing of sprint files."""
    
    planning_dir = Path(__file__).parent / "reference" / "planning"
    
    if not planning_dir.exists():
        print(f"Error: Planning directory not found at {planning_dir}")
        return
    
    sprint_files = sorted(planning_dir.glob("SPRINT_*.md"), 
                          key=lambda x: int(re.search(r'SPRINT_(\d+)', x.name).group(1)))
    
    print(f"Found {len(sprint_files)} sprint planning documents\n")
    print("=" * 80)
    
    total_tasks = 0
    
    for sprint_file in sprint_files:
        print(f"\n{sprint_file.name}")
        print("-" * 80)
        
        sprint = parse_sprint_file(sprint_file)
        
        print(f"Sprint Number: {sprint.number}")
        print(f"Title: {sprint.title}")
        print(f"Summary: {sprint.summary[:100]}...")
        print(f"Goal: {sprint.goal[:100]}..." if len(sprint.goal) > 100 else f"Goal: {sprint.goal}")
        print(f"Number of Tasks: {len(sprint.tasks)}")
        
        for i, task in enumerate(sprint.tasks, 1):
            print(f"  {i}. {task.title}")
        
        total_tasks += len(sprint.tasks)
    
    print("\n" + "=" * 80)
    print(f"Total: {len(sprint_files)} sprints, {total_tasks} tasks")
    print("=" * 80)


if __name__ == "__main__":
    main()
