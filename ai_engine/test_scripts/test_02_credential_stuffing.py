"""
TEST 2 — Credential Stuffing Detection
Scenario: Single IP tries to authenticate against 8 different user accounts
          rapidly — classic credential stuffing from a leaked password dump.
Expected: Rule fires with severity CRITICAL. AI identifies stuffing vs brute force.
"""
import asyncio
import uuid
import sys
import os
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

import httpx
from datetime import datetime, timedelta
from test_scripts.test_utils import (
    generate_token, print_header, print_alert,
    print_ai_report, print_pass, print_fail,
    post_ingest, post_analyze
)

async def test_credential_stuffing():
    print_header("TEST 2 — CREDENTIAL STUFFING DETECTION")

    token = generate_token()
    headers = {"Authorization": f"Bearer {token}"}

    # Attacker IP tries 8 different user accounts (stuffing from leaked DB)
    target_users = [
        "john.doe", "alice.smith", "bob.jones", "carol.white",
        "dave.brown", "emma.wilson", "frank.taylor", "grace.martin"
    ]

    base_time = datetime.utcnow()
    logs = []
    for i, user in enumerate(target_users):
        logs.append({
            "event_id": str(uuid.uuid4()),
            "timestamp": (base_time + timedelta(seconds=i * 3)).isoformat(),
            "source_ip": "198.51.100.77",
            "user_id": user,
            "event_type": "login_failed",
            "target_resource": "web-portal:443",
            "status": "failed",
            "metadata": {
                "user_agent": "python-requests/2.28.0",
                "country": "CN",
                "password_attempt": "P@ssw0rd123!"
            }
        })

    print(f"\n  Injecting {len(logs)} login attempts from single IP: 198.51.100.77")
    print(f"  Targeting {len(target_users)} different user accounts")
    print(f"  Users targeted: {', '.join(target_users)}")

    async with httpx.AsyncClient() as client:
        alerts = await post_ingest(client, headers, logs)
        if alerts is None:
            return

        if not alerts:
            print_fail("Credential Stuffing Detection", "No alert generated. Rule did not fire.")
            return

        stuffing_alert = next((a for a in alerts if "Stuffing" in a["rule_name"] or "Brute" in a["rule_name"]), None)
        if not stuffing_alert:
            print_fail("Credential Stuffing", "Expected alert not found in response.")
            return

        print_pass("Credential Stuffing Alert generated successfully!")
        print_alert(stuffing_alert)

        print(f"\n  Sending alert to Grok AI for analysis...")
        ai_report = await post_analyze(client, headers, stuffing_alert)
        if ai_report:
            print_pass("AI Incident Analysis complete!")
            print_ai_report(ai_report)

if __name__ == "__main__":
    asyncio.run(test_credential_stuffing())
