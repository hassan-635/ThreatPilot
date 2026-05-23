from typing import List, Optional
from models.schemas import LogEvent, Alert

class BaseDetectionRule:
    rule_name: str = "BaseRule"
    
    def __init__(self):
        pass
        
    def evaluate(self, logs: List[LogEvent]) -> List[Alert]:
        """
        Evaluate a batch of logs and return a list of generated alerts.
        Must be implemented by subclasses.
        """
        raise NotImplementedError("Subclasses must implement evaluate()")
