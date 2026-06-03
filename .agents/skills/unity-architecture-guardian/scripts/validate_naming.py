#!/usr/bin/env python3
"""Validate basic Unity file and path naming conventions."""

from __future__ import annotations

import argparse
import re
from pathlib import Path


IGNORED_DIRS = {".git", "Library", "Temp", "Obj", "Build", "Builds", "Logs", "UserSettings"}
UNITY_EXTENSIONS = {".cs", ".asmdef", ".prefab", ".unity", ".asset", ".uxml", ".uss"}
PASCAL_CASE = re.compile(r"^[A-Z][A-Za-z0-9]*$")
KEBAB_CASE = re.compile(r"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$")


def iter_files(root: Path):
    for path in sorted(root.rglob("*")):
        if path.is_dir():
            continue
        if any(part in IGNORED_DIRS for part in path.parts):
            continue
        if path.suffix.lower() in UNITY_EXTENSIONS:
            yield path


def validate(root: Path) -> list[str]:
    warnings: list[str] = []
    for path in iter_files(root):
        stem = path.stem
        suffix = path.suffix.lower()
        parts_lower = [part.lower() for part in path.parts]

        if "editor" in parts_lower and suffix == ".cs":
            text = path.read_text(encoding="utf-8", errors="ignore")
            if "UnityEditor" not in text:
                warnings.append(f"{path}: Editor folder script does not reference UnityEditor; verify placement")

        if suffix in {".cs", ".asmdef"} and not PASCAL_CASE.match(stem.replace(".", "")):
            warnings.append(f"{path}: expected PascalCase-like file name")

        if suffix == ".uss" and not (PASCAL_CASE.match(stem) or KEBAB_CASE.match(stem)):
            warnings.append(f"{path}: USS file should use PascalCase screen name or kebab-case shared style")

        if suffix == ".uxml" and not PASCAL_CASE.match(stem):
            warnings.append(f"{path}: UXML screen/template file should usually use PascalCase")

        if "editor" in parts_lower and "runtime" in parts_lower:
            warnings.append(f"{path}: path mixes Editor and Runtime folders")

        if "tests" in parts_lower and not ({"editmode", "playmode"} & set(parts_lower)):
            warnings.append(f"{path}: tests should be split into EditMode or PlayMode")

    return warnings


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate Unity naming conventions.")
    parser.add_argument("project_path", nargs="?", default=".", help="Unity project or Assets path")
    args = parser.parse_args()

    root = Path(args.project_path).resolve()
    warnings = validate(root)
    if not warnings:
        print(f"No naming warnings under {root}")
        return 0

    print("Naming warnings:")
    for warning in warnings:
        print(f"- {warning}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
