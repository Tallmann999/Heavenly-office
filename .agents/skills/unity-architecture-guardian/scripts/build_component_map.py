#!/usr/bin/env python3
"""Build a read-only component map from Unity-like asset files."""

from __future__ import annotations

import argparse
import json
import re
from pathlib import Path


ASSET_EXTENSIONS = {".prefab", ".unity", ".asset", ".uxml"}
SCRIPT_GUID = re.compile(r"m_Script:\s*\{[^}]*guid:\s*([a-fA-F0-9]+)")
MONO_BEHAVIOUR = re.compile(r"--- !u!114 &(\d+)")
GAME_OBJECT = re.compile(r"m_Name:\s*(.+)")


def inspect_file(path: Path) -> dict:
    text = path.read_text(encoding="utf-8", errors="ignore")
    names = [match.group(1).strip() for match in GAME_OBJECT.finditer(text)]
    script_guids = sorted(set(SCRIPT_GUID.findall(text)))
    mono_count = len(MONO_BEHAVIOUR.findall(text))
    return {
        "path": str(path),
        "type": path.suffix.lower().lstrip("."),
        "names": names[:20],
        "mono_behaviour_count": mono_count,
        "script_guids": script_guids,
    }


def build(root: Path) -> list[dict]:
    rows: list[dict] = []
    for path in sorted(root.rglob("*")):
        if path.is_file() and path.suffix.lower() in ASSET_EXTENSIONS:
            rows.append(inspect_file(path))
    return rows


def print_markdown(rows: list[dict], root: Path) -> None:
    print(f"# Component map for {root}")
    if not rows:
        print("\nNo Unity prefab, scene, asset, or UXML files found.")
        return
    for row in rows:
        print(f"\n## {row['path']}")
        print(f"- type: {row['type']}")
        print(f"- mono behaviours: {row['mono_behaviour_count']}")
        if row["names"]:
            print(f"- object names: {', '.join(row['names'])}")
        if row["script_guids"]:
            print(f"- script guids: {', '.join(row['script_guids'])}")


def main() -> int:
    parser = argparse.ArgumentParser(description="Build a Unity component map.")
    parser.add_argument("project_path", nargs="?", default=".", help="Unity project or Assets path")
    parser.add_argument("--format", choices=["markdown", "json"], default="markdown")
    args = parser.parse_args()

    root = Path(args.project_path).resolve()
    rows = build(root)
    if args.format == "json":
        print(json.dumps({"root": str(root), "files": rows}, indent=2))
    else:
        print_markdown(rows, root)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
