from typing import List, Dict
from collections import defaultdict
from models.schemas import LogEvent, Alert, AlertSeverity, LogEventType
from detection.base import BaseDetectionRule
import uuid
from geopy.distance import geodesic

class BruteForceRule(BaseDetectionRule):
    rule_name = "Brute Force Attack"
    
    def __init__(self, threshold: int = 10, time_window_seconds: int = 300):
        self.threshold = threshold
        self.time_window_seconds = time_window_seconds

    def evaluate(self, logs: List[LogEvent]) -> List[Alert]:
        alerts = []
        failed_logins = [log for log in logs if log.event_type == LogEventType.LOGIN_FAILED]
        
        # Group by source IP
        ip_groups = defaultdict(list)
        for log in failed_logins:
            ip_groups[log.source_ip].append(log)
            
        for ip, group_logs in ip_groups.items():
            if len(group_logs) >= self.threshold:
                alerts.append(Alert(
                    alert_id=str(uuid.uuid4()),
                    rule_name=self.rule_name,
                    severity=AlertSeverity.HIGH,
                    description=f"Detected {len(group_logs)} failed login attempts from IP {ip}.",
                    source_ip=ip,
                    triggering_logs=group_logs
                ))
        return alerts

class CredentialStuffingRule(BaseDetectionRule):
    rule_name = "Credential Stuffing"
    
    def __init__(self, user_threshold: int = 5):
        self.user_threshold = user_threshold

    def evaluate(self, logs: List[LogEvent]) -> List[Alert]:
        alerts = []
        failed_logins = [log for log in logs if log.event_type == LogEventType.LOGIN_FAILED]
        
        # Group by IP to see how many distinct users it tried
        ip_to_users = defaultdict(set)
        ip_to_logs = defaultdict(list)
        for log in failed_logins:
            if log.user_id:
                ip_to_users[log.source_ip].add(log.user_id)
                ip_to_logs[log.source_ip].append(log)
                
        for ip, users in ip_to_users.items():
            if len(users) >= self.user_threshold:
                alerts.append(Alert(
                    alert_id=str(uuid.uuid4()),
                    rule_name=self.rule_name,
                    severity=AlertSeverity.CRITICAL,
                    description=f"IP {ip} attempted logins on {len(users)} distinct user accounts.",
                    source_ip=ip,
                    triggering_logs=ip_to_logs[ip]
                ))
        return alerts

class ImpossibleTravelRule(BaseDetectionRule):
    rule_name = "Impossible Travel"
    
    def __init__(self, speed_threshold_kmh: int = 1000):
        self.speed_threshold_kmh = speed_threshold_kmh
        
    def evaluate(self, logs: List[LogEvent]) -> List[Alert]:
        alerts = []
        # In a real system, we'd query past successful logins from a DB.
        # Here we look within the batch for rapid geographic shifts.
        success_logins = [log for log in logs if log.event_type == LogEventType.LOGIN_SUCCESS]
        
        user_logins = defaultdict(list)
        for log in success_logins:
            if log.user_id and 'lat' in log.metadata and 'lon' in log.metadata:
                user_logins[log.user_id].append(log)
                
        for user, logins in user_logins.items():
            logins.sort(key=lambda x: x.timestamp)
            for i in range(1, len(logins)):
                loc1 = (logins[i-1].metadata['lat'], logins[i-1].metadata['lon'])
                loc2 = (logins[i].metadata['lat'], logins[i].metadata['lon'])
                
                distance_km = geodesic(loc1, loc2).kilometers
                time_diff_hours = (logins[i].timestamp - logins[i-1].timestamp).total_seconds() / 3600.0
                
                if time_diff_hours > 0:
                    speed = distance_km / time_diff_hours
                    if speed > self.speed_threshold_kmh:
                        alerts.append(Alert(
                            alert_id=str(uuid.uuid4()),
                            rule_name=self.rule_name,
                            severity=AlertSeverity.HIGH,
                            description=f"Impossible travel for {user}: {distance_km:.2f}km in {time_diff_hours:.2f} hours.",
                            user_id=user,
                            triggering_logs=[logins[i-1], logins[i]]
                        ))
        return alerts
        
class PortScanningRule(BaseDetectionRule):
    rule_name = "Port Scanning"
    
    def __init__(self, port_threshold: int = 20):
        self.port_threshold = port_threshold
        
    def evaluate(self, logs: List[LogEvent]) -> List[Alert]:
        alerts = []
        port_logs = [log for log in logs if log.event_type == LogEventType.PORT_CONNECTION]
        
        ip_to_ports = defaultdict(set)
        ip_to_logs = defaultdict(list)
        
        for log in port_logs:
            if log.target_resource:
                ip_to_ports[log.source_ip].add(log.target_resource)
                ip_to_logs[log.source_ip].append(log)
                
        for ip, ports in ip_to_ports.items():
            if len(ports) >= self.port_threshold:
                alerts.append(Alert(
                    alert_id=str(uuid.uuid4()),
                    rule_name=self.rule_name,
                    severity=AlertSeverity.MEDIUM,
                    description=f"IP {ip} scanned {len(ports)} distinct ports.",
                    source_ip=ip,
                    triggering_logs=ip_to_logs[ip]
                ))
        return alerts
