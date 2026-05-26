import asyncio
import httpx
from datetime import datetime, timezone
import uuid

BACKEND_URL = "http://localhost:5229/api/Ingest"

async def run_credential_stuffing_test():
    print(f"Sending test logs for Credential Stuffing to {BACKEND_URL}...")
    
    logs = []
    # Generate logins from same IP to 6 different usernames
    for i in range(1, 7):
        logs.append({
            "event_id": str(uuid.uuid4()),
            "timestamp": datetime.now(timezone.utc).isoformat(),
            "source_ip": "45.33.22.11",
            "user_id": f"admin_user_{i}",
            "event_type": "login_failed",
            "target_resource": "web_portal",
            "status": "failed",
            "metadata": {"reason": "invalid_credentials"}
        })

    payload = {"logs": logs}
    
    async with httpx.AsyncClient() as client:
        try:
            response = await client.post(BACKEND_URL, json=payload, timeout=30.0)
            print(f"Status Code: {response.status_code}")
            print(f"Response: {response.text}")
        except Exception as e:
            print(f"Error sending logs: {e}")

if __name__ == "__main__":
    asyncio.run(run_credential_stuffing_test())
