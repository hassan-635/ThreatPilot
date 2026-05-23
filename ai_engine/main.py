from fastapi import FastAPI, Depends, Request
from fastapi.middleware.cors import CORSMiddleware
from slowapi import Limiter, _rate_limit_exceeded_handler
from slowapi.util import get_remote_address
from slowapi.errors import RateLimitExceeded
from dotenv import load_dotenv
import os

# Load environment variables
load_dotenv()

# Initialize Rate Limiter
limiter = Limiter(key_func=get_remote_address)

app = FastAPI(
    title="ThreatPilot AI Engine",
    description="AI-Powered SOC Assistant for Intelligent Threat Detection",
    version="1.0.0"
)

# Add Rate Limiter Exception Handler
app.state.limiter = limiter
app.add_exception_handler(RateLimitExceeded, _rate_limit_exceeded_handler)

# CORS Configuration
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"], # In production, restrict this to the ASP.NET backend IP
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.get("/health", tags=["Health"])
@limiter.limit("10/minute")
async def health_check(request: Request):
    return {"status": "healthy", "service": "ThreatPilot AI Engine"}

from routes.api import router as api_router

# Include API routes
app.include_router(api_router, prefix="/api/v1")
