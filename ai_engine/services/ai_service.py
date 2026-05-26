import os
import json
import re
import httpx
from models.schemas import Alert, AIReport
from prompts.templates import INCIDENT_ANALYSIS_SYSTEM_PROMPT, build_user_prompt
from fastapi import HTTPException

class AIService:
    def __init__(self):
        self.api_key = os.getenv("GROQ_API_KEY")
        self.api_url = os.getenv("GROQ_API_URL", "https://api.groq.com/openai/v1/chat/completions")
        # Free Groq models: llama-3.3-70b-versatile, gemma2-9b-it, mixtral-8x7b-32768
        self.model = os.getenv("GROQ_MODEL", "llama-3.3-70b-versatile")

    def _extract_json(self, text: str) -> dict:
        """Robustly extract JSON from model response even if wrapped in markdown."""
        # Try direct parse first
        try:
            return json.loads(text)
        except json.JSONDecodeError:
            pass
        # Try extracting from ```json ... ``` block
        match = re.search(r"```(?:json)?\s*(\{.*?\})\s*```", text, re.DOTALL)
        if match:
            return json.loads(match.group(1))
        # Try finding first { ... } block
        match = re.search(r"\{.*\}", text, re.DOTALL)
        if match:
            return json.loads(match.group(0))
        raise ValueError("No valid JSON found in AI response")

    async def analyze_incident(self, alert: Alert) -> AIReport:
        if not self.api_key or self.api_key == "gsk_your-groq-api-key-here":
            # Mock response for development without a real key
            return AIReport(
                summary=f"[Mock] {alert.rule_name} detected from {alert.source_ip or 'unknown IP'}.",
                severity_reason=f"Severity set to {alert.severity} by deterministic rule engine.",
                recommended_actions=["Investigate source IP", "Reset affected user credentials", "Check firewall logs"]
            )

        headers = {
            "Authorization": f"Bearer {self.api_key}",
            "Content-Type": "application/json"
        }

        user_prompt = build_user_prompt(alert.model_dump(mode='json'))

        payload = {
            "model": self.model,
            "messages": [
                {"role": "system", "content": INCIDENT_ANALYSIS_SYSTEM_PROMPT},
                {"role": "user", "content": user_prompt}
            ],
            "temperature": 0.85,
            "response_format": {"type": "json_object"}  # Groq supports JSON mode!
        }

        async with httpx.AsyncClient() as client:
            try:
                response = await client.post(
                    self.api_url, headers=headers, json=payload, timeout=60.0
                )
                response.raise_for_status()
                response_data = response.json()

                content = response_data['choices'][0]['message']['content']
                parsed_content = self._extract_json(content)

                return AIReport(**parsed_content)

            except httpx.HTTPStatusError as e:
                raise HTTPException(
                    status_code=500,
                    detail=f"Groq API Error {e.response.status_code}: {e.response.text}"
                )
            except (KeyError, ValueError) as e:
                raise HTTPException(status_code=500, detail=f"AI response parse error: {str(e)}")
            except Exception as e:
                raise HTTPException(status_code=500, detail=f"AI Service Error: {str(e)}")

ai_service = AIService()
