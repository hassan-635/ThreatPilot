import asyncio
import httpx
from datetime import datetime, timezone
import uuid

BACKEND_URL = "http://localhost:5229/api/Ingest"

async def run_impossible_travel_test():
    print(f"Sending test logs for Impossible Travel to {BACKEND_URL}...")
    
    # Needs two logins from same user but far away locations
    logs = [
        {
            "event_id": str(uuid.uuid4()),
            "timestamp": "2026-05-26T10:00:00Z",
            "source_ip": "8.8.8.8", # Mock IP for US
            "user_id": "global_admin",
            "event_type": "login_success",
            "target_resource": "vpn",
            "status": "success",
            "metadata": {"location": "New York, USA", "lat": 40.7128, "lon": -74.0060}
        },
        {
            "event_id": str(uuid.uuid4()),
            "timestamp": "2026-05-26T10:30:00Z", # 30 mins later
            "source_ip": "1.1.1.1", # Mock IP for Australia
            "user_id": "global_admin",
            "event_type": "login_success",
            "target_resource": "vpn",
            "status": "success",
            "metadata": {"location": "Sydney, Australia", "lat": -33.8688, "lon": 151.2093}
        }
    ]

    payload = {"logs": logs}
    
    async with httpx.AsyncClient() as client:
        try:
            response = await client.post(BACKEND_URL, json=payload, timeout=30.0)
            print(f"Status Code: {response.status_code}")
            print(f"Response: {response.text}")
        except Exception as e:
            print(f"Error sending logs: {e}")

if __name__ == "__main__":
    asyncio.run(run_impossible_travel_test())
