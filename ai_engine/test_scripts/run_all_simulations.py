import asyncio
import httpx

API_BASE_URL = "http://localhost:8000/api/v1"

async def run_all_simulations():
    print("Starting comprehensive attack simulation...")
    
    import test_e2e
    import test_credential_stuffing
    import test_impossible_travel
    import test_port_scanning

    print("\n--- 1. Simulating Brute Force Attack ---")
    await test_e2e.run_e2e_test()

    print("\n--- 2. Simulating Credential Stuffing ---")
    await test_credential_stuffing.run_credential_stuffing_test()

    print("\n--- 3. Simulating Impossible Travel ---")
    await test_impossible_travel.run_impossible_travel_test()

    print("\n--- 4. Simulating Port Scanning ---")
    await test_port_scanning.run_port_scanning_test()
    
    print("\nAll simulations completed! Check the WPF Dashboard.")

if __name__ == "__main__":
    asyncio.run(run_all_simulations())
