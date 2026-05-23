import httpx
import jwt
import asyncio
from datetime import datetime, timedelta
import uuid

# Configuration matching the .env.example
JWT_SECRET_KEY = "your_super_secret_jwt_key_here" # Make sure this matches your .env!
API_BASE_URL = "http://127.0.0.1:8000/api/v1"

def generate_mock_token():
    """Generate a valid JWT token for testing."""
    payload = {
        "sub": "test-backend",
        "exp": datetime.utcnow() + timedelta(hours=1)
    }
    return jwt.encode(payload, JWT_SECRET_KEY, algorithm="HS256")

async def test_brute_force_detection():
    print("\n--- Testing Brute Force Detection ---")
    token = generate_mock_token()
    headers = {"Authorization": f"Bearer {token}"}
    
    # Generate 15 failed logins from the same IP (threshold is 10)
    logs = []
    base_time = datetime.utcnow()
    for i in range(15):
        logs.append({
            "event_id": str(uuid.uuid4()),
            "timestamp": (base_time + timedelta(seconds=i)).isoformat(),
            "source_ip": "203.0.113.45",
            "user_id": "admin",
            "event_type": "login_failed",
            "target_resource": "ssh",
            "status": "failed",
            "metadata": {}
        })
        
    payload = {"logs": logs}
    
    async with httpx.AsyncClient() as client:
        try:
            print("Sending 15 failed login logs to /ingest...")
            response = await client.post(f"{API_BASE_URL}/ingest", json=payload, headers=headers)
            response.raise_for_status()
            
            alerts = response.json()
            print(f"Detected {len(alerts)} alerts!")
            for alert in alerts:
                print(f" -> Alert: {alert['rule_name']} (Severity: {alert['severity']})")
                print(f" -> Desc:  {alert['description']}")
                
                # Now test AI Analysis on the first alert
                print("\n--- Testing AI Incident Analysis ---")
                print("Sending alert to /analyze-incident...")
                ai_payload = {"alert": alert}
                ai_response = await client.post(f"{API_BASE_URL}/analyze-incident", json=ai_payload, headers=headers, timeout=30.0)
                ai_response.raise_for_status()
                
                ai_report = ai_response.json()
                print("\n[AI Report Generated]")
                print(f"Summary: {ai_report['summary']}")
                print(f"Severity Reason: {ai_report['severity_reason']}")
                print(f"Recommended Actions: {ai_report['recommended_actions']}")
                
        except httpx.HTTPError as e:
            print(f"HTTP Request failed: {e}")

if __name__ == "__main__":
    asyncio.run(test_brute_force_detection())
