#include <SPI.h>
#include <LoRa.h>
#include <WiFi.h>
#include <PubSubClient.h>
#include <HTTPClient.h>
#include <WiFiClientSecure.h>

#define SS 5
#define RST 14
#define DIO0 26

// WiFi credentials
const char* WIFI_SSID = "h4prog";
const char* WIFI_PASS = "1234567890";

// API endpoint
const char* url = "https://webappnamename-dkfxb8g3eubvhabr.swedencentral-01.azurewebsites.net/api/sensor";

// Azure IoT Hub
const char* HUB    = "GreenHouseIotHub.azure-devices.net";
const char* DEVICE = "WifiArduino";
const char* USER   = "GreenHouseIotHub.azure-devices.net/WifiArduino/?api-version=2021-04-12";
const char* PASS   = "SharedAccessSignature ...";

WiFiClientSecure net;
PubSubClient mqtt(net);

void connectWiFi() {
  Serial.println("Connecting to WiFi...");
  WiFi.begin(WIFI_SSID, WIFI_PASS);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("WiFi connected!");
}


void connectAzure() {
  net.setInsecure();
  mqtt.setServer(HUB, 8883);
  mqtt.setCallback(onMessage);
  while (!mqtt.connected()) {
    mqtt.connect(DEVICE, USER, PASS);
  }
  mqtt.subscribe("devices/WifiArduino/messages/devicebound/#");
  Serial.println("Connected to Azure IoT Hub!");
}


void onMessage(char* topic, byte* payload, unsigned int len) {
  String msg;
  while (len--) msg += (char)*payload++;
  Serial.print("Azure -> LoRa: ");
  Serial.println(msg);

  // Forward message to Sensor Node
  LoRa.beginPacket();
  LoRa.print(msg);
  LoRa.endPacket();
}
void postToAPI(String payload) {
  if (WiFi.status() != WL_CONNECTED) return;

  WiFiClientSecure client;
  client.setInsecure();
  HTTPClient http;

  if (!http.begin(client, url)) return;
  http.addHeader("Content-Type", "application/json");
  int code = http.POST(payload);
  Serial.printf("POST code: %d\n", code);
  if (code > 0) Serial.println(http.getString());
  http.end();
}

void setup() {
 Serial.begin(9600);

  // Setup LoRa
  LoRa.setPins(SS, RST, DIO0);
  if (!LoRa.begin(433E6)) {
    Serial.println("Starting LoRa failed!");
    while (1);
  }
  Serial.println("Gateway node ready");

  connectWiFi();
  connectAzure();
}

void loop() {
  // put your main code here, to run repeatedly:
  int packetSize = LoRa.parsePacket();
  if (packetSize) {
    String incoming = "";
    while (LoRa.available()) {
      incoming += (char)LoRa.read();
    }
    Serial.print("LoRa -> API: ");
    Serial.println(incoming);

    // Forward sensor data to API
    postToAPI(incoming);
  }

  // --- KEEP MQTT LOOP ALIVE ---
  mqtt.loop();
}
