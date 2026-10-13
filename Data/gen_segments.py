"""
Generate random segment input files for BentleyOttmannModule.

Usage:
  python gen_segments.py <N> [output_file]

Output format:
  N
  x1 y1 x2 y2
  ...
"""
import random
import math
import sys

def gen(n: int, width: float = 1000.0, height: float = 1000.0, seed: int = 42) -> list:
    rng = random.Random(seed)
    max_len = min(width, height) * 0.08
    segs = []
    for _ in range(n):
        x1 = rng.uniform(5, width - 5)
        y1 = rng.uniform(5, height - 5)
        angle = rng.uniform(0, 2 * math.pi)
        length = rng.uniform(max_len * 0.2, max_len)
        x2 = max(5.0, min(width - 5, x1 + length * math.cos(angle)))
        y2 = max(5.0, min(height - 5, y1 + length * math.sin(angle)))
        segs.append((x1, y1, x2, y2))
    return segs


def write(segs: list, path: str):
    with open(path, "w") as f:
        f.write(f"{len(segs)}\n")
        for x1, y1, x2, y2 in segs:
            f.write(f"{x1:.6f} {y1:.6f} {x2:.6f} {y2:.6f}\n")
    print(f"Written {len(segs)} segments to '{path}'.")


if __name__ == "__main__":
    n    = int(sys.argv[1]) if len(sys.argv) > 1 else 1000
    out  = sys.argv[2]      if len(sys.argv) > 2 else f"segments_{n}.txt"
    write(gen(n), out)
