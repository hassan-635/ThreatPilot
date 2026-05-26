from fastapi import APIRouter, Depends, HTTPException
from typing import List
from models.schemas import LogBatch, Alert, AIReport, IncidentAnalysisRequest
from services.detection_service import detection_service
from services.ai_service import ai_service
from utils.auth import verify_token

router = APIRouter()

@router.post("/ingest", response_model=List[Alert], summary="Ingest logs for detection")
async def ingest_logs(batch: LogBatch):
    """
    Receives a batch of normalized security logs from the ASP.NET backend.
    Evaluates them against deterministic detection rules.
    Returns any generated alerts.
    """
    try:
        alerts = detection_service.analyze_logs(batch.logs)
        return alerts
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@router.post("/analyze-incident", response_model=AIReport, summary="Generate AI summary for an alert")
async def analyze_incident(request: IncidentAnalysisRequest):
    """
    Receives an Alert object from the ASP.NET backend.
    Uses the Grok API to generate an executive summary, evaluate severity, and recommend remediation.
    """
    try:
        ai_report = await ai_service.analyze_incident(request.alert)
        return ai_report
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
