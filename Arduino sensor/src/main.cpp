#include <SPI.h>
#include <LoRa.h>
#include <Wire.h>
#include <Adafruit_BME280.h>
#include "Adafruit_TSL2591.h"
#include "Adafruit_CCS811.h"

// LoRa pins
#define SS 10
#define RST 9
#define DIO0 2

// Sensor pins
#define ledPin 2
#define soilPin 34
#define waterPin 35

Adafruit_TSL2591 tsl = Adafruit_TSL2591(2591);
Adafruit_BME280 bme;
Adafruit_CCS811 ccs;

int lux;
int temp;
int tryk;
int fugtighed;
int co2;
int vandmaengde;
int jordfugtighed;

unsigned long lastSend = 0;
const unsigned long sendInterval = 60000; // 1 minute
void startCSS() {
  Serial.println("CCS811 init...");
  if (!ccs.begin()) {
    Serial.println("Failed to start CCS811! Check wiring.");
    while (1);
  }
  while (!ccs.available());
}


void startBME() {
  if (!bme.begin()) {
    Serial.println("BME280 not found!");
    while (1);
  }
  Serial.println("BME280 ready");
}

void configureTSL2591() {
  tsl.setGain(TSL2591_GAIN_MED);              // 25x gain
  tsl.setTiming(TSL2591_INTEGRATIONTIME_300MS);
}


void startTSL2591() {
  if (tsl.begin()) {
    Serial.println("TSL2591 ready");
    configureTSL2591();
  } else {
    Serial.println("No TSL2591 found");
    while (1);
  }
}

int readBelysning() {
  uint32_t lum = tsl.getFullLuminosity();
  uint16_t ir = lum >> 16;
  uint16_t full = lum & 0xFFFF;
  lux = tsl.calculateLux(full, ir);
  Serial.println(lux);
  return lux;
}

int readJordFugtighed() {
  int sensorValue = analogRead(soilPin);
  jordfugtighed = map(sensorValue, 0, 4095, 255, 0);
  analogWrite(ledPin, jordfugtighed);
  Serial.println(jordfugtighed);
  return jordfugtighed;
}

int readTemp() 
{
  Serial.println(bme.readTemperature());
  return bme.readTemperature(); 
}
int readTryk() 
{ 
  Serial.println(bme.readPressure() / 100.0F);
  return bme.readPressure() / 100.0F; 
}
int readFugtighed() 
{ 
  Serial.println(bme.readHumidity());
  return bme.readHumidity(); 
}

int readCo2() {
  if (ccs.available() && !ccs.readData()) 
  {
    co2 = ccs.geteCO2();
  }
  Serial.println(co2);
  return co2;
}

int readVandSensor() {
  Serial.println(vandmaengde);
  vandmaengde = analogRead(waterPin);
  return vandmaengde;
}

void setup() {
  Serial.begin(9600);

  // Setup LoRa
  LoRa.setPins(SS, RST, DIO0);
  if (!LoRa.begin(433E6)) {
    Serial.println("Starting LoRa failed!");
    while (1);
  }
    Serial.println("LoRa ready"); // doesthis chck if lora is actually correctly setup and actauly would work 

  // setup all sensors 

  pinMode(ledPin, OUTPUT);
  digitalWrite(ledPin, LOW);
  startTSL2591();
  startBME();
  startCSS();

}

void loop() {
  // --- ALWAYS LISTEN ---
  int packetSize = LoRa.parsePacket();
  if (packetSize) {
    String msg = "";
    while (LoRa.available()) {
      msg += (char)LoRa.read();
    }
    Serial.print("Received command: ");
    Serial.println(msg);

    // Example: simple LED command, will be event for green house handling 
    if (msg == "LEDON") digitalWrite(ledPin, HIGH);
    if (msg == "LEDOFF") digitalWrite(ledPin, LOW);
  }

  // --- EVERY MINUTE: READ & SEND ---
  unsigned long now = millis();
  if (now - lastSend >= sendInterval) {
    lastSend = now;

 // --- PERIODIC SENSOR READ & SEND ---
  unsigned long now = millis();
  if (now - lastSend >= sendInterval) {
    lastSend = now;

    temp          = readTemp();
    tryk          = readTryk();
    fugtighed     = readFugtighed();
    co2           = readCo2();
    lux           = readBelysning();
    jordfugtighed = readJordFugtighed();
    vandmaengde   = readVandSensor();

    String payload = "{";
    payload += "\"temp\":" + String(temp) + ",";
    payload += "\"pressure\":" + String(tryk) + ",";
    payload += "\"humidity\":" + String(fugtighed) + ",";
    payload += "\"co2\":" + String(co2) + ",";
    payload += "\"lux\":" + String(lux) + ",";
    payload += "\"soil\":" + String(jordfugtighed) + ",";
    payload += "\"water\":" + String(vandmaengde);
    payload += "}";

    LoRa.beginPacket();
    LoRa.print(payload);
    LoRa.endPacket();

    Serial.print("Sent: ");
    Serial.println(payload);
  }
}