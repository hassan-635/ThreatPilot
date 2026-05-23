"""
TEST 4 — Port Scanning Detection
Scenario: Attacker performs network reconnaissance on an internal server,
          rapidly probing 25 distinct ports in quick succession from one IP.
Expected: Rule fires with severity MEDIUM. AI explains recon phase of kill chain.
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

async def test_port_scanning():
    print_header("TEST 4 — PORT SCANNING / RECONNAISSANCE DETECTION")

    token = generate_token()
    headers = {"Authorization": f"Bearer {token}"}

    # Classic recon ports an attacker would probe
    recon_ports = [
        21, 22, 23, 25, 53, 80, 110, 135, 139, 143,
        443, 445, 993, 995, 1433, 1521, 3306, 3389,
        5432, 5900, 6379, 8080, 8443, 9200, 27017
    ]

    base_time = datetime.utcnow()
    logs = []
    for i, port in enumerate(recon_ports):
        logs.append({
            "event_id": str(uuid.uuid4()),
            "timestamp": (base_time + timedelta(milliseconds=i * 200)).isoformat(),
            "source_ip": "45.155.205.233",
            "user_id": None,
            "event_type": "port_connection",
            "target_resource": str(port),
            "status": "rejected",
            "metadata": {
                "target_host": "192.168.10.15",
                "protocol": "TCP",
                "country": "RO",
                "tool_signature": "nmap/7.93"
            }
        })

    print(f"\n  Source IP : 45.155.205.233 (Romania — known scan range)")
    print(f"  Target    : 192.168.10.15 (Internal Application Server)")
    print(f"  Ports probed: {len(recon_ports)} ports in {len(recon_ports) * 0.2:.1f} seconds")
    print(f"  Ports: {recon_ports}")

    async with httpx.AsyncClient() as client:
        alerts = await post_ingest(client, headers, logs)
        if alerts is None:
            return

        if not alerts:
            print_fail("Port Scanning Detection", "No alert generated. Rule did not fire.")
            return

        scan_alert = next((a for a in alerts if "Scan" in a["rule_name"] or "Port" in a["rule_name"]), None)
        if not scan_alert:
            print_fail("Port Scanning", "Expected alert not found in response.")
            return

        print_pass("Port Scanning Alert generated successfully!")
        print_alert(scan_alert)

        print(f"\n  Sending alert to Grok AI for analysis...")
        ai_report = await post_analyze(client, headers, scan_alert)
        if ai_report:
            print_pass("AI Incident Analysis complete!")
            print_ai_report(ai_report)

if __name__ == "__main__":
    asyncio.run(test_port_scanning())
