# 🧠 MycoCheck Backend

**MycoCheck Backend** is the server-side component of the MycoCheck system — a smart poultry health monitoring solution designed to detect **Mycoplasma infection symptoms** in chickens, notify farmers via SMS, and facilitate vet-farmer communication in real-time.

It is tightly integrated with a **locally running Flask API** that uses **YOLOv8m** for real-time detection via webcam, feeding snapshots into this backend for further processing and alerting.

---

## 🧩 Features

- ✅ REST API to receive and manage detected snapshots
- 🔁 Real-time vet-farmer chat via **SignalR**
- 📩 **SMS Alerts** using IPROGTECH SMS API
- 🧠 Integration with **YOLOv8m real-time detection (Flask)**
- 📷 Ingests base64-encoded images + accuracy
- 🛡️ Ngrok-friendly + CORS for local + remote access

---

## 🔁 Flask + YOLOv8m Integration

This backend connects to a **locally run Flask API** responsible for monitoring the webcam and detecting chickens with visible symptoms using a trained **YOLOv8m** object detection model.

When the model detects symptoms:
1. A snapshot is captured.
2. Its confidence score is computed.
3. The image is base64 encoded.
4. The snapshot and score are passed and stored on the backend which in turn gets fetched by the frontend for interface.
