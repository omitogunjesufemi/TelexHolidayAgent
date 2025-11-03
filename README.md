# 📅 International Holiday Notifier Agent (A2A)

This agent solves a common challenge for distributed teams: **easily identifying and being notified of upcoming public holidays across different regions**.
It is built with **C#** using the **ASP.NET Core** framework and follows the **Telex Agent-to-Agent (A2A)** protocol.

---

## 🚀 Live Agent Deployment

**Agent Base URL:**
[https://telexholidayagent.up.railway.app/holiday](https://telexholidayagent.up.railway.app/holiday)

**Agent Card Endpoint:**
[https://telexholidayagent.up.railway.app/.well-known/agent.json](https://telexholidayagent.up.railway.app/.well-known/agent.json)

---

## ✨ Features

* **On-Demand Holiday Lookup**: Quickly find the next upcoming public holidays for any supported country code.
* **Caching**: Implements an in-memory `HolidayCache` to store yearly holiday data, reducing external API calls and improving response speed for repeat queries.
* **Robust API Integration**: Uses a dedicated `ApiClient` service to reliably fetch data from the [Nager.Date Public Holiday API](https://date.nager.at).
* **A2A Protocol Compliant**: Processes incoming `JSON-RPC` `message/send` requests and returns structured `A2AResponse` objects.

---

## 💻 Usage

The agent supports the `/holiday next` command followed by a **two-letter ISO country code** (e.g., `NG`, `US`, `DE`).

### **Command Syntax**

| Command              | Description                                                              | Example            |
| -------------------- | ------------------------------------------------------------------------ | ------------------ |
| `/holiday next [CC]` | Finds and lists the next public holidays for the specified country code. | `/holiday next NG` |

---

### **Example Interaction (Nigeria - NG)**

#### **Input Message (JSON-RPC Request)**

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "message/send",
  "params": {
    "message": {
      "kind": "message",
      "role": "user",
      "parts": [
        {
          "kind": "text",
          "text": "/holiday next NG"
        }
      ],
      "messageId": "9229e770-767c-417b-a0b0-f0741243c589"
    },
    "metadata": {}
  }
}
```

#### **Agent Response (JSON)**
```json
{
    "jsonrpc": "2.0",
    "id": 1,
    "result": {
        "kind": "message",
        "role": "agent",
        "parts": [
            {
                "kind": "text",
                "text": "Next Holidays in NG\r\nHoliday: Christmas Day, Date: 2025-12-25\nHoliday: Boxing Day, Date: 2025-12-26\nHoliday: New Year's Day, Date: 2026-01-01\nHoliday: Good Friday, Date: 2026-04-03\nHoliday: Easter Monday, Date: 2026-04-06\nHoliday: Workers' Day, Date: 2026-05-01\nHoliday: Children's Day, Date: 2026-05-27\nHoliday: Democracy Day, Date: 2026-06-12\nHoliday: National Day, Date: 2026-10-01\nHoliday: National Youth Day, Date: 2026-11-01\n"
            }
        ],
        "messageId": "1361ae7d-84a2-4621-971e-bef011476918",
        "contextId": ""
    }
}
```

#### **Agent Response (Simplified Result Text)**

```
Next Holidays in NG
Holiday: Christmas Day, Date: 2025-12-25
Holiday: Boxing Day, Date: 2025-12-26
Holiday: New Year's Day, Date: 2026-01-01
... (and subsequent holidays)
```

---

## 🛠️ Architecture and Stack

The project follows a **clean, maintainable C#/.NET architecture**:

| Component                | Description                                                  |
| ------------------------ | ------------------------------------------------------------ |
| **Platform**             | C# / .NET 8 (ASP.NET Core Minimal APIs)                      |
| **Protocol**             | Telex Agent-to-Agent (A2A)                                   |
| **External Data Source** | [Nager.Date API](https://date.nager.at) (Public Holiday API) |
| **Dependencies**         | `Microsoft.Extensions.Http`, `Newtonsoft.Json`, `A2A`, `A2A.AspNetCore`               |

---

### **Key Components**

* **`AgentAPI.Models`**
  Contains `PublicHoliday`, `CountriesAvailable`, and the singleton `HolidayCache`.

* **`AgentAPI.Services.ApiClient`**
  Handles all asynchronous HTTP requests to the Nager.Date API, abstracting data retrieval from the agent’s logic.

* **`AgentAPI.Services.HolidayAgent`**
  Implements core logic for message parsing, regex matching, cache lookups, and invoking the `ApiClient`.

---

## ⏭️ Future Enhancements (Clock System)

The architecture has been prepared to support **proactive holiday notifications** via the **Clock System** in a future update.

### Planned Features

* **Subscription Logic**
  Implement `/holiday add [CC]` to persist user subscriptions (e.g., in Firestore or memory store).

* **Scheduled Posting**
  Integrate with `taskManager.OnClock` to run a weekly routine that:

  * Checks cached data for all subscribed countries
  * Posts a summary of upcoming holidays to relevant channels automatically

---

### 🧩 Summary

The **International Holiday Notifier Agent** combines **A2A protocol integration**, **external API data**, and **intelligent caching** to create a reliable, extensible solution for distributed teams that need visibility into global holidays.



