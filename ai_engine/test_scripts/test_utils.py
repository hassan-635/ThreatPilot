"""
Shared test utilities: JWT token generation, colored output, and HTTP helper.
"""
import jwt
import httpx
import asyncio
from datetime import datetime, timedelta
from dotenv import load_dotenv
import os

load_dotenv(dotenv_path="../.env")

JWT_SECRET_KEY = os.getenv("JWT_SECRET_KEY", "your_super_secret_jwt_key_here")
API_BASE_URL = "http://127.0.0.1:8000/api/v1"

# ANSI Color Codes for rich terminal output
GREEN  = "\033[92m"
RED    = "\033[91m"
YELLOW = "\033[93m"
CYAN   = "\033[96m"
BOLD   = "\033[1m"
RESET  = "\033[0m"

def generate_token() -> str:
    payload = {
        "sub": "test-suite",
        "exp": datetime.utcnow() + timedelta(hours=2)
    }
    return jwt.encode(payload, JWT_SECRET_KEY, algorithm="HS256")

def print_header(title: str):
    print(f"\n{BOLD}{CYAN}{'='*60}{RESET}")
    print(f"{BOLD}{CYAN}  {title}{RESET}")
    print(f"{BOLD}{CYAN}{'='*60}{RESET}")

def print_alert(alert: dict):
    severity = alert.get("severity", "UNKNOWN")
    color = RED if severity in ("CRITICAL", "HIGH") else YELLOW
    print(f"\n  {BOLD}[ALERT DETECTED]{RESET}")
    print(f"  Rule      : {color}{BOLD}{alert['rule_name']}{RESET}")
    print(f"  Severity  : {color}{BOLD}{severity}{RESET}")
    print(f"  Detail    : {alert['description']}")
    print(f"  Alert ID  : {alert['alert_id']}")

def print_ai_report(report: dict):
    print(f"\n  {BOLD}[AI ANALYSIS REPORT]{RESET}")
    print(f"  {BOLD}Summary:{RESET}")
    print(f"    {report['summary']}")
    print(f"\n  {BOLD}Severity Reason:{RESET}")
    print(f"    {report['severity_reason']}")
    print(f"\n  {BOLD}Recommended Actions:{RESET}")
    for i, action in enumerate(report['recommended_actions'], 1):
        print(f"    {i}. {action}")

def print_pass(label: str):
    print(f"\n  {GREEN}{BOLD}[PASS]{RESET} {label}")

def print_fail(label: str, error: str):
    print(f"\n  {RED}{BOLD}[FAIL]{RESET} {label}")
    print(f"  Error: {error}")

async def post_ingest(client: httpx.AsyncClient, headers: dict, logs: list) -> dict | None:
    try:
        response = await client.post(f"{API_BASE_URL}/ingest", json={"logs": logs}, headers=headers, timeout=30.0)
        response.raise_for_status()
        return response.json()
    except Exception as e:
        print_fail("POST /ingest", str(e))
        return None

async def post_analyze(client: httpx.AsyncClient, headers: dict, alert: dict) -> dict | None:
    try:
        response = await client.post(f"{API_BASE_URL}/analyze-incident", json={"alert": alert}, headers=headers, timeout=120.0)
        response.raise_for_status()
        return response.json()
    except Exception as e:
        print_fail("POST /analyze-incident", str(e))
        return None
