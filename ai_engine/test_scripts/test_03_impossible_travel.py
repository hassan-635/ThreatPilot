"""
TEST 3 — Impossible Travel Detection
Scenario: User 'ceo.account' logs in from Karachi, Pakistan (24.8N, 67.0E)
          and then 4 minutes later logs in from London, UK (51.5N, -0.1W).
          Distance ~6,000 km in 4 minutes = physically impossible.
Expected: Rule fires with severity HIGH. AI explains session hijacking risk.
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

async def test_impossible_travel():
    print_header("TEST 3 — IMPOSSIBLE TRAVEL DETECTION")

    token = generate_token()
    headers = {"Authorization": f"Bearer {token}"}

    base_time = datetime.utcnow()

    logs = [
        # Login 1: Karachi, Pakistan at T+0
        {
            "event_id": str(uuid.uuid4()),
            "timestamp": base_time.isoformat(),
            "source_ip": "39.35.16.100",
            "user_id": "ceo.account",
            "event_type": "login_success",
            "target_resource": "corporate-vpn",
            "status": "success",
            "metadata": {
                "lat": 24.8607,
                "lon": 67.0011,
                "city": "Karachi",
                "country": "PK",
                "device": "MacBook Pro"
            }
        },
        # Login 2: London, UK just 4 minutes later — physically impossible!
        {
            "event_id": str(uuid.uuid4()),
            "timestamp": (base_time + timedelta(minutes=4)).isoformat(),
            "source_ip": "82.132.200.55",
            "user_id": "ceo.account",
            "event_type": "login_success",
            "target_resource": "corporate-vpn",
            "status": "success",
            "metadata": {
                "lat": 51.5074,
                "lon": -0.1278,
                "city": "London",
                "country": "GB",
                "device": "Windows 11 PC"
            }
        }
    ]

    print(f"\n  Login 1: Karachi, PK  (24.86°N, 67.00°E) at T+0 min")
    print(f"  Login 2: London,  UK  (51.50°N, -0.12°W) at T+4 min")
    print(f"  Distance: ~6,000 km | Required speed: ~90,000 km/h (Impossible!)")

    async with httpx.AsyncClient() as client:
        alerts = await post_ingest(client, headers, logs)
        if alerts is None:
            return

        if not alerts:
            print_fail("Impossible Travel Detection", "No alert generated. Rule did not fire.")
            return

        travel_alert = next((a for a in alerts if "Travel" in a["rule_name"]), None)
        if not travel_alert:
            print_fail("Impossible Travel", "Expected alert not found in response.")
            return

        print_pass("Impossible Travel Alert generated successfully!")
        print_alert(travel_alert)

        print(f"\n  Sending alert to Grok AI for analysis...")
        ai_report = await post_analyze(client, headers, travel_alert)
        if ai_report:
            print_pass("AI Incident Analysis complete!")
            print_ai_report(ai_report)

if __name__ == "__main__":
    asyncio.run(test_impossible_travel())
