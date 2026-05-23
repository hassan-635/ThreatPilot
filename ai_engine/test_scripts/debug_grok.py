import asyncio
import httpx
import json
import os
from pathlib import Path
from dotenv import load_dotenv

# Always load .env from ai_engine root, regardless of where script is run from
env_path = Path(__file__).parent.parent / ".env"
load_dotenv(dotenv_path=env_path)

api_key = os.getenv("GROK_API_KEY")
api_url = os.getenv("GROK_API_URL", "https://api.x.ai/v1/chat/completions")

print(f"API Key prefix : {api_key[:15] if api_key else 'NOT SET'}")
print(f"API URL        : {api_url}")

async def test_grok():
    headers = {
        "Authorization": f"Bearer {api_key}",
        "Content-Type": "application/json"
    }
    payload = {
        "model": "grok-3-mini",
        "messages": [
            {
                "role": "system",
                "content": "You are a cybersecurity analyst. Always respond with valid JSON only."
            },
            {
                "role": "user",
                "content": 'Analyze: Brute Force Attack HIGH severity from 203.0.113.45. Return JSON with keys: summary, severity_reason, recommended_actions (list).'
            }
        ],
        "temperature": 0.1
    }

    print("\nSending request to Grok API...")
    async with httpx.AsyncClient() as client:
        try:
            response = await client.post(api_url, headers=headers, json=payload, timeout=60.0)
            print(f"HTTP Status    : {response.status_code}")
            if response.status_code == 200:
                data = response.json()
                content = data["choices"][0]["message"]["content"]
                print(f"\nGrok Raw Response:\n{content}")
            else:
                print(f"Error Response : {response.text[:1000]}")
        except Exception as e:
            print(f"Exception      : {e}")

asyncio.run(test_grok())
