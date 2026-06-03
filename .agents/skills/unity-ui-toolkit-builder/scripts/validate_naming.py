#!/usr/bin/env python3
"""Validate basic Unity UI Toolkit file naming conventions."""

from __future__ import annotations

import argparse
import re
from pathlib import Path


PASCAL_CASE = re.compile(r"^[A-Z][A-Za-z0-9]*$")
KEBAB_CASE = re.compile(r"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$")


def validate(root: Path) -> list[str]:
    warnings: list[str] = []
    for path in sorted(root.rglob("*")):
        if not path.is_file():
            continue
        suffix = path.suffix.lower()
        if suffix not in {".uxml", ".uss", ".cs"}:
            continue
        stem = path.stem
        if suffix == ".uxml" and not PASCAL_CASE.match(stem):
            warnings.append(f"{path}: UXML screen/template file should usually use PascalCase")
        elif suffix == ".uss" and not (PASCAL_CASE.match(stem) or KEBAB_CASE.match(stem)):
            warnings.append(f"{path}: USS file should use PascalCase screen name or kebab-case shared style")
        elif suffix == ".cs" and "UI" in path.parts and not PASCAL_CASE.match(stem):
            warnings.append(f"{path}: UI C# file should use PascalCase")
    return warnings


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate UI Toolkit naming conventions.")
    parser.add_argument("project_path", nargs="?", default=".", help="Unity project or Assets path")
    args = parser.parse_args()

    root = Path(args.project_path).resolve()
    warnings = validate(root)
    if not warnings:
        print(f"No UI naming warnings under {root}")
        return 0
    print("UI naming warnings:")
    for warning in warnings:
        print(f"- {warning}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
