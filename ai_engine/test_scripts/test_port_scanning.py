import asyncio
import httpx
from datetime import datetime, timezone
import uuid

BACKEND_URL = "http://localhost:5229/api/Ingest"

async def run_port_scanning_test():
    print(f"Sending test logs for Port Scanning to {BACKEND_URL}...")
    
    logs = []
    # Generate 25 connections to different ports from same IP
    for port in range(1, 26):
        logs.append({
            "event_id": str(uuid.uuid4()),
            "timestamp": datetime.now(timezone.utc).isoformat(),
            "source_ip": "192.168.1.100",
            "user_id": None,
            "event_type": "port_connection",
            "target_resource": f"10.0.0.5:{port}",
            "status": "blocked",
            "metadata": {"protocol": "tcp"}
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
    asyncio.run(run_port_scanning_test())
