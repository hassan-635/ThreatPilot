from pydantic import BaseModel, Field
from typing import List, Dict, Any, Optional
from datetime import datetime
from enum import Enum

class LogEventType(str, Enum):
    LOGIN_FAILED = "login_failed"
    LOGIN_SUCCESS = "login_success"
    PRIVILEGE_ESCALATION = "privilege_escalation"
    PORT_CONNECTION = "port_connection"
    RESOURCE_ACCESS = "resource_access"

class LogEvent(BaseModel):
    event_id: str = Field(..., description="Unique identifier for the log event")
    timestamp: datetime = Field(default_factory=datetime.utcnow)
    source_ip: str = Field(..., description="Source IP address of the event")
    user_id: Optional[str] = Field(None, description="User associated with the event")
    event_type: LogEventType = Field(..., description="Categorized type of the event")
    target_resource: Optional[str] = Field(None, description="Target system, port, or resource")
    status: str = Field(..., description="Outcome of the event, e.g., 'failed' or 'success'")
    metadata: Dict[str, Any] = Field(default_factory=dict, description="Additional contextual data")

class LogBatch(BaseModel):
    logs: List[LogEvent]

class AlertSeverity(str, Enum):
    LOW = "LOW"
    MEDIUM = "MEDIUM"
    HIGH = "HIGH"
    CRITICAL = "CRITICAL"

class Alert(BaseModel):
    alert_id: str
    rule_name: str
    severity: AlertSeverity
    timestamp: datetime = Field(default_factory=datetime.utcnow)
    description: str
    source_ip: Optional[str] = None
    user_id: Optional[str] = None
    triggering_logs: List[LogEvent]

class AIReport(BaseModel):
    summary: str = Field(..., description="Executive summary of the incident")
    severity_reason: str = Field(..., description="Explanation of why this severity was assigned")
    recommended_actions: List[str] = Field(..., description="Actionable remediation steps")

class IncidentAnalysisRequest(BaseModel):
    alert: Alert
