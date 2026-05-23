"""
TEST 5 — Master Suite Runner
Runs ALL attack test scenarios sequentially with clear pass/fail summary.
This is the file to run during a demo to showcase the complete engine.
"""
import asyncio
import sys
import os
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

from test_scripts.test_utils import GREEN, RED, BOLD, CYAN, RESET
from test_scripts.test_01_brute_force import test_brute_force
from test_scripts.test_02_credential_stuffing import test_credential_stuffing
from test_scripts.test_03_impossible_travel import test_impossible_travel
from test_scripts.test_04_port_scanning import test_port_scanning

async def run_all_tests():
    print(f"\n{BOLD}{CYAN}{'#'*60}{RESET}")
    print(f"{BOLD}{CYAN}#   THREATPILOT — FULL ATTACK DETECTION TEST SUITE          #{RESET}")
    print(f"{BOLD}{CYAN}#   Rules First, AI Second — End-to-End Validation          #{RESET}")
    print(f"{BOLD}{CYAN}{'#'*60}{RESET}")

    tests = [
        ("Brute Force Attack",       test_brute_force),
        ("Credential Stuffing",      test_credential_stuffing),
        ("Impossible Travel",        test_impossible_travel),
        ("Port Scanning / Recon",    test_port_scanning),
    ]

    results = []
    for name, fn in tests:
        try:
            await fn()
            results.append((name, True, ""))
        except Exception as e:
            results.append((name, False, str(e)))

    print(f"\n\n{BOLD}{CYAN}{'='*60}{RESET}")
    print(f"{BOLD}{CYAN}  FINAL TEST SUMMARY{RESET}")
    print(f"{BOLD}{CYAN}{'='*60}{RESET}")
    passed = 0
    for name, ok, err in results:
        if ok:
            print(f"  {GREEN}{BOLD}[PASS]{RESET}  {name}")
            passed += 1
        else:
            print(f"  {RED}{BOLD}[FAIL]{RESET}  {name}  —  {err}")

    total = len(results)
    color = GREEN if passed == total else RED
    print(f"\n  Result: {color}{BOLD}{passed}/{total} tests passed{RESET}")
    print(f"{BOLD}{CYAN}{'='*60}{RESET}\n")

if __name__ == "__main__":
    asyncio.run(run_all_tests())
