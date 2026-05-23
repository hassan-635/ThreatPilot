"""
TEST 1 — Brute Force Attack Detection
Scenario: Attacker hammers SSH login of user 'admin' from a single IP
          crossing the threshold of 10 failed attempts within seconds.
Expected: Rule fires with severity HIGH. AI explains attack & recommends IP block.
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

async def test_brute_force():
    print_header("TEST 1 — BRUTE FORCE ATTACK DETECTION")

    token = generate_token()
    headers = {"Authorization": f"Bearer {token}"}

    # Generate 15 failed login events from same IP targeting same user
    base_time = datetime.utcnow()
    logs = []
    for i in range(15):
        logs.append({
            "event_id": str(uuid.uuid4()),
            "timestamp": (base_time + timedelta(seconds=i * 2)).isoformat(),
            "source_ip": "203.0.113.45",
            "user_id": "admin",
            "event_type": "login_failed",
            "target_resource": "ssh:22",
            "status": "failed",
            "metadata": {"user_agent": "PuTTY/0.76", "country": "RU"}
        })

    print(f"\n  Injecting {len(logs)} failed SSH login logs from IP: 203.0.113.45")
    print(f"  Target user: admin | Threshold: 10 attempts")

    async with httpx.AsyncClient() as client:
        alerts = await post_ingest(client, headers, logs)

        if alerts is None:
            return

        if not alerts:
            print_fail("Brute Force Detection", "No alert was generated. Rule did not fire.")
            return

        brute_alert = next((a for a in alerts if "Brute Force" in a["rule_name"]), None)
        if not brute_alert:
            print_fail("Brute Force Detection", "Expected 'Brute Force Attack' alert not found.")
            return

        print_pass("Brute Force Alert generated successfully!")
        print_alert(brute_alert)

        print(f"\n  Sending alert to Grok AI for analysis...")
        ai_report = await post_analyze(client, headers, brute_alert)

        if ai_report:
            print_pass("AI Incident Analysis complete!")
            print_ai_report(ai_report)

if __name__ == "__main__":
    asyncio.run(test_brute_force())
