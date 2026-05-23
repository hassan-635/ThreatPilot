from typing import List
from models.schemas import LogEvent, Alert
from detection.rules import BruteForceRule, CredentialStuffingRule, ImpossibleTravelRule, PortScanningRule

class DetectionService:
    def __init__(self):
        # Initialize all active rules
        self.rules = [
            BruteForceRule(threshold=10),
            CredentialStuffingRule(user_threshold=5),
            ImpossibleTravelRule(speed_threshold_kmh=1000),
            PortScanningRule(port_threshold=20)
        ]

    def analyze_logs(self, logs: List[LogEvent]) -> List[Alert]:
        all_alerts = []
        for rule in self.rules:
            # Each rule evaluates the batch and returns any alerts
            alerts = rule.evaluate(logs)
            all_alerts.extend(alerts)
            
        return all_alerts

# Singleton instance
detection_service = DetectionService()
