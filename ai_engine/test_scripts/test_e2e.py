import asyncio
import httpx
from datetime import datetime, timezone
import uuid

BACKEND_URL = "http://localhost:5229/api/Ingest"

async def run_e2e_test():
    print(f"Sending test logs to {BACKEND_URL}...")
    
    # Generate 15 failed logins to trigger Brute Force
    logs = []
    for i in range(15):
        logs.append({
            "event_id": str(uuid.uuid4()),
            "timestamp": datetime.now(timezone.utc).isoformat(),
            "source_ip": "10.0.5.55",
            "user_id": "testuser",
            "event_type": "login_failed",
            "target_resource": "ssh",
            "status": "failed",
            "metadata": {"reason": "invalid_password"}
        })

    payload = {"logs": logs}
    
    async with httpx.AsyncClient() as client:
        try:
            response = await client.post(BACKEND_URL, json=payload, timeout=30.0)
            print(f"Status Code: {response.status_code}")
            print(f"Response: {response.text}")
            
            if response.status_code == 200:
                print("Successfully sent logs to backend. Check the WPF UI to see if the new alert appeared!")
        except Exception as e:
            print(f"Error sending logs: {e}")

if __name__ == "__main__":
    asyncio.run(run_e2e_test())
