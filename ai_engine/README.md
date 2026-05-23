# 🛡️ ThreatPilot — AI Engine Microservice

Welcome to the **AI-Powered SOC Assistant** core module. ThreatPilot employs a robust **"Rules First, AI Second"** architectural paradigm to detect cyber threats deterministically, and utilizes advanced Large Language Models (LLMs) to analyze, summarize, and mitigate those threats.

---

## 🚀 The Role of AI in ThreatPilot

Our philosophy is simple: **Security detection must be deterministic. AI is for context.**

1. **Deterministic Detection (Rules First)**: The engine ingests normalized logs and runs them against strict detection algorithms (e.g., Brute Force, Credential Stuffing, Impossible Travel, Port Scanning).
2. **Contextual Analysis (AI Second)**: Once a threat is confirmed, the AI model (powered by Groq / LLaMA 3) takes over to perform:
   - **Incident Summarization**: Translating raw JSON logs into a human-readable executive summary.
   - **Severity Interpretation**: Explaining *why* a particular alert holds its severity (e.g., Target is an admin account).
   - **Actionable Remediation**: Recommending exact steps for the SOC Analyst to stop the attack (e.g., blocking IPs, resetting passwords).

---

## 🛠️ Technology Stack
- **Framework**: Python FastAPI
- **Validation**: Pydantic
- **AI Integration**: Groq API (`llama-3.3-70b-versatile`) via `httpx`
- **Security**: JWT Authentication, SlowAPI (Rate Limiting)
- **Environment**: `.env` (python-dotenv)

---

## ⚙️ How to Setup & Run

Follow these steps to run the AI Microservice locally.

### 1. Environment Setup
Create a Python Virtual Environment and install the dependencies:
```powershell
cd f:\ThreatPilot\ai_engine
python -m venv venv
.\venv\Scripts\activate
pip install -r requirements.txt
```

### 2. Configure API Keys
You need a FREE API key from Groq to run the AI engine.
1. Go to [console.groq.com/keys](https://console.groq.com/keys) and generate a new key.
2. Create a file named `.env` in the `ai_engine` folder.
3. Copy the contents of `.env.example` into `.env` and paste your key:
```env
JWT_SECRET_KEY=your_super_secret_jwt_key_here
GROQ_API_KEY=gsk_your-actual-api-key-here
GROQ_API_URL=https://api.groq.com/openai/v1/chat/completions
GROQ_MODEL=llama-3.3-70b-versatile
```

### 3. Start the Server
Run the FastAPI application using Uvicorn:
```powershell
uvicorn main:app --reload
```
The server will start at `http://127.0.0.1:8000`.

---

## 🧪 Testing the Engine

We have built a comprehensive, fully automated test suite that simulates real-world cyber attacks against the engine.

With the server running, open a **new terminal** and execute:
```powershell
cd f:\ThreatPilot\ai_engine
.\venv\Scripts\activate
python test_scripts/run_all_tests.py
```

### What does the test suite do?
It simulates the following attacks, triggers the deterministic rules, and calls the Groq AI to generate live incident reports:
1. **Brute Force Attack** (15 failed SSH logins)
2. **Credential Stuffing** (1 IP targeting 8 different accounts)
3. **Impossible Travel** (Karachi to London in 4 minutes)
4. **Port Scanning / Recon** (25 ports probed in 5 seconds)

---

## 📚 API Documentation (Swagger UI)
FastAPI automatically generates interactive API documentation.
Once the server is running, visit:
👉 **[http://127.0.0.1:8000/docs](http://127.0.0.1:8000/docs)**

From here, you can manually test the `/ingest` and `/analyze-incident` endpoints.

---
*Developed for ThreatPilot — Intelligent Threat Detection and Incident Analysis.*
