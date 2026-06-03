#!/usr/bin/env python3
"""Scan Unity .asmdef files without modifying the project."""

from __future__ import annotations

import argparse
import json
from pathlib import Path


def load_asmdef(path: Path) -> dict:
    try:
        return json.loads(path.read_text(encoding="utf-8-sig"))
    except json.JSONDecodeError as exc:
        return {"name": path.stem, "_error": f"invalid json: {exc}"}


def classify(path: Path) -> str:
    parts = {part.lower() for part in path.parts}
    if "editor" in parts:
        return "editor"
    if "tests" in parts or "editmode" in parts or "playmode" in parts:
        return "tests"
    return "runtime"


def scan(project_path: Path) -> list[dict]:
    rows: list[dict] = []
    for asmdef in sorted(project_path.rglob("*.asmdef")):
        data = load_asmdef(asmdef)
        name = data.get("name") or asmdef.stem
        references = data.get("references") or []
        kind = classify(asmdef)
        warnings: list[str] = []

        if data.get("_error"):
            warnings.append(data["_error"])
        if kind == "editor" and ".editor" not in name.lower():
            warnings.append("editor path assembly name should usually end with .Editor")
        if kind == "runtime" and any(".editor" in str(ref).lower() for ref in references):
            warnings.append("runtime assembly appears to reference an editor assembly")
        if kind == "tests" and "test" not in name.lower():
            warnings.append("test path assembly name should include Tests")

        rows.append(
            {
                "name": name,
                "path": str(asmdef),
                "kind": kind,
                "references": references,
                "warnings": warnings,
            }
        )
    return rows


def main() -> int:
    parser = argparse.ArgumentParser(description="Scan Unity asmdef files.")
    parser.add_argument("project_path", nargs="?", default=".", help="Unity project or Assets path")
    args = parser.parse_args()

    project_path = Path(args.project_path).resolve()
    rows = scan(project_path)

    if not rows:
        print(f"No .asmdef files found under {project_path}")
        return 0

    for row in rows:
        print(f"- {row['name']} [{row['kind']}]")
        print(f"  path: {row['path']}")
        if row["references"]:
            print(f"  references: {', '.join(map(str, row['references']))}")
        for warning in row["warnings"]:
            print(f"  warning: {warning}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
